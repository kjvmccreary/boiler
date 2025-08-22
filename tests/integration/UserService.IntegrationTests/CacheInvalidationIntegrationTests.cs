using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Xunit;
using UserService.IntegrationTests.Fixtures;
using UserService.IntegrationTests.TestUtilities;
using Common.Caching;
using Contracts.Services;
using Common.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DTOs.Entities; // ✅ ADD: Missing using directive for UserRole

namespace UserService.IntegrationTests;

/// <summary>
/// Integration tests specifically for cache invalidation scenarios in Phase 10
/// Tests that cache invalidation works correctly across role changes, user updates, etc.
/// </summary>
public class CacheInvalidationIntegrationTests : IClassFixture<WebApplicationTestFixture>
{
    private readonly WebApplicationTestFixture _factory;
    private readonly HttpClient _client;

    public CacheInvalidationIntegrationTests(WebApplicationTestFixture factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    #region Role Assignment Cache Invalidation

    [Fact]
    public async Task AssignUserToRole_ShouldInvalidateUserPermissionCache()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        var targetUserId = await GetUserIdFromEmailAsync(dbContext, "user@tenant1.com");
        const int tenantId = 1;
        
        // Cache initial permissions
        var initialPermissions = await permissionService.GetUserPermissionsForTenantAsync(targetUserId, tenantId);
        var cachedPermissions = await cache.GetUserPermissionsAsync(targetUserId, tenantId);
        cachedPermissions.Should().NotBeNull("Permissions should be cached after first load");
        
        // Get an admin role to assign (simulate what API would do)
        var adminRole = await dbContext.Roles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Name == "Admin" && r.TenantId == tenantId);
        adminRole.Should().NotBeNull("Admin role should exist for testing");
        
        // Act - Simulate role assignment and cache invalidation (like API would do)
        var userRole = new UserRole
        {
            UserId = targetUserId,
            RoleId = adminRole!.Id,
            TenantId = tenantId,
            AssignedAt = DateTime.UtcNow,
            AssignedBy = "Integration Test",
            IsActive = true
        };
        
        dbContext.UserRoles.Add(userRole);
        await dbContext.SaveChangesAsync();
        
        // Invalidate cache (simulating what the API would do)
        await cache.InvalidateUserPermissionsAsync(targetUserId, tenantId);
        
        // Assert - Cache should be invalidated and permissions should change
        var afterAssignment = await cache.GetUserPermissionsAsync(targetUserId, tenantId);
        afterAssignment.Should().BeNull("Cache should be empty after invalidation");
        
        // Verify permissions are reloaded and different
        var newPermissions = await permissionService.GetUserPermissionsForTenantAsync(targetUserId, tenantId);
        newPermissions.Should().NotBeEmpty("User should have permissions after role assignment");
        
