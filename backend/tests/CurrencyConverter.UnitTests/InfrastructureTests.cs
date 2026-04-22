using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Domain.Models;
using CurrencyConverter.Infrastructure.External;
using CurrencyConverter.Infrastructure.Options;
using CurrencyConverter.Infrastructure.Providers;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace CurrencyConverter.UnitTests;

public sealed class InfrastructureTests
{
    [Fact]
    public async Task FrankfurterProvider_ShouldUseCache_ForLatestRates()
    {
        var apiMock = new Mock<IFrankfurterApi>();
        apiMock
            .Setup(x => x.GetLatestAsync("EUR", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FrankfurterLatestResponse
            {
                Base = "EUR",
                Date = new DateOnly(2020, 1, 1),
                Rates = new Dictionary<string, decimal> { ["USD"] = 1.1m }
            });

        var provider = BuildProvider(apiMock.Object);

        var first = await provider.GetLatestAsync("EUR");
        var second = await provider.GetLatestAsync("EUR");

        first.Rates["USD"].Should().Be(1.1m);
        second.Rates["USD"].Should().Be(1.1m);
        apiMock.Verify(x => x.GetLatestAsync("EUR", null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FrankfurterProvider_ShouldMapHistoricalResponse()
    {
        var apiMock = new Mock<IFrankfurterApi>();
        apiMock
            .Setup(x => x.GetHistoricalAsync("2020-01-01..2020-01-02", "EUR", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FrankfurterHistoricalResponse
            {
                Base = "EUR",
                StartDate = new DateOnly(2020, 1, 1),
                EndDate = new DateOnly(2020, 1, 2),
                Rates = new Dictionary<string, Dictionary<string, decimal>>
                {
                    ["2020-01-01"] = new() { ["USD"] = 1.1m },
                    ["2020-01-02"] = new() { ["USD"] = 1.2m }
                }
            });

        var provider = BuildProvider(apiMock.Object);

        var result = await provider.GetHistoricalAsync("EUR", new DateOnly(2020, 1, 1), new DateOnly(2020, 1, 2));

        result.Count.Should().Be(2);
        result.Should().Contain(x => x.Date == new DateOnly(2020, 1, 1));
        result.Should().Contain(x => x.Date == new DateOnly(2020, 1, 2));
    }

    [Fact]
    public void Factory_ShouldReturnFrankfurterProvider_WhenConfigured()
    {
        var services = new ServiceCollection();
        services.AddScoped<FrankfurterExchangeRateProvider>(_ => BuildProvider(new Mock<IFrankfurterApi>().Object));
        var provider = services.BuildServiceProvider();

        var options = Options.Create(new ProviderOptions { Selected = "Frankfurter" });
        var factory = new ExchangeRateProviderFactory(provider, options);

        var result = factory.Create();

        result.Should().BeOfType<FrankfurterExchangeRateProvider>();
    }

    [Fact]
    public void Factory_ShouldThrow_WhenProviderIsUnknown()
    {
        var services = new ServiceCollection().BuildServiceProvider();
        var options = Options.Create(new ProviderOptions { Selected = "Other" });
        var factory = new ExchangeRateProviderFactory(services, options);

        var action = () => factory.Create();

        action.Should().Throw<InvalidOperationException>();
    }

    private static FrankfurterExchangeRateProvider BuildProvider(IFrankfurterApi api)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var options = Options.Create(new ProviderOptions
        {
            Frankfurter = new FrankfurterOptions
            {
                BaseUrl = "https://api.frankfurter.app/",
                LatestCacheSeconds = 60,
                HistoricalCacheSeconds = 60
            }
        });

        return new FrankfurterExchangeRateProvider(api, cache, options, NullLogger<FrankfurterExchangeRateProvider>.Instance);
    }
}
