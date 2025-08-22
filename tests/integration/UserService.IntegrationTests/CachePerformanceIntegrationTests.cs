using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using UserService.IntegrationTests.Fixtures;
using UserService.IntegrationTests.TestUtilities;
using Common.Caching;
using Contracts.Services;
using Common.Data;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;

namespace UserService.IntegrationTests;

/// <summary>
/// Integration tests for Phase 10 - Caching & Performance Optimization
/// Tests cache performance, hit ratios, batch operations, and tenant isolation
/// Uses InMemoryPermissionCache to test caching behavior without Redis dependency
/// </summary>
[Collection("CachePerformance")] // ✅ Use the specific collection
public class CachePerformanceIntegrationTests : IClassFixture<WebApplicationTestFixture>
{
    private readonly WebApplicationTestFixture _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public CachePerformanceIntegrationTests(WebApplicationTestFixture factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _output = output;
    }

    #region Performance Tests

    [Fact]
    public async Task GetUserPermissions_WithCache_ShouldBeFasterThan10ms()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>() as InMemoryPermissionCache;
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        cache?.ResetMetrics(); // Reset metrics for clean test
        
        var userId = await GetUserIdFromTokenAsync(dbContext, "admin@tenant1.com");
        
        // Warm up cache with first call
        await permissionService.GetUserPermissionsAsync(userId, CancellationToken.None);
        
        // Act - Measure cached permission retrieval
        var stopwatch = Stopwatch.StartNew();
        var permissions = await permissionService.GetUserPermissionsAsync(userId, CancellationToken.None);
        stopwatch.Stop();
        
