using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Background;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using WorkflowService.Domain.Models;
using Xunit;
using DTOs.Workflow.Enums;
using Contracts.Services;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;
using Microsoft.AspNetCore.Http;

namespace WorkflowService.Tests.Background;

public class TimerWorkerTests : TestBase
{
    private TimerWorker CreateWorker(
        ServiceProvider provider,
        int batchSize = 5,
        int intervalSeconds = 60)
    {
        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["WorkflowSettings:Timer:BatchSize"] = batchSize.ToString(),
            ["WorkflowSettings:Timer:IntervalSeconds"] = intervalSeconds.ToString()
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfig)
            .Build();

        var logger = provider.GetRequiredService<ILogger<TimerWorker>>();
        return new TimerWorker(provider, logger, config);
    }

    private ServiceProvider BuildProvider(
        Mock<IWorkflowRuntime> runtimeMock,
        Mock<ITenantProvider>? tenantMock = null)
    {
        tenantMock ??= new Mock<ITenantProvider>();
        tenantMock.Setup(t => t.SetCurrentTenantAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        tenantMock.Setup(t => t.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        runtimeMock
            .Setup(r => r.CompleteTaskAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        var services = new ServiceCollection();

        services.AddLogging(b => b.AddDebug().SetMinimumLevel(LogLevel.Warning));
        services.AddHttpContextAccessor();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Make DbContext singleton for test lifetime to avoid disposed scope issues
        services.AddDbContext<WorkflowDbContext>(
            o => o.UseInMemoryDatabase($"wf_timers_{Guid.NewGuid()}"),
            contextLifetime: ServiceLifetime.Singleton,
            optionsLifetime: ServiceLifetime.Singleton);

        services.AddSingleton<IWorkflowRuntime>(runtimeMock.Object);
        services.AddSingleton<ITenantProvider>(tenantMock.Object);

        return services.BuildServiceProvider();
    }

    private async Task<(ServiceProvider provider, WorkflowDbContext db)> SeedDueTimersAsync(
        int totalTimers,
        int tenantId = 1)
    {
        var runtimeMock = new Mock<IWorkflowRuntime>();
        var provider = BuildProvider(runtimeMock);
        var db = provider.GetRequiredService<WorkflowDbContext>();

        var def = new WorkflowDefinition
        {
            TenantId = tenantId,
            Name = "Timer WF",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.WorkflowDefinitions.Add(def);
        await db.SaveChangesAsync();

        var inst = new WorkflowInstance
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
        db.WorkflowInstances.Add(inst);
        await db.SaveChangesAsync();

        var due = DateTime.UtcNow.AddMinutes(-10);

        for (int i = 0; i < totalTimers; i++)
        {
            db.WorkflowTasks.Add(new WorkflowTask
            {
                TenantId = tenantId,
                WorkflowInstanceId = inst.Id,
                NodeId = $"timer_{i}",
                TaskName = $"T{i}",
                NodeType = "timer",
                Status = WorkflowTaskStatus.Created,
                DueDate = due.AddSeconds(-i),
                CreatedAt = DateTime.UtcNow.AddMinutes(-20),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-20)
            });
        }

        await db.SaveChangesAsync();
        return (provider, db);
    }

    private async Task InvokeSingleCycleAsync(TimerWorker worker, CancellationToken ct = default)
    {
        var mi = typeof(TimerWorker)
            .GetMethod("ProcessCycleAsync", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Could not reflect ProcessCycleAsync");

        var task = (Task)mi.Invoke(worker, new object[] { ct })!;
        await task;
    }

    [Fact]
    public async Task ProcessCycle_ShouldAcquireUpToBatchSize()
    {
        // Arrange: 12 due timers, batch size 5
        var (provider, db) = await SeedDueTimersAsync(totalTimers: 12);
        var worker = CreateWorker(provider, batchSize: 5);

        // Act: invoke one processing cycle via reflection
        await InvokeSingleCycleAsync(worker);

        // Assert: At most 5 tasks moved to InProgress, rest remain Created
        var all = await db.WorkflowTasks.OrderBy(t => t.Id).ToListAsync();
        var inProgress = all.Count(t => t.Status == WorkflowTaskStatus.InProgress);
        var created = all.Count(t => t.Status == WorkflowTaskStatus.Created);

        inProgress.Should().Be(5, "batch size limits acquisition");
        created.Should().Be(7, "remaining timers untouched this cycle");
    }

    [Fact]
    public async Task ProcessCycle_NoDueTimers_ShouldDoNothing()
    {
        // Arrange: seed zero timers
        var runtimeMock = new Mock<IWorkflowRuntime>();
        var provider = BuildProvider(runtimeMock);
        var db = provider.GetRequiredService<WorkflowDbContext>();

        // Add unrelated (non-timer) task for safety
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "WF",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.WorkflowDefinitions.Add(def);
        await db.SaveChangesAsync();

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
        db.WorkflowInstances.Add(inst);
        await db.SaveChangesAsync();

        db.WorkflowTasks.Add(new WorkflowTask
        {
            TenantId = 1,
            WorkflowInstanceId = inst.Id,
            NodeId = "human1",
            TaskName = "Human",
            NodeType = "human",
            Status = WorkflowTaskStatus.Created,
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var worker = CreateWorker(provider, batchSize: 5);

        // Act
        await InvokeSingleCycleAsync(worker);

        // Assert: no task status changes (still created human task only)
        var tasks = await db.WorkflowTasks.ToListAsync();
        tasks.Should().ContainSingle();
        tasks[0].Status.Should().Be(WorkflowTaskStatus.Created);
    }

    [Fact]
    public async Task ProcessCycle_AllTimersLessThanBatch_ShouldAcquireAll()
    {
        // Arrange: 3 timers only, batch size 10
        var (provider, db) = await SeedDueTimersAsync(totalTimers: 3);
        var worker = CreateWorker(provider, batchSize: 10);

        // Act
        await InvokeSingleCycleAsync(worker);

        // Assert
        var all = await db.WorkflowTasks.ToListAsync();
        all.Should().AllSatisfy(t => t.Status.Should().Be(WorkflowTaskStatus.InProgress));
    }
}
