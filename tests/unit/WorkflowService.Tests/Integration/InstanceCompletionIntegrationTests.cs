using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using WorkflowService.Engine;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Engine.Executors;
using WorkflowService.Domain.Models;
using WorkflowService.Domain.Dsl;
using WorkflowService.Persistence;
using Contracts.Services;
using DTOs.Workflow.Enums;
using Microsoft.Extensions.Logging;
using TaskStatus = DTOs.Workflow.Enums.TaskStatus;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Tests.Integration;

public class InstanceCompletionIntegrationTests : TestBase
{
    private readonly Mock<ITenantProvider> _tenant = new();
    private readonly Mock<IConditionEvaluator> _conditions = new();
    private readonly Mock<ITaskNotificationDispatcher> _notifier = new();
    private readonly Mock<IRoleService> _roleService = new();
    private readonly ILogger<WorkflowRuntime> _rtLogger;
    private readonly ILogger<HumanTaskExecutor> _humanLogger;
    private readonly ILogger<StartEndExecutor> _seLogger;

    public InstanceCompletionIntegrationTests()
    {
        _tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);
        _conditions.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _rtLogger = CreateMockLogger<WorkflowRuntime>().Object;
        _humanLogger = CreateMockLogger<HumanTaskExecutor>().Object;
        _seLogger = CreateMockLogger<StartEndExecutor>().Object;
    }

    [Fact]
    public async Task HumanTask_Completion_Completes_Instance_And_Emits_Events_And_Outbox()
    {
        // Arrange: Published definition with Start -> HumanTask -> End
        var json = """
        {
          "nodes": [
            {"id":"start","type":"Start","properties":{}},
            {"id":"approve","type":"HumanTask","properties":{"label":"Approve Task"}},
            {"id":"end","type":"End","properties":{}}
          ],
          "edges": [
            {"id":"e1","source":"start","target":"approve"},
            {"id":"e2","source":"approve","target":"end"}
          ]
        }
        """;

        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Approval",
            Version = 1,
            JSONDefinition = json,
            IsPublished = true,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        await DbContext.SaveChangesAsync();

        var humanExec = new HumanTaskExecutor(_humanLogger, _roleService.Object, _notifier.Object, DbContext);
        var startEndExec = new StartEndExecutor(_seLogger);
        var executors = new List<INodeExecutor> { humanExec, startEndExec };

        var runtime = new WorkflowRuntime(
            DbContext,
            executors,
            _tenant.Object,
            _conditions.Object,
            _notifier.Object,
            _rtLogger);

        // Act 1: Start instance (engine will create human task and pause)
        var instance = await runtime.StartWorkflowAsync(def.Id, "{}", startedByUserId: 42);
        instance.Status.Should().Be(InstanceStatus.Running);

        var task = await DbContext.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instance.Id)
            .OrderBy(t => t.Id)
            .FirstOrDefaultAsync();

        task.Should().NotBeNull("Human task must be created");
        task!.Status.Should().BeOneOf(TaskStatus.Created, TaskStatus.Assigned);

        // CLAIM STEP (test fix): engine requires Claimed or InProgress for completion
        task.Status = TaskStatus.Claimed;
        task.AssignedToUserId = 99;
        task.ClaimedAt = DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;
        await DbContext.SaveChangesAsync();

        // Act 2: Complete task
        await runtime.CompleteTaskAsync(task.Id, """{"approved":true}""", completedByUserId: 99);
        await DbContext.SaveChangesAsync();

        // Reload instance
        var refreshed = await DbContext.WorkflowInstances.FirstAsync(i => i.Id == instance.Id);

        // Assert: Instance completed
        refreshed.Status.Should().Be(InstanceStatus.Completed);
        refreshed.CompletedAt.Should().NotBeNull();

        // Assert: Task marked completed
        var completedTask = await DbContext.WorkflowTasks.FirstAsync(t => t.Id == task.Id);
        completedTask.Status.Should().Be(TaskStatus.Completed);
        completedTask.CompletedAt.Should().NotBeNull();

        // Events
        var events = await DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == instance.Id)
            .Select(e => new { e.Type, e.Name })
            .ToListAsync();

        events.Should().Contain(e => e.Type == "Instance" && e.Name == "Completed");
        events.Should().Contain(e => e.Type == "Task" && (e.Name == "Completed" || e.Name == "TimerCompleted"));

        // Outbox
        var outboxTypes = await DbContext.OutboxMessages
            .Where(o => o.TenantId == 1)
            .Select(o => o.EventType)
            .ToListAsync();

        outboxTypes.Should().Contain("workflow.instance.completed");
    }
}
