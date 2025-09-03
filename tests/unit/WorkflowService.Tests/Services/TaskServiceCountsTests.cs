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

public class TaskServiceCountsTests : TestBase
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

    public TaskServiceCountsTests()
    {
        _user.Setup(u => u.UserId).Returns(5);
        _user.Setup(u => u.Roles).Returns(new[] { "Approver", "QA" });

        _mapper.Setup(m => m.Map<WorkflowTaskDto>(It.IsAny<WorkflowService.Domain.Models.WorkflowTask>()))
            .Returns((WorkflowService.Domain.Models.WorkflowTask t) => new WorkflowTaskDto
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
                CompletedAt = t.CompletedAt,
                ClaimedAt = t.ClaimedAt,
                DueDate = t.DueDate
            });
    }

    private WorkflowInstance SeedInstance()
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "CountsWF",
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
        return inst;
    }

    private WorkflowService.Domain.Models.WorkflowTask AddTask(
        WorkflowInstance inst,
        string idSuffix,
        WorkflowTaskStatus status,
        int? userId = null,
        string? role = null,
        DateTime? due = null,
        DateTime? completed = null,
        DateTime? claimed = null)
    {
        var t = new WorkflowService.Domain.Models.WorkflowTask
        {
            TenantId = inst.TenantId,
            WorkflowInstanceId = inst.Id,
            NodeId = "node_" + idSuffix,
            TaskName = "Task " + idSuffix,
            NodeType = "human",
            Status = status,
            AssignedToUserId = userId,
            AssignedToRole = role,
            DueDate = due,
            CreatedAt = DateTime.UtcNow.AddMinutes(-60),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-5),
            ClaimedAt = claimed,
            CompletedAt = completed
        };
        DbContext.WorkflowTasks.Add(t);
        return t;
    }

    [Fact]
    public async Task GetMyTaskCountsAsync_ShouldReturnExpectedCounts()
    {
        var inst = SeedInstance();
        var now = DateTime.UtcNow;
        var todayStart = now.Date;

        // Available (Created + unassigned)
        AddTask(inst, "avail1", WorkflowTaskStatus.Created);
        // Available (Assigned state but still truly unassigned)
        AddTask(inst, "avail2", WorkflowTaskStatus.Assigned);
        // AssignedToMe (Assigned)
        AddTask(inst, "assigntome1", WorkflowTaskStatus.Assigned, userId: 5);
        // AssignedToMe (Created but directly assigned)
        AddTask(inst, "assigntome2", WorkflowTaskStatus.Created, userId: 5);
        // AssignedToMyRoles (role Approver)
        AddTask(inst, "role1", WorkflowTaskStatus.Assigned, role: "Approver");
        // Claimed
        AddTask(inst, "claimed", WorkflowTaskStatus.Claimed, userId: 5, claimed: now.AddMinutes(-30));
        // InProgress
        AddTask(inst, "inprog", WorkflowTaskStatus.InProgress, userId: 5, claimed: now.AddHours(-1));
        // Completed today
        AddTask(inst, "completedToday", WorkflowTaskStatus.Completed, userId: 7, completed: todayStart.AddHours(1));
        // Overdue (due yesterday, still open)
        AddTask(inst, "overdue", WorkflowTaskStatus.Created, due: now.AddDays(-1));
        // Failed
        AddTask(inst, "failed", WorkflowTaskStatus.Failed, userId: 5);

        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.GetMyTaskCountsAsync();

        resp.Success.Should().BeTrue();
        resp.Data.Should().NotBeNull();

        var c = resp.Data!;
        c.Available.Should().Be(3);          // avail1, avail2, overdue
        c.AssignedToMe.Should().Be(2);       // assigntome1, assigntome2
        c.AssignedToMyRoles.Should().Be(1);  // role1
        c.Claimed.Should().Be(1);            // claimed
        c.InProgress.Should().Be(1);         // inprog
        c.CompletedToday.Should().Be(1);     // completedToday
        c.Overdue.Should().Be(1);            // overdue
        c.Failed.Should().Be(1);             // failed
        c.TotalActionable.Should().Be(8);    // 3 + 2 + 1 + 1 + 1
    }

    [Fact]
    public async Task GetMyTasksListAsync_FilterByStatus_ShouldReturnOnlyThatStatus()
    {
        var inst = SeedInstance();

        AddTask(inst, "c1", WorkflowTaskStatus.Created);
        AddTask(inst, "claimed1", WorkflowTaskStatus.Claimed, userId: 5);
        AddTask(inst, "claimed2", WorkflowTaskStatus.Claimed, userId: 5);
        AddTask(inst, "inprogress", WorkflowTaskStatus.InProgress, userId: 5);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.GetMyTasksListAsync(WorkflowTaskStatus.Claimed, includeRoleTasks: true, includeUnassigned: true);

        resp.Success.Should().BeTrue();
        resp.Data.Should().NotBeNull();
        resp.Data!.Should().NotBeEmpty();
        resp.Data.Should().OnlyContain(t => t.Status == WorkflowTaskStatus.Claimed);
    }
}
