using System.Text.Json.Serialization;

namespace Sulos.Hospice.Care.Models.Patients;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VitalSignKey
{
    Unknown,
    HeartRate,
    HeartRateVariability,
    RespiratoryRate,
    TimeSpentAsleep,
    Steps,
    SpO2
}

public record VitalSign(
    string Id,
    string PatientId,
    VitalSignKey Key,
    decimal? Value,
    string Unit,
    DateTimeOffset Timestamp
);

public static class VitalSignExtensions
{
    public static decimal? GetVitalSignValue(this VitalSign[] vitals, VitalSignKey key)
    {
        var vital = Array.Find(vitals, s => s.Key == key);
        return vital?.Value;
    }
}