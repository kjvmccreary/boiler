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
        // ========================================
        // User Entity to DTO Mappings (Read Operations)
        // ========================================

        // User → UserDto (Main user representation)
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId ?? 0)) // Handle nullable TenantId
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
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => GetUserRoles(src)));

        // User → UserDetailDto (Comprehensive admin view)
        CreateMap<User, UserDetailDto>()
            .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId ?? 0))
            .ForMember(dest => dest.LockedUntil, opt => opt.MapFrom(src => src.LockedOutUntil))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => GetUserRoles(src)))
            .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => 
                DeserializeUserPreferences(src.Preferences)));

        // ========================================
        // DTO to User Entity Mappings (Write Operations)
        // ========================================

        // UserUpdateDto → User (Profile updates)
        CreateMap<UserUpdateDto, User>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Never update ID
            .ForMember(dest => dest.Email, opt => opt.Ignore()) // Email updates require separate process
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Password updates require separate process
            .ForMember(dest => dest.TenantId, opt => opt.Ignore()) // Tenant cannot be changed via profile update
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Never update CreatedAt
            .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore()) // Requires separate email confirmation process
            .ForMember(dest => dest.EmailConfirmationToken, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmationTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordResetTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.FailedLoginAttempts, opt => opt.Ignore())
            .ForMember(dest => dest.LockedOutUntil, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.PrimaryTenant, opt => opt.Ignore())
            .ForMember(dest => dest.TenantUsers, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
            .ForMember(dest => dest.Preferences, opt => opt.Ignore()); // Handled separately

        // UserCreateDto → User (New user creation)
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower())) // Normalize email
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.FailedLoginAttempts, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Set by database
            .ForMember(dest => dest.TenantId, opt => opt.Ignore()) // Set by service logic
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Handled separately with hashing
            .ForMember(dest => dest.EmailConfirmationToken, opt => opt.Ignore())
            .ForMember(dest => dest.EmailConfirmationTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordResetTokenExpiry, opt => opt.Ignore())
            .ForMember(dest => dest.LockedOutUntil, opt => opt.Ignore())
            .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
            .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore()) // Not in UserCreateDto
            .ForMember(dest => dest.TimeZone, opt => opt.Ignore()) // Not in UserCreateDto
            .ForMember(dest => dest.Language, opt => opt.Ignore()) // Not in UserCreateDto
            .ForMember(dest => dest.Preferences, opt => opt.Ignore()) // Set to default
            .ForMember(dest => dest.PrimaryTenant, opt => opt.Ignore())
            .ForMember(dest => dest.TenantUsers, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());

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
