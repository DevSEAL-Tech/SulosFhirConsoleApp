using Hl7.Fhir.Model;
using SulosFhirConsole;

namespace Sulos.Hospice.Care.Core.Common.Fhir;

public interface ISulosFhirClientFactory
{
    Task<SulosFhirClient> CreateReaderAsync();
    Task<SulosFhirClient> CreateWriterAsync();
    Task<SulosFhirClient> CreateFunctionReaderAsync(string organizationId);
    Task<SulosFhirClient> CreateFunctionWriterAsync(string organizationId);
    Task<SulosTransactionBuilder> CreateTransactionBuilder(Bundle.BundleType type = Bundle.BundleType.Batch);
    string GetOrganizationId();
}