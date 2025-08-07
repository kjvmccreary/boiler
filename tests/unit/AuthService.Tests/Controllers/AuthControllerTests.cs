using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using AuthService.Controllers;
using AuthService.Services; // ➕ ADD: For IPasswordService
using Contracts.Auth;
using DTOs.Auth;
using DTOs.Common;

namespace AuthService.Tests.Controllers;

public class AuthControllerTests
{
    private readonly AuthController _controller;
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly Mock<IPasswordService> _mockPasswordService; // ➕ ADD: Mock password service

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _mockPasswordService = new Mock<IPasswordService>(); // ➕ ADD: Initialize mock
        
        _controller = new AuthController(
            _mockAuthService.Object, 
            _mockLogger.Object,
            _mockPasswordService.Object); // ➕ ADD: Pass password service mock
    }

    [Fact]
    public async Task Register_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var expectedResponse = ApiResponseDto<TokenResponseDto>.SuccessResult(
            new TokenResponseDto { AccessToken = "token", RefreshToken = "refresh" }
        );

        _mockAuthService
            .Setup(x => x.RegisterAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Register_WithFailedRegistration_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        var expectedResponse = ApiResponseDto<TokenResponseDto>.ErrorResult("Registration failed");

        _mockAuthService
            .Setup(x => x.RegisterAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Register(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkResult()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "test@test.com",
            Password = "Password123!"
        };

        var expectedResponse = ApiResponseDto<TokenResponseDto>.SuccessResult(
            new TokenResponseDto { AccessToken = "token", RefreshToken = "refresh" }
        );

        _mockAuthService
            .Setup(x => x.LoginAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "test@test.com",
            Password = "WrongPassword"
        };

        var expectedResponse = ApiResponseDto<TokenResponseDto>.ErrorResult("Invalid credentials");

        _mockAuthService
            .Setup(x => x.LoginAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ChangePassword_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var request = new ChangePasswordDto
        {
            CurrentPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };

        var expectedResponse = ApiResponseDto<bool>.SuccessResult(true, "Password changed successfully");

        _mockAuthService
            .Setup(x => x.ChangePasswordAsync(1, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Setup user claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = await _controller.ChangePassword(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public void GetCurrentUser_WithValidToken_ShouldReturnUserInfo()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Email, "test@test.com"),
            new(ClaimTypes.GivenName, "Test"),
            new(ClaimTypes.Surname, "User"),
            new("tenant_id", "1"),
            new("tenant_name", "Test Tenant")
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        var result = _controller.GetCurrentUser();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as ApiResponseDto<object>;
        response!.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
    }

    [Fact]
    public void ValidateToken_WithAuthorizedRequest_ShouldReturnTrue()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = _controller.ValidateToken();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as ApiResponseDto<bool>;
        response!.Success.Should().BeTrue();
        response.Data.Should().BeTrue();
        response.Message.Should().Be("Token is valid");
    }

    // ➕ ADD: Test for the debug hash generation endpoint
    [Fact]
    public void GenerateHash_WithValidPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        const string password = "testpassword";
        const string expectedHash = "$2a$12$hashedpasswordexample";

        _mockPasswordService
            .Setup(x => x.HashPassword(password))
            .Returns(expectedHash);

        // Act
        var result = _controller.GenerateHash(password);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value;
        
        // Verify the response contains the expected structure
        response.Should().NotBeNull();
        
        // Verify the mock was called
        _mockPasswordService.Verify(x => x.HashPassword(password), Times.Once);
    }

    [Fact]
    public void GenerateHash_WithPasswordServiceException_ShouldReturnBadRequest()
    {
        // Arrange
        const string password = "testpassword";

        _mockPasswordService
            .Setup(x => x.HashPassword(password))
            .Throws(new Exception("Hash generation failed"));

        // Act
        var result = _controller.GenerateHash(password);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
