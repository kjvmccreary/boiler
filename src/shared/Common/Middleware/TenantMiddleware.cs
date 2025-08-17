using System.Net;
using Common.Configuration;
using Common.Data;
using Contracts.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder; // 🔧 ADD: Missing using directive for IApplicationBuilder

namespace Common.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;
    private readonly TenantSettings _tenantSettings;

    public TenantMiddleware(
        RequestDelegate next,
        ILogger<TenantMiddleware> logger,
        TenantSettings tenantSettings)
    {
        _next = next;
        _logger = logger;
        _tenantSettings = tenantSettings;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider, ApplicationDbContext dbContext)
    {
        try
        {
            // Skip tenant resolution for certain endpoints
            if (ShouldSkipTenantResolution(context.Request.Path))
            {
                _logger.LogDebug("Skipping tenant resolution for path: {Path}", context.Request.Path);
                await _next(context);
                return;
            }

            // Resolve tenant ID from request
            var tenantId = await tenantProvider.GetCurrentTenantIdAsync();

            if (tenantId.HasValue)
            {
                // Validate tenant exists and is active
                var tenant = await ValidateTenantAsync(dbContext, tenantId.Value);
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant {TenantId} not found or inactive", tenantId);
                    await WriteTenantNotFoundResponse(context);
                    return;
                }

                // Set tenant context for the request
                context.Items["TenantId"] = tenantId.Value;
                context.Items["TenantName"] = tenant.Name;
                context.Items["TenantDomain"] = tenant.Domain;

                // Set database session context for RLS
                await SetDatabaseTenantContextAsync(dbContext, tenantId.Value);

                _logger.LogDebug("Tenant context set for request: TenantId={TenantId}, TenantName={TenantName}", 
                    tenantId.Value, tenant.Name);
            }
            else
            {
                // Handle missing tenant context based on configuration
                if (_tenantSettings.RequireTenantContext && !IsPublicEndpoint(context.Request.Path))
                {
                    _logger.LogWarning("Tenant context required but not found for path: {Path}", context.Request.Path);
                    await WriteTenantRequiredResponse(context);
                    return;
                }

                _logger.LogDebug("No tenant context found, proceeding without tenant isolation");
            }

            // Continue to next middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TenantMiddleware for path: {Path}", context.Request.Path);
            
            // Don't fail the request for tenant resolution errors unless required
            if (_tenantSettings.RequireTenantContext && !IsPublicEndpoint(context.Request.Path))
            {
                await WriteErrorResponse(context, "Tenant resolution failed");
                return;
            }

            // Continue without tenant context
            await _next(context);
        }
    }

    private bool ShouldSkipTenantResolution(PathString path)
    {
        var skipPaths = new[]
        {
            "/health",
            "/api/health",
            "/swagger",
            "/api/auth/login",
            "/api/auth/register",
            "/api/tenants" // Global tenant management
        };

        return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsPublicEndpoint(PathString path)
    {
        var publicPaths = new[]
        {
            "/health",
            "/api/health",
            "/swagger",
            "/api/auth/login",
            "/api/auth/register",
            "/api/auth/refresh",
            "/favicon.ico"
        };

        return publicPaths.Any(publicPath => path.StartsWithSegments(publicPath, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<DTOs.Entities.Tenant?> ValidateTenantAsync(ApplicationDbContext dbContext, int tenantId)
    {
        try
        {
            // Query tenant directly (bypassing tenant filters for validation)
            var tenant = await dbContext.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == tenantId);

            return tenant?.IsActive == true ? tenant : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating tenant {TenantId}", tenantId);
            return null;
        }
    }

    private async Task SetDatabaseTenantContextAsync(ApplicationDbContext dbContext, int tenantId)
    {
        try
        {
            // Set PostgreSQL session variable for Row Level Security
            await dbContext.Database.ExecuteSqlRawAsync(
                "SELECT set_config('app.tenant_id', {0}, false)", tenantId.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to set database tenant context for tenant {TenantId}", tenantId);
            // Continue - RLS is defense in depth, not primary isolation mechanism
        }
    }

    private async Task WriteTenantNotFoundResponse(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            error = "TENANT_NOT_FOUND",
            message = "The requested tenant was not found or is inactive.",
            statusCode = 404
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }

    private async Task WriteTenantRequiredResponse(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            error = "TENANT_REQUIRED",
            message = "Tenant context is required for this endpoint.",
            statusCode = 400
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }

    private async Task WriteErrorResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            error = "TENANT_RESOLUTION_ERROR",
            message = message,
            statusCode = 500
        };

        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
}

// Extension method for easy registration
public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
}
