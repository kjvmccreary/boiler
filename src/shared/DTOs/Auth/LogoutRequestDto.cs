// FILE: src/shared/DTOs/Auth/LogoutRequestDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.Auth;

public class LogoutRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
