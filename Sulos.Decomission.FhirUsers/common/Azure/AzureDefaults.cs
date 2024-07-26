namespace Sulos.Hospice.Care.Core.Common.Azure;

public static class AzureDefaults
{
    public static string GetOauthTokenUrl(string tenantId)
    {
        return $"https://login.microsoftonline.com/{tenantId}/oauth2/token";
    }
}