namespace Ticket.Api.Infrastructure.Middleware;

public static class ExceptionMiddlewareExtension
{
    public static void UseExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}