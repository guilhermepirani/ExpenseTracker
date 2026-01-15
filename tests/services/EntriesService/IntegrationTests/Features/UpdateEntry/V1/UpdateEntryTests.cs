using System.Net;
using System.Net.Http.Json;
using AppCore;
using AppCore.Features.UpdateEntry.V1;
using EntriesService.IntegrationTests.Fixtures;
using EntriesService.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Org.BouncyCastle.Asn1.Misc;

namespace EntriesService.IntegrationTests.Features.UpdateEntry.V1;

[Collection("EntriesCollection")]
public class UpdateEntryTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly HttpClient _httpClient;

    public UpdateEntryTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
        _httpClient = _fixture.CreateClient();
    }

    private readonly UpdateEntryCommand _validCommand = new()
    {
        Id = new Guid("019baf94-4918-4760-8c9a-de13c64d9069"),
        Title = "TestUpdateEntryTitle",
        Amount = 10,
        Description = "TestUpdateEntryDescription",
        Date = DateTime.Parse("2026-01-01")
    };

    [Fact]
    public async Task WhenGivenValidCommand_Returns1RowsAffected()
    {
        var expectedResponse = new UpdateEntryResponse() { RowsAffected = 1 };
        var response = await _httpClient.PutAsJsonAsync(
            "api/v1/entries",
            _validCommand,
            TestContext.Current.CancellationToken);

        var result = await response.Content
            .ReadFromJsonAsync<Result<UpdateEntryResponse>>(
                TestContext.Current.CancellationToken);

        result?.Errors.Should().BeNull();
        result?.IsSuccess.Should().BeTrue();
        result?.StatusCode.Should().Be(HttpStatusCode.OK);
        result?.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task WhenGivenInvalidCommand_ReturnsBadRequest()
    {
        var invalidCommand = _validCommand;
        invalidCommand.Amount = 0;

        var response = await _httpClient.PutAsJsonAsync(
            "api/v1/entries",
            _validCommand,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenGivenInvalidCommand_ReturnsAllErrors()
    {
        var invalidCommand = new UpdateEntryCommand()
        {
            Id = _validCommand.Id,
            Title = "",
            Amount = 0
        };

        var response = await _httpClient.PutAsJsonAsync(
            "api/v1/entries",
            invalidCommand,
            TestContext.Current.CancellationToken);

        var result = await response.Content
            .ReadFromJsonAsync<Result<UpdateEntryResponse>>(
                TestContext.Current.CancellationToken);

        result!.Errors.Should().HaveCount(2);
    }

    [Fact]
    public async Task WhenGivenNonExistentId_ReturnsNotFound()
    {
        var id = new Guid("019baf94-4918-4760-8c9a-de13c64d9060");

        var response = await _httpClient.PutAsJsonAsync(
            "api/v1/entries",
            new UpdateEntryCommand { Id = id },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenGivenInvalidId_ReturnsBadRequest()
    {
        var response = await _httpClient.PutAsJsonAsync(
            "api/v1/entries",
            new { Id = "invalid uuid" },
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public async ValueTask InitializeAsync() =>
        await DbHelper.InitDbForTests(_fixture);

    public async ValueTask DisposeAsync() =>
        await Task.CompletedTask;
}