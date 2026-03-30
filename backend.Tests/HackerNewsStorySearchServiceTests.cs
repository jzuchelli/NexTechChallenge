using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HackerNewsAggregator.Models;
using HackerNewsAggregator.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Backend.Tests;

public sealed class HackerNewsStorySearchServiceTests
{
    private static HackerNewsStorySearchService CreateService(
        IMemoryCache cache,
        Mock<IHackerNewsClient> client)
    {
        return new HackerNewsStorySearchService(cache, client.Object);
    }

    [Fact]
    public async Task SearchNewestStoriesAsync_CachesNormalizedResults()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new Mock<IHackerNewsClient>();
        client.Setup(c => c.GetNewStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new long[] { 1 });
        client.Setup(c => c.GetItemAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HackerNewsItem { Id = 1, Title = "Story", Type = "story" });

        var service = CreateService(cache, client);

        await service.SearchNewestStoriesAsync(null, 1, 10, CancellationToken.None);
        await service.SearchNewestStoriesAsync(null, 1, 10, CancellationToken.None);

        client.Verify(c => c.GetNewStoryIdsAsync(It.IsAny<CancellationToken>()), Times.Once);
        client.Verify(c => c.GetItemAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchNewestStoriesAsync_FiltersInvalidItems()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new Mock<IHackerNewsClient>();
        client.Setup(c => c.GetNewStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new long[] { 1, 2, 3, 4 });
        client.Setup(c => c.GetItemAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HackerNewsItem { Id = 1, Title = "Valid", Type = "story" });
        client.Setup(c => c.GetItemAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HackerNewsItem { Id = 2, Title = "Job", Type = "job" });
        client.Setup(c => c.GetItemAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HackerNewsItem { Id = 3, Title = null, Type = "story" });
        client.Setup(c => c.GetItemAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync((HackerNewsItem?)null);

        var service = CreateService(cache, client);

        var result = await service.SearchNewestStoriesAsync(null, 1, 10, CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Items[0].Id);
    }

    [Fact]
    public async Task SearchNewestStoriesAsync_FiltersByQuery()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new Mock<IHackerNewsClient>();
        client.Setup(c => c.GetNewStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new long[] { 1, 2 });
        client.Setup(c => c.GetItemAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HackerNewsItem { Id = 1, Title = "Hello World", By = "Ada", Url = "https://example.com", Type = "story" });
        client.Setup(c => c.GetItemAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HackerNewsItem { Id = 2, Title = "Other", By = "Bob", Url = "https://other.com", Type = "story" });

        var service = CreateService(cache, client);

        var result = await service.SearchNewestStoriesAsync("HELLO", 1, 10, CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(1, result.Items[0].Id);
    }

    [Fact]
    public async Task SearchNewestStoriesAsync_PagesResults()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new Mock<IHackerNewsClient>();
        client.Setup(c => c.GetNewStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new long[] { 1, 2, 3 });
        client.Setup(c => c.GetItemAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long id, CancellationToken _) => new HackerNewsItem { Id = id, Title = $"Story {id}", Type = "story" });

        var service = CreateService(cache, client);

        var result = await service.SearchNewestStoriesAsync(null, 2, 1, CancellationToken.None);

        Assert.Equal(3, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal(2, result.Page);
        Assert.Equal(1, result.PageSize);
        Assert.Equal(2, result.Items[0].Id);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public async Task SearchNewestStoriesAsync_ThrowsOnInvalidInput(int page, int pageSize)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new Mock<IHackerNewsClient>();
        var service = CreateService(cache, client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            service.SearchNewestStoriesAsync(null, page, pageSize, CancellationToken.None));
    }

    [Fact]
    public async Task SearchNewestStoriesAsync_TakesNewest100Ids()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new Mock<IHackerNewsClient>();
        var ids = Enumerable.Range(1, 150).Select(i => (long)i).ToArray();
        client.Setup(c => c.GetNewStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ids);
        client.Setup(c => c.GetItemAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long id, CancellationToken _) => new HackerNewsItem { Id = id, Title = $"Story {id}", Type = "story" });

        var service = CreateService(cache, client);

        var result = await service.SearchNewestStoriesAsync(null, 1, 100, CancellationToken.None);

        Assert.Equal(100, result.TotalCount);
        client.Verify(c => c.GetItemAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Exactly(100));
    }
}
