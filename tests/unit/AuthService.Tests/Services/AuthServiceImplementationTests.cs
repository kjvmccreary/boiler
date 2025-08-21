using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using AuthService.Services;
using Common.Configuration;
using Contracts.Auth;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Auth;
using DTOs.Common;
using DTOs.Entities;

namespace AuthService.Tests.Services;

public class AuthServiceImplementationTests : TestBase
{
    private readonly AuthServiceImplementation _authService;
    private readonly Mock<ILogger<AuthServiceImplementation>> _mockLogger;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<ITenantManagementRepository> _mockTenantRepository;
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly JwtSettings _jwtSettings;

    public AuthServiceImplementationTests()
    {
        _mockLogger = new Mock<ILogger<AuthServiceImplementation>>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockTokenService = new Mock<ITokenService>();
        _mockTenantRepository = new Mock<ITenantManagementRepository>();
        _mockPermissionService = new Mock<IPermissionService>();
        _mockTenantProvider = new Mock<ITenantProvider>();
        _mockServiceProvider = new Mock<IServiceProvider>();

        _jwtSettings = new JwtSettings
        {
            SecretKey = "test-super-secret-jwt-key-that-is-at-least-256-bits-long-for-testing",
            Issuer = "TestAuthService", 
            Audience = "TestApp",
            ExpiryMinutes = 60,
            RefreshTokenExpiryDays = 7
        };

        _authService = new AuthServiceImplementation(
            Context,
            _mockPasswordService.Object,
            _mockTokenService.Object,
            Mapper,
            _mockLogger.Object,
            _jwtSettings,
            _mockTenantRepository.Object,
            _mockPermissionService.Object,
            _mockTenantProvider.Object,
            _mockServiceProvider.Object
        );
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        const int userId = 1;
        var request = new ChangePasswordDto
        {
            CurrentPassword = "Password123!", // Use the same password as test data
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };

        const string newHashedPassword = "new_hashed_password";

        // Mock password verification for the existing user's password
        _mockPasswordService
            .Setup(x => x.VerifyPassword("Password123!", It.IsAny<string>()))
            .Returns(true);

        _mockPasswordService
            .Setup(x => x.HashPassword(request.NewPassword))
            .Returns(newHashedPassword);

        // Act
        var result = await _authService.ChangePasswordAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue($"Change password should succeed but failed with: {result.Message}");
        result.Data.Should().BeTrue();
        result.Message.Should().Be("Password changed successfully");
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUser_ShouldReturnError()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "test@test.com", // Same email as test user
            FirstName = "Test",
            LastName = "User",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            TenantName = "New Company"
        };

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse("Should fail because user already exists");
        result.Message.Should().Contain("account with this email already exists");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnError()
    {
        // Arrange - Test the failure path which should work
        var request = new LoginRequestDto
        {
            Email = "test@test.com",
            Password = "WrongPassword123!"
        };

        // Mock password verification to return false
        _mockPasswordService
            .Setup(x => x.VerifyPassword("WrongPassword123!", It.IsAny<string>()))
            .Returns(false);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse("Should fail with wrong password");
        result.Message.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ShouldReturnError()
    {
        // Arrange - Test with non-existent user
        var request = new LoginRequestDto
        {
            Email = "nonexistent@test.com",
            Password = "Password123!"
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse("Should fail for non-existent user");
        result.Message.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task GetUserPermissionsAsync_ShouldReturnUserPermissions()
    {
        // Arrange
        const int userId = 1;
        var expectedPermissions = new List<string> { "users.view", "users.edit" };
        
        _mockPermissionService
            .Setup(x => x.GetUserPermissionsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPermissions);

        // Act
        var result = await _authService.GetUserPermissionsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedPermissions);
    }

    [Fact]
    public async Task GetUserRolesAsync_ShouldReturnUserRoles()
    {
        // Arrange
        const int userId = 1;
        const int tenantId = 1;
        var expectedRoles = new List<string> { "Admin", "User" };

        _mockTenantProvider
            .Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(tenantId);

        var userRoles = new List<UserRole>
        {
            new() { UserId = userId, RoleId = 1, TenantId = tenantId, IsActive = true },
            new() { UserId = userId, RoleId = 2, TenantId = tenantId, IsActive = true }
        };

        var roles = new List<Role>
        {
            new() { Id = 1, Name = "Admin", TenantId = tenantId, IsActive = true },
            new() { Id = 2, Name = "User", TenantId = tenantId, IsActive = true }
        };

        Context.UserRoles.AddRange(userRoles);
        Context.Roles.AddRange(roles);
        Context.SaveChanges();

        // Act
        var result = await _authService.GetUserRolesAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedRoles);
    }
}
