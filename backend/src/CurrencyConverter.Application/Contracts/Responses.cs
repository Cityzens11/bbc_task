using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Application.Contracts;

[ExcludeFromCodeCoverage]
public sealed record LatestRatesResponse(DateOnly Date, string BaseCurrency, IReadOnlyDictionary<string, decimal> Rates);

[ExcludeFromCodeCoverage]
public sealed record ConversionResponse(
    decimal Amount,
    string FromCurrency,
    string ToCurrency,
    decimal Rate,
    decimal ConvertedAmount,
    DateOnly RateDate);

[ExcludeFromCodeCoverage]
public sealed record HistoricalRateItem(DateOnly Date, IReadOnlyDictionary<string, decimal> Rates);

[ExcludeFromCodeCoverage]
public sealed record HistoricalRatesResponse(
    string BaseCurrency,
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyCollection<HistoricalRateItem> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
