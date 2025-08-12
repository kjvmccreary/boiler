using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using DTOs.Common;
using DTOs.Auth;
using DTOs.User; // ✅ ADD: Missing using statement for AssignUserRoleDto
using UserService.IntegrationTests.Fixtures;
using Xunit;
using Microsoft.Extensions.Logging; // ✅ FIX: Add missing using directive
using System.Text.Json;

namespace UserService.IntegrationTests.Controllers;

[Collection("RolesController")] // ✅ FIX: Isolated database
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
        
        // ✅ FIX: Read content only once and remove the try-catch
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

        // ✅ FIX: Create a dedicated test role instead of modifying existing Admin role
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

        // ✅ FIX: Create a dedicated test role for deletion
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
            RoleId = 1
        };

        // Act - Use User ID 3 (manager) instead of User ID 2 (regular user)
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

        // Act
        var response = await _client.DeleteAsync("/api/roles/1/users/2");

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

        // Act
        var response = await _client.GetAsync("/api/roles/1/permissions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<string>>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateRolePermissions_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var permissions = new List<string> { "users.view", "users.edit", "roles.view" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/roles/1/permissions", permissions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task GetRoles_WithUserToken_ReturnsSuccess() // ✅ CHANGED: User CAN view roles
    {
        // Arrange - User role HAS roles.view permission in our RBAC model
        var token = await GetUserTokenAsync("user@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/roles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK); // ✅ CHANGED: User can view roles
        
        // ✅ FIX: Remove the try-catch and just expect PagedResult
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

    #endregion
}
