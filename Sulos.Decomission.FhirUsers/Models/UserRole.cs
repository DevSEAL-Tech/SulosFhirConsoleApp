using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Sulos.Hospice.Care.Models.Users;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    [Description("Executive")]
    Executives,
    [Description("Care Team Member")]
    CareTeamMember,
    [Description("Regional Manager")]
    RegionalManager,
    [Description("Office Manager")]
    OfficeManager
}
public record UserRolebk(
    int Id,


    string? Name = null
);


