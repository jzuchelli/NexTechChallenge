using HackerNewsAggregator.Models;

namespace HackerNewsAggregator.Services;

public sealed class HackerNewsClient : IHackerNewsClient
{
    private readonly HttpClient _httpClient;

    public HackerNewsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<long>> GetNewStoryIdsAsync(CancellationToken cancellationToken)
    {
        var ids = await _httpClient.GetFromJsonAsync<long[]>("newstories.json", cancellationToken);
        return ids ?? Array.Empty<long>();
    }

    public Task<HackerNewsItem?> GetItemAsync(long id, CancellationToken cancellationToken)
    {
        return _httpClient.GetFromJsonAsync<HackerNewsItem>($"item/{id}.json", cancellationToken);
    }
}
