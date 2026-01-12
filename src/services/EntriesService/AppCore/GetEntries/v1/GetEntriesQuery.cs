using Mediator.Queries;

namespace AppCore.GetEntries;

public record GetEntriesQuery : IQuery<Result<List<GetEntriesResponse>>>
{
    public Guid? Id { get; set; } = null;
}