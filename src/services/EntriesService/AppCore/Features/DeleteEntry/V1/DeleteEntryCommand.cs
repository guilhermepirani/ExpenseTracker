using Mediator.Commands;

namespace AppCore.Features.DeleteEntry.V1;

public class DeleteEntryCommand : ICommand<Result<DeleteEntryResponse>>
{
    public string? Id { get; set; }
}