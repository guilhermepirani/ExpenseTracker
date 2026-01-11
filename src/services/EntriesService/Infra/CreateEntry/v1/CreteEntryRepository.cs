using AppCore.CreateEntry;

using Microsoft.Extensions.Configuration;

using Npgsql;

using Serilog;

namespace Infra.CreateEntry;

public class CreateEntryRepository : ICreateEntryRepository
{
    private readonly IConfiguration _configuration;

    public CreateEntryRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int> ExecuteAsync(CreateEntryCommand command)
    {
        try
        {
            NpgsqlDataSourceBuilder dataSourceBuilder = new(
                _configuration.GetConnectionString("DefaultConnection"));

            await using var source = dataSourceBuilder.Build();
            await using var cmd = new NpgsqlCommand(
                "INSERT INTO entriesservice.entries.entries " +
                "(title, amount, description, date) " +
                "VALUES ($1, $2, $3, $4)",
                await source.OpenConnectionAsync()
            )
            {
                Parameters =
                {
                    new() { Value = command.Title },
                    new() { Value = command.Amount },
                    new() { Value = command.Description },
                    new() { Value = command.Date }
                }
            };

            return await cmd.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Log.Error("Database error: {ex}", ex);

            throw new Exception("Database error");
        }
    }
}