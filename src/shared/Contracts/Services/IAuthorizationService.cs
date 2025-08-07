namespace Contracts.Services;

/// <summary>
/// High-level custom authorization service that coordinates permission and role checking
/// Renamed to avoid conflict with Microsoft.AspNetCore.Authorization.IAuthorizationService
/// </summary>
public interface ICustomAuthorizationService
{
    /// <summary>
    /// Authorize a user for a specific action/permission
    /// </summary>
    Task<AuthorizationResult> AuthorizeAsync(int userId, string permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authorize a user for multiple permissions (must have all)
    /// </summary>
    Task<AuthorizationResult> AuthorizeAllAsync(int userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authorize a user for multiple permissions (must have at least one)
    /// </summary>
    Task<AuthorizationResult> AuthorizeAnyAsync(int userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get authorization context for a user (permissions, roles, tenant info)
    /// </summary>
    Task<AuthorizationContext> GetAuthorizationContextAsync(int userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of an authorization check
/// </summary>
public class AuthorizationResult
{
    public bool Succeeded { get; set; }
    public string? FailureReason { get; set; }
    public List<string> MissingPermissions { get; set; } = new();

    public static AuthorizationResult Success() => new() { Succeeded = true };
    
    public static AuthorizationResult Failure(string reason, List<string>? missingPermissions = null) => new()
    {
        Succeeded = false,
        FailureReason = reason,
        MissingPermissions = missingPermissions ?? new List<string>()
    };
}

/// <summary>
/// Complete authorization context for a user
/// </summary>
public class AuthorizationContext
{
    public int UserId { get; set; }
    public int? TenantId { get; set; }
    public List<string> Permissions { get; set; } = new();
    public List<string> Roles { get; set; } = new();
    public Dictionary<string, object> AdditionalClaims { get; set; } = new();
}
