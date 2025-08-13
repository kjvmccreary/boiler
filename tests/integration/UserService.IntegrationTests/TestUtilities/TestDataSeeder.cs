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
            // Users permissions (IDs 1-6)
            new Permission { Name = "users.view", Category = "Users", Description = "View users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "users.edit", Category = "Users", Description = "Edit users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "users.create", Category = "Users", Description = "Create users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "users.delete", Category = "Users", Description = "Delete users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "users.manage", Category = "Users", Description = "Manage users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "users.manage_roles", Category = "Users", Description = "Manage user roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Roles permissions (IDs 7-11)
            new Permission { Name = "roles.view", Category = "Roles", Description = "View roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "roles.create", Category = "Roles", Description = "Create roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "roles.edit", Category = "Roles", Description = "Edit roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "roles.delete", Category = "Roles", Description = "Delete roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "roles.manage_permissions", Category = "Roles", Description = "Manage role permissions", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Tenants permissions (IDs 12-13)
            new Permission { Name = "tenants.view", Category = "Tenants", Description = "View tenants", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "tenants.edit", Category = "Tenants", Description = "Edit tenants", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // ✅ NEW: Additional permissions for testing user count and role users functionality
            new Permission { Name = "reports.view", Category = "Reports", Description = "View reports", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "reports.create", Category = "Reports", Description = "Create reports", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "reports.export", Category = "Reports", Description = "Export reports", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
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
            // System roles (ID 1)
            new Role { TenantId = null, Name = "SuperAdmin", Description = "System super admin", IsSystemRole = true, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Tenant 1 roles (IDs 2-4)
            new Role { TenantId = tenant1.Id, Name = "Admin", Description = "Tenant admin", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Role { TenantId = tenant1.Id, Name = "User", Description = "Regular user", IsSystemRole = false, IsDefault = true, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Role { TenantId = tenant1.Id, Name = "Manager", Description = "Department manager", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Tenant 2 roles (IDs 5-6)
            new Role { TenantId = tenant2.Id, Name = "Admin", Description = "Tenant 2 admin", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Role { TenantId = tenant2.Id, Name = "User", Description = "Tenant 2 user", IsSystemRole = false, IsDefault = true, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // ✅ NEW: Additional test roles for user count testing
            new Role { TenantId = tenant1.Id, Name = "Viewer", Description = "Read-only access", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Role { TenantId = tenant1.Id, Name = "Editor", Description = "Content editor", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
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
                GrantedBy = "System",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        // Tenant 1 Admin gets comprehensive permissions (for user count testing)
        var tenant1AdminRole = roles.First(r => r.Name == "Admin" && r.TenantId == tenant1.Id);
        var adminPermissions = permissions.Where(p => 
            p.Category == "Users" || 
            p.Category == "Roles" || 
            p.Category == "Tenants" ||
            p.Category == "Reports").ToList();

        foreach (var permission in adminPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = tenant1AdminRole.Id,
                PermissionId = permission.Id,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = "System",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        // ✅ CRITICAL FIX: Tenant 1 User gets ONLY roles.view permission (NOT users.view)
        // This matches the test expectation that User role cannot view users but can view roles
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
                GrantedBy = "System",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        // Tenant 1 Manager gets intermediate permissions (including users.view for testing)
        var tenant1ManagerRole = roles.First(r => r.Name == "Manager" && r.TenantId == tenant1.Id);
        var managerPermissions = permissions.Where(p => 
            p.Name == "users.view" || 
            p.Name == "users.edit" ||
            p.Name == "roles.view" ||
            p.Name == "reports.view").ToList();

        foreach (var permission in managerPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = tenant1ManagerRole.Id,
                PermissionId = permission.Id,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = "System",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        // ✅ NEW: Additional role permissions for user count testing
        var viewerRole = roles.First(r => r.Name == "Viewer" && r.TenantId == tenant1.Id);
        var viewerPermissions = permissions.Where(p => 
            p.Name == "roles.view" ||
            p.Name == "reports.view").ToList();

        foreach (var permission in viewerPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = viewerRole.Id,
                PermissionId = permission.Id,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = "System",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        var editorRole = roles.First(r => r.Name == "Editor" && r.TenantId == tenant1.Id);
        var editorPermissions = permissions.Where(p => 
            p.Name == "reports.view" ||
            p.Name == "reports.create" ||
            p.Name == "reports.export").ToList();

        foreach (var permission in editorPermissions)
        {
            rolePermissions.Add(new RolePermission
            {
                RoleId = editorRole.Id,
                PermissionId = permission.Id,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = "System",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
                GrantedBy = "System",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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
            // Tenant 1 users (IDs 1-5)
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
            // ✅ NEW: Additional users for user count testing
            new User
            {
                TenantId = tenant1.Id,
                Email = "viewer@tenant1.com",
                FirstName = "View",
                LastName = "Only",
                PasswordHash = "hashed_password",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                TenantId = tenant1.Id,
                Email = "editor@tenant1.com",
                FirstName = "Content",
                LastName = "Editor",
                PasswordHash = "hashed_password",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            
            // Tenant 2 users (IDs 6-7)
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
        var viewerUser1 = users.First(u => u.Email == "viewer@tenant1.com");
        var editorUser1 = users.First(u => u.Email == "editor@tenant1.com");
        var adminUser2 = users.First(u => u.Email == "admin@tenant2.com");
        var regularUser2 = users.First(u => u.Email == "user@tenant2.com");

        var tenant1AdminRole = roles.First(r => r.Name == "Admin" && r.TenantId == tenant1.Id);
        var tenant1UserRole = roles.First(r => r.Name == "User" && r.TenantId == tenant1.Id);
        var tenant1ManagerRole = roles.First(r => r.Name == "Manager" && r.TenantId == tenant1.Id);
        var tenant1ViewerRole = roles.First(r => r.Name == "Viewer" && r.TenantId == tenant1.Id);
        var tenant1EditorRole = roles.First(r => r.Name == "Editor" && r.TenantId == tenant1.Id);
        var tenant2AdminRole = roles.First(r => r.Name == "Admin" && r.TenantId == tenant2.Id);
        var tenant2UserRole = roles.First(r => r.Name == "User" && r.TenantId == tenant2.Id);

        var userRoles = new[]
        {
            // Primary role assignments for authentication testing
            new UserRole { UserId = adminUser1.Id, RoleId = tenant1AdminRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserRole { UserId = regularUser1.Id, RoleId = tenant1UserRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserRole { UserId = managerUser1.Id, RoleId = tenant1ManagerRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserRole { UserId = adminUser2.Id, RoleId = tenant2AdminRole.Id, TenantId = tenant2.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserRole { UserId = regularUser2.Id, RoleId = tenant2UserRole.Id, TenantId = tenant2.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // ✅ NEW: Additional role assignments for user count testing
            new UserRole { UserId = viewerUser1.Id, RoleId = tenant1ViewerRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserRole { UserId = editorUser1.Id, RoleId = tenant1EditorRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // ✅ NEW: Multi-role assignments for testing user count scenarios
            // Admin also has Manager role (for testing multiple role scenarios)
            new UserRole { UserId = adminUser1.Id, RoleId = tenant1ManagerRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            // Editor also has Viewer role
            new UserRole { UserId = editorUser1.Id, RoleId = tenant1ViewerRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
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
                   user.Email.Contains("manager") ? "Manager" : 
                   user.Email.Contains("viewer") ? "Viewer" :
                   user.Email.Contains("editor") ? "Editor" : "User",
            IsActive = true,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToArray();

        dbContext.TenantUsers.AddRange(legacyTenantUsers);
        await dbContext.SaveChangesAsync();
    }
}
