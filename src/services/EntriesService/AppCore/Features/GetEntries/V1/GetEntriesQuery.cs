using Mediator.Queries;

namespace AppCore.Features.GetEntries.V1;

public record GetEntriesQuery : IQuery<Result<List<GetEntriesResponse>>>
{
    public string? Id { get; set; } = null;
}