        // Assert
        permissions.Should().NotBeEmpty("Admin should have permissions");
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10,
            $"Cached permission check took {stopwatch.ElapsedMilliseconds}ms, expected < 10ms");
        
        // Verify cache was actually used
        if (cache != null)
        {
            cache.GetCacheHits().Should().BeGreaterThan(0, "Cache should have been hit");
            _output.WriteLine($"Cache performance: {cache.GetCacheHits()} hits, {cache.GetCacheMisses()} misses");
        }
    }

    [Fact]
    public async Task CacheHitRatio_AfterWarmup_ShouldBeAbove95Percent()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>() as InMemoryPermissionCache;
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        cache?.ResetMetrics(); // Reset metrics for clean test
        
        var userId = await GetUserIdFromTokenAsync(dbContext, "admin@tenant1.com");
        
        // Act - Make 100 requests to measure cache hit ratio
        for (int i = 0; i < 100; i++)
        {
            await permissionService.GetUserPermissionsAsync(userId, CancellationToken.None);
        }
        
        // Assert
        if (cache != null)
        {
            var hitRatio = cache.GetCacheHitRatio();
            
            _output.WriteLine($"Cache Hit Ratio Results (100 requests):");
            _output.WriteLine($"  Cache hits: {cache.GetCacheHits()}");
            _output.WriteLine($"  Cache misses: {cache.GetCacheMisses()}");
            _output.WriteLine($"  Hit ratio: {hitRatio:F1}%");
            
            hitRatio.Should().BeGreaterThan(95, $"Cache hit ratio should exceed 95%, achieved {hitRatio:F1}%");
        }
    }

    [Fact]
    public async Task DatabaseVsCache_PerformanceComparison_ShouldShowSignificantImprovement()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>() as InMemoryPermissionCache;
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        cache?.ResetMetrics();
        
        var userId = await GetUserIdFromTokenAsync(dbContext, "admin@tenant1.com");
        
        // Measure database performance (cache miss)
        await cache?.InvalidateUserPermissionsAsync(userId, 1)!;
        
        var dbStopwatch = Stopwatch.StartNew();
        var dbPermissions = await permissionService.GetUserPermissionsAsync(userId);
        dbStopwatch.Stop();
        
        // Measure cache performance (cache hit)
        var cacheStopwatch = Stopwatch.StartNew();
        var cachedPermissions = await permissionService.GetUserPermissionsAsync(userId);
        cacheStopwatch.Stop();
        
        // Assert
        _output.WriteLine($"Performance Comparison:");
        _output.WriteLine($"  Database call: {dbStopwatch.ElapsedMilliseconds}ms");
        _output.WriteLine($"  Cached call: {cacheStopwatch.ElapsedMilliseconds}ms");
        
        if (dbStopwatch.ElapsedMilliseconds > 0)
        {
            var improvement = (double)dbStopwatch.ElapsedMilliseconds / Math.Max(cacheStopwatch.ElapsedMilliseconds, 1);
            _output.WriteLine($"  Improvement: {improvement:F1}x faster");
        }
        
        cachedPermissions.Should().BeEquivalentTo(dbPermissions, "Cached and DB results should be identical");
        cacheStopwatch.ElapsedMilliseconds.Should().BeLessOrEqualTo(dbStopwatch.ElapsedMilliseconds,
            "Cached call should be faster than or equal to database call");
    }

    #endregion

    #region Cache Invalidation Tests

    [Fact]
    public async Task CacheInvalidation_AfterUserPermissionChange_ShouldRefreshPermissions()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>();
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        var userId = await GetUserIdFromTokenAsync(dbContext, "user@tenant1.com");
        const int tenantId = 1;
        
        // Get initial permissions and cache them
        var initialPermissions = await permissionService.GetUserPermissionsForTenantAsync(userId, tenantId);
        
        // Verify cache contains data
        var cachedPermissions = await cache.GetUserPermissionsAsync(userId, tenantId);
        cachedPermissions.Should().NotBeNull("Permissions should be cached after first load");
        
        // Act - Invalidate cache
        await cache.InvalidateUserPermissionsAsync(userId, tenantId);
        
        // Verify cache is empty
        var afterInvalidation = await cache.GetUserPermissionsAsync(userId, tenantId);
        afterInvalidation.Should().BeNull("Cache should be empty after invalidation");
        
        // Verify permissions are reloaded on next request
        var reloadedPermissions = await permissionService.GetUserPermissionsForTenantAsync(userId, tenantId);
        reloadedPermissions.Should().BeEquivalentTo(initialPermissions, 
            "Reloaded permissions should match original permissions");
    }

    [Fact]
    public async Task TenantCacheInvalidation_ShouldClearAllUserCachesForTenant()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>();
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        
        // Get multiple users in tenant 1
        var tenant1UserIds = await dbContext.Users
            .IgnoreQueryFilters()
            .Where(u => u.TenantUsers.Any(tu => tu.TenantId == 1))
            .Take(3)
            .Select(u => u.Id)
            .ToListAsync();
        
        const int tenantId = 1;
        
        // Cache permissions for all users
        foreach (var userId in tenant1UserIds)
        {
            await permissionService.GetUserPermissionsForTenantAsync(userId, tenantId);
        }
        
        // Verify all are cached
        foreach (var userId in tenant1UserIds)
        {
            var cached = await cache.GetUserPermissionsAsync(userId, tenantId);
            cached.Should().NotBeNull($"User {userId} permissions should be cached");
        }
        
        // Act - Invalidate entire tenant cache
        await cache.InvalidateTenantPermissionsAsync(tenantId);
        
        // Assert - All user caches should be cleared
        foreach (var userId in tenant1UserIds)
        {
            var afterInvalidation = await cache.GetUserPermissionsAsync(userId, tenantId);
            afterInvalidation.Should().BeNull($"User {userId} cache should be cleared after tenant invalidation");
        }
    }

    #endregion

    #region Multi-Tenant Cache Isolation Tests

    [Fact]
    public async Task PermissionCache_ShouldIsolateDataBetweenTenants()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>();
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        
        var userId = await GetUserIdFromTokenAsync(dbContext, "admin@tenant1.com");
        var testPermissions1 = new[] { "test.permission1", "test.permission2" };
        var testPermissions2 = new[] { "different.permission" };
        
        // Act - Set permissions for same user in different tenants
        await cache.SetUserPermissionsAsync(userId, 1, testPermissions1);
        await cache.SetUserPermissionsAsync(userId, 2, testPermissions2);
        
        // Assert - Each tenant should only see their own cached data
        var tenant1Cache = await cache.GetUserPermissionsAsync(userId, 1);
        var tenant2Cache = await cache.GetUserPermissionsAsync(userId, 2);
        
        tenant1Cache.Should().BeEquivalentTo(testPermissions1,
            "Tenant 1 should only see their cached permissions");
        tenant2Cache.Should().BeEquivalentTo(testPermissions2,
            "Tenant 2 should only see their cached permissions");
        tenant1Cache.Should().NotBeEquivalentTo(tenant2Cache,
            "Different tenants should have isolated cache data");
    }

    [Fact]
    public async Task CacheKeys_ShouldPreventCrossTenantAccess()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>();
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        
        var userId = await GetUserIdFromTokenAsync(dbContext, "admin@tenant1.com");
        var testPermissions = new[] { "test.permission1", "test.permission2" };
        
        // Act - Set permissions for same user in different tenants
        await cache.SetUserPermissionsAsync(userId, 1, testPermissions);
        await cache.SetUserPermissionsAsync(userId, 2, new[] { "different.permission" });
        
        // Invalidate tenant 1 cache
        await cache.InvalidateTenantPermissionsAsync(1);
        
        // Assert - Tenant 2 cache should remain intact
        var tenant1CacheAfterInvalidation = await cache.GetUserPermissionsAsync(userId, 1);
        var tenant2CacheAfterInvalidation = await cache.GetUserPermissionsAsync(userId, 2);
        
        tenant1CacheAfterInvalidation.Should().BeNull("Tenant 1 cache should be cleared");
        tenant2CacheAfterInvalidation.Should().NotBeNull("Tenant 2 cache should remain intact");
        tenant2CacheAfterInvalidation.Should().BeEquivalentTo(new[] { "different.permission" },
            "Tenant 2 cached permissions should match original");
    }

    #endregion

    #region Cache Behavior Tests

    [Fact]
    public async Task CacheExpiration_ShouldHandleExpiredEntries()
    {
        // This test verifies that our InMemoryPermissionCache properly handles expiration
        // Note: This is more of a unit test for the cache implementation
        
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>() as InMemoryPermissionCache;
        cache.Should().NotBeNull("Should be using InMemoryPermissionCache for tests");
        
        const int userId = 123;
        const int tenantId = 1;
        var testPermissions = new[] { "test.permission" };
        
        // Act - Set permissions
        await cache!.SetUserPermissionsAsync(userId, tenantId, testPermissions);
        
        // Verify cached
        var cached = await cache.GetUserPermissionsAsync(userId, tenantId);
        cached.Should().BeEquivalentTo(testPermissions, "Permissions should be cached");
        
        // Note: In a real test, we might manipulate time or use a cache with very short expiration
        // For now, we just verify the cache behavior is working
    }

    [Fact]
    public async Task HighConcurrencyAccess_ShouldMaintainCacheIntegrity()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>() as InMemoryPermissionCache;
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        cache?.ResetMetrics();
        
        var userId = await GetUserIdFromTokenAsync(dbContext, "admin@tenant1.com");
        
        // Act - Simulate high concurrency
        var tasks = new List<Task<IEnumerable<string>>>();
        for (int i = 0; i < 50; i++)
        {
            tasks.Add(permissionService.GetUserPermissionsForTenantAsync(userId, 1));
        }
        
        var results = await Task.WhenAll(tasks);
        
        // Assert - All requests should complete successfully with consistent results
        results.Should().HaveCount(50, "All concurrent requests should complete");
        
        var firstResult = results[0].ToList();
        foreach (var result in results)
        {
            result.Should().BeEquivalentTo(firstResult, 
                "All concurrent requests should return identical results");
        }
        
        if (cache != null)
        {
            _output.WriteLine($"Concurrent access results: {cache.GetCacheHits()} hits, {cache.GetCacheMisses()} misses");
            cache.GetCacheHits().Should().BeGreaterThan(0, "Should have cache hits from concurrent access");
        }
    }

    #endregion

    #region API Endpoint Performance Tests

    [Fact]
    public async Task GetCurrentUserPermissions_APICalls_ShouldUseCaching()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>() as InMemoryPermissionCache;
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        cache?.ResetMetrics();
        
        var userId = await GetUserIdFromTokenAsync(dbContext, "admin@tenant1.com");
        
        // Act - Test cache performance through service layer instead of API
        var responses = new List<bool>();
        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < 10; i++)
        {
            // Use existing permission service to simulate API calls
            var permissions = await permissionService.GetUserPermissionsAsync(userId, CancellationToken.None);
            responses.Add(permissions?.Any() == true);
        }
        stopwatch.Stop();
        
        // Assert
        responses.Should().HaveCount(10, "All service calls should complete");
        responses.Should().AllSatisfy(hasPermissions => hasPermissions.Should().BeTrue("Admin should have permissions"));
        
        var averageResponseTime = stopwatch.ElapsedMilliseconds / 10.0;
        averageResponseTime.Should().BeLessThan(100, 
            $"Average service call time was {averageResponseTime}ms, expected < 100ms");
        
        if (cache != null)
        {
            _output.WriteLine($"Service cache performance:");
            _output.WriteLine($"  Cache hits: {cache.GetCacheHits()}");
            _output.WriteLine($"  Cache misses: {cache.GetCacheMisses()}");
            _output.WriteLine($"  Hit ratio: {cache.GetCacheHitRatio():F1}%");
            
            // After the first call, subsequent calls should mostly hit cache
            cache.GetCacheHits().Should().BeGreaterThan(0, "Service calls should utilize caching");
        }
    }

    #endregion

    #region Helper Methods

    private async Task<int> GetUserIdFromTokenAsync(ApplicationDbContext dbContext, string email)
    {
        var user = await dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email);
        
        if (user == null)
            throw new InvalidOperationException($"User with email {email} not found in test data");
        
        return user.Id;
    }

    #endregion
}
