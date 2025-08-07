using Common.Data;
using DTOs.Entities;
using BCrypt.Net; // Keep this using statement

namespace UserService.IntegrationTests.TestUtilities;

public static class TestDataSeeder
{
    public static async Task SeedTestDataAsync(ApplicationDbContext dbContext)
    {
        // Clear existing data first
        dbContext.RefreshTokens.RemoveRange(dbContext.RefreshTokens);
        dbContext.TenantUsers.RemoveRange(dbContext.TenantUsers);
        dbContext.Users.RemoveRange(dbContext.Users);
        dbContext.Tenants.RemoveRange(dbContext.Tenants);
        await dbContext.SaveChangesAsync();

        // Create Test Tenants
        var tenant1 = new Tenant
        {
            Id = 1,
            Name = "Test Tenant 1",
            Domain = "tenant1.test.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var tenant2 = new Tenant
        {
            Id = 2,
            Name = "Test Tenant 2", 
            Domain = "tenant2.test.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Tenants.AddRange(tenant1, tenant2);
        await dbContext.SaveChangesAsync();

        // Create Test Users for Tenant 1
        var adminUser1 = new User
        {
            Id = 1,
            Email = "admin@tenant1.com",
            FirstName = "Admin",
            LastName = "User1",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), // FIXED: Use fully qualified name
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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), // FIXED: Use fully qualified name
            TenantId = 1,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var anotherUser1 = new User
        {
            Id = 3,
            Email = "another@tenant1.com",
            FirstName = "Another",
            LastName = "User1", 
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), // FIXED: Use fully qualified name
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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), // FIXED: Use fully qualified name
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
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), // FIXED: Use fully qualified name
            TenantId = 2,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        dbContext.Users.AddRange(adminUser1, regularUser1, anotherUser1, adminUser2, regularUser2);
        await dbContext.SaveChangesAsync();

        // Create Tenant-User relationships
        var tenantUsers = new[]
        {
            new TenantUser { UserId = 1, TenantId = 1, Role = "SuperAdmin", IsActive = true },
            new TenantUser { UserId = 2, TenantId = 1, Role = "User", IsActive = true },
            new TenantUser { UserId = 3, TenantId = 1, Role = "User", IsActive = true },
            new TenantUser { UserId = 4, TenantId = 2, Role = "Admin", IsActive = true },
            new TenantUser { UserId = 5, TenantId = 2, Role = "User", IsActive = true }
        };

        dbContext.TenantUsers.AddRange(tenantUsers);
        await dbContext.SaveChangesAsync();
    }
}
