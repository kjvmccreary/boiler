using FluentAssertions;
using Xunit;
using WorkflowService.Engine.Executors;
using WorkflowService.Domain.Models;
using WorkflowService.Domain.Dsl;
using Microsoft.Extensions.Logging;
using Moq;
using DTOs.Workflow.Enums;

namespace WorkflowService.Tests.Engine.Executors;

public class HumanTaskExecutorTests : TestBase
{
    private readonly HumanTaskExecutor _executor;

    public HumanTaskExecutorTests()
    {
        var mockLogger = CreateMockLogger<HumanTaskExecutor>();
        _executor = new HumanTaskExecutor(mockLogger.Object);
    }

    [Fact]
    public void CanExecute_HumanTaskNode_ShouldReturnTrue()
    {
        // Arrange
        var node = new WorkflowNode
        {
            Id = "task1",
            Type = NodeTypes.HumanTask,
            Properties = new Dictionary<string, object>()
        };

        // Act
        var result = _executor.CanExecute(node);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_NonHumanTaskNode_ShouldReturnFalse()
    {
        // Arrange
        var node = new WorkflowNode
        {
            Id = "start1",
            Type = NodeTypes.Start,
            Properties = new Dictionary<string, object>()
        };

        // Act
        var result = _executor.CanExecute(node);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExecuteAsync_ValidHumanTask_ShouldCreateTask()
    {
        // Arrange
        var node = new WorkflowNode
        {
            Id = "approval_task",
            Type = NodeTypes.HumanTask,
            Properties = new Dictionary<string, object>
            {
                ["taskName"] = "Approve Request",
                ["instructions"] = "Please review and approve this request",
                ["assignedToRole"] = "approvers"
            }
        };

        var instance = new WorkflowInstance
        {
            Id = 1,
            TenantId = 1,
            WorkflowDefinitionId = 1,
            Status = InstanceStatus.Running,
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };

        var context = "{}";

        // Act
        var result = await _executor.ExecuteAsync(node, instance, context);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ShouldWait.Should().BeTrue();
        result.CreatedTask.Should().NotBeNull();
        result.CreatedTask!.TaskName.Should().Be("Approve Request");
        result.CreatedTask.AssignedToRole.Should().Be("approvers");
        result.CreatedTask.Status.Should().Be(DTOs.Workflow.Enums.TaskStatus.Assigned);
    }

    [Fact]
    public async Task ExecuteAsync_TaskWithUserId_ShouldAssignToUser()
    {
        // Arrange
        var node = new WorkflowNode
        {
            Id = "personal_task",
            Type = NodeTypes.HumanTask,
            Properties = new Dictionary<string, object>
            {
                ["taskName"] = "Personal Task",
                ["assignedToUserId"] = "123"
            }
        };

        var instance = new WorkflowInstance
        {
            Id = 1,
            TenantId = 1,
            WorkflowDefinitionId = 1,
            Status = InstanceStatus.Running,
            Context = "{}",
            StartedAt = DateTime.UtcNow
        };

        var context = "{}";

        // Act
        var result = await _executor.ExecuteAsync(node, instance, context);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.CreatedTask.Should().NotBeNull();
        result.CreatedTask!.AssignedToUserId.Should().Be(123);
        result.CreatedTask.Status.Should().Be(DTOs.Workflow.Enums.TaskStatus.Assigned);
    }
}
