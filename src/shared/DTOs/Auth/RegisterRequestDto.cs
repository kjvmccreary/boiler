// FILE: src/shared/DTOs/Auth/RegisterRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.Auth;

public class RegisterRequestDto
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

    // Optional: Allow users to specify tenant during registration (for new tenant creation)
    public string? TenantName { get; set; }
    public string? TenantDomain { get; set; }
}
