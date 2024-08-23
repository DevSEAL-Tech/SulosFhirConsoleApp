namespace Sulos.Hospice.Care.Core.Common.Azure;

public record ClientCredentialsOptions(
    string Resource,
    string ClientId,
    string ClientSecret,
    string TenantId
);