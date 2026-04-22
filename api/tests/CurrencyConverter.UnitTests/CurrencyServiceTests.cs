using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Application.Services;
using CurrencyConverter.Domain.Exceptions;
using CurrencyConverter.Domain.Models;
using FluentAssertions;

namespace CurrencyConverter.UnitTests;

public sealed class CurrencyServiceTests
{
    [Fact]
    public async Task ConvertAsync_ShouldThrow_WhenAmountIsInvalid()
    {
        var service = new CurrencyService(new FakeFactory(new FakeProvider()));

        var action = () => service.ConvertAsync(0m, "EUR", "USD");

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertAsync_ShouldThrow_WhenCurrencyIsExcluded()
    {
        var service = new CurrencyService(new FakeFactory(new FakeProvider()));

        var action = () => service.ConvertAsync(100m, "TRY", "USD");

        await action.Should().ThrowAsync<UnsupportedCurrencyException>();
    }

    [Fact]
    public async Task ConvertAsync_ShouldReturnSameAmount_WhenCurrenciesMatch()
    {
        var service = new CurrencyService(new FakeFactory(new FakeProvider()));

        var result = await service.ConvertAsync(10m, "EUR", "EUR");

        result.ConvertedAmount.Should().Be(10m);
        result.Rate.Should().Be(1m);
    }

    [Fact]
    public async Task GetLatestAsync_ShouldThrow_WhenBaseCurrencyMissing()
    {
        var service = new CurrencyService(new FakeFactory(new FakeProvider()));

        var action = () => service.GetLatestAsync(" ");

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task ConvertAsync_ShouldReturnConvertedAmount_WhenRateExists()
    {
        var provider = new FakeProvider();
        provider.SetLatest("EUR", new ExchangeRatesSnapshot(
            DateOnly.FromDateTime(DateTime.UtcNow),
            "EUR",
            new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
            {
                ["USD"] = 1.2m
            }));

        var service = new CurrencyService(new FakeFactory(provider));

        var result = await service.ConvertAsync(10m, "EUR", "USD");

        result.ConvertedAmount.Should().Be(12m);
        result.Rate.Should().Be(1.2m);
    }

    [Fact]
    public async Task GetHistoricalAsync_ShouldApplyPagination()
    {
        var provider = new FakeProvider();
        provider.SetHistorical("EUR", Enumerable.Range(1, 25)
            .Select(day => new ExchangeRatesSnapshot(
                new DateOnly(2020, 1, day),
                "EUR",
                new Dictionary<string, decimal> { ["USD"] = 1.1m }))
            .ToList());

        var service = new CurrencyService(new FakeFactory(provider));

        var result = await service.GetHistoricalAsync("EUR", new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 25), 2, 10);

        result.Items.Count.Should().Be(10);
        result.TotalItems.Should().Be(25);
        result.TotalPages.Should().Be(3);
        result.Page.Should().Be(2);
    }

    [Fact]
    public async Task GetHistoricalAsync_ShouldThrow_WhenDateRangeIsInvalid()
    {
        var service = new CurrencyService(new FakeFactory(new FakeProvider()));

        var action = () => service.GetHistoricalAsync("EUR", new DateOnly(2020, 1, 5), new DateOnly(2020, 1, 1), 1, 20);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetHistoricalAsync_ShouldThrow_WhenPagingIsInvalid()
    {
        var service = new CurrencyService(new FakeFactory(new FakeProvider()));

        var invalidPage = () => service.GetHistoricalAsync("EUR", new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 2), 0, 20);
        var invalidPageSize = () => service.GetHistoricalAsync("EUR", new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 2), 1, 0);

        await invalidPage.Should().ThrowAsync<ArgumentException>();
        await invalidPageSize.Should().ThrowAsync<ArgumentException>();
    }

    private sealed class FakeFactory(IExchangeRateProvider provider) : IExchangeRateProviderFactory
    {
        private readonly IExchangeRateProvider _provider = provider;

        public IExchangeRateProvider Create() => _provider;
    }

    private sealed class FakeProvider : IExchangeRateProvider
    {
        private readonly Dictionary<string, ExchangeRatesSnapshot> _latest = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, IReadOnlyCollection<ExchangeRatesSnapshot>> _historical = new(StringComparer.OrdinalIgnoreCase);

        public void SetLatest(string baseCurrency, ExchangeRatesSnapshot snapshot) => _latest[baseCurrency] = snapshot;

        public void SetHistorical(string baseCurrency, IReadOnlyCollection<ExchangeRatesSnapshot> snapshots) => _historical[baseCurrency] = snapshots;

        public Task<ExchangeRatesSnapshot> GetLatestAsync(string baseCurrency, CancellationToken cancellationToken = default)
        {
            if (_latest.TryGetValue(baseCurrency, out var snapshot))
            {
                return Task.FromResult(snapshot);
            }

            throw new InvalidOperationException("No latest data configured.");
        }

        public Task<IReadOnlyCollection<ExchangeRatesSnapshot>> GetHistoricalAsync(
            string baseCurrency,
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken = default)
        {
            if (_historical.TryGetValue(baseCurrency, out var snapshots))
            {
                return Task.FromResult(snapshots);
            }

            return Task.FromResult<IReadOnlyCollection<ExchangeRatesSnapshot>>(Array.Empty<ExchangeRatesSnapshot>());
        }
    }
}
