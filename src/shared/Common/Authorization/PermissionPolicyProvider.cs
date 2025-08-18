using Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Common.Authorization;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;
    private readonly ILogger<PermissionPolicyProvider> _logger;
    private const string RequiresPermissionPrefix = "RequiresPermission:";

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options, ILogger<PermissionPolicyProvider> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => Task.FromResult(_options.DefaultPolicy)!;

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => Task.FromResult(_options.FallbackPolicy);

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // ✅ CRITICAL FIX: Handle RequiresPermission: prefixed policy names
        if (policyName.StartsWith(RequiresPermissionPrefix))
        {
            var permission = policyName.Substring(RequiresPermissionPrefix.Length);
            
            // Check if this is a valid permission
            if (Permissions.IsValidPermission(permission))
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();

                _logger.LogDebug("Created dynamic permission policy for: {PolicyName} (permission: {Permission})", policyName, permission);
                return Task.FromResult<AuthorizationPolicy?>(policy);
            }
            else
            {
                _logger.LogWarning("Invalid permission requested in policy: {PolicyName} (permission: {Permission})", policyName, permission);
            }
        }
        // ✅ Also handle plain permission names (for backward compatibility)
        else if (Permissions.IsValidPermission(policyName))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            _logger.LogDebug("Created dynamic permission policy for: {PolicyName}", policyName);
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        // Fall back to configured policies
        var configuredPolicy = _options.GetPolicy(policyName);
        if (configuredPolicy == null)
        {
            _logger.LogWarning("No policy found for: {PolicyName}", policyName);
        }
        
        return Task.FromResult(configuredPolicy);
    }
}
