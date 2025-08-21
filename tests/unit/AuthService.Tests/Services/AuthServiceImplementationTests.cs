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
using Microsoft.Extensions.DependencyInjection;

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
    private readonly Mock<IRoleTemplateService> _mockRoleTemplateService;
    private readonly Mock<IServiceScope> _mockScope;
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
        _mockRoleTemplateService = new Mock<IRoleTemplateService>();
        _mockScope = new Mock<IServiceScope>();

        // 🔧 CRITICAL FIX: Create proper service scope factory instead of mocking extension methods
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockScopedServiceProvider = new Mock<IServiceProvider>();
        
        // Setup the scoped service provider to return our role template service
        mockScopedServiceProvider
            .Setup(x => x.GetService(typeof(IRoleTemplateService)))
            .Returns(_mockRoleTemplateService.Object);

        _mockScope.Setup(x => x.ServiceProvider)
            .Returns(mockScopedServiceProvider.Object);

        // Setup scope factory (this uses base methods, not extension methods)
        mockScopeFactory
            .Setup(x => x.CreateScope())
            .Returns(_mockScope.Object);

        // Setup main service provider to return the scope factory
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(mockScopeFactory.Object);

        // 🔧 REMOVE: Don't mock extension methods directly
        // _mockServiceProvider.Setup(x => x.CreateScope())  // ❌ This fails in .NET 9

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
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            TenantId = null, // No tenant on initial login
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByIp = "127.0.0.1",
            IsRevoked = false
        };

        // Create a TenantUser relationship:
        var tenantUser = new TenantUser
        {
            TenantId = tenant.Id,
            UserId = user.Id,
            Role = "User",
            IsActive = true,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        Context.TenantUsers.Add(tenantUser);
        Context.SaveChanges();

        _mockPasswordService
            .Setup(x => x.VerifyPassword(request.Password, user.PasswordHash))
            .Returns(true);

        // 🔧 CRITICAL FIX: Mock the tenant-less token generation
        _mockTokenService
            .Setup(x => x.GenerateAccessTokenWithoutTenantAsync(user))
            .ReturnsAsync(accessToken);

        _mockTokenService
            .Setup(x => x.CreateRefreshTokenAsync(user))
            .ReturnsAsync(refreshTokenEntity);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue($"Login should succeed but failed with: {result.Message}");
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be(accessToken);
        result.Data.RefreshToken.Should().Be(refreshTokenEntity.Token);
        result.Data.Tenant.Should().BeNull(); // No tenant in initial login response
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
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            TenantId = 2,
            UserId = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByIp = "127.0.0.1",
            IsRevoked = false
        };
        var newTenant = new Tenant 
        { 
            Id = 2, 
            Name = "New Tenant",
            IsActive = true,
            SubscriptionPlan = "Basic",
            Settings = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockPasswordService
            .Setup(x => x.HashPassword(request.Password))
            .Returns(hashedPassword);

        _mockTokenService
            .Setup(x => x.GenerateAccessTokenAsync(It.IsAny<User>(), It.IsAny<Tenant>()))
            .ReturnsAsync(accessToken);

        _mockTokenService
            .Setup(x => x.CreateRefreshTokenAsync(It.IsAny<User>()))
            .ReturnsAsync(refreshTokenEntity);

        _mockTenantRepository
            .Setup(x => x.GetTenantByNameAsync(request.TenantName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        _mockTenantRepository
            .Setup(x => x.CreateTenantAsync(It.IsAny<Tenant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTenant);

        // 🔧 CRITICAL FIX: Add missing role template service mock
        _mockRoleTemplateService
            .Setup(x => x.CreateDefaultRolesForTenantAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // 🔧 CRITICAL FIX: Set up the context to return empty for existing user check
        // Since registration checks for existing users, we need empty result
        var emptyUsers = Context.Users.Where(u => u.Email == "nonexistent@test.com");
        Context.Users.RemoveRange(Context.Users); // Ensure no existing users
        Context.SaveChanges();

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue($"Registration should succeed but failed with: {result.Message}");
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be(accessToken);
        result.Data.RefreshToken.Should().Be(refreshTokenEntity.Token);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldCreateConfirmedUser()
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
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            TenantId = 2,
            UserId = 0, // Will be set after user creation
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByIp = "127.0.0.1",
            IsRevoked = false
        };
        var newTenant = new Tenant 
        { 
            Id = 2, 
            Name = "New Tenant",
            IsActive = true,
            SubscriptionPlan = "Basic",
            Settings = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockPasswordService
            .Setup(x => x.HashPassword(request.Password))
            .Returns(hashedPassword);

        _mockTokenService
            .Setup(x => x.GenerateAccessTokenAsync(It.IsAny<User>(), It.IsAny<Tenant>()))
            .ReturnsAsync(accessToken);

        _mockTokenService
            .Setup(x => x.CreateRefreshTokenAsync(It.IsAny<User>()))
            .ReturnsAsync(refreshTokenEntity);

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
        
        // IMPROVED: More detailed debugging
        if (!result.Success)
        {
            Console.WriteLine($"Registration failed with message: '{result.Message}'");
            if (result.Errors?.Any() == true)
            {
                Console.WriteLine($"Error details:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  Code: {error.Code}, Message: {error.Message}, Field: {error.Field}");
                }
            }
            
            // Check if any mocks were called as expected
            _mockPasswordService.Verify(x => x.HashPassword(It.IsAny<string>()), Times.AtLeastOnce, "Password hashing should have been called");
            _mockTenantRepository.Verify(x => x.GetTenantByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce, "Tenant lookup should have been called");
        }
        
        result.Success.Should().BeTrue($"Registration should succeed but failed with: {result.Message}");
        result.Data.Should().NotBeNull();
        result.Data!.AccessToken.Should().Be(accessToken);
        result.Data.RefreshToken.Should().Be(refreshTokenEntity.Token);

        // ✅ VERIFY: Registration should also create confirmed users
        var createdUser = Context.Users.First(u => u.Email == request.Email);
        createdUser.EmailConfirmed.Should().BeTrue(); // Both registration and admin creation should be consistent
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
        
        if (!result.Success)
        {
            Console.WriteLine($"Change password failed with message: '{result.Message}'");
            if (result.Errors?.Any() == true)
            {
                Console.WriteLine($"Error details:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"  Code: {error.Code}, Message: {error.Message}, Field: {error.Field}");
                }
            }
        }
        
        result.Success.Should().BeTrue($"Change password should succeed but failed with: {result.Message}");
        result.Data.Should().BeTrue();
        result.Message.Should().Be("Password changed successfully");
    }

    // NEW: Add tests for Enhanced Phase 4 RBAC methods
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
        _mockPermissionService.Verify(x => x.GetUserPermissionsAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
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

        // Setup the database context with test data
        var userRoles = new List<UserRole>
        {
            new() { UserId = userId, RoleId = 1, TenantId = tenantId },
            new() { UserId = userId, RoleId = 2, TenantId = tenantId }
        };

        var roles = new List<Role>
        {
            new() { Id = 1, Name = "Admin", TenantId = tenantId },
            new() { Id = 2, Name = "User", TenantId = tenantId }
        };

        Context.UserRoles.AddRange(userRoles);
        Context.Roles.AddRange(roles);
        Context.SaveChanges();

        // Act
        var result = await _authService.GetUserRolesAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedRoles);
        _mockTenantProvider.Verify(x => x.GetCurrentTenantIdAsync(), Times.Once);
    }

    // 🔧 ADD: Test for consultant tenant creation scenario
    [Fact]
    public async Task RegisterAsync_ExistingUserCreatingNewTenant_ShouldHandleConsultantScenario()
    {
        // Arrange - Create an existing user with UNIQUE ID
        var existingUser = new User
        {
            Id = 100,
            Email = "mike@consultant.com",
            FirstName = "Mike",
            LastName = "Consultant", 
            PasswordHash = "existing_hash",
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        Context.Users.Add(existingUser);
        Context.SaveChanges();

        var request = new RegisterRequestDto
        {
            Email = "mike@consultant.com", // Same email as existing user
            FirstName = "Mike",
            LastName = "Consultant",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            TenantName = "Client A Corp"
        };

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        
        // 🔧 FIX: The new AuthService logic rejects existing users
        result.Success.Should().BeFalse(); // Changed expectation
        result.Message.Should().Contain("account with this email already exists");
        result.Message.Should().Contain("sign in and create additional organizations");
    }
}
