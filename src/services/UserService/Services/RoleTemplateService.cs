using Common.Data;
using Common.Services;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using DTOs.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserService.Services;

public class RoleTemplateService : IRoleTemplateService
{
    private readonly ApplicationDbContext _context;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<RoleTemplateService> _logger;
    private readonly IAuditService _auditService;

    private static readonly Dictionary<string, RoleTemplateDto> RoleTemplates = new()
    {
        {
            "TenantAdmin",
            new RoleTemplateDto
            {
                Name = "Tenant Admin",
                Description = "Full administrative access to the tenant",
                Permissions = new List<string>
                {
                    "users.view", "users.edit", "users.create", "users.delete",
                    "roles.view", "roles.edit", "roles.create", "roles.delete",
                    "permissions.view", "permissions.assign",
                    "settings.view", "settings.edit",
                    "audit.view", "reports.view", "reports.export"
                },
                IsDefault = true
            }
        },
        {
            "Manager",
            new RoleTemplateDto
            {
                Name = "Manager",
                Description = "Management access with user and report permissions",
                Permissions = new List<string>
                {
                    "users.view", "users.edit", "users.create",
                    "roles.view",
                    "reports.view", "reports.create", "reports.export",
                    "dashboard.view"
                },
                IsDefault = true
            }
        },
        {
            "User",
            new RoleTemplateDto
            {
                Name = "User",
                Description = "Basic user access",
                Permissions = new List<string>
                {
                    "profile.view", "profile.edit",
                    "dashboard.view",
                    "reports.view"
                },
                IsDefault = true
            }
        },
        {
            "ReadOnly",
            new RoleTemplateDto
            {
                Name = "Read Only",
                Description = "View-only access to most resources",
                Permissions = new List<string>
                {
                    "users.view",
                    "roles.view",
                    "reports.view",
                    "dashboard.view",
                    "profile.view"
                },
                IsDefault = false
            }
        }
    };

    public RoleTemplateService(
        ApplicationDbContext context,
        IPermissionRepository permissionRepository,
        IRoleRepository roleRepository,
        ILogger<RoleTemplateService> logger,
        IAuditService auditService)
    {
        _context = context;
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
        _logger = logger;
        _auditService = auditService;
    }

    public async Task CreateDefaultRolesForTenantAsync(int tenantId)
    {
        try
        {
            _logger.LogInformation("Creating default roles for tenant {TenantId}", tenantId);

            var defaultTemplates = RoleTemplates.Where(t => t.Value.IsDefault);

            foreach (var template in defaultTemplates)
            {
                await CreateRoleFromTemplateAsync(tenantId, template.Key);
            }

            // ✅ FIXED: Pass details object instead of string
            await _auditService.LogAsync(
                AuditAction.RoleCreated, 
                $"tenants/{tenantId}/roles", 
                new { Message = "Default roles created for tenant", TenantId = tenantId }, 
                true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating default roles for tenant {TenantId}", tenantId);
            throw;
        }
    }

    public async Task CreateRoleFromTemplateAsync(int tenantId, string templateName)
    {
        try
        {
            if (!RoleTemplates.TryGetValue(templateName, out var template))
            {
                throw new ArgumentException($"Role template '{templateName}' not found");
            }

            // Check if role already exists for this tenant
            var existingRole = await _context.Roles
                .Where(r => r.TenantId == tenantId && r.Name == template.Name)
                .FirstOrDefaultAsync();

            if (existingRole != null)
            {
                _logger.LogInformation("Role '{RoleName}' already exists for tenant {TenantId}", 
                    template.Name, tenantId);
                return;
            }

            // Create role
            var role = new Role
            {
                Name = template.Name,
                Description = template.Description,
                TenantId = tenantId,
                IsSystemRole = false,
                IsDefault = template.IsDefault,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // ✅ FIXED: Handle case where permissions might not be found
            var permissions = await _permissionRepository.GetByNamesAsync(template.Permissions);
            var permissionsList = permissions.ToList();
            
            if (permissionsList.Count == 0)
            {
                _logger.LogWarning("No permissions found for role template '{TemplateName}' - tenant {TenantId}", 
                    templateName, tenantId);
            }
            else
            {
                foreach (var permission in permissionsList)
                {
                    var rolePermission = new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permission.Id,
                        GrantedAt = DateTime.UtcNow,
                        GrantedBy = "System",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.RolePermissions.Add(rolePermission);
                }

                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Created role '{RoleName}' for tenant {TenantId} with {PermissionCount} permissions", 
                role.Name, tenantId, permissionsList.Count);

            // ✅ FIXED: Pass details object instead of string
            await _auditService.LogAsync(
                AuditAction.RoleCreated, 
                $"tenants/{tenantId}/roles/{role.Id}", 
                new { 
                    RoleName = role.Name, 
                    TemplateName = templateName, 
                    PermissionCount = permissionsList.Count,
                    TenantId = tenantId 
                }, 
                true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role from template '{TemplateName}' for tenant {TenantId}", 
                templateName, tenantId);
            throw;
        }
    }

    // ✅ FIXED: Make method truly async to resolve warning
    public async Task<List<RoleTemplateDto>> GetAvailableTemplatesAsync()
    {
        // Simulate async operation to resolve the warning
        await Task.Yield();
        return RoleTemplates.Values.ToList();
    }
}
