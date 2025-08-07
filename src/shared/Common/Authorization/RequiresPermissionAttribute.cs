using Microsoft.AspNetCore.Authorization;

namespace Common.Authorization;

/// <summary>
/// Custom authorization attribute that requires specific permissions
/// </summary>
public class RequiresPermissionAttribute : AuthorizeAttribute
{
    public RequiresPermissionAttribute(string permission) : base(permission)
    {
        Policy = permission;
    }
}
