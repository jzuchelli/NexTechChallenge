using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HackerNewsAggregator.Controllers;
using HackerNewsAggregator.Models;
using HackerNewsAggregator.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Backend.Tests;

public sealed class HackerNewsControllerTests
{
    [Fact]
    public async Task GetNewest_ReturnsOkWithStories()
    {
        var service = new Mock<IHackerNewsService>();
        var expected = new List<HackerNewsStory>
        {
            new(1, "Story", "author", "https://example.com", 12345, 10)
        };
        service.Setup(s => s.GetNewestStoriesAsync(20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var controller = new HackerNewsController(service.Object);

        var result = await controller.GetNewest(20, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IReadOnlyList<HackerNewsStory>>(okResult.Value);
        Assert.Single(payload);
        Assert.Equal(1, payload[0].Id);
    }
}
