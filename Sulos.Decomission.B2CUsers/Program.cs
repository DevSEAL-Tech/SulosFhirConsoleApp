using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models.ExternalConnectors;
using Sulos.Decomission.B2CUsers.Options;
using Sulos.Decomission.B2CUsers.Services.Factory;
using Sulos.Decomission.B2CUsers.Services.Interfaces;


Console.WriteLine("Hello, World!");

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole();

builder.Services.Configure<GraphServiceOptions>(builder.Configuration.GetSection("GraphService"));


builder.Services.AddSingleton<ISulosGraphServiceClientFactory, ApiSulosGraphServiceClientFactory>();


using IHost host = builder.Build();

ProviderMethod(host.Services);


await host.RunAsync();


static void ProviderMethod(IServiceProvider hostProvider)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var sulosGraphClientFactory = provider.GetRequiredService<ISulosGraphServiceClientFactory>();
    var sulosGraphClient = sulosGraphClientFactory.CreateGraphServiceClientAsync();
    var allUsers = sulosGraphClient.GetAllUsersInCurrentOrganisation();
}
    
