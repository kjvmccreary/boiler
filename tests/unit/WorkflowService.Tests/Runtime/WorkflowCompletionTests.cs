using System.Text.Json;
using AutoMapper;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using DTOs.Common;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using FluentAssertions;
using WorkflowService.Engine;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using WorkflowService.Domain.Models;
using Contracts.Services;
using Microsoft.Extensions.Logging;
using TaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Tests.Runtime;

public class WorkflowCompletionTests : TestBase
{
    private readonly Mock<ITenantProvider> _tenantProvider = new();
    private readonly Mock<IConditionEvaluator> _conditionEvaluator = new();
    private readonly Mock<IRoleService> _roleService = new();
    private readonly Mock<ITaskNotificationDispatcher> _taskNotifier = new();
    private readonly Mock<IEventPublisher> _eventPublisher = new();
    private readonly Mock<IUserContext> _userContext = new();
    private readonly Mock<IMapper> _mapper = new();

    private IWorkflowRuntime CreateRuntime(WorkflowDbContext db)
    {
        var loggerFactory = LoggerFactory.Create(b => b.AddDebug().AddConsole().SetMinimumLevel(LogLevel.Warning));
        var humanExecLogger = loggerFactory.CreateLogger<WorkflowService.Engine.Executors.HumanTaskExecutor>();
        var startEndLogger = loggerFactory.CreateLogger<WorkflowService.Engine.Executors.StartEndExecutor>();
        var runtimeLogger = loggerFactory.CreateLogger<WorkflowRuntime>();

        var humanExecutor = new WorkflowService.Engine.Executors.HumanTaskExecutor(
            humanExecLogger,
            _roleService.Object,
            _taskNotifier.Object,
            db);

        var startEndExecutor = new WorkflowService.Engine.Executors.StartEndExecutor(startEndLogger);

        var executors = new List<INodeExecutor> { humanExecutor, startEndExecutor };

        return new WorkflowRuntime(
            db,
            executors,
            _tenantProvider.Object,
            _conditionEvaluator.Object,
            _taskNotifier.Object,
            runtimeLogger);
    }

    private TaskService CreateTaskService(WorkflowDbContext db, IWorkflowRuntime runtime)
    {
        var loggerFactory = LoggerFactory.Create(b => b.AddDebug().AddConsole().SetMinimumLevel(LogLevel.Warning));
        var taskLogger = loggerFactory.CreateLogger<TaskService>();

        _mapper.Setup(m => m.Map<WorkflowTaskDto>(It.IsAny<object>()))
            .Returns<object>(o =>
            {
                var t = (WorkflowTask)o;
                return new WorkflowTaskDto
                {
                    Id = t.Id,
                    WorkflowInstanceId = t.WorkflowInstanceId,
                    NodeId = t.NodeId,
                    TaskName = t.TaskName,
                    Status = t.Status,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToRole = t.AssignedToRole,
                    CompletedAt = t.CompletedAt
                };
            });

        return new TaskService(
            db,
            _mapper.Object,
            _tenantProvider.Object,
            runtime,
            _eventPublisher.Object,
            taskLogger,
            _userContext.Object,
            _taskNotifier.Object);
    }

