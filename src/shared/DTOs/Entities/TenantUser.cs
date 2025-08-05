// FILE: src/shared/Common/Entities/TenantUser.cs
namespace DTOs.Entities;
public class TenantUser : BaseEntity
{
    public int TenantId { get; set; } // Changed from Guid to int
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public string? InvitedBy { get; set; }
    public string? Permissions { get; set; } // JSON string for additional permissions

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public User User { get; set; } = null!;
}
