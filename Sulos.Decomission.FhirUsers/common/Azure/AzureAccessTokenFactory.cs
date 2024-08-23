using System.Net.Http;
using System.Net.Http.Json;

namespace Sulos.Hospice.Care.Core.Common.Azure;

public interface IAzureAccessTokenFactory
{
    Task<string> GetAccessTokenAsync(ClientCredentialsOptions options);
}

public class AzureAccessTokenFactory : IAzureAccessTokenFactory
{

    private readonly IHttpClientFactory _httpClientFactory;
    
    public AzureAccessTokenFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
      
    }

    public async Task<string> GetAccessTokenAsync(ClientCredentialsOptions options)
    {

        var token = await GetTokenFromAzureAsync(options);
        return token!.AccessToken;
    }

    private async Task<AzureAccessToken?> GetTokenFromAzureAsync(ClientCredentialsOptions options)
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, AzureDefaults.GetOauthTokenUrl(options.TenantId))
        {
            Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("resource", options.Resource),
                new KeyValuePair<string, string>("client_id", options.ClientId),
                new KeyValuePair<string, string>("client_secret", options.ClientSecret)
            })
        };
        var response = await client.SendAsync(request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<AzureAccessToken>();
        if (token == null)
            throw new InvalidOperationException("Access token was returned as null");
        return token;
    }

}