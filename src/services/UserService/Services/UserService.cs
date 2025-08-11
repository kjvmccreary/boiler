using AutoMapper;
using Common.Repositories;
using Contracts.Repositories;
using Contracts.Services;
using Contracts.User;
using DTOs.Common;
using DTOs.Entities;
using DTOs.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        ITenantProvider tenantProvider,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponseDto<UserDto>> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive && u.TenantId == currentTenantId.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(userDto, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", userId);
            return ApiResponseDto<UserDto>.ErrorResult("An error occurred while retrieving the user");
        }
    }

    public async Task<ApiResponseDto<UserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            var user = await _userRepository.Query()
                .Where(u => u.Email.ToLower() == email.ToLower() && u.IsActive && u.TenantId == currentTenantId.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(userDto, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email {Email}", email);
            return ApiResponseDto<UserDto>.ErrorResult("An error occurred while retrieving the user");
        }
    }

    public async Task<ApiResponseDto<PagedResultDto<UserDto>>> GetUsersAsync(PaginationRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult("Tenant context not found");
            }

            var query = _userRepository.Query()
                .Where(u => u.IsActive && u.TenantId == currentTenantId.Value);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(u => u.FirstName.ToLower().Contains(searchTerm) ||
                                        u.LastName.ToLower().Contains(searchTerm) ||
                                        u.Email.ToLower().Contains(searchTerm));
            }

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var userDtos = _mapper.Map<List<UserDto>>(users);

            // 🔧 .NET 9 FIX: Use constructor instead of property assignment
            var result = new PagedResultDto<UserDto>(userDtos, totalCount, request.PageNumber, request.PageSize);

            return ApiResponseDto<PagedResultDto<UserDto>>.SuccessResult(result, "Users retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult("An error occurred while retrieving users");
        }
    }

    public async Task<ApiResponseDto<UserDto>> CreateUserAsync(UserCreateDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            // Check if user already exists in this tenant
            var existingUser = await _userRepository.Query()
                .Where(u => u.Email.ToLower() == request.Email.ToLower() && u.TenantId == currentTenantId.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUser != null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User with this email already exists in this tenant");
            }

            // Create new user entity
            var user = new User
            {
                Email = request.Email.ToLower(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = request.Password, // Note: Should be hashed by calling service
                TenantId = currentTenantId.Value,
                IsActive = true,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user, cancellationToken);

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(userDto, "User created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user {Email}", request.Email);
            return ApiResponseDto<UserDto>.ErrorResult("An error occurred while creating the user");
        }
    }

    public async Task<ApiResponseDto<UserDto>> UpdateUserAsync(int userId, UserUpdateDto updateUserDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive && u.TenantId == currentTenantId.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User not found");
            }

            // Update admin-editable fields
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.TimeZone = updateUserDto.TimeZone;
            user.Language = updateUserDto.Language;
            user.UpdatedAt = DateTime.UtcNow;

            // 🔧 .NET 9 FIX: Handle optional Email updates safely
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                // Check if email is already taken
                var emailExists = await _userRepository.Query()
                    .AnyAsync(u => u.Email == updateUserDto.Email.ToLower() && u.Id != userId && u.TenantId == currentTenantId.Value, cancellationToken);
                    
                if (emailExists)
                {
                    return ApiResponseDto<UserDto>.ErrorResult("Email address is already in use");
                }
                
                user.Email = updateUserDto.Email.ToLower();
                // Note: You might want to set EmailConfirmed = false and send confirmation email
            }

            await _userRepository.UpdateAsync(user, cancellationToken);

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(userDto, "User updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", userId);
            return ApiResponseDto<UserDto>.ErrorResult("An error occurred while updating the user");
        }
    }

    public async Task<ApiResponseDto<bool>> DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<bool>.ErrorResult("Tenant context not found");
            }

            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive && u.TenantId == currentTenantId.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<bool>.ErrorResult("User not found");
            }

            // Soft delete - set IsActive to false
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            return ApiResponseDto<bool>.SuccessResult(true, "User deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", userId);
            return ApiResponseDto<bool>.ErrorResult("An error occurred while deleting the user");
        }
    }

    public async Task<ApiResponseDto<bool>> ExistsAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<bool>.ErrorResult("Tenant context not found");
            }

            var exists = await _userRepository.Query()
                .AnyAsync(u => u.Id == userId && u.IsActive && u.TenantId == currentTenantId.Value, cancellationToken);

            return ApiResponseDto<bool>.SuccessResult(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} exists", userId);
            return ApiResponseDto<bool>.ErrorResult("An error occurred while checking user existence");
        }
    }

    public async Task<ApiResponseDto<PagedResultDto<UserSummaryDto>>> GetUsersSummaryAsync(int tenantId, PaginationRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _userRepository.Query()
                .Where(u => u.TenantId == tenantId && u.IsActive);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(u => u.FirstName.ToLower().Contains(searchTerm) ||
                                        u.LastName.ToLower().Contains(searchTerm) ||
                                        u.Email.ToLower().Contains(searchTerm));
            }

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query
                .Include(u => u.TenantUsers)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // 🔧 .NET 9 FIX: Use AutoMapper for consistent mapping
            var userSummaryDtos = _mapper.Map<List<UserSummaryDto>>(users);

            // 🔧 .NET 9 FIX: Use constructor instead of property assignment
            var result = new PagedResultDto<UserSummaryDto>(userSummaryDtos, totalCount, request.PageNumber, request.PageSize);

            return ApiResponseDto<PagedResultDto<UserSummaryDto>>.SuccessResult(result, "User summaries retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user summaries for tenant {TenantId}", tenantId);
            return ApiResponseDto<PagedResultDto<UserSummaryDto>>.ErrorResult("An error occurred while retrieving user summaries");
        }
    }

    public async Task<ApiResponseDto<UserDetailDto>> GetUserDetailAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.TenantId == tenantId && u.IsActive)
                .Include(u => u.TenantUsers)
                .Include(u => u.RefreshTokens.Where(rt => rt.IsActive))
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDetailDto>.ErrorResult("User not found");
            }

            // 🔧 .NET 9 FIX: Use AutoMapper and manually set ActiveSessions
            var userDetailDto = _mapper.Map<UserDetailDto>(user);
            
            // Set ActiveSessions count manually
            userDetailDto.ActiveSessions = user.RefreshTokens.Count(rt => rt.IsActive && rt.ExpiryDate > DateTime.UtcNow);

            return ApiResponseDto<UserDetailDto>.SuccessResult(userDetailDto, "User details retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user detail {UserId} for tenant {TenantId}", userId, tenantId);
            return ApiResponseDto<UserDetailDto>.ErrorResult("An error occurred while retrieving user details");
        }
    }

    #region Helper Methods

    private static IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderBy(u => u.LastName).ThenBy(u => u.FirstName);
        }

        var isDescending = sortDirection?.ToLower() == "desc";

        return sortBy.ToLower() switch
        {
            "email" => isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "firstname" => isDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
            "lastname" => isDescending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
            "createdat" => isDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            "updatedat" => isDescending ? query.OrderByDescending(u => u.UpdatedAt) : query.OrderBy(u => u.UpdatedAt),
            "lastloginat" => isDescending ? query.OrderByDescending(u => u.LastLoginAt) : query.OrderBy(u => u.LastLoginAt),
            _ => query.OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
        };
    }

    #endregion
}
