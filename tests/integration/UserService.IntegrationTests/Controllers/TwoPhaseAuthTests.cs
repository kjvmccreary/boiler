using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Xunit;
using FluentAssertions;
using UserService.IntegrationTests.Fixtures;
using UserService.IntegrationTests.TestUtilities;
using Common.Data;
using DTOs.Auth;
using DTOs.Common;
using DTOs.User;

namespace UserService.IntegrationTests.Controllers;

/// <summary>
/// 🔧 CRITICAL FIX: Use shared "Integration Tests" collection to prevent multiple app instances
/// This test class now properly tests the two-phase authentication flow
/// </summary>
[Collection("Integration Tests")]
public class TwoPhaseAuthTests : TestBase
{
    public TwoPhaseAuthTests(WebApplicationTestFixture factory) : base(factory)
    {
    }

    [Fact]
    public async Task AuthenticationHelper_CanGenerateInitialJwt()
    {
        // Act - Test that we can generate a Phase 1 JWT (no tenant)
        var phase1Token = await GetPhase1TokenAsync("admin@tenant1.com");

        // Assert
        phase1Token.Should().NotBeNullOrEmpty();
        
        // Decode JWT and verify NO tenant_id
        var payload = AuthenticationHelper.DecodeJwtPayload(phase1Token);
        payload.Should().NotContainKey("tenant_id", "Phase 1 token should not contain tenant_id");
        payload.Should().ContainKey("nameid", "Token should contain user ID");
        payload.Should().ContainKey("email", "Token should contain email");
        
        Console.WriteLine("✅ Phase 1 JWT Test: Token generated without tenant context");
    }

    [Fact]
    public async Task AuthenticationHelper_CanGenerateTenantAwareJwt()
    {
        // Act - Test that we can generate a Phase 2 JWT (with tenant)
        var tenantToken = await GetAuthTokenAsync("admin@tenant1.com", 1);

        // Assert
        tenantToken.Should().NotBeNullOrEmpty();
        
        // Decode JWT and verify tenant_id is present
        var payload = AuthenticationHelper.DecodeJwtPayload(tenantToken);
        payload.Should().ContainKey("tenant_id", "Phase 2 token should contain tenant_id");
        payload["tenant_id"].ToString().Should().Be("1", "Tenant ID should be 1");
        
        // Should also have permissions now
        payload.Should().ContainKey("permission", "Phase 2 token should contain permissions");
        
        Console.WriteLine("✅ Phase 2 JWT Test: Token generated with tenant context");
    }

    [Fact]
    public async Task TwoPhaseFlow_ShouldSimulateCompleteAuthentication()
    {
        // Act - Test the complete two-phase flow simulation
        var finalToken = await GetTwoPhaseTokenAsync("admin@tenant1.com", 1);

        // Assert
        finalToken.Should().NotBeNullOrEmpty();
        
        // Verify final token has everything needed
        var payload = AuthenticationHelper.DecodeJwtPayload(finalToken);
        payload.Should().ContainKey("tenant_id", "Final token should contain tenant_id");
        payload.Should().ContainKey("permission", "Final token should contain permissions");
        payload.Should().ContainKey("nameid", "Final token should contain user ID");
        
        Console.WriteLine("✅ Complete Two-Phase Flow Test: Authentication simulation successful");
    }

    [Fact]
    public async Task UserService_WithTenantToken_CanAccessUsers()
    {
        // Arrange - Get a tenant-aware token using the simplified helper
        var tenantToken = await GetAuthTokenAsync("admin@tenant1.com", 1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenantToken);

        // Act - Try to access UserService endpoints
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.Should().BeSuccessful("UserService should accept valid tenant-aware JWT");
        
        var usersData = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<UserDto>>>();
        usersData.Should().NotBeNull();
        usersData!.Success.Should().BeTrue();
        usersData.Data.Should().NotBeNull();
        
        Console.WriteLine($"✅ UserService Access Test: Retrieved {usersData.Data!.Items.Count} users");
    }

    [Fact]
    public async Task TenantIsolation_Tenant1Admin_CannotSeeTenant2Users()
    {
        // Arrange - Get tenant 1 token
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com", 1);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant1Token);

        // Act - Get users (should only see tenant 1 users)
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.Should().BeSuccessful();
        var usersData = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<UserDto>>>();
        
        usersData!.Data!.Items.Should().NotBeEmpty();
        usersData.Data.Items.Should().OnlyContain(u => 
            u.Email.Contains("@tenant1.com") || u.Email == "super@admin.com", 
            "Should only see tenant 1 users and super admin");
        
        Console.WriteLine($"✅ Tenant Isolation Test: Tenant 1 admin sees {usersData.Data.Items.Count} users (tenant 1 only)");
    }

    [Fact]
    public async Task TenantIsolation_Tenant2Admin_CannotSeeTenant1Users()
    {
        // Arrange - Get tenant 2 token
        var tenant2Token = await GetTenant2AdminTokenAsync("admin@tenant2.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant2Token);

        // Act - Get users
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.Should().BeSuccessful();
        var usersData = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<UserDto>>>();
        
        usersData!.Data!.Items.Should().NotBeEmpty();
        usersData.Data.Items.Should().OnlyContain(u => 
            u.Email.Contains("@tenant2.com") || u.Email == "super@admin.com", 
            "Should only see tenant 2 users and super admin");
        
        Console.WriteLine($"✅ Tenant Isolation Test: Tenant 2 admin sees {usersData.Data.Items.Count} users (tenant 2 only)");
    }

    [Fact]
    public async Task JwtValidation_ShouldWork()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@tenant1.com", 1);

        // Act & Assert - Validate JWT structure
        AuthenticationHelper.ValidateJwtClaims(token, new Dictionary<string, object>
        {
            { "tenant_id", "1" },
            { "nameid", "1" } // Validate user ID instead of email to avoid duplicates
        });
        
        // Validate permissions
        AuthenticationHelper.ValidateJwtPermissions(token, 
            "users.view", "users.create");
        
        Console.WriteLine("✅ JWT Validation Test: All claims and permissions verified");
    }

    [Theory]
    [InlineData("admin@tenant1.com", 1, true)]  // Admin should have permissions
    [InlineData("user@tenant1.com", 1, false)] // Regular user shouldn't see all users
    [InlineData("admin@tenant2.com", 2, true)]  // Tenant 2 admin should have permissions
    public async Task PermissionCheck_ShouldEnforceProperAccess(
        string userEmail, 
        int tenantId, 
        bool shouldHaveAccess)
    {
        // Arrange
        var token = await GetAuthTokenAsync(userEmail, tenantId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act - Try to access admin endpoint
        var response = await _client.GetAsync("/api/users");

        // Assert
        if (shouldHaveAccess)
        {
            response.Should().BeSuccessful($"{userEmail} should have access to users endpoint");
        }
        else
        {
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
        }
        
        Console.WriteLine($"✅ Permission Test: {userEmail} access = {response.StatusCode}");
    }

    [Fact]
    public async Task UnauthorizedTenantAccess_ShouldBeBlocked()
    {
        // Test that a user cannot access unauthorized tenants
        var shouldFail = await AuthenticationHelper.TestUnauthorizedTenantAccess(
            _client, _dbContext, "admin@tenant1.com", 2);
        
        shouldFail.Should().BeTrue("User should not be able to access unauthorized tenant");
        
        Console.WriteLine("✅ Security Test: Cross-tenant access properly blocked");
    }
}
