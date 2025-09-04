using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Validation;
using Xunit;

namespace WorkflowService.Tests.Validation;

public class GatewayStrategyValidationTests
{
    private readonly WorkflowPublishValidator _validator;

    public GatewayStrategyValidationTests()
    {
        _validator = new WorkflowPublishValidator(new NullLogger<WorkflowPublishValidator>());
    }

    private static WorkflowNode GatewayNode(object strategy)
        => new WorkflowNode
        {
            Id = "gw1",
            Name = "Gateway 1",
            Type = NodeTypes.Gateway,
            Properties = new Dictionary<string, object>
            {
                { "strategy", strategy }
            }
        };

    [Fact]
    public void AbTest_ValidConfig_ShouldPass()
    {
        var node = GatewayNode(new
        {
            kind = "abTest",
            config = new
            {
                keyPath = "user.id",
                variants = new[]
                {
                    new { weight = 70, target = "pathA" },
                    new { weight = 30, target = "pathB" }
                }
            }
        });

        var def = new WorkflowDefinition { Id = 1, Name = "Test", JSONDefinition = "{}" };
        var errors = _validator.Validate(def, new[] { node });

        errors.Should().BeEmpty();
    }

    [Fact]
    public void AbTest_MissingKeyPath_ShouldFail()
    {
        var node = GatewayNode(new
        {
            kind = "abTest",
            config = new
            {
                variants = new[]
                {
                    new { weight = 70, target = "A" },
                    new { weight = 30, target = "B" }
                }
            }
        });

        var def = new WorkflowDefinition { Id = 2, Name = "Test", JSONDefinition = "{}" };
        var errors = _validator.Validate(def, new[] { node });

        errors.Should().Contain(e => e.Contains("keyPath", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void AbTest_WeightsNotSumming_ShouldFail()
    {
        var node = GatewayNode(new
        {
            kind = "abTest",
            config = new
            {
                keyPath = "user.id",
                variants = new[]
                {
                    new { weight = 50, target = "A" },
                    new { weight = 25, target = "B" } // sums 75
                }
            }
        });

        var def = new WorkflowDefinition { Id = 3, Name = "Test", JSONDefinition = "{}" };
        var errors = _validator.Validate(def, new[] { node });

        errors.Should().Contain(e => e.Contains("weights must sum", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void AbTest_DuplicateTargets_ShouldFail()
    {
        var node = GatewayNode(new
        {
            kind = "abTest",
            config = new
            {
                keyPath = "user.id",
                variants = new[]
                {
                    new { weight = 60, target = "A" },
                    new { weight = 40, target = "A" }
                }
            }
        });

        var def = new WorkflowDefinition { Id = 4, Name = "Test", JSONDefinition = "{}" };
        var errors = _validator.Validate(def, new[] { node });

        errors.Should().Contain(e => e.Contains("duplicate variant target", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void AbTest_ZeroWeight_ShouldFail()
    {
        var node = GatewayNode(new
        {
            kind = "abTest",
            config = new
            {
                keyPath = "user.id",
                variants = new[]
                {
                    new { weight = 0, target = "A" },
                    new { weight = 100, target = "B" }
                }
            }
        });

        var def = new WorkflowDefinition { Id = 5, Name = "Test", JSONDefinition = "{}" };
        var errors = _validator.Validate(def, new[] { node });

        errors.Should().Contain(e => e.Contains("weight must be > 0", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void AbTest_NotEnoughVariants_ShouldFail()
    {
        var node = GatewayNode(new
        {
            kind = "abTest",
            config = new
            {
                keyPath = "user.id",
                variants = new[]
                {
                    new { weight = 100, target = "A" }
                }
            }
        });

        var def = new WorkflowDefinition { Id = 6, Name = "Test", JSONDefinition = "{}" };
        var errors = _validator.Validate(def, new[] { node });

        errors.Should().Contain(e => e.Contains("at least", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NonGatewayNode_WithAbTestObject_ShouldNotTriggerStrategyValidation()
    {
        var node = new WorkflowNode
        {
            Id = "auto1",
            Type = NodeTypes.Automatic,
            Name = "Auto",
            Properties = new Dictionary<string, object>
            {
                {
                    "strategy",
                    new {
                        kind = "abTest",
                        config = new {
                            keyPath = "user.id",
                            variants = new[] { new { weight = 50, target = "X" }, new { weight = 50, target = "Y" } }
                        }
                    }
                }
            }
        };

        // Strategy validation should not run because node is not gateway (could be noise)
        var def = new WorkflowDefinition { Id = 7, Name = "Test", JSONDefinition = "{}" };
        var errors = _validator.Validate(def, new[] { node });

        // It should ignore the strategy, producing no errors (automatic node action missing is separate concern).
        // Since automatic node lacks 'action', expect an error related to that but not abTest specifics.
        errors.Should().NotContain(e => e.Contains("abTest", StringComparison.OrdinalIgnoreCase));
    }
}
