using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using WorkflowService.Controllers;
using WorkflowService.Services.Interfaces;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using AutoMapper;

namespace WorkflowService.Tests.Controllers;

public class InstancesControllerTests : TestBase
{
    private readonly Mock<ILogger<InstancesController>> _mockLogger;
    private readonly Mock<IInstanceService> _mockInstanceService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly InstancesController _controller;

    public InstancesControllerTests()
    {
        _mockLogger = CreateMockLogger<InstancesController>();
        _mockInstanceService = new Mock<IInstanceService>();
        _mockMapper = new Mock<IMapper>();

        // Basic mapper setup (only needed for suspend/resume if invoked)
        _mockMapper.Setup(m => m.Map<WorkflowInstanceDto>(It.IsAny<object>()))
            .Returns(new WorkflowInstanceDto());

        _controller = new InstancesController(
            _mockInstanceService.Object,
            _mockLogger.Object,
            DbContext,            // from TestBase
            _mockMapper.Object);

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
    public async Task GetInstance_ExistingId_ShouldReturnOk()
    {
        // Arrange
        var dto = new WorkflowInstanceDto
        {
            Id = 1,
            WorkflowDefinitionId = 1,
            WorkflowDefinitionName = "WF",
            Status = InstanceStatus.Running
        };

        _mockInstanceService.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(dto));

        // Act
        var result = await _controller.GetInstance(1);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task StartInstance_ValidRequest_ShouldReturnCreated()
    {
        // Arrange
        var req = new StartInstanceRequestDto { WorkflowDefinitionId = 10 };
        var dto = new WorkflowInstanceDto { Id = 123, WorkflowDefinitionId = 10, WorkflowDefinitionName = "WF", Status = InstanceStatus.Running };

        _mockInstanceService.Setup(x => x.StartInstanceAsync(It.IsAny<StartInstanceRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(dto));

        // Act
        var result = await _controller.StartInstance(req);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as CreatedAtActionResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task TerminateInstance_ValidRequest_ShouldReturnOk()
    {
        // Arrange
        _mockInstanceService.Setup(x => x.TerminateAsync(5, It.IsAny<TerminateInstanceRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<bool>.SuccessResult(true));

        // Act
        var result = await _controller.TerminateInstance(5);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Suspend_Instance_Should_Call_Service_And_Return_Ok()
    {
        _mockInstanceService.Setup(s => s.SuspendAsync(10, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(new WorkflowInstanceDto { Id = 10, Status = InstanceStatus.Suspended }));

        var result = await _controller.Suspend(10, "test-reason");
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        _mockInstanceService.Verify(s => s.SuspendAsync(10, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Resume_Instance_Should_Call_Service_And_Return_Ok()
    {
        _mockInstanceService.Setup(s => s.ResumeAsync(11, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(new WorkflowInstanceDto { Id = 11, Status = InstanceStatus.Running }));

        var result = await _controller.Resume(11);
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        _mockInstanceService.Verify(s => s.ResumeAsync(11, It.IsAny<CancellationToken>()), Times.Once);
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

        var httpContext = new DefaultHttpContext { User = principal };
        httpContext.Request.Headers["X-Tenant-ID"] = "1";

        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }
}
