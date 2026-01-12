using Mediator.Commands;

namespace AppCore.DeleteEntry;

public class DeleteEntryCommand : ICommand<Result<DeleteEntryResponse>>
{
    public string? Id { get; set; }
}