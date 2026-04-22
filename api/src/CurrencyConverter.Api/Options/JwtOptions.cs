using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Api.Options;

[ExcludeFromCodeCoverage]
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "CurrencyConverter.Api";

    public string Audience { get; set; } = "CurrencyConverter.Client";

    public string SigningKey { get; set; } = "super-secret-dev-signing-key-at-least-32-chars";

    public int ExpirationMinutes { get; set; } = 60;
}
