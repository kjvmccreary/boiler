using Common.Authorization;
using Common.Middleware;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.Extensions;

/// <summary>
/// Extensions for enhanced security and monitoring setup
/// Phase 11 - Enhanced Security and Monitoring
/// ✅ FIXED: Now respects testing environment and configuration settings
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
        
        // Add monitoring service (this IS a service that middleware depends on)
        services.AddScoped<IMonitoringService, MonitoringService>();
        
        return services;
    }

    /// <summary>
    /// Configure enhanced security and monitoring middleware
    /// ✅ FIXED: Now checks environment and configuration before applying middleware
    /// </summary>
    public static IApplicationBuilder UseEnhancedSecurity(this IApplicationBuilder app)
    {
        // ✅ CRITICAL FIX: Check if we should apply enhanced security middleware
        var environment = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
        
        // Check if we're in testing environment
        var isTestingEnvironment = environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase) ||
                                  environment.EnvironmentName.Equals("Test", StringComparison.OrdinalIgnoreCase);
        
        // Check configuration flags
        var enhancedSecurityEnabled = configuration.GetValue<bool>("Security:EnableEnhancedSecurity", true);
        var rateLimitingEnabled = configuration.GetValue<bool>("RateLimiting:Enabled", true);
        var monitoringEnabled = configuration.GetValue<bool>("Monitoring:Enabled", true);
        
        // ✅ In testing environment, check if explicitly enabled
        if (isTestingEnvironment)
        {
            var enableInTesting = configuration.GetValue<bool>("Security:EnableInTesting", false);
            if (!enableInTesting)
            {
                // ✅ FIX: Use a non-static class for logger type parameter
                var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger("SecurityExtensions");
                logger?.LogInformation("🔧 Enhanced security middleware DISABLED in testing environment");
                return app;
            }
        }
        
        // ✅ If globally disabled, skip middleware
        if (!enhancedSecurityEnabled)
        {
            // ✅ FIX: Use a non-static class for logger type parameter
            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("SecurityExtensions");
            logger?.LogInformation("🔧 Enhanced security middleware DISABLED via configuration");
            return app;
        }

        // ✅ Apply middleware conditionally based on configuration
        
        // Add request metrics collection (first to capture all requests) - if monitoring enabled
        if (monitoringEnabled)
        {
            app.UseMiddleware<RequestMetricsMiddleware>();
        }
        
        // Add rate limiting (before security event monitoring) - if rate limiting enabled
        if (rateLimitingEnabled)
        {
            app.UseMiddleware<EnhancedRateLimitingMiddleware>();
        }
        
        // Add security event monitoring middleware (after rate limiting) - if monitoring enabled
        if (monitoringEnabled)
        {
            app.UseMiddleware<SecurityEventMiddleware>();
        }
        
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
