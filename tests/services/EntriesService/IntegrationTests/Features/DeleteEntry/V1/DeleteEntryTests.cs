using System.Net;
using System.Net.Http.Json;
using AppCore;
using AppCore.Features.DeleteEntry.V1;
using AppCore.Features.GetEntries.V1;
using EntriesService.IntegrationTests.Fixtures;
using EntriesService.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EntriesService.IntegrationTests.Features.DeleteEntry.V1;

[Collection("EntriesCollection")]
public class DeleteEntryTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly HttpClient _httpClient;

    public DeleteEntryTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateClient();
    }

    [Fact]
    public async Task WhenGivenValidId_ShouldDelete1Entry()
    {
        var id = "019baf94-4918-4760-8c9a-de13c64d9069";
        var response = await _httpClient
            .DeleteFromJsonAsync<Result<DeleteEntryResponse>>(
                $"api/v1/entries/{id}",
                TestContext.Current.CancellationToken);

        CommonAssertionsForSuccess(response!);

        var entriesLeft = await _httpClient
            .GetFromJsonAsync<Result<List<GetEntriesResponse>>>(
                $"api/v1/entries",
                TestContext.Current.CancellationToken);

        entriesLeft?.Data.Should().HaveCount(3);
    }

    [Fact]
    public async Task WhenGivenValidId_ShouldReturn1RowsAffected()
    {
        var id = "019baf94-4918-4760-8c9a-de13c64d9069";
        var expectedResponse = new DeleteEntryResponse() { RowsAffected = 1 };

        var response = await _httpClient
            .DeleteFromJsonAsync<Result<DeleteEntryResponse>>(
                $"api/v1/entries/{id}",
                TestContext.Current.CancellationToken);

        CommonAssertionsForSuccess(response!);
        response?.Data.Should().BeEquivalentTo(expectedResponse);
    }

    // TODO: refactor to return bad request
    [Fact]
    public async Task WhenGivenInvalidId_ShouldReturnInternalServerError()
    {
        string invalidId = "invalid-uuid-format";

        var request = new HttpRequestMessage(
            HttpMethod.Delete, $"api/v1/entries/{invalidId}");
        var response = await _httpClient.SendAsync(
            request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var errorContent = await response.Content
            .ReadFromJsonAsync<Result<DeleteEntryResponse>>(
                TestContext.Current.CancellationToken);

        CommonAssertionsForFailure(errorContent!);
        errorContent?.Errors.Should().ContainSingle().Which
            .Should().Be("Failed to parse provided ID to UUID");
    }

    [Fact]
    public async Task WhenGivenNonExistentId_ShouldReturn0RowsAffected()
    {
        var id = "019baf94-4918-4760-8c9a-de13c64d9060";
        var expectedResponse = new DeleteEntryResponse() { RowsAffected = 0 };

        var response = await _httpClient
            .DeleteFromJsonAsync<Result<DeleteEntryResponse>>(
                $"api/v1/entries/{id}",
                TestContext.Current.CancellationToken);

        CommonAssertionsForSuccess(response!);
        response?.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task WhenGivenNonExistentId_ShouldDelete0Entries()
    {
        var id = "019baf94-4918-4760-8c9a-de13c64d9060";
        var response = await _httpClient
            .DeleteFromJsonAsync<Result<DeleteEntryResponse>>(
                $"api/v1/entries/{id}",
                TestContext.Current.CancellationToken);

        CommonAssertionsForSuccess(response!);

        var entriesLeft = await _httpClient
            .GetFromJsonAsync<Result<List<GetEntriesResponse>>>(
                $"api/v1/entries",
                TestContext.Current.CancellationToken);

        entriesLeft?.Data.Should().HaveCount(4);
    }


    private static void CommonAssertionsForSuccess(
        Result<DeleteEntryResponse> response)
    {
        response?.StatusCode.Should().Be(HttpStatusCode.OK);
        response?.IsSuccess.Should().BeTrue();
        response?.Errors.Should().BeNull();
    }

    private static void CommonAssertionsForFailure(
        Result<DeleteEntryResponse> response)
    {
        response?.IsSuccess.Should().BeFalse();
        response?.Data.Should().BeNull();
        response?.Errors.Should().NotBeNullOrEmpty();
        response?.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    public async ValueTask InitializeAsync() =>
        await DbHelper.InitDbForTests(_fixture);

    public async ValueTask DisposeAsync() =>
        await Task.CompletedTask;
}