namespace Sulos.Decomission.B2CUsers.Options;

public record GraphServiceOptions(string ClientId,
    string ClientSecret,
    string TenantId,
    string ExtensionApplicationId,
    string OrganizationId,
    string Resource = "https://graph.microsoft.com/v1.0"
    );