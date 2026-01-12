using AppCore.GetEntries;

using Microsoft.Extensions.Configuration;

using Npgsql;

using Serilog;

namespace Infra;

public class GetEntriesRepository : IGetEntriesRepository
{
    private readonly IConfiguration _configuration;

    public GetEntriesRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<GetEntriesResponse> ExecuteAsync(Guid? id)
    {
        try
        {
            NpgsqlDataSourceBuilder dataSourceBuilder = new(
                _configuration.GetConnectionString("DefaultConnection"));

            await using var source = dataSourceBuilder.Build();
            await using var cmd = new NpgsqlCommand(
                "SELECT id, title, amount, description, date " +
                "FROM entries WHERE id = ($1)",
                await source.OpenConnectionAsync()
            )
            {
                Parameters =
                {
                    new() { Value = id }
                }
            };

            var response = new GetEntriesResponse();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                response.Id = reader.GetGuid(0);
                response.Title = reader.GetString(1);
                response.Amount = reader.GetDecimal(2);
                response.Description = reader.GetString(3);
                response.Date = reader.GetDateTime(4);
            }

            return response;
        }
        catch (Exception ex)
        {
            Log.Error("Database error: {ex}", ex);
            throw new Exception("Database error.", ex);
        }
    }

    public async Task<List<GetEntriesResponse>> ExecuteAsync()
    {
        try
        {
            NpgsqlDataSourceBuilder dataSourceBuilder = new(
                _configuration.GetConnectionString("DefaultConnection"));

            await using var source = dataSourceBuilder.Build();
            await using var cmd = new NpgsqlCommand(
                "SELECT id, title, amount, description, date " +
                "FROM entries",
                await source.OpenConnectionAsync()
            );

            var responses = new List<GetEntriesResponse>();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var response = new GetEntriesResponse
                {
                    Id = reader.GetGuid(0),
                    Title = reader.GetString(1),
                    Amount = reader.GetDecimal(2),
                    Description = reader.GetString(3),
                    Date = reader.GetDateTime(4)
                };

                responses.Add(response);
            }

            return responses;
        }
        catch (Exception ex)
        {
            Log.Error("Database error: {ex}", ex);
            throw new Exception("Database error.", ex);
        }
    }
}