    [Fact]
    public async Task Completing_Last_Human_Task_Should_Complete_Instance_Without_Cancelling_Task()
    {
        // Arrange
        var tenantId = 1;
        var userId = 42;
        _tenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(tenantId);
        _userContext.SetupGet(u => u.UserId).Returns(userId);
        _userContext.SetupGet(u => u.Roles).Returns(Array.Empty<string>());
        _conditionEvaluator.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var db = DbContext;

        var definitionJson = JsonSerializer.Serialize(new
        {
            id = "wf_simple",
            name = "Simple Human Flow",
            version = 1,
            nodes = new[]
            {
                new { id = "start", type = "Start", name = "Start" },
                new { id = "task1", type = "HumanTask", name = "Human Task 1" },
                new { id = "end", type = "End", name = "End" }
            },
            edges = new[]
            {
                new { source = "start", target = "task1" },
                new { source = "task1", target = "end" }
            }
        });

        var def = new WorkflowDefinition
        {
            TenantId = tenantId,
            Name = "Simple",
            Version = 1,
            JSONDefinition = definitionJson,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.WorkflowDefinitions.Add(def);
        await db.SaveChangesAsync();

        var runtime = CreateRuntime(db);
        var taskService = CreateTaskService(db, runtime);

        // Start workflow (creates human task)
        var instance = await runtime.StartWorkflowAsync(def.Id, "{}", userId);
        await db.Entry(instance).ReloadAsync();

        var task = await db.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instance.Id)
            .FirstOrDefaultAsync();
        task.Should().NotBeNull();
        task!.Status.Should().Be(TaskStatus.Created);

        // Claim
        var claimResp = await taskService.ClaimTaskAsync(task.Id);
        claimResp.Success.Should().BeTrue();
        (await db.WorkflowTasks.FindAsync(task.Id))!.Status.Should().Be(TaskStatus.Claimed);

        // Complete
        var completeResp = await taskService.CompleteTaskAsync(task.Id,
            new CompleteTaskRequestDto { CompletionData = "{}" });
        completeResp.Success.Should().BeTrue();

        var finalTask = await db.WorkflowTasks.AsNoTracking()
            .FirstAsync(t => t.Id == task.Id);
        var finalInstance = await db.WorkflowInstances.AsNoTracking()
            .FirstAsync(i => i.Id == instance.Id);

        // Assertions on state
        finalTask.Status.Should().Be(TaskStatus.Completed);
        finalTask.CompletedAt.Should().NotBeNull();
        finalInstance.Status.Should().Be(InstanceStatus.Completed);

        // Load events (separate collections)
        var taskEvents = await db.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == instance.Id && e.Type == "Task")
            .ToListAsync();
        var instanceEvents = await db.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == instance.Id && e.Type == "Instance")
            .ToListAsync();

        // No cancellation for this task
        taskEvents.Where(e => e.Name == "Cancelled" && e.Data.Contains($"\"taskId\":{finalTask.Id}"))
            .Should().BeEmpty("no cancellation should be emitted after successful completion");

        // Completion event exists
        taskEvents.Any(e => e.Name == "Completed" && e.Data.Contains($"\"taskId\":{finalTask.Id}"))
            .Should().BeTrue("task completion event should be recorded");

        // Instance completion event exists (was previously missed due to filtering bug)
        instanceEvents.Any(e => e.Name == "Completed")
            .Should().BeTrue("instance completion event should be emitted");

        // (Optional) If test ever fails, dump events for diagnostics
        // Uncomment for debugging:
        // if (!instanceEvents.Any(e => e.Name == "Completed"))
        // {
        //     var dump = string.Join("\n", instanceEvents.Select(e => $"{e.Type}:{e.Name}:{e.Data}"));
        //     throw new Xunit.Sdk.XunitException("Missing instance completed event.\nEvents:\n" + dump);
        // }
    }

    [Fact]
    public async Task Completing_Task_With_PlainText_CompletionData_Should_Persist_Raw_Wrapper()
    {
        // Arrange
        var tenantId = 1;
        var userId = 99;
        _tenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(tenantId);
        _userContext.SetupGet(u => u.UserId).Returns(userId);
        _userContext.SetupGet(u => u.Roles).Returns(Array.Empty<string>());
        _conditionEvaluator.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var db = DbContext;

        var definitionJson = JsonSerializer.Serialize(new
        {
            id = "wf_plaintext",
            name = "Plain Text Completion",
            version = 1,
            nodes = new[]
            {
                new { id = "start", type = "Start", name = "Start" },
                new { id = "task1", type = "HumanTask", name = "Human Task" },
                new { id = "end", type = "End", name = "End" }
            },
            edges = new[]
            {
                new { source = "start", target = "task1" },
                new { source = "task1", target = "end" }
            }
        });

        var def = new WorkflowDefinition
        {
            TenantId = tenantId,
            Name = "PlainText",
            Version = 1,
            JSONDefinition = definitionJson,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.WorkflowDefinitions.Add(def);
        await db.SaveChangesAsync();

        var runtime = CreateRuntime(db);
        var taskService = CreateTaskService(db, runtime);

        var instance = await runtime.StartWorkflowAsync(def.Id, "{}", userId);
        var task = await db.WorkflowTasks.FirstAsync(t => t.WorkflowInstanceId == instance.Id);

        // Claim
        (await taskService.ClaimTaskAsync(task.Id)).Success.Should().BeTrue();

        // Complete with plain text
        var completionText = "Per Postman";
        (await taskService.CompleteTaskAsync(task.Id,
            new CompleteTaskRequestDto { CompletionData = completionText })).Success.Should().BeTrue();

        // Reload instance to inspect context
        var finalInstance = await db.WorkflowInstances.AsNoTracking()
            .FirstAsync(i => i.Id == instance.Id);

        var contextDict = JsonSerializer.Deserialize<Dictionary<string, object>>(finalInstance.Context)
                          ?? new Dictionary<string, object>();

        contextDict.ContainsKey("task_task1").Should().BeTrue("task completion payload should be stored");

        var rawNode = contextDict["task_task1"];

        // Serialize back to string to inspect; raw wrapper expected
        var rawJson = JsonSerializer.Serialize(rawNode);
        rawJson.Should().Contain("\"raw\"");
        rawJson.Should().Contain("Per Postman");

        // Sanity: task completed & instance completed
        var finalTask = await db.WorkflowTasks.AsNoTracking().FirstAsync(t => t.Id == task.Id);
        finalTask.Status.Should().Be(TaskStatus.Completed);
        finalInstance.Status.Should().Be(InstanceStatus.Completed);
    }

    [Fact]
    public async Task Completing_Task_Should_Emit_ContextUpdated_Event()
    {
        // (Reuse arrange from existing test; shorten for brevity)
        var tenantId = 1; var userId = 7;
        _tenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(tenantId);
        _userContext.SetupGet(u => u.UserId).Returns(userId);
        _conditionEvaluator.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var db = DbContext;
        var defJson = "{\"id\":\"wf_evt\",\"name\":\"Evt\",\"version\":1,\"nodes\":[{\"id\":\"start\",\"type\":\"Start\"},{\"id\":\"t1\",\"type\":\"HumanTask\"},{\"id\":\"end\",\"type\":\"End\"}],\"edges\":[{\"source\":\"start\",\"target\":\"t1\"},{\"source\":\"t1\",\"target\":\"end\"}]}";
        db.WorkflowDefinitions.Add(new WorkflowDefinition { TenantId = tenantId, Name="Evt", Version=1, JSONDefinition=defJson, IsPublished=true, CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow});
        await db.SaveChangesAsync();

        var runtime = CreateRuntime(db);
        var taskService = CreateTaskService(db, runtime);

        var instance = await runtime.StartWorkflowAsync(db.WorkflowDefinitions.First().Id, "{}", userId);
        var task = await db.WorkflowTasks.FirstAsync(t => t.WorkflowInstanceId == instance.Id);
        (await taskService.ClaimTaskAsync(task.Id)).Success.Should().BeTrue();
        (await taskService.CompleteTaskAsync(task.Id, new CompleteTaskRequestDto { CompletionData = "{\"x\":1}" })).Success.Should().BeTrue();

        var contextEvents = await db.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == instance.Id && e.Type == "Task" && e.Name == "Context_Updated")
            .ToListAsync();

        contextEvents.Should().HaveCount(1);
        contextEvents[0].Data.Should().Contain("\"taskNodeId\"");
        contextEvents[0].Data.Should().Contain("\"dataShape\"");
    }
}
