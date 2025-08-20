// FILE: src/services/UserService/Mappings/UserMappingProfile.cs
using AutoMapper;
using DTOs.Entities;
using DTOs.User;
using System.Text.Json;

namespace UserService.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // User → UserDto (Main user representation)
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.TenantId, opt => opt.Ignore()) // 🔧 FIX: Set from context
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => GetUserRoles(src)))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.TimeZone, opt => opt.MapFrom(src => src.TimeZone))
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language))
            .ForMember(dest => dest.LastLoginAt, opt => opt.MapFrom(src => src.LastLoginAt))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
            .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => 
                DeserializeUserPreferences(src.Preferences)));

        // User → UserSummaryDto (Lightweight for lists)  
        CreateMap<User, UserSummaryDto>()
            //.ForMember(dest => dest.TenantId, opt => opt.Ignore()) // 🔧 ADD: Set from context
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => GetUserRoles(src)));

        // User → UserDetailDto (Comprehensive admin view)
        CreateMap<User, UserDetailDto>()
            .ForMember(dest => dest.TenantId, opt => opt.Ignore()) // 🔧 FIX: Set from context  
            .ForMember(dest => dest.LockedUntil, opt => opt.MapFrom(src => src.LockedOutUntil))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => GetUserRoles(src)))
            .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => 
                DeserializeUserPreferences(src.Preferences)));

        // UserUpdateDto → User (Profile updates)
        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmationToken, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmationTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordResetTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.FailedLoginAttempts, opt => opt.Ignore())
            .ForMember(dest => dest.LockedOutUntil, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.TenantUsers, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore()) // 🔧 ADD: UserRoles navigation
            .ForMember(dest => dest.Preferences, opt => opt.Ignore());

        // UserCreateDto → User (New user creation)
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FailedLoginAttempts, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmationToken, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmationTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordResetTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.LockedOutUntil, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
            .ForMember(dest => dest.TimeZone, opt => opt.Ignore())
            .ForMember(dest => dest.Language, opt => opt.Ignore())
            .ForMember(dest => dest.Preferences, opt => opt.Ignore())
            .ForMember(dest => dest.TenantUsers, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore()); // 🔧 ADD: UserRoles navigation

        // ========================================
        // Preferences Mappings
        // ========================================

        // UserPreferencesDto → JSON string (for database storage)
        CreateMap<UserPreferencesDto, string>()
            .ConvertUsing(src => JsonSerializer.Serialize(src, (JsonSerializerOptions?)null));

        // JSON string → UserPreferencesDto (from database)
        CreateMap<string, UserPreferencesDto?>()
            .ConvertUsing(src => DeserializeUserPreferences(src));
    }

    #region Helper Methods

    /// <summary>
    /// Safely deserializes user preferences from JSON string
    /// </summary>
    private static UserPreferencesDto? DeserializeUserPreferences(string? preferencesJson)
    {
        if (string.IsNullOrWhiteSpace(preferencesJson))
            return null;

        try
        {
            return JsonSerializer.Deserialize<UserPreferencesDto>(preferencesJson);
        }
        catch (JsonException)
        {
            // If deserialization fails, return default preferences
            return new UserPreferencesDto();
        }
    }

    // Helper method to prioritize RBAC roles over legacy roles
    private static List<string> GetUserRoles(User user)
    {
        // 🔧 FIX: Prefer RBAC UserRoles over legacy TenantUsers
        if (user.UserRoles != null && user.UserRoles.Any(ur => ur.IsActive))
        {
            return user.UserRoles
                .Where(ur => ur.IsActive && ur.Role != null)
                .Select(ur => ur.Role.Name)
                .ToList();
        }

        // Fallback to legacy TenantUsers if no RBAC roles found
        if (user.TenantUsers != null && user.TenantUsers.Any(tu => tu.IsActive))
        {
            return user.TenantUsers
                .Where(tu => tu.IsActive)
                .Select(tu => tu.Role)
                .ToList();
        }

        return new List<string>();
    }

    #endregion
}
