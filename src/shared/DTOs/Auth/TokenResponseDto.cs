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
    
    // ✅ FIXED: Initialize TenantDto with required properties for .NET 9 compatibility
    public TenantDto Tenant { get; set; } = new()
    {
        Name = string.Empty,
        SubscriptionPlan = "Basic"
    };
}
