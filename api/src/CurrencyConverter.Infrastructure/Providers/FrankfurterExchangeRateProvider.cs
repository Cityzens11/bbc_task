using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Domain.Models;
using CurrencyConverter.Infrastructure.External;
using CurrencyConverter.Infrastructure.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CurrencyConverter.Infrastructure.Providers;

public sealed class FrankfurterExchangeRateProvider(
    IFrankfurterApi frankfurterApi,
    IMemoryCache memoryCache,
    IOptions<ProviderOptions> providerOptions,
    ILogger<FrankfurterExchangeRateProvider> logger) : IExchangeRateProvider
{
    private readonly IFrankfurterApi _frankfurterApi = frankfurterApi;
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly FrankfurterOptions _options = providerOptions.Value.Frankfurter;
    private readonly ILogger<FrankfurterExchangeRateProvider> _logger = logger;

    public async Task<ExchangeRatesSnapshot> GetLatestAsync(string baseCurrency, CancellationToken cancellationToken = default)
    {
        var key = $"latest:{baseCurrency}";

        if (_memoryCache.TryGetValue<ExchangeRatesSnapshot>(key, out var cached) && cached is not null)
        {
            return cached;
        }

        _logger.LogInformation("Fetching latest rates from Frankfurter for base {BaseCurrency}", baseCurrency);
        var response = await _frankfurterApi.GetLatestAsync(baseCurrency, null, cancellationToken);

        var snapshot = new ExchangeRatesSnapshot(
            response.Date,
            response.Base,
            new Dictionary<string, decimal>(response.Rates, StringComparer.OrdinalIgnoreCase));

        _memoryCache.Set(key, snapshot, TimeSpan.FromSeconds(_options.LatestCacheSeconds));

        return snapshot;
    }

    public async Task<IReadOnlyCollection<ExchangeRatesSnapshot>> GetHistoricalAsync(
        string baseCurrency,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        var key = $"history:{baseCurrency}:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";

        if (_memoryCache.TryGetValue<IReadOnlyCollection<ExchangeRatesSnapshot>>(key, out var cached) && cached is not null)
        {
            return cached;
        }

        var range = $"{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}";
        _logger.LogInformation(
            "Fetching historical rates from Frankfurter for base {BaseCurrency} in range {StartDate}..{EndDate}",
            baseCurrency,
            startDate,
            endDate);

        var response = await _frankfurterApi.GetHistoricalAsync(range, baseCurrency, cancellationToken);

        var result = response.Rates
            .Select(kv => new ExchangeRatesSnapshot(
                DateOnly.Parse(kv.Key),
                response.Base,
                new Dictionary<string, decimal>(kv.Value, StringComparer.OrdinalIgnoreCase)))
            .ToList();

        _memoryCache.Set(key, result, TimeSpan.FromSeconds(_options.HistoricalCacheSeconds));

        return result;
    }
}
