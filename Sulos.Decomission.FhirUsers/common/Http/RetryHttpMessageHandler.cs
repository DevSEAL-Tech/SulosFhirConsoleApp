using System.Net;
using Polly;

namespace Sulos.Hospice.Care.Core.Common.Http;

public class RetryHttpMessageHandler : DelegatingHandler
{
    private int MaxRetries { get; }

    public RetryHttpMessageHandler(HttpMessageHandler inner, int maxRetries = 6)
        : base(inner)
    {
        MaxRetries = maxRetries;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(MaxRetries, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)))
            .ExecuteAsync(async () => await base.SendAsync(request, cancellationToken).ConfigureAwait(false))
            .ConfigureAwait(false);
    }
}