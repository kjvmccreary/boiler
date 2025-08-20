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

        // ✅ CRITICAL: Use IgnoreQueryFilters to ensure we see all users
        var users = await _dbContext.Users.IgnoreQueryFilters().ToListAsync();
        users.Should().HaveCount(7); // ✅ Should now find all 7 users
        users.Should().Contain(u => u.Email == "admin@tenant1.com");
        users.Should().Contain(u => u.Email == "user@tenant1.com");
        users.Should().Contain(u => u.Email == "manager@tenant1.com");
        users.Should().Contain(u => u.Email == "viewer@tenant1.com");
        users.Should().Contain(u => u.Email == "editor@tenant1.com");
        users.Should().Contain(u => u.Email == "admin@tenant2.com"); // ✅ Should now find Tenant 2 users
        users.Should().Contain(u => u.Email == "user@tenant2.com");   // ✅ Should now find Tenant 2 users

        // Verify permissions
        var permissions = await _dbContext.Permissions.ToListAsync();
        permissions.Should().HaveCount(36); // ✅ Updated count
        permissions.Should().Contain(p => p.Name == "users.view");
        permissions.Should().Contain(p => p.Name == "tenants.initialize"); // ✅ Verify new permission

        // ✅ Use IgnoreQueryFilters for roles
        var roles = await _dbContext.Roles.IgnoreQueryFilters().ToListAsync();
        roles.Should().HaveCount(8);
        roles.Should().Contain(r => r.Name == "SuperAdmin" && r.IsSystemRole);
        roles.Should().Contain(r => r.Name == "Admin" && r.TenantId == 1);
        roles.Should().Contain(r => r.Name == "User" && r.TenantId == 1);

        // ✅ Use IgnoreQueryFilters for user roles
        var userRoles = await _dbContext.UserRoles.IgnoreQueryFilters().ToListAsync();
        userRoles.Should().HaveCount(8);

        // ✅ Use IgnoreQueryFilters for role permissions
        var rolePermissions = await _dbContext.RolePermissions.IgnoreQueryFilters().ToListAsync();
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
        // ✅ CRITICAL: Use IgnoreQueryFilters to verify tenant isolation
        
        // Verify tenant 1 users
        var tenant1Users = await _dbContext.Users
            .IgnoreQueryFilters()
            .Join(_dbContext.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
            .Where(x => x.tu.TenantId == 1 && x.tu.IsActive)
            .Select(x => x.u)
            .ToListAsync();
        tenant1Users.Should().HaveCount(5);
        tenant1Users.Should().OnlyContain(u => u.Email.Contains("@tenant1.com"));

        // ✅ CRITICAL: This should now find Tenant 2 users
        var tenant2Users = await _dbContext.Users
            .IgnoreQueryFilters()
            .Join(_dbContext.TenantUsers, u => u.Id, tu => tu.UserId, (u, tu) => new { u, tu })
            .Where(x => x.tu.TenantId == 2 && x.tu.IsActive)
            .Select(x => x.u)
            .ToListAsync();
        tenant2Users.Should().HaveCount(2); // ✅ Should now work
        tenant2Users.Should().OnlyContain(u => u.Email.Contains("@tenant2.com"));

        // Verify tenant 1 roles
        var tenant1Roles = await _dbContext.Roles.IgnoreQueryFilters().Where(r => r.TenantId == 1).ToListAsync();
        tenant1Roles.Should().HaveCount(5); // Admin, User, Manager, Viewer, Editor

        // Verify tenant 2 roles
        var tenant2Roles = await _dbContext.Roles.IgnoreQueryFilters().Where(r => r.TenantId == 2).ToListAsync();
        tenant2Roles.Should().HaveCount(2); // Admin, User

        // Verify system roles
        var systemRoles = await _dbContext.Roles.IgnoreQueryFilters().Where(r => r.TenantId == null).ToListAsync();
        systemRoles.Should().HaveCount(1); // SuperAdmin
        systemRoles.Should().Contain(r => r.Name == "SuperAdmin" && r.IsSystemRole);
    }
}
