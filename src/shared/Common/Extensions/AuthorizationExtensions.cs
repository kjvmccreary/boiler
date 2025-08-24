using Common.Constants;
using Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Common.Authorization;

namespace Common.Extensions;

/// <summary>
/// Extension methods for configuring authorization
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Add basic authorization policies
    /// </summary>
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Admin policies
            options.AddPolicy("RequireSystemAdmin", policy =>
                policy.RequireClaim("permission", Permissions.System.ManageSettings));

            options.AddPolicy("RequireTenantAdmin", policy =>
                policy.RequireClaim("permission", Permissions.Tenants.ManageSettings));

            // User management policies
            options.AddPolicy("CanViewUsers", policy =>
                policy.RequireClaim("permission", Permissions.Users.View));

            options.AddPolicy("CanManageUsers", policy =>
                policy.RequireClaim("permission", Permissions.Users.Edit));

            options.AddPolicy("CanDeleteUsers", policy =>
                policy.RequireClaim("permission", Permissions.Users.Delete));

            // Role management policies
            options.AddPolicy("CanViewRoles", policy =>
                policy.RequireClaim("permission", Permissions.Roles.View));

            options.AddPolicy("CanManageRoles", policy =>
                policy.RequireClaim("permission", Permissions.Roles.Edit));
        });

        return services;
    }

    /// <summary>
    /// Helper method to check if user has permission (for use in controllers)
    /// Will be implemented in Phase 3 when the actual service exists
    /// </summary>
    public static async Task<bool> UserHasPermissionAsync(
        this IServiceProvider serviceProvider,
        int userId, 
        string permission)
    {
        // This will be implemented in Phase 3 when the actual service exists
        var permissionService = serviceProvider.GetService<IPermissionService>();
        if (permissionService != null)
        {
            return await permissionService.UserHasPermissionAsync(userId, permission);
        }
        
        // For Phase 2, always return false (no permission checking yet)
        return false;
    }

    /// <summary>
    /// Extension method for our custom authorization service (will be available in Phase 3)
    /// </summary>
    public static async Task<Contracts.Services.AuthorizationResult> AuthorizeUserAsync(
        this IServiceProvider serviceProvider,
        int userId,
        string permission)
    {
        var authService = serviceProvider.GetService<ICustomAuthorizationService>();
        if (authService != null)
        {
            return await authService.AuthorizeAsync(userId, permission);
        }

        // For Phase 2, return failure
        return Contracts.Services.AuthorizationResult.Failure("Authorization service not available");
    }

    /// <summary>
    /// Add enhanced permission-based authorization with dynamic policies
    /// </summary>
    public static IServiceCollection AddEnhancedAuthorizationPolicies(this IServiceCollection services)
    {
        // Replace the default policy provider with our permission-aware one
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        
        // The EnhancedPermissionAuthorizationHandler should already be registered
        // services.AddScoped<IAuthorizationHandler, EnhancedPermissionAuthorizationHandler>();
        
        return services;
    }
}
