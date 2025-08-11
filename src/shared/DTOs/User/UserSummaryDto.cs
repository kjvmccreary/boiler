// FILE: src/shared/DTOs/User/UserSummaryDto.cs
namespace DTOs.User;

public class UserSummaryDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    // 🔧 .NET 9 FIX: Change from single Role to multi-role support
    public List<string> Roles { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
}
