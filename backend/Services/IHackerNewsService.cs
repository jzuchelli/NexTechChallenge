using HackerNewsAggregator.Models;

namespace HackerNewsAggregator.Services;

public interface IHackerNewsService
{
    Task<IReadOnlyList<HackerNewsStory>> GetNewestStoriesAsync(int count, CancellationToken cancellationToken);
}
