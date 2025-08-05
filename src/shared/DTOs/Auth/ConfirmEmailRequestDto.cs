// FILE: src/shared/DTOs/Auth/ConfirmEmailRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.Auth;

public class ConfirmEmailRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}
