using ApiGateway.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

namespace ApiGateway.Tests.Services;

public class ServiceDiscoveryTests
{
    private readonly Mock<ILogger<ServiceDiscovery>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ServiceDiscovery _serviceDiscovery;

    public ServiceDiscoveryTests()
    {
        _mockLogger = new Mock<ILogger<ServiceDiscovery>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        
        _configuration = CreateTestConfiguration();
        _serviceDiscovery = new ServiceDiscovery(_mockLogger.Object, _httpClient, _configuration);
    }

    [Fact]
    public async Task Should_Register_Services()
    {
        // Arrange
        var endpoint = new ServiceEndpoint
        {
            ServiceName = "test-service",
            Host = "localhost",
            Port = 5001,
            InstanceId = "test-instance-1",
            IsHealthy = true
        };

        // Act
        await _serviceDiscovery.RegisterServiceAsync(endpoint);
        var services = await _serviceDiscovery.GetHealthyServicesAsync("test-service");

        // Assert
        services.Should().HaveCount(1);
        services.First().ServiceName.Should().Be("test-service");
        services.First().InstanceId.Should().Be("test-instance-1");
    }

    [Fact]
    public async Task Should_Detect_Unhealthy_Services()
    {
        // Arrange
        var endpoint = new ServiceEndpoint
        {
            ServiceName = "unhealthy-service",
            Host = "localhost",
            Port = 5002,
            InstanceId = "unhealthy-instance",
            HealthCheckPath = "/health",
            IsHealthy = true // Start as healthy, will become unhealthy
        };

        // Setup HTTP client to return 500 for health check
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        await _serviceDiscovery.RegisterServiceAsync(endpoint);

        // ✅ FIXED: Wait for health check to run (1 second interval + buffer)
        await Task.Delay(1500); // Wait 1.5 seconds for health check to complete

        // Act
        var healthyServices = await _serviceDiscovery.GetHealthyServicesAsync("unhealthy-service");

        // Assert
        healthyServices.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Load_Balance_Requests()
    {
        // Arrange
        var endpoint1 = new ServiceEndpoint
        {
            ServiceName = "load-balanced-service",
            Host = "localhost",
            Port = 5001,
            InstanceId = "instance-1",
            IsHealthy = true
        };

        var endpoint2 = new ServiceEndpoint
        {
            ServiceName = "load-balanced-service",
            Host = "localhost",
            Port = 5002,
            InstanceId = "instance-2",
            IsHealthy = true
        };

        await _serviceDiscovery.RegisterServiceAsync(endpoint1);
        await _serviceDiscovery.RegisterServiceAsync(endpoint2);

        // Act - Get service endpoint multiple times
        var selectedEndpoints = new List<ServiceEndpoint?>();
        for (int i = 0; i < 10; i++)
        {
            var endpoint = await _serviceDiscovery.GetServiceEndpointAsync("load-balanced-service");
            selectedEndpoints.Add(endpoint);
        }

        // Assert - Should have both instances selected (round-robin)
        var uniqueInstances = selectedEndpoints
            .Where(e => e != null)
            .Select(e => e!.InstanceId)
            .Distinct()
            .ToList();

        uniqueInstances.Should().HaveCount(2);
        uniqueInstances.Should().Contain("instance-1");
        uniqueInstances.Should().Contain("instance-2");
    }

    [Fact]
    public async Task Should_Return_Service_Health_Status()
    {
        // Arrange
        var endpoint = new ServiceEndpoint
        {
            ServiceName = "status-service",
            Host = "localhost",
            Port = 5003,
            InstanceId = "status-instance",
            IsHealthy = true
        };

        await _serviceDiscovery.RegisterServiceAsync(endpoint);

        // Act
        var healthStatus = await _serviceDiscovery.GetServiceHealthStatusAsync();

        // Assert
        healthStatus.Should().ContainKey("status-service");
        healthStatus["status-service"].ServiceName.Should().Be("status-service");
        healthStatus["status-service"].TotalInstances.Should().Be(1);
        healthStatus["status-service"].HealthyInstances.Should().Be(1);
        healthStatus["status-service"].Instances.Should().HaveCount(1);
    }

    [Fact]
    public async Task Should_Unregister_Services()
    {
        // Arrange
        var endpoint = new ServiceEndpoint
        {
            ServiceName = "temp-service",
            Host = "localhost",
            Port = 5004,
            InstanceId = "temp-instance",
            IsHealthy = true
        };

        await _serviceDiscovery.RegisterServiceAsync(endpoint);
        var servicesBeforeUnregister = await _serviceDiscovery.GetHealthyServicesAsync("temp-service");

        // Act
        await _serviceDiscovery.UnregisterServiceAsync("temp-service", "temp-instance");
        var servicesAfterUnregister = await _serviceDiscovery.GetHealthyServicesAsync("temp-service");

        // Assert
        servicesBeforeUnregister.Should().HaveCount(1);
        servicesAfterUnregister.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Handle_Service_Not_Found()
    {
        // Act
        var services = await _serviceDiscovery.GetHealthyServicesAsync("non-existent-service");
        var endpoint = await _serviceDiscovery.GetServiceEndpointAsync("non-existent-service");

        // Assert
        services.Should().BeEmpty();
        endpoint.Should().BeNull();
    }

    // ✅ FIXED: Use integer values only (no decimal)
    private static IConfiguration CreateTestConfiguration()
    {
        var configData = new Dictionary<string, string?>
        {
            // ✅ FIXED: Use "1" (1 second) instead of "0.1" (can't convert to int)
            ["ServiceDiscovery:HealthCheckIntervalSeconds"] = "1", // Must be integer
            ["ServiceDiscovery:HealthCheckTimeoutSeconds"] = "5"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }
}
