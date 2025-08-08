using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using DTOs.Common;
using DTOs.Auth;
using UserService.IntegrationTests.Fixtures;
using Xunit;

namespace UserService.IntegrationTests.Controllers;

public class RolesControllerTests : TestBase
{
    public RolesControllerTests(WebApplicationTestFixture fixture) : base(fixture) { }

    #region Role List Tests

    [Fact]
    public async Task GetRoles_WithAdminToken_ReturnsRoleList()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@tenant1.com", "Admin");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        
        // Should include tenant roles (Admin, User, Manager) + system roles (SuperAdmin)
        result.Data!.Should().HaveCountGreaterOrEqualTo(4);
        result.Data.Should().Contain(r => r.Name == "Admin" && r.TenantId == 1);
        result.Data.Should().Contain(r => r.Name == "User" && r.TenantId == 1);
        result.Data.Should().Contain(r => r.Name == "Manager" && r.TenantId == 1);
        result.Data.Should().Contain(r => r.Name == "SuperAdmin" && r.IsSystemRole);
    }

    [Fact]
    public async Task GetRoles_WithUserToken_ReturnsForbidden()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetRoles_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles?page=1&pageSize=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleDto>>>();
        result!.Data!.Should().HaveCountLessOrEqualTo(2);
    }

    [Fact]
    public async Task GetRoles_WithSearchTerm_ReturnsFilteredResults()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles?searchTerm=Admin");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleDto>>>();
        result!.Data!.Should().OnlyContain(r => r.Name.Contains("Admin", StringComparison.OrdinalIgnoreCase) ||
                                              r.Description.Contains("Admin", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetRoles_InvalidPagination_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles?page=0&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleDto>>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("Page number must be greater than 0");
    }

    #endregion

    #region Individual Role Tests

    [Fact]
    public async Task GetRole_WithValidId_ReturnsRole()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Get Admin role (ID: 2)
        var response = await _client.GetAsync($"/api/roles/{GetTenant1AdminRoleId()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(GetTenant1AdminRoleId());
        result.Data.Name.Should().Be("Admin");
        result.Data.TenantId.Should().Be(1);
        result.Data.Permissions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetRole_WithSystemRole_ReturnsRole()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Get SuperAdmin role (ID: 1)
        var response = await _client.GetAsync($"/api/roles/{GetSystemSuperAdminRoleId()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(GetSystemSuperAdminRoleId());
        result.Data.Name.Should().Be("SuperAdmin");
        result.Data.IsSystemRole.Should().BeTrue();
        result.Data.TenantId.Should().BeNull();
    }

    [Fact]
    public async Task GetRole_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("Role not found");
    }

    [Fact]
    public async Task GetRole_CrossTenantAccess_ReturnsNotFound()
    {
        // Arrange - Tenant 1 admin trying to access Tenant 2 role
        var token = await GetAuthTokenAsync("admin@tenant1.com", "Admin");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Try to access Tenant 2 Admin role
        var response = await _client.GetAsync($"/api/roles/{GetTenant2AdminRoleId()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Role Creation Tests

    [Fact]
    public async Task CreateRole_WithValidData_ReturnsCreatedRole()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateRoleDto
        {
            Name = "ContentManager",
            Description = "Manages content and reports",
            IsDefault = false,
            Permissions = new List<string> { "reports.view", "reports.create" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("ContentManager");
        result.Data.Description.Should().Be("Manages content and reports");
        result.Data.TenantId.Should().Be(1);
        result.Data.IsSystemRole.Should().BeFalse();
        result.Data.Permissions.Should().Contain("reports.view");
        result.Data.Permissions.Should().Contain("reports.create");

        // Verify Location header
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/roles/{result.Data.Id}");
    }

    [Fact]
    public async Task CreateRole_WithDuplicateName_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateRoleDto
        {
            Name = "Admin", // This already exists
            Description = "Duplicate admin role",
            Permissions = new List<string>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("already taken");
    }

    [Fact]
    public async Task CreateRole_WithUserToken_ReturnsForbidden()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateRoleDto
        {
            Name = "TestRole",
            Description = "Test role",
            Permissions = new List<string>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region Role Update Tests

    [Fact]
    public async Task UpdateRole_WithValidData_ReturnsUpdatedRole()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // First create a role to update
        var createRequest = new CreateRoleDto
        {
            Name = "TestRole",
            Description = "Original description",
            Permissions = new List<string> { "users.view" }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/roles", createRequest);
        var createdRole = await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();

        var updateRequest = new UpdateRoleDto
        {
            Name = "UpdatedTestRole",
            Description = "Updated description",
            Permissions = new List<string> { "users.view", "users.edit" }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/roles/{createdRole!.Data!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("UpdatedTestRole");
        result.Data.Description.Should().Be("Updated description");
        result.Data.Permissions.Should().Contain("users.view");
        result.Data.Permissions.Should().Contain("users.edit");
    }

    [Fact]
    public async Task UpdateRole_SystemRole_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var updateRequest = new UpdateRoleDto
        {
            Name = "ModifiedSuperAdmin",
            Description = "Modified system role",
            Permissions = new List<string>()
        };

        // Act - Try to update SuperAdmin role
        var response = await _client.PutAsJsonAsync($"/api/roles/{GetSystemSuperAdminRoleId()}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("System roles cannot be modified");
    }

    [Fact]
    public async Task UpdateRole_NonExistentRole_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var updateRequest = new UpdateRoleDto
        {
            Name = "NonExistent",
            Description = "Does not exist",
            Permissions = new List<string>()
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/roles/999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Role Deletion Tests

    [Fact]
    public async Task DeleteRole_WithValidRole_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // First create a role to delete
        var createRequest = new CreateRoleDto
        {
            Name = "DeletableRole",
            Description = "Role to be deleted",
            Permissions = new List<string>()
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

        // Verify role is deleted
        var getResponse = await _client.GetAsync($"/api/roles/{createdRole.Data.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRole_SystemRole_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Try to delete SuperAdmin role
        var response = await _client.DeleteAsync($"/api/roles/{GetSystemSuperAdminRoleId()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("System roles cannot be deleted");
    }

    [Fact]
    public async Task DeleteRole_RoleWithUsers_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Try to delete Admin role which has users assigned
        var response = await _client.DeleteAsync($"/api/roles/{GetTenant1AdminRoleId()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("has users assigned");
    }

    [Fact]
    public async Task DeleteRole_NonExistentRole_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/roles/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Role Permission Tests

    [Fact]
    public async Task GetRolePermissions_WithValidRole_ReturnsPermissions()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Get Admin role permissions
        var response = await _client.GetAsync($"/api/roles/{GetTenant1AdminRoleId()}/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<string>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data!.Should().Contain("users.view");
        result.Data.Should().Contain("users.edit");
        result.Data.Should().Contain("roles.view");
    }

    [Fact]
    public async Task UpdateRolePermissions_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // First create a role to update permissions for
        var createRequest = new CreateRoleDto
        {
            Name = "PermissionTestRole",
            Description = "For permission testing",
            Permissions = new List<string> { "users.view" }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/roles", createRequest);
        var createdRole = await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();

        var updateRequest = new RolePermissionUpdateDto
        {
            Permissions = new List<string> { "users.view", "users.edit", "reports.view" }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/roles/{createdRole!.Data!.Id}/permissions", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeTrue();

        // Verify permissions were updated
        var permissionsResponse = await _client.GetAsync($"/api/roles/{createdRole.Data.Id}/permissions");
        var permissions = await permissionsResponse.Content.ReadFromJsonAsync<ApiResponseDto<List<string>>>();
        permissions!.Data!.Should().Contain("users.view");
        permissions.Data.Should().Contain("users.edit");
        permissions.Data.Should().Contain("reports.view");
    }

    [Fact]
    public async Task UpdateRolePermissions_SystemRole_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var updateRequest = new RolePermissionUpdateDto
        {
            Permissions = new List<string> { "users.view" }
        };

        // Act - Try to update SuperAdmin role permissions
        var response = await _client.PutAsJsonAsync($"/api/roles/{GetSystemSuperAdminRoleId()}/permissions", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("System role permissions cannot be modified");
    }

    #endregion

    #region User Role Assignment Tests

    [Fact]
    public async Task AssignRoleToUser_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var assignRequest = new AssignRoleDto
        {
            UserId = GetTenant1UserUserId(), // Regular user
            RoleId = GetTenant1ManagerRoleId() // Manager role
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task AssignRoleToUser_NonExistentRole_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var assignRequest = new AssignRoleDto
        {
            UserId = GetTenant1UserUserId(),
            RoleId = 999 // Non-existent role
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles/assign", assignRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("Role not found");
    }

    [Fact]
    public async Task RemoveRoleFromUser_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Remove Manager role from manager user
        var response = await _client.DeleteAsync($"/api/roles/{GetTenant1ManagerRoleId()}/users/{GetTenant1ManagerUserId()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserRoles_WithValidUser_ReturnsRoles()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Get roles for admin user
        var response = await _client.GetAsync($"/api/roles/users/{GetTenant1AdminUserId()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleDto>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data!.Should().Contain(r => r.Name == "Admin");
    }

    [Fact]
    public async Task GetUserRoles_UserAccessingOwnRoles_ReturnsSuccess()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - User accessing their own roles
        var response = await _client.GetAsync($"/api/roles/users/{GetTenant1UserUserId()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleDto>>>();
        result!.Success.Should().BeTrue();
        result.Data!.Should().Contain(r => r.Name == "User");
    }

    [Fact]
    public async Task GetUserRoles_UserAccessingOtherUserRoles_ReturnsForbidden()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - User trying to access another user's roles
        var response = await _client.GetAsync($"/api/roles/users/{GetTenant1AdminUserId()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetRoleUsers_WithValidRole_ReturnsUsers()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Get users in Admin role
        var response = await _client.GetAsync($"/api/roles/{GetTenant1AdminRoleId()}/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<UserInfo>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        result.Data!.Should().Contain(u => u.Email == "admin@tenant1.com");
    }

    #endregion

    #region Tenant Isolation Tests

    [Fact]
    public async Task GetRoles_Tenant1Admin_OnlySeesOwnTenantRoles()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@tenant1.com", "Admin");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleDto>>>();
        var tenantRoles = result!.Data!.Where(r => r.TenantId.HasValue).ToList();
        tenantRoles.Should().OnlyContain(r => r.TenantId == 1);
    }

    [Fact]
    public async Task GetRoles_Tenant2Admin_OnlySeesOwnTenantRoles()
    {
        // Arrange
        var token = await GetTenant2AdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleDto>>>();
        var tenantRoles = result!.Data!.Where(r => r.TenantId.HasValue).ToList();
        tenantRoles.Should().OnlyContain(r => r.TenantId == 2);
    }

    [Fact]
    public async Task CreateRole_CreatedInCorrectTenant_ReturnsSuccess()
    {
        // Arrange
        var token = await GetTenant2AdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateRoleDto
        {
            Name = "Tenant2SpecificRole",
            Description = "Role specific to tenant 2",
            Permissions = new List<string> { "users.view" }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<RoleDto>>();
        result!.Data!.TenantId.Should().Be(2);
        result.Data.Name.Should().Be("Tenant2SpecificRole");
    }

    #endregion

    #region Authorization Tests

    [Theory]
    [InlineData("User")]
    [InlineData("Manager")]
    public async Task RoleManagement_NonAdminRoles_ReturnsForbidden(string role)
    {
        // Arrange
        var token = await GetAuthTokenAsync($"{role.ToLower()}@tenant1.com", role);
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act & Assert
        var getResponse = await _client.GetAsync("/api/roles");
        getResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var createResponse = await _client.PostAsJsonAsync("/api/roles", new CreateRoleDto { Name = "Test" });
        createResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("SuperAdmin")]
    [InlineData("TenantAdmin")]
    public async Task RoleManagement_AdminRoles_ReturnsSuccess(string role)
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@tenant1.com", role);
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task GetRoles_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateRole_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateRoleDto
        {
            Name = "", // Empty name
            Description = "Test role",
            Permissions = new List<string>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/roles", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetRoles_InvalidPageSize_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles?pageSize=101"); // Over limit

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleDto>>>();
        result!.Message.Should().Contain("Page size must be between 1 and 100");
    }

    #endregion
}
