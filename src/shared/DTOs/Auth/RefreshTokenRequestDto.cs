// FILE: src/shared/DTOs/Auth/RefreshTokenRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.Auth;

public class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
