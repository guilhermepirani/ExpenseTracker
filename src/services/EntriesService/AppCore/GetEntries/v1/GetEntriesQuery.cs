using Mediator.Queries;

namespace AppCore.GetEntries;

public record GetEntriesQuery : IQuery<Result<List<GetEntriesResponse>>>
{
    public int? Id { get; set; } = null;
}