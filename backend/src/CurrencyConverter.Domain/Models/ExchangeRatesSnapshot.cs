using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Domain.Models;

[ExcludeFromCodeCoverage]
public sealed record ExchangeRatesSnapshot(DateOnly Date, string BaseCurrency, IReadOnlyDictionary<string, decimal> Rates);
