using CurrencyConverter.Domain.Models;

namespace CurrencyConverter.Application.Abstractions;

public interface IExchangeRateProvider
{
    Task<ExchangeRatesSnapshot> GetLatestAsync(string baseCurrency, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ExchangeRatesSnapshot>> GetHistoricalAsync(
        string baseCurrency,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}