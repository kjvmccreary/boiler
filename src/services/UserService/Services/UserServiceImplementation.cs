// FILE: src/services/UserService/Services/UserServiceImplementation.cs
using AutoMapper;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Common;
using DTOs.Entities;
using DTOs.User;
using Microsoft.EntityFrameworkCore;
using Common.Data; // 🔧 ADD: Import for ApplicationDbContext

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
    private readonly IPasswordService _passwordService;
    private readonly ApplicationDbContext _context; // 🔧 ADD: Missing context field

    public UserServiceImplementation(
        IUserRepository userRepository,
        ITenantProvider tenantProvider,
        IMapper mapper,
        ILogger<UserServiceImplementation> logger,
        IRoleService roleService,
        IPermissionService permissionService,
        IPasswordService passwordService,
        ApplicationDbContext context) // 🔧 ADD: Inject ApplicationDbContext
    {
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;
        _mapper = mapper;
        _logger = logger;
        _roleService = roleService;
        _permissionService = permissionService;
        _passwordService = passwordService;
        _context = context; // 🔧 ADD: Initialize context field
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

            // 🔧 FIX: Use TenantUsers join instead of direct User.TenantId filtering
            var user = await (from u in _userRepository.Query()
                              join tu in _context.TenantUsers on u.Id equals tu.UserId
                              where u.Id == userId && u.IsActive && tu.TenantId == currentTenantId.Value && tu.IsActive
                              select u)
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.TenantUsers)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User not found");
            }

            // 🔧 MINIMAL CHANGE: Set TenantId after mapping
            var userDto = _mapper.Map<UserDto>(user);
            userDto.TenantId = currentTenantId.Value; // ✅ Set correct TenantId
            
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
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            // 🔧 FIX: Use TenantUsers join instead of direct User.TenantId filtering
            var user = await (from u in _userRepository.Query()
                              join tu in _context.TenantUsers on u.Id equals tu.UserId
                              where u.Email.ToLower() == email.ToLower() && u.IsActive 
                                    && tu.TenantId == currentTenantId.Value && tu.IsActive
                              select u)
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.TenantUsers)
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
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult("Tenant context not found");
            }

            // 🔧 CRITICAL FIX: Explicit tenant filtering using TenantUsers join
            var query = from u in _userRepository.Query()
                        join tu in _context.TenantUsers on u.Id equals tu.UserId
                        where u.IsActive && tu.TenantId == currentTenantId.Value && tu.IsActive
                        select u;

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(searchLower) ||
                    u.LastName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower));
            }

            query = ApplySorting(query, request.SortBy, request.SortDirection);
            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.TenantUsers)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Set TenantId correctly
            var userDtos = users.Select(u =>
            {
                var dto = _mapper.Map<UserDto>(u);
                dto.TenantId = currentTenantId.Value;
                return dto;
            }).ToList();

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
            // Get current tenant ID
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (currentTenantId == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            _logger.LogInformation("Creating/adding user {Email} for tenant {TenantId}", request.Email, currentTenantId.Value);

            // 🔧 FIX: Use context with IgnoreQueryFilters to find user across all tenants
            var existingUser = await _context.Users
                .IgnoreQueryFilters()  // This is critical - ignore tenant filters
                .Where(u => u.Email.ToLower() == request.Email.ToLower())
                .FirstOrDefaultAsync(cancellationToken);

            if (existingUser != null)
            {
                // User exists - check if they already have a TenantUser record for current tenant
                var existingTenantUser = await _context.TenantUsers
                    .FirstOrDefaultAsync(tu => tu.UserId == existingUser.Id && 
                                              tu.TenantId == currentTenantId.Value && 
                                              tu.IsActive, 
                                        cancellationToken);

                if (existingTenantUser != null)
                {
                    // User already exists in this tenant - return graceful message
                    _logger.LogInformation("User {Email} already exists in tenant {TenantId}", request.Email, currentTenantId.Value);
                    var existingUserDto = _mapper.Map<UserDto>(existingUser);
                    return ApiResponseDto<UserDto>.SuccessResult(existingUserDto, 
                        $"User '{request.Email}' already exists in this organization");
                }
                else
                {
                    // User exists but not in current tenant - create TenantUser relationship
                    _logger.LogInformation("Adding existing user {Email} (ID: {UserId}) to tenant {TenantId}", 
                        request.Email, existingUser.Id, currentTenantId.Value);

                    var tenantUser = new TenantUser
                    {
                        UserId = existingUser.Id,
                        TenantId = currentTenantId.Value,
                        Role = "User", // Default role - can be updated later
                        IsActive = true,
                        JoinedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.TenantUsers.Add(tenantUser);
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("Successfully added user {Email} to tenant {TenantId}", request.Email, currentTenantId.Value);

                    // Return existing user with enhanced success message
                    var addedUserDto = _mapper.Map<UserDto>(existingUser);
                    
                    // 🔧 ENHANCED: More informative message about existing user behavior
                    var detailedMessage = $"User '{request.Email}' has been successfully added to this organization. " +
                        $"Note: This user already exists in the system with the name '{existingUser.FirstName} {existingUser.LastName}'. " +
                        $"Their existing personal information and password have been preserved and were not updated with the form data provided.";
                    
                    return ApiResponseDto<UserDto>.SuccessResult(addedUserDto, detailedMessage);
                }
            }
            else
            {
                // User doesn't exist - create new user and TenantUser relationship
                _logger.LogInformation("Creating new user {Email} for tenant {TenantId}", request.Email, currentTenantId.Value);

                // Create new user entity
                var user = new User
                {
                    Email = request.Email.ToLower(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PasswordHash = _passwordService.HashPassword(request.Password),
                    IsActive = true,
                    EmailConfirmed = true, // Set to true for admin-created users
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Use transaction to ensure both User and TenantUser are created together
                var strategy = _context.Database.CreateExecutionStrategy();
                var result = await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        // Create the user
                        await _userRepository.AddAsync(user, cancellationToken);

                        // Create TenantUser relationship
                        var tenantUser = new TenantUser
                        {
                            UserId = user.Id,
                            TenantId = currentTenantId.Value,
                            Role = "User", // Default role - can be updated later
                            IsActive = true,
                            JoinedAt = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        _context.TenantUsers.Add(tenantUser);
                        await _context.SaveChangesAsync(cancellationToken);

                        await transaction.CommitAsync(cancellationToken);

                        _logger.LogInformation("Successfully created new user {Email} (ID: {UserId}) for tenant {TenantId}", 
                            request.Email, user.Id, currentTenantId.Value);

                        var userDto = _mapper.Map<UserDto>(user);
                        return ApiResponseDto<UserDto>.SuccessResult(userDto, "User created successfully");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        _logger.LogError(ex, "Transaction failed while creating user {Email}", request.Email);
                        throw;
                    }
                });

                return result;
            }
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
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            // 🔧 FIX: Use TenantUsers join to find user instead of User.TenantId
            var user = await (from u in _userRepository.Query()
                              join tu in _context.TenantUsers on u.Id equals tu.UserId
                              where u.Id == userId && u.IsActive && tu.TenantId == currentTenantId.Value && tu.IsActive
                              select u)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User not found");
            }

            // ✅ ADD: Handle email updates with validation
            if (!string.IsNullOrEmpty(request.Email) && request.Email.ToLower() != user.Email.ToLower())
            {
                // Check if email is already taken (check across all tenants since email should be unique)
                var emailExists = await _userRepository.Query()
                    .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != userId, cancellationToken);
                    
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
                    var tenantUser = user.TenantUsers.FirstOrDefault(tu => tu.TenantId == currentTenantId.Value);
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
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<bool>.ErrorResult("Tenant context not found");
            }

            // 🔧 FIX: Use TenantUsers join to find user instead of User.TenantId
            var user = await (from u in _userRepository.Query()
                              join tu in _context.TenantUsers on u.Id equals tu.UserId
                              where u.Id == userId && tu.TenantId == currentTenantId.Value && tu.IsActive
                              select u)
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

            // 🔧 FIX: Use TenantUsers join to check if user exists in tenant
            var exists = await (from u in _userRepository.Query()
                                join tu in _context.TenantUsers on u.Id equals tu.UserId
                                where u.Id == userId && u.IsActive && tu.TenantId == currentTenantId.Value && tu.IsActive
                                select u)
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
            // 🔧 FIX: Use TenantUsers join for summary as well
            var query = from u in _userRepository.Query()
                        join tu in _context.TenantUsers on u.Id equals tu.UserId
                        where u.IsActive && tu.TenantId == tenantId && tu.IsActive
                        select u;

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
            // 🔧 FIX: Use TenantUsers join for detail as well
            var user = await (from u in _userRepository.Query()
                              join tu in _context.TenantUsers on u.Id equals tu.UserId
                              where u.Id == userId && u.IsActive && tu.TenantId == tenantId && tu.IsActive
                              select u)
                .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.TenantUsers)
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

            // 🔧 FIX: Use TenantUsers join to verify user exists in current tenant
            var userExists = await (from u in _userRepository.Query()
                                    join tu in _context.TenantUsers on u.Id equals tu.UserId
                                    where u.Id == userId && u.IsActive && tu.TenantId == currentTenantId.Value && tu.IsActive
                                    select u)
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

            // 🔧 FIX: Use TenantUsers join to verify user exists in current tenant
            var userExists = await (from u in _userRepository.Query()
                                    join tu in _context.TenantUsers on u.Id equals tu.UserId
                                    where u.Id == userId && u.IsActive && tu.TenantId == currentTenantId.Value && tu.IsActive
                                    select u)
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

            // 🔧 FIX: Use TenantUsers join to find user instead of User.TenantId
            var user = await (from u in _userRepository.Query()
                              join tu in _context.TenantUsers on u.Id equals tu.UserId
                              where u.Id == userId && tu.TenantId == currentTenantId.Value && tu.IsActive
                              select u)
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

    // Add this method to properly set TenantId after mapping
    private async Task<UserDto> MapUserToDtoWithTenantAsync(User user)
    {
        var userDto = _mapper.Map<UserDto>(user);
        
        // 🔧 CRITICAL FIX: Set TenantId from tenant context
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (tenantId.HasValue)
        {
            userDto.TenantId = tenantId.Value;
        }
        
        return userDto;
    }

    // Update your GetUsersAsync method to use this:
    public async Task<ApiResponseDto<PagedResultDto<UserDto>>> GetUsersAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Users
                .Include(u => u.UserRoles!)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => 
                    u.FirstName.Contains(searchTerm) || 
                    u.LastName.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // 🔧 CRITICAL FIX: Map with tenant context
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            var userDtos = users.Select(u =>
            {
                var dto = _mapper.Map<UserDto>(u);
                if (tenantId.HasValue)
                {
                    dto.TenantId = tenantId.Value;
                }
                return dto;
            }).ToList();

            var result = new PagedResultDto<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            return ApiResponseDto<PagedResultDto<UserDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult(
                "An error occurred while retrieving users");
        }
    }
}
