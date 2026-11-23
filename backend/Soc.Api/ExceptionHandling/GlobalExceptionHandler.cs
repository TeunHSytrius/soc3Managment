using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Soc.Business.Exceptions;

namespace Soc.Api.ExceptionHandling;

/// <summary>
/// Translates domain exceptions into HTTP problem details responses.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title, logLevel) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validatiefout", LogLevel.Information),
            CustomerNotFoundException => (StatusCodes.Status404NotFound, "Klant niet gevonden", LogLevel.Information),
            CustomerInactiveException => (StatusCodes.Status409Conflict, "Klant gedeactiveerd", LogLevel.Information),
            _ => (StatusCodes.Status500InternalServerError, "Er is een onverwachte fout opgetreden.", LogLevel.Error),
        };

        if (logLevel == LogLevel.Error)
        {
            _logger.LogError(exception, "Unhandled exception while handling {Path}", context.Request.Path);
        }
        else
        {
            _logger.LogInformation(exception, "Handled domain exception while handling {Path}", context.Request.Path);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path,
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
