using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph.Models;
using Sulos.Decomission.B2CUsers.Options;
using Sulos.Decomission.B2CUsers.Services.Extensions;
using Sulos.Decomission.B2CUsers.Services.Factory;
using Sulos.Decomission.B2CUsers.Services.Interfaces;
using System.Text.Json;

Console.WriteLine("Hello, World!");

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddOptions<GraphServiceOptions>()
    .Bind(builder.Configuration.GetSection("GraphService"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<RunParameterOptions>()
    .Bind(builder.Configuration.GetSection("Parameters"))
    .ValidateDataAnnotations()
    .ValidateOnStart();


builder.Services.AddSingleton<ISulosGraphServiceClientFactory, ApiSulosGraphServiceClientFactory>();

using (IHost host = builder.Build())
{
    using IServiceScope serviceScope = host.Services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    var runParametersOptions = provider.GetRequiredService<IOptions<RunParameterOptions>>();

    var organization = runParametersOptions.Value.Organization;

    var users = await GetUsersInOrganizationAsync(host.Services, organization);
    await ExportUsers(host.Services, users,organization);
    await DeleteUsers(host.Services, users);
    await host.RunAsync();
}


static async Task<IEnumerable<User>> GetUsersInOrganizationAsync(IServiceProvider hostProvider, string organizationId)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var sulosGraphClientFactory = provider.GetRequiredService<ISulosGraphServiceClientFactory>();
    var sulosGraphClient = sulosGraphClientFactory.CreateGraphServiceClientAsync();
    return await sulosGraphClient.GetAllUsersInCurrentOrganisation(organizationId);
}

static async Task ExportUsers(IServiceProvider hostProvider, IEnumerable<User> users, string organizationId)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var graphOptions = provider.GetRequiredService<IOptions<GraphServiceOptions>>();
    var graphServiceExtensions = new GraphServiceExtensions(graphOptions.Value.ExtensionApplicationId);

    var exportableUsers = users.Select(el => el.ToExportObject(graphServiceExtensions));

    var serializedUsers = JsonSerializer.Serialize(exportableUsers);

    await File.WriteAllTextAsync($"b2c-{organizationId}-user.json", serializedUsers);
}



static async Task DeleteUsers(IServiceProvider hostProvider, IEnumerable<User> users)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var sulosGraphClientFactory = provider.GetRequiredService<ISulosGraphServiceClientFactory>();
    var sulosGraphClient = sulosGraphClientFactory.CreateGraphServiceClientAsync();

    var graphOptions = provider.GetRequiredService<IOptions<GraphServiceOptions>>();

    var graphServiceExtensions = new GraphServiceExtensions(graphOptions.Value.ExtensionApplicationId);
    var fhirIdExtension = graphServiceExtensions[GraphServiceExtensionAttributes.FhirID];

    var cancelTokenSource = new CancellationTokenSource();
    foreach (var user in users)
    {
        Console.WriteLine($"Deleting {user.DisplayName} ({user.AdditionalData[fhirIdExtension]})");
        await sulosGraphClient.DeleteUserByUserId(user.Id!, cancelTokenSource.Token);
    }
}
    
