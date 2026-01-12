using AppCore;
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

    public async Task<int> ExecuteAsync(Entry entry)
    {
        try
        {
            NpgsqlDataSourceBuilder dataSourceBuilder = new(
                _configuration.GetConnectionString("DefaultConnection"));

            await using var source = dataSourceBuilder.Build();
            await using var cmd = new NpgsqlCommand(
                "INSERT INTO entries " +
                "(id, title, amount, description, date) " +
                "VALUES ($1, $2, $3, $4, $5)",
                await source.OpenConnectionAsync()
            )
            {
                Parameters =
                {
                    new() { Value = entry.Id },
                    new() { Value = entry.Title },
                    new() { Value = entry.Amount },
                    new() { Value = entry.Description },
                    new() { Value = entry.Date }
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