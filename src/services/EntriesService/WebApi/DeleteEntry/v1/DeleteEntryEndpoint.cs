using AppCore.DeleteEntry;

using Mediator.Dispatcher;

using Microsoft.AspNetCore.Mvc;

using Serilog.Context;

namespace EntriesService.Api;

public class DeleteEntryEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapDelete("/entries/{id}", async (
            [FromRoute] string id,
            IDispatcher dispatcher,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteEntryCommand() { Id = id };

            // Expand query properties on the log context
            LogContext.PushProperty("REQUEST", command, true);

            var result = await dispatcher.HandleAsync(command, cancellationToken);

            httpContext.Response.StatusCode = (int)result.StatusCode;
            return result;
        })
        .MapToApiVersion(1);
    }
}