// FILE: src shared/Common/Repositories/TenantRepository.cs
using Common.Data;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Repositories;

public abstract class TenantRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly int _tenantId; // Changed from Guid to int
    protected readonly ILogger _logger;
    protected readonly ITenantProvider _tenantProvider; // CHANGED: Made protected instead of private

    protected TenantRepository(
        ApplicationDbContext context,
        ITenantProvider tenantProvider,
        ILogger logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    public virtual IQueryable<T> Query()
    {
        // Check if entity is tenant-aware (has TenantId property)
        if (typeof(TenantEntity).IsAssignableFrom(typeof(T)))
        {
            // For tenant entities, we need to get tenant ID synchronously for LINQ queries
            var tenantId = GetCurrentTenantIdSync();
            if (tenantId.HasValue)
            {
                return _dbSet.Where(e => EF.Property<int>(e, "TenantId") == tenantId.Value); // Changed from Guid to int
            }
        }

        // For non-tenant entities or when no tenant context, return all
        return _dbSet;
    }

    protected int? GetCurrentTenantIdSync() // CHANGED: Made protected so derived classes can use it
    {
        try
        {
            return _tenantProvider.GetCurrentTenantIdAsync().GetAwaiter().GetResult();
        }
        catch
        {
            return null;
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
        if (entity is TenantEntity tenantEntity)
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (tenantId.HasValue)
            {
                tenantEntity.TenantId = tenantId.Value; // Now int = int, no conversion needed
            }
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
            .Include(t => t.TenantUsers) // 🔧 FIX: Use TenantUsers instead of Users
            .ThenInclude(tu => tu.User) // 🔧 FIX: Include the actual User through TenantUsers
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

// UPDATED: This repository is for global tenant management operations (not tenant-scoped)
// Now implements both ITenantManagementRepository AND IRepository<Tenant>
public class TenantManagementRepository : ITenantManagementRepository, IRepository<Tenant>
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

    // IRepository<Tenant> methods
    public IQueryable<Tenant> Query()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<Tenant?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<Tenant> AddAsync(Tenant entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        _dbSet.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Tenant> UpdateAsync(Tenant entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tenant = await GetByIdAsync(id, cancellationToken);
        if (tenant != null)
        {
            _dbSet.Remove(tenant);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    // ITenantManagementRepository methods (some delegate to IRepository methods)
    public async Task<Tenant?> GetTenantByIdAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(tenantId, cancellationToken);
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
        return await GetAllAsync(cancellationToken);
    }

    public async Task<Tenant> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        return await AddAsync(tenant, cancellationToken);
    }

    public async Task<Tenant> UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        return await UpdateAsync(tenant, cancellationToken);
    }

    public async Task DeleteTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        await DeleteAsync(tenantId, cancellationToken);
    }
}
