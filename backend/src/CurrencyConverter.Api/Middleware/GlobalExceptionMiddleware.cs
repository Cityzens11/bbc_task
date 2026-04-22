using CurrencyConverter.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Api.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnsupportedCurrencyException ex)
        {
            await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Unsupported currency", ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Invalid request", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblemDetailsAsync(context, StatusCodes.Status500InternalServerError, "Internal server error", "An unexpected error occurred.");
        }
    }

    private static Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        return context.Response.WriteAsJsonAsync(problem);
    }
}
