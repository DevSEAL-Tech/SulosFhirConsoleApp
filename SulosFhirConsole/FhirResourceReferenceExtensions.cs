using Hl7.Fhir.Model;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public static class FhirResourceReferenceExtensions
{
    public static string GetId(this ResourceReference resourceReference) => resourceReference.SplitReference()[1];

    public static string GetResourceType(this ResourceReference resourceReference) => resourceReference.SplitReference()[0];

    private static string[] SplitReference(this ResourceReference resourceReference) =>
        resourceReference.Reference.Split("/");
}