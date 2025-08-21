using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Common.Data;
using DTOs.Entities;
using BCrypt.Net;

namespace UserService.PerformanceTests.Utilities;

public static class PerformanceDataSeeder
{
    public static async Task SeedPerformanceDataAsync(ApplicationDbContext context, ILogger? logger = null)
    {
        logger?.LogInformation("🚀 Starting performance data seeding...");

        // Clear existing data
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var tenants = new List<Tenant>();
        var users = new List<User>();
        var roles = new List<Role>();
        var permissions = new List<Permission>();

        // Create multiple tenants for performance testing
        for (int tenantIndex = 1; tenantIndex <= 5; tenantIndex++)
        {
            var tenant = new Tenant
            {
                Id = tenantIndex,
                Name = $"Performance Tenant {tenantIndex}",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            tenants.Add(tenant);
        }

        await context.Tenants.AddRangeAsync(tenants);
        await context.SaveChangesAsync();
        logger?.LogInformation("✅ Created {Count} tenants", tenants.Count);

        // Create system permissions (shared across all tenants)
        var systemPermissions = new[]
        {
            "users.view", "users.create", "users.edit", "users.delete",
            "roles.view", "roles.create", "roles.edit", "roles.delete", "roles.assign",
            "permissions.view", "settings.view", "settings.edit",
            "reports.view", "reports.export", "audit.view"
        };

        foreach (var permissionName in systemPermissions)
        {
            permissions.Add(new Permission
            {
                Name = permissionName,
                Description = $"Permission to {permissionName.Replace('.', ' ')}",
                Category = permissionName.Split('.')[0],
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();
        logger?.LogInformation("✅ Created {Count} permissions", permissions.Count);

        // Create system roles (shared across tenants)
        var systemRoles = new[]
        {
            new Role
            {
                Name = "SuperAdmin",
                Description = "System super administrator",
                IsSystemRole = true,
                TenantId = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Roles.AddRangeAsync(systemRoles);
        await context.SaveChangesAsync();

        var superAdminRole = systemRoles[0];

        // Assign all permissions to SuperAdmin
        var rolePermissions = new List<RolePermission>();
        foreach (var permission in permissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = superAdminRole.Id,
                PermissionId = permission.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await context.RolePermissions.AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();

        // Create tenant-specific roles and users for each tenant
        var userIdCounter = 1;
        foreach (var tenant in tenants)
        {
            // Create tenant-specific roles
            var tenantRoles = new[]
            {
                new Role
                {
                    Name = "Admin",
                    Description = $"Administrator for {tenant.Name}",
                    IsSystemRole = false,
                    TenantId = tenant.Id,
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Name = "Manager", 
                    Description = $"Manager for {tenant.Name}",
                    IsSystemRole = false,
                    TenantId = tenant.Id,
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Name = "User",
                    Description = $"Regular user for {tenant.Name}",
                    IsSystemRole = false,
                    TenantId = tenant.Id,
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            roles.AddRange(tenantRoles);
            await context.Roles.AddRangeAsync(tenantRoles);
            await context.SaveChangesAsync();

            // Assign permissions to tenant roles
            var tenantRolePermissions = new List<RolePermission>();
            
            // Admin gets all permissions
            foreach (var permission in permissions)
            {
                tenantRolePermissions.Add(new RolePermission
                {
                    RoleId = tenantRoles[0].Id, // Admin role
                    PermissionId = permission.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Manager gets limited permissions
            var managerPermissions = permissions.Where(p => 
                p.Name.Contains("view") || (p.Name.Contains("edit") && !p.Name.Contains("delete"))).ToList();
            foreach (var permission in managerPermissions)
            {
                tenantRolePermissions.Add(new RolePermission
                {
                    RoleId = tenantRoles[1].Id, // Manager role
                    PermissionId = permission.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // User gets minimal permissions
            var userPermissions = permissions.Where(p => p.Name.Contains("view") && !p.Name.Contains("settings")).ToList();
            foreach (var permission in userPermissions)
            {
                tenantRolePermissions.Add(new RolePermission
                {
                    RoleId = tenantRoles[2].Id, // User role
                    PermissionId = permission.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await context.RolePermissions.AddRangeAsync(tenantRolePermissions);
            await context.SaveChangesAsync();

            // Create multiple users per tenant for load testing
            var tenantUsers = new List<User>();
            for (int userIndex = 1; userIndex <= 20; userIndex++) // 20 users per tenant
            {
                var user = new User
                {
                    Id = userIdCounter++,
                    Email = $"user{userIndex}@tenant{tenant.Id}.com",
                    FirstName = $"User{userIndex}",
                    LastName = $"Tenant{tenant.Id}",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                tenantUsers.Add(user);
            }

            users.AddRange(tenantUsers);
            await context.Users.AddRangeAsync(tenantUsers);
            await context.SaveChangesAsync();

            // Create TenantUser associations since User doesn't have TenantId
            var tenantUserAssociations = new List<TenantUser>();
            foreach (var user in tenantUsers)
            {
                tenantUserAssociations.Add(new TenantUser
                {
                    UserId = user.Id,
                    TenantId = tenant.Id,
                    Role = "Member", // Default role in tenant
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            await context.TenantUsers.AddRangeAsync(tenantUserAssociations);
            await context.SaveChangesAsync();

            // Assign roles to users
            var userRoleAssignments = new List<UserRole>();
            for (int i = 0; i < tenantUsers.Count; i++)
            {
                var user = tenantUsers[i];
                Role roleToAssign;
                
                // First user is admin, next 3 are managers, rest are regular users
                if (i == 0)
                    roleToAssign = tenantRoles[0]; // Admin
                else if (i <= 3)
                    roleToAssign = tenantRoles[1]; // Manager
                else
                    roleToAssign = tenantRoles[2]; // User

                userRoleAssignments.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleToAssign.Id,
                    TenantId = tenant.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await context.UserRoles.AddRangeAsync(userRoleAssignments);
            await context.SaveChangesAsync();

            logger?.LogInformation("✅ Created {UserCount} users with role assignments for {TenantName}", 
                tenantUsers.Count, tenant.Name);
        }

        // Create SuperAdmin user (cross-tenant)
        var superAdminUser = new User
        {
            Id = userIdCounter++,
            Email = "superadmin@system.com",
            FirstName = "Super",
            LastName = "Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin123!"),
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(superAdminUser);
        await context.SaveChangesAsync();

        // Create TenantUser association for SuperAdmin
        var superAdminTenantUser = new TenantUser
        {
            UserId = superAdminUser.Id,
            TenantId = 1, // Associate with tenant 1
            Role = "SuperAdmin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.TenantUsers.AddAsync(superAdminTenantUser);
        await context.SaveChangesAsync();

        // Assign SuperAdmin role
        var superAdminUserRole = new UserRole
        {
            UserId = superAdminUser.Id,
            RoleId = superAdminRole.Id,
            TenantId = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.UserRoles.AddAsync(superAdminUserRole);
        await context.SaveChangesAsync();

        // Log final statistics
        var finalStats = new
        {
            Tenants = await context.Tenants.CountAsync(),
            Users = await context.Users.CountAsync(),
            Roles = await context.Roles.CountAsync(),
            Permissions = await context.Permissions.CountAsync(),
            UserRoles = await context.UserRoles.CountAsync(),
            RolePermissions = await context.RolePermissions.CountAsync(),
            TenantUsers = await context.TenantUsers.CountAsync()
        };

        logger?.LogInformation("🎯 Performance data seeding completed: {@Stats}", finalStats);
    }
}
