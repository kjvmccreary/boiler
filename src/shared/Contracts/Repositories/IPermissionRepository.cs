using DTOs.Entities;

namespace Contracts.Repositories;

public interface IPermissionRepository : IRepository<Permission>
{
    /// <summary>
    /// Get permission by name
    /// </summary>
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get permissions by category
    /// </summary>
    Task<IEnumerable<Permission>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all permission categories
    /// </summary>
    Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get permissions by names
    /// </summary>
    Task<IEnumerable<Permission>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if permission name exists
    /// </summary>
    Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
}
