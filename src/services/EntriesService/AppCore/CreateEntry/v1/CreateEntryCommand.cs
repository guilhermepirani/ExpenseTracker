using Mediator.Commands;

namespace AppCore.CreateEntry;

public class CreateEntryCommand : ICommand<CreateEntryResponse>
{
    public required string Title { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; } = null;
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
}