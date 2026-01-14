using AppCore;
using AppCore.Features.UpdateEntry.V1;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Serilog;

namespace Infra.Features.UpdateEntry.V1;

public class UpdateEntryRepository : IUpdateEntryRepository
{
    private readonly IConfiguration _configuration;
    public UpdateEntryRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int> ExecuteAsync(Entry entry)
    {
        try
        {
            NpgsqlDataSourceBuilder dataSourceBuilder = new(
                _configuration.GetConnectionString("DefaultConnection")
            );

            await using var source = dataSourceBuilder.Build();
            await using var cmd = new NpgsqlCommand(
                "UPDATE entries " +
                "SET title = ($1), amount = ($2), description = ($3), date = ($4) " +
                "WHERE id = ($5)",
                await source.OpenConnectionAsync())
            {
                Parameters =
                {
                    new() { Value = entry.Title },
                    new() { Value = entry.Amount },
                    new() { Value = entry.Description },
                    new() { Value = entry.Date },
                    new() { Value = entry.Id }
                }
            };

            return await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Log.Error("Database Error? {ex}", ex);
            throw new Exception("Database error");
        }
    }
}