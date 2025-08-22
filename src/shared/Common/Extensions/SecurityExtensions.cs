using Common.Authorization;
using Common.Middleware;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions;

/// <summary>
/// Extensions for enhanced security and monitoring setup
/// Phase 11 - Enhanced Security and Monitoring
/// </summary>
public static class SecurityExtensions
{
    /// <summary>
    /// Add enhanced security services with audit integration
    /// </summary>
    public static IServiceCollection AddEnhancedSecurity(this IServiceCollection services)
    {
        // Replace the basic authorization handler with the enhanced one
        services.AddScoped<IAuthorizationHandler, EnhancedPermissionAuthorizationHandler>();
        
        // 🔧 FIX: Don't register middleware as services - they're instantiated by the pipeline
        // ❌ REMOVE: services.AddScoped<SecurityEventMiddleware>();
        // ❌ REMOVE: services.AddScoped<EnhancedRateLimitingMiddleware>();
        // ❌ REMOVE: services.AddScoped<RequestMetricsMiddleware>();
        
        // Add monitoring service (this IS a service that middleware depends on)
        services.AddScoped<IMonitoringService, MonitoringService>();
        
        return services;
    }

    /// <summary>
    /// Configure enhanced security and monitoring middleware
    /// </summary>
    public static IApplicationBuilder UseEnhancedSecurity(this IApplicationBuilder app)
    {
        // Add request metrics collection (first to capture all requests)
        app.UseMiddleware<RequestMetricsMiddleware>();
        
        // Add rate limiting (before security event monitoring)
        app.UseMiddleware<EnhancedRateLimitingMiddleware>();
        
        // Add security event monitoring middleware (after rate limiting)
        app.UseMiddleware<SecurityEventMiddleware>();
        
        return app;
    }

    /// <summary>
    /// Add enhanced authorization policies with audit support
    /// </summary>
    public static IServiceCollection AddEnhancedAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Create dynamic policies for all permissions
            var allPermissions = Common.Constants.Permissions.GetAllPermissions();
            
            foreach (var permission in allPermissions)
            {
                options.AddPolicy(permission, policy =>
                    policy.Requirements.Add(new PermissionRequirement(permission)));
            }

            // Special high-security policies
            options.AddPolicy("HighSecurity", policy =>
            {
                policy.Requirements.Add(new PermissionRequirement("system.manage"));
                policy.RequireAuthenticatedUser();
            });
        });

        return services;
    }
}
