using Asp.Versioning;
using CurrencyConverter.Api.Constants;
using CurrencyConverter.Api.Contracts.Requests;
using CurrencyConverter.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/rates")]
public sealed class RatesController(ICurrencyService currencyService) : ControllerBase
{
    private readonly ICurrencyService _currencyService = currencyService;

    [HttpGet("latest")]
    [Authorize(Policy = PolicyNames.RatesReader)]
    public async Task<IActionResult> GetLatest([FromQuery] string baseCurrency = "EUR", CancellationToken cancellationToken = default)
    {
        var result = await _currencyService.GetLatestAsync(baseCurrency, cancellationToken);
        return Ok(result);
    }

    [HttpGet("convert")]
    [Authorize(Policy = PolicyNames.Converter)]
    public async Task<IActionResult> Convert(
        [FromQuery] decimal amount,
        [FromQuery] string fromCurrency,
        [FromQuery] string toCurrency,
        CancellationToken cancellationToken = default)
    {
        var result = await _currencyService.ConvertAsync(amount, fromCurrency, toCurrency, cancellationToken);
        return Ok(result);
    }

    [HttpGet("historical")]
    [Authorize(Policy = PolicyNames.RatesReader)]
    public async Task<IActionResult> GetHistorical([FromQuery] HistoricalRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _currencyService.GetHistoricalAsync(
            request.BaseCurrency,
            request.StartDate,
            request.EndDate,
            request.Page,
            request.PageSize,
            cancellationToken);

        return Ok(result);
    }
}
