using DTOs.Auth;
using DTOs.Common;

namespace Contracts.Auth;

public interface IAuthService
{
    // Existing methods
    Task<ApiResponseDto<TokenResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<TokenResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> ChangePasswordAsync(int userId, ChangePasswordDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> ResetPasswordAsync(string email, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);

    // NEW: RBAC-related methods for Enhanced Phase 4
    Task<List<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<string>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default);
    
    // 🔧 NEW: Tenant switching method
    Task<ApiResponseDto<TokenResponseDto>> SwitchTenantAsync(int userId, int tenantId, CancellationToken cancellationToken = default);
}
