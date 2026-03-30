using HackerNewsAggregator.Models;

namespace HackerNewsAggregator.Services;

public interface IHackerNewsClient
{
    Task<IReadOnlyList<long>> GetNewStoryIdsAsync(CancellationToken cancellationToken);
    Task<HackerNewsItem?> GetItemAsync(long id, CancellationToken cancellationToken);
}
