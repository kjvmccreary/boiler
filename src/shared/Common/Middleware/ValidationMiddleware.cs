using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DTOs.Common;
using Common.Exceptions;
using System.Text.Json;

namespace Common.Middleware;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationMiddleware> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger, IServiceProvider serviceProvider)
    {
        _next = next;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {ValidationErrors}", ex.Message);
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (Common.Exceptions.ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {ValidationErrors}", ex.Message);
            await HandleCustomValidationExceptionAsync(context, ex);
        }
    }

    private static async Task HandleValidationExceptionAsync(HttpContext context, FluentValidation.ValidationException ex)
    {
        var errors = ex.Errors.Select(error => new ErrorDto(
            "VALIDATION_ERROR",
            error.ErrorMessage,
            error.PropertyName,
            error.AttemptedValue
        )).ToList();

        var response = ApiResponseDto<object>.ErrorResult("Validation failed.", errors);
        response.TraceId = context.TraceIdentifier;

        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static async Task HandleCustomValidationExceptionAsync(HttpContext context, Common.Exceptions.ValidationException ex)
    {
        var response = ApiResponseDto<object>.ErrorResult("Validation failed.", ex.ValidationErrors);
        response.TraceId = context.TraceIdentifier;

        context.Response.StatusCode = 400;
        context.Response.ContentType = "application/json";

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
