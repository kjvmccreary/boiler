using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using DTOs.Common;
using DTOs.Auth;
using DTOs.User;
using Contracts.Services;
using UserService.IntegrationTests.Fixtures;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace UserService.IntegrationTests.Security;

[Collection("CrossTenantSecurity")]
public class CrossTenantSecurityTests : TestBase
{
    public CrossTenantSecurityTests(WebApplicationTestFixture fixture) : base(fixture) { }

    #region Role Access Cross-Tenant Tests

    [Fact]
    public async Task GetRole_CrossTenantAttempt_ShouldReturnNotFound()
    {
        // Arrange - Tenant 1 admin trying to access Tenant 2 role
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        // Act - Try to access Tenant 2 Admin role (assuming ID 5 based on seeded data)
        var response = await _client.GetAsync("/api/roles/5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Be("Role not found");
    }

    [Fact]
    public async Task UpdateRole_CrossTenantAttempt_ShouldReturnNotFound()
    {
        // Arrange - Tenant 1 admin trying to update Tenant 2 role
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        var updateRequest = new UpdateRoleDto
        {
            Name = "HackedRole",
            Description = "Attempting to modify cross-tenant role",
            Permissions = new List<string> { "users.view" }
        };

        // Act - Try to update Tenant 2 role
        var response = await _client.PutAsJsonAsync("/api/roles/5", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Be("Role not found");
    }

    [Fact]
    public async Task DeleteRole_CrossTenantAttempt_ShouldReturnNotFound()
    {
        // Arrange - Tenant 1 admin trying to delete Tenant 2 role
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        // Act - Try to delete Tenant 2 role
        var response = await _client.DeleteAsync("/api/roles/6"); // Tenant 2 User role

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Be("Role not found");
    }

    [Fact]
    public async Task GetRolePermissions_CrossTenantAttempt_ShouldReturnNotFound()
    {
        // Arrange - Tenant 1 admin trying to view Tenant 2 role permissions
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        // Act - Try to get permissions for Tenant 2 role
        var response = await _client.GetAsync("/api/roles/5/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<string>>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Be("Role not found");
    }

    [Fact]
    public async Task UpdateRolePermissions_CrossTenantAttempt_ShouldReturnNotFound()
    {
        // Arrange - Tenant 1 admin trying to update Tenant 2 role permissions
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        var permissions = new List<string> { "malicious.permission" };

        // Act - Try to update permissions for Tenant 2 role
        var response = await _client.PutAsJsonAsync("/api/roles/5/permissions", permissions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Be("Role not found");
    }

    [Fact]
    public async Task GetRoleUsers_CrossTenantAttempt_ShouldReturnNotFound()
    {
        // Arrange - Tenant 1 admin trying to view users in Tenant 2 role
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        // Act - Try to get users for Tenant 2 role
        var response = await _client.GetAsync("/api/roles/5/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<UserInfo>>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Be("Role not found");
    }

    #endregion

    #region User-Role Assignment Cross-Tenant Tests

    [Fact]
    public async Task AssignRoleToUser_CrossTenantRole_ShouldFail()
    {
        // Arrange - Tenant 1 admin trying to assign Tenant 2 role to Tenant 1 user
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        var assignRequest = new AssignRoleDto
        {
            UserId = 2, // Tenant 1 user
            RoleId = 5  // Tenant 2 role
        };

        // Act - Try to assign cross-tenant role
        var response = await _client.PostAsJsonAsync("/api/roles/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("Role not found");
    }

    [Fact]
    public async Task AssignRoleToUser_CrossTenantUser_ShouldFail()
    {
        // Arrange - Tenant 1 admin trying to assign role to Tenant 2 user
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        var assignRequest = new AssignRoleDto
        {
            UserId = 6, // Tenant 2 user (admin@tenant2.com) 
            RoleId = 2  // Tenant 1 role
        };

        // Act - Try to assign role to cross-tenant user
        var response = await _client.PostAsJsonAsync("/api/roles/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        // Note: This might return different error depending on which validation fails first
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task RemoveRoleFromUser_CrossTenantRole_ShouldFail()
    {
        // Arrange - Tenant 1 admin trying to remove Tenant 2 role from user
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        // Act - Try to remove cross-tenant role
        var response = await _client.DeleteAsync("/api/roles/5/users/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserRoles_CrossTenantUser_ShouldReturnForbiddenOrNotFound()
    {
        // Arrange - Tenant 1 admin trying to view Tenant 2 user's roles
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        // Act - Try to get roles for Tenant 2 user
        var response = await _client.GetAsync("/api/roles/users/6"); // Tenant 2 admin user

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleDto>>>();
        result!.Success.Should().BeFalse();
    }

    #endregion

    #region User Management Cross-Tenant Tests

    [Fact]
    public async Task GetUser_CrossTenantAttempt_ShouldReturnNotFound()
    {
        // Arrange - Tenant 1 admin trying to access Tenant 2 user
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        // Act - Try to access Tenant 2 user
        var response = await _client.GetAsync("/api/users/6"); // Tenant 2 admin user

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    [Fact]
    public async Task GetUsers_ShouldOnlyReturnCurrentTenantUsers()
    {
        // Arrange - Tenant 1 admin should only see Tenant 1 users
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        // Act - Get users list
        var response = await _client.GetAsync("/api/users?pageSize=50");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<UserDto>>>();
        result!.Success.Should().BeTrue();
        result.Data!.Items.Should().OnlyContain(u => u.TenantId == 1);
        result.Data.Items.Should().NotContain(u => u.Email.Contains("@tenant2.com"));
        result.Data.Items.Should().Contain(u => u.Email.Contains("@tenant1.com"));
    }

    [Fact]
    public async Task UpdateUser_CrossTenantAttempt_ShouldReturnNotFound()
    {
        // Arrange - Tenant 1 admin trying to update Tenant 2 user
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        var updateRequest = new UserUpdateDto
        {
            FirstName = "Hacked",
            LastName = "User",
            IsActive = false
        };

        // Act - Try to update Tenant 2 user
        var response = await _client.PutAsync("/api/users/6", JsonContent.Create(updateRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    [Fact]
    public async Task DeleteUser_CrossTenantAttempt_ShouldReturnNotFound()
    {
        // Arrange - Tenant 1 admin trying to delete Tenant 2 user
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        // Act - Try to delete Tenant 2 user
        var response = await _client.DeleteAsync("/api/users/7"); // Tenant 2 regular user

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    #endregion

    #region Tenant Verification Tests

    [Fact]
    public async Task GetRoles_ShouldOnlyReturnCurrentTenantAndSystemRoles()
    {
        // Arrange - Tenant 1 admin should only see Tenant 1 and system roles
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);

        // Act - Get roles list
        var response = await _client.GetAsync("/api/roles?pageSize=50");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<RoleDto>>>();
        result!.Success.Should().BeTrue();
        
        // Should contain Tenant 1 roles and system roles
        result.Data!.Items.Should().OnlyContain(r => 
            r.TenantId == 1 || r.TenantId == null || r.TenantId == 0 || r.IsSystemRole);
        
        // Should not contain Tenant 2 specific roles
        result.Data.Items.Where(r => !r.IsSystemRole).Should().OnlyContain(r => r.TenantId == 1);
    }

    [Fact]
    public async Task TenantIsolation_DifferentTenantsCannotSeeEachOthersData()
    {
        // Arrange
        var tenant1Token = await GetAuthTokenAsync("admin@tenant1.com");
        var tenant2Token = await GetTenant2AdminTokenAsync();

        // Act - Get data from both tenants
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant1Token);
        var tenant1RolesResponse = await _client.GetAsync("/api/roles");
        var tenant1UsersResponse = await _client.GetAsync("/api/users");

        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenant2Token);
        var tenant2RolesResponse = await _client.GetAsync("/api/roles");
        var tenant2UsersResponse = await _client.GetAsync("/api/users");

        // Assert
        tenant1RolesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        tenant1UsersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        tenant2RolesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        tenant2UsersResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var tenant1Roles = await tenant1RolesResponse.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<RoleDto>>>();
        var tenant1Users = await tenant1UsersResponse.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<UserDto>>>();
        var tenant2Roles = await tenant2RolesResponse.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<RoleDto>>>();
        var tenant2Users = await tenant2UsersResponse.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<UserDto>>>();

        // Verify tenant isolation
        tenant1Users!.Data!.Items.Should().OnlyContain(u => u.TenantId == 1);
        tenant2Users!.Data!.Items.Should().OnlyContain(u => u.TenantId == 2);

        // Tenant-specific roles should be isolated (excluding system roles)
        var tenant1SpecificRoles = tenant1Roles!.Data!.Items.Where(r => !r.IsSystemRole).ToList();
        var tenant2SpecificRoles = tenant2Roles!.Data!.Items.Where(r => !r.IsSystemRole).ToList();

        tenant1SpecificRoles.Should().OnlyContain(r => r.TenantId == 1);
        tenant2SpecificRoles.Should().OnlyContain(r => r.TenantId == 2);
    }

    #endregion

    #region System Role Security Tests

    [Fact]
    public async Task SystemRoles_ShouldBeReadOnlyForTenantAdmins()
    {
        // Arrange - Tenant admin should be able to view but not modify system roles
        var tenantToken = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", tenantToken);

        // Act & Assert - Should be able to view system role
        var viewResponse = await _client.GetAsync("/api/roles/1"); // SuperAdmin role
        viewResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Should be able to view permissions
        var permissionsResponse = await _client.GetAsync("/api/roles/1/permissions");
        permissionsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Should NOT be able to update system role
        var updateRequest = new UpdateRoleDto
        {
            Name = "HackedSuperAdmin",
            Description = "Attempting to modify system role",
            Permissions = new List<string> { "limited.permission" }
        };
        var updateResponse = await _client.PutAsJsonAsync("/api/roles/1", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        // Should NOT be able to delete system role
        var deleteResponse = await _client.DeleteAsync("/api/roles/1");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        // Should NOT be able to update system role permissions
        var updatePermissionsResponse = await _client.PutAsJsonAsync("/api/roles/1/permissions", new List<string> { "limited.permission" });
        updatePermissionsResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    #endregion
}
