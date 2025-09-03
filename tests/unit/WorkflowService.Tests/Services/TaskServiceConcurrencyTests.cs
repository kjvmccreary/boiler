using System.Linq;                // ensure LINQ for Single()
using AutoMapper;
using FluentAssertions;
using Moq;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using Contracts.Services;
using DTOs.Workflow;              // <-- ADDED (WorkflowTaskDto lives here)
using DTOs.Workflow.Enums;
using Xunit;
using Microsoft.Extensions.Logging;

namespace WorkflowService.Tests.Services;

public class TaskServiceConcurrencyTests : TestBase
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

    public TaskServiceConcurrencyTests()
    {
        _user.Setup(u => u.UserId).Returns(42);
        _user.Setup(u => u.Roles).Returns(new[] { "Reviewer" });

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
                ClaimedAt = t.ClaimedAt
            });
    }

    private WorkflowInstance SeedInstance()
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "ConcWF",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = System.DateTime.UtcNow,
            UpdatedAt = System.DateTime.UtcNow
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
            StartedAt = System.DateTime.UtcNow,
            CreatedAt = System.DateTime.UtcNow,
            UpdatedAt = System.DateTime.UtcNow
        };
        DbContext.WorkflowInstances.Add(inst);
        DbContext.SaveChanges();

        var task = new WorkflowTask
        {
            TenantId = 1,
            WorkflowInstanceId = inst.Id,
            NodeId = "n1",
            TaskName = "Concurrent Claim",
            NodeType = "human",
            Status = DTOs.Workflow.Enums.TaskStatus.Created,
            CreatedAt = System.DateTime.UtcNow.AddMinutes(-10),
            UpdatedAt = System.DateTime.UtcNow.AddMinutes(-5)
        };
        DbContext.WorkflowTasks.Add(task);
        DbContext.SaveChanges();
        return inst;
    }

    [Fact]
    public async Task DoubleClaim_ShouldSecondFail_IdempotentGuard()
    {
        var inst = SeedInstance();
        var task = DbContext.WorkflowTasks.Single(t => t.WorkflowInstanceId == inst.Id);
        var svc = CreateService();

        // First claim should succeed
        var first = await svc.ClaimTaskAsync(task.Id);
        first.Success.Should().BeTrue();

        // Second claim attempt (same user context) should fail because status is now Claimed
        var second = await svc.ClaimTaskAsync(task.Id);
        second.Success.Should().BeFalse();
        second.Message.Should().Match(m => m != null && m.ToLower().Contains("cannot be claimed"));
    }
}
