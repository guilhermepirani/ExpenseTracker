namespace AppCore.CreateEntry;

public interface ICreateEntryRepository
{
    public Task<int> ExecuteAsync(CreateEntryCommand command);
}