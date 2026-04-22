using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Api.Contracts.Requests;

[ExcludeFromCodeCoverage]
public sealed record HistoricalRequest(
    string BaseCurrency,
    DateOnly StartDate,
    DateOnly EndDate,
    int Page = 1,
    int PageSize = 20);
