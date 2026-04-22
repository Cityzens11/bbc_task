using System.Diagnostics;
using System.Security.Claims;

namespace CurrencyConverter.Api.Middleware;

public sealed class ObservabilityMiddleware(RequestDelegate next, ILogger<ObservabilityMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ObservabilityMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? string.Empty;
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        await _next(context);

        stopwatch.Stop();
        var clientId = context.User.FindFirstValue("client_id") ?? "anonymous";

        _logger.LogInformation(
            "Request completed {Method} {Path} responded {StatusCode} in {ElapsedMs} ms from {ClientIp} by {ClientId} correlation {CorrelationId}",
            context.Request.Method,
            context.Request.Path.Value,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            clientIp,
            clientId,
            correlationId);
    }
}
