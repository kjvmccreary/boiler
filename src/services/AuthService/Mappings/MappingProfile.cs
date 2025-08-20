// FILE: src/services/AuthService/Mappings/MappingProfile.cs
using AutoMapper;
using DTOs.Entities;
using DTOs.Tenant;
using DTOs.User;

namespace AuthService.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.TenantId, opt => opt.Ignore()) // 🔧 FIX: TenantId will be set from context
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                // 🔧 FIX: Use RBAC UserRoles instead of legacy TenantUsers
                src.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role.Name).ToList()));

        // Tenant mappings
        CreateMap<Tenant, TenantDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Both are int now
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src =>
                ParseJsonSettings(src.Settings)));
    }

    private static Dictionary<string, object> ParseJsonSettings(string settingsJson)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(settingsJson) || settingsJson == "{}")
                return new Dictionary<string, object>();

            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(settingsJson)
                   ?? new Dictionary<string, object>();
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }
}
