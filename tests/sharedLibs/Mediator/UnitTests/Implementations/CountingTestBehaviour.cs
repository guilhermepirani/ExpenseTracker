using Mediator.Pipelines;

namespace Mediator.UnitTests;

/// <summary>
/// A test behaviour with a counter to track execution.
/// </summary>
public class CountingTestBehaviour<TRequest, TResponse> : IPipelineBehaviour<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public static int ExecutionCount { get; set; }

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        ExecutionCount++;
        return await next();
    }
}