using System.Diagnostics.CodeAnalysis;

namespace HackerNewsAggregator.Models;

[ExcludeFromCodeCoverage]
public sealed record PagedResult<T>(
    int Page,
    int PageSize,
    int TotalCount,
    IReadOnlyList<T> Items);
