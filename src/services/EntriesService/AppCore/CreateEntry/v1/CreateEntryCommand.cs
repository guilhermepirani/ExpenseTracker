using Mediator.Commands;

namespace AppCore.CreateEntry;

public class CreateEntryCommand : ICommand<CreateEntryResponse>
{
    public string? Title { get; set; }
}