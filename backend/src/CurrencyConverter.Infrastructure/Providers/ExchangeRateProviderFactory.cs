using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CurrencyConverter.Infrastructure.Providers;

public sealed class ExchangeRateProviderFactory(
    IServiceProvider serviceProvider,
    IOptions<ProviderOptions> providerOptions) : IExchangeRateProviderFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ProviderOptions _providerOptions = providerOptions.Value;

    public IExchangeRateProvider Create()
    {
        return _providerOptions.Selected.ToUpperInvariant() switch
        {
            "FRANKFURTER" => _serviceProvider.GetRequiredService<FrankfurterExchangeRateProvider>(),
            _ => throw new InvalidOperationException($"Unknown provider '{_providerOptions.Selected}'.")
        };
    }
}
