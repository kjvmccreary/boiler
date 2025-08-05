// FILE: src/shared/DTOs/Auth/ResetPasswordRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.Auth;

public class ResetPasswordRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
