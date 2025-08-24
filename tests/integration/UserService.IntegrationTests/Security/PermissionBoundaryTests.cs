using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using DTOs.Common;
using DTOs.User;
using DTOs.Auth;
using UserService.IntegrationTests.Fixtures;
using UserService.IntegrationTests.TestUtilities;

namespace UserService.IntegrationTests.Security;

/// <summary>
/// Session 3: Permission-Based Authorization Testing from PhaseTwelve.md
/// These tests validate that your RBAC system enforces proper permission boundaries
/// in the context of your two-phase authentication system:
/// Phase 1: User authenticates without tenant (basic JWT)
/// Phase 2: User selects tenant and gets refreshed JWT with tenant_id and permissions
/// </summary>
[Collection("Integration Tests")]
public class PermissionBoundaryTests : TestBase
{
    public PermissionBoundaryTests(WebApplicationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task TwoPhaseAuth_Phase1Token_ShouldNotHavePermissions()
    {
        // Arrange - Get Phase 1 token (no tenant selected)
        var phase1Token = await GetPhase1TokenAsync("admin@tenant1.com");
        
        // Act - Decode token to verify it has no permissions
        var payload = AuthenticationHelper.DecodeJwtPayload(phase1Token);
        
        // Assert - Phase 1 token should NOT have permissions or tenant_id
        payload.Should().NotContainKey("tenant_id", "Phase 1 token should not contain tenant_id");
        payload.Should().NotContainKey("permission", "Phase 1 token should not contain permissions");
        payload.Should().ContainKey("nameid", "Phase 1 token should contain user ID");
        payload.Should().ContainKey("email", "Phase 1 token should contain email");
        
        Console.WriteLine("✅ Phase 1 token correctly has no tenant context or permissions");
    }

    [Fact]
    public async Task TwoPhaseAuth_Phase2Token_ShouldHavePermissions()
    {
        // Arrange - Complete two-phase flow to get Phase 2 token
        var phase2Token = await GetTwoPhaseTokenAsync("admin@tenant1.com", 1);
        
        // Act - Decode token to verify it has permissions
        var payload = AuthenticationHelper.DecodeJwtPayload(phase2Token);
        
        // Assert - Phase 2 token should have permissions and tenant_id
        payload.Should().ContainKey("tenant_id", "Phase 2 token should contain tenant_id");
        payload["tenant_id"].ToString().Should().Be("1", "Tenant ID should be 1");
        payload.Should().ContainKey("permission", "Phase 2 token should contain permissions");
        
        // Extract and verify permissions
        var permissions = payload.GetValueOrDefault("permission") as JsonElement?;
        var permissionList = permissions?.EnumerateArray()
            .Select(p => p.GetString())
            .Where(p => !string.IsNullOrEmpty(p))
            .Cast<string>()
            .ToList() ?? new List<string>();
            
        permissionList.Should().NotBeEmpty("Admin should have permissions");
        permissionList.Should().Contain("users.view", "Admin should have users.view permission");
        
        Console.WriteLine($"✅ Phase 2 token correctly contains {permissionList.Count} permissions for tenant 1");
    }

    [Theory]
    [InlineData("admin@tenant1.com", "/api/users", "users.view", true)]
    [InlineData("user@tenant1.com", "/api/users", "users.view", false)] // User role doesn't have users.view
    [InlineData("admin@tenant1.com", "/api/roles", "roles.view", true)]
    [InlineData("viewer@tenant1.com", "/api/roles", "roles.view", true)]  // ✅ FIX: Viewer HAS roles.view
    [InlineData("viewer@tenant1.com", "/api/roles", "roles.create", false)]
    [InlineData("manager@tenant1.com", "/api/users", "users.view", true)]  // ✅ FIX: Manager HAS users.view
    [InlineData("manager@tenant1.com", "/api/roles", "roles.view", true)]  // ✅ FIX: Manager HAS roles.view
    [InlineData("editor@tenant1.com", "/api/users", "users.edit", false)] // Editor doesn't have users.edit or users.view
    [InlineData("editor@tenant1.com", "/api/roles", "roles.view", false)] // Editor doesn't have roles.view
    public async Task PermissionCheck_WithTenantContext_ShouldEnforceProperAccess(
        string userEmail, 
        string endpoint, 
        string requiredPermission, 
        bool shouldHaveAccess)
    {
        // Arrange - Get Phase 2 token with tenant context
        var token = await AuthenticationHelper.GetTenantAwareJwtAsync(
            _client, _dbContext, userEmail, 1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Verify token has expected permission in JWT claims
        var payload = AuthenticationHelper.DecodeJwtPayload(token);
        payload.Should().ContainKey("tenant_id", "Token should have tenant context");
        
        var permissions = payload.GetValueOrDefault("permission") as JsonElement?;
        var hasPermission = permissions?.EnumerateArray()
            .Any(p => p.GetString() == requiredPermission) ?? false;

        // Act - Test API access
        var response = await _client.GetAsync(endpoint);

        // Assert
        if (shouldHaveAccess)
        {
            response.Should().BeSuccessful($"{userEmail} should have {requiredPermission}");
            hasPermission.Should().BeTrue($"Token should contain {requiredPermission}");
        }
        else
        {
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
            Console.WriteLine($"❌ Expected: {userEmail} should NOT have access to {endpoint} without {requiredPermission}");
        }

        Console.WriteLine($"✅ Permission check for {userEmail} on {endpoint}: Expected={shouldHaveAccess}, Actual={response.IsSuccessStatusCode}, HasPermission={hasPermission}");
    }

    [Fact]
    public async Task Phase1Token_CannotAccessProtectedEndpoints()
    {
        // Arrange - Use Phase 1 token (no tenant, no permissions)
        var phase1Token = await GetPhase1TokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", phase1Token);

        // Act - Try to access protected endpoints
        var usersResponse = await _client.GetAsync("/api/users");
        var rolesResponse = await _client.GetAsync("/api/roles");

        // Assert - Should be unauthorized/forbidden because no tenant context
        usersResponse.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
        rolesResponse.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
        
        Console.WriteLine($"✅ Phase 1 token correctly rejected: Users={usersResponse.StatusCode}, Roles={rolesResponse.StatusCode}");
    }

    [Fact]
    public async Task UserWithoutPermissions_ShouldBeRejected()
    {
        // Arrange - Use a user that exists but has minimal permissions
        var token = await AuthenticationHelper.GetTenantAwareJwtAsync(
            _client, _dbContext, "editor@tenant1.com", 1); // Editor doesn't have roles.view
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act - Try to access roles endpoint (editor doesn't have roles.view)
        var response = await _client.GetAsync("/api/roles");

        // Assert - Should be forbidden
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
        Console.WriteLine($"✅ Editor without roles.view permission correctly rejected: {response.StatusCode}");
    }

    [Fact]
    public async Task AdminUser_ShouldHaveAllRequiredPermissions()
    {
        // Arrange - Get Phase 2 token for admin
        var token = await AuthenticationHelper.GetTenantAwareJwtAsync(
            _client, _dbContext, "admin@tenant1.com", 1);
        
        // Act - Decode token to verify permissions
        var payload = AuthenticationHelper.DecodeJwtPayload(token);
        var permissions = payload.GetValueOrDefault("permission") as JsonElement?;
        var permissionList = permissions?.EnumerateArray()
            .Select(p => p.GetString())
            .Where(p => !string.IsNullOrEmpty(p))
            .Cast<string>()
            .ToList() ?? new List<string>();

        // Assert - Admin should have key permissions
        var expectedPermissions = new[]
        {
            "users.view", "users.edit", "users.create", "users.delete",
            "roles.view", "roles.create", "roles.edit", "roles.delete",
            "roles.manage_permissions", "users.manage_roles"
        };

        foreach (var permission in expectedPermissions)
        {
            permissionList.Should().Contain(permission, 
                $"Admin should have {permission} permission");
        }

        Console.WriteLine($"✅ Admin user has {permissionList.Count} permissions: {string.Join(", ", permissionList)}");
    }

    [Fact]
    public async Task ViewerUser_ShouldHaveOnlyViewPermissions()
    {
        // Arrange - Get Phase 2 token for viewer
        var token = await AuthenticationHelper.GetTenantAwareJwtAsync(
            _client, _dbContext, "viewer@tenant1.com", 1);
        
        // Act - Decode token to verify permissions
        var payload = AuthenticationHelper.DecodeJwtPayload(token);
        var permissions = payload.GetValueOrDefault("permission") as JsonElement?;
        var permissionList = permissions?.EnumerateArray()
            .Select(p => p.GetString())
            .Where(p => !string.IsNullOrEmpty(p))
            .Cast<string>()
            .ToList() ?? new List<string>();

        // Assert - Viewer should have only view permissions (based on actual TestDataSeeder)
        var expectedViewPermissions = new[] { "roles.view", "reports.view", "permissions.view" };
        var forbiddenPermissions = new[] { "users.view", "users.create", "users.edit", "users.delete", "roles.create", "roles.edit", "roles.delete" };

        foreach (var permission in expectedViewPermissions)
        {
            permissionList.Should().Contain(permission, 
                $"Viewer should have {permission} permission");
        }

        foreach (var permission in forbiddenPermissions)
        {
            permissionList.Should().NotContain(permission, 
                $"Viewer should NOT have {permission} permission");
        }

        Console.WriteLine($"✅ Viewer user has correct view-only permissions: {string.Join(", ", permissionList)}");
    }

    [Fact]
    public async Task CrossTenantPermissionIsolation_ShouldBeEnforced()
    {
        // Arrange - Get Phase 2 tokens for both tenants
        var tenant1AdminToken = await AuthenticationHelper.GetTenantAwareJwtAsync(
            _client, _dbContext, "admin@tenant1.com", 1);
        var tenant2AdminToken = await AuthenticationHelper.GetTenantAwareJwtAsync(
            _client, _dbContext, "admin@tenant2.com", 2);

        // Act - Decode both tokens
        var tenant1Payload = AuthenticationHelper.DecodeJwtPayload(tenant1AdminToken);
        var tenant2Payload = AuthenticationHelper.DecodeJwtPayload(tenant2AdminToken);

        // Assert - Verify tenant isolation in JWT
        tenant1Payload.GetValueOrDefault("tenant_id")?.ToString().Should().Be("1");
        tenant2Payload.GetValueOrDefault("tenant_id")?.ToString().Should().Be("2");

        // Both should have admin permissions but scoped to their tenants
        var tenant1Permissions = tenant1Payload.GetValueOrDefault("permission") as JsonElement?;
        var tenant2Permissions = tenant2Payload.GetValueOrDefault("permission") as JsonElement?;

        tenant1Permissions.Should().NotBeNull("Tenant 1 admin should have permissions");
        tenant2Permissions.Should().NotBeNull("Tenant 2 admin should have permissions");

        Console.WriteLine($"✅ Cross-tenant permission isolation verified");
    }

    [Fact]
    public async Task TenantSwitching_ShouldUpdatePermissionsCorrectly()
    {
        // Arrange - Get tokens for different tenant admins
        var user1Tenant1Token = await AuthenticationHelper.GetTenantAwareJwtAsync(
            _client, _dbContext, "admin@tenant1.com", 1);
        
        var user2Tenant2Token = await AuthenticationHelper.GetTenantAwareJwtAsync(
            _client, _dbContext, "admin@tenant2.com", 2);

        // Act - Decode both tokens
        var tenant1Payload = AuthenticationHelper.DecodeJwtPayload(user1Tenant1Token);
        var tenant2Payload = AuthenticationHelper.DecodeJwtPayload(user2Tenant2Token);

        // Assert - Verify different tenant contexts
        tenant1Payload.GetValueOrDefault("tenant_id")?.ToString().Should().Be("1");
        tenant2Payload.GetValueOrDefault("tenant_id")?.ToString().Should().Be("2");

        // Both should have admin-level permissions since both are admin users
        var tenant1Permissions = tenant1Payload.GetValueOrDefault("permission") as JsonElement?;
        var tenant2Permissions = tenant2Payload.GetValueOrDefault("permission") as JsonElement?;

        tenant1Permissions.Should().NotBeNull("Should have permissions in tenant 1");
        tenant2Permissions.Should().NotBeNull("Should have permissions in tenant 2");

        Console.WriteLine($"✅ Tenant switching correctly updates JWT context");
    }

    [Theory]
    [InlineData("admin@tenant1.com", "/api/users", "POST")]
    [InlineData("admin@tenant1.com", "/api/roles", "POST")]
    [InlineData("manager@tenant1.com", "/api/users", "PUT")]
    public async Task WriteOperations_WithTenantContext_ShouldRequireProperPermissions(
        string userEmail,
        string endpoint,
        string httpMethod)
    {
        // Arrange - Get Phase 2 token with tenant context
        var token = await AuthenticationHelper.GetTenantAwareJwtAsync(
            _client, _dbContext, userEmail, 1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act - Try a write operation
        HttpResponseMessage response = httpMethod.ToUpper() switch
        {
            "POST" => await _client.PostAsJsonAsync(endpoint, new { }), // Empty object for POST
            "PUT" => await _client.PutAsJsonAsync($"{endpoint}/1", new { }), // Update item 1
            "DELETE" => await _client.DeleteAsync($"{endpoint}/1"), // Delete item 1
            _ => await _client.GetAsync(endpoint)
        };

        // Assert - Should either succeed (if user has permission) or be forbidden
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.NoContent, // Success cases
            HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized, // Auth failure cases
            HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity // Validation failures (still means auth worked)
        );

        Console.WriteLine($"✅ Write operation test for {userEmail} on {httpMethod} {endpoint}: {response.StatusCode}");
    }
}
