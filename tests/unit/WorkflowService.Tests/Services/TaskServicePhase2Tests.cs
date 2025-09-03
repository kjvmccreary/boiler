using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Services;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;
using Xunit;

namespace WorkflowService.Tests.Services;

public class TaskServicePhase2Tests : TestBase
{
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IWorkflowRuntime> _runtime = new();
    private readonly Mock<IEventPublisher> _events = new();
    private readonly Mock<ILogger<TaskService>> _logger = new();
    private readonly Mock<IUserContext> _user = new();
    private readonly Mock<ITaskNotificationDispatcher> _notifier = new();
    private TaskService CreateService(ITenantProvider? tenantOverride = null) =>
        new TaskService(DbContext, _mapper.Object, tenantOverride ?? MockTenantProvider.Object,
            _runtime.Object, _events.Object, _logger.Object, _user.Object, _notifier.Object);

    public TaskServicePhase2Tests()
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
                UpdatedAt = t.UpdatedAt
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
        SeedDefinitionAndInstance(int tenantId = 1, string name = "WF")
    {
        var def = new WorkflowService.Domain.Models.WorkflowDefinition
        {
            TenantId = tenantId,
            Name = name,
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

    private WorkflowService.Domain.Models.WorkflowTask NewTask(
        int tenantId,
        int instId,
        string nodeId,
        WorkflowTaskStatus status,
        int? userId = null,
        string? role = null,
        DateTime? due = null,
        DateTime? completed = null)
    {
        var task = new WorkflowService.Domain.Models.WorkflowTask
        {
            TenantId = tenantId,
            WorkflowInstanceId = instId,
            NodeId = nodeId,
            TaskName = nodeId,
            NodeType = "human",
            Status = status,
            AssignedToUserId = userId,
            AssignedToRole = role,
            DueDate = due,
            CreatedAt = DateTime.UtcNow.AddMinutes(-30),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-10),
            ClaimedAt = status == WorkflowTaskStatus.Claimed ? DateTime.UtcNow.AddMinutes(-20) : null,
            CompletedAt = completed
        };
        DbContext.WorkflowTasks.Add(task);
        return task;
    }

    // ---------- GetAllTasks / Tenant Scoping & Pagination ----------

    [Fact]
    public async Task GetAllTasksAsync_ShouldApplyTenantScoping_AndPagination()
    {
        var (_, inst1) = SeedDefinitionAndInstance(tenantId: 1, name: "WF1");
        var (_, inst2) = SeedDefinitionAndInstance(tenantId: 2, name: "WF2");

        for (int i = 0; i < 8; i++)
            NewTask(1, inst1.Id, $"t1_{i}", WorkflowTaskStatus.Created);
        for (int i = 0; i < 5; i++)
            NewTask(2, inst2.Id, $"t2_{i}", WorkflowTaskStatus.Created);
        DbContext.SaveChanges();

        // Default MockTenantProvider returns tenant 1 (from TestBase)
        var svc = CreateService();
        var req = new GetTasksRequestDto { Page = 2, PageSize = 3, SortBy = "createdAt" };

        var resp = await svc.GetAllTasksAsync(req);

        resp.Success.Should().BeTrue();
        resp.Data!.TotalCount.Should().Be(8);
        resp.Data.Items.Should().HaveCount(3);
        resp.Data.Items.Should().OnlyContain(t => t.TaskName.StartsWith("t1_"));
    }

    [Fact]
    public async Task GetAllTasksAsync_TenantMissing_ShouldReturnError()
    {
        var tenantMissing = new Mock<ITenantProvider>();
        tenantMissing.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync((int?)null);
        var svc = CreateService(tenantMissing.Object);

        var resp = await svc.GetAllTasksAsync(new GetTasksRequestDto { Page = 1, PageSize = 10 });

        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("Tenant");
    }

    // ---------- GetTaskById ----------

    [Fact]
    public async Task GetTaskByIdAsync_Found_ShouldReturnTask()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        var task = NewTask(1, inst.Id, "taskA", WorkflowTaskStatus.Created);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.GetTaskByIdAsync(task.Id);

        resp.Success.Should().BeTrue();
        resp.Data!.Id.Should().Be(task.Id);
    }

