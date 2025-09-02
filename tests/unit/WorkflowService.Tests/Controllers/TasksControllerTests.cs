using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using WorkflowService.Controllers;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using DTOs.Workflow;
using DTOs.Common;
using DTOs.Workflow.Enums;
using WorkflowService.Domain.Models;
using Microsoft.Extensions.Logging; // ✅ Added
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus; // ✅ Disambiguate TaskStatus

namespace WorkflowService.Tests.Controllers;

public class TasksControllerTests : TestBase
{
    private readonly Mock<ITaskService> _taskService = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<ILogger<TasksController>> _logger = new();
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _controller = new TasksController(
            DbContext,
            _logger.Object,
            _taskService.Object,
            _uow.Object);

        SetUserContext(_controller);
    }

    private static void SetUserContext(ControllerBase controller)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("permission", "workflow.view_tasks"),
            new Claim("permission", "workflow.claim_tasks"),
            new Claim("permission", "workflow.complete_tasks"),
            new Claim("permission", "workflow.admin")
        };
        var ctx = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
        };
        controller.ControllerContext = new ControllerContext { HttpContext = ctx };
    }

    private WorkflowTask SeedTask(WorkflowTaskStatus status = WorkflowTaskStatus.Created, int? assignedUserId = null)
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "WF",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();

        var inst = new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowInstances.Add(inst);
        DbContext.SaveChanges();

        var task = new WorkflowTask
        {
            TenantId = 1,
            WorkflowInstanceId = inst.Id,
            NodeId = "n1",
            TaskName = "Task 1",
            NodeType = "human",
            Status = status,
            AssignedToUserId = assignedUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowTasks.Add(task);
        DbContext.SaveChanges();
        return task;
    }

    [Fact]
    public async Task GetTasks_ShouldReturnOk()
    {
        // Seed 1 task directly (controller queries DbContext)
        SeedTask();

        var result = await _controller.GetTasks();
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        var payload = ok!.Value as ApiResponseDto<List<WorkflowTaskDto>>;
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetMyTasks_ShouldReturnOk()
    {
        _taskService.Setup(s => s.GetMyTasksListAsync(null, true, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<List<TaskSummaryDto>>.SuccessResult(
                new List<TaskSummaryDto>
                {
                    new() { Id = 10, TaskName = "Mine", Status = WorkflowTaskStatus.Created }
                }));

        var result = await _controller.GetMyTasks(null, true, true);
        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
    }

    [Fact]
    public async Task GetMyTasks_Failure_ShouldReturnBadRequest()
    {
        _taskService.Setup(s => s.GetMyTasksListAsync(null, true, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<List<TaskSummaryDto>>.ErrorResult("err"));

        var result = await _controller.GetMyTasks(null, true, true);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetMyTaskSummary_ShouldReturnOk()
    {
        _taskService.Setup(s => s.GetMyTaskCountsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<TaskCountsDto>.SuccessResult(new TaskCountsDto { Available = 1 }));

        var result = await _controller.GetMyTaskSummary(default);
        (result.Result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task GetTask_Found_ShouldReturnOk()
    {
        _taskService.Setup(s => s.GetTaskByIdAsync(123, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.SuccessResult(new WorkflowTaskDto { Id = 123 }));

        var result = await _controller.GetTask(123);
        (result.Result as OkObjectResult).Should().NotBeNull();
    }

    [Fact]
    public async Task GetTask_NotFound_ShouldReturnNotFound()
    {
        _taskService.Setup(s => s.GetTaskByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.ErrorResult("not found"));

        var result = await _controller.GetTask(999);
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ClaimTask_Success_ShouldCommit()
    {
        _taskService.Setup(s => s.ClaimTaskAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.SuccessResult(new WorkflowTaskDto { Id = 5 }));

        var result = await _controller.ClaimTask(5);
        (result.Result as OkObjectResult).Should().NotBeNull();
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ClaimTask_Failure_ShouldNotCommit()
    {
        _taskService.Setup(s => s.ClaimTaskAsync(6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.ErrorResult("bad"));

        var result = await _controller.ClaimTask(6);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CompleteTask_Success_ShouldCommit()
    {
        _taskService.Setup(s => s.CompleteTaskAsync(7, It.IsAny<CompleteTaskRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.SuccessResult(new WorkflowTaskDto { Id = 7 }));

        var result = await _controller.CompleteTask(7, new CompleteTaskRequestDto { CompletionData = "{}" });
        (result.Result as OkObjectResult).Should().NotBeNull();
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CompleteTask_Failure_ShouldNotCommit()
    {
        _taskService.Setup(s => s.CompleteTaskAsync(8, It.IsAny<CompleteTaskRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.ErrorResult("bad"));

        var result = await _controller.CompleteTask(8, new CompleteTaskRequestDto { CompletionData = "{}" });
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignTask_Success_ShouldCommit()
    {
        _taskService.Setup(s => s.AssignTaskAsync(9, It.IsAny<AssignTaskRequestDto>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.SuccessResult(new WorkflowTaskDto { Id = 9 }));

        var result = await _controller.AssignTask(9, new AssignTaskRequestDto { AssignedToUserId = 2 });
        (result.Result as OkObjectResult).Should().NotBeNull();
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AssignTask_Failure_ShouldNotCommit()
    {
        _taskService.Setup(s => s.AssignTaskAsync(10, It.IsAny<AssignTaskRequestDto>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.ErrorResult("bad"));

        var result = await _controller.AssignTask(10, new AssignTaskRequestDto { AssignedToUserId = 2 });
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CancelTask_Success_ShouldCommit()
    {
        _taskService.Setup(s => s.CancelTaskAsync(11, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.SuccessResult(new WorkflowTaskDto { Id = 11 }));

        var result = await _controller.CancelTask(11);
        (result.Result as OkObjectResult).Should().NotBeNull();
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelTask_Failure_ShouldNotCommit()
    {
        _taskService.Setup(s => s.CancelTaskAsync(12, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.ErrorResult("bad"));

        var result = await _controller.CancelTask(12);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReleaseTask_Success_ShouldCommit()
    {
        _taskService.Setup(s => s.ReleaseTaskAsync(13, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.SuccessResult(new WorkflowTaskDto { Id = 13 }));

        var result = await _controller.ReleaseTask(13);
        (result.Result as OkObjectResult).Should().NotBeNull();
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReleaseTask_Failure_ShouldNotCommit()
    {
        _taskService.Setup(s => s.ReleaseTaskAsync(14, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponseDto<WorkflowTaskDto>.ErrorResult("bad"));

        var result = await _controller.ReleaseTask(14);
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
