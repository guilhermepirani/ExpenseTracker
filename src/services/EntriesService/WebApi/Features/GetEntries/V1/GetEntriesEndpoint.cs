using AppCore.Features.GetEntries.V1;
using Mediator.Dispatcher;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace Api.Features.GetEntries.V1;

public class GetEntriesEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/entries", async (
            IDispatcher dispatcher,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var query = new GetEntriesQuery();

            // Expand query properties on the log context
            LogContext.PushProperty("REQUEST", query, true);

            var result = await dispatcher.HandleAsync(query, cancellationToken);

            httpContext.Response.StatusCode = (int)result.StatusCode;
            return result;
        })
        .MapToApiVersion(1);

        routeBuilder.MapGet("/entries/{id}", async (
            [FromRoute] string id,
            IDispatcher dispatcher,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var query = new GetEntriesQuery() { Id = id };

            // Expand query properties on the log context
            LogContext.PushProperty("REQUEST", query, true);

            var result = await dispatcher.HandleAsync(query, cancellationToken);

            httpContext.Response.StatusCode = (int)result.StatusCode;
            return result;
        })
        .MapToApiVersion(1);
    }
}