    [Fact]
    public async Task GetTaskByIdAsync_NotFound_ShouldReturnError()
    {
        var svc = CreateService();
        var resp = await svc.GetTaskByIdAsync(9999);
        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task GetTaskByIdAsync_TenantMissing_ShouldReturnError()
    {
        var tenantMissing = new Mock<ITenantProvider>();
        tenantMissing.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync((int?)null);
        var svc = CreateService(tenantMissing.Object);

        var resp = await svc.GetTaskByIdAsync(1);
        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("Tenant");
    }

    // ---------- ApplyTaskFiltersAndPagination (indirect via GetAllTasksAsync) ----------

    [Fact]
    public async Task GetAllTasksAsync_FilterByStatus_AndWorkflowDefinition()
    {
        var (def, inst) = SeedDefinitionAndInstance();
        NewTask(1, inst.Id, "s1", WorkflowTaskStatus.Created);
        NewTask(1, inst.Id, "s2", WorkflowTaskStatus.Claimed, userId: 1);
        DbContext.SaveChanges();

        var svc = CreateService();
        var req = new GetTasksRequestDto
        {
            Status = WorkflowTaskStatus.Claimed,
            WorkflowDefinitionId = def.Id,
            Page = 1,
            PageSize = 10,
            SortBy = "createdAt"
        };

        var resp = await svc.GetAllTasksAsync(req);
        resp.Success.Should().BeTrue();
        resp.Data!.Items.Should().HaveCount(1);
        resp.Data.Items[0].TaskName.Should().Be("s2");
    }

    [Fact]
    public async Task GetAllTasksAsync_FilterByAssignedUser_AndRole()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        NewTask(1, inst.Id, "u1", WorkflowTaskStatus.Assigned, userId: 7);
        NewTask(1, inst.Id, "role1", WorkflowTaskStatus.Assigned, role: "Approver");
        NewTask(1, inst.Id, "other", WorkflowTaskStatus.Created);
        DbContext.SaveChanges();

        var svc = CreateService();

        var byUser = await svc.GetAllTasksAsync(new GetTasksRequestDto
        {
            AssignedToUserId = 7, Page = 1, PageSize = 10, SortBy = "createdAt"
        });
        byUser.Data!.Items.Should().HaveCount(1).And.OnlyContain(t => t.TaskName == "u1");

        var byRole = await svc.GetAllTasksAsync(new GetTasksRequestDto
        {
            AssignedToRole = "Approver", Page = 1, PageSize = 10, SortBy = "createdAt"
        });
        byRole.Data!.Items.Should().HaveCount(1).And.OnlyContain(t => t.TaskName == "role1");
    }

    [Fact]
    public async Task GetAllTasksAsync_FilterByDueRange_AndSearch()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        NewTask(1, inst.Id, "alpha", WorkflowTaskStatus.Created, due: DateTime.UtcNow.AddHours(1));
        NewTask(1, inst.Id, "beta", WorkflowTaskStatus.Created, due: DateTime.UtcNow.AddHours(5));
        NewTask(1, inst.Id, "gamma", WorkflowTaskStatus.Created, due: DateTime.UtcNow.AddHours(10));
        DbContext.SaveChanges();

        var svc = CreateService();

        var dueBefore = await svc.GetAllTasksAsync(new GetTasksRequestDto
        {
            DueBefore = DateTime.UtcNow.AddHours(6),
            Page = 1, PageSize = 10, SortBy = "createdAt"
        });
        dueBefore.Data!.Items.Should().HaveCount(2);

