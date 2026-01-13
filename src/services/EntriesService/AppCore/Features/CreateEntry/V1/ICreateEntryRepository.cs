namespace AppCore.Features.CreateEntry.V1;

public interface ICreateEntryRepository
{
    public Task<int> ExecuteAsync(Entry entry);
}