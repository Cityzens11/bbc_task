using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Api.Contracts.Requests;

[ExcludeFromCodeCoverage]
public sealed record ConvertRequest(decimal Amount, string FromCurrency, string ToCurrency);
