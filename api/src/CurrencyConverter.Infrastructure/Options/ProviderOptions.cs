using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Infrastructure.Options;

[ExcludeFromCodeCoverage]
public sealed class ProviderOptions
{
    public const string SectionName = "Provider";

    public string Selected { get; set; } = "Frankfurter";

    public FrankfurterOptions Frankfurter { get; set; } = new();
}

[ExcludeFromCodeCoverage]
public sealed class FrankfurterOptions
{
    public string BaseUrl { get; set; } = "https://api.frankfurter.app/";

    public int LatestCacheSeconds { get; set; } = 60;

    public int HistoricalCacheSeconds { get; set; } = 300;
}
