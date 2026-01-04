using Mediator.Commands;

namespace AppCore.CreateEntry;

public class CreateEntryCommandHandler
    : ICommandHandler<CreateEntryCommand, CreateEntryResponse>
{
    public async Task<CreateEntryResponse> HandleAsync(CreateEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        return new CreateEntryResponse { Id = 5 };
    }
}