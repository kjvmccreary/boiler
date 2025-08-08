using Common.Data;
using DTOs.Entities;
using BCrypt.Net; // Keep this using statement

namespace UserService.IntegrationTests.TestUtilities;

public static class TestDataSeeder
{
    public static async Task SeedTestDataAsync(ApplicationDbContext dbContext)
    {
        // Clear existing data first (ENHANCED with RBAC entities)
        dbContext.UserRoles.RemoveRange(dbContext.UserRoles);
        dbContext.RolePermissions.RemoveRange(dbContext.RolePermissions);
        dbContext.RefreshTokens.RemoveRange(dbContext.RefreshTokens);
        dbContext.TenantUsers.RemoveRange(dbContext.TenantUsers);
        dbContext.Users.RemoveRange(dbContext.Users);
        dbContext.Roles.RemoveRange(dbContext.Roles);
        dbContext.Permissions.RemoveRange(dbContext.Permissions);
        dbContext.Tenants.RemoveRange(dbContext.Tenants);
        await dbContext.SaveChangesAsync();

        // Create Test Tenants
        var tenant1 = new Tenant
        {
            Id = 1,
            Name = "Test Tenant 1",
            Domain = "tenant1.test.com",
            SubscriptionPlan = "Premium", // Add required property
            Settings = "{\"theme\":\"blue\"}", // Add required property
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var tenant2 = new Tenant
        {
            Id = 2,
            Name = "Test Tenant 2", 
            Domain = "tenant2.test.com",
            SubscriptionPlan = "Basic", // Add required property
            Settings = "{\"theme\":\"green\"}", // Add required property
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Tenants.AddRange(tenant1, tenant2);
        await dbContext.SaveChangesAsync();

        // ENHANCED: Create Test Permissions
        var permissions = new List<Permission>
        {
            // Users permissions
            new() { Id = 1, Name = "users.view", Category = "Users", Description = "View users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "users.edit", Category = "Users", Description = "Edit users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 3, Name = "users.create", Category = "Users", Description = "Create users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 4, Name = "users.delete", Category = "Users", Description = "Delete users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Roles permissions
            new() { Id = 5, Name = "roles.view", Category = "Roles", Description = "View roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 6, Name = "roles.create", Category = "Roles", Description = "Create roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 7, Name = "roles.edit", Category = "Roles", Description = "Edit roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 8, Name = "roles.delete", Category = "Roles", Description = "Delete roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            
            // Reports permissions
            new() { Id = 9, Name = "reports.view", Category = "Reports", Description = "View reports", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 10, Name = "reports.create", Category = "Reports", Description = "Create reports", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        dbContext.Permissions.AddRange(permissions);
        await dbContext.SaveChangesAsync();

        // ENHANCED: Create Test Roles
        var roles = new List<Role>
        {
            // System roles (no tenant)
            new() 
            { 
                Id = 1, 
                TenantId = null, 
                Name = "SuperAdmin", 
                Description = "System super administrator", 
                IsSystemRole = true, 
                IsDefault = false, 
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            
            // Tenant 1 roles
            new() 
            { 
                Id = 2, 
                TenantId = 1, 
                Name = "Admin", 
                Description = "Tenant administrator", 
                IsSystemRole = false, 
                IsDefault = false, 
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new() 
            { 
                Id = 3, 
                TenantId = 1, 
                Name = "User", 
                Description = "Standard user", 
                IsSystemRole = false, 
                IsDefault = true, 
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new() 
            { 
                Id = 4, 
                TenantId = 1, 
                Name = "Manager", 
                Description = "Manager role", 
                IsSystemRole = false, 
                IsDefault = false, 
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            
            // Tenant 2 roles
            new() 
            { 
                Id = 5, 
                TenantId = 2, 
                Name = "Admin", 
                Description = "Tenant 2 administrator", 
                IsSystemRole = false, 
                IsDefault = false, 
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new() 
            { 
                Id = 6, 
                TenantId = 2, 
                Name = "User", 
                Description = "Tenant 2 standard user", 
                IsSystemRole = false, 
                IsDefault = true, 
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        dbContext.Roles.AddRange(roles);
        await dbContext.SaveChangesAsync();

        // ENHANCED: Create Role-Permission assignments
        var rolePermissions = new List<RolePermission>
        {
            // Admin role (ID: 2) gets comprehensive permissions
            new() { RoleId = 2, PermissionId = 1, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // users.view
            new() { RoleId = 2, PermissionId = 2, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // users.edit
            new() { RoleId = 2, PermissionId = 3, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // users.create
            new() { RoleId = 2, PermissionId = 4, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // users.delete
            new() { RoleId = 2, PermissionId = 5, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // roles.view
            new() { RoleId = 2, PermissionId = 6, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // roles.create
            new() { RoleId = 2, PermissionId = 7, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // roles.edit
            new() { RoleId = 2, PermissionId = 8, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // roles.delete
            new() { RoleId = 2, PermissionId = 9, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // reports.view
            new() { RoleId = 2, PermissionId = 10, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // reports.create
            
            // User role (ID: 3) gets basic permissions
            new() { RoleId = 3, PermissionId = 1, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // users.view
            new() { RoleId = 3, PermissionId = 9, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // reports.view
            
            // Manager role (ID: 4) gets intermediate permissions
            new() { RoleId = 4, PermissionId = 1, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // users.view
            new() { RoleId = 4, PermissionId = 2, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // users.edit
            new() { RoleId = 4, PermissionId = 5, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // roles.view
            new() { RoleId = 4, PermissionId = 9, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // reports.view
            new() { RoleId = 4, PermissionId = 10, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // reports.create
            
            // Tenant 2 Admin role (ID: 5) gets comprehensive permissions
            new() { RoleId = 5, PermissionId = 1, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // users.view
            new() { RoleId = 5, PermissionId = 2, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // users.edit
            new() { RoleId = 5, PermissionId = 3, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // users.create
            new() { RoleId = 5, PermissionId = 5, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // roles.view
            new() { RoleId = 5, PermissionId = 6, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // roles.create
            
            // Tenant 2 User role (ID: 6) gets basic permissions
            new() { RoleId = 6, PermissionId = 1, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // users.view
            new() { RoleId = 6, PermissionId = 9, GrantedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }  // reports.view
        };

        dbContext.RolePermissions.AddRange(rolePermissions);
        await dbContext.SaveChangesAsync();

        // Create Test Users for Tenant 1
        var adminUser1 = new User
        {
            Id = 1,
            Email = "admin@tenant1.com",
            FirstName = "Admin",
            LastName = "User1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            TenantId = 1,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        var regularUser1 = new User
        {
            Id = 2,
            Email = "user@tenant1.com",
            FirstName = "Regular", 
            LastName = "User1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            TenantId = 1,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var managerUser1 = new User
        {
            Id = 3,
            Email = "manager@tenant1.com",
            FirstName = "Manager",
            LastName = "User1", 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            TenantId = 1,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Create Test Users for Tenant 2
        var adminUser2 = new User
        {
            Id = 4,
            Email = "admin@tenant2.com",
            FirstName = "Admin",
            LastName = "User2",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            TenantId = 2,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var regularUser2 = new User
        {
            Id = 5,
            Email = "user@tenant2.com",
            FirstName = "Regular",
            LastName = "User2",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            TenantId = 2,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Users.AddRange(adminUser1, regularUser1, managerUser1, adminUser2, regularUser2);
        await dbContext.SaveChangesAsync();

        // ENHANCED: Create User-Role assignments
        var userRoles = new List<UserRole>
        {
            // Tenant 1 assignments
            new() { UserId = 1, RoleId = 2, TenantId = 1, AssignedAt = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // Admin user gets Admin role
            new() { UserId = 2, RoleId = 3, TenantId = 1, AssignedAt = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // Regular user gets User role
            new() { UserId = 3, RoleId = 4, TenantId = 1, AssignedAt = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // Manager user gets Manager role
            
            // Tenant 2 assignments
            new() { UserId = 4, RoleId = 5, TenantId = 2, AssignedAt = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }, // Admin user gets Admin role
            new() { UserId = 5, RoleId = 6, TenantId = 2, AssignedAt = DateTime.UtcNow, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }  // Regular user gets User role
        };

        dbContext.UserRoles.AddRange(userRoles);
        await dbContext.SaveChangesAsync();

        // Create Tenant-User relationships (ENHANCED with new roles)
        var tenantUsers = new[]
        {
            new TenantUser { UserId = 1, TenantId = 1, Role = "Admin", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new TenantUser { UserId = 2, TenantId = 1, Role = "User", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new TenantUser { UserId = 3, TenantId = 1, Role = "Manager", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new TenantUser { UserId = 4, TenantId = 2, Role = "Admin", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new TenantUser { UserId = 5, TenantId = 2, Role = "User", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        dbContext.TenantUsers.AddRange(tenantUsers);
        await dbContext.SaveChangesAsync();
    }
}
