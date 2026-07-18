using ERP.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "An unhandled exception occurred.");

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context,Exception exception)
    {
        var problemDetails = new ProblemDetails();

        switch (exception)
        {
            case UnauthorizedException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Unauthorized";
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                break;
        }

        problemDetails.Status = context.Response.StatusCode;
        problemDetails.Detail = exception.Message;

        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}