using Common.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace Common.Middleware
{
    /// <summary>
    /// Middleware to ensure consistent API response format
    /// </summary>
    public class ApiResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiResponseMiddleware> _logger;

        public ApiResponseMiddleware(RequestDelegate next, ILogger<ApiResponseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip for non-API requests
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            try
            {
                await _next(context);
                
                // Handle successful responses that don't return ApiResponse format
                if (context.Response.StatusCode == 200 && !context.Response.HasStarted)
                {
                    // If response doesn't seem to be in ApiResponse format, don't wrap it
                    // This allows controllers that already return ApiResponse to work unchanged
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "🔒 SECURITY: Unhandled exception in API request to {Path}", context.Request.Path);

            var response = context.Response;
            response.ContentType = "application/json";

            var apiResponse = exception switch
            {
                ArgumentException => ApiResponse.Error("Invalid input provided", new List<string> { exception.Message }),
                UnauthorizedAccessException => ApiResponse.Unauthorized(),
                InvalidOperationException => ApiResponse.Error(exception.Message),
                _ => ApiResponse.Error("An internal server error occurred")
            };

            response.StatusCode = exception switch
            {
                ArgumentException => 400,
                UnauthorizedAccessException => 401,
                InvalidOperationException => 400,
                _ => 500
            };

            var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse);
        }
    }

    public static class ApiResponseMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiResponse(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiResponseMiddleware>();
        }
    }
}
