using FluentAssertions;
using Moq;
using Xunit;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using DTOs.Workflow;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using Contracts.Services;
using Microsoft.Extensions.Logging;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Tests.Services;

public class AdminServiceTests : TestBase
{
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly Mock<IWorkflowRuntime> _mockWorkflowRuntime;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly Mock<ILogger<AdminService>> _mockLogger;
    private readonly AdminService _adminService;

    public AdminServiceTests()
    {
        _mockTenantProvider = new Mock<ITenantProvider>();
        _mockWorkflowRuntime = new Mock<IWorkflowRuntime>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _mockLogger = CreateMockLogger<AdminService>();

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        _adminService = new AdminService(
            DbContext,
            Mapper,
            _mockTenantProvider.Object,
            _mockWorkflowRuntime.Object,
            _mockEventPublisher.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task RetryInstanceAsync_FailedInstance_ShouldRetrySuccessfully()
    {
        // Arrange
        var instance = await CreateFailedInstanceAsync();
        var request = new RetryInstanceRequestDto
        {
            ResetToNodeId = "task1"
        };

        // Act
        var result = await _adminService.RetryInstanceAsync(instance.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();

        // ✅ FIXED: Add missing autoCommit parameter to avoid expression tree issues
        _mockWorkflowRuntime.Verify(x => x.RetryWorkflowAsync(
            It.Is<int>(instanceId => instanceId == instance.Id),
            It.Is<string>(nodeId => nodeId == "task1"),
            It.IsAny<CancellationToken>(),
            It.IsAny<bool>()),  // ✅ ADD: autoCommit parameter
            Times.Once);
    }

    [Fact]
    public async Task RetryInstanceAsync_RunningInstance_ShouldReturnError()
    {
        // Arrange
        var instance = await CreateRunningInstanceAsync();
        var request = new RetryInstanceRequestDto
        {
            ResetToNodeId = "task1"
        };

        // Act
        var result = await _adminService.RetryInstanceAsync(instance.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Only failed workflow instances can be retried");
    }

    [Fact]
    public async Task MoveToNodeAsync_ValidInstance_ShouldMoveSuccessfully()
    {
        // Arrange
        var instance = await CreateRunningInstanceAsync();
        var request = new MoveToNodeRequestDto
        {
            TargetNodeId = "task2",
            Reason = "Skipping problematic step",
            ContextUpdates = """{"skipReason": "Manual override"}"""
        };

        // Act
        var result = await _adminService.MoveToNodeAsync(instance.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();

        // ✅ FIXED: Add missing autoCommit parameter to avoid expression tree issues
        _mockWorkflowRuntime.Verify(x => x.ContinueWorkflowAsync(
            It.Is<int>(instanceId => instanceId == instance.Id),
            It.IsAny<CancellationToken>(),
            It.IsAny<bool>()),  // ✅ ADD: autoCommit parameter
            Times.Once);
    }

    [Fact]
    public async Task ForceCompleteAsync_RunningInstance_ShouldCompleteSuccessfully()
    {
        // Arrange
        var instance = await CreateRunningInstanceAsync();
        var request = new ForceCompleteRequestDto
        {
            Reason = "Emergency completion required"
        };

        // Act
        var result = await _adminService.ForceCompleteAsync(instance.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();

        // ✅ FIXED: Use explicit lambda to avoid expression tree issues
        _mockEventPublisher.Verify(x => x.PublishInstanceCompletedAsync(
            It.IsAny<WorkflowInstance>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAnalyticsAsync_WithInstances_ShouldReturnAnalytics()
    {
        // Arrange
        await SeedAnalyticsTestDataAsync();
        var request = new GetAnalyticsRequestDto
        {
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow
        };

        // Act
        var result = await _adminService.GetAnalyticsAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TotalInstances.Should().BeGreaterThan(0);
        result.Data.InstancesByStatus.Should().NotBeEmpty();
        result.Data.InstancesByDefinition.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetSystemHealthAsync_ShouldReturnHealthStatus()
    {
        // Arrange
        await SeedAnalyticsTestDataAsync();

        // Act
        var result = await _adminService.GetSystemHealthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Status.Should().NotBeNullOrEmpty();
        result.Data.SystemMetrics.Should().NotBeEmpty();
        result.Data.CheckedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task BulkOperationAsync_CancelMultipleInstances_ShouldProcessAll()
    {
        // Arrange
        var instances = await SeedBulkTestInstancesAsync();
        var request = new BulkInstanceOperationRequestDto
        {
            Operation = "cancel",
            Status = InstanceStatus.Running,
            Reason = "Bulk cancellation for maintenance"
        };

        // Act
        var result = await _adminService.BulkOperationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.OperationType.Should().Be("cancel");
        result.Data.SuccessCount.Should().BeGreaterThan(0);
        result.Data.TotalCount.Should().Be(instances.Count(i => i.Status == InstanceStatus.Running));

        // ✅ FIXED: Add missing autoCommit parameter to avoid expression tree issues
        _mockWorkflowRuntime.Verify(x => x.CancelWorkflowAsync(
            It.IsAny<int>(),
            It.Is<string>(reason => reason == "Bulk cancellation for maintenance"),
            It.IsAny<int?>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<bool>()),  // ✅ ADD: autoCommit parameter
            Times.AtLeastOnce);
    }

    private async Task<WorkflowInstance> CreateFailedInstanceAsync()
    {
        var definition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Failed Workflow",
            Version = 1,
            JSONDefinition = """
            {
                "nodes": [
                    {"id": "start", "type": "Start"},
                    {"id": "task1", "type": "HumanTask"},
                    {"id": "end", "type": "End"}
                ]
            }
            """,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var instance = new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Failed,
            CurrentNodeIds = """["task1"]""",
            Context = "{}",
            ErrorMessage = "Task execution failed",
            StartedAt = DateTime.UtcNow.AddHours(-1),
            CompletedAt = DateTime.UtcNow
        };

        definition.Instances = new List<WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();

        return instance;
    }

    private async Task<WorkflowInstance> CreateRunningInstanceAsync()
    {
        var definition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Running Workflow",
            Version = 1,
            JSONDefinition = """
            {
                "nodes": [
                    {"id": "start", "type": "Start"},
                    {"id": "task1", "type": "HumanTask"},
                    {"id": "task2", "type": "HumanTask"},
                    {"id": "end", "type": "End"}
                ]
            }
            """,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var instance = new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = """["task1"]""",
            Context = "{}",
            StartedAt = DateTime.UtcNow.AddMinutes(-30)
        };

        definition.Instances = new List<WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();

        return instance;
    }

    private async Task SeedAnalyticsTestDataAsync()
    {
        var definition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Analytics Test Workflow",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var instances = new[]
        {
            new WorkflowInstance
            {
                TenantId = 1,
                WorkflowDefinitionId = definition.Id,
                DefinitionVersion = 1,
                Status = InstanceStatus.Completed,
                CurrentNodeIds = "[]",
                Context = "{}",
                StartedAt = DateTime.UtcNow.AddDays(-2),
                CompletedAt = DateTime.UtcNow.AddDays(-2).AddHours(1)
            },
            new WorkflowInstance
            {
                TenantId = 1,
                WorkflowDefinitionId = definition.Id,
                DefinitionVersion = 1,
                Status = InstanceStatus.Running,
                CurrentNodeIds = """["task1"]""",
                Context = "{}",
                StartedAt = DateTime.UtcNow.AddHours(-1)
            },
            new WorkflowInstance
            {
                TenantId = 1,
                WorkflowDefinitionId = definition.Id,
                DefinitionVersion = 1,
                Status = InstanceStatus.Failed,
                CurrentNodeIds = """["task1"]""",
                Context = "{}",
                ErrorMessage = "Processing error",
                StartedAt = DateTime.UtcNow.AddDays(-1),
                CompletedAt = DateTime.UtcNow.AddDays(-1).AddMinutes(30)
            }
        };

        definition.Instances = instances.ToList();
        foreach (var instance in instances)
        {
            instance.WorkflowDefinition = definition;
        }

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.AddRange(instances);
        await DbContext.SaveChangesAsync();
    }

    private async Task<List<WorkflowInstance>> SeedBulkTestInstancesAsync()
    {
        var definition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Bulk Test Workflow",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var instances = new[]
        {
            new WorkflowInstance
            {
                TenantId = 1,
                WorkflowDefinitionId = definition.Id,
                DefinitionVersion = 1,
                Status = InstanceStatus.Running,
                CurrentNodeIds = """["task1"]""",
                Context = "{}",
                StartedAt = DateTime.UtcNow.AddHours(-2)
            },
            new WorkflowInstance
            {
                TenantId = 1,
                WorkflowDefinitionId = definition.Id,
                DefinitionVersion = 1,
                Status = InstanceStatus.Running,
                CurrentNodeIds = """["task2"]""",
                Context = "{}",
                StartedAt = DateTime.UtcNow.AddHours(-1)
            },
            new WorkflowInstance
            {
                TenantId = 1,
                WorkflowDefinitionId = definition.Id,
                DefinitionVersion = 1,
                Status = InstanceStatus.Completed,
                CurrentNodeIds = "[]",
                Context = "{}",
                StartedAt = DateTime.UtcNow.AddDays(-1),
                CompletedAt = DateTime.UtcNow.AddDays(-1).AddHours(1)
            }
        };

        definition.Instances = instances.ToList();
        foreach (var instance in instances)
        {
            instance.WorkflowDefinition = definition;
        }

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.AddRange(instances);
        await DbContext.SaveChangesAsync();

        return instances.ToList();
    }
}
