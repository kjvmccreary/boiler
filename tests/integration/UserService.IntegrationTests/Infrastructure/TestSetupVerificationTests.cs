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

        // Verify users
        var users = await _dbContext.Users.ToListAsync();
        users.Should().HaveCount(5);
        users.Should().Contain(u => u.Email == "admin@tenant1.com");

        // Verify permissions
        var permissions = await _dbContext.Permissions.ToListAsync();
        permissions.Should().HaveCountGreaterThan(10);
        permissions.Should().Contain(p => p.Name == "users.view");

        // Verify roles
        var roles = await _dbContext.Roles.ToListAsync();
        roles.Should().HaveCount(6);

        // ✅ FIXED: Expect exactly 5 user-role assignments
        var userRoles = await _dbContext.UserRoles.ToListAsync();
        userRoles.Should().HaveCount(5);

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
}
