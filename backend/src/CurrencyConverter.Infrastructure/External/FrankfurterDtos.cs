using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CurrencyConverter.Infrastructure.External;

[ExcludeFromCodeCoverage]
public sealed class FrankfurterLatestResponse
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("base")]
    public required string Base { get; set; }

    [JsonPropertyName("date")]
    public DateOnly Date { get; set; }

    [JsonPropertyName("rates")]
    public Dictionary<string, decimal> Rates { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

[ExcludeFromCodeCoverage]
public sealed class FrankfurterHistoricalResponse
{
    [JsonPropertyName("base")]
    public required string Base { get; set; }

    [JsonPropertyName("start_date")]
    public DateOnly StartDate { get; set; }

    [JsonPropertyName("end_date")]
    public DateOnly EndDate { get; set; }

    [JsonPropertyName("rates")]
    public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
