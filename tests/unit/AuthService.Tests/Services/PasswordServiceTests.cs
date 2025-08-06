using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using AuthService.Services;

namespace AuthService.Tests.Services;

public class PasswordServiceTests
{
    private readonly PasswordService _passwordService;
    private readonly Mock<ILogger<PasswordService>> _mockLogger;

    public PasswordServiceTests()
    {
        _mockLogger = new Mock<ILogger<PasswordService>>();
        _passwordService = new PasswordService(_mockLogger.Object);
    }

    [Fact]
    public void HashPassword_WithValidPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        const string password = "TestPassword123!";

        // Act
        var hashedPassword = _passwordService.HashPassword(password);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().NotBe(password);
        hashedPassword.Length.Should().Be(60); // BCrypt hash length
    }

    [Theory]
    [InlineData("")] // FIXED: Remove null to avoid xUnit warning
    [InlineData("   ")]
    public void HashPassword_WithInvalidPassword_ShouldThrowArgumentException(string password)
    {
        // Act & Assert
        _passwordService.Invoking(x => x.HashPassword(password))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        const string password = "TestPassword123!";
        var hashedPassword = _passwordService.HashPassword(password);

        // Act
        var result = _passwordService.VerifyPassword(password, hashedPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        const string correctPassword = "TestPassword123!";
        const string incorrectPassword = "WrongPassword123!";
        var hashedPassword = _passwordService.HashPassword(correctPassword);

        // Act
        var result = _passwordService.VerifyPassword(incorrectPassword, hashedPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("", "validhash")] // FIXED: Remove null parameters
    [InlineData("validpassword", "")]
    public void VerifyPassword_WithInvalidInputs_ShouldReturnFalse(string password, string hash)
    {
        // Act
        var result = _passwordService.VerifyPassword(password, hash);

        // Assert
        result.Should().BeFalse();
    }
}
