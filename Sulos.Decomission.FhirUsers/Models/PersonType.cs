using System.Text.Json.Serialization;

namespace Sulos.Hospice.Care.Models.Users;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PersonType
{
    InternalUser,
    ExternalUser,
    Patient
}