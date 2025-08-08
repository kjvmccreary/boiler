using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Common.Data;
using Common.Services;
using Contracts.Services;
using DTOs.Entities;
using Xunit;

namespace PermissionService.Tests;

public class PermissionServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly Mock<ILogger<Common.Services.PermissionService>> _mockLogger;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Common.Services.PermissionService _permissionService;

    public PermissionServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockTenantProvider = new Mock<ITenantProvider>();
        _mockLogger = new Mock<ILogger<Common.Services.PermissionService>>();
        
        _context = new ApplicationDbContext(options, _mockHttpContextAccessor.Object, _mockTenantProvider.Object);

        _permissionService = new Common.Services.PermissionService(
            _context,
            _mockTenantProvider.Object,
            _mockLogger.Object
        );

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        // Create test permissions
        var permissions = new List<Permission>
        {
            new() { Id = 1, Name = "users.view", Category = "Users", Description = "View users", IsActive = true },
            new() { Id = 2, Name = "users.edit", Category = "Users", Description = "Edit users", IsActive = true },
            new() { Id = 3, Name = "users.delete", Category = "Users", Description = "Delete users", IsActive = true },
            new() { Id = 4, Name = "roles.view", Category = "Roles", Description = "View roles", IsActive = true },
            new() { Id = 5, Name = "roles.edit", Category = "Roles", Description = "Edit roles", IsActive = true },
            new() { Id = 6, Name = "reports.view", Category = "Reports", Description = "View reports", IsActive = true },
            new() { Id = 7, Name = "reports.export", Category = "Reports", Description = "Export reports", IsActive = true },
            new() { Id = 8, Name = "system.admin", Category = "System", Description = "System admin", IsActive = true },
            new() { Id = 9, Name = "inactive.permission", Category = "Test", Description = "Inactive permission", IsActive = false }
        };

        // Create test roles
        var roles = new List<Role>
        {
            new() { Id = 1, Name = "Admin", TenantId = 1, IsSystemRole = false, IsActive = true },
            new() { Id = 2, Name = "User", TenantId = 1, IsSystemRole = false, IsActive = true },
            new() { Id = 3, Name = "Manager", TenantId = 1, IsSystemRole = false, IsActive = true },
            new() { Id = 4, Name = "Admin", TenantId = 2, IsSystemRole = false, IsActive = true },
            new() { Id = 5, Name = "SuperAdmin", TenantId = null, IsSystemRole = true, IsActive = true }
        };

        // Create role-permission mappings
        var rolePermissions = new List<RolePermission>
        {
            // Tenant 1 Admin role permissions
            new() { RoleId = 1, PermissionId = 1, GrantedAt = DateTime.UtcNow }, // users.view
            new() { RoleId = 1, PermissionId = 2, GrantedAt = DateTime.UtcNow }, // users.edit
            new() { RoleId = 1, PermissionId = 3, GrantedAt = DateTime.UtcNow }, // users.delete
            new() { RoleId = 1, PermissionId = 4, GrantedAt = DateTime.UtcNow }, // roles.view
            new() { RoleId = 1, PermissionId = 5, GrantedAt = DateTime.UtcNow }, // roles.edit

            // Tenant 1 User role permissions
            new() { RoleId = 2, PermissionId = 1, GrantedAt = DateTime.UtcNow }, // users.view
            new() { RoleId = 2, PermissionId = 6, GrantedAt = DateTime.UtcNow }, // reports.view

            // Tenant 1 Manager role permissions
            new() { RoleId = 3, PermissionId = 1, GrantedAt = DateTime.UtcNow }, // users.view
            new() { RoleId = 3, PermissionId = 2, GrantedAt = DateTime.UtcNow }, // users.edit
            new() { RoleId = 3, PermissionId = 6, GrantedAt = DateTime.UtcNow }, // reports.view
            new() { RoleId = 3, PermissionId = 7, GrantedAt = DateTime.UtcNow }, // reports.export

            // Tenant 2 Admin role permissions
            new() { RoleId = 4, PermissionId = 1, GrantedAt = DateTime.UtcNow }, // users.view
            new() { RoleId = 4, PermissionId = 2, GrantedAt = DateTime.UtcNow }, // users.edit

            // SuperAdmin permissions
            new() { RoleId = 5, PermissionId = 8, GrantedAt = DateTime.UtcNow } // system.admin
        };

        // Create user-role assignments
        var userRoles = new List<UserRole>
        {
            // Tenant 1 users
            new() { UserId = 1, RoleId = 1, TenantId = 1, IsActive = true, AssignedAt = DateTime.UtcNow }, // User 1 = Admin in Tenant 1
            new() { UserId = 2, RoleId = 2, TenantId = 1, IsActive = true, AssignedAt = DateTime.UtcNow }, // User 2 = User in Tenant 1
            new() { UserId = 3, RoleId = 3, TenantId = 1, IsActive = true, AssignedAt = DateTime.UtcNow }, // User 3 = Manager in Tenant 1
            
            // Tenant 2 users
            new() { UserId = 4, RoleId = 4, TenantId = 2, IsActive = true, AssignedAt = DateTime.UtcNow }, // User 4 = Admin in Tenant 2
            
            // Cross-tenant SuperAdmin
            new() { UserId = 5, RoleId = 5, TenantId = 1, IsActive = true, AssignedAt = DateTime.UtcNow }, // User 5 = SuperAdmin

            // Inactive user role
            new() { UserId = 6, RoleId = 2, TenantId = 1, IsActive = false, AssignedAt = DateTime.UtcNow } // User 6 = Inactive User in Tenant 1
        };

        _context.Permissions.AddRange(permissions);
        _context.Roles.AddRange(roles);
        _context.RolePermissions.AddRange(rolePermissions);
        _context.UserRoles.AddRange(userRoles);
        _context.SaveChanges();
    }

    #region UserHasPermissionAsync Tests

    [Fact]
    public async Task UserHasPermissionAsync_WithValidUserAndPermission_ShouldReturnTrue()
    {
        // Arrange
        const int userId = 1; // Admin in Tenant 1
        const int tenantId = 1;
        const string permission = "users.view";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasPermissionAsync(userId, permission);

        // Assert
        result.Should().BeTrue();
        VerifyLogging(LogLevel.Debug, "Permission check");
    }

    [Fact]
    public async Task UserHasPermissionAsync_WithUserWithoutPermission_ShouldReturnFalse()
    {
        // Arrange
        const int userId = 2; // User in Tenant 1 (only has users.view and reports.view)
        const int tenantId = 1;
        const string permission = "users.delete"; // Admin-only permission

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasPermissionAsync(userId, permission);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UserHasPermissionAsync_WithNoTenantContext_ShouldReturnFalse()
    {
        // Arrange
        const int userId = 1;
        const string permission = "users.view";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync((int?)null);

        // Act
        var result = await _permissionService.UserHasPermissionAsync(userId, permission);

        // Assert
        result.Should().BeFalse();
        VerifyLogging(LogLevel.Warning, "No tenant context available");
    }

    [Fact]
    public async Task UserHasPermissionAsync_WithNonExistentUser_ShouldReturnFalse()
    {
        // Arrange
        const int userId = 999; // Non-existent user
        const int tenantId = 1;
        const string permission = "users.view";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasPermissionAsync(userId, permission);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UserHasPermissionAsync_WithInactiveUserRole_ShouldReturnFalse()
    {
        // Arrange
        const int userId = 6; // User with inactive role
        const int tenantId = 1;
        const string permission = "users.view";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasPermissionAsync(userId, permission);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UserHasPermissionAsync_WithCrossTenantAccess_ShouldReturnFalse()
    {
        // Arrange
        const int userId = 1; // Admin in Tenant 1
        const int tenantId = 2; // Different tenant
        const string permission = "users.view";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasPermissionAsync(userId, permission);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetUserPermissionsAsync Tests

    [Fact]
    public async Task GetUserPermissionsAsync_WithValidUser_ShouldReturnUserPermissions()
    {
        // Arrange
        const int userId = 1; // Admin in Tenant 1
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.GetUserPermissionsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5); // Admin has 5 permissions
        result.Should().Contain("users.view");
        result.Should().Contain("users.edit");
        result.Should().Contain("users.delete");
        result.Should().Contain("roles.view");
        result.Should().Contain("roles.edit");
        VerifyLogging(LogLevel.Debug, "Retrieved");
    }

    [Fact]
    public async Task GetUserPermissionsAsync_WithUserRole_ShouldReturnLimitedPermissions()
    {
        // Arrange
        const int userId = 2; // User in Tenant 1
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.GetUserPermissionsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // User has 2 permissions
        result.Should().Contain("users.view");
        result.Should().Contain("reports.view");
        result.Should().NotContain("users.edit");
        result.Should().NotContain("users.delete");
    }

    [Fact]
    public async Task GetUserPermissionsAsync_WithNoTenantContext_ShouldReturnEmpty()
    {
        // Arrange
        const int userId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync((int?)null);

        // Act
        var result = await _permissionService.GetUserPermissionsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        VerifyLogging(LogLevel.Warning, "No tenant context available");
    }

    [Fact]
    public async Task GetUserPermissionsAsync_WithNonExistentUser_ShouldReturnEmpty()
    {
        // Arrange
        const int userId = 999;
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.GetUserPermissionsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion
    #region GetUserPermissionsForTenantAsync Tests

    [Fact]
    public async Task GetUserPermissionsForTenantAsync_WithValidUserAndTenant_ShouldReturnPermissions()
    {
        // Arrange
        const int userId = 1; // Admin in Tenant 1
        const int tenantId = 1;

        // Act
        var result = await _permissionService.GetUserPermissionsForTenantAsync(userId, tenantId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
        result.Should().Contain("users.view");
        result.Should().Contain("users.edit");
        result.Should().Contain("users.delete");
        result.Should().Contain("roles.view");
        result.Should().Contain("roles.edit");
    }

    [Fact]
    public async Task GetUserPermissionsForTenantAsync_WithDifferentTenant_ShouldReturnEmpty()
    {
        // Arrange
        const int userId = 1; // Admin in Tenant 1
        const int tenantId = 2; // Different tenant

        // Act
        var result = await _permissionService.GetUserPermissionsForTenantAsync(userId, tenantId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserPermissionsForTenantAsync_WithCrossTenantUser_ShouldReturnCorrectPermissions()
    {
        // Arrange
        const int userId = 4; // Admin in Tenant 2
        const int tenantId = 2;

        // Act
        var result = await _permissionService.GetUserPermissionsForTenantAsync(userId, tenantId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // Tenant 2 admin has fewer permissions
        result.Should().Contain("users.view");
        result.Should().Contain("users.edit");
    }

    #endregion

    #region UserHasAnyPermissionAsync Tests

    [Fact]
    public async Task UserHasAnyPermissionAsync_WithAtLeastOnePermission_ShouldReturnTrue()
    {
        // Arrange
        const int userId = 2; // User with users.view and reports.view
        const int tenantId = 1;
        var permissions = new[] { "users.edit", "users.view", "users.delete" }; // Has users.view

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasAnyPermissionAsync(userId, permissions);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UserHasAnyPermissionAsync_WithNoMatchingPermissions_ShouldReturnFalse()
    {
        // Arrange
        const int userId = 2; // User with users.view and reports.view
        const int tenantId = 1;
        var permissions = new[] { "users.edit", "users.delete", "roles.view" }; // None of these

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasAnyPermissionAsync(userId, permissions);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UserHasAnyPermissionAsync_WithEmptyPermissionsList_ShouldReturnFalse()
    {
        // Arrange
        const int userId = 1;
        const int tenantId = 1;
        var permissions = Array.Empty<string>();

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasAnyPermissionAsync(userId, permissions);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region UserHasAllPermissionsAsync Tests

    [Fact]
    public async Task UserHasAllPermissionsAsync_WithAllRequiredPermissions_ShouldReturnTrue()
    {
        // Arrange
        const int userId = 1; // Admin with multiple permissions
        const int tenantId = 1;
        var permissions = new[] { "users.view", "users.edit", "roles.view" }; // Admin has all these

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasAllPermissionsAsync(userId, permissions);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UserHasAllPermissionsAsync_WithMissingPermissions_ShouldReturnFalse()
    {
        // Arrange
        const int userId = 2; // User with limited permissions
        const int tenantId = 1;
        var permissions = new[] { "users.view", "users.edit", "users.delete" }; // Missing edit and delete

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasAllPermissionsAsync(userId, permissions);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UserHasAllPermissionsAsync_WithEmptyPermissionsList_ShouldReturnTrue()
    {
        // Arrange
        const int userId = 1;
        const int tenantId = 1;
        var permissions = Array.Empty<string>();

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasAllPermissionsAsync(userId, permissions);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region GetAllAvailablePermissionsAsync Tests

    [Fact]
    public async Task GetAllAvailablePermissionsAsync_ShouldReturnOnlyActivePermissions()
    {
        // Act
        var result = await _permissionService.GetAllAvailablePermissionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(8); // 8 active permissions (excluding inactive one)
        result.Should().OnlyContain(p => p.IsActive);
        result.Should().Contain(p => p.Name == "users.view");
        result.Should().NotContain(p => p.Name == "inactive.permission");
    }

    [Fact]
    public async Task GetAllAvailablePermissionsAsync_ShouldReturnPermissionInfo()
    {
        // Act
        var result = await _permissionService.GetAllAvailablePermissionsAsync();

        // Assert
        var userViewPermission = result.FirstOrDefault(p => p.Name == "users.view");
        userViewPermission.Should().NotBeNull();
        userViewPermission!.Id.Should().Be(1);
        userViewPermission.Category.Should().Be("Users");
        userViewPermission.Description.Should().Be("View users");
        userViewPermission.IsActive.Should().BeTrue();
    }

    #endregion

    #region GetPermissionsByCategoryAsync Tests

    [Fact]
    public async Task GetPermissionsByCategoryAsync_ShouldGroupPermissionsByCategory()
    {
        // Act
        var result = await _permissionService.GetPermissionsByCategoryAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4); // Users, Roles, Reports, System
        result.Should().ContainKey("Users");
        result.Should().ContainKey("Roles");
        result.Should().ContainKey("Reports");
        result.Should().ContainKey("System");

        result["Users"].Should().HaveCount(3); // users.view, users.edit, users.delete
        result["Roles"].Should().HaveCount(2); // roles.view, roles.edit
        result["Reports"].Should().HaveCount(2); // reports.view, reports.export
        result["System"].Should().HaveCount(1); // system.admin
    }

    [Fact]
    public async Task GetPermissionsByCategoryAsync_ShouldNotIncludeInactivePermissions()
    {
        // Act
        var result = await _permissionService.GetPermissionsByCategoryAsync();

        // Assert
        result.Should().NotContainKey("Test"); // Category with only inactive permission
        result.Values.SelectMany(p => p).Should().OnlyContain(p => p.IsActive);
    }

    #endregion

    #region GetPermissionCategoriesAsync Tests

    [Fact]
    public async Task GetPermissionCategoriesAsync_ShouldReturnDistinctCategories()
    {
        // Act
        var result = await _permissionService.GetPermissionCategoriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().Contain("Users");
        result.Should().Contain("Roles");
        result.Should().Contain("Reports");
        result.Should().Contain("System");
        result.Should().BeInAscendingOrder(); // Should be sorted
    }

    [Fact]
    public async Task GetPermissionCategoriesAsync_ShouldNotIncludeInactivePermissionCategories()
    {
        // Act
        var result = await _permissionService.GetPermissionCategoriesAsync();

        // Assert
        result.Should().NotContain("Test"); // Category with only inactive permissions
    }

    #endregion

    #region GetPermissionsForCategoryAsync Tests

    [Fact]
    public async Task GetPermissionsForCategoryAsync_WithValidCategory_ShouldReturnPermissions()
    {
        // Act
        var result = await _permissionService.GetPermissionsForCategoryAsync("Users");

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "users.view");
        result.Should().Contain(p => p.Name == "users.edit");
        result.Should().Contain(p => p.Name == "users.delete");
        result.Should().BeInAscendingOrder(p => p.Name); // Should be sorted by name
    }

    [Fact]
    public async Task GetPermissionsForCategoryAsync_WithNonExistentCategory_ShouldReturnEmpty()
    {
        // Act
        var result = await _permissionService.GetPermissionsForCategoryAsync("NonExistent");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPermissionsForCategoryAsync_WithInactivePermissions_ShouldNotReturnInactive()
    {
        // Act
        var result = await _permissionService.GetPermissionsForCategoryAsync("Test");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty(); // Should not return inactive permissions
    }

    #endregion
    #region Performance Tests

    [Fact]
    public async Task UserHasPermissionAsync_WithLargeDataset_ShouldPerformEfficiently()
    {
        // Arrange - Add more test data
        const int userId = 1;
        const int tenantId = 1;
        const string permission = "users.view";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act - Multiple calls to test performance
        for (int i = 0; i < 100; i++)
        {
            await _permissionService.UserHasPermissionAsync(userId, permission);
        }

        stopwatch.Stop();

        // Assert - Should complete within reasonable time
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Less than 1 second for 100 calls
    }

    [Fact]
    public async Task GetUserPermissionsAsync_WithConcurrentCalls_ShouldHandleParallelism()
    {
        // Arrange
        const int userId = 1;
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act - Multiple concurrent calls
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => _permissionService.GetUserPermissionsAsync(userId))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert - All results should be consistent
        results.Should().HaveCount(10);
        results.Should().OnlyContain(r => r.Count() == 5); // All should return same 5 permissions
        results.Should().OnlyContain(r => r.Contains("users.view"));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task UserHasPermissionAsync_WithDatabaseException_ShouldReturnFalseAndLog()
    {
        // Arrange - Dispose context to force exception
        _context.Dispose();

        const int userId = 1;
        const string permission = "users.view";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _permissionService.UserHasPermissionAsync(userId, permission);

        // Assert
        result.Should().BeFalse();
        VerifyLogging(LogLevel.Error, "Error checking permission");
    }

    [Fact]
    public async Task GetUserPermissionsAsync_WithDatabaseException_ShouldReturnEmptyAndLog()
    {
        // Arrange - Dispose context to force exception
        _context.Dispose();

        const int userId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _permissionService.GetUserPermissionsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        VerifyLogging(LogLevel.Error, "Error getting permissions");
    }

    [Fact]
    public async Task GetPermissionCategoriesAsync_WithDatabaseException_ShouldReturnEmptyAndLog()
    {
        // Arrange - Dispose context to force exception
        _context.Dispose();

        // Act
        var result = await _permissionService.GetPermissionCategoriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        VerifyLogging(LogLevel.Error, "Error getting permission categories");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid.permission")]
    public async Task UserHasPermissionAsync_WithInvalidPermissionName_ShouldReturnFalse(string invalidPermission)
    {
        // Arrange
        const int userId = 1;
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasPermissionAsync(userId, invalidPermission);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public async Task UserHasPermissionAsync_WithInvalidUserId_ShouldReturnFalse(int invalidUserId)
    {
        // Arrange
        const int tenantId = 1;
        const string permission = "users.view";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _permissionService.UserHasPermissionAsync(invalidUserId, permission);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UserPermissionCaching_ShouldShowConsistentResults()
    {
        // Arrange
        const int userId = 1;
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act - Multiple calls should return consistent results
        var result1 = await _permissionService.GetUserPermissionsAsync(userId);
        var result2 = await _permissionService.GetUserPermissionsAsync(userId);
        var result3 = await _permissionService.GetUserPermissionsAsync(userId);

        // Assert
        result1.Should().BeEquivalentTo(result2);
        result2.Should().BeEquivalentTo(result3);
        result1.Should().HaveCount(5);
    }

    [Fact]
    public async Task CrossTenantPermissionIsolation_ShouldPreventDataLeakage()
    {
        // Arrange
        const int userId = 1; // Admin in Tenant 1

        // Act - Check permissions in different tenants
        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync()).ReturnsAsync(1);
        var tenant1Permissions = await _permissionService.GetUserPermissionsAsync(userId);

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync()).ReturnsAsync(2);
        var tenant2Permissions = await _permissionService.GetUserPermissionsAsync(userId);

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync()).ReturnsAsync(3);
        var tenant3Permissions = await _permissionService.GetUserPermissionsAsync(userId);

        // Assert
        tenant1Permissions.Should().HaveCount(5); // User has role in tenant 1
        tenant2Permissions.Should().BeEmpty(); // No role in tenant 2
        tenant3Permissions.Should().BeEmpty(); // No role in tenant 3
    }

    #endregion

    #region Helper Methods

    private void VerifyLogging(LogLevel logLevel, string message)
    {
        _mockLogger.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    #endregion

    public void Dispose()
    {
        _context.Dispose();
    }
}
