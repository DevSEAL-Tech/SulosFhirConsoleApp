using System.Text.Json.Serialization;
using Hl7.Fhir.Rest;

namespace Sulos.Hospice.Care.Core.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortDirection
{
    Ascending,
    Descending
}

public static class SortDirectionExtensions
{
    public static SortOrder ToFhirSortOrder(this SortDirection direction)
    {
        return direction switch
        {
            SortDirection.Ascending => SortOrder.Ascending,
            SortDirection.Descending => SortOrder.Descending,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}