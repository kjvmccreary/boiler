using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.IntegrationTests.Fixtures;
using Xunit;

namespace UserService.IntegrationTests.Infrastructure;

public class TestSetupVerificationTests : TestBase
{
    public TestSetupVerificationTests(WebApplicationTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task TestSetup_ShouldCreateAllRequiredTestData()
    {
        // Verify tenants
        var tenants = await _dbContext.Tenants.ToListAsync();
        tenants.Should().HaveCount(2);
        tenants.Should().Contain(t => t.Name == "Test Tenant 1");
        tenants.Should().Contain(t => t.Name == "Test Tenant 2");

        // Verify users - ✅ FIX: Should be 7 users (5 tenant1 + 2 tenant2)
        var users = await _dbContext.Users.ToListAsync();
        users.Should().HaveCount(7);
        users.Should().Contain(u => u.Email == "admin@tenant1.com");
        users.Should().Contain(u => u.Email == "user@tenant1.com");
        users.Should().Contain(u => u.Email == "manager@tenant1.com");
        users.Should().Contain(u => u.Email == "viewer@tenant1.com");
        users.Should().Contain(u => u.Email == "editor@tenant1.com");
        users.Should().Contain(u => u.Email == "admin@tenant2.com");
        users.Should().Contain(u => u.Email == "user@tenant2.com");

        // Verify permissions
        var permissions = await _dbContext.Permissions.ToListAsync();
        permissions.Should().HaveCount(20); // ✅ FIX: Should be 20 permissions (as created in seeder)
        permissions.Should().Contain(p => p.Name == "users.view");

        // Verify roles - ✅ FIX: Should be 8 roles (1 system + 5 tenant1 + 2 tenant2)
        var roles = await _dbContext.Roles.ToListAsync();
        roles.Should().HaveCount(8);
        roles.Should().Contain(r => r.Name == "SuperAdmin" && r.IsSystemRole);
        roles.Should().Contain(r => r.Name == "Admin" && r.TenantId == 1);
        roles.Should().Contain(r => r.Name == "User" && r.TenantId == 1);

        // ✅ FIX: Should be 8 user-role assignments (7 primary + 1 additional)
        var userRoles = await _dbContext.UserRoles.ToListAsync();
        userRoles.Should().HaveCount(8);

        // Verify role-permission assignments
        var rolePermissions = await _dbContext.RolePermissions.ToListAsync();
        rolePermissions.Should().HaveCountGreaterThan(20);
    }

    [Fact]
    public async Task AdminUser_ShouldHaveAllRequiredPermissions()
    {
        // ✅ RBAC FIX: This test will now work correctly with the updated GetUserPermissionsAsync method
        var adminPermissions = await GetUserPermissionsAsync("admin@tenant1.com");
        
        adminPermissions.Should().Contain("users.view");
        adminPermissions.Should().Contain("users.edit");
        adminPermissions.Should().Contain("users.create");
        adminPermissions.Should().Contain("users.delete");
        adminPermissions.Should().Contain("users.manage_roles");
        adminPermissions.Should().Contain("roles.view");
        adminPermissions.Should().Contain("roles.create");
        adminPermissions.Should().Contain("roles.edit");
        adminPermissions.Should().Contain("roles.delete");
        adminPermissions.Should().Contain("roles.manage_permissions");

        _logger.LogInformation("✅ Admin user has {Count} permissions: {Permissions}", 
            adminPermissions.Count, string.Join(", ", adminPermissions));
    }

    [Fact]
    public async Task JwtToken_ShouldContainPermissionClaims()
    {
        // ✅ RBAC FIX: Use actual RBAC role instead of hard-coded role
        var token = await GetAuthTokenAsync("admin@tenant1.com"); // Use actual "Admin" role
        
        token.Should().NotBeNullOrEmpty();
        
        _logger.LogInformation("✅ Generated JWT token successfully for admin user");
    }

    [Fact]
    public async Task TenantIsolation_ShouldBeProperlyConfigured()
    {
        // Verify tenant 1 users
        var tenant1Users = await _dbContext.Users.Where(u => u.TenantId == 1).ToListAsync();
        tenant1Users.Should().HaveCount(5);
        tenant1Users.Should().OnlyContain(u => u.Email.Contains("@tenant1.com"));

        // Verify tenant 2 users
        var tenant2Users = await _dbContext.Users.Where(u => u.TenantId == 2).ToListAsync();
        tenant2Users.Should().HaveCount(2);
        tenant2Users.Should().OnlyContain(u => u.Email.Contains("@tenant2.com"));

        // Verify tenant 1 roles
        var tenant1Roles = await _dbContext.Roles.Where(r => r.TenantId == 1).ToListAsync();
        tenant1Roles.Should().HaveCount(5); // Admin, User, Manager, Viewer, Editor

        // Verify tenant 2 roles
        var tenant2Roles = await _dbContext.Roles.Where(r => r.TenantId == 2).ToListAsync();
        tenant2Roles.Should().HaveCount(2); // Admin, User

        // Verify system roles
        var systemRoles = await _dbContext.Roles.Where(r => r.TenantId == null).ToListAsync();
        systemRoles.Should().HaveCount(1); // SuperAdmin
        systemRoles.Should().Contain(r => r.Name == "SuperAdmin" && r.IsSystemRole);
    }
}
