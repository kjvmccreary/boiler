// FILE: src/shared/DTOs/User/UserDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.User;

public class UserDto
{
    public int Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Roles { get; set; } = new();
}
