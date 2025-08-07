using Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Common.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionAuthorizationHandler> _logger;

    public PermissionAuthorizationHandler(
        IPermissionService permissionService,
        ILogger<PermissionAuthorizationHandler> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // FIX: Use fully qualified name to avoid ambiguity
        var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Authorization failed: No valid user ID found in claims");
            context.Fail();
            return;
        }

        var hasPermission = await _permissionService.UserHasPermissionAsync(userId, requirement.Permission);
        
        if (hasPermission)
        {
            _logger.LogDebug("Authorization succeeded: User {UserId} has permission {Permission}", userId, requirement.Permission);
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("Authorization failed: User {UserId} does not have permission {Permission}", userId, requirement.Permission);
            context.Fail();
        }
    }
}
