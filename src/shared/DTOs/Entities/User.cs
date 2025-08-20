// FILE: src/shared/DTOs/Entities/User.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.Entities
{
    /// <summary>
    /// Represents a user in the system who can belong to multiple tenants
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// User's email address (unique across the entire system)
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hashed password
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// User's first name
        /// </summary>
        [Required]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// User's last name
        /// </summary>
        [Required]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Phone number (optional)
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// User's timezone
        /// </summary>
        public string? TimeZone { get; set; }

        /// <summary>
        /// User's preferred language
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// JSON field for storing user preferences
        /// </summary>
        public string? Preferences { get; set; }

        /// <summary>
        /// Whether the user's email is confirmed
        /// </summary>
        public bool EmailConfirmed { get; set; } = false;

        /// <summary>
        /// Token for email confirmation
        /// </summary>
        public string? EmailConfirmationToken { get; set; }

        /// <summary>
        /// When the email confirmation token expires
        /// </summary>
        public DateTime? EmailConfirmationTokenExpiry { get; set; }

        /// <summary>
        /// Token for password reset
        /// </summary>
        public string? PasswordResetToken { get; set; }

        /// <summary>
        /// When the password reset token expires
        /// </summary>
        public DateTime? PasswordResetTokenExpiry { get; set; }

        /// <summary>
        /// Last time the user logged in
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Whether the user account is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Number of failed login attempts
        /// </summary>
        public int FailedLoginAttempts { get; set; } = 0;

        /// <summary>
        /// When the user is locked out until (null = not locked out)
        /// </summary>
        public DateTime? LockedOutUntil { get; set; }

        // Navigation properties
        public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        
        // 🔧 FIX: Add explicit UserRoles navigation property to prevent EF Core shadow property issues
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
