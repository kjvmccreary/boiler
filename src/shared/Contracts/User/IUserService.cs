// src/shared/Contracts/User/IUserService.cs
using DTOs.User;
using DTOs.Common;

namespace Contracts.User;

public interface IUserService
{
    Task<ApiResponseDto<UserDto>> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<UserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<PagedResultDto<UserDto>>> GetUsersAsync(PaginationRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<UserDto>> CreateUserAsync(UserCreateDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<UserDto>> UpdateUserAsync(int userId, UserUpdateDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> DeleteUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> ExistsAsync(int userId, CancellationToken cancellationToken = default);
}
