using Hl7.Fhir.Model;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public static class FhirDomainResourceExtensions
{
    public static bool? GetSulosExtensionValueAsBool(this DomainResource resource, string type)
    {
        var extension = resource.GetExtensionValue<FhirBoolean>(type.AsExtensionUrl());
        return extension?.Value;
    }
    public static string? GetSulosExtensionValue(this DomainResource resource, string type)
    {
        var extension = resource.GetExtensionValue<FhirString>(type.AsExtensionUrl());
        return extension?.Value;
    }

    public static TEnum? GetSulosExtensionEnum<TEnum>(this DomainResource resource, string type)
        where TEnum : struct
    {
        var value = resource.GetSulosExtensionValue(type);
        return Enum.TryParse<TEnum>(value, true, out var actual)
            ? actual
            : null;
    }

    public static void SetSulosExtensionEnum<TEnum>(this DomainResource resource, string type, TEnum value)
        where TEnum : struct
    {
        var actual = Enum.GetName(typeof(TEnum), value);
        resource.SetSulosExtensionValue(type, actual);
    }

    public static void SetSulosExtensionValue(this DomainResource resource, string type, string? value)
    {
        resource.RemoveExtension(type.AsExtensionUrl());
        resource.AddExtension(type.AsExtensionUrl(), new FhirString(value));
    }

    public static void SetSulosExtensionValueAsBool(this DomainResource resource, string type, bool? value)
    {
        resource.RemoveExtension(type.AsExtensionUrl());
        resource.AddExtension(type.AsExtensionUrl(), new FhirBoolean(value));
    }

    public static Extension? FindExtension(this DomainResource resource, string extension) =>
        resource.Extension?.Find(e => e.Url == $"{SulosConstants.SulosFhirBaseUrl}/{extension}");

    private static string AsExtensionUrl(this string type)
    {
        return $"{SulosConstants.SulosFhirBaseUrl}/{type}";
    }
}