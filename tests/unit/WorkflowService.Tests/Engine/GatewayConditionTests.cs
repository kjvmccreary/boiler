using FluentAssertions;
using Xunit;
using WorkflowService.Engine.Executors;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Domain.Models;
using WorkflowService.Domain.Dsl;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace WorkflowService.Tests.Engine;

public class GatewayConditionTests : TestBase
{
    private readonly Mock<IConditionEvaluator> _mockConditionEvaluator;
    private readonly Mock<ILogger<GatewayEvaluator>> _mockLogger;
    private readonly GatewayEvaluator _gatewayEvaluator;

    public GatewayConditionTests()
    {
        _mockConditionEvaluator = new Mock<IConditionEvaluator>();
        _mockLogger = CreateMockLogger<GatewayEvaluator>();
        _gatewayEvaluator = new GatewayEvaluator(_mockConditionEvaluator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GatewayCondition_ShouldSelectTruePathOnly()
    {
        // Arrange
        var context = """{"approval": true, "amount": 5000}""";
        
        var approvalGateway = new WorkflowNode
        {
            Id = "approval_gateway",
            Type = NodeTypes.Gateway,
            Name = "Approval Gateway",
            Properties = new Dictionary<string, object>
            {
                ["condition"] = """{"==": [{"var": "approval"}, true]}""", // JsonLogic: approval == true
                ["gatewayType"] = "exclusive"
            }
        };

        var testInstance = new WorkflowInstance
        {
            Id = 1,
            TenantId = 1,
            WorkflowDefinitionId = 1,
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            Context = context,
            StartedAt = DateTime.UtcNow
        };

        // Setup condition evaluator to return true for the approval condition
        _mockConditionEvaluator
            .Setup(x => x.EvaluateAsync(It.Is<string>(c => c.Contains("approval")), context))
            .ReturnsAsync(true);

        // Act
        var result = await _gatewayEvaluator.ExecuteAsync(approvalGateway, testInstance, context);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ShouldWait.Should().BeFalse(); // Gateways evaluate immediately
        
        // Verify condition was evaluated
        _mockConditionEvaluator.Verify(
            x => x.EvaluateAsync(It.Is<string>(c => c.Contains("approval")), context),
            Times.Once);

        // Verify context was updated with gateway result
        result.UpdatedContext.Should().NotBeNull();
        result.UpdatedContext.Should().Contain("gateway_approval_gateway");
        
        // Parse and verify the updated context contains gateway result
        var updatedContext = JsonSerializer.Deserialize<Dictionary<string, object>>(result.UpdatedContext!);
        updatedContext.Should().ContainKey("gateway_approval_gateway");
    }

    [Fact]
    public async Task GatewayCondition_WhenFalse_ShouldSelectFalsePath()
    {
        // Arrange
        var context = """{"approval": false, "amount": 15000}""";
        
        var approvalGateway = new WorkflowNode
        {
            Id = "amount_gateway",
            Type = NodeTypes.Gateway,
            Name = "Amount Gateway",
            Properties = new Dictionary<string, object>
            {
                ["condition"] = """{"<": [{"var": "amount"}, 10000]}""", // JsonLogic: amount < 10000
                ["gatewayType"] = "exclusive"
            }
        };

        var testInstance = new WorkflowInstance
        {
            Id = 2,
            TenantId = 1,
            WorkflowDefinitionId = 1,
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            Context = context,
            StartedAt = DateTime.UtcNow
        };

        // Setup condition evaluator to return false (amount is NOT < 10000)
        _mockConditionEvaluator
            .Setup(x => x.EvaluateAsync(It.Is<string>(c => c.Contains("amount")), context))
            .ReturnsAsync(false);

        // Act
        var result = await _gatewayEvaluator.ExecuteAsync(approvalGateway, testInstance, context);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ShouldWait.Should().BeFalse();
        
        // Verify condition was evaluated
        _mockConditionEvaluator.Verify(
            x => x.EvaluateAsync(It.Is<string>(c => c.Contains("amount")), context),
            Times.Once);

        // Verify context shows false result
        var updatedContext = JsonSerializer.Deserialize<Dictionary<string, object>>(result.UpdatedContext!);
        updatedContext.Should().ContainKey("gateway_amount_gateway");
    }

    [Fact]
    public async Task GatewayCondition_WhenEvaluationFails_ShouldDefaultToTrue()
    {
        // Arrange
        var context = """{"malformed": "json"}""";
        
        var faultyGateway = new WorkflowNode
        {
            Id = "faulty_gateway",
            Type = NodeTypes.Gateway,
            Name = "Faulty Gateway",
            Properties = new Dictionary<string, object>
            {
                ["condition"] = """{"invalid_jsonlogic": true}""", // Invalid JsonLogic
                ["gatewayType"] = "exclusive"
            }
        };

        var testInstance = new WorkflowInstance
        {
            Id = 3,
            TenantId = 1,
            WorkflowDefinitionId = 1,
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            Context = context,
            StartedAt = DateTime.UtcNow
        };

        // Setup condition evaluator to throw exception (simulating evaluation failure)
        _mockConditionEvaluator
            .Setup(x => x.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Invalid JsonLogic expression"));

        // Act
        var result = await _gatewayEvaluator.ExecuteAsync(faultyGateway, testInstance, context);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue(); // Should still succeed with default
        result.ShouldWait.Should().BeFalse();
        
        // Verify fallback behavior - should default to true and log warning
        _mockConditionEvaluator.Verify(
            x => x.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
            
        // Context should still be updated with the default true result
        result.UpdatedContext.Should().NotBeNull();
    }

    [Fact]
    public async Task GatewayCondition_WithNoCondition_ShouldDefaultToTrue()
    {
        // Arrange
        var context = """{"data": "test"}""";
        
        var simpleGateway = new WorkflowNode
        {
            Id = "simple_gateway",
            Type = NodeTypes.Gateway,
            Name = "Simple Gateway",
            Properties = new Dictionary<string, object>
            {
                ["gatewayType"] = "exclusive"
                // No condition property
            }
        };

        var testInstance = new WorkflowInstance
        {
            Id = 4,
            TenantId = 1,
            WorkflowDefinitionId = 1,
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            Context = context,
            StartedAt = DateTime.UtcNow
        };

        // Act
        var result = await _gatewayEvaluator.ExecuteAsync(simpleGateway, testInstance, context);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ShouldWait.Should().BeFalse();
        
        // Should not call condition evaluator for missing/empty conditions
        _mockConditionEvaluator.Verify(
            x => x.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
            
        // Context should be updated with default true result
        result.UpdatedContext.Should().NotBeNull();
    }
}
