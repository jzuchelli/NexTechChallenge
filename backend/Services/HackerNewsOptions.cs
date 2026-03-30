namespace HackerNewsAggregator.Services;

public sealed class HackerNewsOptions
{
    public int NewStoryIdsTtlSeconds { get; set; } = 60;
    public int ItemTtlSeconds { get; set; } = 300;
}
