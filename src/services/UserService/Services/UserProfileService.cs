// FILE: src/services/UserService/Services/UserProfileService.cs
using AutoMapper;
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
    private readonly IPermissionService _permissionService; // 🔧 ADD: Permission service for admin check
    private readonly IMapper _mapper;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(
        IUserRepository userRepository,
        ITenantProvider tenantProvider,
        IPermissionService permissionService, // 🔧 ADD: Inject permission service
        IMapper mapper,
        ILogger<UserProfileService> logger)
    {
        _userRepository = userRepository;
        _tenantProvider = tenantProvider;
        _permissionService = permissionService; // 🔧 ADD: Initialize permission service
        _mapper = mapper;
        _logger = logger;
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

            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive && u.TenantId == currentTenantId.Value)
                .Include(u => u.UserRoles.Where(ur => ur.IsActive)) // ✅ ADD: Include RBAC roles
                    .ThenInclude(ur => ur.Role) // ✅ ADD: Include Role details
                .Include(u => u.TenantUsers) // Keep for fallback
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserDto>.ErrorResult("User profile not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving profile for user {UserId}", userId);
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

            // 🔧 .NET 9 FIX: Use permission-based admin check instead of role-based
            var isAdmin = await IsUserAdminAsync(userId, cancellationToken);
            if (!isAdmin)
            {
                _logger.LogWarning("Non-admin user {UserId} attempted to update their profile. User permissions: {UserPermissions}", 
                    userId, string.Join(", ", await _permissionService.GetUserPermissionsAsync(userId, cancellationToken)));
                return ApiResponseDto<UserDto>.ErrorResult("Only admin users can update their profile");
            }

            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive && u.TenantId == currentTenantId.Value)
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

            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive && u.TenantId == currentTenantId.Value)
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

            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive && u.TenantId == currentTenantId.Value)
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

    // 🔧 .NET 9 FIX: Use permission-based admin check instead of role-based
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
