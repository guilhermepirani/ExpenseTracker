using Mediator;
using Mediator.Pipelines;

using Serilog;

namespace AppCore.Behaviours;

public class LoggingBeheviour<TRequest, TResponse>
    : IPipelineBehaviour<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        Log.Information(
            "Starting {RequestType} with body: {REQUEST}.",
            typeof(TRequest).Name); ;

        var response = await next();

        Log.Information(
            "Finished {RequestType}.",
            typeof(TRequest).Name);

        return response;
    }
}