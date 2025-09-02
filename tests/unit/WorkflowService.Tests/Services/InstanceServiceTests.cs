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

public class InstanceServiceTests : TestBase
{
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly Mock<IWorkflowRuntime> _mockWorkflowRuntime;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly Mock<ILogger<InstanceService>> _mockLogger;
    private readonly InstanceService _instanceService;

    public InstanceServiceTests()
    {
        _mockTenantProvider = new Mock<ITenantProvider>();
        _mockWorkflowRuntime = new Mock<IWorkflowRuntime>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _mockLogger = CreateMockLogger<InstanceService>();

        _mockTenantProvider.Setup(x => x.GetCurrentTenantIdAsync())
            .ReturnsAsync(1);

        _instanceService = new InstanceService(
            DbContext,
            Mapper,
            _mockTenantProvider.Object,
            _mockWorkflowRuntime.Object,
            _mockEventPublisher.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task StartInstanceAsync_ValidDefinition_ShouldStartSuccessfully()
    {
        // Arrange
        var definition = await CreatePublishedDefinitionAsync();
        var request = new StartInstanceRequestDto
        {
            WorkflowDefinitionId = definition.Id,
            InitialContext = """{"startData": "test value"}"""
        };

        var mockInstance = new WorkflowInstance
        {
            Id = 1,
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            Status = InstanceStatus.Running,
            StartedAt = DateTime.UtcNow
        };

        // ✅ FIXED: Specify ALL parameters explicitly to avoid optional parameter expression tree issues
        _mockWorkflowRuntime.Setup(x => x.StartWorkflowAsync(
            It.Is<int>(defId => defId == definition.Id),
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<bool>()))  // ✅ ADD: autoCommit parameter
            .ReturnsAsync(mockInstance);

        // Act
        var result = await _instanceService.StartInstanceAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Status.Should().Be(InstanceStatus.Running);

        // ✅ FIXED: Specify ALL parameters explicitly to avoid optional parameter expression tree issues
        _mockWorkflowRuntime.Verify(x => x.StartWorkflowAsync(
            It.Is<int>(defId => defId == definition.Id),
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<bool>()),  // ✅ ADD: autoCommit parameter
            Times.Once);
    }

    [Fact]
    public async Task StartInstanceAsync_UnpublishedDefinition_ShouldReturnError()
    {
        // Arrange
        var definition = await CreateDraftDefinitionAsync();
        var request = new StartInstanceRequestDto
        {
            WorkflowDefinitionId = definition.Id,
            InitialContext = """{"startData": "test value"}"""
        };

        // Act
        var result = await _instanceService.StartInstanceAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Published workflow definition not found");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingInstance_ShouldReturnInstance()
    {
        // Arrange
        var instance = await CreateTestInstanceAsync();

        // Act
        var result = await _instanceService.GetByIdAsync(instance.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(instance.Id);
        result.Data.Status.Should().Be(InstanceStatus.Running);
    }

    [Fact]
    public async Task GetAllAsync_WithFilters_ShouldReturnFilteredResults()
    {
        // Arrange
        await SeedTestInstancesAsync();
        var request = new GetInstancesRequestDto
        {
            Status = InstanceStatus.Running,
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _instanceService.GetAllAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().NotBeEmpty();
        result.Data.Items.Should().OnlyContain(i => i.Status == InstanceStatus.Running);
    }

    [Fact]
    public async Task GetHistoryAsync_ValidInstance_ShouldReturnEvents()
    {
        // Arrange
        var instance = await CreateTestInstanceWithEventsAsync();

        // Act
        var result = await _instanceService.GetHistoryAsync(instance.Id);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Should().NotBeEmpty();
        result.Data.Should().OnlyContain(e => e.WorkflowInstanceId == instance.Id);
    }

    [Fact]
    public async Task SignalAsync_ValidInstance_ShouldTriggerContinuation()
    {
        // Arrange
        var instance = await CreateTestInstanceAsync();
        var request = new SignalInstanceRequestDto
        {
            SignalName = "approval_received",
            SignalData = """{"approved": true, "comments": "Looks good"}"""
        };

        // Act
        var result = await _instanceService.SignalAsync(instance.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task TerminateAsync_RunningInstance_ShouldTerminateSuccessfully()
    {
        // Arrange
        var instance = await CreateTestInstanceAsync();
        var request = new TerminateInstanceRequestDto
        {
            Reason = "Business requirement changed"
        };

        // Act
        var result = await _instanceService.TerminateAsync(instance.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();

        // ✅ FIXED: Specify ALL parameters explicitly to avoid optional parameter expression tree issues
        _mockWorkflowRuntime.Verify(x => x.CancelWorkflowAsync(
            It.Is<int>(instanceId => instanceId == instance.Id),
            It.Is<string>(reason => reason == "Business requirement changed"),
            It.IsAny<int?>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<bool>()),  // ✅ ADD: autoCommit parameter
            Times.Once);
    }

    private async Task<WorkflowDefinition> CreatePublishedDefinitionAsync()
    {
        var definition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Test Workflow",
            Version = 1,
            JSONDefinition = """
            {
                "key": "test-workflow",
                "nodes": [
                    {"id": "start", "type": "Start"},
                    {"id": "task1", "type": "HumanTask"},
                    {"id": "end", "type": "End"}
                ]
            }
            """,
            IsPublished = true,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.WorkflowDefinitions.Add(definition);
        await DbContext.SaveChangesAsync();
        return definition;
    }

    private async Task<WorkflowDefinition> CreateDraftDefinitionAsync()
    {
        var definition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Draft Workflow",
            Version = 1,
            JSONDefinition = """
            {
                "key": "draft-workflow",
                "nodes": [
                    {"id": "start", "type": "Start"},
                    {"id": "end", "type": "End"}
                ]
            }
            """,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        DbContext.WorkflowDefinitions.Add(definition);
        await DbContext.SaveChangesAsync();
        return definition;
    }

    private async Task<WorkflowInstance> CreateTestInstanceAsync()
    {
        var definition = await CreatePublishedDefinitionAsync();
        
        var instance = new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = """["task1"]""",
            Context = """{"started": true}""",
            StartedAt = DateTime.UtcNow
        };

        instance.WorkflowDefinition = definition;
        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();
        return instance;
    }

    private async Task SeedTestInstancesAsync()
    {
        var definition = await CreatePublishedDefinitionAsync();

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
                StartedAt = DateTime.UtcNow.AddHours(-2),
                CompletedAt = DateTime.UtcNow.AddMinutes(-30)
            }
        };

        foreach (var instance in instances)
        {
            instance.WorkflowDefinition = definition;
        }

        DbContext.WorkflowInstances.AddRange(instances);
        await DbContext.SaveChangesAsync();
    }

    private async Task<WorkflowInstance> CreateTestInstanceWithEventsAsync()
    {
        var instance = await CreateTestInstanceAsync();

        var events = new[]
        {
            new WorkflowEvent
            {
                TenantId = 1,
                WorkflowInstanceId = instance.Id,
                Type = "Instance",
                Name = "Started",
                Data = "{}",
                OccurredAt = DateTime.UtcNow.AddMinutes(-5)
            },
            new WorkflowEvent
            {
                TenantId = 1,
                WorkflowInstanceId = instance.Id,
                Type = "Task",
                Name = "Created",
                Data = """{"taskId": "task1"}""",
                OccurredAt = DateTime.UtcNow.AddMinutes(-3)
            }
        };

        DbContext.WorkflowEvents.AddRange(events);
        await DbContext.SaveChangesAsync();
        return instance;
    }
}
