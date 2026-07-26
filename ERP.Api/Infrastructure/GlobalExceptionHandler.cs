using ERP.Application.Common.Exceptions;
using ERP.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Infrastructure;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "An error occurred while processing request {RequestMethod} {RequestPath}. TraceId: {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.TraceIdentifier);

        var (statusCode, title, detail, errors) = MapException(exception);

        // Ensure internal exception details are not leaked in production for 500 status codes
        if (statusCode == StatusCodes.Status500InternalServerError && !environment.IsDevelopment())
        {
            detail = "An unexpected internal server error occurred. Please try again later.";
        }

        httpContext.Response.StatusCode = statusCode;

        if (errors is not null && errors.Count > 0)
        {
            var validationProblemDetails = new HttpValidationProblemDetails(errors)
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };
            validationProblemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
            return true;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title, string Detail, IDictionary<string, string[]>? Errors) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException valEx => (
                StatusCodes.Status400BadRequest,
                "Validation Error",
                "One or more validation errors occurred.",
                valEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            ),
            NotFoundException nfEx => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                nfEx.Message,
                null
            ),
            KeyNotFoundException knfEx => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                knfEx.Message,
                null
            ),
            ConflictException cEx => (
                StatusCodes.Status409Conflict,
                "Conflict",
                cEx.Message,
                null
            ),
            BusinessRuleValidationException brEx => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                brEx.Message,
                null
            ),
            UnauthorizedException unEx => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                unEx.Message,
                null
            ),
            UnauthorizedAccessException uae => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                uae.Message,
                null
            ),
            DomainException domEx => (
                StatusCodes.Status400BadRequest,
                "Domain Error",
                domEx.Message,
                null
            ),
            InvalidOperationException invEx => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                invEx.Message,
                null
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                exception.Message,
                null
            )
        };
    }
}
