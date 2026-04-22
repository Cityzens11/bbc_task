using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class UnsupportedCurrencyException(string currency)
    : Exception($"Currency '{currency}' is not supported for conversion. Excluded currencies: TRY, PLN, THB, MXN.")
{
    public string Currency { get; } = currency;
}
