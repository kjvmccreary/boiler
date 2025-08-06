// FILE: src/shared/DTOs/Auth/TokenResponseDto.cs
using DTOs.Tenant;
using DTOs.User;

namespace DTOs.Auth;

public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public UserDto User { get; set; } = new();
    public TenantDto Tenant { get; set; } = new();
}
