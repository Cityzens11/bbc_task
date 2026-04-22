using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Application.Contracts;
using CurrencyConverter.Domain.Constants;
using CurrencyConverter.Domain.Exceptions;

namespace CurrencyConverter.Application.Services;

public sealed class CurrencyService(IExchangeRateProviderFactory providerFactory) : ICurrencyService
{
    private readonly IExchangeRateProviderFactory _providerFactory = providerFactory;

    public async Task<LatestRatesResponse> GetLatestAsync(string baseCurrency, CancellationToken cancellationToken = default)
    {
        var normalizedBase = NormalizeCurrency(baseCurrency);
        var provider = _providerFactory.Create();
        var snapshot = await provider.GetLatestAsync(normalizedBase, cancellationToken);
        return new LatestRatesResponse(snapshot.Date, snapshot.BaseCurrency, snapshot.Rates);
    }

    public async Task<ConversionResponse> ConvertAsync(decimal amount, string fromCurrency, string toCurrency, CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        }

        var from = NormalizeCurrency(fromCurrency);
        var to = NormalizeCurrency(toCurrency);

        ValidateExcluded(from);
        ValidateExcluded(to);

        if (from.Equals(to, StringComparison.OrdinalIgnoreCase))
        {
            return new ConversionResponse(amount, from, to, 1m, amount, DateOnly.FromDateTime(DateTime.UtcNow));
        }

        var provider = _providerFactory.Create();
        var snapshot = await provider.GetLatestAsync(from, cancellationToken);

        if (!snapshot.Rates.TryGetValue(to, out var rate))
        {
            throw new InvalidOperationException($"No exchange rate found from {from} to {to}.");
        }

        var converted = decimal.Round(amount * rate, 4, MidpointRounding.AwayFromZero);
        return new ConversionResponse(amount, from, to, rate, converted, snapshot.Date);
    }

    public async Task<HistoricalRatesResponse> GetHistoricalAsync(
        string baseCurrency,
        DateOnly startDate,
        DateOnly endDate,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedBase = NormalizeCurrency(baseCurrency);

        if (startDate > endDate)
        {
            throw new ArgumentException("Start date cannot be after end date.");
        }

        if (page <= 0)
        {
            throw new ArgumentException("Page must be greater than zero.", nameof(page));
        }

        if (pageSize is <= 0 or > 200)
        {
            throw new ArgumentException("Page size must be between 1 and 200.", nameof(pageSize));
        }

        var provider = _providerFactory.Create();
        var data = await provider.GetHistoricalAsync(normalizedBase, startDate, endDate, cancellationToken);
        var ordered = data.OrderBy(x => x.Date).ToList();

        var totalItems = ordered.Count;
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        var pagedItems = ordered.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new HistoricalRateItem(x.Date, x.Rates))
            .ToList();

        return new HistoricalRatesResponse(
            normalizedBase,
            startDate,
            endDate,
            pagedItems,
            page,
            pageSize,
            totalItems,
            totalPages);
    }

    private static string NormalizeCurrency(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Currency code is required.", nameof(value));
        }

        return value.Trim().ToUpperInvariant();
    }

    private static void ValidateExcluded(string currency)
    {
        if (ExcludedCurrencies.Values.Contains(currency))
        {
            throw new UnsupportedCurrencyException(currency);
        }
    }
}
