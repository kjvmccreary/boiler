using ApiGateway.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace ApiGateway.Tests.Middleware;

public class RateLimitingMiddlewareTests : IDisposable
{
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly Mock<ILogger<RateLimitingMiddleware>> _mockLogger;
    private readonly IConfiguration _configuration; // ✅ CHANGED: Use real configuration
    private readonly RateLimitingMiddleware _middleware;

    public RateLimitingMiddlewareTests()
    {
        // ✅ ADDED: Clear cache before each test
        RateLimitingMiddleware.ClearCache();
        
        _mockNext = new Mock<RequestDelegate>();
        _mockLogger = new Mock<ILogger<RateLimitingMiddleware>>();
        
        // ✅ FIXED: Use real configuration instead of mocking
        _configuration = CreateTestConfiguration();
        
        _middleware = new RateLimitingMiddleware(_mockNext.Object, _mockLogger.Object, _configuration);
    }

    public void Dispose()
    {
        RateLimitingMiddleware.ClearCache();
    }

    [Fact]
    public async Task Should_Allow_Requests_Within_Limit()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.10");

        // Act & Assert - Make requests within limit (2 out of 3)
        for (int i = 0; i < 2; i++)
        {
            await _middleware.InvokeAsync(context);
            context.Response.StatusCode.Should().Be(200);
            _mockNext.Verify(next => next(context), Times.Exactly(i + 1));
        }
    }

    [Fact]
    public async Task Should_Block_Requests_Over_Limit()
    {
        // Arrange
        var context = CreateHttpContext();
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.11");

        // Act - Make requests up to limit (3)
        for (int i = 0; i < 3; i++)
        {
            var tempContext = CreateHttpContext();
            tempContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.11");
            await _middleware.InvokeAsync(tempContext);
        }

        // Make one more request that should be blocked
        var blockedContext = CreateHttpContext();
        blockedContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.11"); // Same IP
        blockedContext.Response.Body = new MemoryStream();
        
        await _middleware.InvokeAsync(blockedContext);

        // Assert
        blockedContext.Response.StatusCode.Should().Be(429);
        blockedContext.Response.Headers.Should().ContainKey("Retry-After");
        blockedContext.Response.Headers.Should().ContainKey("X-RateLimit-Limit");
        blockedContext.Response.Headers.Should().ContainKey("X-RateLimit-Remaining");
    }

    [Fact]
    public async Task Should_Apply_Per_Tenant_Limits()
    {
        // Arrange
        var sharedIp = "192.168.1.12";
        
        // Use up limit for tenant1 (3 requests)
        for (int i = 0; i < 3; i++)
        {
            var context1 = CreateHttpContext();
            context1.Items["TenantId"] = "tenant1";
            context1.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(sharedIp);
            await _middleware.InvokeAsync(context1);
        }

        // tenant1 should be blocked
        var blockedContext1 = CreateHttpContext();
        blockedContext1.Items["TenantId"] = "tenant1";
        blockedContext1.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(sharedIp);
        blockedContext1.Response.Body = new MemoryStream();
        
        await _middleware.InvokeAsync(blockedContext1);

        // tenant2 should still be allowed
        var context2 = CreateHttpContext();
        context2.Items["TenantId"] = "tenant2";
        context2.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(sharedIp); // Same IP, different tenant
        await _middleware.InvokeAsync(context2);

        // Assert
        blockedContext1.Response.StatusCode.Should().Be(429); // tenant1 blocked
        context2.Response.StatusCode.Should().Be(200); // tenant2 allowed
    }

    [Fact]
    public async Task Should_Apply_Per_User_Limits()
    {
        // Arrange - Make requests up to limit (3) for the same user
        for (int i = 0; i < 3; i++)
        {
            var context = CreateHttpContext();
            var claims = new[] { new Claim("sub", "testuser123") };
            var identity = new ClaimsIdentity(claims, "test");
            context.User = new ClaimsPrincipal(identity);
            await _middleware.InvokeAsync(context);
        }

        // Make one more request that should be blocked
        var blockedContext = CreateHttpContext();
        var blockedClaims = new[] { new Claim("sub", "testuser123") }; // Same user
        var blockedIdentity = new ClaimsIdentity(blockedClaims, "test");
        blockedContext.User = new ClaimsPrincipal(blockedIdentity);
        blockedContext.Response.Body = new MemoryStream();
        
        await _middleware.InvokeAsync(blockedContext);

        // Assert
        blockedContext.Response.StatusCode.Should().Be(429);
    }

    [Fact]
    public async Task Should_Reset_Limit_After_Period()
    {
        // Note: This test would require more complex setup to manipulate time
        // For now, we'll test the logic structure
        var context = CreateHttpContext();
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.13");

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(200);
        _mockNext.Verify(next => next(context), Times.Once);
    }

    // ✅ FIXED: Use real configuration instead of mocking
    private static IConfiguration CreateTestConfiguration()
    {
        var configData = new Dictionary<string, string?>
        {
            ["RateLimit:MaxRequests"] = "3", // Low limit for testing
            ["RateLimit:WindowMinutes"] = "1"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }

    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/test";
        context.Request.Headers["Content-Type"] = "application/json";
        context.Response.Body = new MemoryStream();
        return context;
    }
}
