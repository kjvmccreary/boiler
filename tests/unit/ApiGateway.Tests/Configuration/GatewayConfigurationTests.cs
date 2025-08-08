using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit; // ✅ ADDED: Missing using directive

namespace ApiGateway.Tests.Configuration;

public class GatewayConfigurationTests
{
    [Fact]
    public void Should_Load_Valid_Routes()
    {
        // Arrange
        var ocelotConfig = new Dictionary<string, string>
        {
            ["Routes:0:DownstreamPathTemplate"] = "/api/auth/{everything}",
            ["Routes:0:DownstreamScheme"] = "https",
            ["Routes:0:DownstreamHostAndPorts:0:Host"] = "localhost",
            ["Routes:0:DownstreamHostAndPorts:0:Port"] = "7001",
            ["Routes:0:UpstreamPathTemplate"] = "/api/auth/{everything}",
            ["Routes:0:UpstreamHttpMethod:0"] = "GET",
            ["Routes:0:UpstreamHttpMethod:1"] = "POST"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(ocelotConfig!)
            .Build();

        // Act
        var routesSection = configuration.GetSection("Routes");

        // Assert
        routesSection.Should().NotBeNull();
        routesSection.GetChildren().Should().HaveCount(1);
        
        var firstRoute = routesSection.GetSection("0");
        firstRoute["DownstreamPathTemplate"].Should().Be("/api/auth/{everything}");
        firstRoute["DownstreamScheme"].Should().Be("https");
    }

    [Fact]
    public void Should_Validate_Downstream_Services()
    {
        // Arrange
        var serviceDiscoveryConfig = new Dictionary<string, string>
        {
            ["ServiceDiscovery:KnownServices:0:ServiceName"] = "auth-service",
            ["ServiceDiscovery:KnownServices:0:Host"] = "localhost",
            ["ServiceDiscovery:KnownServices:0:Port"] = "7001",
            ["ServiceDiscovery:KnownServices:0:Scheme"] = "https",
            ["ServiceDiscovery:KnownServices:1:ServiceName"] = "user-service",
            ["ServiceDiscovery:KnownServices:1:Host"] = "localhost",
            ["ServiceDiscovery:KnownServices:1:Port"] = "7002",
            ["ServiceDiscovery:KnownServices:1:Scheme"] = "https"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(serviceDiscoveryConfig!)
            .Build();

        // Act
        var knownServicesSection = configuration.GetSection("ServiceDiscovery:KnownServices");

        // Assert
        knownServicesSection.Should().NotBeNull();
        knownServicesSection.GetChildren().Should().HaveCount(2);
        
        var authService = knownServicesSection.GetSection("0");
        authService["ServiceName"].Should().Be("auth-service");
        authService["Port"].Should().Be("7001");
        
        var userService = knownServicesSection.GetSection("1");
        userService["ServiceName"].Should().Be("user-service");
        userService["Port"].Should().Be("7002");
    }

    [Fact]
    public void Should_Configure_Authentication()
    {
        // Arrange
        var jwtConfig = new Dictionary<string, string>
        {
            ["JwtSettings:SecretKey"] = "TestSecretKeyThatIsAtLeast32CharactersLong!",
            ["JwtSettings:Issuer"] = "TestIssuer",
            ["JwtSettings:Audience"] = "TestAudience",
            ["JwtSettings:ExpiryMinutes"] = "60"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(jwtConfig!)
            .Build();

        // Act
        var jwtSection = configuration.GetSection("JwtSettings");

        // Assert
        jwtSection.Should().NotBeNull();
        jwtSection["SecretKey"].Should().Be("TestSecretKeyThatIsAtLeast32CharactersLong!");
        jwtSection["Issuer"].Should().Be("TestIssuer");
        jwtSection["Audience"].Should().Be("TestAudience");
        jwtSection["ExpiryMinutes"].Should().Be("60");
    }

    [Fact]
    public void Should_Setup_Rate_Limiting()
    {
        // Arrange
        var rateLimitConfig = new Dictionary<string, string>
        {
            ["RateLimit:MaxRequests"] = "100",
            ["RateLimit:WindowMinutes"] = "1",
            ["GlobalConfiguration:RateLimitOptions:DisableRateLimitHeaders"] = "false",
            ["GlobalConfiguration:RateLimitOptions:QuotaExceededMessage"] = "Rate limit exceeded",
            ["GlobalConfiguration:RateLimitOptions:HttpStatusCode"] = "429"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(rateLimitConfig!)
            .Build();

        // Act
        var rateLimitSection = configuration.GetSection("RateLimit");
        var globalRateLimitSection = configuration.GetSection("GlobalConfiguration:RateLimitOptions");

        // Assert
        rateLimitSection["MaxRequests"].Should().Be("100");
        rateLimitSection["WindowMinutes"].Should().Be("1");
        globalRateLimitSection["HttpStatusCode"].Should().Be("429");
        globalRateLimitSection["QuotaExceededMessage"].Should().Be("Rate limit exceeded");
    }

    [Fact]
    public void Should_Load_Service_Discovery_Configuration()
    {
        // Arrange
        var serviceDiscoveryConfig = new Dictionary<string, string>
        {
            ["ServiceDiscovery:HealthCheckIntervalSeconds"] = "30",
            ["ServiceDiscovery:HealthCheckTimeoutSeconds"] = "5"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(serviceDiscoveryConfig!)
            .Build();

        // Act
        var serviceDiscoverySection = configuration.GetSection("ServiceDiscovery");

        // Assert
        serviceDiscoverySection["HealthCheckIntervalSeconds"].Should().Be("30");
        serviceDiscoverySection["HealthCheckTimeoutSeconds"].Should().Be("5");
    }
}
