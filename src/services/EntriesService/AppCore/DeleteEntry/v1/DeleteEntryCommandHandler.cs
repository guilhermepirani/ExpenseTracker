using System.Net;

using Mediator.Commands;

using Serilog;

namespace AppCore.DeleteEntry;

public class DeleteEntryCommandHandler
    : ICommandHandler<DeleteEntryCommand, Result<DeleteEntryResponse>>
{
    private readonly IDeleteEntryRepository _repository;

    public DeleteEntryCommandHandler(IDeleteEntryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<DeleteEntryResponse>> HandleAsync(
        DeleteEntryCommand command,
        CancellationToken cancellationToken)
    {
        var hasGuid = Guid.TryParse(command.Id, out Guid deleteTarget);

        if (!hasGuid)
        {
            return ResultFactory
                .CreateFailure<Result<DeleteEntryResponse>>(
                    HttpStatusCode.InternalServerError,
                    ["Failed to parse provided ID to UUID"]);
        }

        int affectedRows = await _repository.ExecuteAsync(deleteTarget);

        Log.Information($"{affectedRows} rows affected by DeleteCommand");

        return Result<DeleteEntryResponse>.Success(
            HttpStatusCode.OK, new DeleteEntryResponse()
            { RowsAffected = affectedRows });
    }
}