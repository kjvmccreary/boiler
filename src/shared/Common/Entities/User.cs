// FILE: src/shared/Common/Entities/User.cs
namespace Common.Entities;

public class User : TenantEntity
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockedOutUntil { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
