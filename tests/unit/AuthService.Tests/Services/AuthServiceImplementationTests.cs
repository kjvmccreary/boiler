using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using AuthService.Services;
using Common.Configuration;
using Common.Data;
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
    private readonly JwtSettings _jwtSettings;

    public AuthServiceImplementationTests()
    {
        _mockLogger = new Mock<ILogger<AuthServiceImplementation>>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockTokenService = new Mock<ITokenService>();
        _mockTenantRepository = new Mock<ITenantManagementRepository>();
        
        _jwtSettings = new JwtSettings
        {
            SecretKey = "test-super-secret-jwt-key-that-is-at-least-256-bits-long-for-testing",
            Issuer = "TestAuthService",
            Audience = "TestApp",
            ExpiryMinutes = 60,
            RefreshTokenExpiryDays = 7
        };

        // FIXED: Match actual constructor signature (7 parameters)
        _authService = new AuthServiceImplementation(
            Context,                    // ApplicationDbContext
            _mockPasswordService.Object, // IPasswordService
            _mockTokenService.Object,   // ITokenService
            Mapper,                     // IMapper
            _mockLogger.Object,         // ILogger
            _jwtSettings,              // JwtSettings
            _mockTenantRepository.Object // ITenantManagementRepository
        );
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccessWithToken()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "test@test.com",
            Password = "Password123!"
        };

        var user = GetTestUser();
        var tenant = GetTestTenant();
        const string accessToken = "access_token";
        var refreshTokenEntity = new RefreshToken
        {
            Token = "refresh_token",
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };

        _mockPasswordService
            .Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(true);

        _mockTokenService
            .Setup(x => x.GenerateAccessTokenAsync(user, tenant))
            .ReturnsAsync(accessToken);

        _mockTokenService
            .Setup(x => x.CreateRefreshTokenAsync(user))
            .ReturnsAsync(refreshTokenEntity);

        // FIXED: Use correct method name
        _mockTenantRepository
            .Setup(x => x.GetTenantByIdAsync(user.TenantId!.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be(accessToken);
        result.Data.RefreshToken.Should().Be(refreshTokenEntity.Token);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnSuccessWithToken()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "newuser@test.com",
            FirstName = "New",
            LastName = "User",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            TenantName = "New Tenant"
        };

        const string hashedPassword = "hashed_password";
        const string accessToken = "access_token";
        var refreshTokenEntity = new RefreshToken
        {
            Token = "refresh_token",
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        };
        var newTenant = new Tenant { Id = 2, Name = "New Tenant" };

        _mockPasswordService
            .Setup(x => x.HashPassword(request.Password))
            .Returns(hashedPassword);

        _mockTokenService
            .Setup(x => x.GenerateAccessTokenAsync(It.IsAny<User>(), It.IsAny<Tenant>()))
            .ReturnsAsync(accessToken);

        _mockTokenService
            .Setup(x => x.CreateRefreshTokenAsync(It.IsAny<User>()))
            .ReturnsAsync(refreshTokenEntity);

        // FIXED: Use correct method names
        _mockTenantRepository
            .Setup(x => x.GetTenantByNameAsync(request.TenantName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        _mockTenantRepository
            .Setup(x => x.CreateTenantAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTenant);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be(accessToken);
        result.Data.RefreshToken.Should().Be(refreshTokenEntity.Token);
    }

    [Fact]
    public async Task ChangePasswordAsync_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        const int userId = 1;
        var request = new ChangePasswordDto
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };

        var user = GetTestUser();
        const string newHashedPassword = "new_hashed_password";

        _mockPasswordService
            .Setup(x => x.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            .Returns(true);

        _mockPasswordService
            .Setup(x => x.HashPassword(request.NewPassword))
            .Returns(newHashedPassword);

        // Act
        var result = await _authService.ChangePasswordAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
        result.Message.Should().Be("Password changed successfully");
    }
}
