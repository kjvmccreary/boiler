// FILE: src/shared/Common/Constants/Roles.cs
namespace Common.Constants;

public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string TenantAdmin = "TenantAdmin";
    public const string User = "User";
    public const string ReadOnly = "ReadOnly";

    public static readonly string[] AllRoles = { SuperAdmin, TenantAdmin, User, ReadOnly };

    public static bool IsValid(string role)
    {
        return AllRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
