using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using DTOs.Entities;

namespace UserService.PerformanceTests.Utilities;

public static class PerformanceDataSeeder
{
    public static async Task SeedPerformanceDataAsync(ApplicationDbContext context, ILogger? logger = null)
    {
        logger?.LogInformation("🚀 Starting minimal performance data seeding...");

        // Clear existing data
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        // Create minimal test data for performance tests
        var tenant = new Tenant
        {
            Id = 1,
            Name = "Performance Test Tenant",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Tenants.AddAsync(tenant);
        await context.SaveChangesAsync();

        // Create minimal permissions
        var permissions = new[]
        {
            new Permission { Name = "users.view", Description = "View users", Category = "Users", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "users.edit", Description = "Edit users", Category = "Users", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Permission { Name = "roles.view", Description = "View roles", Category = "Roles", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        await context.Permissions.AddRangeAsync(permissions);
        await context.SaveChangesAsync();

        // Create minimal roles
        var adminRole = new Role
        {
            Name = "Admin",
            Description = "Administrator",
            IsSystemRole = false,
            TenantId = 1,
            IsDefault = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Roles.AddAsync(adminRole);
        await context.SaveChangesAsync();

        // Assign permissions to role
        var rolePermissions = permissions.Select(p => new RolePermission
        {
            RoleId = adminRole.Id,
            PermissionId = p.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToArray();
        await context.RolePermissions.AddRangeAsync(rolePermissions);
        await context.SaveChangesAsync();

        // Create minimal test user
        var user = new User
        {
            Id = 1,
            Email = "admin@tenant1.com",
            FirstName = "Admin",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Create TenantUser association
        var tenantUser = new TenantUser
        {
            UserId = user.Id,
            TenantId = tenant.Id,
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.TenantUsers.AddAsync(tenantUser);
        await context.SaveChangesAsync();

        // Assign role to user
        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = adminRole.Id,
            TenantId = tenant.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await context.UserRoles.AddAsync(userRole);
        await context.SaveChangesAsync();

        logger?.LogInformation("✅ Minimal performance data seeding completed");
    }
}
