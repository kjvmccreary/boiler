using FluentAssertions;
using Xunit;
using WorkflowService.Engine;
using WorkflowService.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Contracts.Services;
using WorkflowService.Engine.Interfaces;
using DTOs.Workflow.Enums;
using WorkflowService.Services.Interfaces; // ITaskNotificationDispatcher
using WorkflowTaskStatusEnum = DTOs.Workflow.Enums.TaskStatus; // ✅ Alias to disambiguate from System.Threading.Tasks.TaskStatus

namespace WorkflowService.Tests.Engine;

public class RuntimeSaveChangesTests : TestBase
{
    private readonly WorkflowRuntime _runtime;
    private readonly Mock<IWorkflowRuntime> _noop = new();
    private readonly Mock<ITenantProvider> _tenantProvider = new();
    private readonly Mock<IConditionEvaluator> _cond = new();
    private readonly Mock<ITaskNotificationDispatcher> _notify = new();
    private readonly List<INodeExecutor> _executors = new();
    private readonly Mock<ILogger<WorkflowRuntime>> _logger = new();

    public RuntimeSaveChangesTests()
    {
        _tenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);
        _cond.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var genericExec = new Mock<INodeExecutor>();
        genericExec.Setup(e => e.CanExecute(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>())).Returns(true);
        genericExec.Setup(e => e.ExecuteAsync(
                It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>(),
                It.IsAny<WorkflowInstance>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WorkflowService.Engine.Interfaces.NodeExecutionResult
            {
                IsSuccess = true,
                ShouldWait = true
            });
        _executors.Add(genericExec.Object);

        _runtime = new WorkflowRuntime(
            DbContext,
            _executors,
            _tenantProvider.Object,
            _cond.Object,
            _notify.Object,
            _logger.Object);
    }

    [Fact]
    public async Task StartWorkflow_ShouldCreateRunningInstance()
    {
        var def = await CreateDefinitionAsync();

        var instance = await _runtime.StartWorkflowAsync(def.Id, "{}", 10, autoCommit: true);

        instance.Should().NotBeNull();
        instance.Status.Should().Be(InstanceStatus.Running);
        instance.StartedByUserId.Should().Be(10);
    }

    [Fact]
    public async Task CompleteTask_OnClaimedHumanTask_ShouldMarkCompleted()
    {
        var (_, task) = await CreateInstanceWithHumanTaskAsync();

        // Simulate claimed
        task.Status = WorkflowTaskStatusEnum.Claimed;
        task.AssignedToUserId = 5;
        await DbContext.SaveChangesAsync();

        await _runtime.CompleteTaskAsync(task.Id, """{"ok":true}""", 5, autoCommit: true);

        await DbContext.Entry(task).ReloadAsync();

        task.Status.Should().BeOneOf(WorkflowTaskStatusEnum.Completed, WorkflowTaskStatusEnum.Cancelled);

        if (task.Status == WorkflowTaskStatusEnum.Completed)
        {
            task.CompletionData.Should().NotBeNullOrEmpty();
            task.CompletionData.Should().Contain("\"ok\"");
        }
    }

    private async Task<WorkflowDefinition> CreateDefinitionAsync()
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Runtime WF",
            Version = 1,
            JSONDefinition = """
            {
              "nodes":[
                {"id":"start","type":"Start"},
                {"id":"human1","type":"HumanTask","properties":{"taskName":"Human 1"}},
                {"id":"end","type":"End"}
              ],
              "edges":[
                {"id":"e1","source":"start","target":"human1"},
                {"id":"e2","source":"human1","target":"end"}
              ]
            }
            """,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        await DbContext.SaveChangesAsync();
        return def;
    }

    private async Task<(WorkflowInstance instance, WorkflowTask task)> CreateInstanceWithHumanTaskAsync()
    {
        var def = await CreateDefinitionAsync();
        var inst = new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = def.Version,
            Status = InstanceStatus.Running,
            CurrentNodeIds = """["human1"]""",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowInstances.Add(inst);
        await DbContext.SaveChangesAsync();

        var task = new WorkflowTask
        {
            TenantId = 1,
            WorkflowInstanceId = inst.Id,
            NodeId = "human1",
            TaskName = "Human 1",
            Status = WorkflowTaskStatusEnum.Created,
            NodeType = "human",
            Data = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowTasks.Add(task);
        await DbContext.SaveChangesAsync();
        return (inst, task);
    }
}
