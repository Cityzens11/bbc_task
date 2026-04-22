using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Domain.Constants;

[ExcludeFromCodeCoverage]
public static class ExcludedCurrencies
{
    public static readonly FrozenSet<string> Values =
        new[] { "TRY", "PLN", "THB", "MXN" }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
}
