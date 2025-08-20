using System.Security.Claims;
using UserEntity = DTOs.Entities.User;
using TenantEntity = DTOs.Entities.Tenant; 
using RefreshTokenEntity = DTOs.Entities.RefreshToken;

namespace Contracts.Services;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(UserEntity user, TenantEntity tenant);
    Task<string> GenerateAccessTokenWithoutTenantAsync(UserEntity user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Task<RefreshTokenEntity> CreateRefreshTokenAsync(UserEntity user);
}
