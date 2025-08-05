// FILE: src/shared/DTOs/Entities/User.cs
namespace DTOs.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }
    public DateTime? EmailConfirmationTokenExpiry { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockedOutUntil { get; set; } // FIXED: Changed from LockoutEnd to LockedOutUntil
    public DateTime? LastLoginAt { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation properties
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
