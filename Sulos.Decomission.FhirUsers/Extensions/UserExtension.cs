using Sulos.Decomission.Shared.Models;
using Sulos.Hospice.Care.Models.Common;

namespace Sulos.Decomission.FhirUsers.Extensions
{
    public static class UserExtension
    {
        public static UserExport ToExportObject(this SulosPersonModel user)
        {
            return new UserExport
            {
                FirstName = user.FirstName,
                LastName = user.LastName!,
                EmailAddresses = [user.EmailAddress] ,
                FhirId =user.Id,
                Role = "",
                RoleType = user.PersonType.ToString(),
                Profile = "",
                OrganizationId = ""
            };
        }
    }
}
