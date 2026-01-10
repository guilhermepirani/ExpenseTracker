using Mediator.Commands;

namespace AppCore.CreateEntry;

public class CreateEntryCommandHandler
    : ICommandHandler<CreateEntryCommand, Result<CreateEntryResponse>>
{
    public async Task<Result<CreateEntryResponse>> HandleAsync(CreateEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        throw new Exception("Teste");
        var response = new CreateEntryResponse { Id = 5 };
        return Result<CreateEntryResponse>.Success(response);
    }
}