using Common.Data;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Common.Constants;
using Common.Services;
using Microsoft.Extensions.Logging;

namespace Common.Services;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<RoleService> _logger;
    private readonly IAuditService _auditService;

    public RoleService(
        ApplicationDbContext context,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUserRoleRepository userRoleRepository,
        ITenantProvider tenantProvider,
        IAuditService auditService,
        ILogger<RoleService> logger)
    {
        _context = context;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _userRoleRepository = userRoleRepository;
        _tenantProvider = tenantProvider;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<RoleInfo> CreateRoleAsync(string name, string description, List<string> permissions, CancellationToken cancellationToken = default)
    {
        try
        {
            // 🔧 FIX: Add input validation
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Role name is required", nameof(name));
            }

            if (name.Length > 100)
            {
                throw new ArgumentException("Role name cannot exceed 100 characters", nameof(name));
            }

            if (!string.IsNullOrEmpty(description) && description.Length > 500)
            {
                throw new ArgumentException("Role description cannot exceed 500 characters", nameof(description));
            }

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

            await _auditService.LogAsync(AuditAction.RoleCreated, $"role:{role.Id}", 
                new { RoleName = name, Description = description, Permissions = permissions }, true);

            _logger.LogInformation("Created role {RoleName} for tenant {TenantId}", name, tenantId);
            return await MapToRoleInfoAsync(role, cancellationToken);
        }
        catch (Exception ex)
        {
            await _auditService.LogAsync(AuditAction.RoleCreated, $"role:{name}", 
                new { RoleName = name, Error = ex.Message }, false, ex.Message);
            throw;
        }
    }

    public async Task<bool> UpdateRoleAsync(int roleId, string name, string description, List<string> permissions, CancellationToken cancellationToken = default)
    {
        try
        {
            // 🔧 FIX: Add input validation
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Role name is required", nameof(name));
            }

            if (name.Length > 100)
            {
                throw new ArgumentException("Role name cannot exceed 100 characters", nameof(name));
            }

            if (!string.IsNullOrEmpty(description) && description.Length > 500)
            {
                throw new ArgumentException("Role description cannot exceed 500 characters", nameof(description));
            }

            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                throw new InvalidOperationException("Tenant context not found");
            }

            // First check if role exists globally
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId, cancellationToken);
            if (!roleExists)
            {
                return false; // Role doesn't exist at all
            }

            // Then check tenant ownership and get the role
            var role = await _roleRepository.Query()
                .FirstOrDefaultAsync(r => r.Id == roleId && r.TenantId == tenantId.Value, cancellationToken);

            if (role == null)
            {
                // 🔧 FIX: Role exists but doesn't belong to this tenant - this is a cross-tenant attempt
                await _auditService.LogAsync(AuditAction.UnauthorizedAccess, $"role:{roleId}", 
                    new { RoleId = roleId, AttemptedAction = "Update", Reason = "CrossTenantAccess" }, false);
                    
                throw new UnauthorizedAccessException($"Access denied to role {roleId}");
            }

            // 🔧 FIX: Check if it's a system role
            if (role.IsSystemRole)
            {
                await _auditService.LogAsync(AuditAction.UnauthorizedAccess, $"role:{roleId}", 
                    new { RoleId = roleId, AttemptedAction = "Update", Reason = "SystemRoleProtection" }, false);
                    
                throw new InvalidOperationException("System roles cannot be modified");
            }

            // Check if name is available (excluding current role)
            if (!await IsRoleNameAvailableAsync(name, roleId, cancellationToken))
            {
                throw new InvalidOperationException($"Role name '{name}' already exists in this tenant");
            }

            // Get old permissions for audit
            var oldPermissions = await GetRolePermissionsAsync(roleId, cancellationToken);

            // Update role properties
            var oldName = role.Name;
            var oldDescription = role.Description;
            role.Name = name;
            role.Description = description;
            role.UpdatedAt = DateTime.UtcNow;

            await _roleRepository.UpdateAsync(role, cancellationToken);

            // Update permissions
            await UpdateRolePermissionsAsync(roleId, permissions, cancellationToken);

            await _auditService.LogAsync(AuditAction.RoleUpdated, $"role:{roleId}", 
                new { 
                    RoleId = roleId,
                    OldName = oldName, 
                    NewName = name,
                    OldDescription = oldDescription,
                    NewDescription = description,
                    OldPermissions = oldPermissions,
                    NewPermissions = permissions
                }, true);

            _logger.LogInformation("Updated role {RoleId} for tenant {TenantId}", roleId, tenantId);
            return true;
        }
        catch (Exception ex)
        {
            await _auditService.LogAsync(AuditAction.RoleUpdated, $"role:{roleId}", 
                new { RoleId = roleId, Error = ex.Message }, false, ex.Message);
            
            _logger.LogError(ex, "Error updating role {RoleId}", roleId);
            throw;
        }
    }

    public async Task<RoleInfo?> GetRoleByIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (!tenantId.HasValue)
        {
            return null;
        }

        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            return null;
        }

        // 🔒 SECURITY FIX: Check tenant boundaries - throw exception for cross-tenant access
        if (role.TenantId.HasValue && role.TenantId.Value != tenantId.Value)
        {
            // Log security violation
            await _auditService.LogAsync(AuditAction.UnauthorizedAccess, $"role:{roleId}", 
                new { RoleId = roleId, AttemptedAction = "View", Reason = "CrossTenantAccess" }, false);
            
            throw new UnauthorizedAccessException($"Access denied to role {roleId}");
        }

        return await MapToRoleInfoAsync(role, cancellationToken);
    }

    public async Task<RoleInfo?> GetRoleWithPermissionsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (!tenantId.HasValue)
        {
            return null;
        }

        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (role == null)
        {
            return null;
        }

        // 🔒 SECURITY FIX: Check tenant boundaries 
        if (role.TenantId.HasValue && role.TenantId.Value != tenantId.Value)
        {
            await _auditService.LogAsync(AuditAction.UnauthorizedAccess, $"role:{roleId}", 
                new { RoleId = roleId, AttemptedAction = "ViewPermissions", Reason = "CrossTenantAccess" }, false);
            
            throw new UnauthorizedAccessException($"Access denied to role {roleId}");
        }

        return await MapToRoleInfoAsync(role, cancellationToken);
    }

    public async Task<bool> DeleteRoleAsync(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return false;
            }

            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
            {
                return false;
            }

            // 🔒 SECURITY FIX: Check tenant boundaries BEFORE other checks
            if (role.TenantId.HasValue && role.TenantId.Value != tenantId.Value)
            {
                await _auditService.LogAsync(AuditAction.UnauthorizedAccess, $"role:{roleId}", 
                    new { RoleId = roleId, AttemptedAction = "Delete", Reason = "CrossTenantAccess" }, false);
                
                throw new UnauthorizedAccessException($"Access denied to role {roleId}");
            }

            // Continue with existing logic...
            if (role.IsSystemRole)
            {
                // 🔧 FIX: Add expected error logging for system role protection
                _logger.LogError("Cannot delete system role {RoleId} - {RoleName}", roleId, role.Name);
                throw new InvalidOperationException("System roles cannot be deleted");
            }

            // Check if role has users assigned
            var hasUsers = await _context.UserRoles
                .AnyAsync(ur => ur.RoleId == roleId && ur.IsActive, cancellationToken);

            if (hasUsers)
            {
                // 🔧 FIX: Add expected error logging for roles with users
                _logger.LogError("Cannot delete role {RoleId} - {RoleName} because it has users assigned", roleId, role.Name);
                throw new InvalidOperationException("Cannot delete role that has users assigned to it");
            }

            try
            {
                await _auditService.LogAsync(AuditAction.RoleDeleted, $"role:{roleId}", 
                    new { RoleId = roleId, RoleName = role.Name }, true);

                await _roleRepository.DeleteAsync(roleId, cancellationToken);

                // 🔧 FIX: Add expected information logging for successful deletion
                _logger.LogInformation("Deleted role {RoleId} - {RoleName} for tenant {TenantId}", roleId, role.Name, tenantId);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {RoleId}", roleId);
                await _auditService.LogAsync(AuditAction.RoleDeleted, $"role:{roleId}", 
                    new { RoleId = roleId, Error = ex.Message }, false);
                return false;
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw security exceptions
        }
        catch (InvalidOperationException)
        {
            throw; // Re-throw business rule exceptions 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting role {RoleId}", roleId);
            throw;
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

    public async Task AssignRoleToUserAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                throw new InvalidOperationException("Tenant context not found");
            }

            // 🔧 FIX: Check if user exists and belongs to the current tenant
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == userId && u.TenantId == tenantId.Value, cancellationToken);
            
            if (!userExists)
            {
                await _auditService.LogAsync(AuditAction.UnauthorizedAccess, $"user:{userId}:role:{roleId}", 
                    new { UserId = userId, RoleId = roleId, AttemptedAction = "AssignRole", Reason = "UserNotFoundOrCrossTenant" }, false);
                    
                throw new InvalidOperationException("User not found or access denied");
            }

            // Check if role exists and belongs to tenant
            var role = await _roleRepository.Query()
                .FirstOrDefaultAsync(r => r.Id == roleId && 
                    (r.TenantId == tenantId.Value || r.IsSystemRole), cancellationToken);

            if (role == null)
            {
                await _auditService.LogAsync(AuditAction.UnauthorizedAccess, $"user:{userId}:role:{roleId}", 
                    new { UserId = userId, RoleId = roleId, AttemptedAction = "AssignRole", Reason = "RoleNotFoundOrCrossTenant" }, false);
                    
                throw new InvalidOperationException("Role not found or access denied");
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
                    
                    await _auditService.LogAsync(AuditAction.RoleAssigned, $"user:{userId}:role:{roleId}", 
                        new { UserId = userId, RoleId = roleId, RoleName = role.Name, Action = "Reactivated" }, true);
                }
                return; // 🔧 FIX: Return void instead of bool
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

            await _auditService.LogAsync(AuditAction.RoleAssigned, $"user:{userId}:role:{roleId}", 
                new { UserId = userId, RoleId = roleId, RoleName = role.Name, TenantId = tenantId }, true);

            _logger.LogInformation("Assigned role {RoleId} to user {UserId} in tenant {TenantId}", 
                roleId, userId, tenantId);
        }
        catch (Exception ex)
        {
            await _auditService.LogAsync(AuditAction.RoleAssigned, $"user:{userId}:role:{roleId}", 
                new { UserId = userId, RoleId = roleId, Error = ex.Message }, false, ex.Message);
                
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
                // Get role name for audit
                var role = await _roleRepository.Query()
                    .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

                userRole.IsActive = false;
                userRole.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);

                await _auditService.LogAsync(AuditAction.RoleRemoved, $"user:{userId}:role:{roleId}", 
                    new { UserId = userId, RoleId = roleId, RoleName = role?.Name, TenantId = tenantId }, true);

                _logger.LogInformation("Removed role {RoleId} from user {UserId} in tenant {TenantId}", 
                    roleId, userId, tenantId);
            }
            // 🔧 FIX: No return value needed, just return void instead of bool
        }
        catch (Exception ex)
        {
            await _auditService.LogAsync(AuditAction.RoleRemoved, $"user:{userId}:role:{roleId}", 
                new { UserId = userId, RoleId = roleId, Error = ex.Message }, false, ex.Message);
                
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            throw;
        }
    }

    public async Task<IEnumerable<UserInfo>> GetUsersInRoleAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (!tenantId.HasValue)
        {
            throw new InvalidOperationException("No tenant context available");
        }

        // 🔒 SECURITY FIX: First check if role exists and belongs to current tenant
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            throw new ArgumentException($"Role {roleId} not found");
        }

        if (role.TenantId.HasValue && role.TenantId.Value != tenantId.Value)
        {
            await _auditService.LogAsync(AuditAction.UnauthorizedAccess, $"role:{roleId}", 
                new { RoleId = roleId, AttemptedAction = "ViewUsers", Reason = "CrossTenantAccess" }, false);
            
            throw new UnauthorizedAccessException($"Access denied to role {roleId}");
        }

        // Now get the users for this role (only within current tenant)
        var users = await _context.UserRoles
            .Include(ur => ur.User)
            .Where(ur => ur.RoleId == roleId && ur.TenantId == tenantId.Value && ur.IsActive)
            .Select(ur => ur.User)
            .ToListAsync(cancellationToken);

        return users.Select(u => new UserInfo
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            IsActive = u.IsActive,
            TenantId = tenantId.Value  // 🔧 FIX: Use current tenant context
        });
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

            // 🔧 FIX: Check if the user exists and belongs to the current tenant
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == userId && u.TenantId == tenantId.Value, cancellationToken);
            
            if (!userExists)
            {
                await _auditService.LogAsync(AuditAction.UnauthorizedAccess, $"user:{userId}", 
                    new { UserId = userId, AttemptedAction = "ViewRoles", Reason = "UserNotFoundOrCrossTenant" }, false);
                    
                throw new UnauthorizedAccessException($"Access denied to user {userId}");
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
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw authorization exceptions
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

    public async Task<bool> UpdateRolePermissionsAsync(int roleId, List<string> permissions, CancellationToken cancellationToken = default)
    {
        var currentTenantId = await _tenantProvider.GetCurrentTenantIdAsync();

        // 🔧 FIX: Add retry logic for concurrent updates with better exception handling
        const int maxRetries = 5;
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
                if (role == null || (role.TenantId != currentTenantId && !role.IsSystemRole))
                {
                    return false;
                }

                if (role.IsSystemRole)
                {
                    throw new InvalidOperationException("System role permissions cannot be modified");
                }

                await _auditService.LogAsync(AuditAction.PermissionGranted, $"role:{roleId}", 
                    new { RoleId = roleId, Permissions = permissions }, true);

                await AssignPermissionsToRoleAsync(roleId, permissions, cancellationToken);
                return true;
            }
            catch (DbUpdateConcurrencyException) when (attempt < maxRetries - 1)
            {
                // 🔧 FIX: Wait with exponential backoff before retrying
                await Task.Delay(TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)), cancellationToken);
                continue;
            }
            catch (ArgumentException ex) when (ex.Message.Contains("same key has already been added") && attempt < maxRetries - 1)
            {
                // 🔧 FIX: This is InMemory DB concurrency - wait and retry with exponential backoff
                _logger.LogWarning("InMemory DB concurrency conflict on attempt {Attempt}: {Message}", attempt + 1, ex.Message);
                await Task.Delay(TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)), cancellationToken);
                continue;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("An item with the same key has already been added") && attempt < maxRetries - 1)
            {
                // 🔧 FIX: Another variation of the same InMemory DB issue
                _logger.LogWarning("InMemory DB operation conflict on attempt {Attempt}: {Message}", attempt + 1, ex.Message);
                await Task.Delay(TimeSpan.FromMilliseconds(150 * Math.Pow(2, attempt)), cancellationToken);
                continue;
            }
        }
        
        _logger.LogError("Failed to update role permissions after {MaxRetries} attempts for role {RoleId}", maxRetries, roleId);
        return false;
    }

    private static readonly SemaphoreSlim _concurrencySemaphore = new(1, 1);
    
    private async Task AssignPermissionsToRoleAsync(int roleId, List<string> permissionNames, CancellationToken cancellationToken)
    {
        // 🔧 FIX: Better detection for InMemory database
        var isInMemory = _context.Database.ProviderName?.Contains("InMemory") == true;
        
        if (isInMemory)
        {
            // 🔧 FIX: For InMemory database, use semaphore to serialize access
            await _concurrencySemaphore.WaitAsync(cancellationToken);
            try
            {
                await AssignPermissionsInternalAsync(roleId, permissionNames, cancellationToken);
            }
            finally
            {
                _concurrencySemaphore.Release();
            }
        }
        else
        {
            // Real database: Use transaction for atomicity
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await AssignPermissionsInternalAsync(roleId, permissionNames, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }

    private async Task AssignPermissionsInternalAsync(int roleId, List<string> permissionNames, CancellationToken cancellationToken)
    {
        // 🔧 FIX: Add small delay for InMemory database to reduce race conditions
        var isInMemory = _context.Database.ProviderName?.Contains("InMemory") == true;
        if (isInMemory)
        {
            await Task.Delay(50, cancellationToken); // Small delay to reduce race conditions
        }

        // Remove existing permissions first to ensure clean state
        var existingRolePermissions = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);

        if (existingRolePermissions.Any())
        {
            _context.RolePermissions.RemoveRange(existingRolePermissions);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // Add new permissions only if provided
        if (permissionNames.Any())
        {
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
            TenantId = role.TenantId ?? 0,
            Permissions = permissions,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }

    #endregion
}
