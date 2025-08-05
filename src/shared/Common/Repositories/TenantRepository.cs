// FILE: src/shared/Common/Repositories/TenantRepository.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Common.Data;
using DTOs.Entities;
using Contracts.Repositories;
using Contracts.Services;

namespace Common.Repositories;

public abstract class TenantRepository<T> : IRepository<T> where T : class, DTOs.Entities.BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly Guid _tenantId;
    protected readonly ILogger _logger;

    protected TenantRepository(
        ApplicationDbContext context,
        ITenantProvider tenantProvider,
        ILogger logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _tenantId = tenantProvider.GetCurrentTenantId();
        _logger = logger;
    }

    public virtual IQueryable<T> Query()
    {
        // Check if entity is tenant-aware (has TenantId property)
        if (typeof(DTOs.Entities.TenantEntity).IsAssignableFrom(typeof(T)))
        {
            // All queries automatically filtered by tenant for tenant entities
            return _dbSet.Where(e => EF.Property<Guid>(e, "TenantId") == _tenantId);
        }
        else
        {
            // For non-tenant entities (like base User), return all
            return _dbSet;
        }
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().ToListAsync(cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        // Set TenantId for tenant entities
        if (entity is DTOs.Entities.TenantEntity tenantEntity)
        {
            tenantEntity.TenantId = _tenantId;
        }

        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        _dbSet.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;

        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Query().AnyAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await Query().CountAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}

// This repository is for tenant-scoped operations (not typically used since tenants don't belong to other tenants)
public class TenantScopedRepository : TenantRepository<Tenant>, ITenantRepository
{
    public TenantScopedRepository(
        ApplicationDbContext context,
        ITenantProvider tenantProvider,
        ILogger<TenantScopedRepository> logger)
        : base(context, tenantProvider, logger)
    {
    }

    public async Task<Tenant?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        return await Query()
            .FirstOrDefaultAsync(t => t.Domain == domain, cancellationToken);
    }

    public async Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Query()
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<bool> DomainExistsAsync(string domain, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AnyAsync(t => t.Domain == domain, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AnyAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetActiveTenantsAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(t => t.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tenant?> GetWithUsersAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync(cancellationToken);

        var term = searchTerm.ToLower();
        return await Query()
            .Where(t => t.Name.ToLower().Contains(term) ||
                       (t.Domain != null && t.Domain.ToLower().Contains(term)))
            .ToListAsync(cancellationToken);
    }
}

// This repository is for global tenant management operations (not tenant-scoped)
public class TenantManagementRepository : ITenantManagementRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Tenant> _dbSet;
    private readonly ILogger<TenantManagementRepository> _logger;

    public TenantManagementRepository(
        ApplicationDbContext context,
        ILogger<TenantManagementRepository> logger)
    {
        _context = context;
        _dbSet = context.Set<Tenant>();
        _logger = logger;
    }

    public async Task<Tenant?> GetTenantByIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Id == (int)(object)tenantId, cancellationToken);
    }

    public async Task<Tenant?> GetTenantByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Domain == domain, cancellationToken);
    }

    public async Task<Tenant?> GetTenantByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetAllTenantsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Tenant> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        tenant.CreatedAt = DateTime.UtcNow;
        tenant.UpdatedAt = DateTime.UtcNow;

        _dbSet.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);
        return tenant;
    }

    public async Task<Tenant> UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        tenant.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(tenant);
        await _context.SaveChangesAsync(cancellationToken);
        return tenant;
    }

    public async Task DeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await GetTenantByIdAsync(tenantId, cancellationToken);
        if (tenant != null)
        {
            _dbSet.Remove(tenant);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
