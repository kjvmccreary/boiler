// FILE: src/shared/DTOs/User/UserDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.User;

public class UserDto
{
    public int Id { get; set; }
    public int TenantId { get; set; } // Changed from Guid to int to match User entity
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Roles { get; set; } = new();
}
