using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Sulos.Decomission.B2CUsers.Options;
using Sulos.Decomission.B2CUsers.Services.Interfaces;

namespace Sulos.Decomission.B2CUsers.Services.Factory;


public class ApiSulosGraphServiceClientFactory : ISulosGraphServiceClientFactory
{
    private readonly GraphServiceOptions _clientOptions;
    private readonly ILoggerFactory _loggerFactory;

    public ApiSulosGraphServiceClientFactory(IOptions<GraphServiceOptions> clientOptionsFactory,
        ILoggerFactory loggerFactory)
    {
        _clientOptions = clientOptionsFactory.Value;
        _loggerFactory = loggerFactory;
    }

    public ISulosGraphServiceClient CreateGraphServiceClientAsync()
    {
        var logger = _loggerFactory.CreateLogger<SulosGraphServiceClient>();
        return new SulosGraphServiceClient(
            new GraphServiceClient(
                new ClientSecretCredential(_clientOptions.TenantId, _clientOptions.ClientId, _clientOptions.ClientSecret)
            ),
            _clientOptions.ExtensionApplicationId,
            logger);
    }
}
