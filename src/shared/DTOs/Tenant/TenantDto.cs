// FILE: src/shared/DTOs/Tenant/TenantDto.cs
namespace DTOs.Tenant;

public class TenantDto
{
    public int Id { get; set; } // Changed from Guid to int to match Tenant entity
    public string Name { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string SubscriptionPlan { get; set; } = "Basic";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Dictionary<string, object> Settings { get; set; } = new();
}
