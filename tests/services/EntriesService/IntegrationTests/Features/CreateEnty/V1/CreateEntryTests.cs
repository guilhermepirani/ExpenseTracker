using System.Net;
using System.Net.Http.Json;
using AppCore;
using AppCore.Features.CreateEntry.V1;
using AppCore.Features.GetEntries.V1;
using EntriesService.IntegrationTests.Fixtures;
using EntriesService.IntegrationTests.Helpers;
using FluentAssertions;

namespace EntriesService.IntegrationTests.Features.CreateEnty.V1;

[Collection("EntriesCollection")]
public class CreateEntryTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly HttpClient _httpClient;

    public CreateEntryTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
        _httpClient = _fixture.CreateClient();
    }

    private readonly CreateEntryCommand _validCommand = new()
    {
        Title = "TestEntryTitle",
        Amount = 10,
        Description = "TestEntryDescription",
        Date = DateTime.Parse("2026-01-01")
    };

    [Fact]
    public async Task WhenGivenValidCommand_ReturnsCreatedId()
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/entries",
            _validCommand,
            TestContext.Current.CancellationToken);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CreateEntryResponse>>(
                TestContext.Current.CancellationToken);

        result?.Data.Should().NotBeNull();
        result?.Data.Should().BeAssignableTo(typeof(CreateEntryResponse));
        result?.StatusCode.Should().Be(HttpStatusCode.Created);
        result?.IsSuccess.Should().BeTrue();
        result?.Errors.Should().BeNull();
        result?.Data?.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task WhenGivenValidCommand_Creates1Entry()
    {
        await _httpClient.PostAsJsonAsync(
            "api/v1/entries",
            _validCommand,
            TestContext.Current.CancellationToken);

        var entries = await _httpClient
            .GetFromJsonAsync<Result<List<GetEntriesResponse>>>(
                "api/v1/entries",
                TestContext.Current.CancellationToken);

        entries?.Data.Should().HaveCount(5);
    }

    [Fact]
    public async Task WhenGivenValidCommand_ReturnsLocationHeader()
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/entries",
            _validCommand,
            TestContext.Current.CancellationToken);

        response.Headers.Should().ContainKey("Location");
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location.Should().BeAssignableTo<Uri>();
    }

    [Fact]
    public async Task WhenGivenInvalidCommand_ReturnsBadRequest()
    {
        var invalidCommand = _validCommand;
        invalidCommand.Amount = 0;
        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/entries",
            invalidCommand,
            TestContext.Current.CancellationToken);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CreateEntryResponse>>(
                TestContext.Current.CancellationToken);

        result?.IsSuccess.Should().BeFalse();
        result?.Data.Should().BeNull();
        result?.Errors.Should().NotBeNullOrEmpty();
        result?.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenGivenInvalidCommand_ReturnsAllErrors()
    {
        var invalidCommand = new CreateEntryCommand
        {
            Title = "",
            Amount = 0,
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/v1/entries",
            invalidCommand,
            TestContext.Current.CancellationToken);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CreateEntryResponse>>(
                TestContext.Current.CancellationToken);

        result?.Errors.Should().HaveCount(2);
    }

    public async ValueTask InitializeAsync() =>
        await DbHelper.InitDbForTests(_fixture);

    public async ValueTask DisposeAsync() =>
        await Task.CompletedTask;
}