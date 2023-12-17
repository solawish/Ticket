using System.Net;
using System.Text.Json;

namespace Ticket.Api.Infrastructure.Middleware;

public class ExceptionMiddleware
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        JsonSerializerOptions jsonOptions)
    {
        _logger = logger;
        _jsonOptions = jsonOptions;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode =
            exception is ArgumentException
                ? (int)HttpStatusCode.BadRequest
                : (int)HttpStatusCode.InternalServerError;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(
                new
                {
                    Message = $"系統異常: {exception.Message}"
                }, _jsonOptions)
            );

        _logger.LogError(exception, "發生 Exception");
    }
}