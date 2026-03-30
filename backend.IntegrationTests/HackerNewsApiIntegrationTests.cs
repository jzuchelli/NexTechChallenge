using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Backend.IntegrationTests;

public sealed class HackerNewsApiIntegrationTests
{
    private static readonly Uri DefaultBaseAddress = new("http://localhost:5129");

    [Fact]
    public async Task GetNewestStories_ReturnsStories()
    {
        using var client = new HttpClient { BaseAddress = GetBaseAddress() };

        var response = await client.GetAsync("/api/hackernews/newest?count=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<StoryDto>>();

        Assert.NotNull(payload);
        Assert.InRange(payload.Count, 0, 5);
    }

    [Fact]
    public async Task GetNewestStories_RejectsInvalidCount()
    {
        using var client = new HttpClient { BaseAddress = GetBaseAddress() };

        var response = await client.GetAsync("/api/hackernews/newest?count=0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static Uri GetBaseAddress()
    {
        var env = Environment.GetEnvironmentVariable("HN_BACKEND_BASE_URL");
        return string.IsNullOrWhiteSpace(env) ? DefaultBaseAddress : new Uri(env);
    }

    private sealed record StoryDto(long Id, string? Title, string? By, string? Url, long Time, int? Score);
}
