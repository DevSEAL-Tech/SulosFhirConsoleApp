using Hl7.Fhir.Support;

namespace Sulos.Hospice.Care.Core;

public static class DateTimeExtensions
{
    public static string ToFhirTimestamp(this DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.ToUniversalTime().ToFhirDateTime();
    }
}