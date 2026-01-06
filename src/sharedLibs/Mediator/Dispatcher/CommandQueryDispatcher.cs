using Mediator.Commands;
using Mediator.Pipelines;
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
    public Task<TResult> HandleAsync<TResult>(ICommand<TResult> command,
        CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(command, cancellationToken);
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
    public Task<TResult> HandleAsync<TResult>(IQuery<TResult> query,
        CancellationToken cancellationToken = default)
    {
        return ExecuteAsync(query, cancellationToken);
    }

    private async Task<TResult> ExecuteAsync<TResult>(
        IRequest<TResult> request,
        CancellationToken cancellationToken)
    {
        var requestType = request.GetType();

        var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(requestType, typeof(TResult));

        var handler = _serviceProvider.GetRequiredService(handlerType)
            ?? throw new InvalidOperationException("No Handler found");

        var method = handlerType.GetMethod("HandleAsync")
            ?? throw new InvalidOperationException("No Method found");

        var behaviourType = typeof(IPipelineBehaviour<,>)
            .MakeGenericType(requestType, typeof(TResult));

        var behaviours = _serviceProvider.GetServices(behaviourType).Reverse()
            ?? throw new InvalidOperationException("No Behaviour found");

        var behaviourHandlerMethod = behaviourType.GetMethod("HandleAsync")
            ?? throw new InvalidOperationException("No Behaviour Method found");

        RequestHandlerDelegate<TResult> handlerDelegate = () =>
            (Task<TResult>)method.Invoke(handler, [request, cancellationToken])!;

        foreach (var behaviour in behaviours)
        {
            var next = handlerDelegate;
            handlerDelegate = () => (Task<TResult>)behaviourHandlerMethod
                .Invoke(behaviour, [request, next, cancellationToken])!;
        }

        return await handlerDelegate();
    }
}