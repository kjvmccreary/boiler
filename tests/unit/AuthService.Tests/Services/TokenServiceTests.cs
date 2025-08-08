using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Services;
using Common.Configuration;
using DTOs.Entities;

namespace AuthService.Tests.Services;

public class TokenServiceTests : TestBase
{
    private readonly TokenService _tokenService;
    private readonly Mock<ILogger<TokenService>> _mockLogger;
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly JwtSettings _jwtSettings;

    public TokenServiceTests()
    {
        _mockLogger = new Mock<ILogger<TokenService>>();
        _mockServiceProvider = new Mock<IServiceProvider>();
        _jwtSettings = new JwtSettings
        {
            SecretKey = "test-super-secret-jwt-key-that-is-at-least-256-bits-long-for-testing",
            Issuer = "TestAuthService",
            Audience = "TestApp",
            ExpiryMinutes = 60,
            RefreshTokenExpiryDays = 7,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        // FIXED: Pass all three required parameters
        _tokenService = new TokenService(_jwtSettings, _mockServiceProvider.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_WithValidUser_ShouldReturnValidToken()
    {
        // Arrange
        var user = GetTestUser();
        var tenant = GetTestTenant();

        // Act
        var token = await _tokenService.GenerateAccessTokenAsync(user, tenant);

        // Assert
        token.Should().NotBeNullOrEmpty();

        // Verify token structure
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);

        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == user.Email);
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.GivenName && c.Value == user.FirstName);
        jsonToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Surname && c.Value == user.LastName);
        jsonToken.Claims.Should().Contain(c => c.Type == "tenant_id" && c.Value == tenant.Id.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == "tenant_name" && c.Value == tenant.Name);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidRefreshToken()
    {
        // Act
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Length.Should().Be(88); // Base64 encoded 64-byte string
    }

    [Fact]
    public async Task GetPrincipalFromExpiredToken_WithValidToken_ShouldReturnClaimsPrincipal()
    {
        // Arrange
        var user = GetTestUser();
        var tenant = GetTestTenant();
        var token = await _tokenService.GenerateAccessTokenAsync(user, tenant);

        // Act - FIXED: Use correct method name
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.Identity.Should().NotBeNull();
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithInvalidToken_ShouldReturnNull()
    {
        // Arrange
        const string invalidToken = "invalid.jwt.token";

        // Act - FIXED: Use correct method name
        var principal = _tokenService.GetPrincipalFromExpiredToken(invalidToken);

        // Assert
        principal.Should().BeNull();
    }

    [Theory]
    [InlineData("")] // FIXED: Remove null parameter to avoid xUnit warning
    [InlineData("   ")]
    public void GetPrincipalFromExpiredToken_WithEmptyToken_ShouldReturnNull(string token)
    {
        // Act - FIXED: Use correct method name
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public async Task CreateRefreshTokenAsync_ShouldReturnValidRefreshToken()
    {
        // Arrange
        var user = GetTestUser();

        // Act
        var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

        // Assert
        refreshToken.Should().NotBeNull();
        refreshToken.Token.Should().NotBeNullOrEmpty();
        refreshToken.UserId.Should().Be(user.Id);
        refreshToken.ExpiryDate.Should().BeAfter(DateTime.UtcNow);
        refreshToken.IsRevoked.Should().BeFalse();
    }
}
