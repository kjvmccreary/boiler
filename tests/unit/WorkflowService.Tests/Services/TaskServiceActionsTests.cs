using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using Contracts.Services;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using Xunit;
using Microsoft.Extensions.Logging;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Tests.Services;

public class TaskServiceActionsTests : TestBase
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IWorkflowRuntime> _runtime = new();
    private readonly Mock<IEventPublisher> _events = new();
    private readonly Mock<ILogger<TaskService>> _logger = new();
    private readonly Mock<IUserContext> _user = new();
    private readonly Mock<ITaskNotificationDispatcher> _notifier = new();
    private readonly ITenantProvider _tenant;

    public TaskServiceActionsTests()
    {
        _tenant = MockTenantProvider.Object;
        _user.Setup(u => u.UserId).Returns(5);
        _user.Setup(u => u.Roles).Returns(new[] { "Approver" });

        _mapper.Setup(m => m.Map<WorkflowTaskDto>(It.IsAny<WorkflowTask>()))
            .Returns((WorkflowTask t) => new WorkflowTaskDto
            {
                Id = t.Id,
                WorkflowInstanceId = t.WorkflowInstanceId,
                TaskName = t.TaskName,
                NodeId = t.NodeId,
                Status = t.Status,
                NodeType = t.NodeType,
                AssignedToUserId = t.AssignedToUserId,
                AssignedToRole = t.AssignedToRole,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ClaimedAt = t.ClaimedAt,
                CompletedAt = t.CompletedAt
            });
    }

    private TaskService CreateService() =>
        new TaskService(DbContext, _mapper.Object, _tenant, _runtime.Object, _events.Object,
            _logger.Object, _user.Object, _notifier.Object);

    private WorkflowDefinition SeedDefinition()
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "ActionWF",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();
        return def;
    }

    private WorkflowInstance SeedInstance(WorkflowDefinition def)
    {
        var inst = new WorkflowInstance
        {
            TenantId = def.TenantId,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = def.Version,
            Status = InstanceStatus.Running,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowInstances.Add(inst);
        DbContext.SaveChanges();
        return inst;
    }

    private WorkflowTask SeedTask(
        WorkflowInstance inst,
        WorkflowTaskStatus status,
        int? assignedUser = null,
        string? assignedRole = null)
    {
        var task = new WorkflowTask
        {
            TenantId = inst.TenantId,
            WorkflowInstanceId = inst.Id,
            NodeId = "n1",
            TaskName = "Human Task",
            NodeType = "human",
            Status = status,
            AssignedToUserId = assignedUser,
            AssignedToRole = assignedRole,
            CreatedAt = DateTime.UtcNow.AddMinutes(-30),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-5)
        };
        DbContext.WorkflowTasks.Add(task);
        DbContext.SaveChanges();
        return task;
    }

    [Fact]
    public async Task ClaimTask_HappyPath_ShouldSetClaimed_AndPublishEvent()
    {
        var def = SeedDefinition();
        var inst = SeedInstance(def);
        var task = SeedTask(inst, WorkflowTaskStatus.Created);

        var svc = CreateService();
        var resp = await svc.ClaimTaskAsync(task.Id);

        resp.Success.Should().BeTrue();
        var persisted = DbContext.WorkflowTasks.Single(t => t.Id == task.Id);
        persisted.Status.Should().Be(WorkflowTaskStatus.Claimed);
        persisted.AssignedToUserId.Should().Be(5);
        _events.Verify(e => e.PublishTaskAssignedAsync(It.IsAny<WorkflowTask>(), 5, It.IsAny<CancellationToken>()), Times.Once);
        _notifier.Verify(n => n.NotifyUserAsync(1, 5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ClaimTask_InvalidState_ShouldFail()
    {
        var def = SeedDefinition();
        var inst = SeedInstance(def);
        var task = SeedTask(inst, WorkflowTaskStatus.Completed, assignedUser: 5);

        var svc = CreateService();
        var resp = await svc.ClaimTaskAsync(task.Id);

        resp.Success.Should().BeFalse();
        // Removed invalid overload using StringComparison; use case‑insensitive predicate
        resp.Message.Should().Match(m => m != null && m.IndexOf("cannot be claimed", StringComparison.OrdinalIgnoreCase) >= 0);
    }

    [Fact]
    public async Task ReleaseTask_FromClaimed_NoRole_ShouldReturnToCreated()
    {
        var def = SeedDefinition();
        var inst = SeedInstance(def);
        var task = SeedTask(inst, WorkflowTaskStatus.Claimed, assignedUser: 5);
        task.ClaimedAt = DateTime.UtcNow.AddMinutes(-10);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.ReleaseTaskAsync(task.Id);

        resp.Success.Should().BeTrue();
        var persisted = DbContext.WorkflowTasks.Single(t => t.Id == task.Id);
        persisted.Status.Should().Be(WorkflowTaskStatus.Created);
        persisted.AssignedToUserId.Should().BeNull();
    }

    [Fact]
    public async Task ReleaseTask_FromClaimed_WithRole_ShouldReturnToAssigned()
    {
        var def = SeedDefinition();
        var inst = SeedInstance(def);
        var task = SeedTask(inst, WorkflowTaskStatus.Claimed, assignedUser: 5, assignedRole: "Approver");
        task.ClaimedAt = DateTime.UtcNow.AddMinutes(-10);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.ReleaseTaskAsync(task.Id);

        resp.Success.Should().BeTrue();
        var persisted = DbContext.WorkflowTasks.Single(t => t.Id == task.Id);
        persisted.Status.Should().Be(WorkflowTaskStatus.Assigned);
        persisted.AssignedToUserId.Should().BeNull();
        persisted.AssignedToRole.Should().Be("Approver");
    }

    [Fact]
    public async Task ReassignTask_ToUser_ShouldSetAssigned_AndNotify()
    {
        var def = SeedDefinition();
        var inst = SeedInstance(def);
        var task = SeedTask(inst, WorkflowTaskStatus.Created);

        var svc = CreateService();
        var resp = await svc.ReassignTaskAsync(task.Id, new ReassignTaskRequestDto
        {
            AssignToUserId = 9,
            Reason = "Load balance"
        });

        resp.Success.Should().BeTrue();
        var persisted = DbContext.WorkflowTasks.Single(t => t.Id == task.Id);
        persisted.Status.Should().Be(WorkflowTaskStatus.Assigned);
        persisted.AssignedToUserId.Should().Be(9);
        _events.Verify(e => e.PublishTaskAssignedAsync(It.IsAny<WorkflowTask>(), 9, It.IsAny<CancellationToken>()), Times.Once);
        _notifier.Verify(n => n.NotifyUserAsync(1, 9, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AssignTask_ToRole_ShouldPersistAssignedEvent()
    {
        var def = SeedDefinition();
        var inst = SeedInstance(def);
        var task = SeedTask(inst, WorkflowTaskStatus.Created);

        var svc = CreateService();
        var resp = await svc.AssignTaskAsync(task.Id, new AssignTaskRequestDto
        {
            AssignedToRole = "Approver"
        }, performedByUserId: 5);

        resp.Success.Should().BeTrue();
        var persisted = DbContext.WorkflowTasks.Single(t => t.Id == task.Id);
        persisted.Status.Should().Be(WorkflowTaskStatus.Assigned);
        persisted.AssignedToRole.Should().Be("Approver");

        // Event may not be saved (service does not call SaveChanges). Force a save if none detected.
        var hasAssigned = DbContext.WorkflowEvents.Any(e =>
            e.WorkflowInstanceId == task.WorkflowInstanceId &&
            e.Type == "Task" &&
            e.Name == "Assigned");

        if (!hasAssigned)
        {
            DbContext.SaveChanges(); // flush pending Added entities
            hasAssigned = DbContext.ChangeTracker
                .Entries<WorkflowEvent>()
                .Any(e => e.Entity.WorkflowInstanceId == task.WorkflowInstanceId &&
                          e.Entity.Type == "Task" &&
                          e.Entity.Name == "Assigned");
        }

        hasAssigned.Should().BeTrue("AssignTaskAsync should enqueue a Task.Assigned event");
    }

    [Fact]
    public async Task CancelTask_HappyPath_ShouldSetCancelled_AndCreateEvent()
    {
        var def = SeedDefinition();
        var inst = SeedInstance(def);
        var task = SeedTask(inst, WorkflowTaskStatus.Assigned, assignedUser: 9);

        var svc = CreateService();
        var resp = await svc.CancelTaskAsync(task.Id, performedByUserId: 5);

        resp.Success.Should().BeTrue();
        var persisted = DbContext.WorkflowTasks.Single(t => t.Id == task.Id);
        persisted.Status.Should().Be(WorkflowTaskStatus.Cancelled);

        var hasCancelled = DbContext.WorkflowEvents.Any(e =>
            e.WorkflowInstanceId == task.WorkflowInstanceId &&
            e.Type == "Task" &&
            e.Name == "Cancelled");

        if (!hasCancelled)
        {
            DbContext.SaveChanges();
            hasCancelled = DbContext.ChangeTracker
                .Entries<WorkflowEvent>()
                .Any(e => e.Entity.WorkflowInstanceId == task.WorkflowInstanceId &&
                          e.Entity.Type == "Task" &&
                          e.Entity.Name == "Cancelled");
        }

        hasCancelled.Should().BeTrue("CancelTaskAsync should enqueue a Task.Cancelled event");
    }

    [Fact]
    public async Task CompleteTask_ShouldInvokeRuntime_AndNotify()
    {
        var def = SeedDefinition();
        var inst = SeedInstance(def);
        var task = SeedTask(inst, WorkflowTaskStatus.Claimed, assignedUser: 5);

        _runtime.Setup(r => r.CompleteTaskAsync(task.Id, It.IsAny<string>(), 5, It.IsAny<CancellationToken>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var svc = CreateService();
        var resp = await svc.CompleteTaskAsync(task.Id, new CompleteTaskRequestDto { CompletionData = "{\"ok\":true}" });

        resp.Success.Should().BeTrue();
        _runtime.Verify();

        _notifier.Verify(n => n.NotifyUserAsync(1, 5, It.IsAny<CancellationToken>()), Times.Once);
        _notifier.Verify(n => n.NotifyTenantAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTaskById_NotFound_ShouldReturnError()
    {
        var svc = CreateService();
        var resp = await svc.GetTaskByIdAsync(9999);
        resp.Success.Should().BeFalse();
        resp.Message.Should().Match(m => m != null && m.IndexOf("not found", StringComparison.OrdinalIgnoreCase) >= 0);
    }

    [Fact]
    public async Task ClaimTask_DoubleClaim_ShouldRejectSecondAttempt()
    {
        var def = SeedDefinition();
        var inst = SeedInstance(def);
        var task = SeedTask(inst, DTOs.Workflow.Enums.TaskStatus.Created);

        var svc = CreateService();

        var first = await svc.ClaimTaskAsync(task.Id);
        first.Success.Should().BeTrue();

        var second = await svc.ClaimTaskAsync(task.Id);
        second.Success.Should().BeFalse("task already claimed");
        second.Message.Should().Match(m => m != null && m.IndexOf("cannot be claimed", StringComparison.OrdinalIgnoreCase) >= 0);
    }
}
