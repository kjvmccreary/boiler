using Common.Data;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;

namespace UserService.IntegrationTests.TestUtilities;

public static class TestDataSeeder
{
    public static async Task SeedTestDataAsync(ApplicationDbContext dbContext)
    {
        // ✅ CHECK IF ALREADY SEEDED
        if (await dbContext.Tenants.AnyAsync())
        {
            return; // Already seeded
        }
        
        // Create all test data with PREDICTABLE IDs
        await CreateTenantsAsync(dbContext);
        await CreatePermissionsAsync(dbContext);
        await CreateRolesAsync(dbContext);
        await CreateRolePermissionsAsync(dbContext);
        await CreateUsersAsync(dbContext);
        await CreateUserRoleAssignmentsAsync(dbContext);
        await CreateLegacyTenantUsersAsync(dbContext);
    }

    private static async Task CreateTenantsAsync(ApplicationDbContext dbContext)
    {
        var tenants = new[]
        {
            new Tenant { Name = "Test Tenant 1", Domain = "tenant1.test.com", SubscriptionPlan = "Premium", Settings = "{\"theme\":\"blue\"}", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Tenant { Name = "Test Tenant 2", Domain = "tenant2.test.com", SubscriptionPlan = "Basic", Settings = "{\"theme\":\"green\"}", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        dbContext.Tenants.AddRange(tenants);
        await dbContext.SaveChangesAsync();
    }

    private static async Task CreatePermissionsAsync(ApplicationDbContext dbContext)
    {
        var permissions = new[]
        {
            // Users permissions
            new Permission { Name = "users.view", Category = "Users", Description = "View users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "users.edit", Category = "Users", Description = "Edit users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "users.create", Category = "Users", Description = "Create users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "users.delete", Category = "Users", Description = "Delete users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "users.manage", Category = "Users", Description = "Manage users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "users.manage_roles", Category = "Users", Description = "Manage user roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            // Roles permissions
            new Permission { Name = "roles.view", Category = "Roles", Description = "View roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "roles.create", Category = "Roles", Description = "Create roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "roles.edit", Category = "Roles", Description = "Edit roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "roles.delete", Category = "Roles", Description = "Delete roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "roles.manage_permissions", Category = "Roles", Description = "Manage role permissions", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            // Tenants permissions
            new Permission { Name = "tenants.view", Category = "Tenants", Description = "View tenants", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "tenants.edit", Category = "Tenants", Description = "Edit tenants", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
        };

        dbContext.Permissions.AddRange(permissions);
        await dbContext.SaveChangesAsync();
    }

    private static async Task CreateRolesAsync(ApplicationDbContext dbContext)
    {
        var tenant1 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 1");
        var tenant2 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 2");

        var roles = new[]
        {
            // System roles
            new Role { TenantId = null, Name = "SuperAdmin", Description = "System super admin", IsSystemRole = true, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            // Tenant 1 roles
            new Role { TenantId = tenant1.Id, Name = "Admin", Description = "Tenant admin", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Role { TenantId = tenant1.Id, Name = "User", Description = "Regular user", IsSystemRole = false, IsDefault = true, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Role { TenantId = tenant1.Id, Name = "Manager", Description = "Department manager", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            // Tenant 2 roles  
            new Role { TenantId = tenant2.Id, Name = "Admin", Description = "Tenant 2 admin", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Role { TenantId = tenant2.Id, Name = "User", Description = "Tenant 2 user", IsSystemRole = false, IsDefault = true, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
        };

        dbContext.Roles.AddRange(roles);
        await dbContext.SaveChangesAsync();
    }

    private static async Task CreateRolePermissionsAsync(ApplicationDbContext dbContext)
    {
        var tenant1 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 1");
        var permissions = await dbContext.Permissions.ToListAsync();
        var roles = await dbContext.Roles.ToListAsync();

        var rolePermissions = new List<RolePermission>();

        // SuperAdmin gets ALL permissions
        var superAdminRole = roles.First(r => r.Name == "SuperAdmin");
        foreach (var permission in permissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = superAdminRole.Id,
                PermissionId = permission.Id,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = "System"
            });
        }

        // Tenant 1 Admin gets most permissions (but NOT all - to test authorization)
        var tenant1AdminRole = roles.First(r => r.Name == "Admin" && r.TenantId == tenant1.Id);
        var adminPermissions = permissions.Where(p => 
            p.Category == "Users" || 
            p.Category == "Roles" || 
            p.Category == "Tenants").ToList();

        foreach (var permission in adminPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = tenant1AdminRole.Id,
                PermissionId = permission.Id,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = "System"
            });
        }

        // ✅ FIX: Tenant 1 User gets VERY LIMITED permissions (NO users.view to match test expectations)
        var tenant1UserRole = roles.First(r => r.Name == "User" && r.TenantId == tenant1.Id);
        var userPermissions = permissions.Where(p => 
            p.Name == "roles.view").ToList(); // Only roles.view, NOT users.view

        foreach (var permission in userPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = tenant1UserRole.Id,
                PermissionId = permission.Id,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = "System"
            });
        }

        // Tenant 1 Manager gets intermediate permissions
        var tenant1ManagerRole = roles.First(r => r.Name == "Manager" && r.TenantId == tenant1.Id);
        var managerPermissions = permissions.Where(p => 
            p.Name == "users.view" || 
            p.Name == "users.edit" ||
            p.Name == "roles.view").ToList();

        foreach (var permission in managerPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = tenant1ManagerRole.Id,
                PermissionId = permission.Id,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = "System"
            });
        }

        // Tenant 2 Admin gets admin permissions
        var tenant2AdminRole = roles.First(r => r.Name == "Admin" && r.TenantId != tenant1.Id);
        foreach (var permission in adminPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = tenant2AdminRole.Id,
                PermissionId = permission.Id,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = "System"
            });
        }

        dbContext.RolePermissions.AddRange(rolePermissions);
        await dbContext.SaveChangesAsync();
    }

    private static async Task CreateUsersAsync(ApplicationDbContext dbContext)
    {
        var tenant1 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 1");
        var tenant2 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 2");

        var users = new[]
        {
            // Tenant 1 users
            new User
            {
                TenantId = tenant1.Id,
                Email = "admin@tenant1.com",
                FirstName = "Admin",
                LastName = "User1",
                PasswordHash = "hashed_password",
                EmailConfirmed = true,
                IsActive = true,
                LastLoginAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                TenantId = tenant1.Id,
                Email = "user@tenant1.com",
                FirstName = "Regular",
                LastName = "User1",
                PasswordHash = "hashed_password",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                TenantId = tenant1.Id,
                Email = "manager@tenant1.com",
                FirstName = "Manager",
                LastName = "User1",
                PasswordHash = "hashed_password",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            // Tenant 2 users
            new User
            {
                TenantId = tenant2.Id,
                Email = "admin@tenant2.com",
                FirstName = "Admin",
                LastName = "User2",
                PasswordHash = "hashed_password",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                TenantId = tenant2.Id,
                Email = "user@tenant2.com",
                FirstName = "Regular",
                LastName = "User2",
                PasswordHash = "hashed_password",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        dbContext.Users.AddRange(users);
        await dbContext.SaveChangesAsync();
    }

    private static async Task CreateUserRoleAssignmentsAsync(ApplicationDbContext dbContext)
    {
        var tenant1 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 1");
        var tenant2 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 2");
        var users = await dbContext.Users.ToListAsync();
        var roles = await dbContext.Roles.ToListAsync();

        var adminUser1 = users.First(u => u.Email == "admin@tenant1.com");
        var regularUser1 = users.First(u => u.Email == "user@tenant1.com");
        var managerUser1 = users.First(u => u.Email == "manager@tenant1.com");
        var adminUser2 = users.First(u => u.Email == "admin@tenant2.com");
        var regularUser2 = users.First(u => u.Email == "user@tenant2.com");

        var tenant1AdminRole = roles.First(r => r.Name == "Admin" && r.TenantId == tenant1.Id);
        var tenant1UserRole = roles.First(r => r.Name == "User" && r.TenantId == tenant1.Id);
        var tenant1ManagerRole = roles.First(r => r.Name == "Manager" && r.TenantId == tenant1.Id);
        var tenant2AdminRole = roles.First(r => r.Name == "Admin" && r.TenantId == tenant2.Id);
        var tenant2UserRole = roles.First(r => r.Name == "User" && r.TenantId == tenant2.Id);

        var userRoles = new[]
        {
            // ✅ ENSURE: Only assign the intended roles, NO SuperAdmin to regular users
            new UserRole { UserId = adminUser1.Id, RoleId = tenant1AdminRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true },
            new UserRole { UserId = regularUser1.Id, RoleId = tenant1UserRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true },
            new UserRole { UserId = managerUser1.Id, RoleId = tenant1ManagerRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true },
            new UserRole { UserId = adminUser2.Id, RoleId = tenant2AdminRole.Id, TenantId = tenant2.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true },
            new UserRole { UserId = regularUser2.Id, RoleId = tenant2UserRole.Id, TenantId = tenant2.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true }
            // ✅ IMPORTANT: Do NOT assign SuperAdmin role to any test users
        };

        dbContext.UserRoles.AddRange(userRoles);
        await dbContext.SaveChangesAsync();
    }

    private static async Task CreateLegacyTenantUsersAsync(ApplicationDbContext dbContext)
    {
        var users = await dbContext.Users.ToListAsync();

        var legacyTenantUsers = users.Select(user => new TenantUser
        {
            UserId = user.Id,
            TenantId = user.TenantId!.Value,
            Role = user.Email.Contains("admin") ? "Admin" : 
                   user.Email.Contains("manager") ? "Manager" : "User",
            IsActive = true,
            JoinedAt = DateTime.UtcNow
        }).ToArray();

        dbContext.TenantUsers.AddRange(legacyTenantUsers);
        await dbContext.SaveChangesAsync();
    }
}
