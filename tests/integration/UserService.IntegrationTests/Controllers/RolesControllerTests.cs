using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using DTOs.Common;
using DTOs.Auth;
using DTOs.User;
using UserService.IntegrationTests.Fixtures;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace UserService.IntegrationTests.Controllers;

[Collection("RolesController")]
public class RolesControllerTests : TestBase
{
    public RolesControllerTests(WebApplicationTestFixture fixture) : base(fixture) { }

    #region Role CRUD Tests

    [Fact]
    public async Task GetRoles_WithAdminToken_ReturnsRoleList()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<RoleDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateRole_WithValidData_ReturnsCreatedRole()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateRoleDto
        {
            Name = "TestRole",
            Description = "Test Role Description",
            Permissions = new List<string> { "users.view", "users.edit" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("TestRole");
    }

    [Fact]
    public async Task UpdateRole_WithValidData_ReturnsUpdatedRole()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateRoleDto
        {
            Name = "TestRoleForUpdate",
            Description = "Test Role that will be updated",
            Permissions = new List<string> { "users.view" }
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/roles", createRequest);
        var createdRole = await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();

        var updateRequest = new UpdateRoleDto
        {
            Name = "UpdatedTestRole",
            Description = "Updated Description",
            Permissions = new List<string> { "users.view" }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/roles/{createdRole!.Data!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("UpdatedTestRole");
    }

    [Fact]
    public async Task DeleteRole_WithValidRole_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateRoleDto
        {
            Name = "TestRoleForDeletion",
            Description = "Role to be deleted",
            Permissions = new List<string> { "users.view" }
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/roles", createRequest);
        var createdRole = await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();

        // Act
        var response = await _client.DeleteAsync($"/api/roles/{createdRole!.Data!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    #endregion

    #region Role Assignment Tests

    [Fact]
    public async Task AssignRoleToUser_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var assignRequest = new AssignUserRoleDto
        {
            RoleId = 2 // Admin role for Tenant 1
        };

        // Act - Assign Admin role to Manager user (ID 3)
        var response = await _client.PostAsJsonAsync("/api/users/3/roles", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task RemoveRoleFromUser_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Remove Manager role from Manager user
        var response = await _client.DeleteAsync("/api/roles/4/users/3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    #endregion

    #region Permission Tests

    [Fact]
    public async Task GetRolePermissions_WithValidRole_ReturnsPermissions()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Get permissions for Admin role (ID 2)
        var response = await _client.GetAsync("/api/roles/2/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<string>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
        result.Data.Should().Contain("users.view");
        result.Data.Should().Contain("roles.view");
    }

    [Fact]
    public async Task GetRolePermissions_WithSystemRole_ReturnsSystemPermissions()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Get permissions for SuperAdmin role (ID 1)
        var response = await _client.GetAsync("/api/roles/1/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<string>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().NotBeEmpty();
        result.Data.Should().Contain("users.view");
        result.Data.Should().Contain("roles.view");
        result.Data.Should().Contain("tenants.view");
    }

    [Fact]
    public async Task UpdateRolePermissions_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var permissions = new List<string> { "users.view", "users.edit", "roles.view" };

        // Act - Update permissions for Admin role
        var response = await _client.PutAsJsonAsync("/api/roles/2/permissions", permissions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRolePermissions_WithInvalidPermissions_ShouldSucceed()
    {
        // Note: Backend doesn't validate permission names, so invalid permissions are accepted
        
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var permissions = new List<string> { "invalid.permission", "users.view" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/roles/2/permissions", permissions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    #endregion

    #region ✅ NEW: User Count Tests
    
    [Fact]
    public async Task GetRoles_WithUserAssignments_ReturnsCorrectUserCounts()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<RoleDto>>>();
        
        // Verify admin role has at least 1 user (the admin user)
        var adminRole = result!.Data!.Items.FirstOrDefault(r => r.Name == "Admin");
        adminRole.Should().NotBeNull();
        adminRole!.UserCount.Should().BeGreaterOrEqualTo(1);
        
        // Verify Manager role has at least 1 user
        var managerRole = result.Data.Items.FirstOrDefault(r => r.Name == "Manager");
        managerRole.Should().NotBeNull();
        managerRole!.UserCount.Should().BeGreaterOrEqualTo(1);
        
        // Verify Viewer role has at least 1 user
        var viewerRole = result.Data.Items.FirstOrDefault(r => r.Name == "Viewer");
        viewerRole.Should().NotBeNull();
        viewerRole!.UserCount.Should().BeGreaterOrEqualTo(1);
    }

    #endregion

    #region ✅ NEW: Role Users Endpoint Tests

    [Fact]
    public async Task GetRoleUsers_WithAdminRole_ReturnsAssignedUsers()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Get users for Admin role (ID 2)
        var response = await _client.GetAsync("/api/roles/2/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<UserInfo>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data!.Should().Contain(u => u.Email == "admin@tenant1.com");
    }

    [Fact]
    public async Task GetRoleUsers_WithViewerRole_ReturnsAssignedUsers()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Get users for Viewer role (ID 7)
        var response = await _client.GetAsync("/api/roles/7/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<UserInfo>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data!.Should().Contain(u => u.Email == "viewer@tenant1.com");
        result.Data.Should().Contain(u => u.Email == "editor@tenant1.com"); // Editor also has Viewer role
    }

    [Fact]
    public async Task GetRoleUsers_WithEmptyRole_ReturnsEmptyList()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Create a role with no users
        var createRequest = new CreateRoleDto
        {
            Name = "EmptyTestRole",
            Description = "Role with no users for testing",
            Permissions = new List<string> { "users.view" }
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/roles", createRequest);
        var createdRole = await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();

        // Act
        var response = await _client.GetAsync($"/api/roles/{createdRole!.Data!.Id}/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<UserInfo>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRoleUsers_WithNonExistentRole_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Try to get users for non-existent role
        var response = await _client.GetAsync("/api/roles/9999/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region ✅ NEW: Cross-Tenant Security Tests

    [Fact]
    public async Task GetRoleUsers_CrossTenantAttempt_ReturnsNotFound()
    {
        // Arrange - Tenant 1 admin trying to access Tenant 2 role
        var token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Try to get users for a tenant 2 role (assuming role ID 5 is tenant 2 Admin)
        var response = await _client.GetAsync("/api/roles/5/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRolePermissions_CrossTenantAttempt_ReturnsNotFound()
    {
        // Arrange - Tenant 1 admin trying to access Tenant 2 role permissions
        var token = await GetAuthTokenAsync("admin@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Try to get permissions for a tenant 2 role
        var response = await _client.GetAsync("/api/roles/5/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task GetRoles_WithUserToken_ReturnsSuccess()
    {
        // Arrange - User role HAS roles.view permission in our RBAC model
        var token = await GetUserTokenAsync("user@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<RoleDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateRole_WithoutPermission_ReturnsForbidden()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateRoleDto
        {
            Name = "TestRole",
            Description = "Test Description"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetRoleUsers_WithoutPermission_ReturnsForbidden()
    {
        // Arrange - User role does NOT have users.view permission
        var token = await GetUserTokenAsync("user@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles/2/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetRoleUsers_WithManagerRole_ReturnsSuccess()
    {
        // Arrange - Manager role HAS users.view permission
        var token = await GetManagerTokenAsync("manager@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles/2/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<UserInfo>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    #endregion

    #region ✅ NEW: Enhanced Permission Tests

    [Fact]
    public async Task GetRolePermissions_WithMultiplePermissions_ReturnsAllPermissions()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Get permissions for Manager role
        var response = await _client.GetAsync("/api/roles/4/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<string>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().Contain("users.view");
        result.Data.Should().Contain("users.edit");
        result.Data.Should().Contain("roles.view");
        result.Data.Should().Contain("reports.view");
    }

    [Fact]
    public async Task UpdateRolePermissions_RemoveAllPermissions_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Create a test role first
        var createRequest = new CreateRoleDto
        {
            Name = "TestRoleForPermissionRemoval",
            Description = "Test role for permission removal",
            Permissions = new List<string> { "users.view", "roles.view" }
        };
        
        var createResponse = await _client.PostAsJsonAsync("/api/roles", createRequest);
        var createdRole = await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();

        // Act - Remove all permissions (empty list)
        var response = await _client.PutAsJsonAsync($"/api/roles/{createdRole!.Data!.Id}/permissions", new List<string>());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeTrue();

        // Verify permissions were removed
        var permissionsResponse = await _client.GetAsync($"/api/roles/{createdRole.Data.Id}/permissions");
        var permissionsResult = await permissionsResponse.Content.ReadFromJsonAsync<ApiResponseDto<List<string>>>();
        permissionsResult!.Data.Should().BeEmpty();
    }

    #endregion
}
