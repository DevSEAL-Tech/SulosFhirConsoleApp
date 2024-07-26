namespace Sulos.Hospice.Care.Core.Common.Azure;

public record SulosAzureAdPermissions(bool? Enabled, string? Role, string? RoleType, string? Profile);