// FILE: src/shared/Contracts/User/IUserProfileService.cs
using DTOs.Common;
using DTOs.User;

namespace Contracts.User;

public interface IUserProfileService
{
    Task<ApiResponseDto<UserDto>> GetUserProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<UserDto>> UpdateUserProfileAsync(int userId, UserUpdateDto updateProfileDto, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> UpdateUserPreferencesAsync(int userId, UserPreferencesDto preferences, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<UserPreferencesDto>> GetUserPreferencesAsync(int userId, CancellationToken cancellationToken = default);
}
