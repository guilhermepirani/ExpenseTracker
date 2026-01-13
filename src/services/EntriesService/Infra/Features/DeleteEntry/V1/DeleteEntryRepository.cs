using AppCore.Features.DeleteEntry.V1;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Serilog;

namespace Infra.Features.DeleteEntry.V1;

public class DeleteEntryRespository : IDeleteEntryRepository
{
    private readonly IConfiguration _configuration;

    public DeleteEntryRespository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int> ExecuteAsync(Guid id)
    {
        try
        {
            NpgsqlDataSourceBuilder dataSourceBuilder = new(
                _configuration.GetConnectionString("DefaultConnection"));

            await using var source = dataSourceBuilder.Build();
            await using var cmd = new NpgsqlCommand(
                "DELETE FROM entries WHERE id = ($1)",
                await source.OpenConnectionAsync())
            {
                Parameters =
                {
                    new() { Value = id }
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