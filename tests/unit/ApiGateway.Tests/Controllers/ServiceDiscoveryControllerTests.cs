using ApiGateway.Controllers;
using ApiGateway.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit; // ✅ ADDED: Missing using directive

namespace ApiGateway.Tests.Controllers;

public class ServiceDiscoveryControllerTests
{
    private readonly Mock<IServiceDiscovery> _mockServiceDiscovery;
    private readonly Mock<ILogger<ServiceDiscoveryController>> _mockLogger;
    private readonly ServiceDiscoveryController _controller;

    public ServiceDiscoveryControllerTests()
    {
        _mockServiceDiscovery = new Mock<IServiceDiscovery>();
        _mockLogger = new Mock<ILogger<ServiceDiscoveryController>>();
        _controller = new ServiceDiscoveryController(_mockServiceDiscovery.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetServicesAsync_Should_Return_All_Services()
    {
        // Arrange
        var expectedServices = new Dictionary<string, ServiceHealthStatus>
        {
            ["auth-service"] = new ServiceHealthStatus
            {
                ServiceName = "auth-service",
                TotalInstances = 1,
                HealthyInstances = 1
            }
        };

        _mockServiceDiscovery
            .Setup(x => x.GetServiceHealthStatusAsync())
            .ReturnsAsync(expectedServices);

        // Act
        var result = await _controller.GetServicesAsync();

        // Assert
        result.Should().BeOfType<ActionResult<Dictionary<string, ServiceHealthStatus>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expectedServices);
    }

    [Fact]
    public async Task GetServiceEndpointsAsync_Should_Return_Healthy_Endpoints()
    {
        // Arrange
        var serviceName = "auth-service";
        var expectedEndpoints = new List<ServiceEndpoint>
        {
            new ServiceEndpoint
            {
                ServiceName = serviceName,
                Host = "localhost",
                Port = 7001,
                IsHealthy = true
            }
        };

        _mockServiceDiscovery
            .Setup(x => x.GetHealthyServicesAsync(serviceName))
            .ReturnsAsync(expectedEndpoints);

        // Act
        var result = await _controller.GetServiceEndpointsAsync(serviceName);

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<ServiceEndpoint>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(expectedEndpoints);
    }

    [Fact]
    public async Task GetServiceEndpointsAsync_Should_Return_NotFound_When_No_Healthy_Instances()
    {
        // Arrange
        var serviceName = "non-existent-service";
        _mockServiceDiscovery
            .Setup(x => x.GetHealthyServicesAsync(serviceName))
            .ReturnsAsync(new List<ServiceEndpoint>());

        // Act
        var result = await _controller.GetServiceEndpointsAsync(serviceName);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task RegisterServiceAsync_Should_Return_Ok_With_Valid_Endpoint()
    {
        // Arrange
        var endpoint = new ServiceEndpoint
        {
            ServiceName = "test-service",
            Host = "localhost",
            Port = 7001
        };

        _mockServiceDiscovery
            .Setup(x => x.RegisterServiceAsync(It.IsAny<ServiceEndpoint>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.RegisterServiceAsync(endpoint);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
        
        _mockServiceDiscovery.Verify(x => x.RegisterServiceAsync(It.Is<ServiceEndpoint>(e => 
            e.ServiceName == "test-service" && 
            e.Host == "localhost" && 
            e.Port == 7001)), Times.Once);
    }

    [Fact]
    public async Task RegisterServiceAsync_Should_Return_BadRequest_With_Invalid_Data()
    {
        // Arrange
        var endpoint = new ServiceEndpoint
        {
            ServiceName = "", // Invalid: empty service name
            Host = "localhost",
            Port = 7001
        };

        // Act
        var result = await _controller.RegisterServiceAsync(endpoint);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mockServiceDiscovery.Verify(x => x.RegisterServiceAsync(It.IsAny<ServiceEndpoint>()), Times.Never);
    }

    [Fact]
    public async Task UnregisterServiceAsync_Should_Return_Ok()
    {
        // Arrange
        var serviceName = "test-service";
        var instanceId = "test-instance";

        _mockServiceDiscovery
            .Setup(x => x.UnregisterServiceAsync(serviceName, instanceId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UnregisterServiceAsync(serviceName, instanceId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockServiceDiscovery.Verify(x => x.UnregisterServiceAsync(serviceName, instanceId), Times.Once);
    }
}
