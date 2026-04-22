using CurrencyConverter.Application.Abstractions;
using CurrencyConverter.Application.Services;
using CurrencyConverter.Infrastructure.External;
using CurrencyConverter.Infrastructure.Http;
using CurrencyConverter.Infrastructure.Options;
using CurrencyConverter.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace CurrencyConverter.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ProviderOptions>(configuration.GetSection(ProviderOptions.SectionName));

        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddTransient<CorrelationHeaderHandler>();

        services.AddHttpClient("FrankfurterHttpClient", (serviceProvider, httpClient) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<ProviderOptions>>().Value;
                httpClient.BaseAddress = new Uri(options.Frankfurter.BaseUrl);
                httpClient.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddHttpMessageHandler<CorrelationHeaderHandler>()
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        services.AddScoped(serviceProvider =>
        {
            var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient("FrankfurterHttpClient");
            return RestEase.RestClient.For<IFrankfurterApi>(client);
        });

        services.AddScoped<FrankfurterExchangeRateProvider>();
        services.AddScoped<IExchangeRateProviderFactory, ExchangeRateProviderFactory>();
        services.AddScoped<ICurrencyService, CurrencyService>();

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(response => (int)response.StatusCode == 429)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }
}
