using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Common.Data;
using Common.Services;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Xunit;

namespace RoleService.Tests;

public class RoleServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly Mock<ILogger<Common.Services.RoleService>> _mockLogger;
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Common.Services.RoleService _roleService;

    // Create real repository instances using the in-memory context
    private readonly RoleRepository _roleRepository;
    private readonly PermissionRepository _permissionRepository;
    private readonly UserRoleRepository _userRoleRepository;

    public RoleServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockTenantProvider = new Mock<ITenantProvider>();
        _mockLogger = new Mock<ILogger<Common.Services.RoleService>>();
        
        _context = new ApplicationDbContext(options, _mockHttpContextAccessor.Object, _mockTenantProvider.Object);
        
        // Create real repository instances
        _roleRepository = new RoleRepository(_context);
        _permissionRepository = new PermissionRepository(_context);
        _userRoleRepository = new UserRoleRepository(_context);

        _roleService = new Common.Services.RoleService(
            _context,
            _roleRepository,
            _permissionRepository,
            _userRoleRepository,
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
            new() { Id = 3, Name = "users.create", Category = "Users", Description = "Create users", IsActive = true },
            new() { Id = 4, Name = "roles.view", Category = "Roles", Description = "View roles", IsActive = true },
            new() { Id = 5, Name = "roles.edit", Category = "Roles", Description = "Edit roles", IsActive = true },
            new() { Id = 6, Name = "system.admin", Category = "System", Description = "System admin", IsActive = true }
        };

        _context.Permissions.AddRange(permissions);
        _context.SaveChanges();
    }

    #region CreateRoleAsync Tests

    [Fact]
    public async Task CreateRoleAsync_WithValidData_ShouldCreateRoleSuccessfully()
    {
        // Arrange
        const int tenantId = 1;
        const string roleName = "TestRole";
        const string description = "Test Description";
        var permissions = new List<string> { "users.view", "users.edit" };

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _roleService.CreateRoleAsync(roleName, description, permissions);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(roleName);
        result.Description.Should().Be(description);
        result.TenantId.Should().Be(tenantId);
        result.IsSystemRole.Should().BeFalse();
        result.IsDefault.Should().BeFalse();
        result.Permissions.Should().Contain("users.view");
        result.Permissions.Should().Contain("users.edit");

        // Verify in database
        var dbRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        dbRole.Should().NotBeNull();
        dbRole!.TenantId.Should().Be(tenantId);

        VerifyLogging(LogLevel.Information, "Created role");
    }

    [Fact]
    public async Task CreateRoleAsync_WithNoTenantContext_ShouldThrowInvalidOperationException()
    {
        // Arrange
        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync((int?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _roleService.CreateRoleAsync("TestRole", "Description", new List<string>()));

        exception.Message.Should().Contain("Tenant context not found");
        VerifyLogging(LogLevel.Error, "Error creating role");
    }

    [Fact]
    public async Task CreateRoleAsync_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        const int tenantId = 1;
        const string roleName = "ExistingRole";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create existing role
        var existingRole = new Role
        {
            Id = 1,
            Name = roleName,
            TenantId = tenantId,
            Description = "Existing",
            IsSystemRole = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(existingRole);
        await _context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _roleService.CreateRoleAsync(roleName, "Description", new List<string>()));

        exception.Message.Should().Contain("already exists in this tenant");
        VerifyLogging(LogLevel.Error, "Error creating role");
    }

    #endregion

    #region UpdateRoleAsync Tests

    [Fact]
    public async Task UpdateRoleAsync_WithValidData_ShouldUpdateRoleSuccessfully()
    {
        // Arrange
        const int tenantId = 1;
        const string newName = "UpdatedRole";
        const string newDescription = "Updated Description";
        var newPermissions = new List<string> { "users.view", "roles.view" };

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create existing role
        var existingRole = new Role
        {
            Id = 10,
            TenantId = tenantId,
            Name = "OldRole",
            Description = "Old Description",
            IsSystemRole = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(existingRole);
        await _context.SaveChangesAsync();

        // Act
        var result = await _roleService.UpdateRoleAsync(existingRole.Id, newName, newDescription, newPermissions);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(newName);
        result.Description.Should().Be(newDescription);
        result.Permissions.Should().Contain("users.view");
        result.Permissions.Should().Contain("roles.view");

        VerifyLogging(LogLevel.Information, "Updated role");
    }

    [Fact]
    public async Task UpdateRoleAsync_WithSystemRole_ShouldThrowInvalidOperationException()
    {
        // Arrange
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create system role
        var systemRole = new Role
        {
            Id = 11,
            TenantId = tenantId,
            Name = "SystemRole",
            IsSystemRole = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(systemRole);
        await _context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _roleService.UpdateRoleAsync(systemRole.Id, "NewName", "NewDescription", new List<string>()));

        exception.Message.Should().Contain("System roles cannot be modified");
        VerifyLogging(LogLevel.Error, "Error updating role");
    }

    [Fact]
    public async Task UpdateRoleAsync_WithNonExistentRole_ShouldThrowInvalidOperationException()
    {
        // Arrange
        const int tenantId = 1;
        const int roleId = 999;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _roleService.UpdateRoleAsync(roleId, "NewName", "NewDescription", new List<string>()));

        exception.Message.Should().Contain("Role not found");
        VerifyLogging(LogLevel.Error, "Error updating role");
    }

    #endregion

    #region DeleteRoleAsync Tests

    [Fact]
    public async Task DeleteRoleAsync_WithValidRole_ShouldDeleteSuccessfully()
    {
        // Arrange
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create role to delete
        var role = new Role
        {
            Id = 12,
            TenantId = tenantId,
            Name = "TestRole",
            IsSystemRole = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        await _roleService.DeleteRoleAsync(role.Id);

        // Assert
        var deletedRole = await _context.Roles.FindAsync(role.Id);
        deletedRole.Should().BeNull();

        VerifyLogging(LogLevel.Information, "Deleted role");
    }

    [Fact]
    public async Task DeleteRoleAsync_WithSystemRole_ShouldThrowInvalidOperationException()
    {
        // Arrange
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create system role
        var systemRole = new Role
        {
            Id = 13,
            TenantId = tenantId,
            Name = "SystemRole",
            IsSystemRole = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(systemRole);
        await _context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _roleService.DeleteRoleAsync(systemRole.Id));

        exception.Message.Should().Contain("System roles cannot be deleted");
        VerifyLogging(LogLevel.Error, "Error deleting role");
    }

    [Fact]
    public async Task DeleteRoleAsync_WithUsersAssigned_ShouldThrowInvalidOperationException()
    {
        // Arrange
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create role
        var role = new Role
        {
            Id = 14,
            TenantId = tenantId,
            Name = "TestRole",
            IsSystemRole = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(role);

        // Add user role assignment
        var userRole = new UserRole 
        { 
            UserId = 1, 
            RoleId = role.Id, 
            TenantId = tenantId, 
            IsActive = true,
            AssignedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _roleService.DeleteRoleAsync(role.Id));

        exception.Message.Should().Contain("has users assigned");
        VerifyLogging(LogLevel.Error, "Error deleting role");
    }

    #endregion

    #region GetRoleByIdAsync Tests

    [Fact]
    public async Task GetRoleByIdAsync_WithValidId_ShouldReturnRole()
    {
        // Arrange
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create role
        var role = new Role
        {
            Id = 15,
            TenantId = tenantId,
            Name = "TestRole",
            Description = "Test Description",
            IsSystemRole = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        var result = await _roleService.GetRoleByIdAsync(role.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(role.Id);
        result.Name.Should().Be("TestRole");
        result.TenantId.Should().Be(tenantId);
    }

    [Fact]
    public async Task GetRoleByIdAsync_WithSystemRole_ShouldReturnRole()
    {
        // Arrange
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create system role
        var systemRole = new Role
        {
            Id = 16,
            TenantId = null,
            Name = "SystemRole",
            Description = "System Description",
            IsSystemRole = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(systemRole);
        await _context.SaveChangesAsync();

        // Act
        var result = await _roleService.GetRoleByIdAsync(systemRole.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(systemRole.Id);
        result.Name.Should().Be("SystemRole");
        result.IsSystemRole.Should().BeTrue();
        result.TenantId.Should().Be(0); // Mapped from null
    }

    [Fact]
    public async Task GetRoleByIdAsync_WithNoTenantContext_ShouldReturnNull()
    {
        // Arrange
        const int roleId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync((int?)null);

        // Act
        var result = await _roleService.GetRoleByIdAsync(roleId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetRoleByIdAsync_WithNonExistentRole_ShouldReturnNull()
    {
        // Arrange
        const int tenantId = 1;
        const int roleId = 999;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _roleService.GetRoleByIdAsync(roleId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetTenantRolesAsync Tests

    [Fact]
    public async Task GetTenantRolesAsync_ShouldReturnTenantAndSystemRoles()
    {
        // Arrange
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create tenant role
        var tenantRole = new Role
        {
            Id = 17,
            TenantId = tenantId,
            Name = "TenantRole",
            IsSystemRole = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Create system role
        var systemRole = new Role
        {
            Id = 18,
            TenantId = null,
            Name = "SystemRole",
            IsSystemRole = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Roles.AddRange(tenantRole, systemRole);
        await _context.SaveChangesAsync();

        // Act
        var result = await _roleService.GetTenantRolesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.Name == "TenantRole" && r.TenantId == tenantId);
        result.Should().Contain(r => r.Name == "SystemRole" && r.IsSystemRole);
    }

    [Fact]
    public async Task GetTenantRolesAsync_WithNoTenantContext_ShouldReturnEmpty()
    {
        // Arrange
        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync((int?)null);

        // Act
        var result = await _roleService.GetTenantRolesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region AssignRoleToUserAsync Tests

    [Fact]
    public async Task AssignRoleToUserAsync_WithValidData_ShouldAssignRole()
    {
        // Arrange
        const int tenantId = 1;
        const int userId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create role
        var role = new Role
        {
            Id = 19,
            TenantId = tenantId,
            Name = "TestRole",
            IsSystemRole = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        await _roleService.AssignRoleToUserAsync(userId, role.Id);

        // Assert
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);
        
        userRole.Should().NotBeNull();
        userRole!.TenantId.Should().Be(tenantId);
        userRole.IsActive.Should().BeTrue();

        VerifyLogging(LogLevel.Information, "Assigned role");
    }

    [Fact]
    public async Task AssignRoleToUserAsync_WithNonExistentRole_ShouldThrowInvalidOperationException()
    {
        // Arrange
        const int tenantId = 1;
        const int userId = 1;
        const int roleId = 999;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _roleService.AssignRoleToUserAsync(userId, roleId));

        exception.Message.Should().Contain("Role not found");
        VerifyLogging(LogLevel.Error, "Error assigning role");
    }

    [Fact]
    public async Task AssignRoleToUserAsync_WithExistingInactiveAssignment_ShouldReactivate()
    {
        // Arrange
        const int tenantId = 1;
        const int userId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create role
        var role = new Role
        {
            Id = 20,
            TenantId = tenantId,
            Name = "TestRole",
            IsSystemRole = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(role);

        // Add inactive user role
        var existingUserRole = new UserRole
        {
            UserId = userId,
            RoleId = role.Id,
            TenantId = tenantId,
            IsActive = false,
            AssignedAt = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.UserRoles.Add(existingUserRole);
        await _context.SaveChangesAsync();

        // Act
        await _roleService.AssignRoleToUserAsync(userId, role.Id);

        // Assert
        var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);
        
        userRole.Should().NotBeNull();
        userRole!.IsActive.Should().BeTrue();
    }

    #endregion

    #region IsRoleNameAvailableAsync Tests

    [Fact]
    public async Task IsRoleNameAvailableAsync_WithAvailableName_ShouldReturnTrue()
    {
        // Arrange
        const int tenantId = 1;
        const string roleName = "AvailableRole";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Act
        var result = await _roleService.IsRoleNameAvailableAsync(roleName);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsRoleNameAvailableAsync_WithTakenName_ShouldReturnFalse()
    {
        // Arrange
        const int tenantId = 1;
        const string roleName = "TakenRole";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create role with the name
        var role = new Role
        {
            Id = 21,
            Name = roleName,
            TenantId = tenantId,
            IsSystemRole = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        var result = await _roleService.IsRoleNameAvailableAsync(roleName);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsRoleNameAvailableAsync_WithExcludedRole_ShouldReturnTrue()
    {
        // Arrange
        const int tenantId = 1;
        const string roleName = "ExistingRole";

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // Create role with the name
        var role = new Role
        {
            Id = 22,
            Name = roleName,
            TenantId = tenantId,
            IsSystemRole = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act - exclude the role we just created
        var result = await _roleService.IsRoleNameAvailableAsync(roleName, role.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsRoleNameAvailableAsync_WithNoTenantContext_ShouldReturnFalse()
    {
        // Arrange
        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync((int?)null);

        // Act
        var result = await _roleService.IsRoleNameAvailableAsync("TestRole");

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region CreateDefaultRolesForTenantAsync Tests

    [Fact]
    public async Task CreateDefaultRolesForTenantAsync_ShouldCreateDefaultRoles()
    {
        // Arrange
        const int tenantId = 2;

        // Act
        await _roleService.CreateDefaultRolesForTenantAsync(tenantId);

        // Assert
        var roles = await _context.Roles
            .Where(r => r.TenantId == tenantId && r.IsDefault)
            .ToListAsync();

        roles.Should().HaveCount(2);
        roles.Should().Contain(r => r.Name == "TenantAdmin");
        roles.Should().Contain(r => r.Name == "User");

        VerifyLogging(LogLevel.Information, "Created default roles");
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

#region Repository Implementations for Testing

public class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Role> Query() => _context.Roles.AsQueryable();

    public async Task<Role> AddAsync(Role entity, CancellationToken cancellationToken = default)
    {
        _context.Roles.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Role> UpdateAsync(Role entity, CancellationToken cancellationToken = default)
    {
        _context.Roles.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Roles.FindAsync(id);
        if (entity != null)
        {
            _context.Roles.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles.ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles.AnyAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles.CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    // IRoleRepository specific methods
    public async Task<Role?> GetByNameAsync(string name, int? tenantId = null, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name && r.TenantId == tenantId, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetTenantRolesAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Where(r => r.TenantId == tenantId || r.IsSystemRole)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetSystemRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Where(r => r.IsSystemRole)
            .ToListAsync(cancellationToken);
    }

    public async Task<Role?> GetWithPermissionsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetDefaultRolesAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Where(r => r.TenantId == tenantId && r.IsDefault)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, int? tenantId, int? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Roles.Where(r => r.Name == name && r.TenantId == tenantId);
        
        if (excludeRoleId.HasValue)
        {
            query = query.Where(r => r.Id != excludeRoleId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}

public class PermissionRepository : IPermissionRepository
{
    private readonly ApplicationDbContext _context;

    public PermissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Permission> Query() => _context.Permissions.AsQueryable();

    public async Task<Permission> AddAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        _context.Permissions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Permission> UpdateAsync(Permission entity, CancellationToken cancellationToken = default)
    {
        _context.Permissions.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Permissions.FindAsync(id);
        if (entity != null)
        {
            _context.Permissions.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.FindAsync(id);
    }

    public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    // IPermissionRepository specific methods
    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .Where(p => p.Category == category)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .Select(p => p.Category)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Permission>> GetByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .Where(p => names.Contains(p.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .AnyAsync(p => p.Name == name, cancellationToken);
    }
}

public class UserRoleRepository : IUserRoleRepository
{
    private readonly ApplicationDbContext _context;

    public UserRoleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<UserRole> Query() => _context.UserRoles.AsQueryable();

    public async Task<UserRole> AddAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        _context.UserRoles.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<UserRole> UpdateAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        _context.UserRoles.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.UserRoles.FindAsync(id);
        if (entity != null)
        {
            _context.UserRoles.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<UserRole?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles.FindAsync(id);
    }

    public async Task<IEnumerable<UserRole>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles.ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles.AnyAsync(ur => ur.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles.CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    // IUserRoleRepository specific methods
    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.TenantId == tenantId && ur.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetRoleUsersAsync(int roleId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.RoleId == roleId && ur.TenantId == tenantId && ur.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserRole?> GetUserRoleAsync(int userId, int roleId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.TenantId == tenantId, cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesWithDetailsAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Where(ur => ur.UserId == userId && ur.TenantId == tenantId && ur.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UserHasRoleAsync(int userId, int roleId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.TenantId == tenantId && ur.IsActive, cancellationToken);
    }

    public async Task RemoveUserRolesAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        foreach (var userRole in userRoles)
        {
            userRole.IsActive = false;
            userRole.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

#endregion
