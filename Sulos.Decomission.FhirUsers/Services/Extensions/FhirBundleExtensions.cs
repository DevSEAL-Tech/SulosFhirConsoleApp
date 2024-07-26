using Hl7.Fhir.Model;
using Sulos.Hospice.Care.Core.Common.Fhir.Exceptions;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public static class FhirBundleExtensions
{
    public static IEnumerable<T> GetAllResources<T>(this Bundle bundle)
        where T : Resource
    {
        foreach (var resource in bundle.GetResources())
        {
            if (resource is OperationOutcome)
                throw new SulosFhirOperationException(resource);

            yield return (T)resource;
        }
    }

    public static IEnumerable<T> GetResourcesOfType<T>(this Bundle bundle) where T : Resource =>
        bundle
            .GetResources()
            .Where(resource => resource.TypeName == typeof(T).Name)
            .Select(resource => (T)resource);
}