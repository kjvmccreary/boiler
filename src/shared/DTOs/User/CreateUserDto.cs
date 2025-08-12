// FILE: src/shared/DTOs/User/CreateUserDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.User;

/// <summary>
/// DTO for creating a new user (Renamed from UserCreateDto for consistency)
/// </summary>
public class CreateUserDto
{
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

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
