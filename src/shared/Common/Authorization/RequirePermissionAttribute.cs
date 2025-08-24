using Microsoft.AspNetCore.Authorization;

namespace Common.Authorization;

/// <summary>
/// Authorization attribute that requires a specific permission
/// Provides declarative permission-based security at the controller/action level
/// </summary>
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
    {
        Policy = $"Permission:{permission}";
    }
}
