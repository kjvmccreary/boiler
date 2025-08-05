// FILE: src/shared/Common/Entities/TenantEntity.cs
namespace Common.Entities;

public abstract class TenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
}
