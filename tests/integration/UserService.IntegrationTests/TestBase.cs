using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.Data;
using UserService.IntegrationTests.TestUtilities;
using UserService.IntegrationTests.Fixtures;
using Xunit;

namespace UserService.IntegrationTests;

/// <summary>
/// 🔧 CRITICAL FIX: All integration tests should inherit from this base class
/// and use the shared "Integration Tests" collection to prevent multiple application instances
/// </summary>
[Collection("Integration Tests")]
public abstract class TestBase : IClassFixture<WebApplicationTestFixture>, IAsyncLifetime
{
    protected readonly WebApplicationTestFixture _fixture;
    protected readonly HttpClient _client;
    protected readonly IServiceScope _scope;
    protected readonly ApplicationDbContext _dbContext;
    protected readonly ILogger<TestBase> _logger;

    protected TestBase(WebApplicationTestFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient();
        _scope = _fixture.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _logger = _scope.ServiceProvider.GetRequiredService<ILogger<TestBase>>();
    }

    public virtual async Task InitializeAsync()
    {
        try 
        {
            // ✅ CRITICAL FIX: Clear any potential rate limiting cache before test
            await RateLimitingTestUtilities.ClearRateLimitCacheAsync(_fixture.Services);
            
            // ✅ FIX: Initialize database with clean seeding
            await InitializeDatabaseAsync();
            
            // Verify data was seeded correctly with detailed logging
            await LogTestDataStatus();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize test database");
            throw;
        }
    }

    public virtual async Task DisposeAsync()
    {
        _scope.Dispose();
        await Task.CompletedTask;
    }

    private async Task InitializeDatabaseAsync()
    {
        // ✅ CRITICAL FIX: Let TestDataSeeder handle complete database recreation
        await TestDataSeeder.SeedTestDataAsync(_dbContext);
    }

