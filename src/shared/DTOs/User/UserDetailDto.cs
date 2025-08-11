// FILE: src/shared/DTOs/User/UserDetailDto.cs
namespace DTOs.User;

public class UserDetailDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? PhoneNumber { get; set; }
    public string? TimeZone { get; set; }
    public string? Language { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool EmailConfirmed { get; set; } = false;
    
    // Admin-only fields
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockedUntil { get; set; }
    public List<string> Roles { get; set; } = new();
    
    // 🔧 .NET 9 FIX: Add missing ActiveSessions property
    public int ActiveSessions { get; set; } = 0;
    
    // Optional: Full preferences for admin view
    public UserPreferencesDto? Preferences { get; set; }
}
