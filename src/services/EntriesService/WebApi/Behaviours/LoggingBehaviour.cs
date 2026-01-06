using Mediator;
using Mediator.Pipelines;

using Serilog;

namespace EntriesService.Api.Behaviours;

public class LoggingBeheviour<TRequest, TResponse>
    : IPipelineBehaviour<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        Log.Information(
            "Starting request of type {RequestType}",
            typeof(TRequest).Name); ;

        var response = await next();

        Log.Information(
            "Finished request of type {RequestType}",
            typeof(TRequest).Name);

        return response;
    }
}