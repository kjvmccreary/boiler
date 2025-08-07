using Common.Authorization;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions;

public static class AuthorizationServiceExtensions
{
    public static IServiceCollection AddDynamicAuthorization(this IServiceCollection services)
    {
        // FIX: Use fully qualified interface name to avoid ambiguity
        services.AddScoped<Contracts.Services.IPermissionService, PermissionService>();

        // Register authorization components
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

        return services;
    }
}