        // The new permissions should be different (more permissions due to Admin role)
        newPermissions.Count().Should().BeGreaterThan(initialPermissions.Count(), 
            "User should have more permissions after being assigned Admin role");
    }

    [Fact]
    public async Task RemoveUserFromRole_ShouldInvalidateUserPermissionCache()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        var targetUserId = await GetUserIdFromEmailAsync(dbContext, "admin@tenant1.com"); // Start with admin user
        const int tenantId = 1;
        
        // Cache initial permissions (admin should have many)
        var initialPermissions = await permissionService.GetUserPermissionsForTenantAsync(targetUserId, tenantId);
        var cachedPermissions = await cache.GetUserPermissionsAsync(targetUserId, tenantId);
        cachedPermissions.Should().NotBeNull("Permissions should be cached after first load");
        
        // Find the admin role assignment to remove
        var adminUserRole = await dbContext.UserRoles
            .IgnoreQueryFilters()
            .Include(ur => ur.Role)
            .FirstOrDefaultAsync(ur => ur.UserId == targetUserId && ur.TenantId == tenantId && ur.Role!.Name == "Admin");
    
        adminUserRole.Should().NotBeNull("Admin user should have an admin role assignment");
    
        // Act - Remove user from admin role (simulate API behavior)
        dbContext.UserRoles.Remove(adminUserRole!);
        await dbContext.SaveChangesAsync();
    
        // Invalidate cache (simulating what API would do)
        await cache.InvalidateUserPermissionsAsync(targetUserId, tenantId);
    
        // Assert - Cache should be invalidated and permissions should change
        var afterRemoval = await cache.GetUserPermissionsAsync(targetUserId, tenantId);
        afterRemoval.Should().BeNull("Cache should be empty after invalidation");
    
        // Verify permissions are reloaded and different
        var newPermissions = await permissionService.GetUserPermissionsForTenantAsync(targetUserId, tenantId);
    
        // The user should now have different permissions (potentially empty if no other roles)
        newPermissions.Should().NotBeEquivalentTo(initialPermissions, 
            "User permissions should change after role removal");
    }

    #endregion

    #region Tenant-Level Cache Invalidation

    [Fact]
    public async Task TenantDeactivation_ShouldClearAllTenantCaches()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        const int tenantId = 2; // Use tenant 2 for this test
        
        // Get all users in tenant 2
        var tenant2Users = await dbContext.Users
            .IgnoreQueryFilters()
            .Where(u => u.TenantUsers.Any(tu => tu.TenantId == tenantId))
            .Select(u => u.Id)
            .ToListAsync();
        
        tenant2Users.Should().NotBeEmpty("Tenant 2 should have users for testing");
        
        // Cache permissions for all tenant 2 users
        foreach (var userId in tenant2Users)
        {
            await permissionService.GetUserPermissionsForTenantAsync(userId, tenantId);
        }
        
        // Verify all are cached
        foreach (var userId in tenant2Users)
        {
            var cached = await cache.GetUserPermissionsAsync(userId, tenantId);
            cached.Should().NotBeNull($"User {userId} permissions should be cached");
        }
        
        // Act - Simulate tenant deactivation (invalidate all tenant caches)
        await cache.InvalidateTenantPermissionsAsync(tenantId);
        
        // Assert - All tenant caches should be cleared
        foreach (var userId in tenant2Users)
        {
            var afterInvalidation = await cache.GetUserPermissionsAsync(userId, tenantId);
            afterInvalidation.Should().BeNull($"User {userId} cache should be cleared after tenant deactivation");
        }
        
        // Verify other tenant caches are not affected
        var tenant1UserId = await GetUserIdFromEmailAsync(dbContext, "admin@tenant1.com");
        await permissionService.GetUserPermissionsForTenantAsync(tenant1UserId, 1); // Cache it
        
        var tenant1Cache = await cache.GetUserPermissionsAsync(tenant1UserId, 1);
        tenant1Cache.Should().NotBeNull("Other tenant caches should not be affected");
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task ConcurrentCacheInvalidation_ShouldNotCauseDataCorruption()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
        
        await TestDataSeeder.SeedTestDataAsync(dbContext);
        var userId = await GetUserIdFromEmailAsync(dbContext, "admin@tenant1.com");
        const int tenantId = 1;
        
        // ✅ FIXED: Clear cache first to ensure clean state
        await cache.InvalidateUserPermissionsAsync(userId, tenantId);
        
        // Cache initial data with clean state
        var originalPermissions = await permissionService.GetUserPermissionsForTenantAsync(userId, tenantId);
        originalPermissions.Should().NotBeEmpty("Admin should have permissions in clean state");
        
        // ✅ FIXED: Ensure data is actually cached before testing concurrency
        var cachedCheck = await cache.GetUserPermissionsAsync(userId, tenantId);
        cachedCheck.Should().NotBeNull("Permissions should be cached after first load");
        
        // Act - Concurrent invalidation and access
        var invalidationTasks = Enumerable.Range(0, 10)
            .Select(_ => cache.InvalidateUserPermissionsAsync(userId, tenantId))
            .ToList();
        
        var accessTasks = Enumerable.Range(0, 10)
            .Select(_ => permissionService.GetUserPermissionsForTenantAsync(userId, tenantId))
            .ToList();
        
        // ✅ FIXED: Wait for all tasks and collect results
        await Task.WhenAll(invalidationTasks.Concat(accessTasks));
        var accessResults = await Task.WhenAll(accessTasks);
        
        // ✅ FIXED: Verify all access results are consistent
        foreach (var result in accessResults)
        {
            result.Should().BeEquivalentTo(originalPermissions, 
                "All concurrent access calls should return consistent data");
        }
        
        // Assert - Final state should be consistent with original
        var finalPermissions = await permissionService.GetUserPermissionsForTenantAsync(userId, tenantId);
        finalPermissions.Should().BeEquivalentTo(originalPermissions,
            "Final permissions should match original permissions after concurrent operations");
        
        // ✅ ADDITIONAL: Verify cache is in a valid state
        var finalCachedPermissions = await cache.GetUserPermissionsAsync(userId, tenantId);
        if (finalCachedPermissions != null)
        {
            finalCachedPermissions.Should().BeEquivalentTo(originalPermissions,
                "Cached permissions should be consistent if present");
        }
    }

    [Fact]
    public async Task CacheInvalidation_WithNonExistentUser_ShouldNotThrow()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IPermissionCache>();
        
        const int nonExistentUserId = 99999;
        const int tenantId = 1;
        
        // Act & Assert - Should not throw for non-existent user
        await cache.InvalidateUserPermissionsAsync(nonExistentUserId, tenantId);
        
        var cachedData = await cache.GetUserPermissionsAsync(nonExistentUserId, tenantId);
        cachedData.Should().BeNull("Non-existent user should have no cached data");
    }

    #endregion

    #region Helper Methods

    private async Task<int> GetUserIdFromEmailAsync(ApplicationDbContext dbContext, string email)
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
