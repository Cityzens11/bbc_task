using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace CurrencyConverter.Infrastructure.Http;

public sealed class CorrelationHeaderHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString();

        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            request.Headers.Remove("X-Correlation-ID");
            request.Headers.Add("X-Correlation-ID", correlationId);
        }

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return base.SendAsync(request, cancellationToken);
    }
}
