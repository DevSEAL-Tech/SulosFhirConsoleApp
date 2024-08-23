using Azure.Core.Pipeline;
using Azure.Core;
using Azure.Identity;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;
using Sulos.Api.Common.Fhir;
using Sulos.Decomission.FhirUsers.Services.Interfaces;
using Sulos.Hospice.Care.Core.Common.Azure;
using Sulos.Hospice.Care.Core.Common.Fhir;
using Sulos.Hospice.Care.Core.Common.Fhir.Http;
using Sulos.Hospice.Care.Core.Common.Fhir.Searching;
using Group = Hl7.Fhir.Model.Group;
using Bundle = Hl7.Fhir.Model.Bundle;
using Hl7.Fhir.Rest;
using Sulos.Hospice.Care.Models.Patients;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;
using MediatR;
using Sulos.Decomission.B2CUsers.Options;

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
    builder.Services.TryAddTransient<IFhirClientOptionsFactory, FhirClientOptionsFactory>();
    builder.Services.TryAddTransient<IFhirHttpMessageHandlerFactory, FhirHttpMessageHandlerFactory>();
    builder.Services.TryAddTransient<IAzureAccessTokenFactory, AzureAccessTokenFactory>();
    builder.Services.AddHttpClient();

    static void ConfigureKeyVaultClientOptions<TOptions>(TOptions options, IServiceProvider provider)
            where TOptions : ClientOptions
    {
        var handler = new HttpClientHandler();
    #pragma warning disable S4830
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
    #pragma warning restore S4830
        options.Transport = new HttpClientTransport(handler);
    }


    using (IHost host = builder.Build())
    {
        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;
        var runParametersOptions = provider.GetRequiredService<IOptions<RunParameterOptions>>();

        var organization = runParametersOptions.Value.Organization;

        var patients = await GetPatientWithPractitionersAsync(host.Services);
        var transactionBuilder = await BuildGroupMembersQueryTransactionBundle(host.Services, patients);
        var groupMembersQueryResponses = await GetPaginatedQueryResponses(host.Services, transactionBuilder.ToBundle());
        var response = BuildPatientsWithPractitioners(patients, groupMembersQueryResponses);
        var serializedUsers = JsonSerializer.Serialize(response);
        await File.WriteAllTextAsync($"fhir-{organization}-user.json", serializedUsers);

        await DeleteUsers(host.Services, response);
        await host.RunAsync();
    }

static async Task<Patient[]> GetPatientWithPractitionersAsync(IServiceProvider hostProvider)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var sulosclientFactory = provider.GetRequiredService<ISulosFhirClientFactory>();
    var sulospatientService = provider.GetRequiredService<ISulosGetPatients>();
    var sulosClient = await sulosclientFactory.CreateReaderAsync();

    var patiensList = sulospatientService.GetPatients(sulosClient);
    return await patiensList;
}

async Task<SulosTransactionBuilder> BuildGroupMembersQueryTransactionBundle(IServiceProvider hostProvider, Patient[] patients)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var sulosclientFactory = provider.GetRequiredService<ISulosFhirClientFactory>();
    var transactionBuilder = await sulosclientFactory.CreateTransactionBuilder().ConfigureAwait(false);

    foreach (var patient in patients)
    {
        transactionBuilder
            .Search<Group>(
                new FhirSearchParamsBuilder()
                    .Where("type", "practitioner")
                    .WhereGroupMember<Patient>(patient.Id)
                    .Include("Group:member")
                    .Build()
            );
    }

    return transactionBuilder;
}

static async Task<Bundle[]> GetPaginatedQueryResponses(IServiceProvider hostProvider, Bundle query)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    var sulosClientFactory = provider.GetRequiredService<ISulosFhirClientFactory>();
    var sulosClient = await sulosClientFactory.CreateReaderAsync();
    var groupMembersQueryResponses = await sulosClient.GetPaginatedQueryResponses(query);
    return groupMembersQueryResponses;
}

static SulosPatientModelV2[] BuildPatientsWithPractitioners(IEnumerable<Patient> patients, IEnumerable<Bundle> groupMembersQueryResponses)
{
    return groupMembersQueryResponses
        .SelectMany(b => b.GetResources())
        .Select(r => (Bundle)r)
        .Zip(patients)
        .Select(BuildPatientWithPractitioners)
        .ToArray();
}


static SulosPatientModelV2 BuildPatientWithPractitioners((Bundle, Patient) patientData)
{
    var (groupMembersBundle, patient) = patientData;

    return patient.ToSulosPatientModel(practitioners: groupMembersBundle.GetResourcesOfType<Practitioner>().ToArray()
    );
}

static async Task DeleteUsers(IServiceProvider hostProvider, IEnumerable<SulosPatientModelV2> patients)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var sulosClientFactory = provider.GetRequiredService<ISulosFhirClientFactory>();
    var sulosClient = await sulosClientFactory.CreateWriterAsync();

    int taiwo = 0;
    foreach (var patient in patients)
    {
        if (taiwo == -1)
        {
            var existingPatient = await sulosClient.EnsureEntityExists<Patient>(patient.Id).ConfigureAwait(false);
            Console.WriteLine($"Deleting {existingPatient.Name}");
            await sulosClient.DeleteAsync(location: $"Patient/{patient.Id}").ConfigureAwait(false);
        }
        taiwo++;
        //await sulosGraphClient.DeleteUserByUserId(user.Id!, cancelTokenSource.Token);
    }
}