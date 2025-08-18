using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using DTOs.Common;
using DTOs.Tenant;
using UserService.IntegrationTests.Fixtures;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace UserService.IntegrationTests.Controllers;

[Collection("TenantManagement")]
public class TenantsControllerTests : TestBase
{
    public TenantsControllerTests(WebApplicationTestFixture fixture) : base(fixture) { }

    [Fact]
    public async Task CreateTenant_WithValidData_ShouldCreateTenantAndAdminUser()
    {
        // Arrange - ✅ FIXED: Use GetAuthTokenAsync instead of GetSuperAdminTokenAsync
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateTenantDto
        {
            Name = "Test Company",
            Domain = "testcompany.com",
            SubscriptionPlan = "Premium",
            Settings = new Dictionary<string, object> { { "feature1", true } },
            AdminUser = new CreateTenantAdminDto
            {
                Email = "admin@testcompany.com",
                FirstName = "John",
                LastName = "Admin",
                Password = "SecurePassword123!"
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tenants", createRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<TenantDto>>();
        
        result!.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Test Company");
        result.Data.Domain.Should().Be("testcompany.com");
        result.Data.IsActive.Should().BeTrue();

        // Verify admin user was created
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == "admin@testcompany.com");
        user.Should().NotBeNull();
        user!.TenantId.Should().Be(result.Data.Id);

        // Verify TenantUser relationship was created
        var tenantUser = await _dbContext.TenantUsers
            .FirstOrDefaultAsync(tu => tu.UserId == user.Id && tu.TenantId == result.Data.Id);
        tenantUser.Should().NotBeNull();
        tenantUser!.Role.Should().Be("TenantAdmin");

        // Verify default roles were created
        var rolesCount = await _dbContext.Roles
            .CountAsync(r => r.TenantId == result.Data.Id);
        rolesCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateTenant_WithDuplicateDomain_ShouldReturnError()
    {
        // Arrange - ✅ FIXED: Use GetAuthTokenAsync
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // First tenant
        var firstRequest = new CreateTenantDto
        {
            Name = "First Company",
            Domain = "duplicate.com",
            SubscriptionPlan = "Basic",
            AdminUser = new CreateTenantAdminDto
            {
                Email = "admin1@duplicate.com",
                FirstName = "First",
                LastName = "Admin",
                Password = "Password123!"
            }
        };

        await _client.PostAsJsonAsync("/api/tenants", firstRequest);

        // Second tenant with same domain
        var secondRequest = new CreateTenantDto
        {
            Name = "Second Company",
            Domain = "duplicate.com", // Same domain
            SubscriptionPlan = "Basic",
            AdminUser = new CreateTenantAdminDto
            {
                Email = "admin2@duplicate.com",
                FirstName = "Second",
                LastName = "Admin",
                Password = "Password123!"
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tenants", secondRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<TenantDto>>();
        
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("already in use");
    }

    [Fact]
    public async Task GetTenants_WithPagination_ShouldReturnPagedResults()
    {
        // Arrange - ✅ FIXED: Use GetAuthTokenAsync
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Create multiple tenants
        for (int i = 1; i <= 5; i++)
        {
            var createRequest = new CreateTenantDto
            {
                Name = $"Test Company {i}",
                Domain = $"testcompany{i}.com",
                SubscriptionPlan = "Basic",
                AdminUser = new CreateTenantAdminDto
                {
                    Email = $"admin{i}@testcompany{i}.com",
                    FirstName = "Admin",
                    LastName = $"User{i}",
                    Password = "Password123!"
                }
            };

            await _client.PostAsJsonAsync("/api/tenants", createRequest);
        }

        // Act
        var response = await _client.GetAsync("/api/tenants?page=1&pageSize=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<PagedResultDto<TenantDto>>>();
        
        result!.Success.Should().BeTrue();
        result.Data!.Items.Should().HaveCountLessOrEqualTo(3);
        // ✅ FIXED: Use PageNumber instead of Page
        result.Data.PageNumber.Should().Be(1);
        result.Data.PageSize.Should().Be(3);
        result.Data.TotalCount.Should().BeGreaterOrEqualTo(5);
    }

    [Fact]
    public async Task UpdateTenant_WithValidData_ShouldUpdateTenant()
    {
        // Arrange - ✅ FIXED: Use GetAuthTokenAsync
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Create tenant first
        var createRequest = new CreateTenantDto
        {
            Name = "Original Company",
            Domain = "original.com",
            SubscriptionPlan = "Basic",
            AdminUser = new CreateTenantAdminDto
            {
                Email = "admin@original.com",
                FirstName = "Admin",
                LastName = "User",
                Password = "Password123!"
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tenants", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<TenantDto>>();
        var tenantId = createResult!.Data!.Id;

        // Update request
        var updateRequest = new UpdateTenantDto
        {
            Name = "Updated Company",
            Domain = "updated.com",
            SubscriptionPlan = "Premium",
            IsActive = true,
            Settings = new Dictionary<string, object> { { "newFeature", true } }
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tenants/{tenantId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<TenantDto>>();
        
        result!.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Updated Company");
        result.Data.Domain.Should().Be("updated.com");
        result.Data.SubscriptionPlan.Should().Be("Premium");
    }

    [Fact]
    public async Task DeleteTenant_WithUsers_ShouldReturnError()
    {
        // Arrange - ✅ FIXED: Use GetAuthTokenAsync
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Create tenant (which creates admin user)
        var createRequest = new CreateTenantDto
        {
            Name = "Company With Users",
            Domain = "withusers.com",
            SubscriptionPlan = "Basic",
            AdminUser = new CreateTenantAdminDto
            {
                Email = "admin@withusers.com",
                FirstName = "Admin",
                LastName = "User",
                Password = "Password123!"
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tenants", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<TenantDto>>();
        var tenantId = createResult!.Data!.Id;

        // Act - Try to delete tenant with users
        var response = await _client.DeleteAsync($"/api/tenants/{tenantId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<bool>>();
        
        result!.Success.Should().BeFalse();
        result.Message.Should().Contain("Cannot delete tenant with");
    }

    [Fact]
    public async Task InitializeTenant_ShouldCreateDefaultRoles()
    {
        // Arrange - ✅ FIXED: Use GetAuthTokenAsync
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Create tenant
        var createRequest = new CreateTenantDto
        {
            Name = "Initialize Test Company",
            Domain = "initialize.com",
            SubscriptionPlan = "Basic",
            AdminUser = new CreateTenantAdminDto
            {
                Email = "admin@initialize.com",
                FirstName = "Admin",
                LastName = "User",
                Password = "Password123!"
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/tenants", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseDto<TenantDto>>();
        var tenantId = createResult!.Data!.Id;

        // Clear existing roles for clean test
        var existingRoles = _dbContext.Roles.Where(r => r.TenantId == tenantId);
        _dbContext.Roles.RemoveRange(existingRoles);
        await _dbContext.SaveChangesAsync();

        var initRequest = new TenantInitializationDto
        {
            TenantId = tenantId,
            CreateDefaultRoles = true,
            RoleTemplates = new List<string> { "Manager", "User" }
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/tenants/{tenantId}/initialize", initRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify roles were created
        var roles = await _dbContext.Roles
            .Where(r => r.TenantId == tenantId)
            .ToListAsync();
        
        roles.Should().NotBeEmpty();
        roles.Should().Contain(r => r.Name == "Tenant Admin");
        roles.Should().Contain(r => r.Name == "Manager");
        roles.Should().Contain(r => r.Name == "User");
    }

    [Fact]
    public async Task GetRoleTemplates_ShouldReturnAvailableTemplates()
    {
        // Arrange - ✅ FIXED: Use GetAuthTokenAsync
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/tenants/role-templates");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<List<RoleTemplateDto>>>();
        
        result!.Success.Should().BeTrue();
        result.Data!.Should().NotBeEmpty();
        result.Data.Should().Contain(t => t.Name == "Tenant Admin");
        result.Data.Should().Contain(t => t.Name == "Manager");
        result.Data.Should().Contain(t => t.Name == "User");
    }

    [Theory]
    [InlineData("tenants.create")]
    [InlineData("tenants.view")]
    [InlineData("tenants.edit")]
    [InlineData("tenants.delete")]
    public async Task TenantEndpoints_WithoutPermission_ShouldReturn403(string requiredPermission)
    {
        // Arrange - Use user token without admin permissions
        var token = await GetUserTokenAsync("user@tenant1.com");
        _client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        var createRequest = new CreateTenantDto
        {
            Name = "Unauthorized Test",
            Domain = "unauthorized.com",
            SubscriptionPlan = "Basic",
            AdminUser = new CreateTenantAdminDto
            {
                Email = "admin@unauthorized.com",
                FirstName = "Admin",
                LastName = "User",
                Password = "Password123!"
            }
        };

        HttpResponseMessage response = requiredPermission switch
        {
            "tenants.create" => await _client.PostAsJsonAsync("/api/tenants", createRequest),
            "tenants.view" => await _client.GetAsync("/api/tenants"),
            "tenants.edit" => await _client.PutAsJsonAsync("/api/tenants/1", new UpdateTenantDto 
            { 
                Name = "Test", 
                SubscriptionPlan = "Basic", 
                IsActive = true 
            }),
            "tenants.delete" => await _client.DeleteAsync("/api/tenants/1"),
            _ => throw new ArgumentException($"Unknown permission: {requiredPermission}")
        };

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
