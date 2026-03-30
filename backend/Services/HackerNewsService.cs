using HackerNewsAggregator.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HackerNewsAggregator.Services;

public sealed class HackerNewsService : IHackerNewsService
{
    private const int MinCount = 1;
    private const int MaxCount = 100;
    private const string NewStoryIdsCacheKey = "hn:newstories:ids";

    private readonly IMemoryCache _cache;
    private readonly IHackerNewsClient _client;
    private readonly HackerNewsOptions _options;

    public HackerNewsService(
        IMemoryCache cache,
        IHackerNewsClient client,
        IOptions<HackerNewsOptions> options)
    {
        _cache = cache;
        _client = client;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<HackerNewsStory>> GetNewestStoriesAsync(int count, CancellationToken cancellationToken)
    {
        if (count is < MinCount or > MaxCount)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Count must be between {MinCount} and {MaxCount}.");
        }

        var ids = await _cache.GetOrCreateAsync(NewStoryIdsCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.NewStoryIdsTtlSeconds);
            return await _client.GetNewStoryIdsAsync(cancellationToken);
        }) ?? Array.Empty<long>();

        var selectedIds = ids.Take(count).ToArray();
        var tasks = selectedIds.Select(id => GetCachedItemAsync(id, cancellationToken));
        var items = await Task.WhenAll(tasks);

        var stories = new List<HackerNewsStory>(items.Length);
        foreach (var item in items)
        {
            if (item is null)
            {
                continue;
            }

            if (!string.Equals(item.Type, "story", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            stories.Add(new HackerNewsStory(
                item.Id,
                item.Title,
                item.By,
                item.Url,
                item.Time,
                item.Score));
        }

        return stories;
    }

    private async Task<HackerNewsItem?> GetCachedItemAsync(long id, CancellationToken cancellationToken)
    {
        var cacheKey = $"hn:item:{id}";
        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.ItemTtlSeconds);
            return await _client.GetItemAsync(id, cancellationToken);
        });
    }
}
