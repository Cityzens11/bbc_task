using RestEase;

namespace CurrencyConverter.Infrastructure.External;

public interface IFrankfurterApi
{
    [Get("latest")]
    Task<FrankfurterLatestResponse> GetLatestAsync(
        [Query("from")] string from,
        [Query("to")] string? to = null,
        CancellationToken cancellationToken = default);

    [Get("{range}")]
    Task<FrankfurterHistoricalResponse> GetHistoricalAsync(
        [Path] string range,
        [Query("from")] string from,
        CancellationToken cancellationToken = default);
}
