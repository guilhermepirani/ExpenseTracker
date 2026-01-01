using Mediator.Commands;
using Mediator.Queries;

using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Dispatcher;

/// <summary>
/// A dispatcher implementation that routes commands and queries to their respective handlers using reflection and dependency injection.
/// </summary>
/// <remarks>
/// This class uses the CQRS (Command Query Responsibility Segregation) pattern to separate command and query handling.
/// It leverages reflection to dynamically resolve and invoke handler methods at runtime.
/// </remarks>
public class CommandQueryDispatcher(IServiceProvider serviceProvider)
    : IDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    /// <summary>
    /// Sends a command to its corresponding handler asynchronously and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the command handler.</typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the result.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no handler is found, the HandleAsync method is not found, or the task generation fails.
    /// </exception>
    public async Task<TResult> HandleAsync<TResult>(ICommand<TResult> command,
        CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResult));

        var handler = _serviceProvider.GetRequiredService(handlerType)
            ?? throw new InvalidOperationException("No Handler found");

        var method = handlerType.GetMethod("HandleAsync")
            ?? throw new InvalidOperationException("No Method found");

        var task = method.Invoke(handler, [command, cancellationToken])
            as Task<TResult>;

        return task is not null
            ? await task
            : throw new InvalidOperationException(
                $"Failed to generate a Task for {handlerType}");
    }

    /// <summary>
    /// Executes a query against its corresponding handler asynchronously and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the query handler.</typeparam>
    /// <param name="query">The query to be executed.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the query result.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no handler is found, the HandleAsync method is not found, or the task generation fails.
    /// </exception>
    public async Task<TResult> HandleAsync<TResult>(IQuery<TResult> query,
        CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResult));

        var handler = _serviceProvider.GetRequiredService(handlerType)
            ?? throw new InvalidOperationException("No Handler found");

        var method = handlerType.GetMethod("HandleAsync")
            ?? throw new InvalidOperationException("No Method found");

        var task = method.Invoke(handler, [query, cancellationToken])
            as Task<TResult>;

        return task is not null
            ? await task
            : throw new InvalidOperationException(
                $"Failed to generate a Task for {handlerType}");
    }
}