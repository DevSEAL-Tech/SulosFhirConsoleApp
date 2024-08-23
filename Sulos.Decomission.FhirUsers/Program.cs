using Azure.Identity;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sulos.Api.Common.Fhir;
using Sulos.Decomission.FhirUsers.Services.Interfaces;
using Sulos.Hospice.Care.Core.Common.Fhir;
using Sulos.Hospice.Care.Core.Common.Fhir.Http;
using Task = System.Threading.Tasks.Task;
using Sulos.Decomission.B2CUsers.Options;
using Sulos.Hospice.Care.Models.Common;
using Sulos.Hospice.Care.Models.Users;
using Sulos.Hospice.Care.Core.Common.Azure;
using Sulos.Decomission.Shared.Extensions;
using Sulos.Decomission.FhirUsers.Extensions;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddOptions<FhirConfigOptions>()
    .Bind(builder.Configuration.GetSection("Fhir"))
    .ValidateOnStart();
builder.Services.AddOptions<RunParameterOptions>()
    .Bind(builder.Configuration.GetSection("Parameters"))
    .ValidateOnStart();

builder.Services.AddAzureClients(builder =>
{
       
    builder.UseCredential(new DefaultAzureCredential());
});
builder.Services.AddSingleton<ISulosFhirClientFactory, ApiSulosFhirClientFactory>();
builder.Services.TryAddTransient<ISulosGetPatients, SulosGetPatients>();
builder.Services.TryAddTransient<ISulosGetPractitioners, SulosGetPractitioners>();
builder.Services.TryAddTransient<IFhirClientOptionsFactory, FhirClientOptionsFactory>();
builder.Services.TryAddTransient<IAzureAccessTokenFactory, AzureAccessTokenFactory>();
builder.Services.TryAddTransient<IFhirHttpMessageHandlerFactory, FhirHttpMessageHandlerFactory>();
builder.Services.AddHttpClient();

using (IHost host = builder.Build())
{
    using IServiceScope serviceScope = host.Services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    var runParametersOptions = provider.GetRequiredService<IOptions<RunParameterOptions>>();

    var organization = runParametersOptions.Value.Organization;

    var patients = await GetPatientAsync(provider);
    var practitioners = await GetPractitionersAsync(provider);

    var everyone = patients.Select(pat => pat.ToSulosPersonModel()).Concat(practitioners.Select(pra => pra.ToSulosPersonModel()));

    var exportableUsers = everyone.Select(el => el.ToExportObject());

    var serializedUsers = UserExportExtensions.SerializeCollection(exportableUsers);

    await File.WriteAllTextAsync($"fhir-{organization}-user.json", serializedUsers);

    await DeleteUsers(provider, everyone);
    await host.RunAsync();
}

static async Task<Patient[]> GetPatientAsync(IServiceProvider provider)
{
    var runParametersOptions = provider.GetRequiredService<IOptions<RunParameterOptions>>();
    var organization = runParametersOptions.Value.Organization;

    var sulosclientFactory = provider.GetRequiredService<ISulosFhirClientFactory>();
    var sulospatientService = provider.GetRequiredService<ISulosGetPatients>();
    var sulosClient = await sulosclientFactory.CreateReaderAsync(organization);

    var patiensList = sulospatientService.GetPatients(sulosClient);
    return await patiensList;
}


static async Task<Practitioner[]> GetPractitionersAsync(IServiceProvider provider)
{
    var runParametersOptions = provider.GetRequiredService<IOptions<RunParameterOptions>>();
    var organization = runParametersOptions.Value.Organization;

    var sulosclientFactory = provider.GetRequiredService<ISulosFhirClientFactory>();
    var sulospatientService = provider.GetRequiredService<ISulosGetPractitioners>();
    var sulosClient = await sulosclientFactory.CreateReaderAsync(organization);

    var patiensList = sulospatientService.GetPractitioners(sulosClient);
    return await patiensList;
}

static async Task DeleteUsers(IServiceProvider hostProvider, IEnumerable<SulosPersonModel> sulosPersons)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var sulosClientFactory = provider.GetRequiredService<ISulosFhirClientFactory>();
    var sulosClient = await sulosClientFactory.CreateWriterAsync("");

    foreach (var person in sulosPersons)
    {
        if (person is {PersonType: PersonType.InternalUser })
        {
            var existingPerson = await sulosClient.EnsureEntityExists<Practitioner>(person.Id).ConfigureAwait(false);
            Console.WriteLine($"Deleting {existingPerson.GetFirstName()} {existingPerson.GetLastName()}");
            // sulosClient.DeleteAsync(existingPerson).ConfigureAwait(false);
        }else if (person is {PersonType: PersonType.Patient })
        {
            var existingPerson = await sulosClient.EnsureEntityExists<Patient>(person.Id).ConfigureAwait(false);
            Console.WriteLine($"Deleting {existingPerson.GetFirstName()} {existingPerson.GetLastName()}");
            //await sulosClient.DeleteAsync(existingPerson).ConfigureAwait(false);
        }
    }
}