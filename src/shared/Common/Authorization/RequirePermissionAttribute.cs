using System;
using Microsoft.AspNetCore.Authorization;

namespace Common.Authorization;

/// <summary>
/// LEGACY: Old single-permission attribute.
/// Prefer using <see cref="RequiresPermissionAttribute"/> which aligns with current policy naming.
/// </summary>
[Obsolete("Use RequiresPermissionAttribute instead. This legacy attribute will be removed after migration.")]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
    {
        Policy = $"Permission:{permission}";
    }
}
