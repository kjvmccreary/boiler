using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using WorkflowService.Services;
using DTOs.Workflow;
using AutoMapper;
using Contracts.Services;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Services.Interfaces;
using DTOs.Workflow.Enums;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Tests.Services;

public class TaskServiceTests : TestBase
{
    private readonly TaskService _taskService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IWorkflowRuntime> _mockRuntime;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly Mock<ILogger<TaskService>> _mockLogger;
    private readonly Mock<IUserContext> _mockUserContext;
    private readonly Mock<ITaskNotificationDispatcher> _mockTaskNotifier;

    public TaskServiceTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockRuntime = new Mock<IWorkflowRuntime>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _mockLogger = CreateMockLogger<TaskService>();
        _mockUserContext = new Mock<IUserContext>();
        _mockTaskNotifier = new Mock<ITaskNotificationDispatcher>();

        // Default mapper behavior for any WorkflowTask -> WorkflowTaskDto
        _mockMapper
            .Setup(m => m.Map<WorkflowTaskDto>(It.IsAny<WorkflowService.Domain.Models.WorkflowTask>()))
            .Returns((WorkflowService.Domain.Models.WorkflowTask t) => new WorkflowTaskDto
            {
                Id = t.Id,
                WorkflowInstanceId = t.WorkflowInstanceId,
                NodeId = t.NodeId,
                TaskName = t.TaskName,
                Status = t.Status,
                AssignedToUserId = t.AssignedToUserId,
                AssignedToRole = t.AssignedToRole,
                DueDate = t.DueDate,
                ClaimedAt = t.ClaimedAt,
                CompletedAt = t.CompletedAt,
                CompletionData = t.CompletionData,
                ErrorMessage = t.ErrorMessage,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            });

        // Event publisher mocks (must return a non-null Task)
        _mockEventPublisher
            .Setup(p => p.PublishTaskAssignedAsync(It.IsAny<WorkflowService.Domain.Models.WorkflowTask>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Notification dispatcher
        _mockTaskNotifier.Setup(x => x.NotifyUserAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockTaskNotifier.Setup(x => x.NotifyTenantAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // User & tenant context
        _mockUserContext.Setup(x => x.UserId).Returns(1);
        _mockUserContext.Setup(x => x.Roles).Returns(new[] { "Approver", "QA" });

        _taskService = new TaskService(
            DbContext,
            _mockMapper.Object,
            MockTenantProvider.Object,
            _mockRuntime.Object,
            _mockEventPublisher.Object,
            _mockLogger.Object,
            _mockUserContext.Object,
            _mockTaskNotifier.Object);
    }

    [Fact]
    public async Task GetMyTasksAsync_ShouldReturnUserTasks()
    {
        // Arrange
        await SeedTestDataAsync();
        var request = new GetTasksRequestDto { Page = 1, PageSize = 10 };

        // Act
        var result = await _taskService.GetMyTasksAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task ClaimTaskAsync_ValidTask_ShouldClaimSuccessfully()
    {
        // Arrange
        var task = await CreateTestTaskAsync();

        // Act
        var result = await _taskService.ClaimTaskAsync(task.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();

        var updatedTask = await DbContext.WorkflowTasks.FindAsync(task.Id);
        updatedTask!.Status.Should().Be(WorkflowTaskStatus.Claimed);
        updatedTask.AssignedToUserId.Should().Be(1);
    }

    [Fact]
    public async Task CompleteTaskAsync_ValidTask_ShouldCompleteSuccessfully()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Claimed, assignedUserId: 1);
        var request = new CompleteTaskRequestDto
        {
            CompletionData = """{"approved": true}""",
            CompletionNotes = "Task completed"
        };

        _mockRuntime.Setup(r => r.CompleteTaskAsync(task.Id, It.IsAny<string>(), 1, It.IsAny<CancellationToken>(), false))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.CompleteTaskAsync(task.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        // Verify runtime was called
        _mockRuntime.Verify(r => r.CompleteTaskAsync(task.Id, It.IsAny<string>(), 1, It.IsAny<CancellationToken>(), false), Times.Once);
    }

    // ===== ReleaseTask =====

    [Fact]
    public async Task ReleaseTaskAsync_ClaimedByUser_ShouldRelease()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Claimed, assignedUserId: 1);

        // Act
        var result = await _taskService.ReleaseTaskAsync(task.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();

        var updatedTask = await DbContext.WorkflowTasks.FindAsync(task.Id);
        updatedTask!.Status.Should().Be(WorkflowTaskStatus.Created);
        updatedTask.AssignedToUserId.Should().BeNull();
        updatedTask.ClaimedAt.Should().BeNull();
    }

    [Fact]
    public async Task ReleaseTaskAsync_NotAssignedToCurrentUser_ShouldFail()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Claimed, assignedUserId: 2);

        // Act
        var result = await _taskService.ReleaseTaskAsync(task.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not assigned to you");
    }

    [Fact]
    public async Task ReleaseTaskAsync_InvalidState_ShouldFail()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Created);

        // Act
        var result = await _taskService.ReleaseTaskAsync(task.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("cannot be released");
    }

    // ===== ReassignTask =====

    [Fact]
    public async Task ReassignTaskAsync_AssignToUser_ShouldSucceed()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Created);
        var request = new ReassignTaskRequestDto { AssignToUserId = 2, Reason = "Load balancing" };

