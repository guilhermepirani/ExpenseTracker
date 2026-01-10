using Mediator;
using Mediator.Pipelines;

using Serilog;

namespace AppCore.Behaviours;

public class ExceptionHandlingBehaviour<TRequest, TResponse>
    : IPipelineBehaviour<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    public async Task<TResponse> HandleAsync(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            Log.Error(
                "Unhandled exception during request processing. {message}",
                ex.Message);

            return ResultFactory.CreateFailure<TResponse>(new[] { ex.Message });
        }
    }
}