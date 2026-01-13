namespace AppCore.Features.DeleteEntry.V1;

public interface IDeleteEntryRepository
{
    public Task<int> ExecuteAsync(Guid id);
}