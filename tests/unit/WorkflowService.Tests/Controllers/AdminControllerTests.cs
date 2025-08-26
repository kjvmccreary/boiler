using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using WorkflowService.Controllers;
using WorkflowService.Persistence;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace WorkflowService.Tests.Controllers;

public class AdminControllerTests : TestBase
{
    private readonly Mock<ILogger<AdminController>> _mockLogger;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        _mockLogger = CreateMockLogger<AdminController>();
        
        // Use actual DbContext from TestBase
        _controller = new AdminController(
            DbContext,
            _mockLogger.Object);

        SetupControllerContext();
    }

    [Fact]
    public async Task GetWorkflowStats_ShouldReturnOkResult()
    {
        // Arrange
        await SeedTestDataAsync();

        // Act - Use actual controller method name
        var result = await _controller.GetWorkflowStats();

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
        
        var responseData = actionResult.Value as ApiResponseDto<WorkflowStatsDto>;
        responseData.Should().NotBeNull();
        responseData!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task RetryInstance_FailedInstance_ShouldReturnOkResult()
    {
        // Arrange
        var instance = await CreateFailedInstanceAsync();
        var request = new RetryInstanceRequestDto
        {
            RetryReason = "Fixing network issue"
        };

        // Act
        var result = await _controller.RetryInstance(instance.Id, request);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task MoveInstanceToNode_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var instance = await CreateRunningInstanceAsync();
        var request = new MoveToNodeRequestDto
        {
            TargetNodeId = "task2",
            Reason = "Skipping problematic step"
        };

        // Act - Use actual controller method name
        var result = await _controller.MoveInstanceToNode(instance.Id, request);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
    }

    private async Task SeedTestDataAsync()
    {
        var definition = new WorkflowService.Domain.Models.WorkflowDefinition
        {
            TenantId = 1,
            Name = "Test Workflow",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var instance = new WorkflowService.Domain.Models.WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Completed,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        definition.Instances = new List<WorkflowService.Domain.Models.WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();
    }

    private async Task<WorkflowService.Domain.Models.WorkflowInstance> CreateFailedInstanceAsync()
    {
        var definition = new WorkflowService.Domain.Models.WorkflowDefinition
        {
            TenantId = 1,
            Name = "Failed Workflow",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var instance = new WorkflowService.Domain.Models.WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Failed,
            CurrentNodeIds = """["task1"]""",
            Context = "{}",
            ErrorMessage = "Task execution failed",
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        };

        definition.Instances = new List<WorkflowService.Domain.Models.WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();

        return instance;
    }

    private async Task<WorkflowService.Domain.Models.WorkflowInstance> CreateRunningInstanceAsync()
    {
        var definition = new WorkflowService.Domain.Models.WorkflowDefinition
        {
            TenantId = 1,
            Name = "Running Workflow",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var instance = new WorkflowService.Domain.Models.WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = """["task1"]""",
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };

        definition.Instances = new List<WorkflowService.Domain.Models.WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();

        return instance;
    }

    private void SetupControllerContext()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("tenantId", "1"),
            new Claim(ClaimTypes.Email, "admin@example.com"),
            new Claim("permissions", "workflow.admin")
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
