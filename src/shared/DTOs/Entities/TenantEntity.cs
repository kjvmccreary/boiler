// FILE: src/shared/Common/Entities/TenantEntity.cs
namespace DTOs.Entities;

public abstract class TenantEntity : BaseEntity
{
    public int? TenantId { get; set; } // 🔧 FIXED: Made nullable to support tenant-less initial login

    // Navigation property
    public virtual Tenant? Tenant { get; set; }
}
