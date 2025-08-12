using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Common.Data;
using Common.Services;
using Contracts.Services;
using DTOs.Entities;
using Xunit;
using Microsoft.AspNetCore.Http;

namespace PermissionService.Tests;

public class PermissionServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly Mock<ILogger<Common.Services.PermissionService>> _mockLogger;
    private readonly Common.Services.PermissionService _permissionService;

    public PermissionServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockTenantProvider = new Mock<ITenantProvider>();
        _mockLogger = new Mock<ILogger<Common.Services.PermissionService>>();
        
        _context = new ApplicationDbContext(options, Mock.Of<IHttpContextAccessor>(), _mockTenantProvider.Object);
        
        _permissionService = new Common.Services.PermissionService(
            _context,
            _mockTenantProvider.Object,
            _mockLogger.Object
        );

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Add test permissions, roles, and user roles
        var permissions = new List<Permission>
        {
            new() { Id = 1, Name = "users.view", Category = "Users", IsActive = true },
            new() { Id = 2, Name = "users.edit", Category = "Users", IsActive = true },
            new() { Id = 3, Name = "roles.view", Category = "Roles", IsActive = true }
        };

        var roles = new List<Role>
        {
            new() { Id = 1, Name = "Admin", TenantId = 1, IsActive = true },
            new() { Id = 2, Name = "User", TenantId = 1, IsActive = true }
        };

        var rolePermissions = new List<RolePermission>
        {
            new() { RoleId = 1, PermissionId = 1, GrantedAt = DateTime.UtcNow },
            new() { RoleId = 1, PermissionId = 2, GrantedAt = DateTime.UtcNow },
            new() { RoleId = 2, PermissionId = 1, GrantedAt = DateTime.UtcNow }
        };

        var userRoles = new List<UserRole>
        {
            new() { UserId = 1, RoleId = 1, TenantId = 1, IsActive = true, AssignedAt = DateTime.UtcNow },
            new() { UserId = 2, RoleId = 2, TenantId = 1, IsActive = true, AssignedAt = DateTime.UtcNow }
        };

        _context.Permissions.AddRange(permissions);
        _context.Roles.AddRange(roles);
        _context.RolePermissions.AddRange(rolePermissions);
        _context.UserRoles.AddRange(userRoles);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetUserPermissionsAsync_WithValidUser_ReturnsPermissions()
    {
        // Arrange
        const int userId = 1;
        
        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _permissionService.GetUserPermissionsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("users.view");
        result.Should().Contain("users.edit");
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UserHasPermissionAsync_WithValidPermission_ReturnsTrue()
    {
        // Arrange
        const int userId = 1;
        const string permission = "users.edit";
        
        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _permissionService.UserHasPermissionAsync(userId, permission);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UserHasPermissionAsync_WithInvalidPermission_ReturnsFalse()
    {
        // Arrange
        const int userId = 2; // User role only has users.view
        const string permission = "users.edit";
        
        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _permissionService.UserHasPermissionAsync(userId, permission);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
