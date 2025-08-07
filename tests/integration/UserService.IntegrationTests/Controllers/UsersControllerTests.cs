using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using DTOs.Common;
using DTOs.User;
using UserService.IntegrationTests.Fixtures;
using Xunit;

namespace UserService.IntegrationTests.Controllers;

public class UsersControllerTests : TestBase
{
    public UsersControllerTests(WebApplicationTestFixture fixture) : base(fixture) { }

    #region Profile Management Tests

    [Fact]
    public async Task GetUserProfile_WithValidToken_ReturnsUserProfile()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("admin@tenant1.com");
        result.Data.TenantId.Should().Be(1);
    }

    [Fact]
    public async Task GetUserProfile_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateUserProfile_WithValidData_ReturnsUpdatedProfile()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var updateRequest = new UserUpdateDto
        {
            FirstName = "Updated",
            LastName = "Name",
            PhoneNumber = "+1-555-0123",
            TimeZone = "America/New_York",
            Language = "en"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/profile", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.FirstName.Should().Be("Updated");
        result.Data.LastName.Should().Be("Name");
        result.Data.PhoneNumber.Should().Be("+1-555-0123");
    }

    [Fact]
    public async Task UpdateUserProfile_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var updateRequest = new UserUpdateDto
        {
            FirstName = "", // Invalid - required field
            LastName = "Name"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/profile", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Admin User List Tests

    [Fact]
    public async Task GetUsers_WithAdminToken_ReturnsUserList()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@tenant1.com", "SuperAdmin");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<UserDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(3); // Only tenant 1 users
        result.Data.Items.Should().OnlyContain(u => u.TenantId == 1);
    }

    [Fact]
    public async Task GetUsers_WithUserToken_ReturnsForbidden()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUsers_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users?page=1&pageSize=2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<UserDto>>>();
        result!.Data!.Items.Should().HaveCount(2);
        result.Data.PageNumber.Should().Be(1); // FIXED: Use PageNumber instead of Page
        result.Data.PageSize.Should().Be(2);
        result.Data.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task GetUsers_WithSearchTerm_ReturnsFilteredResults()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users?searchTerm=Regular");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<UserDto>>>();
        result!.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().FirstName.Should().Be("Regular");
    }

    #endregion

    #region Individual User Access Tests

    [Fact]
    public async Task GetUser_AdminAccessingAnyUser_ReturnsUser()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Admin accessing regular user
        var response = await _client.GetAsync("/api/users/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();
        result!.Data!.Id.Should().Be(2);
        result.Data.Email.Should().Be("user@tenant1.com");
    }

    [Fact]
    public async Task GetUser_UserAccessingOwnProfile_ReturnsUser()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - User accessing their own profile
        var response = await _client.GetAsync("/api/users/2");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<UserDto>>();
        result!.Data!.Id.Should().Be(2);
        result.Data.Email.Should().Be("user@tenant1.com");
    }

    [Fact]
    public async Task GetUser_UserAccessingOtherUser_ReturnsForbidden()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - User trying to access another user
        var response = await _client.GetAsync("/api/users/3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetUser_NonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Tenant Isolation Tests

    [Fact]
    public async Task GetUsers_Tenant1Admin_OnlySeesOwnTenantUsers()
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@tenant1.com", "SuperAdmin");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<UserDto>>>();
        result!.Data!.Items.Should().OnlyContain(u => u.TenantId == 1);
        result.Data.Items.Should().HaveCount(3); // Only tenant 1 users
    }

    [Fact]
    public async Task GetUsers_Tenant2Admin_OnlySeesOwnTenantUsers()
    {
        // Arrange
        var token = await GetTenant2AdminTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<UserDto>>>();
        result!.Data!.Items.Should().OnlyContain(u => u.TenantId == 2);
        result.Data.Items.Should().HaveCount(2); // Only tenant 2 users
    }

    [Fact]
    public async Task GetUser_CrossTenantAccess_ReturnsNotFound()
    {
        // Arrange - Tenant 1 admin trying to access Tenant 2 user
        var token = await GetAuthTokenAsync("admin@tenant1.com", "SuperAdmin");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Try to access user from tenant 2
        var response = await _client.GetAsync("/api/users/4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region User Deletion Tests

    [Fact]
    public async Task DeleteUser_AdminDeletingOtherUser_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/users/3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeTrue();

        // Verify user is soft deleted
        var userResponse = await _client.GetAsync("/api/users/3");
        userResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_AdminDeletingSelf_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act - Admin trying to delete themselves
        var response = await _client.DeleteAsync("/api/users/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("cannot delete your own account"); // FIXED: Use Message instead of ErrorMessage
    }

    [Fact]
    public async Task DeleteUser_UserTryingToDelete_ReturnsForbidden()
    {
        // Arrange
        var token = await GetUserTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/users/3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteUser_NonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/users/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_CrossTenantAttempt_ReturnsNotFound()
    {
        // Arrange - Tenant 1 admin trying to delete Tenant 2 user
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/users/4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task GetUsers_InvalidPage_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users?page=0&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetUsers_InvalidPageSize_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users?page=1&pageSize=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Authorization Policy Tests

    [Theory]
    [InlineData("User")]
    [InlineData("Manager")]
    [InlineData("Employee")]
    public async Task GetUsers_NonAdminRoles_ReturnsForbidden(string role)
    {
        // Arrange
        var token = await GetAuthTokenAsync("user@tenant1.com", role);
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("SuperAdmin")]
    public async Task GetUsers_AdminRoles_ReturnsSuccess(string role)
    {
        // Arrange
        var token = await GetAuthTokenAsync("admin@tenant1.com", role);
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion
}
