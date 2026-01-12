using System.Net;

using Mediator.Queries;

using Serilog;

namespace AppCore.GetEntries;

public class GetEntriesQueryHandler
    : IQueryHandler<GetEntriesQuery, Result<List<GetEntriesResponse>>>
{

    private readonly IGetEntriesRepository _repository;

    public GetEntriesQueryHandler(IGetEntriesRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<GetEntriesResponse>>> HandleAsync(
        GetEntriesQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.Id is null)
        {
            var entriesResponse = await _repository.ExecuteAsync();

            Log.Information($"Query returned {entriesResponse.Count} entries.");
            return Result<List<GetEntriesResponse>>
                .Success(HttpStatusCode.OK, entriesResponse);
        }

        var hasGuid = Guid.TryParse(query.Id, out Guid searchFor);

        if (!hasGuid)
        {
            return ResultFactory
                .CreateFailure<Result<List<GetEntriesResponse>>>(
                    HttpStatusCode.InternalServerError,
                    ["Failed to parse provided ID to UUID"]);
        }

        var entryResponse = await _repository.ExecuteAsync(searchFor);

        if (entryResponse.Id == Guid.Empty)
        {
            Log.Information("Query returned 0 Entries.");
            return Result<List<GetEntriesResponse>>.Success(
                HttpStatusCode.OK, new List<GetEntriesResponse>());
        }

        Log.Information("Query returned 1 Entry.");
        return Result<List<GetEntriesResponse>>.Success(
            HttpStatusCode.OK, new List<GetEntriesResponse>()
                { entryResponse });
    }
}