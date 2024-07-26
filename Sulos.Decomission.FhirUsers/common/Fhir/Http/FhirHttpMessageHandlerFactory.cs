using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sulos.Hospice.Care.Core.Common.Azure;
using Sulos.Hospice.Care.Core.Common.Http;



namespace Sulos.Hospice.Care.Core.Common.Fhir.Http;

public interface IFhirHttpMessageHandlerFactory
{
    HttpMessageHandler CreateHandler(ClientCredentialsOptions options, HttpMessageHandler? rootHandler = null);
}

public class FhirHttpMessageHandlerFactory : IFhirHttpMessageHandlerFactory
{
    private readonly IAzureAccessTokenFactory _accessTokenFactory;
    private readonly IHostEnvironment _env;
    private readonly ILoggerFactory _loggerFactory;

    public FhirHttpMessageHandlerFactory(
        IAzureAccessTokenFactory accessTokenFactory,
        IHostEnvironment env,
        ILoggerFactory loggerFactory)
    {
        _accessTokenFactory = accessTokenFactory;
        _env = env;
        _loggerFactory = loggerFactory;
    }
    
    public HttpMessageHandler CreateHandler(ClientCredentialsOptions options, HttpMessageHandler? rootHandler = null)
    {
        var root = rootHandler ?? new HttpClientHandler();
        var auditingHandler = new FhirAuditingHttpMessageHandler(root);
        var logger = _loggerFactory.CreateLogger<LoggingHttpMessageHandler>();
        var loggingHandler = new LoggingHttpMessageHandler(auditingHandler, logger);
        var accessTokenHandler = new AzureAccessTokenMessageHandler(loggingHandler, _accessTokenFactory, _env, options);
        return new RetryHttpMessageHandler(accessTokenHandler);
    }
}