using Common.Data;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Permission> _dbSet;
    private readonly ILogger<PermissionRepository> _logger;

    public PermissionRepository(
        ApplicationDbContext context,
        ILogger<PermissionRepository> logger)
    {
        _context = context;
        _dbSet = context.Set<Permission>();
        _logger = logger;
    }

    public IQueryable<Permission> Query()
    {
        return _dbSet.Where(p => p.IsActive);
    }

    public async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().OrderBy(p => p.Category).ThenBy(p => p.Name).ToListAsync(cancellationToken);
    }

    public async Task<Permission> AddAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        _dbSet.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Permission> UpdateAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Query().AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await Query().CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    // Permission-specific methods
    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(p => p.Category == category)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(p => names.Contains(p.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Query().AnyAsync(p => p.Name == name, cancellationToken);
    }
}
