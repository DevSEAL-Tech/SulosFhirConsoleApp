namespace Sulos.Decomission.B2CUsers.Options;

public record GraphServiceOptions
{
   public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
    public required string TenantId { get; init; }
    public required string ExtensionApplicationId { get; init; }
    public required string OrganizationId { get; init; }
    public string Resource => "https://graph.microsoft.com/v1.0";
}