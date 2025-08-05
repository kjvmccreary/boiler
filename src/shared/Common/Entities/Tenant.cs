// FILE: src/shared/Common/Entities/Tenant.cs
namespace Common.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string SubscriptionPlan { get; set; } = "Basic";
    public bool IsActive { get; set; } = true;
    public string Settings { get; set; } = "{}"; // JSON string for tenant-specific settings

    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
}
