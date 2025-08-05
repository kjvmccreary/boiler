using DTOs.Entities;

namespace Contracts.Repositories;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default);
    Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> DomainExistsAsync(string domain, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tenant>> GetActiveTenantsAsync(CancellationToken cancellationToken = default);
    Task<Tenant?> GetWithUsersAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tenant>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

// Note: This is a non-tenant-scoped repository since we need to access all tenants
public interface ITenantManagementRepository
{
    Task<Tenant?> GetTenantByIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Tenant?> GetTenantByDomainAsync(string domain, CancellationToken cancellationToken = default);
    Task<Tenant?> GetTenantByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tenant>> GetAllTenantsAsync(CancellationToken cancellationToken = default);
    Task<Tenant> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task<Tenant> UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task DeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
