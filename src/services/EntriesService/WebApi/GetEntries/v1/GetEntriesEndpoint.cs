using AppCore.GetEntries;

using Mediator.Dispatcher;

using Microsoft.AspNetCore.Mvc;

namespace EntriesService.Api;

public class GetEntriesEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/entries", async (
            IDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            var query = new GetEntriesQuery();
            return await dispatcher.HandleAsync(query, cancellationToken);
        })
        .MapToApiVersion(1);

        routeBuilder.MapGet("/entries/{id}", async (
            [FromRoute] int id,
            IDispatcher dispatcher,
            CancellationToken cancellationToken) =>
        {
            var query = new GetEntriesQuery() { Id = id };
            return await dispatcher.HandleAsync(query, cancellationToken);
        })
        .MapToApiVersion(1);
    }
}