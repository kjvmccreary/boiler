using Microsoft.AspNetCore.Authorization;

namespace WorkflowService.Security;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            WorkflowPolicies.ConfigurePolicies(options);
        });

        return services;
    }
}
