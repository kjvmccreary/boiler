using Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Common.Authorization;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;
    private readonly ILogger<PermissionPolicyProvider> _logger;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options, ILogger<PermissionPolicyProvider> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Task.FromResult(_options.DefaultPolicy)!;

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => Task.FromResult(_options.FallbackPolicy);

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check if this is a permission-based policy
        if (Permissions.IsValidPermission(policyName))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            _logger.LogDebug("Created dynamic permission policy for: {PolicyName}", policyName);
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        // Fall back to configured policies
        var configuredPolicy = _options.GetPolicy(policyName);
        return Task.FromResult(configuredPolicy);
    }
}
