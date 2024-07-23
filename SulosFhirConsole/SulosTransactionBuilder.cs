using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public class SulosTransactionBuilder : TransactionBuilder
{
    public SulosTransactionBuilder(
        string resource,
        Bundle.BundleType type = Bundle.BundleType.Batch
        ) : base(resource, type)
    {
    }

    public SulosTransactionBuilder Search<T>(SearchParams searchParams)
    {
        Search(searchParams, typeof(T).Name);
        return this;
    }
}