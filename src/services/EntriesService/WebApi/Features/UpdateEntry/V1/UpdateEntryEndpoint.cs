using AppCore.Features.UpdateEntry.V1;
using Mediator.Dispatcher;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace Api.Features.UpdateEntry.V1;

public class UpdateEntryEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPut("/entries", async (
            [FromBody] UpdateEntryCommand command,
            IDispatcher dispatcher,
            HttpContext httpContext,
            CancellationToken cancellationToken
        ) =>
        {
            // Expand command properties on the log context
            LogContext.PushProperty("REQUEST", command, true);

            var result = await dispatcher.HandleAsync(command, cancellationToken);

            httpContext.Response.StatusCode = (int)result.StatusCode;

            return result;
        })
        .MapToApiVersion(1);
    }
}