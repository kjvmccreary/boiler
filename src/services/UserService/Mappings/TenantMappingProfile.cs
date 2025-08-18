using AutoMapper;
using DTOs.Entities;
using DTOs.Tenant;
using System.Text.Json;

namespace UserService.Mappings;

public class TenantMappingProfile : Profile
{
    public TenantMappingProfile()
    {
        CreateMap<Tenant, TenantDto>()
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => ParseSettings(src.Settings)))
            .ForMember(dest => dest.UserCount, opt => opt.Ignore())
            .ForMember(dest => dest.RoleCount, opt => opt.Ignore())
            .ForMember(dest => dest.ActiveUserCount, opt => opt.Ignore());

        CreateMap<CreateTenantDto, Tenant>()
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => SerializeSettings(src.Settings)))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Users, opt => opt.Ignore())
            .ForMember(dest => dest.TenantUsers, opt => opt.Ignore());

        CreateMap<UpdateTenantDto, Tenant>()
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => SerializeSettings(src.Settings)))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Users, opt => opt.Ignore())
            .ForMember(dest => dest.TenantUsers, opt => opt.Ignore());
    }
    
    // Helper methods to avoid expression tree compilation issues
    private static Dictionary<string, object> ParseSettings(string? settings)
    {
        if (string.IsNullOrEmpty(settings))
            return new Dictionary<string, object>();
            
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(settings) ?? new Dictionary<string, object>();
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }
    
    private static string SerializeSettings(Dictionary<string, object> settings)
    {
        try
        {
            return JsonSerializer.Serialize(settings);
        }
        catch
        {
            return "{}";
        }
    }
}
