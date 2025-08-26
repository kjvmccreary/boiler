using FluentAssertions;
using Xunit;
using WorkflowService.Engine;
using Microsoft.Extensions.Logging;
using Moq;

namespace WorkflowService.Tests.Engine;

public class ConditionEvaluatorTests : TestBase
{
    private readonly JsonLogicConditionEvaluator _evaluator;

    public ConditionEvaluatorTests()
    {
        var mockLogger = CreateMockLogger<JsonLogicConditionEvaluator>();
        _evaluator = new JsonLogicConditionEvaluator(mockLogger.Object);
    }

    [Fact]
    public async Task EvaluateAsync_EmptyCondition_ShouldReturnTrue()
    {
        // Arrange
        var condition = "";
        var context = "{}";

        // Act
        var result = await _evaluator.EvaluateAsync(condition, context);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EvaluateAsync_SimpleEquality_ShouldEvaluateCorrectly()
    {
        // Arrange
        var condition = """{"==": [{"var": "approve"}, true]}""";
        var context = """{"approve": true}""";

        // Act
        var result = await _evaluator.EvaluateAsync(condition, context);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EvaluateAsync_SimpleEquality_False_ShouldEvaluateCorrectly()
    {
        // Arrange
        var condition = """{"==": [{"var": "approve"}, true]}""";
        var context = """{"approve": false}""";

        // Act
        var result = await _evaluator.EvaluateAsync(condition, context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task EvaluateAsync_ComplexCondition_ShouldEvaluateCorrectly()
    {
        // Arrange
        var condition = """
        {
            "and": [
                {"==": [{"var": "status"}, "approved"]},
                {">": [{"var": "amount"}, 1000]}
            ]
        }
        """;
        var context = """{"status": "approved", "amount": 1500}""";

        // Act
        var result = await _evaluator.EvaluateAsync(condition, context);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateCondition_ValidJsonLogic_ShouldReturnTrue()
    {
        // Arrange
        var condition = """{"==": [{"var": "test"}, true]}""";

        // Act
        var result = _evaluator.ValidateCondition(condition);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateCondition_InvalidJson_ShouldReturnFalse()
    {
        // Arrange
        var condition = "invalid json";

        // Act
        var result = _evaluator.ValidateCondition(condition);

        // Assert
        result.Should().BeFalse();
    }
}
