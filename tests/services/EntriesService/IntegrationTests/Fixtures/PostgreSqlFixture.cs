using System.Data.Common;
using EntriesService.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Npgsql;
using Testcontainers.PostgreSql;

namespace EntriesService.IntegrationTests.Fixtures;

public class PostgreSqlFixture
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer =
        new PostgreSqlBuilder("postgres:18.1-alpine3.23")
            .WithPortBinding(5433, 5432)
            .Build();

    public async ValueTask InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(
            _postgreSqlContainer.GetConnectionString());
    }

    protected override async void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var settings = new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] =
                 _postgreSqlContainer.GetConnectionString()
            };
            config.AddInMemoryCollection(settings!);
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync().AsTask();
    }
}