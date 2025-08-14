using DTOs.Entities;
using Common.Data;
using Contracts.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Services
{
    public interface IRoleManagementService
    {
        Task<Role> CreateRoleAsync(int tenantId, string name, string description, List<int> permissionIds, string createdBy);
        Task<Role> UpdateRoleAsync(int roleId, string name, string description, List<int> permissionIds, string updatedBy);
        Task<bool> DeleteRoleAsync(int roleId, string deletedBy);
        Task<bool> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds, string assignedBy);
        Task<bool> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds, string removedBy);
        Task<List<Role>> GetTenantRolesAsync(int tenantId);
        Task<Role?> GetRoleByIdAsync(int roleId);
    }

    public class RoleManagementService : IRoleManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<RoleManagementService> _logger;

        public RoleManagementService(
            ApplicationDbContext context,
            ITenantProvider tenantProvider,
            ILogger<RoleManagementService> logger)
        {
            _context = context;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<Role> CreateRoleAsync(int tenantId, string name, string description, List<int> permissionIds, string createdBy)
        {
            try
            {
                // 🔧 FIX: Input validation and sanitization
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Role name cannot be empty");

                name = name.Trim();
                description = description?.Trim() ?? string.Empty;

                if (name.Length > 100)
                    throw new ArgumentException("Role name cannot exceed 100 characters");

                // 🔒 SECURITY: Check if role already exists in tenant
                var existingRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.Name == name);

                if (existingRole != null)
                    throw new InvalidOperationException($"Role '{name}' already exists in this tenant");

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Create role
                    var role = new Role
                    {
                        TenantId = tenantId,
                        Name = name,
                        Description = description,
                        IsSystemRole = false,
                        IsDefault = false,
                        IsActive = true
                    };

                    _context.Roles.Add(role);
                    await _context.SaveChangesAsync();

                    // 🔧 FIX: Assign permissions with proper validation
                    if (permissionIds?.Any() == true)
                    {
                        // Validate permissions exist and are active
                        var validPermissions = await _context.Permissions
                            .Where(p => permissionIds.Contains(p.Id) && p.IsActive)
                            .Select(p => p.Id)
                            .ToListAsync();

                        if (validPermissions.Count != permissionIds.Count)
                        {
                            var invalidIds = permissionIds.Except(validPermissions);
                            _logger.LogWarning("🔒 SECURITY: Invalid permission IDs attempted: {InvalidIds}", string.Join(",", invalidIds));
                        }

                        var rolePermissions = validPermissions.Select(permId => new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = permId,
                            GrantedAt = DateTime.UtcNow,
                            GrantedBy = createdBy
                        }).ToList();

                        _context.RolePermissions.AddRange(rolePermissions);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    
                    _logger.LogInformation("📝 AUDIT: Role '{RoleName}' created in tenant {TenantId} by {CreatedBy}", name, tenantId, createdBy);
                    return role;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔒 SECURITY: Error creating role '{RoleName}' in tenant {TenantId}", name, tenantId);
                throw;
            }
        }

        public async Task<bool> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds, string assignedBy)
        {
            try
            {
                // 🔒 SECURITY: Validate role exists and tenant access
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                if (role == null)
                {
                    _logger.LogWarning("🔒 SECURITY: Attempted to assign permissions to non-existent role {RoleId}", roleId);
                    return false;
                }

                var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
                if (role.TenantId != currentTenantId)
                {
                    _logger.LogWarning("🔒 SECURITY: Cross-tenant permission assignment attempt for role {RoleId}", roleId);
                    return false;
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 🔧 FIX: Remove existing permissions first to ensure clean state
                    var existingPermissions = await _context.RolePermissions
                        .Where(rp => rp.RoleId == roleId)
                        .ToListAsync();

                    _context.RolePermissions.RemoveRange(existingPermissions);

                    // Add new permissions
                    var validPermissions = await _context.Permissions
                        .Where(p => permissionIds.Contains(p.Id) && p.IsActive)
                        .Select(p => p.Id)
                        .ToListAsync();

                    var newRolePermissions = validPermissions.Select(permId => new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permId,
                        GrantedAt = DateTime.UtcNow,
                        GrantedBy = assignedBy
                    }).ToList();

                    _context.RolePermissions.AddRange(newRolePermissions);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("📝 AUDIT: Permissions updated for role {RoleId} by {AssignedBy}. Added: {PermissionCount}", 
                        roleId, assignedBy, validPermissions.Count);
                    
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔒 SECURITY: Error assigning permissions to role {RoleId}", roleId);
                return false;
            }
        }

        public async Task<Role> UpdateRoleAsync(int roleId, string name, string description, List<int> permissionIds, string updatedBy)
        {
            try
            {
                // 🔧 FIX: Input validation
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Role name cannot be empty");

                name = name.Trim();
                description = description?.Trim() ?? string.Empty;

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                if (role == null)
                    throw new InvalidOperationException($"Role with ID {roleId} not found");

                // 🔒 SECURITY: Prevent modification of system roles
                if (role.IsSystemRole)
                    throw new InvalidOperationException("Cannot modify system roles");

                var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
                if (role.TenantId != currentTenantId)
                    throw new InvalidOperationException("Cannot modify roles from other tenants");

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Update role properties
                    role.Name = name;
                    role.Description = description;
                    role.UpdatedAt = DateTime.UtcNow;

                    // Update permissions
                    await AssignPermissionsToRoleAsync(roleId, permissionIds, updatedBy);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("📝 AUDIT: Role {RoleId} updated by {UpdatedBy}", roleId, updatedBy);
                    return role;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔒 SECURITY: Error updating role {RoleId}", roleId);
                throw;
            }
        }

        public async Task<bool> DeleteRoleAsync(int roleId, string deletedBy)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
                if (role == null)
                    return false;

                // 🔒 SECURITY: Prevent deletion of system roles
                if (role.IsSystemRole)
                    throw new InvalidOperationException("Cannot delete system roles");

                var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();
                if (role.TenantId != currentTenantId)
                    throw new InvalidOperationException("Cannot delete roles from other tenants");

                // Check if role is assigned to users
                var userCount = await _context.UserRoles.CountAsync(ur => ur.RoleId == roleId && ur.IsActive);
                if (userCount > 0)
                    throw new InvalidOperationException($"Cannot delete role that is assigned to {userCount} users");

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Remove role permissions
                    var rolePermissions = await _context.RolePermissions
                        .Where(rp => rp.RoleId == roleId)
                        .ToListAsync();
                    
                    _context.RolePermissions.RemoveRange(rolePermissions);
                    
                    // Soft delete the role
                    role.IsActive = false;
                    role.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("📝 AUDIT: Role {RoleId} deleted by {DeletedBy}", roleId, deletedBy);
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔒 SECURITY: Error deleting role {RoleId}", roleId);
                return false;
            }
        }

        public async Task<bool> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds, string removedBy)
        {
            try
            {
                var rolePermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
                    .ToListAsync();

                _context.RolePermissions.RemoveRange(rolePermissions);
                await _context.SaveChangesAsync();

                _logger.LogInformation("📝 AUDIT: Permissions removed from role {RoleId} by {RemovedBy}", roleId, removedBy);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing permissions from role {RoleId}", roleId);
                return false;
            }
        }

        public async Task<List<Role>> GetTenantRolesAsync(int tenantId)
        {
            return await _context.Roles
                .Where(r => r.TenantId == tenantId && r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }
    }
}
