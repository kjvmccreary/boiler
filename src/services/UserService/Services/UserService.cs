using AutoMapper;
using Common.Data; // 🔧 FIX: Use ApplicationDbContext instead of UserDbContext
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
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;
    private readonly ApplicationDbContext _context; // 🔧 FIX: Use ApplicationDbContext

    public UserService(
        IUserRepository userRepository,
        ITenantProvider tenantProvider,
        IMapper mapper,
        ILogger<UserService> logger,
        IRoleService roleService,
        IPermissionService permissionService,
        ApplicationDbContext context) // 🔧 FIX: Use ApplicationDbContext
    {
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;
        _mapper = mapper;
        _logger = logger;
        _roleService = roleService;
        _permissionService = permissionService;
        _context = context;
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
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Id == userId && x.u.IsActive && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                .Select(x => x.u)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.TenantId = currentTenantId.Value; // 🔧 FIX: Set TenantId from context
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
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Email.ToLower() == email.ToLower() && x.u.IsActive && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                .Select(x => x.u)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.TenantId = currentTenantId.Value; // 🔧 FIX: Set TenantId from context
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
            _logger.LogInformation("🔍 UserService.GetUsersAsync: Using tenant ID: {TenantId}", currentTenantId);
            
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult("Tenant context not found");
            }

            var query = _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.IsActive && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                .Select(x => x.u);

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
            
            // 🔧 FIX: Set TenantId from context for all users
            foreach (var userDto in userDtos)
            {
                userDto.TenantId = currentTenantId.Value;
            }

            var result = new PagedResultDto<UserDto>(userDtos, totalCount, request.PageNumber, request.PageSize);

            return ApiResponseDto<PagedResultDto<UserDto>>.SuccessResult(result, "Users retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            return ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult("An error occurred while retrieving users");
        }
    }

    public async Task<ApiResponseDto<UserDto>> CreateUserAsync(CreateUserDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            // 🔧 FIX: Check if user already exists in this tenant using TenantUsers join
            var existingUserInTenant = await _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Email.ToLower() == request.Email.ToLower() && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                .Select(x => x.u)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUserInTenant != null)
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
                IsActive = true,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // 🔧 FIX: Use transaction to create both User and TenantUser
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _userRepository.AddAsync(user, cancellationToken);

                // Create TenantUser relationship
                var tenantUser = new TenantUser
                {
                    TenantId = currentTenantId.Value,
                    UserId = user.Id,
                    Role = "User", // Default role
                    IsActive = true,
                    JoinedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.TenantUsers.Add(tenantUser);
                await _context.SaveChangesAsync(cancellationToken);
                
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.TenantId = currentTenantId.Value; // 🔧 FIX: Set TenantId from context
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

            // 🔧 FIX: Use TenantUsers join to find user
            var user = await _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Id == userId && x.u.IsActive && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                .Select(x => x.u)
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
            user.IsActive = updateUserDto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // 🔧 .NET 9 FIX: Handle optional Email updates safely
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                // 🔧 FIX: Check if email is already taken using TenantUsers join
                var emailExists = await _userRepository.Query()
                    .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                    .Where(x => x.u.Email == updateUserDto.Email.ToLower() && x.u.Id != userId && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                    .AnyAsync(cancellationToken);
                    
                if (emailExists)
                {
                    return ApiResponseDto<UserDto>.ErrorResult("Email address is already in use");
                }
                
                user.Email = updateUserDto.Email.ToLower();
                // Note: You might want to set EmailConfirmed = false and send confirmation email
            }

            await _userRepository.UpdateAsync(user, cancellationToken);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.TenantId = currentTenantId.Value; // 🔧 FIX: Set TenantId from context
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

            // 🔧 FIX: Use TenantUsers join to find user
            var user = await _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Id == userId && x.u.IsActive && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                .Select(x => x.u)
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

            // 🔧 FIX: Use TenantUsers join to check existence
            var exists = await _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Id == userId && x.u.IsActive && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                .AnyAsync(cancellationToken);

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
            // 🔧 FIX: Use TenantUsers join instead of direct TenantId filter
            var query = _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.tu.TenantId == tenantId && x.u.IsActive && x.tu.IsActive)
                .Select(x => x.u);

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

            var userSummaryDtos = _mapper.Map<List<UserSummaryDto>>(users);

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
            // 🔧 FIX: Use TenantUsers join instead of direct TenantId filter
            var user = await _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Id == userId && x.tu.TenantId == tenantId && x.u.IsActive && x.tu.IsActive)
                .Select(x => x.u)
                .Include(u => u.TenantUsers)
                .Include(u => u.RefreshTokens.Where(rt => rt.IsActive))
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDetailDto>.ErrorResult("User not found");
            }

            var userDetailDto = _mapper.Map<UserDetailDto>(user);
            userDetailDto.TenantId = tenantId; // 🔧 FIX: Set TenantId from parameter
            
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

            // 🔧 FIX: Verify user exists in current tenant using TenantUsers join
            var userExists = await _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Id == userId && x.tu.TenantId == currentTenantId.Value && x.u.IsActive && x.tu.IsActive)
                .AnyAsync(cancellationToken);

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

            // 🔧 FIX: Verify user exists in current tenant using TenantUsers join
            var userExists = await _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Id == userId && x.tu.TenantId == currentTenantId.Value && x.u.IsActive && x.tu.IsActive)
                .AnyAsync(cancellationToken);

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

            // 🔧 FIX: Use TenantUsers join to find user
            var user = await _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Id == userId && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                .Select(x => x.u)
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
