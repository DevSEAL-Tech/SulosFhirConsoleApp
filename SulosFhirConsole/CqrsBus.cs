using MediatR;
using Sulos.Hospice.Care.Core.Common.Cqrs.Commands;
using Sulos.Hospice.Care.Core.Common.Cqrs.Queries;

namespace Sulos.Hospice.Care.Core.Common.Cqrs;

public interface ICqrsBus
{
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken token = default);
    Task<Unit> CommandAsync(ICommand command, CancellationToken token = default);
    Task<TResult> CommandAsync<TResult>(ICommand<TResult> command, CancellationToken token = default);
}

public class CqrsBus : ICqrsBus
{
    private readonly IQueryBus _queryBus;
    private readonly ICommandBus _commandBus;

    public CqrsBus(IQueryBus queryBus, ICommandBus commandBus)
    {
        _queryBus = queryBus;
        _commandBus = commandBus;
    }

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken token = default)
    {
        return await _queryBus.ExecuteAsync(query, token).ConfigureAwait(false);
    }

    public async Task<Unit> CommandAsync(ICommand command, CancellationToken token = default)
    {
        return await _commandBus.ExecuteAsync(command, token).ConfigureAwait(false);
    }

    public async Task<TResult> CommandAsync<TResult>(ICommand<TResult> command, CancellationToken token = default)
    {
        return await _commandBus.ExecuteAsync(command, token).ConfigureAwait(false);
    }
}