using System.Text.Json.Serialization;

namespace Sulos.Hospice.Care.Core.Common.Azure;

public record AzureAccessToken(string AccessToken, int ExpiresInSeconds)
{
    [JsonPropertyName("access_token")] public string AccessToken { get; } = AccessToken;
    [JsonPropertyName("expires_in")] public int ExpiresInSeconds { get; } = ExpiresInSeconds;
}