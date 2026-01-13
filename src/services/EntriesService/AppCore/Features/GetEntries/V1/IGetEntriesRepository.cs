namespace AppCore.Features.GetEntries.V1;

public interface IGetEntriesRepository
{
    public Task<GetEntriesResponse> ExecuteAsync(Guid? id);
    public Task<List<GetEntriesResponse>> ExecuteAsync();
}