
using Sulos.Hospice.Care.Models.Users;

namespace Sulos.Hospice.Care.Models.Common;

public record SulosPersonModel(
    string Id,
    string FirstName,
    string LastName,
    PersonType PersonType,
    UserRole? UserRole = null,
    string? UserRoleType = null
);

