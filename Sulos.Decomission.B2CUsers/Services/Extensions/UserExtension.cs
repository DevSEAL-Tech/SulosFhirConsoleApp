using Microsoft.Graph.Models;
using Sulos.Decomission.Shared.Models;

namespace Sulos.Decomission.B2CUsers.Services.Extensions;

public static class UserExtension
{
    public static UserExport ToExportObject(this User user, GraphServiceExtensions graphServiceExtensions)
    {
        return new UserExport
        {
            FirstName = user.GivenName!,
            LastName = user.Surname!,
            EmailAddresses = user.Identities.Select(id => id.IssuerAssignedId).ToArray(),
            FhirId = getValue(GraphServiceExtensionAttributes.FhirID, user, graphServiceExtensions),
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
