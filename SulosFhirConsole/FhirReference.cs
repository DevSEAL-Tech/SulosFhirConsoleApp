using Hl7.Fhir.Model;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public class FhirReference : ResourceReference
{
    public FhirReference(Type type, string id)
        : base(Value(type, id))
    {
        Type = type.Name;
    }

    public static string Value(Type type, string id)
    {
        return $"{type.Name}/{id}";
    }
}
public class FhirReference<T> : FhirReference
{
    public FhirReference(string id)
        : base(typeof(T), id)
    {

    }

    public static string Value(string id)
    {
        return FhirReference.Value(typeof(T), id);
    }
}