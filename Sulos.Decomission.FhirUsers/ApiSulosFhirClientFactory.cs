using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sulos.Hospice.Care.Core.Common.Azure;
using Sulos.Hospice.Care.Core.Common.Fhir;
using Sulos.Hospice.Care.Core.Common.Fhir.Http;


namespace Sulos.Api.Common.Fhir;

public class ApiSulosFhirClientFactory : ISulosFhirClientFactory
{
    private readonly IFhirHttpMessageHandlerFactory _messageHandlerFactory;
    private readonly IFhirClientOptionsFactory _clientOptionsFactory;
    private readonly IOptions<FhirConfigOptions> _fhirConfigOptions;
    //private readonly CurrentSulosUser? _sulosUser;
    private readonly PathString _requestPath;

    public ApiSulosFhirClientFactory(
        IFhirHttpMessageHandlerFactory messageHandlerFactory,
        IFhirClientOptionsFactory clientOptionsFactory,
        IHttpContextAccessor httpContextAccessor,
        IOptions<FhirConfigOptions> fhirConfigOptions
    )
    {
        _messageHandlerFactory = messageHandlerFactory;
        _clientOptionsFactory = clientOptionsFactory;
        _fhirConfigOptions = fhirConfigOptions;

        // Do not save context as a member variable, extract the data needed from the context as member variables.
        // The context is Disposable, so tasks that use these methods can't be run in the background.
        var context = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        //_sulosUser = context.SulosUser();
        _requestPath = context.Request.Path;
    }

    public async Task<SulosFhirClient> CreateReaderAsync()
    {
        //if (ShouldThrowUserNotFound())
        //    throw new CurrentSulosUserNotFoundException();

        var organizationId = "mysulos";// _sulosUser.DetermineOrganizationId();
        var options = await _clientOptionsFactory.CreateReaderAsync(organizationId);
        return CreateSulosFhirClient(options);
    }

    public async Task<SulosTransactionBuilder> CreateTransactionBuilder(
        Bundle.BundleType type = Bundle.BundleType.Batch
    )
    {
        //if (ShouldThrowUserNotFound())
        //    throw new CurrentSulosUserNotFoundException();

        var organizationId = "mysulos";// _sulosUser.DetermineOrganizationId();
        var options = await _clientOptionsFactory.CreateReaderAsync(organizationId);
        return new SulosTransactionBuilder(options.Resource, type);
    }

    public async Task<SulosFhirClient> CreateWriterAsync()
    {
        //if (ShouldThrowUserNotFound())
        //    throw new CurrentSulosUserNotFoundException();

        //if (_sulosUser == null)
        //    throw new InvalidOperationException("Cannot create writer fhir client without hospice");

        var options = await _clientOptionsFactory.CreateWriterAsync("mysulos");
        return CreateSulosFhirClient(options);
    }

    public async Task<SulosFhirClient> CreateFunctionReaderAsync(string organizationId)
    {
        //GuardAgainstNonFunctionInvocation();

        var options = await _clientOptionsFactory.CreateReaderAsync(organizationId);
        return CreateSulosFhirClient(options);
    }

    public async Task<SulosFhirClient> CreateFunctionWriterAsync(string organizationId)
    {
       // GuardAgainstNonFunctionInvocation();

        var options = await _clientOptionsFactory.CreateWriterAsync(organizationId);
        return CreateSulosFhirClient(options);
    }

    public string GetOrganizationId()
    {
        return "mysulos";// _sulosUser.DetermineOrganizationId();
    }

    private SulosFhirClient CreateSulosFhirClient(ClientCredentialsOptions options)
    {
        //var handler = _messageHandlerFactory.CreateHandler(options, _sulosUser);
        return new SulosFhirClient(
            "mysulos",//_sulosUser.DetermineOrganizationId(),
            options,
            _fhirConfigOptions
        );
    }

    

   
}