using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using Sulos.Hospice.Care.Core.Common.Azure;
using Sulos.Hospice.Care.Core.Common.Fhir;
using Sulos.Hospice.Care.Core.Common.Fhir.Http;


namespace Sulos.Api.Common.Fhir;

public interface ISulosFhirClientFactory
{
    Task<SulosFhirClient> CreateReaderAsync(string organization);
    Task<SulosFhirClient> CreateWriterAsync(string organization);
    Task<SulosTransactionBuilder> CreateTransactionBuilder(string organization, Bundle.BundleType type = Bundle.BundleType.Batch);
}

public class ApiSulosFhirClientFactory : ISulosFhirClientFactory
{
    private readonly IFhirHttpMessageHandlerFactory _messageHandlerFactory;
    private readonly IFhirClientOptionsFactory _clientOptionsFactory;
    private readonly IOptions<FhirConfigOptions> _fhirConfigOptions;
  
    public ApiSulosFhirClientFactory(
        IFhirHttpMessageHandlerFactory messageHandlerFactory,
        IFhirClientOptionsFactory clientOptionsFactory,
    
        IOptions<FhirConfigOptions> fhirConfigOptions
    )
    {
        _messageHandlerFactory = messageHandlerFactory;
        _clientOptionsFactory = clientOptionsFactory;
        _fhirConfigOptions = fhirConfigOptions;
    }

    public async Task<SulosFhirClient> CreateReaderAsync(string organization)
    {
        var options = _clientOptionsFactory.CreateReaderAsync(organization);
        return CreateSulosFhirClient(options,organization);
    }

    public async Task<SulosFhirClient> CreateWriterAsync(string organization)
    {
        var options = _clientOptionsFactory.CreateWriterAsync(organization);
        return CreateSulosFhirClient(options,organization);
    }

    private SulosFhirClient CreateSulosFhirClient(ClientCredentialsOptions options, string organization)
    {
        var handler = _messageHandlerFactory.CreateHandler(options, null);
        return new SulosFhirClient(
            organization,
            options,
            handler,
            _fhirConfigOptions
        );
    }

    public async Task<SulosTransactionBuilder> CreateTransactionBuilder(string organization, Bundle.BundleType type = Bundle.BundleType.Batch)
    {
        var options = _clientOptionsFactory.CreateReaderAsync(organization);
        return new SulosTransactionBuilder(options.Resource, type);
    }
}
