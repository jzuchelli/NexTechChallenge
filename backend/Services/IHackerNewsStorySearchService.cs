using HackerNewsAggregator.Models;

namespace HackerNewsAggregator.Services;

public interface IHackerNewsStorySearchService
{
    Task<PagedResult<HackerNewsStory>> SearchNewestStoriesAsync(
        string? query,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
