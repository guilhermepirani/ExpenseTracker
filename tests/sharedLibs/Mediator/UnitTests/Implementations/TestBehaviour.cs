using Mediator.Pipelines;

namespace Mediator.UnitTests;

/// <summary>
/// A test pipeline behaviour implementation for testing purposes.
/// </summary>
public class TestBehaviour<TRequest, TResponse> : IPipelineBehaviour<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        return next();
    }
}