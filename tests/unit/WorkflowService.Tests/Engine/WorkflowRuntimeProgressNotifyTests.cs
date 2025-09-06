using FluentAssertions;
using Moq;
using Xunit;
using WorkflowService.Engine;
using WorkflowService.Engine.Interfaces;
using DTOs.Workflow.Enums;
using WorkflowService.Domain.Models;
using Contracts.Services;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Tests.Engine;

public class WorkflowRuntimeProgressNotifyTests : TestBase
{
    private readonly Mock<ITenantProvider> _tenant = new();
    private readonly Mock<IConditionEvaluator> _conds = new();
    private readonly Mock<ITaskNotificationDispatcher> _taskNotif = new();
    private readonly Mock<IWorkflowNotificationDispatcher> _instNotif = new();
    private readonly List<INodeExecutor> _executors = new();

    public WorkflowRuntimeProgressNotifyTests()
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
    public async Task Start_And_Complete_Should_Invoke_Progress_Notifier()
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "ProgressNotify",
            Version = 1,
            JSONDefinition = """
            {
              "nodes":[
                {"id":"start","type":"Start","properties":{}},
                {"id":"end","type":"End","properties":{}}
              ],
              "edges":[
                {"id":"e1","source":"start","target":"end"}
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

        var startExec = new Mock<INodeExecutor>();
        startExec.Setup(e => e.CanExecute(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>()))
            .Returns<WorkflowService.Domain.Dsl.WorkflowNode>(n => n.Type == "Start");
        startExec.Setup(e => e.ExecuteAsync(
            It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>(), It.IsAny<WorkflowInstance>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NodeExecutionResult { IsSuccess = true, ShouldWait = false });

        var endExec = new Mock<INodeExecutor>();
        endExec.Setup(e => e.CanExecute(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>()))
            .Returns<WorkflowService.Domain.Dsl.WorkflowNode>(n => n.Type == "End");
        endExec.Setup(e => e.ExecuteAsync(
            It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>(), It.IsAny<WorkflowInstance>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NodeExecutionResult { IsSuccess = true, ShouldWait = false });

        _executors.Add(startExec.Object);
        _executors.Add(endExec.Object);

        var rt = CreateRuntime();
        var inst = await rt.StartWorkflowAsync(def.Id, "{}");

        inst.Status.Should().Be(InstanceStatus.Completed);

        _instNotif.Verify(n => n.NotifyInstanceProgressAsync(
            1,
            inst.Id,
            It.Is<int>(p => p >= 0 && p <= 100),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<IEnumerable<string>>(),
            It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }
}
