using Mediator.Queries;

namespace AppCore.GetEntries;

public class GetEntriesQueryHandler
    : IQueryHandler<GetEntriesQuery, List<GetEntriesResponse>>
{
    public async Task<List<GetEntriesResponse>> HandleAsync(GetEntriesQuery query,
        CancellationToken cancellationToken = default)
    {
        var entry1 = new Entry
        {
            Id = 1,
            Title = "Entry 1"
        };

        var entry2 = new Entry
        {
            Id = 2,
            Title = "Entry 2"
        };

        var response = new List<GetEntriesResponse>();

        if (query.Id is null)
        {
            response.Add(new GetEntriesResponse
            {
                Id = entry1.Id,
                Title = entry1.Title
            });

            response.Add(new GetEntriesResponse
            {
                Id = entry2.Id,
                Title = entry2.Title
            });

            await Task.Yield();
            return response;
        }

        if (query.Id == entry1.Id)
        {
            response.Add(new GetEntriesResponse
            {
                Id = entry1.Id,
                Title = entry1.Title
            });
        }

        if (query.Id == entry2.Id)
        {
            response.Add(new GetEntriesResponse
            {
                Id = entry2.Id,
                Title = entry2.Title
            });
        }

        await Task.Yield();
        return response;
    }

}