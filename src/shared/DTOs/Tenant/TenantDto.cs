// FILE: src/shared/DTOs/Tenant/TenantDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.Tenant;

public class TenantDto
{
    public int Id { get; set; } // Already matches Tenant entity
    public required string Name { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public required string SubscriptionPlan { get; set; } = "Basic";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Dictionary<string, object> Settings { get; set; } = new();
    
    // 🆕 NEW: Statistics for tenant management
    public int UserCount { get; set; }
    public int RoleCount { get; set; }
    public int ActiveUserCount { get; set; }
}

// 🆕 NEW: Create Tenant DTO
public class CreateTenantDto
{
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
    
    [StringLength(255)]
    public string? Domain { get; set; }
    
    [Required]
    [StringLength(50)]
    public string SubscriptionPlan { get; set; } = "Basic";
    
    public Dictionary<string, object> Settings { get; set; } = new();
    
    // Admin user for this tenant
    [Required]
    public required CreateTenantAdminDto AdminUser { get; set; }
}

// 🆕 NEW: Create Tenant Admin DTO
public class CreateTenantAdminDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string FirstName { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string LastName { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 8)]
    public required string Password { get; set; }
}

// 🆕 NEW: Update Tenant DTO
public class UpdateTenantDto
{
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
    
    [StringLength(255)]
    public string? Domain { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string SubscriptionPlan { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public Dictionary<string, object> Settings { get; set; } = new();
}

// 🆕 NEW: Tenant Initialization DTO
public class TenantInitializationDto
{
    public int TenantId { get; set; }
    public bool CreateDefaultRoles { get; set; } = true;
    public bool CreateDefaultPermissions { get; set; } = true;
    public List<string> RoleTemplates { get; set; } = new();
}

// 🆕 NEW: Role Template DTO
public class RoleTemplateDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public List<string> Permissions { get; set; } = new();
    public bool IsDefault { get; set; }
}

// 🆕 NEW: Tenant Statistics DTO
public class TenantStatisticsDto
{
    public int TenantId { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalRoles { get; set; }
    public int CustomRoles { get; set; }
    public DateTime LastActivity { get; set; }
    public Dictionary<string, int> SubscriptionUsage { get; set; } = new();
}

// 🆕 NEW: Tenant Settings DTO (for detailed settings management)
public class TenantSettingsDto
{
    public int TenantId { get; set; }
    public Dictionary<string, object> GeneralSettings { get; set; } = new();
    public Dictionary<string, bool> FeatureFlags { get; set; } = new();
    public Dictionary<string, string> Branding { get; set; } = new();
    public Dictionary<string, object> IntegrationSettings { get; set; } = new();
}
