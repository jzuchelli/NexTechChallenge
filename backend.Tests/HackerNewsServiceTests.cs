using System;
using System.Threading;
using System.Threading.Tasks;
using HackerNewsAggregator.Models;
using HackerNewsAggregator.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;

namespace Backend.Tests;

public sealed class HackerNewsServiceTests
{
    private static HackerNewsService CreateService(
        IMemoryCache cache,
        Mock<IHackerNewsClient> client,
        HackerNewsOptions? options = null)
    {
        var optionsWrapper = Options.Create(options ?? new HackerNewsOptions());
        return new HackerNewsService(cache, client.Object, optionsWrapper);
    }

    [Fact]
    public async Task GetNewestStoriesAsync_UsesCacheForIds()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new Mock<IHackerNewsClient>();
        client.Setup(c => c.GetNewStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new long[] { 1, 2 });
        client.Setup(c => c.GetItemAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HackerNewsItem { Id = 1, Type = "story" });

        var service = CreateService(cache, client);

        await service.GetNewestStoriesAsync(1, CancellationToken.None);
        await service.GetNewestStoriesAsync(1, CancellationToken.None);

        client.Verify(c => c.GetNewStoryIdsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetNewestStoriesAsync_FiltersNonStories()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new Mock<IHackerNewsClient>();
        client.Setup(c => c.GetNewStoryIdsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new long[] { 10, 11 });
        client.Setup(c => c.GetItemAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HackerNewsItem { Id = 10, Type = "story", Title = "A" });
        client.Setup(c => c.GetItemAsync(11, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HackerNewsItem { Id = 11, Type = "job", Title = "B" });

        var service = CreateService(cache, client);

        var stories = await service.GetNewestStoriesAsync(2, CancellationToken.None);

        Assert.Single(stories);
        Assert.Equal(10, stories[0].Id);
    }

    [Fact]
    public async Task GetNewestStoriesAsync_ThrowsOnInvalidCount()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var client = new Mock<IHackerNewsClient>();
        var service = CreateService(cache, client);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetNewestStoriesAsync(0, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetNewestStoriesAsync(101, CancellationToken.None));
    }
}
