using AppCore.Features.CreateEntry.V1;
using Mediator.Dispatcher;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace Api.Features.CreateEntry.V1;

public class CreateEntryEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/entries", async (
            [FromBody] CreateEntryCommand command,
            IDispatcher dispatcher,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            // Expand command properties on the log context
            LogContext.PushProperty("REQUEST", command, true);

            var result = await dispatcher.HandleAsync(command, cancellationToken);

            httpContext.Response.StatusCode = (int)result.StatusCode;

            // TODO: get address from settings
            if (result.Data is not null)
            {
                httpContext.Response.Headers.Location =
                    $"http://localhost:5199/api/v1/entries/{result.Data.Id}";
            }

            return result;
        })
        .MapToApiVersion(1);
    }
}