using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using WorkflowService.Controllers;
using WorkflowService.Persistence;
using DTOs.Common;
using DTOs.Workflow;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;
using WorkflowService.Engine.Interfaces; // CHANGED: runtime instead of execution
using Contracts.Services;

namespace WorkflowService.Tests.Controllers;

public class TasksControllerTests : TestBase
{
    private readonly Mock<ILogger<TasksController>> _mockLogger;
    private readonly Mock<IWorkflowRuntime> _mockRuntime;
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _mockLogger = CreateMockLogger<TasksController>();
        _mockRuntime = new Mock<IWorkflowRuntime>();
        _mockTenantProvider = new Mock<ITenantProvider>();
        _mockTenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);

        _controller = new TasksController(
            DbContext,
            _mockLogger.Object,
            _mockRuntime.Object,
            _mockTenantProvider.Object);

        SetupControllerContext();
    }

    [Fact]
    public async Task GetTasks_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        await SeedTestTasksAsync();

        // Act - Use actual controller method signature
        var result = await _controller.GetTasks(
            mine: true,
            mineIncludeRoles: false,
            mineIncludeUnassigned: false,
            status: WorkflowTaskStatus.Created,
            page: 1,
            pageSize: 10);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ClaimTask_ValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Created);
        var request = new ClaimTaskRequestDto
        {
            ClaimNotes = "Taking this task"
        };

        // Act - Use correct method signature
        var result = await _controller.ClaimTask(task.Id, request);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task CompleteTask_ValidRequest_ShouldInvokeRuntime()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Claimed, assignedUserId: 1);
        var request = new CompleteTaskRequestDto
        {
            CompletionData = """{"approved": true}""",
            CompletionNotes = "Task completed successfully"
        };

        // Act
        var result = await _controller.CompleteTask(task.Id, request);

        // Assert
        result.Should().NotBeNull();
        var actionResult = result.Result as OkObjectResult;
        actionResult.Should().NotBeNull();
        actionResult!.StatusCode.Should().Be(200);
        _mockRuntime.Verify(r => r.CompleteTaskAsync(task.Id, It.IsAny<string>(), 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    private async Task SeedTestTasksAsync()
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
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            CurrentNodeIds = """["task1"]""",
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };

        var task = new WorkflowService.Domain.Models.WorkflowTask
        {
            TenantId = 1,
            WorkflowInstanceId = instance.Id,
            NodeId = "task1",
            TaskName = "Test Task",
            Status = WorkflowTaskStatus.Created,
            Data = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        definition.Instances = new List<WorkflowService.Domain.Models.WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;
        instance.Tasks = new List<WorkflowService.Domain.Models.WorkflowTask> { task };

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.Add(instance);
        DbContext.WorkflowTasks.Add(task);
        await DbContext.SaveChangesAsync();
    }

    private async Task<WorkflowService.Domain.Models.WorkflowTask> CreateTestTaskAsync(
        WorkflowTaskStatus status,
        int? assignedUserId = null)
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
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            CurrentNodeIds = """["task1"]""",
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };

        var task = new WorkflowService.Domain.Models.WorkflowTask
        {
            TenantId = 1,
            WorkflowInstanceId = instance.Id,
            NodeId = "test_task",
            TaskName = "Test Task",
            Status = status,
            AssignedToUserId = assignedUserId,
            Data = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (status == WorkflowTaskStatus.Claimed && assignedUserId.HasValue)
            task.ClaimedAt = DateTime.UtcNow;

        definition.Instances = new List<WorkflowService.Domain.Models.WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;
        instance.Tasks = new List<WorkflowService.Domain.Models.WorkflowTask> { task };

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.Add(instance);
        DbContext.WorkflowTasks.Add(task);
        await DbContext.SaveChangesAsync();

        return task;
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
