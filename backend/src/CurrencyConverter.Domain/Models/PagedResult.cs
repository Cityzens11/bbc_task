using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Domain.Models;

[ExcludeFromCodeCoverage]
public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
