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

public class WorkflowRuntimeProgressSmokeTests : TestBase
{
    private readonly Mock<ITenantProvider> _tenant = new();
    private readonly Mock<IConditionEvaluator> _conds = new();
    private readonly Mock<ITaskNotificationDispatcher> _taskNotif = new();
    private readonly Mock<IWorkflowNotificationDispatcher> _instNotif = new();
    private readonly List<INodeExecutor> _executors = new();

    public WorkflowRuntimeProgressSmokeTests()
    {
        _tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);
        _conds.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
    }

    private WorkflowRuntime CreateRuntime() => new(
        DbContext,
        _executors,
        _tenant.Object,
        _conds.Object,
        _taskNotif.Object,
        CreateMockLogger<WorkflowRuntime>().Object,
        _instNotif.Object);

    [Fact]
    public async Task AutoWorkflow_Should_Emit_Progress_And_Complete()
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "AutoLinear",
            Version = 1,
            JSONDefinition = """
            {
              "nodes":[
                {"id":"start","type":"Start","properties":{}},
                {"id":"a","type":"Task","properties":{}},
                {"id":"end","type":"End","properties":{}}
              ],
              "edges":[
                {"id":"e1","source":"start","target":"a"},
                {"id":"e2","source":"a","target":"end"}
              ]
            }
            """,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PublishedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();

        // Simple executors: treat Task/End as auto
        var startExec = new Mock<INodeExecutor>();
        startExec.Setup(e => e.CanExecute(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>()))
            .Returns<WorkflowService.Domain.Dsl.WorkflowNode>(n => n.Type == "Start");
        startExec.Setup(e => e.ExecuteAsync(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>(), It.IsAny<WorkflowInstance>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NodeExecutionResult { IsSuccess = true, ShouldWait = false });

        var autoExec = new Mock<INodeExecutor>();
        autoExec.Setup(e => e.CanExecute(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>()))
            .Returns<WorkflowService.Domain.Dsl.WorkflowNode>(n => n.Type == "Task" || n.Type == "End");
        autoExec.Setup(e => e.ExecuteAsync(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>(), It.IsAny<WorkflowInstance>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NodeExecutionResult { IsSuccess = true, ShouldWait = false });

        _executors.Add(startExec.Object);
        _executors.Add(autoExec.Object);

        var rt = CreateRuntime();
        var inst = await rt.StartWorkflowAsync(def.Id, "{}");

        inst.Status.Should().Be(InstanceStatus.Completed);

        var progressEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Instance" && e.Name == "Progress")
            .ToList();

        progressEvents.Should().NotBeEmpty();
        progressEvents.Last().Data.Should().Contain("\"percentage\":100");
    }
}
