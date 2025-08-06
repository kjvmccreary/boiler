// FILE: src/services/UserService/Services/UserProfileService.cs
using AutoMapper;
using Contracts.Repositories;
using Contracts.User;
using DTOs.Common;
using DTOs.Entities;
using DTOs.User;
using Microsoft.EntityFrameworkCore;

namespace UserService.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserProfileService> _logger;

    public UserProfileService(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserProfileService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponseDto<UserDto>> GetUserProfileAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive)
                .Include(u => u.TenantUsers)
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
            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive)
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

            // Note: IsActive and Roles are not updated here (admin-only fields)

            await _userRepository.UpdateAsync(user, cancellationToken);

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(userDto);
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
            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<bool>.ErrorResult("User not found");
            }

            // Serialize preferences to JSON for database storage
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
            var user = await _userRepository.Query()
                .Where(u => u.Id == userId && u.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<UserPreferencesDto>.ErrorResult("User not found");
            }

            UserPreferencesDto preferences;
            
            if (string.IsNullOrWhiteSpace(user.Preferences))
            {
                // Return default preferences if none exist
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
                    // If deserialization fails, return default preferences
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
}
