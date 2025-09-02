using System; // Needed for StringComparison
using FluentAssertions;
using Xunit;
using WorkflowService.Services.Validation;
using WorkflowService.Domain.Dsl;

namespace WorkflowService.Tests.Services.Validation;

public class GraphValidationTests : TestBase
{
    private readonly WorkflowGraphValidator _validator = new();

    [Fact]
    public void Validation_ShouldRejectMultipleStartNodes()
    {
        var def = new WorkflowDefinitionJson
        {
            Id = "wf",
            Name = "multi-start",
            Nodes = new()
            {
                new() { Id = "s1", Type = "Start" },
                new() { Id = "s2", Type = "Start" },
                new() { Id = "end", Type = "End" }
            },
            Edges = new()
            {
                new() { Id = "e1", Source = "s1", Target = "end" },
                new() { Id = "e2", Source = "s2", Target = "end" }
            }
        };
        var result = _validator.ValidateForPublish(def);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Exactly one Start node is required") && e.Contains("found 2"));
    }

    [Fact]
    public void Validation_ShouldRejectNoStartNode()
    {
        var def = new WorkflowDefinitionJson
        {
            Id = "wf",
            Name = "no-start",
            Nodes = new()
            {
                new() { Id = "task", Type = "HumanTask" },
                new() { Id = "end", Type = "End" }
            },
            Edges = new()
            {
                new() { Id = "e1", Source = "task", Target = "end" }
            }
        };
        var result = _validator.ValidateForPublish(def);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Exactly one Start node is required") && e.Contains("found 0"));
    }

    [Fact]
    public void Validation_ShouldRejectNoEndNodes()
    {
        var def = new WorkflowDefinitionJson
        {
            Id = "wf",
            Name = "no-end",
            Nodes = new()
            {
                new() { Id = "start", Type = "Start" },
                new() { Id = "task", Type = "HumanTask" }
            },
            Edges = new()
            {
                new() { Id = "e1", Source = "start", Target = "task" }
            }
        };
        var result = _validator.ValidateForPublish(def);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("At least one End node is required"));
    }

    [Fact]
    public void Validation_ShouldRejectDuplicateNodeIds()
    {
        var def = new WorkflowDefinitionJson
        {
            Id = "duplicate-ids-workflow",
            Name = "Workflow with Duplicate Node IDs",
            Nodes = new List<WorkflowNode>
            {
                new() { Id = "start1", Type = "Start", Name = "Start Node" },
                new() { Id = "task", Type = "HumanTask", Name = "Task 1" },
                new() { Id = "task", Type = "HumanTask", Name = "Task 1 Duplicate" },
                new() { Id = "end1", Type = "End", Name = "End Node" }
            },
            Edges = new List<WorkflowEdge>
            {
                new() { Id = "e1", Source = "start1", Target = "task" },
                new() { Id = "e2", Source = "task", Target = "end1" }
            }
        };

        var result = _validator.ValidateForPublish(def);

        result.IsValid.Should().BeFalse();
        result.Errors.Should()
            .Contain(e => e.Contains("Duplicate node id", StringComparison.OrdinalIgnoreCase)
                          && e.Contains("task"));
    }

    [Fact]
    public void Validation_ShouldRejectUnreachableNodes()
    {
        var def = new WorkflowDefinitionJson
        {
            Id = "wf",
            Name = "unreachable",
            Nodes = new()
            {
                new() { Id = "start", Type = "Start" },
                new() { Id = "ok", Type = "HumanTask" },
                new() { Id = "orphan", Type = "HumanTask" },
                new() { Id = "end", Type = "End" }
            },
            Edges = new()
            {
                new() { Id = "e1", Source = "start", Target = "ok" },
                new() { Id = "e2", Source = "ok", Target = "end" }
            }
        };
        var result = _validator.ValidateForPublish(def);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Unreachable") && e.Contains("orphan"));
    }

    [Fact]
    public void Validation_ShouldRejectOrphanedEdges()
    {
        var def = new WorkflowDefinitionJson
        {
            Id = "wf",
            Name = "edge",
            Nodes = new()
            {
                new() { Id = "start", Type = "Start" },
                new() { Id = "end", Type = "End" }
            },
            Edges = new()
            {
                new() { Id = "e1", Source = "start", Target = "ghost" },
                new() { Id = "e2", Source = "ghost2", Target = "end" }
            }
        };
        var result = _validator.ValidateForPublish(def);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Edge e1") && e.Contains("ghost"));
        result.Errors.Should().Contain(e => e.Contains("Edge e2") && e.Contains("ghost2"));
    }

    [Fact]
    public void Validation_ShouldAcceptValidWorkflow()
    {
        var def = new WorkflowDefinitionJson
        {
            Id = "wf",
            Name = "valid",
            Nodes = new()
            {
                new() { Id = "start", Type = "Start" },
                new() { Id = "task", Type = "HumanTask" },
                new() { Id = "end", Type = "End" }
            },
            Edges = new()
            {
                new() { Id = "e1", Source = "start", Target = "task" },
                new() { Id = "e2", Source = "task", Target = "end" }
            }
        };
        var result = _validator.ValidateForPublish(def);
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
