using Mediator;
using Mediator.Pipelines;

using Serilog;

namespace EntriesService.AppCore.Behaviours;

public class ExceptionHandlingBehaviour<TRequest, TResponse>
    : IPipelineBehaviour<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
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
            Log.Error("Finished due to unhandled exception. Message: {ex}",
            ex);
            throw;
        }
    }
}