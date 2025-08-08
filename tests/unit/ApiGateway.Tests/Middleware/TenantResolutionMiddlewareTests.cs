using ApiGateway.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit; // ✅ ADDED: Missing using directive

namespace ApiGateway.Tests.Middleware;

public class TenantResolutionMiddlewareTests
{
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly Mock<ILogger<TenantResolutionMiddleware>> _mockLogger;
    private readonly TenantResolutionMiddleware _middleware;

    public TenantResolutionMiddlewareTests()
    {
        _mockNext = new Mock<RequestDelegate>();
        _mockLogger = new Mock<ILogger<TenantResolutionMiddleware>>();
        _middleware = new TenantResolutionMiddleware(_mockNext.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Should_Extract_Tenant_From_Domain()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Host = new HostString("tenant1.example.com");

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Items["TenantId"].Should().Be("tenant1");
        context.Request.Headers["X-Tenant-ID"].Should().Contain("tenant1");
        _mockNext.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task Should_Extract_Tenant_From_Header()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Headers["X-Tenant-ID"] = "tenant2";

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Items["TenantId"].Should().Be("tenant2");
        context.Request.Headers["X-Tenant-ID"].Should().Contain("tenant2");
        _mockNext.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task Should_Extract_Tenant_From_Claim()
    {
        // Arrange
        var context = CreateHttpContext();
        var claims = new[]
        {
            new Claim("tenant_id", "tenant3")
        };
        var identity = new ClaimsIdentity(claims, "test");
        context.User = new ClaimsPrincipal(identity);

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Items["TenantId"].Should().Be("tenant3");
        context.Request.Headers["X-Tenant-ID"].Should().Contain("tenant3");
        _mockNext.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task Should_Handle_Missing_Tenant()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Host = new HostString("localhost");

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Items.Should().NotContainKey("TenantId");
        _mockNext.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task Should_Prioritize_Domain_Over_Header()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Host = new HostString("tenant1.example.com");
        context.Request.Headers["X-Tenant-ID"] = "tenant2";

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Items["TenantId"].Should().Be("tenant1");
        context.Request.Headers["X-Tenant-ID"].Should().Contain("tenant1");
        _mockNext.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task Should_Extract_Tenant_From_Path()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Request.Path = "/tenant/tenant4/api/users";

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Items["TenantId"].Should().Be("tenant4");
        context.Request.Headers["X-Tenant-ID"].Should().Contain("tenant4");
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
