using System.Net;
using System.Net.Http.Json;
using AppCore;
using AppCore.Features.GetEntries.V1;
using EntriesService.IntegrationTests.Fixtures;
using EntriesService.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EntriesService.IntegrationTests.Features.GetEntries.V1;

[Collection("EntriesCollection")]
public class GetEntriesTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;
    private readonly HttpClient _httpClient;

    public GetEntriesTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateClient(
            new WebApplicationFactoryClientOptions()
            { BaseAddress = new Uri("http://localhost:5199") });
    }

    [Fact]
    public async Task WhenNotGivenId_ShouldReturn4Entries()
    {
        var response = await _httpClient
            .GetFromJsonAsync<Result<List<GetEntriesResponse>>>(
                "api/v1/entries",
                TestContext.Current.CancellationToken);

        CommonAssertionsForSuccess(response!);
        response?.Data.Should().BeOfType<List<GetEntriesResponse>>()
            .And.HaveCount(4);
    }

    [Fact]
    public async Task WhenGivenValidId_ShouldReturn1Entry()
    {
        string id = "019baf94-4918-4760-8c9a-de13c64d9069";
        var response = await _httpClient
            .GetFromJsonAsync<Result<List<GetEntriesResponse>>>(
                $"api/v1/entries/{id}",
                TestContext.Current.CancellationToken);

        CommonAssertionsForSuccess(response!);
        response?.Data.Should().BeOfType<List<GetEntriesResponse>>()
            .And.HaveCount(1);
    }

    [Fact]
    public async Task WhenGivenNonExistentId_ShouldReturn0Entries()
    {
        string id = "019baf94-4918-4760-8c9a-de13c64d9060";
        var response = await _httpClient
            .GetFromJsonAsync<Result<List<GetEntriesResponse>>>(
                $"api/v1/entries/{id}",
                TestContext.Current.CancellationToken);

        CommonAssertionsForSuccess(response!);
        response?.Data.Should().BeOfType<List<GetEntriesResponse>>()
            .And.HaveCount(0);
    }

    [Fact]
    public async Task WhenGivenInvalidId_ShouldReturnBadRequest()
    {
        string invalidId = "invalid-uuid-format";

        var request = new HttpRequestMessage(
            HttpMethod.Get, $"api/v1/entries/{invalidId}");
        var response = await _httpClient.SendAsync(
            request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errorContent = await response.Content
            .ReadFromJsonAsync<Result<List<GetEntriesResponse>>>(
                TestContext.Current.CancellationToken);

        CommonAssertionsForFailure(errorContent!);
        errorContent!.Errors.Should().ContainSingle().Which
            .Should().Be("If you pass an ID it must be of type UUID.");
    }

    private static void CommonAssertionsForSuccess(
        Result<List<GetEntriesResponse>> response)
    {
        response?.StatusCode.Should().Be(HttpStatusCode.OK);
        response?.IsSuccess.Should().BeTrue();
        response?.Errors.Should().BeNull();
    }

    private static void CommonAssertionsForFailure(
        Result<List<GetEntriesResponse>> response)
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