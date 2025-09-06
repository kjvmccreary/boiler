using FluentAssertions;
using Moq;
using Xunit;
using WorkflowService.Engine;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using Contracts.Services;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Tests.Engine;

public class WorkflowRuntimeProgressTests : TestBase
{
    private readonly Mock<ITenantProvider> _tenant = new();
    private readonly Mock<IConditionEvaluator> _conditions = new();
    private readonly Mock<ITaskNotificationDispatcher> _taskNotifier = new();
    private readonly Mock<IWorkflowNotificationDispatcher> _instanceNotifier = new();
    private readonly List<INodeExecutor> _executors = new();

    public WorkflowRuntimeProgressTests()
    {
        _tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);
        _conditions.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
    }

    private WorkflowRuntime CreateRuntime() => new(
        DbContext,
        _executors,
        _tenant.Object,
        _conditions.Object,
        _taskNotifier.Object,
        CreateMockLogger<WorkflowRuntime>().Object,
        _instanceNotifier.Object);

    [Fact]
    public async Task LinearHumanWorkflow_Emits_Progress_And_Final_100()
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "ProgressFlow",
            Version = 1,
            JSONDefinition = """
            {
              "nodes":[
                {"id":"start","type":"Start","properties":{}},
                {"id":"approve","type":"HumanTask","properties":{"label":"Approve"}},
                {"id":"end","type":"End","properties":{}}
              ],
              "edges":[
                {"id":"e1","source":"start","target":"approve"},
                {"id":"e2","source":"approve","target":"end"}
              ]
            }
            """,
            IsPublished = true,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();

        // Executors: Start + End as auto
        var startExec = new Mock<INodeExecutor>();
        startExec.Setup(e => e.CanExecute(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>()))
            .Returns<WorkflowService.Domain.Dsl.WorkflowNode>(n => n.Type == "Start");
        startExec.Setup(e => e.ExecuteAsync(
                It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>(),
                It.IsAny<WorkflowInstance>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NodeExecutionResult { IsSuccess = true, ShouldWait = false });

        var endExec = new Mock<INodeExecutor>();
        endExec.Setup(e => e.CanExecute(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>()))
            .Returns<WorkflowService.Domain.Dsl.WorkflowNode>(n => n.Type == "End");
        endExec.Setup(e => e.ExecuteAsync(
                It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>(),
                It.IsAny<WorkflowInstance>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NodeExecutionResult { IsSuccess = true, ShouldWait = false });

        // Human task executor (wait)
        var humanExec = new Mock<INodeExecutor>();
        humanExec.Setup(e => e.CanExecute(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>()))
            .Returns<WorkflowService.Domain.Dsl.WorkflowNode>(n => n.Id == "approve");
        humanExec.Setup(e => e.ExecuteAsync(
                It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>(),
                It.IsAny<WorkflowInstance>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkflowService.Domain.Dsl.WorkflowNode node, WorkflowInstance inst, string ctx, CancellationToken ct) =>
                new NodeExecutionResult
                {
                    IsSuccess = true,
                    ShouldWait = true,
                    CreatedTask = new WorkflowTask
                    {
                        TenantId = inst.TenantId,
                        WorkflowInstanceId = inst.Id,
                        NodeId = node.Id,
                        TaskName = node.Id,
                        NodeType = "human",
                        Status = DTOs.Workflow.Enums.TaskStatus.Created,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                });

        _executors.Clear();
        _executors.Add(startExec.Object);
        _executors.Add(endExec.Object);
        _executors.Add(humanExec.Object);

        var rt = CreateRuntime();
        var instance = await rt.StartWorkflowAsync(def.Id, "{}");

        instance.Status.Should().Be(InstanceStatus.Running);

        // Progress events so far (should have at least one)
        var initialProgressEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == instance.Id && e.Type == "Instance" && e.Name == "Progress")
            .ToList();
        initialProgressEvents.Count.Should().BeGreaterThan(0);

        // Claim and complete the human task
        var task = DbContext.WorkflowTasks.Single(t => t.WorkflowInstanceId == instance.Id && t.NodeId == "approve");
        task.Status = DTOs.Workflow.Enums.TaskStatus.Claimed;
        DbContext.SaveChanges();

        await rt.CompleteTaskAsync(task.Id, "{\"ok\":true}", completedByUserId: 5);

        var finalInstance = DbContext.WorkflowInstances.Single(i => i.Id == instance.Id);
        finalInstance.Status.Should().Be(InstanceStatus.Completed);

        var allProgress = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == instance.Id && e.Type == "Instance" && e.Name == "Progress")
            .OrderBy(e => e.OccurredAt)
            .ToList();

        allProgress.Count.Should().BeGreaterThanOrEqualTo(initialProgressEvents.Count);

        // Final progress payload should have percentage 100
        var last = allProgress.Last();
        last.Data.Should().Contain("\"percentage\":100").Or.Contain("\"percentage\": 100");
    }
}
