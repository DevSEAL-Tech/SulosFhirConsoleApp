using MediatR;
using System.Windows.Input;

namespace Sulos.Hospice.Care.Core.Common.Cqrs.Commands;

public interface ICommandBus
{
    Task<Unit> ExecuteAsync(ICommand command, CancellationToken token = default);
    Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken token = default);
}

public class CommandBus : ICommandBus
{
    private readonly IMediator _mediator;

    public CommandBus(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<Unit> ExecuteAsync(ICommand command, CancellationToken token = default)
    {
        return await _mediator.Send(command, token).ConfigureAwait(false);
    }

    public async Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken token = default)
    {
        return await _mediator.Send(command, token).ConfigureAwait(false);
    }
}


public interface ICommand : IRequest<Unit>
{

}

public interface ICommand<out T> : IRequest<T>
{

}