using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace HackerNewsAggregator.Models;

[ExcludeFromCodeCoverage]
public sealed class HackerNewsItem
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("by")]
    public string? By { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("time")]
    public long Time { get; init; }

    [JsonPropertyName("score")]
    public int? Score { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }
}
