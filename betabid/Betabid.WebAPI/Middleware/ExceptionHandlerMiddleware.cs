using System.Security.Authentication;
using System.Text.Json;
using Betabid.Application.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace betabid.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = exception switch
        {
            AuthenticationException => StatusCodes.Status401Unauthorized,
            ArgumentException => StatusCodes.Status400BadRequest,
            ValidationException => StatusCodes.Status400BadRequest,
            EntityNotFoundException => StatusCodes.Status404NotFound,
            DbUpdateException => StatusCodes.Status400BadRequest,
            HttpRequestException => StatusCodes.Status400BadRequest,
            AccessViolationException => StatusCodes.Status403Forbidden,
            UserUpdateException => StatusCodes.Status400BadRequest,
            LotAlreadyStartedException => StatusCodes.Status400BadRequest,
            LotAlreadySavedException => StatusCodes.Status400BadRequest,
            LotHasBidsException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError,
        };

        context.Response.ContentType = "application/json";

        var innerExceptionMes = string.Empty;
        if (exception.InnerException?.Message is not null)
        {
            innerExceptionMes = exception.InnerException?.Message;
        }

        var errorObject = new { error = exception.Message + innerExceptionMes };
        var errorJson = JsonSerializer.Serialize(errorObject);
        await context.Response.WriteAsync(errorJson);
    }
}
