using HackerNewsAggregator.Models;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNewsAggregator.Services;

public sealed class HackerNewsStorySearchService : IHackerNewsStorySearchService
{
    private const int MaxStories = 100;
    private const int MaxPageSize = 100;
    private const int MinPageSize = 1;
    private const string CacheKey = "hn:newest:normalized";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private readonly IMemoryCache _cache;
    private readonly IHackerNewsClient _client;

    public HackerNewsStorySearchService(IMemoryCache cache, IHackerNewsClient client)
    {
        _cache = cache;
        _client = client;
    }

    public async Task<PagedResult<HackerNewsStory>> SearchNewestStoriesAsync(
        string? query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        if (page < MinPageSize)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be >= 1.");
        }

        if (pageSize is < MinPageSize or > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), $"PageSize must be between {MinPageSize} and {MaxPageSize}.");
        }

        var normalizedStories = await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheTtl;
            return await FetchNormalizedStoriesAsync(cancellationToken);
        }) ?? new List<NormalizedStory>();

        IEnumerable<NormalizedStory> filtered = normalizedStories;
        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim().ToLowerInvariant();
            filtered = filtered.Where(story => story.SearchText.Contains(term, StringComparison.Ordinal));
        }

        var filteredList = filtered.ToList();
        var total = filteredList.Count;
        var items = filteredList
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(story => story.Story)
            .ToList();

        return new PagedResult<HackerNewsStory>(page, pageSize, total, items);
    }

    private async Task<List<NormalizedStory>> FetchNormalizedStoriesAsync(CancellationToken cancellationToken)
    {
        var ids = await _client.GetNewStoryIdsAsync(cancellationToken);
        var selectedIds = ids.Take(MaxStories).ToArray();

        var tasks = selectedIds.Select(id => _client.GetItemAsync(id, cancellationToken));
        var items = await Task.WhenAll(tasks);

        var results = new List<NormalizedStory>(items.Length);
        results.AddRange(from item in items.OfType<HackerNewsItem>() 
            where string.Equals(item.Type, "story", StringComparison.OrdinalIgnoreCase) 
            where !string.IsNullOrWhiteSpace(item.Title) 
            let story = new HackerNewsStory(item.Id, item.Title, item.By, item.Url, item.Time, item.Score) 
            select new NormalizedStory(story, BuildSearchText(item)));

        return results;
    }

    private static string BuildSearchText(HackerNewsItem item)
    {
        var title = item.Title?.ToLowerInvariant() ?? string.Empty;
        var by = item.By?.ToLowerInvariant() ?? string.Empty;
        var url = item.Url?.ToLowerInvariant() ?? string.Empty;
        return string.Concat(title, " ", by, " ", url);
    }

    private sealed record NormalizedStory(HackerNewsStory Story, string SearchText);
}
