using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Api.Constants;

[ExcludeFromCodeCoverage]
public static class PolicyNames
{
    public const string RateLimiter = "fixed";
    public const string RatesReader = "RatesReaderPolicy";
    public const string Converter = "ConverterPolicy";
    public const string FrontendCors = "FrontendPolicy";
}