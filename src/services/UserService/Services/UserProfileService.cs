// FILE: src/services/UserService/Services/UserProfileService.cs
using AutoMapper;
using Common.Data;
using Contracts.Repositories;
using Contracts.Services;
using Contracts.User;
using DTOs.Common;
using DTOs.Entities;
using DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace UserService.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;
    private readonly ILogger<UserProfileService> _logger;
    private readonly ApplicationDbContext _context; // 🔧 ADD: Context for TenantUsers access

    public UserProfileService(
        IUserRepository userRepository,
        ITenantProvider tenantProvider,
        IPermissionService permissionService,
        IMapper mapper,
        ILogger<UserProfileService> logger,
        ApplicationDbContext context) // 🔧 ADD: Inject context
    {
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;
        _permissionService = permissionService;
        _mapper = mapper;
        _logger = logger;
        _context = context; // 🔧 ADD: Initialize context
    }

    public async Task<ApiResponseDto<UserDto>> GetUserProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            // 🔧 FIX: Use TenantUsers join to verify user access to current tenant
            var user = await _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Id == userId && x.u.IsActive && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                .Select(x => x.u)
                .Include(u => u.UserRoles.Where(ur => ur.IsActive && ur.TenantId == currentTenantId.Value))
                    .ThenInclude(ur => ur.Role)
                .Include(u => u.TenantUsers.Where(tu => tu.IsActive))
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User profile not found or access denied");
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.TenantId = currentTenantId.Value; // 🔧 FIX: Set TenantId from context
            
            _logger.LogInformation("User {UserId} successfully accessed profile in tenant {TenantId}", userId, currentTenantId.Value);
            
            return ApiResponseDto<UserDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving profile for user {UserId} in tenant context", userId);
            return ApiResponseDto<UserDto>.ErrorResult("An error occurred while retrieving your profile");
        }
    }

    public async Task<ApiResponseDto<UserDto>> UpdateUserProfileAsync(int userId, UserUpdateDto updateProfileDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserDto>.ErrorResult("Tenant context not found");
            }

            // Check if user has admin permissions
            var isAdmin = await IsUserAdminAsync(userId, cancellationToken);
            if (!isAdmin)
            {
                _logger.LogWarning("Non-admin user {UserId} attempted to update their profile. User permissions: {UserPermissions}", 
                    userId, string.Join(", ", await _permissionService.GetUserPermissionsAsync(userId, cancellationToken)));
                return ApiResponseDto<UserDto>.ErrorResult("Only admin users can update their profile");
            }

            // 🔧 FIX: Use TenantUsers join to find user
            var user = await _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Id == userId && x.u.IsActive && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                .Select(x => x.u)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User profile not found");
            }

            // Update profile fields (only the ones users can modify themselves)
            user.FirstName = updateProfileDto.FirstName;
            user.LastName = updateProfileDto.LastName;
            user.PhoneNumber = updateProfileDto.PhoneNumber;
            user.TimeZone = updateProfileDto.TimeZone;
            user.Language = updateProfileDto.Language;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            var userDto = _mapper.Map<UserDto>(user);
            userDto.TenantId = currentTenantId.Value; // 🔧 FIX: Set TenantId from context
            return ApiResponseDto<UserDto>.SuccessResult(userDto, "Profile updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
            return ApiResponseDto<UserDto>.ErrorResult("An error occurred while updating your profile");
        }
    }

    public async Task<ApiResponseDto<bool>> UpdateUserPreferencesAsync(int userId, UserPreferencesDto preferences, CancellationToken cancellationToken = default)
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

            user.Preferences = System.Text.Json.JsonSerializer.Serialize(preferences);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            return ApiResponseDto<bool>.SuccessResult(true, "Preferences updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating preferences for user {UserId}", userId);
            return ApiResponseDto<bool>.ErrorResult("An error occurred while updating preferences");
        }
    }

    public async Task<ApiResponseDto<UserPreferencesDto>> GetUserPreferencesAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!currentTenantId.HasValue)
            {
                return ApiResponseDto<UserPreferencesDto>.ErrorResult("Tenant context not found");
            }

            // 🔧 FIX: Use TenantUsers join to find user
            var user = await _userRepository.Query()
                .Join(_context.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
                .Where(x => x.u.Id == userId && x.u.IsActive && x.tu.TenantId == currentTenantId.Value && x.tu.IsActive)
                .Select(x => x.u)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserPreferencesDto>.ErrorResult("User not found");
            }

            UserPreferencesDto preferences;
            
            if (string.IsNullOrWhiteSpace(user.Preferences))
            {
                preferences = new UserPreferencesDto();
            }
            else
            {
                try
                {
                    preferences = System.Text.Json.JsonSerializer.Deserialize<UserPreferencesDto>(user.Preferences) 
                                 ?? new UserPreferencesDto();
                }
                catch (Exception)
                {
                    preferences = new UserPreferencesDto();
                }
            }

            return ApiResponseDto<UserPreferencesDto>.SuccessResult(preferences);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving preferences for user {UserId}", userId);
            return ApiResponseDto<UserPreferencesDto>.ErrorResult("An error occurred while retrieving preferences");
        }
    }

    private async Task<bool> IsUserAdminAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Use permission service to check if user has admin permissions
            var adminPermissions = new[]
            {
                "users.edit",
                "users.create", 
                "users.delete",
                "roles.edit",
                "tenants.edit"
            };

            var hasAdminPermissions = await _permissionService.UserHasAnyPermissionAsync(userId, adminPermissions, cancellationToken);
            
            _logger.LogInformation("🔧 ADMIN CHECK: Permission-based admin check for user {UserId}: {IsAdmin} (checked permissions: {Permissions})", 
                userId, hasAdminPermissions, string.Join(", ", adminPermissions));
            
            if (!hasAdminPermissions)
            {
                var userPermissions = await _permissionService.GetUserPermissionsAsync(userId, cancellationToken);
                _logger.LogWarning("🔧 ADMIN CHECK: User {UserId} failed admin check. User has {PermissionCount} permissions: {UserPermissions}", 
                    userId, userPermissions.Count(), string.Join(", ", userPermissions.Take(10)));
            }
            
            return hasAdminPermissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking admin permissions for user {UserId}", userId);
            return false;
        }
    }
}
