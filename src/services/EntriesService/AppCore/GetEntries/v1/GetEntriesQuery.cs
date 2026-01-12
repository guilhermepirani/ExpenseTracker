using Mediator.Queries;

namespace AppCore.GetEntries;

public record GetEntriesQuery : IQuery<Result<List<GetEntriesResponse>>>
{
    public string? Id { get; set; } = null;
}