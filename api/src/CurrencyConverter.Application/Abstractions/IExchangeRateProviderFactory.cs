namespace CurrencyConverter.Application.Abstractions;

public interface IExchangeRateProviderFactory
{
    IExchangeRateProvider Create();
}
