// FILE: src/shared/Common/Entities/TenantEntity.cs
namespace DTOs.Entities;

public abstract class TenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
}
