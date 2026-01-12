namespace AppCore.GetEntries;

public interface IGetEntriesRepository
{
    public Task<GetEntriesResponse> ExecuteAsync(Guid? id);
    public Task<List<GetEntriesResponse>> ExecuteAsync();
}