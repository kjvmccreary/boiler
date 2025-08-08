using ApiGateway.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Xunit; // ✅ ADDED: Missing using directive

namespace ApiGateway.Tests.Middleware;

public class AuthorizationContextMiddlewareTests
{
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly Mock<ILogger<AuthorizationContextMiddleware>> _mockLogger;
    private readonly AuthorizationContextMiddleware _middleware;

    public AuthorizationContextMiddlewareTests()
    {
        _mockNext = new Mock<RequestDelegate>();
        _mockLogger = new Mock<ILogger<AuthorizationContextMiddleware>>();
        _middleware = new AuthorizationContextMiddleware(_mockNext.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Should_Forward_Authorization_Header()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Headers["Authorization"] = "Bearer test-token";

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Request.Headers["X-Forwarded-Authorization"].Should().Contain("Bearer test-token");
        _mockNext.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task Should_Forward_User_Permissions()
    {
        // Arrange
        var context = CreateHttpContext();
        var claims = new[]
        {
            new Claim("sub", "user123"),
            new Claim("tenant_id", "tenant1"),
            new Claim("role", "Admin"),
            new Claim("permission", "users.view"),
            new Claim("permission", "users.edit"),
            new Claim("email", "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "test");
        context.User = new ClaimsPrincipal(identity);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Request.Headers.Should().ContainKey("X-User-Context");
        
        var userContextHeader = context.Request.Headers["X-User-Context"].ToString();
        var userContextJson = Encoding.UTF8.GetString(Convert.FromBase64String(userContextHeader));
        var userContext = JsonSerializer.Deserialize<JsonElement>(userContextJson);
        
        userContext.GetProperty("UserId").GetString().Should().Be("user123");
        userContext.GetProperty("TenantId").GetString().Should().Be("tenant1");
        userContext.GetProperty("Email").GetString().Should().Be("test@example.com");
        
        var roles = userContext.GetProperty("Roles").EnumerateArray().Select(r => r.GetString()).ToArray();
        roles.Should().Contain("Admin");
        
        var permissions = userContext.GetProperty("Permissions").EnumerateArray().Select(p => p.GetString()).ToArray();
        permissions.Should().Contain("users.view");
        permissions.Should().Contain("users.edit");
        
        _mockNext.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task Should_Forward_Tenant_Context()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Items["TenantId"] = "tenant1";

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Request.Headers["X-Tenant-Context"].Should().Contain("tenant1");
        _mockNext.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task Should_Handle_Anonymous_Requests()
    {
        // Arrange
        var context = CreateHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity()); // Anonymous user

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Request.Headers.Should().NotContainKey("X-User-Context");
        context.Request.Headers.Should().NotContainKey("X-Forwarded-Authorization");
        _mockNext.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task Should_Not_Forward_Empty_Authorization_Header()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Headers["Authorization"] = "";

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Request.Headers.Should().NotContainKey("X-Forwarded-Authorization");
        _mockNext.Verify(next => next(context), Times.Once);
    }

    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/test";
        context.Request.Headers["Content-Type"] = "application/json";
        return context;
    }
}
