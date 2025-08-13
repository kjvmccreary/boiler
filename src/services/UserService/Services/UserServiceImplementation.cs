// FILE: src/services/UserService/Services/UserServiceImplementation.cs
using AutoMapper;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Common;
using DTOs.Entities;
using DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace UserService.Services;

// Explicitly specify which IUserService we're implementing
public class UserServiceImplementation : Contracts.User.IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<UserServiceImplementation> _logger;
    // 🔧 .NET 9 FIX: Add dependencies for role/permission functionality
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;

    public UserServiceImplementation(
        IUserRepository userRepository,
        ITenantProvider tenantProvider,
        IMapper mapper,
        ILogger<UserServiceImplementation> logger,
        IRoleService roleService,
        IPermissionService permissionService)
    {
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;
        _mapper = mapper;
        _logger = logger;
        _roleService = roleService;
        _permissionService = permissionService;
    }

    public async Task<ApiResponseDto<UserDto>> GetUserByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // FIXED: Apply tenant filtering explicitly
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive && u.TenantId == currentTenantId.Value)
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.TenantUsers) // Keep for fallback compatibility
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", userId);
            return ApiResponseDto<UserDto>.ErrorResult("An error occurred while retrieving the user");
        }
    }

    public async Task<ApiResponseDto<UserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            // FIXED: Apply tenant filtering explicitly
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            var user = await _userRepository.Query()
                .Where(u => u.Email.ToLower() == email.ToLower() && u.IsActive && u.TenantId == currentTenantId.Value)
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.TenantUsers) // Keep for fallback compatibility
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by email {Email}", email);
            return ApiResponseDto<UserDto>.ErrorResult("An error occurred while retrieving the user");
        }
    }

    public async Task<ApiResponseDto<PagedResultDto<UserDto>>> GetUsersAsync(PaginationRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            // FIXED: Apply tenant filtering explicitly
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult("Tenant context not found");
            }

            var query = _userRepository.Query()
                .Where(u => u.IsActive && u.TenantId == currentTenantId.Value);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(searchLower) ||
                    u.LastName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower));
            }

            // Apply sorting BEFORE Include
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            // Count total items BEFORE applying pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination and Include
            var users = await query
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.TenantUsers) // Keep for fallback compatibility
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var userDtos = _mapper.Map<List<UserDto>>(users);

            var pagedResult = new PagedResultDto<UserDto>(userDtos, totalCount, request.PageNumber, request.PageSize);

            return ApiResponseDto<PagedResultDto<UserDto>>.SuccessResult(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult("An error occurred while retrieving users");
        }
    }

    public async Task<ApiResponseDto<UserDto>> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _userRepository.Query()
                .Where(u => u.Email.ToLower() == request.Email.ToLower())
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUser != null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User with this email already exists");
            }

            // Get current tenant ID
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (currentTenantId == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
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

    public async Task<ApiResponseDto<UserDto>> UpdateUserAsync(int userId, UserUpdateDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            // FIXED: Apply tenant filtering explicitly
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

            // ✅ ADD: Handle email updates with validation
            if (!string.IsNullOrEmpty(request.Email) && request.Email.ToLower() != user.Email.ToLower())
            {
                // Check if email is already taken
                var emailExists = await _userRepository.Query()
                    .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != userId && u.TenantId == currentTenantId.Value, cancellationToken);
                    
                if (emailExists)
                {
                    return ApiResponseDto<UserDto>.ErrorResult("Email address is already in use");
                }
                
                user.Email = request.Email.ToLower();
                // Note: You might want to set EmailConfirmed = false and send confirmation email
            }

            // Update user fields
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.TimeZone = request.TimeZone;
            user.Language = request.Language;
            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // Update roles if provided (admin operation)
            if (request.Roles.Any())
            {
                // Note: This would typically involve updating TenantUser relationships
                // For now, we'll store the primary role
                var primaryRole = request.Roles.FirstOrDefault();
                if (!string.IsNullOrEmpty(primaryRole))
                {
                    // Update the user's primary role in TenantUser relationship
                    var tenantUser = user.TenantUsers.FirstOrDefault(tu => tu.TenantId == user.TenantId);
                    if (tenantUser != null)
                    {
                        tenantUser.Role = primaryRole;
                    }
                }
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
            // ✅ FIXED: Apply tenant filtering and check existence
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<bool>.ErrorResult("Tenant context not found");
            }

            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.TenantId == currentTenantId.Value)
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
            // FIXED: Apply tenant filtering explicitly
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

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(searchLower) ||
                    u.LastName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower));
            }

            // Apply sorting BEFORE Include
            query = ApplySorting(query, request.SortBy, request.SortDirection);

            // Count total items BEFORE applying pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination and Include
            var users = await query
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.TenantUsers) // Keep for fallback compatibility
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var userSummaryDtos = _mapper.Map<List<UserSummaryDto>>(users);

            var pagedResult = new PagedResultDto<UserSummaryDto>(userSummaryDtos, totalCount, request.PageNumber, request.PageSize);

            return ApiResponseDto<PagedResultDto<UserSummaryDto>>.SuccessResult(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users summary for tenant {TenantId}", tenantId);
            return ApiResponseDto<PagedResultDto<UserSummaryDto>>.ErrorResult("An error occurred while retrieving users");
        }
    }

    public async Task<ApiResponseDto<UserDetailDto>> GetUserDetailAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.TenantId == tenantId && u.IsActive)
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.TenantUsers) // Keep for fallback compatibility
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDetailDto>.ErrorResult("User not found");
            }

            var userDetailDto = _mapper.Map<UserDetailDto>(user);
            return ApiResponseDto<UserDetailDto>.SuccessResult(userDetailDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user detail {UserId} for tenant {TenantId}", userId, tenantId);
            return ApiResponseDto<UserDetailDto>.ErrorResult("An error occurred while retrieving user details");
        }
    }

    #region Helper Methods

    private static IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, string sortDirection)
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

    // 🔧 .NET 9 FIX: Add missing role-related methods
    public async Task<ApiResponseDto<List<string>>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<List<string>>.ErrorResult("Tenant context not found");
            }

            var userRoles = await _roleService.GetUserRolesAsync(userId, cancellationToken);
            var roleNames = userRoles.Select(r => r.Name).ToList();

            return ApiResponseDto<List<string>>.SuccessResult(roleNames, "User roles retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
            return ApiResponseDto<List<string>>.ErrorResult("An error occurred while retrieving user roles");
        }
    }

    public async Task<ApiResponseDto<bool>> AssignRoleToUserAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<bool>.ErrorResult("Tenant context not found");
            }

            // Verify user exists in current tenant
            var userExists = await _userRepository.Query()
                .AnyAsync(u => u.Id == userId && u.TenantId == currentTenantId.Value && u.IsActive, cancellationToken);

            if (!userExists)
            {
                return ApiResponseDto<bool>.ErrorResult("User not found");
            }

            await _roleService.AssignRoleToUserAsync(userId, roleId, cancellationToken);
            return ApiResponseDto<bool>.SuccessResult(true, "Role assigned to user successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
            return ApiResponseDto<bool>.ErrorResult("An error occurred while assigning the role");
        }
    }

    public async Task<ApiResponseDto<bool>> RemoveRoleFromUserAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<bool>.ErrorResult("Tenant context not found");
            }

            // Verify user exists in current tenant
            var userExists = await _userRepository.Query()
                .AnyAsync(u => u.Id == userId && u.TenantId == currentTenantId.Value && u.IsActive, cancellationToken);

            if (!userExists)
            {
                return ApiResponseDto<bool>.ErrorResult("User not found");
            }

            await _roleService.RemoveRoleFromUserAsync(userId, roleId, cancellationToken);
            return ApiResponseDto<bool>.SuccessResult(true, "Role removed from user successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            return ApiResponseDto<bool>.ErrorResult("An error occurred while removing the role");
        }
    }

    public async Task<ApiResponseDto<List<string>>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<List<string>>.ErrorResult("Tenant context not found");
            }

            var permissions = await _permissionService.GetUserPermissionsForTenantAsync(userId, currentTenantId.Value, cancellationToken);
            return ApiResponseDto<List<string>>.SuccessResult(permissions.ToList(), "User permissions retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions for user {UserId}", userId);
            return ApiResponseDto<List<string>>.ErrorResult("An error occurred while retrieving user permissions");
        }
    }

    public async Task<ApiResponseDto<bool>> UpdateUserStatusAsync(int userId, bool isActive, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<bool>.ErrorResult("Tenant context not found");
            }

            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.TenantId == currentTenantId.Value)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<bool>.ErrorResult("User not found");
            }

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            return ApiResponseDto<bool>.SuccessResult(true, $"User status updated to {(isActive ? "active" : "inactive")} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for user {UserId}", userId);
            return ApiResponseDto<bool>.ErrorResult("An error occurred while updating user status");
        }
    }
}
