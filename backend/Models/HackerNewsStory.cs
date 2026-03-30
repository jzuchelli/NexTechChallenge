using System.Diagnostics.CodeAnalysis;

namespace HackerNewsAggregator.Models;

[ExcludeFromCodeCoverage]
public sealed record HackerNewsStory(
    long Id,
    string? Title,
    string? By,
    string? Url,
    long Time,
    int? Score);
