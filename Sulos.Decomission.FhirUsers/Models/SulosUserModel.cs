using Sulos.Hospice.Care.Models.Common;

namespace Sulos.Hospice.Care.Models.Users;

public record SulosUserModel(
    string Id,
    string FirstName,
    string LastName,
    UserStatus? Status,
    UserProfile? Profile,
    UserRole? UserRole,
    string? UserRoleType = null,
    SulosContactModel? Contact = null,
    DateTimeOffset? LastLoginTime = null
) : SulosPersonModel(Id, FirstName, LastName, PersonType.InternalUser, UserRole, UserRoleType)

{
    public static SulosUserModel FromPersonModel(
        SulosPersonModel person,
        UserStatus? status,
        UserProfile? profile,
        SulosContactModel? contact)
    {
        return new SulosUserModel(
            person.Id,
            person.FirstName,
            person.LastName,
            status,
            profile,
            person.UserRole,
            person.UserRoleType,
            contact
            
        );
    }
}