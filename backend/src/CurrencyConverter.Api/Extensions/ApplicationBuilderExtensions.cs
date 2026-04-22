using System.Diagnostics.CodeAnalysis;
using CurrencyConverter.Api.Middleware;

namespace CurrencyConverter.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseMiddleware<CorrelationMiddleware>();
        app.UseMiddleware<ObservabilityMiddleware>();
        return app;
    }
}
