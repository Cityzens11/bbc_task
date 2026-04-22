using CurrencyConverter.Application.Contracts;

namespace CurrencyConverter.Application.Abstractions;

public interface ICurrencyService
{
    Task<LatestRatesResponse> GetLatestAsync(string baseCurrency, CancellationToken cancellationToken = default);

    Task<ConversionResponse> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, CancellationToken cancellationToken = default);

    Task<HistoricalRatesResponse> GetHistoricalAsync(
        string baseCurrency,
        DateOnly startDate,
        DateOnly endDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
