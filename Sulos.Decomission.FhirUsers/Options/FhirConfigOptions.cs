namespace Sulos.Hospice.Care.Core.Common.Fhir;

public class FhirConfigOptions
{
    public const string ConfigSection = "Fhir";
    public string Url { get; set; } = "";
    public string ReaderClientId { get; set; } = "";
    public string ReaderClientSecret { get; set; } = "";
    public string WriterClientId { get; set; } = "";
    public string WriterClientSecret { get; set; } = "";
    public string TenantId { get; set; } = "";
    public int MaxBundleEntryCount { get; set; } = 500;
}