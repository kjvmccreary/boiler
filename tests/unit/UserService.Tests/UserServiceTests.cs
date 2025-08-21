using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using Common.Data;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Common;
using DTOs.User;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Xunit;
using UserService.Services;
using Contracts.User;
using Common.Repositories;

namespace UserService.Tests;

public class UserServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UserServiceImplementation>> _mockLogger;
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly UserServiceImplementation _userService;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(warnings => warnings.Default(WarningBehavior.Ignore))
            .Options;

        _mockTenantProvider = new Mock<ITenantProvider>();
        _context = new ApplicationDbContext(options, Mock.Of<IHttpContextAccessor>(), _mockTenantProvider.Object);
        
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UserServiceImplementation>>();
        _mockRoleService = new Mock<IRoleService>();
        _mockPermissionService = new Mock<IPermissionService>();
        _mockPasswordService = new Mock<IPasswordService>();

        var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
        _userRepository = new UserRepository(_context, _mockTenantProvider.Object, mockUserRepositoryLogger.Object);

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        _userService = new UserServiceImplementation(
            _userRepository,
            _mockTenantProvider.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockRoleService.Object,
            _mockPermissionService.Object,
            _mockPasswordService.Object,
            _context
        );
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidUser_ReturnsUser()
    {
        // Arrange
        const int userId = 1;
        const int tenantId = 1;
        
        var user = new User 
        { 
            Id = userId, 
            Email = "test@example.com", 
            FirstName = "Test", 
            LastName = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var userDto = new UserDto 
        { 
            Id = userId, 
            Email = "test@example.com", 
            FirstName = "Test", 
            LastName = "User",
            TenantId = tenantId
        };

        _context.Users.Add(user);
        var tenantUser = new TenantUser
        {
            TenantId = tenantId,
            UserId = userId,
            IsActive = true,
            Role = "User",
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.TenantUsers.Add(tenantUser);
        await _context.SaveChangesAsync();

        _mockMapper.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
            .Returns(userDto);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNonExistentUser_ReturnsError()
    {
        // Arrange
        const int userId = 999;

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("User not found");
    }

    [Fact]
    public async Task ExistsAsync_WithExistingUser_ReturnsTrue()
    {
        // Arrange
        const int userId = 1;
        const int tenantId = 1;
        
        var user = new User 
        { 
            Id = userId, 
            IsActive = true,
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        var tenantUser = new TenantUser
        {
            TenantId = tenantId,
            UserId = userId,
            IsActive = true,
            Role = "User",
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.TenantUsers.Add(tenantUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.ExistsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistentUser_ReturnsFalse()
    {
        // Arrange
        const int userId = 999;

        // Act
        var result = await _userService.ExistsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserPermissionsAsync_WithValidUser_ReturnsPermissions()
    {
        // Arrange
        const int userId = 1;
        const int tenantId = 1;
        var expectedPermissions = new List<string> { "users.view", "users.edit" };
        
        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        _mockPermissionService.Setup(x => x.GetUserPermissionsForTenantAsync(userId, tenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPermissions);

        // Act
        var result = await _userService.GetUserPermissionsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(expectedPermissions);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
