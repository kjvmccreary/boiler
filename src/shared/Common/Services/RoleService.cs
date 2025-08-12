using Common.Data;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Common.Constants;

namespace Common.Services;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        ApplicationDbContext context,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUserRoleRepository userRoleRepository,
        ITenantProvider tenantProvider,
        ILogger<RoleService> logger)
    {
        _context = context;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _userRoleRepository = userRoleRepository;
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    public async Task<RoleInfo> CreateRoleAsync(string name, string description, List<string> permissions, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                throw new InvalidOperationException("Tenant context not found");
            }

            // Check if role name already exists
            if (!await IsRoleNameAvailableAsync(name, null, cancellationToken))
            {
                throw new InvalidOperationException($"Role name '{name}' already exists in this tenant");
            }

            // Create the role
            var role = new Role
            {
                TenantId = tenantId.Value,
                Name = name,
                Description = description,
                IsSystemRole = false,
                IsDefault = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _roleRepository.AddAsync(role, cancellationToken);

            // Add permissions if provided
            if (permissions.Any())
            {
                await UpdateRolePermissionsAsync(role.Id, permissions, cancellationToken);
            }

            _logger.LogInformation("Created role {RoleName} for tenant {TenantId}", name, tenantId);
            return await MapToRoleInfoAsync(role, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role {RoleName}", name);
            throw;
        }
    }

    // 🔧 .NET 9 FIX: Change return type from Task<RoleInfo> to Task<bool>
    public async Task<bool> UpdateRoleAsync(int roleId, string name, string description, List<string> permissions, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                throw new InvalidOperationException("Tenant context not found");
            }

            var role = await _roleRepository.Query()
                .FirstOrDefaultAsync(r => r.Id == roleId && r.TenantId == tenantId.Value, cancellationToken);

            if (role == null)
            {
                return false; // 🔧 .NET 9 FIX: Return false instead of throwing exception
            }

            if (role.IsSystemRole)
            {
                throw new InvalidOperationException("System roles cannot be modified");
            }

            // Check if name is available (excluding current role)
            if (!await IsRoleNameAvailableAsync(name, roleId, cancellationToken))
            {
                throw new InvalidOperationException($"Role name '{name}' already exists in this tenant");
            }

            // Update role properties
            role.Name = name;
            role.Description = description;
            role.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.UpdateAsync(role, cancellationToken);

            // Update permissions
            await UpdateRolePermissionsAsync(roleId, permissions, cancellationToken);

            _logger.LogInformation("Updated role {RoleId} for tenant {TenantId}", roleId, tenantId);
            return true; // 🔧 .NET 9 FIX: Return true for success
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", roleId);
            throw;
        }
    }

    // 🔧 .NET 9 FIX: Change return type from Task to Task<bool>
    public async Task<bool> DeleteRoleAsync(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                throw new InvalidOperationException("Tenant context not found");
            }

            var role = await _roleRepository.Query()
                .FirstOrDefaultAsync(r => r.Id == roleId && r.TenantId == tenantId.Value, cancellationToken);

            if (role == null)
            {
                return false; // 🔧 .NET 9 FIX: Return false instead of throwing exception
            }

            if (role.IsSystemRole)
            {
                throw new InvalidOperationException("System roles cannot be deleted");
            }

            // Check if role has users assigned
            var hasUsers = await _context.UserRoles
                .AnyAsync(ur => ur.RoleId == roleId && ur.TenantId == tenantId.Value, cancellationToken);

            if (hasUsers)
            {
                throw new InvalidOperationException("Cannot delete role that has users assigned to it");
            }

            // Delete role permissions first
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync(cancellationToken);

            _context.RolePermissions.RemoveRange(rolePermissions);

            // Delete the role
            await _roleRepository.DeleteAsync(roleId, cancellationToken);

            _logger.LogInformation("Deleted role {RoleId} for tenant {TenantId}", roleId, tenantId);
            return true; // 🔧 .NET 9 FIX: Return true for success
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", roleId);
            throw;
        }
    }

    public async Task<RoleInfo?> GetRoleByIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return null;
            }

            var role = await _roleRepository.Query()
                .FirstOrDefaultAsync(r => r.Id == roleId && 
                    (r.TenantId == tenantId.Value || r.IsSystemRole), cancellationToken);

            if (role == null)
            {
                return null;
            }

            return await MapToRoleInfoAsync(role, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role {RoleId}", roleId);
            return null;
        }
    }

    public async Task<IEnumerable<RoleInfo>> GetTenantRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return Enumerable.Empty<RoleInfo>();
            }

            var roles = await _roleRepository.Query()
                .Where(r => r.TenantId == tenantId.Value || r.IsSystemRole)
                .Where(r => r.IsActive)
                .OrderBy(r => r.IsSystemRole ? 0 : 1)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);

            var roleInfos = new List<RoleInfo>();
            foreach (var role in roles)
            {
                roleInfos.Add(await MapToRoleInfoAsync(role, cancellationToken));
            }

            return roleInfos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tenant roles");
            return Enumerable.Empty<RoleInfo>();
        }
    }

    public async Task<RoleInfo?> GetRoleWithPermissionsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await GetRoleByIdAsync(roleId, cancellationToken);
    }

    public async Task AssignRoleToUserAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                throw new InvalidOperationException("Tenant context not found");
            }

            // Check if role exists and belongs to tenant
            var role = await _roleRepository.Query()
                .FirstOrDefaultAsync(r => r.Id == roleId && 
                    (r.TenantId == tenantId.Value || r.IsSystemRole), cancellationToken);

            if (role == null)
            {
                throw new InvalidOperationException("Role not found");
            }

            // Check if assignment already exists
            var existingAssignment = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && 
                    ur.TenantId == tenantId.Value, cancellationToken);

            if (existingAssignment != null)
            {
                // Reactivate if inactive
                if (!existingAssignment.IsActive)
                {
                    existingAssignment.IsActive = true;
                    existingAssignment.AssignedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                }
                return;
            }

            // Create new assignment
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                TenantId = tenantId.Value,
                AssignedAt = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRoleRepository.AddAsync(userRole, cancellationToken);

            _logger.LogInformation("Assigned role {RoleId} to user {UserId} in tenant {TenantId}", 
                roleId, userId, tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
            throw;
        }
    }

    public async Task RemoveRoleFromUserAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                throw new InvalidOperationException("Tenant context not found");
            }

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && 
                    ur.TenantId == tenantId.Value, cancellationToken);

            if (userRole != null)
            {
                userRole.IsActive = false;
                userRole.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Removed role {RoleId} from user {UserId} in tenant {TenantId}", 
                    roleId, userId, tenantId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            throw;
        }
    }

    public async Task<IEnumerable<UserInfo>> GetUsersInRoleAsync(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return Enumerable.Empty<UserInfo>();
            }

            var users = await _context.UserRoles
                .Where(ur => ur.RoleId == roleId && ur.TenantId == tenantId.Value && ur.IsActive)
                .Join(_context.Users,
                    ur => ur.UserId,
                    u => u.Id,
                    (ur, u) => new UserInfo
                    {
                        Id = u.Id,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        IsActive = u.IsActive
                    })
                .ToListAsync(cancellationToken);

            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users in role {RoleId}", roleId);
            return Enumerable.Empty<UserInfo>();
        }
    }

    public async Task<IEnumerable<RoleInfo>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return Enumerable.Empty<RoleInfo>();
            }

            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.TenantId == tenantId.Value && ur.IsActive)
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r)
                .ToListAsync(cancellationToken);

            var roleInfos = new List<RoleInfo>();
            foreach (var role in roles)
            {
                roleInfos.Add(await MapToRoleInfoAsync(role, cancellationToken));
            }

            return roleInfos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
            return Enumerable.Empty<RoleInfo>();
        }
    }

    public async Task<bool> IsRoleNameAvailableAsync(string name, int? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return false;
            }

            var query = _roleRepository.Query()
                .Where(r => r.Name.ToLower() == name.ToLower() && 
                    (r.TenantId == tenantId.Value || r.IsSystemRole));

            if (excludeRoleId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRoleId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role name availability for {RoleName}", name);
            return false;
        }
    }

    public async Task CreateDefaultRolesForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Create default roles for new tenant
            var defaultRoles = new[]
            {
                new Role
                {
                    TenantId = tenantId,
                    Name = "TenantAdmin",
                    Description = "Administrator for this tenant",
                    IsSystemRole = false,
                    IsDefault = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Role
                {
                    TenantId = tenantId,
                    Name = "User",
                    Description = "Standard user role",
                    IsSystemRole = false,
                    IsDefault = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            foreach (var role in defaultRoles)
            {
                await _roleRepository.AddAsync(role, cancellationToken);

                // Assign default permissions based on role
                var permissions = GetDefaultPermissionsForRole(role.Name);
                if (permissions.Any())
                {
                    await AssignPermissionsToRoleAsync(role.Id, permissions, cancellationToken);
                }
            }

            _logger.LogInformation("Created default roles for tenant {TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating default roles for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task<PagedResult<RoleInfo>> GetTenantRolesPagedAsync(int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return new PagedResult<RoleInfo>();
            }

            var query = _roleRepository.Query()
                .Where(r => (r.TenantId == tenantId.Value || r.IsSystemRole) && r.IsActive);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(r => r.Name.ToLower().Contains(searchLower) ||
                                        r.Description.ToLower().Contains(searchLower));
            }

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination and ordering
            var roles = await query
                .OrderBy(r => r.IsSystemRole ? 0 : 1)
                .ThenBy(r => r.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var roleInfos = new List<RoleInfo>();
            foreach (var role in roles)
            {
                roleInfos.Add(await MapToRoleInfoAsync(role, cancellationToken));
            }

            return new PagedResult<RoleInfo>
            {
                Items = roleInfos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged roles");
            return new PagedResult<RoleInfo>();
        }
    }

    public async Task<List<string>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var permissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Join(_context.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p.Name)
                .ToListAsync(cancellationToken);

            return permissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions for role {RoleId}", roleId);
            return new List<string>();
        }
    }

    // 🔧 .NET 9 FIX: Change return type from Task to Task<bool>
    public async Task<bool> UpdateRolePermissionsAsync(int roleId, List<string> permissions, CancellationToken cancellationToken = default)
    {
        try
        {
            // Remove existing permissions
            var existingPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync(cancellationToken);

            _context.RolePermissions.RemoveRange(existingPermissions);

            // Add new permissions
            await AssignPermissionsToRoleAsync(roleId, permissions, cancellationToken);

            _logger.LogInformation("Updated permissions for role {RoleId}", roleId);
            return true; // 🔧 .NET 9 FIX: Return true for success
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permissions for role {RoleId}", roleId);
            throw;
        }
    }

    #region Helper Methods

    private async Task<RoleInfo> MapToRoleInfoAsync(Role role, CancellationToken cancellationToken)
    {
        var permissions = await GetRolePermissionsAsync(role.Id, cancellationToken);

        return new RoleInfo
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            IsDefault = role.IsDefault,
            // 🔧 .NET 9 FIX: Map null TenantId to 0 for system roles to match test expectations
            TenantId = role.TenantId ?? 0,
            Permissions = permissions,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }

    private async Task AssignPermissionsToRoleAsync(int roleId, List<string> permissionNames, CancellationToken cancellationToken)
    {
        if (!permissionNames.Any()) return;

        var permissions = await _context.Permissions
            .Where(p => permissionNames.Contains(p.Name))
            .ToListAsync(cancellationToken);

        var rolePermissions = permissions.Select(p => new RolePermission
        {
            RoleId = roleId,
            PermissionId = p.Id,
            GrantedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        _context.RolePermissions.AddRange(rolePermissions);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static List<string> GetDefaultPermissionsForRole(string roleName)
    {
        return roleName switch
        {
            "TenantAdmin" => new List<string>
            {
                Permissions.Users.View, Permissions.Users.Edit, Permissions.Users.Create, Permissions.Users.Delete,
                Permissions.Roles.View, Permissions.Roles.Create, Permissions.Roles.Edit, Permissions.Roles.Delete,
                Permissions.Reports.View, Permissions.Reports.Create, Permissions.Reports.Export
            },
            "User" => new List<string>
            {
                Permissions.Users.View, Permissions.Users.Edit,
                Permissions.Reports.View
            },
            _ => new List<string>()
        };
    }

    #endregion
}
