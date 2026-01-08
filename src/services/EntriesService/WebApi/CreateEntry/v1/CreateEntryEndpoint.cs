using System.Text.RegularExpressions;

using AppCore.CreateEntry;

using Mediator.Dispatcher;

using Microsoft.AspNetCore.Mvc;

using Serilog.Context;

namespace EntriesService.Api;

public class CreateEntryEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/entries", async (
            [FromBody] CreateEntryCommand command,
            IDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            // Expand command properties on the log context
            LogContext.PushProperty("REQUEST", command, true);

            return await dispatcher.HandleAsync(command, cancellationToken);

        })
        .MapToApiVersion(1);
    }
}