using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using WorkflowService.Controllers;
using WorkflowService.Services.Interfaces; // ✅ ADD: Import service interfaces
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace WorkflowService.Tests.Controllers;

public class InstancesControllerTests : TestBase
{
    private readonly Mock<ILogger<InstancesController>> _mockLogger;
    private readonly Mock<IInstanceService> _mockInstanceService; // ✅ ADD: Mock the service
    private readonly InstancesController _controller;

    public InstancesControllerTests()
    {
        _mockLogger = CreateMockLogger<InstancesController>();
        _mockInstanceService = new Mock<IInstanceService>(); // ✅ ADD: Initialize mock service
        
        // ✅ FIX: Pass the mock service instead of DbContext
        _controller = new InstancesController(
            _mockInstanceService.Object,
            _mockLogger.Object);

        SetupControllerContext();
    }

    [Fact]
    public async Task GetInstances_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var expectedResponse = ApiResponseDto<PagedResultDto<WorkflowInstanceDto>>.SuccessResult(
            new PagedResultDto<WorkflowInstanceDto>
            {
                Items = new List<WorkflowInstanceDto>(),
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 50
            });

        _mockInstanceService.Setup(x => x.GetAllAsync(It.IsAny<GetInstancesRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetInstances(
            status: InstanceStatus.Running,
            workflowDefinitionId: null,
            startedByUserId: null,
            page: 1,
            pageSize: 50);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);

        // Verify service was called with correct parameters
        _mockInstanceService.Verify(x => x.GetAllAsync(
            It.Is<GetInstancesRequestDto>(r => 
                r.Status == InstanceStatus.Running &&
                r.Page == 1 &&
                r.PageSize == 50), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task GetInstance_ExistingId_ShouldReturnOkResult()
    {
        // Arrange
        var instanceDto = new WorkflowInstanceDto
        {
            Id = 1,
            WorkflowDefinitionId = 1,
            WorkflowDefinitionName = "Test Workflow",
            Status = InstanceStatus.Running
        };

        var expectedResponse = ApiResponseDto<WorkflowInstanceDto>.SuccessResult(instanceDto);

        _mockInstanceService.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetInstance(1);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);

        _mockInstanceService.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StartInstance_ValidRequest_ShouldReturnCreatedResult()
    {
        // Arrange
        var request = new StartInstanceRequestDto
        {
            WorkflowDefinitionId = 1,
            InitialContext = """{"requestId": 123}"""
        };

        var instanceDto = new WorkflowInstanceDto
        {
            Id = 1,
            WorkflowDefinitionId = 1,
            WorkflowDefinitionName = "Test Workflow",
            Status = InstanceStatus.Running
        };

        var expectedResponse = ApiResponseDto<WorkflowInstanceDto>.SuccessResult(instanceDto);

        _mockInstanceService.Setup(x => x.StartInstanceAsync(It.IsAny<StartInstanceRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.StartInstance(request);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as CreatedAtActionResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(201);

        _mockInstanceService.Verify(x => x.StartInstanceAsync(
            It.Is<StartInstanceRequestDto>(r => r.WorkflowDefinitionId == 1), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task TerminateInstance_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var expectedResponse = ApiResponseDto<bool>.SuccessResult(true);

        _mockInstanceService.Setup(x => x.TerminateAsync(1, It.IsAny<TerminateInstanceRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.TerminateInstance(1);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);

        _mockInstanceService.Verify(x => x.TerminateAsync(
            1, 
            It.Is<TerminateInstanceRequestDto>(r => r.Reason == "Manual termination"), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task GetInstanceHistory_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var events = new List<WorkflowEventDto>
        {
            new WorkflowEventDto
            {
                Id = 1,
                WorkflowInstanceId = 1,
                Type = "Instance",
                Name = "Started",
                OccurredAt = DateTime.UtcNow
            }
        };

        var expectedResponse = ApiResponseDto<List<WorkflowEventDto>>.SuccessResult(events);

        _mockInstanceService.Setup(x => x.GetHistoryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetInstanceHistory(1);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);

        _mockInstanceService.Verify(x => x.GetHistoryAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetInstanceStatus_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var status = new InstanceStatusDto
        {
            InstanceId = 1,
            Status = InstanceStatus.Running,
            ProgressPercentage = 50.0
        };

        var expectedResponse = ApiResponseDto<InstanceStatusDto>.SuccessResult(status);

        _mockInstanceService.Setup(x => x.GetStatusAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetInstanceStatus(1);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);

        _mockInstanceService.Verify(x => x.GetStatusAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StartInstance_ServiceReturnsError_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new StartInstanceRequestDto
        {
            WorkflowDefinitionId = 999 // Non-existent
        };

        var expectedResponse = ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Published workflow definition not found");

        _mockInstanceService.Setup(x => x.StartInstanceAsync(It.IsAny<StartInstanceRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.StartInstance(request);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as BadRequestObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetInstance_NonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var expectedResponse = ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Workflow instance not found");

        _mockInstanceService.Setup(x => x.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetInstance(999);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as NotFoundObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(404);
    }

    private void SetupControllerContext()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("tenantId", "1"),
            new Claim(ClaimTypes.Email, "test@example.com")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        httpContext.Request.Headers["X-Tenant-ID"] = "1";

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }
}
