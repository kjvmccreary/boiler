using FluentAssertions;
using Xunit;
using WorkflowService.Engine.Executors;
using WorkflowService.Domain.Models;
using WorkflowService.Domain.Dsl;
using Microsoft.Extensions.Logging;
using Moq;
using DTOs.Workflow.Enums;
using Contracts.Services; // ✅ ADD: Import for IRoleService

namespace WorkflowService.Tests.Engine.Executors;

public class HumanTaskExecutorTests : TestBase
{
    private readonly HumanTaskExecutor _executor;
    private readonly Mock<IRoleService> _mockRoleService; // ✅ ADD: Mock role service

    public HumanTaskExecutorTests()
    {
        var mockLogger = CreateMockLogger<HumanTaskExecutor>();
        _mockRoleService = new Mock<IRoleService>(); // ✅ ADD: Initialize mock
        
        // ✅ FIXED: Pass both logger and role service to constructor
        _executor = new HumanTaskExecutor(mockLogger.Object, _mockRoleService.Object);
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

        // ✅ ADD: Setup mock role service behavior
        _mockRoleService.Setup(x => x.IsRoleNameAvailableAsync("approvers", null, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(false); // Role exists (not available means it exists)

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

    // ✅ ADD: Test for role validation behavior
    [Fact]
    public async Task ExecuteAsync_WithNonExistentRole_ShouldStillCreateTask()
    {
        // Arrange
        var node = new WorkflowNode
        {
            Id = "task_with_invalid_role",
            Type = NodeTypes.HumanTask,
            Properties = new Dictionary<string, object>
            {
                ["taskName"] = "Task with Invalid Role",
                ["assignedToRole"] = "nonexistent_role"
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

        // ✅ ADD: Setup mock role service to indicate role doesn't exist
        _mockRoleService.Setup(x => x.IsRoleNameAvailableAsync("nonexistent_role", null, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(true); // Role doesn't exist (available means it doesn't exist)

        // Act
        var result = await _executor.ExecuteAsync(node, instance, context);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.CreatedTask.Should().NotBeNull();
        result.CreatedTask!.AssignedToRole.Should().Be("nonexistent_role");
        
        // ✅ Verify that role validation was called
        _mockRoleService.Verify(x => x.IsRoleNameAvailableAsync("nonexistent_role", null, It.IsAny<CancellationToken>()), 
                               Times.Once);
    }
}
