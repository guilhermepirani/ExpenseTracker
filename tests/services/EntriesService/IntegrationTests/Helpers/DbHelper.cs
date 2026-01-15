using EntriesService.IntegrationTests.Fixtures;

namespace EntriesService.IntegrationTests.Helpers;

public static class DbHelper
{
    public static async Task InitDbForTests(PostgreSqlFixture fixture)
    {
        await ResetEntriesTable(fixture);
        await PopulateDbForTests(fixture);
    }

    private static async Task ResetEntriesTable(PostgreSqlFixture fixture)
    {
        await using var connection = fixture.GetConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = ResetEntriesTableSql();
        await command.Connection?.OpenAsync()!;
        await command.ExecuteNonQueryAsync();
    }

    private static async Task PopulateDbForTests(
        PostgreSqlFixture fixture)
    {
        await using var connection = fixture.GetConnection();
        await using var command = connection.CreateCommand();
        command.CommandText = GetCreateEntriesSql();
        await command.Connection?.OpenAsync()!;
        await command.ExecuteNonQueryAsync();
    }

    private static string GetCreateEntriesSql()
    {
        return
            "INSERT INTO entries (id, title, amount, description, date) VALUES " +
            "('019baf94-4918-4760-8c9a-de13c64d9069', 'TestEntry1', 10, 'Description1', '2026-01-12 00:21:29.496669'), " +
            "('019bb2d4-7723-4fb8-81d5-7c60432d3eb9', 'TestEntry2', 20, 'Description2', '2026-01-12 19:48:59.981073'), " +
            "('019bb3b3-1283-77bf-9817-c4f14903a62c', 'TestEntry3', 30, 'Description3', '2026-01-12 19:33:35.986304'), " +
            "('019bb3c1-2bfc-73d9-a437-4247e2d93571', 'TestEntry4', 40, 'Description4', '2026-01-12 15:30:27.185208'); ";
    }

    private static string ResetEntriesTableSql()
    {
        return
            "DROP TABLE IF EXISTS entries CASCADE; " +
            "CREATE TABLE IF NOT EXISTS entries ( " +
            "id uuid NOT NULL PRIMARY KEY, " +
            "title character varying(50) NOT NULL, " +
            "amount numeric NOT NULL, " +
            "description character varying(500) NULL, " +
            "date timestamp without time zone NOT NULL DEFAULT now()); ";
    }
}