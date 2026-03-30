using System.ComponentModel.DataAnnotations;
using HackerNewsAggregator.Models;
using HackerNewsAggregator.Services;

namespace HackerNewsAggregator.Controllers;

[ApiController]
[Route("api/hackernews")]
public sealed class HackerNewsController : ControllerBase
{
    private readonly IHackerNewsService _service;

    public HackerNewsController(IHackerNewsService service)
    {
        _service = service;
    }

    [HttpGet("newest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<HackerNewsStory>>> GetNewest(
        [FromQuery, Range(1, 100)] int count = 20,
        CancellationToken cancellationToken = default)
    {
        var stories = await _service.GetNewestStoriesAsync(count, cancellationToken);
        return Ok(stories);
    }
}
