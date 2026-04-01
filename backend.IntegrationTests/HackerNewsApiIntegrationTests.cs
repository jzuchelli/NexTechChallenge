using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Backend.IntegrationTests;

[Collection("backend")]
public sealed class HackerNewsApiIntegrationTests
{
    private readonly BackendApiFixture _fixture;

    public HackerNewsApiIntegrationTests(BackendApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetNewestStories_ReturnsStories()
    {
        using var client = new HttpClient { BaseAddress = _fixture.BaseAddress };

        var response = await client.GetAsync("/api/hackernews/newest?count=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<StoryDto>>();

        Assert.NotNull(payload);
        Assert.InRange(payload.Count, 0, 5);
    }

    [Fact]
    public async Task GetNewestStories_RejectsInvalidCount()
    {
        using var client = new HttpClient { BaseAddress = _fixture.BaseAddress };

        var response = await client.GetAsync("/api/hackernews/newest?count=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private sealed record StoryDto(long Id, string? Title, string? By, string? Url, long Time, int? Score);
}
