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
    public async Task StartInstanceAsync_ValidDefinition_ShouldStartInstance()
    {
        // Arrange
        var definition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Test Workflow",
            Version = 1,
            JSONDefinition = """
            {
                "nodes": [
                    {"id": "start", "type": "Start"},
                    {"id": "end", "type": "End"}
                ],
                "edges": [
                    {"from": "start", "to": "end"}
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

        var mockInstance = new WorkflowInstance
        {
            Id = 1,
            TenantId = 1,
            WorkflowDefinitionId = definition.Id,
            DefinitionVersion = definition.Version,
            Status = InstanceStatus.Running,
            CurrentNodeIds = """["start"]""",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            StartedByUserId = 1
        };

        _mockWorkflowRuntime.Setup(x => x.StartWorkflowAsync(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockInstance);

        var request = new StartInstanceRequestDto
        {
            WorkflowDefinitionId = definition.Id,
            InitialContext = """{"requestId": 123}"""
        };

        // Act
        var result = await _instanceService.StartInstanceAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Status.Should().Be(InstanceStatus.Running);
        result.Data.WorkflowDefinitionId.Should().Be(definition.Id);

        _mockWorkflowRuntime.Verify(x => x.StartWorkflowAsync(
            definition.Id, """{"requestId": 123}""", It.IsAny<int?>(), It.IsAny<CancellationToken>()),
            Times.Once);
        
        _mockEventPublisher.Verify(x => x.PublishInstanceStartedAsync(
            It.IsAny<WorkflowInstance>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task StartInstanceAsync_UnpublishedDefinition_ShouldReturnError()
    {
        // Arrange
        var definition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Draft Workflow",
            Version = 1,
            JSONDefinition = "{}",
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(definition);
        await DbContext.SaveChangesAsync();

        var request = new StartInstanceRequestDto
        {
            WorkflowDefinitionId = definition.Id,
            InitialContext = "{}"
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
        var definition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Test Workflow",
            Version = 1,
            JSONDefinition = "{}",
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
            CurrentNodeIds = """["start"]""",
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };

        definition.Instances = new List<WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();

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
    public async Task SignalAsync_ValidInstance_ShouldSignalWorkflow()
    {
        // Arrange
        var definition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Test Workflow",
            Version = 1,
            JSONDefinition = "{}",
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
            CurrentNodeIds = """["waiting"]""",
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };

        definition.Instances = new List<WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();

        var request = new SignalInstanceRequestDto
        {
            SignalName = "approve",
            SignalData = """{"approved": true}"""
        };

        // Act
        var result = await _instanceService.SignalAsync(instance.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();

        _mockWorkflowRuntime.Verify(x => x.SignalWorkflowAsync(
            instance.Id, "approve", """{"approved": true}""", It.IsAny<int?>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task TerminateAsync_RunningInstance_ShouldTerminate()
    {
        // Arrange
        var definition = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Test Workflow",
            Version = 1,
            JSONDefinition = "{}",
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
            StartedAt = DateTime.UtcNow
        };

        definition.Instances = new List<WorkflowInstance> { instance };
        instance.WorkflowDefinition = definition;

        DbContext.WorkflowDefinitions.Add(definition);
        DbContext.WorkflowInstances.Add(instance);
        await DbContext.SaveChangesAsync();

        var request = new TerminateInstanceRequestDto
        {
            Reason = "User cancelled operation"
        };

        // Act
        var result = await _instanceService.TerminateAsync(instance.Id, request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        _mockWorkflowRuntime.Verify(x => x.CancelWorkflowAsync(
            instance.Id, "User cancelled operation", It.IsAny<int?>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