        var search = await svc.GetAllTasksAsync(new GetTasksRequestDto
        {
            SearchTerm = "amm", // matches gamma
            Page = 1,
            PageSize = 10,
            SortBy = "createdAt"
        });
        search.Data!.Items.Should().HaveCount(1);
        search.Data.Items[0].TaskName.Should().Be("gamma");
    }

    [Theory]
    [InlineData("taskName", false)]
    [InlineData("taskName", true)]
    [InlineData("status", false)]
    [InlineData("status", true)]
    [InlineData("dueDate", false)]
    [InlineData("dueDate", true)]
    public async Task GetAllTasksAsync_SortVariants_ShouldReturnConsistentOrdering(string sortBy, bool desc)
    {
        var (_, inst) = SeedDefinitionAndInstance();
        NewTask(1, inst.Id, "b_task", WorkflowTaskStatus.Created, due: DateTime.UtcNow.AddHours(2));
        NewTask(1, inst.Id, "a_task", WorkflowTaskStatus.Assigned, due: DateTime.UtcNow.AddHours(1));
        NewTask(1, inst.Id, "c_task", WorkflowTaskStatus.Claimed, due: DateTime.UtcNow.AddHours(3));
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.GetAllTasksAsync(new GetTasksRequestDto
        {
            SortBy = sortBy,
            SortDescending = desc,
            Page = 1,
            PageSize = 10
        });

        resp.Success.Should().BeTrue();
        resp.Data!.Items.Should().HaveCount(3);
    }

    // ---------- Statistics ----------

    [Fact]
    public async Task GetStatisticsAsync_NonEmpty_ShouldReturnAggregates()
    {
        var (_, inst) = SeedDefinitionAndInstance();
        NewTask(1, inst.Id, "a", WorkflowTaskStatus.Created);
        NewTask(1, inst.Id, "b", WorkflowTaskStatus.Claimed, userId: 1);
        NewTask(1, inst.Id, "c", WorkflowTaskStatus.Completed, userId: 1, completed: DateTime.UtcNow.AddMinutes(-5));
        NewTask(1, inst.Id, "d", WorkflowTaskStatus.Assigned, userId: 2);
        DbContext.SaveChanges();

        var svc = CreateService();
        var resp = await svc.GetStatisticsAsync();

        resp.Success.Should().BeTrue();
        resp.Data!.TotalTasks.Should().Be(4);
        resp.Data.CompletedTasks.Should().Be(1);
        resp.Data.TasksByStatus.Should().ContainKey(WorkflowTaskStatus.Completed);
    }

    [Fact]
    public async Task GetStatisticsAsync_Empty_ShouldReturnZeros()
    {
        var svc = CreateService();
        var resp = await svc.GetStatisticsAsync();
        resp.Success.Should().BeTrue();
        resp.Data!.TotalTasks.Should().Be(0);
        resp.Data.TasksByStatus.Should().BeEmpty();
    }

    // ---------- Error Paths ----------

    [Fact]
    public async Task GetAllTasksAsync_Exception_ShouldReturnError()
    {
        var throwingTenant = new Mock<ITenantProvider>();
        throwingTenant.Setup(t => t.GetCurrentTenantIdAsync()).ThrowsAsync(new Exception("boom"));
        var svc = CreateService(throwingTenant.Object);

        var resp = await svc.GetAllTasksAsync(new GetTasksRequestDto { Page = 1, PageSize = 5 });

        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("Failed");
    }

    [Fact]
    public async Task GetTaskByIdAsync_Exception_ShouldReturnError()
    {
        var throwingTenant = new Mock<ITenantProvider>();
        throwingTenant.Setup(t => t.GetCurrentTenantIdAsync()).ThrowsAsync(new Exception("boom"));
        var svc = CreateService(throwingTenant.Object);

        var resp = await svc.GetTaskByIdAsync(1);
        resp.Success.Should().BeFalse();
        resp.Message.Should().Contain("Failed");
    }
}
