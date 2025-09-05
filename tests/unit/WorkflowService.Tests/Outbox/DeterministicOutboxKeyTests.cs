using System;
using Xunit;
using WorkflowService.Outbox;

namespace WorkflowService.Tests.Outbox;

public class DeterministicOutboxKeyTests
{
    [Fact]
    public void Instance_SameInputs_StableGuid()
    {
        var a = DeterministicOutboxKey.Instance(1, 42, "Started", 3);
        var b = DeterministicOutboxKey.Instance(1, 42, "started", 3);
        Assert.Equal(a, b);
    }

    [Fact]
    public void Instance_DifferentPhase_DifferentGuid()
    {
        var a = DeterministicOutboxKey.Instance(1, 42, "started", 3);
        var b = DeterministicOutboxKey.Instance(1, 42, "completed", 3);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void CrossTenant_Differs()
    {
        var a = DeterministicOutboxKey.Instance(1, 42, "started", 1);
        var b = DeterministicOutboxKey.Instance(2, 42, "started", 1);
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void DefinitionPublished_EquivalentToGeneric()
    {
        var viaSpecific = DeterministicOutboxKey.DefinitionPublished(4, 10, 2);
        var viaGeneric = DeterministicOutboxKey.Definition(4, 10, "published", 2);
        Assert.Equal(viaSpecific, viaGeneric);
    }

    [Fact]
    public void Custom_SameNormalizedInput_Stable()
    {
        var a = DeterministicOutboxKey.Custom(5, "CustomCategory", "Entity", 99, "Event");
        var b = DeterministicOutboxKey.Custom(5, "customcategory", "entity", 99, "event");
        Assert.Equal(a, b);
    }
}
