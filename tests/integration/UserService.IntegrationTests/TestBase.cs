using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Common.Data;
using UserService.IntegrationTests.TestUtilities;
using UserService.IntegrationTests.Fixtures;
using Xunit;

namespace UserService.IntegrationTests;

public abstract class TestBase : IClassFixture<WebApplicationTestFixture>, IAsyncLifetime
{
    protected readonly WebApplicationTestFixture _fixture;
    protected readonly HttpClient _client;
    protected readonly IServiceScope _scope;
    protected readonly ApplicationDbContext _dbContext;

    protected TestBase(WebApplicationTestFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient();
        _scope = _fixture.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public virtual async Task InitializeAsync()
    {
        // Ensure database is created and clean
        await _dbContext.Database.EnsureCreatedAsync();
        await CleanDatabase();
        
        // Seed test data
        await TestDataSeeder.SeedTestDataAsync(_dbContext);
    }

    public virtual async Task DisposeAsync()
    {
        await CleanDatabase();
        _scope.Dispose();
    }

    protected virtual async Task CleanDatabase()
    {
        // Clean in reverse order of dependencies (ENHANCED with RBAC entities)
        _dbContext.UserRoles.RemoveRange(_dbContext.UserRoles);
        _dbContext.RolePermissions.RemoveRange(_dbContext.RolePermissions);
        _dbContext.RefreshTokens.RemoveRange(_dbContext.RefreshTokens);
        _dbContext.TenantUsers.RemoveRange(_dbContext.TenantUsers);
        _dbContext.Users.RemoveRange(_dbContext.Users);
        _dbContext.Roles.RemoveRange(_dbContext.Roles);
        _dbContext.Permissions.RemoveRange(_dbContext.Permissions);
        _dbContext.Tenants.RemoveRange(_dbContext.Tenants);
        
        await _dbContext.SaveChangesAsync();
    }

    #region Enhanced Authentication Helpers

    protected async Task<string> GetAuthTokenAsync(string email = "admin@tenant1.com", string role = "Admin")
    {
        return await AuthenticationHelper.GetValidTokenAsync(_client, _dbContext, email, role);
    }

    protected async Task<string> GetUserTokenAsync(string email = "user@tenant1.com")
    {
        return await AuthenticationHelper.GetValidTokenAsync(_client, _dbContext, email, "User");
    }

    protected async Task<string> GetManagerTokenAsync(string email = "manager@tenant1.com")
    {
        return await AuthenticationHelper.GetValidTokenAsync(_client, _dbContext, email, "Manager");
    }

    protected async Task<string> GetTenant2AdminTokenAsync(string email = "admin@tenant2.com")
    {
        return await AuthenticationHelper.GetValidTokenAsync(_client, _dbContext, email, "Admin");
    }

    protected async Task<string> GetTenant2UserTokenAsync(string email = "user@tenant2.com")
    {
        return await AuthenticationHelper.GetValidTokenAsync(_client, _dbContext, email, "User");
    }

    #endregion

    #region RBAC Test Data Helpers

    protected int GetTenant1AdminRoleId() => 2;
    protected int GetTenant1UserRoleId() => 3;
    protected int GetTenant1ManagerRoleId() => 4;
    protected int GetTenant2AdminRoleId() => 5;
    protected int GetTenant2UserRoleId() => 6;
    protected int GetSystemSuperAdminRoleId() => 1;

    protected int GetTenant1AdminUserId() => 1;
    protected int GetTenant1UserUserId() => 2;
    protected int GetTenant1ManagerUserId() => 3;
    protected int GetTenant2AdminUserId() => 4;
    protected int GetTenant2UserUserId() => 5;

    #endregion
}