        // Act
        var result = await _taskService.ReassignTaskAsync(task.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();

        var updatedTask = await DbContext.WorkflowTasks.FindAsync(task.Id);
        updatedTask!.AssignedToUserId.Should().Be(2);
        updatedTask.Status.Should().Be(WorkflowTaskStatus.Assigned);
    }

    [Fact]
    public async Task ReassignTaskAsync_CompletedTask_ShouldFail()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Completed);

        // Act
        var result = await _taskService.ReassignTaskAsync(task.Id, new ReassignTaskRequestDto { AssignToUserId = 2 });

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Cannot reassign");
    }

    // ===== AssignTask =====

    [Fact]
    public async Task AssignTaskAsync_Unassigned_ShouldSucceed()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Created);

        // Act
        var result = await _taskService.AssignTaskAsync(task.Id,
            new AssignTaskRequestDto { AssignedToUserId = 7 }, performedByUserId: 1);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();

        var updatedTask = await DbContext.WorkflowTasks.FindAsync(task.Id);
        updatedTask!.Status.Should().Be(WorkflowTaskStatus.Assigned);
        updatedTask.AssignedToUserId.Should().Be(7);
    }

    [Fact]
    public async Task AssignTaskAsync_AlreadyCompleted_ShouldFail()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Completed);

        // Act
        var result = await _taskService.AssignTaskAsync(task.Id,
            new AssignTaskRequestDto { AssignedToUserId = 7 }, performedByUserId: 1);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Cannot assign task");
    }

    // ===== CancelTask =====

    [Fact]
    public async Task CancelTaskAsync_ActiveTask_ShouldSucceed()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Assigned, assignedUserId: 2);

        // Act
        var result = await _taskService.CancelTaskAsync(task.Id, performedByUserId: 1);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();

        var updatedTask = await DbContext.WorkflowTasks.FindAsync(task.Id);
        updatedTask!.Status.Should().Be(WorkflowTaskStatus.Cancelled);
        updatedTask.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelTaskAsync_AlreadyCompleted_ShouldFail()
    {
        // Arrange
        var task = await CreateTestTaskAsync(WorkflowTaskStatus.Completed);

        // Act
        var result = await _taskService.CancelTaskAsync(task.Id, performedByUserId: 1);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Cannot cancel");
    }

    // ===== GetMyTaskCounts =====

    [Fact]
    public async Task GetMyTaskCountsAsync_ShouldReturnExpectedCounts()
    {
        // Setup tasks with different statuses & due dates
        var def = new WorkflowService.Domain.Models.WorkflowDefinition
        {
            TenantId = 1,
            Name = "Counts WF",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        await DbContext.SaveChangesAsync();

        var inst = new WorkflowService.Domain.Models.WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };
        DbContext.WorkflowInstances.Add(inst);
        await DbContext.SaveChangesAsync();

        var now = DateTime.UtcNow;

        var tasks = new[]
        {
            NewTask(inst.Id, "t1", WorkflowTaskStatus.Created),
            NewTask(inst.Id, "t2", WorkflowTaskStatus.Assigned, assignedUserId: null, role:"Approver"),
            NewTask(inst.Id, "t3", WorkflowTaskStatus.Claimed, assignedUserId:1),
            NewTask(inst.Id, "t4", WorkflowTaskStatus.InProgress, assignedUserId:1),
            NewTask(inst.Id, "t5", WorkflowTaskStatus.Completed, assignedUserId:1, completedAt: now),
            NewTask(inst.Id, "t6", WorkflowTaskStatus.Created, dueDate: now.AddHours(-2)),
            NewTask(inst.Id, "t7", WorkflowTaskStatus.Failed, assignedUserId:1)
        };

        DbContext.WorkflowTasks.AddRange(tasks);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _taskService.GetMyTaskCountsAsync();

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        var counts = result.Data!;

        counts.Available.Should().Be(2);
        counts.AssignedToMyRoles.Should().Be(1);
        counts.AssignedToMe.Should().Be(0);
        counts.Claimed.Should().Be(1);
        counts.InProgress.Should().Be(1);
        counts.CompletedToday.Should().Be(1);
        counts.Overdue.Should().Be(1);
        counts.Failed.Should().Be(1);
    }

    private WorkflowService.Domain.Models.WorkflowTask NewTask(
        int instanceId,
        string nodeId,
        WorkflowTaskStatus status,
        int? assignedUserId = null,
        string? role = null,
        DateTime? dueDate = null,
        DateTime? completedAt = null)
        => new()
        {
            TenantId = 1,
            WorkflowInstanceId = instanceId,
            NodeId = nodeId,
            TaskName = nodeId,
            Status = status,
            NodeType = "human",
            AssignedToUserId = assignedUserId,
            AssignedToRole = role,
            DueDate = dueDate,
            CompletedAt = completedAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ClaimedAt = status == WorkflowTaskStatus.Claimed ? DateTime.UtcNow : null
        };

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
        DbContext.WorkflowDefinitions.Add(definition);
        await DbContext.SaveChangesAsync();

        var instance = new WorkflowService.Domain.Models.WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };
        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();

        var task = new WorkflowService.Domain.Models.WorkflowTask
        {
            TenantId = 1,
            WorkflowInstanceId = instance.Id,
            NodeId = "test_task",
            TaskName = "Test Task",
            Status = WorkflowTaskStatus.Created,
            NodeType = "human",
            Data = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowTasks.Add(task);
        await DbContext.SaveChangesAsync();
    }

    private async Task<WorkflowService.Domain.Models.WorkflowTask> CreateTestTaskAsync(
        WorkflowTaskStatus status = WorkflowTaskStatus.Created,
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
        DbContext.WorkflowDefinitions.Add(definition);
        await DbContext.SaveChangesAsync();

        var instance = new WorkflowService.Domain.Models.WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };
        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();

        var task = new WorkflowService.Domain.Models.WorkflowTask
        {
            TenantId = 1,
            WorkflowInstanceId = instance.Id,
            NodeId = "test_task",
            TaskName = "Test Task",
            Status = status,
            NodeType = "human",
            AssignedToUserId = assignedUserId,
            Data = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ClaimedAt = status == WorkflowTaskStatus.Claimed ? DateTime.UtcNow : null,
            CompletedAt = status == WorkflowTaskStatus.Completed ? DateTime.UtcNow : null
        };
        DbContext.WorkflowTasks.Add(task);
        await DbContext.SaveChangesAsync();

        return task;
    }
}
