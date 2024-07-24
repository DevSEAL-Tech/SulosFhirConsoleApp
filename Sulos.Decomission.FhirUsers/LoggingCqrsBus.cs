using System.Diagnostics;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using Sulos.Hospice.Care.Core.Common.Cqrs.Commands;
using Sulos.Hospice.Care.Core.Common.Cqrs.Queries;

namespace Sulos.Hospice.Care.Core.Common.Cqrs;

public class LoggingCqrsBus : ICqrsBus
{
    private readonly ICqrsBus _innerBus;
    private readonly ILogger<LoggingCqrsBus> _logger;

    public LoggingCqrsBus(ICqrsBus bus, ILogger<LoggingCqrsBus> logger)
    {
        _innerBus = bus;
        _logger = logger;
    }

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken token = default)
    {
        using (CreateQueryScope(query))
            return await HandleAsync(query, () => _innerBus.QueryAsync(query, token)).ConfigureAwait(false);
    }

    public async Task<Unit> CommandAsync(ICommand command, CancellationToken token = default)
    {
        using (CreateCommandScope(command))
            return await HandleAsync(command, () => _innerBus.CommandAsync(command, token)).ConfigureAwait(false);
    }

    public async Task<TResult> CommandAsync<TResult>(ICommand<TResult> command, CancellationToken token = default)
    {
        using (CreateCommandScope(command))
            return await HandleAsync(command, () => _innerBus.CommandAsync(command, token)).ConfigureAwait(false);
    }

    private async Task<TResult> HandleAsync<TResult>(
        IRequest<TResult> request,
        Func<Task<TResult>> handler)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestName = request.GetType().Name;
        try
        {
            _logger.LogInformation("Handling {Request}...", requestName);
            var result = await handler().ConfigureAwait(false);
            stopwatch.Stop();
            _logger.LogInformation("Finished handling {Request} {ElapsedTime} ms", requestName,
                stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            throw HandleException(ex, stopwatch, requestName);
        }
    }

    private IDisposable CreateQueryScope<TResult>(IQuery<TResult> query)
    {
        var scopes = new Dictionary<string, string>
        {
            {"Query", query.GetType().Name}
        };
        return _logger.BeginScope(scopes)!;
    }

    private IDisposable CreateCommandScope(ICommand command)
    {
        var scopes = new Dictionary<string, string>
        {
            {"Command", command.GetType().Name}
        };
        return _logger.BeginScope(scopes)!;
    }

    private IDisposable CreateCommandScope<TResult>(ICommand<TResult> command)
    {
        var scopes = new Dictionary<string, string>
        {
            {"Command", command.GetType().Name}
        };
        return _logger.BeginScope(scopes)!;
    }

    private Exception HandleException(Exception ex, Stopwatch stopwatch, string requestName)
    {
        stopwatch.Stop();
        if (ex is FhirOperationException operationException)
            return HandleFhirException(operationException, stopwatch, requestName);

        _logger.LogError(ex, "Failed to handle {Request} after {ElapsedTime} ms", requestName, stopwatch.ElapsedMilliseconds);
        return ex;
    }

    private Exception HandleFhirException(FhirOperationException exception, Stopwatch stopwatch, string requestName)
    {
        var outcome = exception.Outcome?.ToJson();
        _logger.LogError(exception, "Fhir Operation failed for {Request} after {ElapsedTime} ms with outcome {Outcome}", requestName, stopwatch.ElapsedMilliseconds, outcome);
        return exception;
    }
}