using System.Data.Common;
using System.Net;

using Mediator.Commands;

using Serilog;

namespace AppCore.CreateEntry;

public class CreateEntryCommandHandler
    : ICommandHandler<CreateEntryCommand, Result<CreateEntryResponse>>
{
    private readonly ICreateEntryRepository _repository;

    public CreateEntryCommandHandler(
        ICreateEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CreateEntryResponse>> HandleAsync(
        CreateEntryCommand command,
        CancellationToken cancellationToken = default)
    {
        // Test purposes
        //throw new Exception("Teste");

        var response = await _repository.ExecuteAsync(command);

        if (response is 0)
        {
            Log.Error("Entry was NOT created.");

            return ResultFactory
                .CreateFailure<Result<CreateEntryResponse>>(
                    HttpStatusCode.InternalServerError,
                    ["Entry was NOT created."]);
        }

        Log.Information("Entry created.");

        var createdEntry = new CreateEntryResponse { Id = 5 };
        return Result<CreateEntryResponse>
            .Success(HttpStatusCode.Created, createdEntry);
    }
}