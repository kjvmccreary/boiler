// FILE: src/shared/Contracts/Auth/IAuthService.cs
using DTOs.Auth;
using DTOs.Common;

namespace Contracts.Auth;

public interface IAuthService
{
    Task<ApiResponseDto<TokenResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<TokenResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> ChangePasswordAsync(int userId, ChangePasswordDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> ResetPasswordAsync(string email, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> ConfirmEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}
