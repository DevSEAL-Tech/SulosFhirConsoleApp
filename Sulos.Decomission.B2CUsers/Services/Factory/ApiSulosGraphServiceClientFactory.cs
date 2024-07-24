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
    private readonly ILogger<SulosGraphServiceClient> _logger;

    public ApiSulosGraphServiceClientFactory(IOptions<GraphServiceOptions> clientOptionsFactory,
        ILogger<SulosGraphServiceClient> logger)
    {
        _clientOptions = clientOptionsFactory.Value;
        _logger = logger;
    }

    public ISulosGraphServiceClient CreateGraphServiceClientAsync()
    {
        return new SulosGraphServiceClient(
            new GraphServiceClient(
                new ClientSecretCredential(_clientOptions.TenantId, _clientOptions.ClientId, _clientOptions.ClientSecret)
            ),
            _clientOptions.OrganizationId,
            _clientOptions.ExtensionApplicationId,
            _logger);
    }
}
