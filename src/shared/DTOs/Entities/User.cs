// FILE: src/shared/DTOs/Entities/User.cs
namespace DTOs.Entities;

public class User : BaseEntity
{
    // Primary tenant - the "home" tenant for this user
    public int? TenantId { get; set; }

    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // ➕ ADD THESE MISSING PROPERTIES:
    public string? PhoneNumber { get; set; }
    public string? TimeZone { get; set; }
    public string? Language { get; set; }
    public string? Preferences { get; set; } // JSON string for UserPreferencesDto

    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }
    public DateTime? EmailConfirmationTokenExpiry { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockedOutUntil { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation properties
    public Tenant? PrimaryTenant { get; set; } // The user's primary/home tenant
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>(); // All tenant associations
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    
    // 🔧 .NET 9 FIX: Add missing UserRoles navigation property
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>(); // User role assignments
}
