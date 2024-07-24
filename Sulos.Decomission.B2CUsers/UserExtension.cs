using Microsoft.Graph.Models;
using Sulos.Decomission.B2CUsers.Services.Extensions;

namespace Sulos.Decomission.B2CUsers;

public static class UserExtension
{
    public static B2CUserExport ToExportObject(this User user, GraphServiceExtensions graphServiceExtensions)
    {
        return new B2CUserExport
        {
            FirstName = user.GivenName!,
            LastName = user.Surname!,
            EmailAddresses = user.Identities.Select(id => id.IssuerAssignedId).ToArray(),
            FhirId =getValue(GraphServiceExtensionAttributes.FhirID, user, graphServiceExtensions),
            Role = getValue(GraphServiceExtensionAttributes.Role, user, graphServiceExtensions),
            RoleType = getValue(GraphServiceExtensionAttributes.RoleType, user, graphServiceExtensions),
            Profile = getValue(GraphServiceExtensionAttributes.Profile, user, graphServiceExtensions),
            OrganizationId = getValue(GraphServiceExtensionAttributes.OrganizationID, user, graphServiceExtensions)
        };
    }

    private static string getValue(GraphServiceExtensionAttributes key, User user, GraphServiceExtensions graphServiceExtensions)
    {
        if (!user.AdditionalData.TryGetValue(graphServiceExtensions[key], out object? value))
        {
            value = "";
        }
        return value.ToString();
    }
}

public class B2CUserExport
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FhirId { get; set; } = "";
    public string Role { get; set; } = "";
    public string RoleType { get; set; } = "";
    public string Profile { get; set; } = "";
    public string OrganizationId { get; set; } = "";
    public string[] EmailAddresses { get; set; }
}