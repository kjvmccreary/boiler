using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace WorkflowService.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException)
        {
            // Client disconnected / request aborted
            if (!context.Response.HasStarted)
            {
                // 499 is non-standard; ASP.NET Core provides a constant
                context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            }
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogError(ex, "UNHANDLED_EXCEPTION TraceId={TraceId} Path={Path}", traceId, context.Request.Path);

            if (context.Response.HasStarted) return;

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                error = "internal_error",
                message = "An unexpected error occurred.",
                traceId
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseWorkflowExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
