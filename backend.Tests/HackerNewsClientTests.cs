using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HackerNewsAggregator.Models;
using HackerNewsAggregator.Services;

namespace Backend.Tests;

public sealed class HackerNewsClientTests
{
    [Fact]
    public async Task GetNewStoryIdsAsync_ReturnsIds()
    {
        var handler = new StubMessageHandler(request =>
        {
            Assert.EndsWith("newstories.json", request.RequestUri?.ToString());
            var content = JsonContent.Create(new long[] { 1, 2, 3 });
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
        };

        var client = new HackerNewsClient(httpClient);
        var ids = await client.GetNewStoryIdsAsync(CancellationToken.None);

        Assert.Equal(new long[] { 1, 2, 3 }, ids);
    }

    [Fact]
    public async Task GetItemAsync_ReturnsItem()
    {
        var handler = new StubMessageHandler(request =>
        {
            Assert.Contains("item/42.json", request.RequestUri?.ToString());
            var item = new HackerNewsItem { Id = 42, Title = "Story", Type = "story" };
            var content = JsonContent.Create(item, options: new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
        });

        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
        };

        var client = new HackerNewsClient(httpClient);
        var itemResult = await client.GetItemAsync(42, CancellationToken.None);

        Assert.NotNull(itemResult);
        Assert.Equal(42, itemResult.Id);
        Assert.Equal("Story", itemResult.Title);
    }

    private sealed class StubMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public StubMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }
}
