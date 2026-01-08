using Mediator;
using Mediator.Commands;
using Mediator.Pipelines;

using Serilog;

namespace EntriesService.AppCore.Behaviours;

public class ValidationBeheviour<TRequest, TResponse>
    : IPipelineBehaviour<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        Log.Information(
            "Validating of type {RequestType}", typeof(TRequest).Name);

        var response = await next();

        Log.Information(
            "Backtracking for {RequestType}", typeof(TRequest).Name);

        return response;
    }
}