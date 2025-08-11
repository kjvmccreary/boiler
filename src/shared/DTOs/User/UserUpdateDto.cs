// FILE: src/shared/DTOs/User/UserUpdateDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.User;

public class UserUpdateDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    // 🔧 .NET 9 FIX: Add missing Email property
    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    public string? TimeZone { get; set; }

    [StringLength(10)]
    public string? Language { get; set; }

    public bool IsActive { get; set; } = true;
    public List<string> Roles { get; set; } = new();
}
