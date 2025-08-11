// src/shared/Contracts/User/IUserService.cs
using DTOs.Common;
using DTOs.User;

namespace Contracts.User;

public interface IUserService
{
    Task<ApiResponseDto<UserDto>> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<UserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<PagedResultDto<UserDto>>> GetUsersAsync(PaginationRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<UserDto>> CreateUserAsync(UserCreateDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<UserDto>> UpdateUserAsync(int userId, UserUpdateDto updateUserDto, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> DeleteUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> ExistsAsync(int userId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<PagedResultDto<UserSummaryDto>>> GetUsersSummaryAsync(int tenantId, PaginationRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<UserDetailDto>> GetUserDetailAsync(int userId, int tenantId, CancellationToken cancellationToken = default);
}
