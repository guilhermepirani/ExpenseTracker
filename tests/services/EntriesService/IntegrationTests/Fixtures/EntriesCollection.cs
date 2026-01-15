namespace EntriesService.IntegrationTests.Fixtures;

[CollectionDefinition("EntriesCollection")]
public class EntriesCollection
    : ICollectionFixture<PostgreSqlFixture>
{ }