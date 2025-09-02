using FluentAssertions;
using Xunit;
using WorkflowService.Background;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using DTOs.Workflow.Enums;
using Contracts.Services;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace WorkflowService.Tests.Background;

public class TimerWorkerTests : TestBase
{
    private readonly Mock<IWorkflowRuntime> _mockRuntime;
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly IConfiguration _configuration;

    private IServiceProvider BuildProvider(string? dbName = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton(_mockRuntime.Object);
        services.AddSingleton(_mockTenantProvider.Object);
        services.AddDbContext<WorkflowDbContext>(o =>
            o.UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString()));
        services.AddSingleton(typeof(ILogger<TimerWorker>), CreateMockLogger<TimerWorker>().Object);
        services.AddSingleton(typeof(ILogger<WorkflowDbContext>), CreateMockLogger<WorkflowDbContext>().Object);
        return services.BuildServiceProvider();
    }

    public TimerWorkerTests()
    {
        _mockRuntime = new Mock<IWorkflowRuntime>();
        _mockTenantProvider = new Mock<ITenantProvider>();
        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync()).ReturnsAsync(1);

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"WorkflowSettings:Timer:IntervalSeconds", "1"},
                {"WorkflowSettings:Timer:BatchSize", "50"}
            })
            .Build();
    }

    private async Task<WorkflowTask> SeedTimerAsync(IServiceProvider provider, int tenantId, DateTime due, bool runningInstance = true)
    {
        using var scope = provider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

        var def = new WorkflowDefinition
        {
            TenantId = tenantId,
            Name = $"WF {tenantId}",
            Version = 1,
            JSONDefinition = """
            {
              "nodes":[
                {"id":"start","type":"Start"},
                {"id":"timer1","type":"Timer"},
                {"id":"end","type":"End"}
              ],
              "edges":[
                {"id":"e1","source":"start","target":"timer1"},
                {"id":"e2","source":"timer1","target":"end"}
              ]
            }
            """,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        ctx.WorkflowDefinitions.Add(def);
        await ctx.SaveChangesAsync();

        var inst = new WorkflowInstance
        {
            TenantId = tenantId,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = 1,
            Status = runningInstance ? InstanceStatus.Running : InstanceStatus.Completed,
            CurrentNodeIds = """["timer1"]""",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        ctx.WorkflowInstances.Add(inst);
        await ctx.SaveChangesAsync();

        var task = new WorkflowTask
        {
            TenantId = tenantId,
            WorkflowInstanceId = inst.Id,
            NodeId = "timer1",
            TaskName = "Timer",
            Status = WorkflowTaskStatus.Created,
            NodeType = "timer",
            DueDate = due,
            Data = "{}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        ctx.WorkflowTasks.Add(task);
        await ctx.SaveChangesAsync();

        return task;
    }

    [Fact]
    public async Task TimerWorker_ShouldAdvanceAfterDue()
    {
        var provider = BuildProvider("timers-db-advance");
        var task = await SeedTimerAsync(provider, 1, DateTime.UtcNow.AddMinutes(-2));
        var logger = CreateMockLogger<TimerWorker>().Object;
        var worker = new TimerWorker(provider, logger, _configuration);

        _mockRuntime.Setup(r => r.CompleteTaskAsync(task.Id, It.IsAny<string>(), 0, It.IsAny<CancellationToken>(), false))
            .Returns(Task.CompletedTask);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(350));
        await worker.StartAsync(cts.Token);

        _mockRuntime.Verify(r => r.CompleteTaskAsync(task.Id, It.IsAny<string>(), 0, It.IsAny<CancellationToken>(), false),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task TimerWorker_ShouldNotProcessFutureTasks()
    {
        var provider = BuildProvider("timers-db-future");
        var task = await SeedTimerAsync(provider, 1, DateTime.UtcNow.AddMinutes(10));
        var logger = CreateMockLogger<TimerWorker>().Object;
        var worker = new TimerWorker(provider, logger, _configuration);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(350));
        await worker.StartAsync(cts.Token);

        _mockRuntime.Verify(r => r.CompleteTaskAsync(task.Id, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), false),
            Times.Never);
    }

    [Fact]
    public async Task TimerWorker_ShouldOnlyProcessRunningInstances()
    {
        var provider = BuildProvider("timers-db-status");
        var task = await SeedTimerAsync(provider, 1, DateTime.UtcNow.AddMinutes(-5), runningInstance: false);
        var logger = CreateMockLogger<TimerWorker>().Object;
        var worker = new TimerWorker(provider, logger, _configuration);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(350));
        await worker.StartAsync(cts.Token);

        _mockRuntime.Verify(r => r.CompleteTaskAsync(task.Id, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>(), false),
            Times.Never);
    }

    [Fact]
    public async Task TimerWorker_ShouldHandleMultipleTenants()
    {
        // Use single shared DB so both tenants' tasks are visible
        var provider = BuildProvider("timers-db-multi");
        var task1 = await SeedTimerAsync(provider, 1, DateTime.UtcNow.AddMinutes(-3));
        var task2 = await SeedTimerAsync(provider, 2, DateTime.UtcNow.AddMinutes(-2));
        var logger = CreateMockLogger<TimerWorker>().Object;
        var worker = new TimerWorker(provider, logger, _configuration);

        _mockRuntime.Setup(r => r.CompleteTaskAsync(It.IsAny<int>(), It.IsAny<string>(), 0, It.IsAny<CancellationToken>(), false))
            .Returns(Task.CompletedTask);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
        await worker.StartAsync(cts.Token);

        _mockRuntime.Verify(r => r.CompleteTaskAsync(task1.Id, It.IsAny<string>(), 0, It.IsAny<CancellationToken>(), false),
            Times.AtLeastOnce);
        _mockRuntime.Verify(r => r.CompleteTaskAsync(task2.Id, It.IsAny<string>(), 0, It.IsAny<CancellationToken>(), false),
            Times.AtLeastOnce);
    }
}
