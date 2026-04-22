using System.Diagnostics.CodeAnalysis;

namespace CurrencyConverter.Api.Contracts.Requests;

[ExcludeFromCodeCoverage]
public sealed record TokenRequest(string ClientId, string[] Roles);
