namespace Sulos.Hospice.Care.Core.Common.Fhir;

public class FhirConfigOptions
{
    public const string ConfigSection = "Fhir";
    public string UrlPostfix { get; set; } = "";
    public string ReaderClientIdPostfix { get; set; } = "";
    public string ReaderClientSecretPostfix { get; set; } = "";
    public string WriterClientIdPostfix { get; set; } = "";
    public string WriterClientSecretPostfix { get; set; } = "";
    public string TenantIdPostfix { get; set; } = "";
    public int MaxBundleEntryCount { get; set; } = 500;
}