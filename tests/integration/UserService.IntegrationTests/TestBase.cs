using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Common.Data;
using UserService.IntegrationTests.TestUtilities;
using UserService.IntegrationTests.Fixtures;
using Xunit; // ADDED: For IClassFixture and IAsyncLifetime

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
        // Clean in reverse order of dependencies
        _dbContext.RefreshTokens.RemoveRange(_dbContext.RefreshTokens);
        _dbContext.TenantUsers.RemoveRange(_dbContext.TenantUsers);
        _dbContext.Users.RemoveRange(_dbContext.Users);
        _dbContext.Tenants.RemoveRange(_dbContext.Tenants);
        
        await _dbContext.SaveChangesAsync();
    }

    protected async Task<string> GetAuthTokenAsync(string email = "admin@tenant1.com", string role = "SuperAdmin")
    {
        return await AuthenticationHelper.GetValidTokenAsync(_client, _dbContext, email, role);
    }

    protected async Task<string> GetUserTokenAsync(string email = "user@tenant1.com")
    {
        return await AuthenticationHelper.GetValidTokenAsync(_client, _dbContext, email, "User");
    }

    protected async Task<string> GetTenant2AdminTokenAsync(string email = "admin@tenant2.com")
    {
        return await AuthenticationHelper.GetValidTokenAsync(_client, _dbContext, email, "Admin");
    }
}