    private async Task LogTestDataStatus()
    {
        try
        {
            var tenantCount = await _dbContext.Tenants.CountAsync();
            var userCount = await _dbContext.Users.CountAsync();
            var roleCount = await _dbContext.Roles.CountAsync();
            var permissionCount = await _dbContext.Permissions.CountAsync();
            var userRoleCount = await _dbContext.UserRoles.CountAsync();
            var totalRolePermissionCount = await _dbContext.RolePermissions.CountAsync();

            _logger.LogInformation("📊 Test Data Status: Tenants={TenantCount}, Users={UserCount}, Roles={RoleCount}, Permissions={PermissionCount}, UserRoles={UserRoleCount}, RolePermissions={RolePermissionCount}",
                tenantCount, userCount, roleCount, permissionCount, userRoleCount, totalRolePermissionCount);

            // ✅ CHECK ACTUAL TENANT AND USER IDs
            var tenant1 = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == "Test Tenant 1");
            var tenant2 = await _dbContext.Tenants.FirstOrDefaultAsync(t => t.Name == "Test Tenant 2");
            var adminUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "admin@tenant1.com");
            var regularUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "user@tenant1.com");

            _logger.LogInformation("🏢 Tenant IDs: Tenant1={Tenant1Id}, Tenant2={Tenant2Id}", 
                tenant1?.Id ?? 0, tenant2?.Id ?? 0);
            _logger.LogInformation("👤 User IDs: Admin={AdminId}, User={UserId}", 
                adminUser?.Id ?? 0, regularUser?.Id ?? 0);

            // Log detailed admin user analysis for debugging
            if (adminUser != null)
            {
                var adminUserWithRoles = await _dbContext.Users
                    .Include(u => u.UserRoles.Where(ur => ur.IsActive))
                        .ThenInclude(ur => ur.Role)
                            .ThenInclude(r => r.RolePermissions)
                                .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(u => u.Id == adminUser.Id);

                var adminPermissions = adminUserWithRoles?.UserRoles
                    .Where(ur => ur.IsActive)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList() ?? new List<string>();
                
                _logger.LogInformation("🔑 Admin user analysis:");
                _logger.LogInformation("   - Email: {Email}", adminUserWithRoles?.Email);
                _logger.LogInformation("   - UserRoles count: {UserRoleCount}", adminUserWithRoles?.UserRoles.Count ?? 0);
                _logger.LogInformation("   - Active UserRoles: {ActiveUserRoleCount}", adminUserWithRoles?.UserRoles.Count(ur => ur.IsActive) ?? 0);
                _logger.LogInformation("   - Permissions: {PermissionCount} [{Permissions}]", adminPermissions.Count, string.Join(", ", adminPermissions));

                // Instead, get tenant from TenantUsers:
                var userTenant = await _dbContext.TenantUsers
                    .Include(tu => tu.Tenant)
                    .Where(tu => tu.UserId == adminUser.Id && tu.IsActive)
                    .FirstOrDefaultAsync();

                _logger.LogInformation("   - Tenant: {TenantName}", userTenant?.Tenant?.Name ?? "NULL");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not log test data status");
        }
    }

    #region Enhanced Authentication Helpers - Two-Phase Compatible

    /// <summary>
    /// 🔧 TWO-PHASE FIX: Get authentication token with proper two-phase simulation
    /// </summary>
    protected async Task<string> GetAuthTokenAsync(string email = "admin@tenant1.com", int? tenantId = null)
    {
        // Use the enhanced two-phase aware authentication helper
        return await AuthenticationHelper.GetTenantAwareJwtAsync(_client, _dbContext, email, tenantId ?? 1);
    }

    /// <summary>
    /// Gets a user token for the user's default tenant
    /// </summary>
    protected async Task<string> GetUserTokenAsync(string email = "user@tenant1.com")
    {
        // Determine tenant from email
        int tenantId = email.Contains("tenant2") ? 2 : 1;
        return await GetAuthTokenAsync(email, tenantId);
    }

    protected async Task<string> GetManagerTokenAsync(string email = "manager@tenant1.com")
    {
        return await AuthenticationHelper.GetTenantAwareJwtAsync(_client, _dbContext, email, 1);
    }

    protected async Task<string> GetTenant2AdminTokenAsync(string email = "admin@tenant2.com")
    {
        return await AuthenticationHelper.GetTenantAwareJwtAsync(_client, _dbContext, email, 2);
    }

    protected async Task<string> GetTenant2UserTokenAsync(string email = "user@tenant2.com")
    {
        return await AuthenticationHelper.GetTenantAwareJwtAsync(_client, _dbContext, email, 2);
    }

    /// <summary>
    /// 🔧 TWO-PHASE FIX: Test Phase 1 JWT generation (no tenant)
    /// </summary>
    protected async Task<string> GetPhase1TokenAsync(string email = "admin@tenant1.com")
    {
        return await AuthenticationHelper.GetInitialJwtAsync(_dbContext, email);
    }

    /// <summary>
    /// 🔧 TWO-PHASE FIX: Test the complete two-phase flow
    /// </summary>
    protected async Task<string> GetTwoPhaseTokenAsync(string email = "admin@tenant1.com", int tenantId = 1)
    {
        return await AuthenticationHelper.GetTenantTokenViaTwoPhaseFlow(_client, _dbContext, email, tenantId);
    }

    #endregion

    #region Test Data Verification Helpers - RBAC Version

    protected async Task<List<string>> GetUserPermissionsAsync(string email)
    {
        // ✅ RBAC FIX: Use same pattern as AuthenticationHelper to avoid EF Include filtering issues
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null) return new List<string>();

        // ✅ RBAC FIX: Load UserRoles separately to avoid Include filtering
        var userRoles = await _dbContext.UserRoles
            .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .Where(ur => ur.UserId == user.Id && ur.IsActive)
            .ToListAsync();

        return userRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();
    }

    #endregion

    #region Rate Limiting Protection for Tests

    /// <summary>
    /// Add a small delay between API calls to prevent any potential rate limiting
    /// </summary>
    protected async Task DelayForRateLimitAsync(int milliseconds = 50)
    {
        await RateLimitingTestUtilities.DelayForRateLimitingAsync(milliseconds);
    }

    #endregion
}
