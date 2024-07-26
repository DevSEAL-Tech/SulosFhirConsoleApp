using Hl7.Fhir.Model;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public interface ISulosFhirClientFactory
{
    Task<SulosFhirClient> CreateReaderAsync();
    Task<SulosFhirClient> CreateWriterAsync();
    //Task<SulosFhirClient> CreateFunctionReaderAsync();
    //Task<SulosFhirClient> CreateFunctionWriterAsync();
    Task<SulosTransactionBuilder> CreateTransactionBuilder(Bundle.BundleType type = Bundle.BundleType.Batch);

}