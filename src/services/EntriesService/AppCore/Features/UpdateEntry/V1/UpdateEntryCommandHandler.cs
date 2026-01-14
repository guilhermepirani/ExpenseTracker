using System.Net;
using AppCore.Features.GetEntries.V1;
using Mediator.Commands;
using Serilog;

namespace AppCore.Features.UpdateEntry.V1;

public class UpdateEntryCommandHandler
    : ICommandHandler<UpdateEntryCommand, Result<UpdateEntryResponse>>
{
    private readonly IUpdateEntryRepository _updateEntryRepository;
    private readonly IGetEntriesRepository _getEntriesRepository;

    public UpdateEntryCommandHandler(
        IUpdateEntryRepository updateEntryRepository,
        IGetEntriesRepository getEntriesRepository)
    {
        _updateEntryRepository = updateEntryRepository;
        _getEntriesRepository = getEntriesRepository;
    }

    public async Task<Result<UpdateEntryResponse>> HandleAsync(
        UpdateEntryCommand command,
        CancellationToken cancellationToken)
    {
        var entryResponse = await _getEntriesRepository
            .ExecuteAsync(command.Id);

        if (entryResponse.Id == Guid.Empty)
        {
            Log.Information("Entry not found.");
            return Result<UpdateEntryResponse>.Failure(
                HttpStatusCode.NotFound,
                ["Entry not found."]);
        }

        var updatedEntry = command.UpdateEntry(
            entryResponse.MapToEntry());

        int response = await _updateEntryRepository
            .ExecuteAsync(updatedEntry);

        Log.Information($"Command executed. {response} Rows Affected");
        return Result<UpdateEntryResponse>.Success(
            HttpStatusCode.OK,
            new UpdateEntryResponse() { RowsAffected = response });
    }
}