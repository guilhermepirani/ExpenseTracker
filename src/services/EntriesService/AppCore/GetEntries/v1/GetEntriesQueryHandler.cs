using System.Net;

using Mediator.Queries;

namespace AppCore.GetEntries;

public class GetEntriesQueryHandler
    : IQueryHandler<GetEntriesQuery, Result<List<GetEntriesResponse>>>
{
    public async Task<Result<List<GetEntriesResponse>>> HandleAsync(GetEntriesQuery query,
        CancellationToken cancellationToken = default)
    {
        var entry1 = new Entry
        {
            Id = Guid.Parse("019baf61-3364-73dd-88c0-134ff1e76ad1"),
            Title = "Entry 1"
        };

        var entry2 = new Entry
        {
            Id = Guid.Parse("019baf62-b258-746e-aad2-aba210a07bc9"),
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
            return Result<List<GetEntriesResponse>>.Success(
                HttpStatusCode.OK, response);
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
        return Result<List<GetEntriesResponse>>
            .Success(HttpStatusCode.OK, response);
    }
}