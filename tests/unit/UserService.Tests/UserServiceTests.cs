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
using MockQueryable.Moq; // 🔧 .NET 9 FIX: Add this package for async LINQ support

namespace UserService.Tests;

public class UserServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UserService.Services.UserServiceImplementation>> _mockLogger; // ✅ FIXED: Use correct type
    private readonly Mock<IRoleService> _mockRoleService;
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly Mock<IPasswordService> _mockPasswordService; // ✅ NEW: Mock password service
    private readonly UserService.Services.UserServiceImplementation _userService; // ✅ FIXED: Use correct type

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options, Mock.Of<IHttpContextAccessor>(), Mock.Of<ITenantProvider>());
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTenantProvider = new Mock<ITenantProvider>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UserService.Services.UserServiceImplementation>>(); // ✅ FIXED
        _mockRoleService = new Mock<IRoleService>();
        _mockPermissionService = new Mock<IPermissionService>();
        _mockPasswordService = new Mock<IPasswordService>(); // ✅ NEW: Initialize mock password service

        _userService = new UserService.Services.UserServiceImplementation( // ✅ UPDATED: Use correct class name
            _mockUserRepository.Object,
            _mockTenantProvider.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockRoleService.Object,
            _mockPermissionService.Object,
            _mockPasswordService.Object // ✅ ADD: Include password service
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
            TenantId = tenantId, 
            IsActive = true 
        };
        var userDto = new UserDto 
        { 
            Id = userId, 
            Email = "test@example.com", 
            FirstName = "Test", 
            LastName = "User" 
        };

        // 🔧 .NET 9 FIX: Setup tenant provider
        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // 🔧 .NET 9 FIX: Create async-capable mock queryable
        var users = new List<User> { user };
        var mockQueryable = users.AsQueryable().BuildMockDbSet();
        
        _mockUserRepository.Setup(x => x.Query())
            .Returns(mockQueryable.Object);

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
        const int tenantId = 1;

        // 🔧 .NET 9 FIX: Setup tenant provider
        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // 🔧 .NET 9 FIX: Create empty async-capable mock queryable
        var users = new List<User>(); // Empty list - no users
        var mockQueryable = users.AsQueryable().BuildMockDbSet();
        
        _mockUserRepository.Setup(x => x.Query())
            .Returns(mockQueryable.Object);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("User not found");
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

    [Fact]
    public async Task ExistsAsync_WithExistingUser_ReturnsTrue()
    {
        // Arrange
        const int userId = 1;
        const int tenantId = 1;
        
        var user = new User { Id = userId, TenantId = tenantId, IsActive = true };

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // 🔧 .NET 9 FIX: Create async-capable mock queryable
        var users = new List<User> { user };
        var mockQueryable = users.AsQueryable().BuildMockDbSet();
        
        _mockUserRepository.Setup(x => x.Query())
            .Returns(mockQueryable.Object);

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
        const int tenantId = 1;

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // 🔧 .NET 9 FIX: Create empty async-capable mock queryable
        var users = new List<User>(); // Empty list - no users
        var mockQueryable = users.AsQueryable().BuildMockDbSet();
        
        _mockUserRepository.Setup(x => x.Query())
            .Returns(mockQueryable.Object);

        // Act
        var result = await _userService.ExistsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeFalse();
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ShouldCreateActiveUser()
    {
        // Arrange
        const int tenantId = 1;
        var createRequest = new CreateUserDto
        {
            Email = "newuser@example.com",
            FirstName = "New",
            LastName = "User", 
            Password = "Password123!"
        };

        var expectedUser = new User
        {
            Id = 1,
            Email = "newuser@example.com",
            FirstName = "New",
            LastName = "User",
            TenantId = tenantId,
            IsActive = true,
            EmailConfirmed = true, // ✅ UPDATED: Should be true for admin-created users
            PasswordHash = "hashed_password_123"
        };

        var userDto = new UserDto
        {
            Id = 1,
            Email = "newuser@example.com",
            FirstName = "New",
            LastName = "User",
            IsActive = true,
            EmailConfirmed = true // ✅ UPDATED: Expect true
        };

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        // ✅ NEW: Mock password service
        _mockPasswordService.Setup(x => x.HashPassword("Password123!"))
            .Returns("hashed_password_123");

        // Empty list for existing user check
        var emptyUsers = new List<User>().AsQueryable().BuildMockDbSet();
        _mockUserRepository.Setup(x => x.Query()).Returns(emptyUsers.Object);

        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken ct) => user); // Return the user

        _mockMapper.Setup(x => x.Map<UserDto>(It.IsAny<User>()))
            .Returns(userDto);

        // Act
        var result = await _userService.CreateUserAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.EmailConfirmed.Should().BeTrue(); // ✅ VERIFY: Email should be confirmed
        
        // Verify password was hashed
        _mockPasswordService.Verify(x => x.HashPassword("Password123!"), Times.Once);
        
        // Verify user was added with correct properties
        _mockUserRepository.Verify(x => x.AddAsync(
            It.Is<User>(u => u.EmailConfirmed == true && u.IsActive == true), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithDuplicateEmail_ShouldReturnError()
    {
        // Arrange
        const int tenantId = 1;
        var createRequest = new CreateUserDto
        {
            Email = "existing@example.com",
            FirstName = "New",
            LastName = "User",
            Password = "Password123!"
        };

        var existingUser = new User 
        { 
            Id = 1, 
            Email = "existing@example.com", 
            TenantId = tenantId 
        };

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        var existingUsers = new List<User> { existingUser }.AsQueryable().BuildMockDbSet();
        _mockUserRepository.Setup(x => x.Query()).Returns(existingUsers.Object);

        // Act
        var result = await _userService.CreateUserAsync(createRequest);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already exists");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
