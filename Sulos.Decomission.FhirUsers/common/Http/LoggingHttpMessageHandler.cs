using Microsoft.Extensions.Logging;

namespace Sulos.Hospice.Care.Core.Common.Http;

public class LoggingHttpMessageHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHttpMessageHandler> _logger;

    public LoggingHttpMessageHandler(HttpMessageHandler next, ILogger<LoggingHttpMessageHandler> logger)
        : base(next)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Request starting {Version} [{Method}] {Url}", request.Version, request.Method,
                request.RequestUri);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Finished request {Version} [{Method}] {Url}", request.Version, request.Method,
                request.RequestUri);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request failed {Version} [{Method}] {Url}", request.Version, request.Method,
                request.RequestUri);
            throw;
        }
        
    }
}