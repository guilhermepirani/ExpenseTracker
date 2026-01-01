using Mediator.Commands;
using Mediator.Queries;

namespace Mediator.Dispatcher;

/// <summary>
/// A dispatcher abstraction that routes commands and queries to their respective handlers using reflection and dependency injection.
/// </summary>
public interface IDispatcher
{

    /// <summary>
    /// Sends a command to its corresponding handler asynchronously and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the command handler.</typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the result.</returns>
    Task<TResult> HandleAsync<TResult>(ICommand<TResult> command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query against its corresponding handler asynchronously and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the query handler.</typeparam>
    /// <param name="query">The query to be executed.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the query result.</returns>
    Task<TResult> HandleAsync<TResult>(IQuery<TResult> query,
        CancellationToken cancellationToken = default);
}