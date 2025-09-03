using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Services;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces; // ADDED: Provides IEventPublisher, IUserContext, ITaskNotificationDispatcher
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;
using Xunit;

namespace WorkflowService.Tests.Services;

public class TaskServicePhase3Tests : TestBase
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IWorkflowRuntime> _runtime = new();
    private readonly Mock<IEventPublisher> _events = new();
    private readonly Mock<ILogger<TaskService>> _logger = new();
    private readonly Mock<IUserContext> _user = new();
    private readonly Mock<ITaskNotificationDispatcher> _notifier = new();

    private TaskService CreateService(ITenantProvider? tenant = null) =>
        new TaskService(DbContext, _mapper.Object, tenant ?? MockTenantProvider.Object,
            _runtime.Object, _events.Object, _logger.Object, _user.Object, _notifier.Object);

    public TaskServicePhase3Tests()
    {
        _user.Setup(u => u.UserId).Returns(1);
        _user.Setup(u => u.Roles).Returns(new[] { "Approver", "QA" });

        _mapper.Setup(m => m.Map<WorkflowTaskDto>(It.IsAny<WorkflowService.Domain.Models.WorkflowTask>()))
            .Returns((WorkflowService.Domain.Models.WorkflowTask t) => new WorkflowTaskDto
            {
                Id = t.Id,
                WorkflowInstanceId = t.WorkflowInstanceId,
                TaskName = t.TaskName,
                NodeId = t.NodeId,
                Status = t.Status,
                AssignedToUserId = t.AssignedToUserId,
                AssignedToRole = t.AssignedToRole,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CompletedAt = t.CompletedAt,
                ClaimedAt = t.ClaimedAt
            });

        _events.Setup(e => e.PublishTaskAssignedAsync(It.IsAny<WorkflowService.Domain.Models.WorkflowTask>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _notifier.Setup(n => n.NotifyUserAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _notifier.Setup(n => n.NotifyTenantAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    // ---------- Helpers ----------

    private (WorkflowService.Domain.Models.WorkflowDefinition def, WorkflowService.Domain.Models.WorkflowInstance inst)
        SeedDefinitionAndInstance(int tenantId = 1)
    {
        var def = new WorkflowService.Domain.Models.WorkflowDefinition
        {
            TenantId = tenantId,
            Name = $"WF_{tenantId}",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();

        var inst = new WorkflowService.Domain.Models.WorkflowInstance
        {
            TenantId = tenantId,
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
        return (def, inst);
    }

    private WorkflowService.Domain.Models.WorkflowTask AddTask(
        int tenantId,
        int instanceId,
        string name,
        WorkflowTaskStatus status,
        int? userId = null,
        string? role = null,
        DateTime? due = null,
        DateTime? completed = null)
    {
        var t = new WorkflowService.Domain.Models.WorkflowTask
        {
            TenantId = tenantId,
            WorkflowInstanceId = instanceId,
            NodeId = name,
            TaskName = name,
            NodeType = "human",
            Status = status,
            AssignedToUserId = userId,
            AssignedToRole = role,
            DueDate = due,
            CreatedAt = DateTime.UtcNow.AddMinutes(-30),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-5),
            ClaimedAt = status == WorkflowTaskStatus.Claimed ? DateTime.UtcNow.AddMinutes(-10) : null,
            CompletedAt = completed
        };
        DbContext.WorkflowTasks.Add(t);
        return t;
    }

    // ---------- ClaimTask Negative Paths ----------

    [Fact]
    public async Task ClaimTaskAsync_InvalidState_ShouldFail()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        var t = AddTask(1, inst.Id, "c1", WorkflowTaskStatus.Completed);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.ClaimTaskAsync(t.Id);

        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("cannot be claimed");
    }

    [Fact]
    public async Task ClaimTaskAsync_AssignedToAnotherUser_ShouldFail()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        var t = AddTask(1, inst.Id, "c2", WorkflowTaskStatus.Assigned, userId: 99);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.ClaimTaskAsync(t.Id);

        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("another user");
    }

    [Fact]
    public async Task ClaimTaskAsync_RoleMismatch_ShouldFail()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        var t = AddTask(1, inst.Id, "c3", WorkflowTaskStatus.Assigned, role: "Finance");
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.ClaimTaskAsync(t.Id);

        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("lack the required role");
    }

    // ---------- CompleteTask Error Path ----------

    [Fact]
    public async Task CompleteTaskAsync_RuntimeThrows_ShouldReturnError()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        var t = AddTask(1, inst.Id, "cmp1", WorkflowTaskStatus.Claimed, userId: 1);
        DbContext.SaveChanges();

        _runtime.Setup(r => r.CompleteTaskAsync(t.Id, It.IsAny<string>(), 1, It.IsAny<CancellationToken>(), false))
            .ThrowsAsync(new Exception("runtime failure"));

        var svc = CreateService();
        var resp = await svc.CompleteTaskAsync(t.Id, new CompleteTaskRequestDto { CompletionData = "{}" });

        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("Failed to complete");
    }

    // ---------- AssignTask Additional Paths ----------

    [Fact]
    public async Task AssignTaskAsync_NotFound_ShouldReturnError()
    {
        var svc = CreateService();
        var resp = await svc.AssignTaskAsync(9999, new AssignTaskRequestDto { AssignedToUserId = 2 }, 1);
        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("not found");
    }

    // ---------- CancelTask Additional Paths ----------

    [Fact]
    public async Task CancelTaskAsync_NotFound_ShouldReturnError()
    {
        var svc = CreateService();
        var resp = await svc.CancelTaskAsync(9999, 1);
        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("not found");
    }

    // ---------- Reassign Variations ----------

    [Fact]
    public async Task ReassignTaskAsync_RoleAssignment_ShouldSucceed()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        var t = AddTask(1, inst.Id, "r1", WorkflowTaskStatus.Created);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.ReassignTaskAsync(t.Id, new ReassignTaskRequestDto { AssignToRole = "Approver", Reason = "Role routing" });

        resp.Success.Should().BeTrue();
        resp.Data!.AssignedToRole.Should().Be("Approver");
        resp.Data.Status.Should().Be(WorkflowTaskStatus.Assigned);
    }

    [Fact]
    public async Task ReassignTaskAsync_ClearAssignment_ShouldSetCreated()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        var t = AddTask(1, inst.Id, "r2", WorkflowTaskStatus.Assigned, userId: 5);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.ReassignTaskAsync(t.Id, new ReassignTaskRequestDto
        {
            AssignToUserId = null,
            AssignToRole = null,
            Reason = "Unassign"
        });

        resp.Success.Should().BeTrue();
        resp.Data!.AssignedToUserId.Should().BeNull();
        resp.Data.AssignedToRole.Should().BeNull();
        resp.Data.Status.Should().Be(WorkflowTaskStatus.Created);
    }

    // ---------- MyTasksList Filtering ----------

    [Fact]
    public async Task GetMyTasksListAsync_FilterByStatus_ShouldReturnOnlyThatStatus()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        AddTask(1, inst.Id, "m1", WorkflowTaskStatus.Created);
        AddTask(1, inst.Id, "m2", WorkflowTaskStatus.Claimed, userId: 1);
        AddTask(1, inst.Id, "m3", WorkflowTaskStatus.InProgress, userId: 1);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.GetMyTasksListAsync(WorkflowTaskStatus.Claimed, includeRoleTasks: true, includeUnassigned: true);

        resp.Success.Should().BeTrue();
        resp.Data!.Should().OnlyContain(t => t.Status == WorkflowTaskStatus.Claimed);
    }

    [Fact]
    public async Task GetMyTasksListAsync_IncludesExpectedAccessibleTasks()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        // Accessible by user (assigned to role)
        AddTask(1, inst.Id, "rt", WorkflowTaskStatus.Assigned, role: "Approver");
        // Unassigned available
        AddTask(1, inst.Id, "ua", WorkflowTaskStatus.Created);
        // Claimed by user
        AddTask(1, inst.Id, "cu", WorkflowTaskStatus.Claimed, userId: 1);
        // Assigned to other user (should not appear in 'my' list unless claimed)
        AddTask(1, inst.Id, "ou", WorkflowTaskStatus.Assigned, userId: 42);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.GetMyTasksListAsync(status: null, includeRoleTasks: true, includeUnassigned: true);

        resp.Success.Should().BeTrue();
        var names = resp.Data!.Select(t => t.TaskName).ToList();
        names.Should().Contain(new[] { "rt", "ua", "cu" });
        names.Should().NotContain("ou");
    }

    // ---------- DueAfter Filter & Pagination Beyond Range ----------

    [Fact]
    public async Task GetAllTasksAsync_FilterByDueAfter_ShouldReturnExpected()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        AddTask(1, inst.Id, "d1", WorkflowTaskStatus.Created, due: DateTime.UtcNow.AddHours(1));
        AddTask(1, inst.Id, "d2", WorkflowTaskStatus.Created, due: DateTime.UtcNow.AddHours(5));
        AddTask(1, inst.Id, "d3", WorkflowTaskStatus.Created, due: DateTime.UtcNow.AddHours(9));
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.GetAllTasksAsync(new GetTasksRequestDto
        {
            DueAfter = DateTime.UtcNow.AddHours(4),
            Page = 1,
            PageSize = 10,
            SortBy = "dueDate"
        });

        resp.Success.Should().BeTrue();
        resp.Data!.Items.Should().HaveCount(2);
        resp.Data.Items.Select(i => i.TaskName).Should().BeEquivalentTo(new[] { "d2", "d3" });
    }

    [Fact]
    public async Task GetAllTasksAsync_PaginationBeyondRange_ShouldReturnEmpty()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        for (int i = 0; i < 4; i++)
            AddTask(1, inst.Id, $"pg{i}", WorkflowTaskStatus.Created);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.GetAllTasksAsync(new GetTasksRequestDto
        {
            Page = 3,
            PageSize = 3,
            SortBy = "createdAt"
        });

        resp.Success.Should().BeTrue();
        resp.Data!.Items.Should().BeEmpty();
        resp.Data.TotalCount.Should().Be(4);
    }

    // ---------- Sort by CompletedAt ----------

    [Fact]
    public async Task GetAllTasksAsync_SortByCompletedAt_ShouldOrderByCompletion()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        AddTask(1, inst.Id, "cA", WorkflowTaskStatus.Completed, completed: DateTime.UtcNow.AddMinutes(-30));
        AddTask(1, inst.Id, "cB", WorkflowTaskStatus.Completed, completed: DateTime.UtcNow.AddMinutes(-10));
        AddTask(1, inst.Id, "cC", WorkflowTaskStatus.Completed, completed: DateTime.UtcNow.AddMinutes(-20));
        DbContext.SaveChanges();

        var svc = CreateService();
        var respAsc = await svc.GetAllTasksAsync(new GetTasksRequestDto
        {
            SortBy = "completedAt",
            SortDescending = false,
            Page = 1,
            PageSize = 10
        });
        var ascNames = respAsc.Data!.Items.Select(i => i.TaskName).ToList();
        ascNames.Should().ContainInOrder("cA", "cC", "cB");

        var respDesc = await svc.GetAllTasksAsync(new GetTasksRequestDto
        {
            SortBy = "completedAt",
            SortDescending = true,
            Page = 1,
            PageSize = 10
        });
        var descNames = respDesc.Data!.Items.Select(i => i.TaskName).ToList();
        descNames.Should().ContainInOrder("cB", "cC", "cA");
    }
}
