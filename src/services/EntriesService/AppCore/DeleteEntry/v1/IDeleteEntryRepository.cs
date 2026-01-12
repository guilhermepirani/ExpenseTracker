namespace AppCore.DeleteEntry;

public interface IDeleteEntryRepository
{
    public Task<int> ExecuteAsync(Guid id);
}