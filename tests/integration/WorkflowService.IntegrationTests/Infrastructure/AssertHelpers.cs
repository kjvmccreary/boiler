using System.Text.Json;
using FluentAssertions;

namespace WorkflowService.IntegrationTests.Infrastructure;

public static class AssertHelpers
{
    public static void AssertApiNotFound(string json)
    {
        var doc = JsonDocument.Parse(json);
        doc.RootElement.TryGetProperty("success", out var s).Should().BeTrue();
        s.GetBoolean().Should().BeFalse();
    }
}
