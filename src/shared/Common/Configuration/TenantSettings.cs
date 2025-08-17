// FILE: src shared/Common/Configuration/TenantSettings.cs
namespace Common.Configuration;

public class TenantSettings
{
    public const string SectionName = "TenantSettings";

    public TenantResolutionStrategy ResolutionStrategy { get; set; } = TenantResolutionStrategy.Domain;
    public string DefaultTenantId { get; set; } = string.Empty;
    public bool EnableRowLevelSecurity { get; set; } = true;
    public bool AllowCrossTenantQueries { get; set; } = false;
    public string TenantHeaderName { get; set; } = "X-Tenant-ID";
    
    // 🆕 NEW: Control whether tenant context is required for protected endpoints
    public bool RequireTenantContext { get; set; } = true;
}

public enum TenantResolutionStrategy
{
    Domain,
    Path,
    Header,
    Subdomain
}
