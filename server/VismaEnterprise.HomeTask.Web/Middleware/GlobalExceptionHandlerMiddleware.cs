using System.Net;
using System.Text.Json;

namespace VismaEnterprise.HomeTask.Web.Middleware;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            return Task.CompletedTask;
        }

        var jsonResponse = JsonSerializer.Serialize(new { errorMessage = exception.Message });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)(exception is UnauthorizedAccessException ? HttpStatusCode.Forbidden : HttpStatusCode.BadRequest);

        return context.Response.WriteAsync(jsonResponse);
    }
}