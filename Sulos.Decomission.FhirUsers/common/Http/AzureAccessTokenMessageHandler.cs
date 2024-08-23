using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;
using Sulos.Hospice.Care.Core.Common.Azure;

namespace Sulos.Hospice.Care.Core.Common.Http;

public class AzureAccessTokenMessageHandler : DelegatingHandler
{
    private readonly IAzureAccessTokenFactory _azureAccessTokenFactory;
    private readonly IHostEnvironment _env;

    private ClientCredentialsOptions ClientCredentials { get; }
    private bool IsDevelopment => _env.IsDevelopment();

    public AzureAccessTokenMessageHandler(
        HttpMessageHandler inner,
        IAzureAccessTokenFactory azureAccessTokenFactory,
        IHostEnvironment env,
        ClientCredentialsOptions clientCredentials) 
        : base(inner)
    {
        ClientCredentials = clientCredentials;
        _azureAccessTokenFactory = azureAccessTokenFactory;
        _env = env;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization != null || IsDevelopment)
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        var accessToken = await _azureAccessTokenFactory.GetAccessTokenAsync(ClientCredentials).ConfigureAwait(false);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}