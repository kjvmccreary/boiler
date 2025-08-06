using FluentAssertions;
using DTOs.Auth;
using DTOs.Validators;

namespace AuthService.Tests.Validators;

public class ValidatorTests
{
    [Fact]
    public void LoginRequestDtoValidator_WithValidData_ShouldPass()
    {
        // Arrange
        var validator = new LoginRequestDtoValidator();
        var dto = new LoginRequestDto
        {
            Email = "test@test.com",
            Password = "Password123!"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Password123!")] // Empty email
    [InlineData("invalid-email", "Password123!")] // Invalid email format
    [InlineData("test@test.com", "")] // Empty password
    [InlineData("test@test.com", "123")] // Short password
    public void LoginRequestDtoValidator_WithInvalidData_ShouldFail(string email, string password)
    {
        // Arrange
        var validator = new LoginRequestDtoValidator();
        var dto = new LoginRequestDto
        {
            Email = email,
            Password = password
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void RegisterRequestDtoValidator_WithValidData_ShouldPass()
    {
        // Arrange
        var validator = new RegisterRequestDtoValidator();
        var dto = new RegisterRequestDto
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RegisterRequestDtoValidator_WithMismatchedPasswords_ShouldFail()
    {
        // Arrange
        var validator = new RegisterRequestDtoValidator();
        var dto = new RegisterRequestDto
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "Password123!",
            ConfirmPassword = "DifferentPassword123!"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("do not match"));
    }

    [Theory]
    [InlineData("weakpass")] // No uppercase, no digit, no special char
    [InlineData("WEAKPASS")] // No lowercase, no digit, no special char
    [InlineData("WeakPass")] // No digit, no special char
    [InlineData("WeakPass1")] // No special char
    public void RegisterRequestDtoValidator_WithWeakPassword_ShouldFail(string password)
    {
        // Arrange
        var validator = new RegisterRequestDtoValidator();
        var dto = new RegisterRequestDto
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Password = password,
            ConfirmPassword = password
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("uppercase"));
    }

    [Fact]
    public void ChangePasswordDtoValidator_WithValidData_ShouldPass()
    {
        // Arrange
        var validator = new ChangePasswordDtoValidator();
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ChangePasswordDtoValidator_WithSamePasswords_ShouldFail()
    {
        // Arrange
        var validator = new ChangePasswordDtoValidator();
        var dto = new ChangePasswordDto
        {
            CurrentPassword = "Password123!",
            NewPassword = "Password123!",
            ConfirmNewPassword = "Password123!"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("must be different"));
    }
}
