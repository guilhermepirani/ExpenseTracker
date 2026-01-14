namespace AppCore.Features.UpdateEntry.V1;

public interface IUpdateEntryRepository
{
    public Task<int> ExecuteAsync(Entry entry);
}