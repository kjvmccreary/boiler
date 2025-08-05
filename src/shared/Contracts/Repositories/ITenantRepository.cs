// FILE: src/shared/Contracts/Repositories/ITenantRepository.cs
using TenantEntity = DTOs.Entities.Tenant; // Type alias to avoid namespace conflict

namespace Contracts.Repositories;

public interface ITenantRepository : IRepository<TenantEntity>
{
    Task<TenantEntity?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default);
    Task<TenantEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> DomainExistsAsync(string domain, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantEntity>> GetActiveTenantsAsync(CancellationToken cancellationToken = default);
    Task<TenantEntity?> GetWithUsersAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantEntity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

// Note: This is a non-tenant-scoped repository since we need to access all tenants
public interface ITenantManagementRepository
{
    Task<TenantEntity?> GetTenantByIdAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<TenantEntity?> GetTenantByDomainAsync(string domain, CancellationToken cancellationToken = default);
    Task<TenantEntity?> GetTenantByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<TenantEntity>> GetAllTenantsAsync(CancellationToken cancellationToken = default);
    Task<TenantEntity> CreateTenantAsync(TenantEntity tenant, CancellationToken cancellationToken = default);
    Task<TenantEntity> UpdateTenantAsync(TenantEntity tenant, CancellationToken cancellationToken = default);
    Task DeleteTenantAsync(int tenantId, CancellationToken cancellationToken = default);
}
