using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sulos.Hospice.Care.Core.Common.Azure;
using Sulos.Hospice.Care.Core.Common.Fhir;
using Sulos.Hospice.Care.Core.Common.Fhir.Http;
using Sulos.Hospice.Care.Core.Common.Fhir.Searching;


namespace Sulos.Api.Common.Fhir;

public class ApiSulosFhirClientFactory : ISulosFhirClientFactory
{
    private readonly IFhirHttpMessageHandlerFactory _messageHandlerFactory;
    private readonly IFhirClientOptionsFactory _clientOptionsFactory;
    private readonly IOptions<FhirConfigOptions> _fhirConfigOptions;
  
    private readonly PathString _requestPath;

    public ApiSulosFhirClientFactory(
        IFhirHttpMessageHandlerFactory messageHandlerFactory,
        IFhirClientOptionsFactory clientOptionsFactory,
    
        IOptions<FhirConfigOptions> fhirConfigOptions
    )
    {
        _messageHandlerFactory = messageHandlerFactory;
        _clientOptionsFactory = clientOptionsFactory;
        _fhirConfigOptions = fhirConfigOptions;

        // Do not save context as a member variable, extract the data needed from the context as member variables.
        // The context is Disposable, so tasks that use these methods can't be run in the background.
        //var context = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        //_requestPath = context.Request.Path;
    }

    public async Task<SulosFhirClient> CreateReaderAsync()
    {
        //var organizationId = "mysulos";
        var options = await _clientOptionsFactory.CreateReaderAsync();
        return CreateSulosFhirClient(options);
    }

    public async Task<SulosFhirClient> CreateWriterAsync()
    {
        
        var options = await _clientOptionsFactory.CreateWriterAsync();
        return CreateSulosFhirClient(options);
    }

    public async Task<SulosFhirClient> CreateFunctionReaderAsync()
    {
        
        var options = await _clientOptionsFactory.CreateReaderAsync();
        return CreateSulosFhirClient(options);
    }

    public async Task<SulosFhirClient> CreateFunctionWriterAsync()
    {
       
        var options = await _clientOptionsFactory.CreateWriterAsync();
        return CreateSulosFhirClient(options);
    }

    public string GetOrganizationId()
    {
        return "mysulos";// _sulosUser.DetermineOrganizationId();
    }

    private SulosFhirClient CreateSulosFhirClient(ClientCredentialsOptions options)
    {
        var handler = _messageHandlerFactory.CreateHandler(options, null);
        return new SulosFhirClient(
            "mysulos",
            options,
            handler,
            _fhirConfigOptions
        );
    }

    public async Task<SulosTransactionBuilder> CreateTransactionBuilder(Bundle.BundleType type = Bundle.BundleType.Batch)
    {
        var options = await _clientOptionsFactory.CreateReaderAsync();
        return new SulosTransactionBuilder(options.Resource, type);
    }
}
