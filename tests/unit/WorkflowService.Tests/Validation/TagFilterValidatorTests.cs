using System;
using System.Linq;
using FluentAssertions;
using WorkflowService.Services.Validation;
using Xunit;

namespace WorkflowService.Tests.Validation;

public class TagFilterValidatorTests
{
    [Fact]
    public void Validate_ValidDistinctLists_Succeeds()
    {
        var r = TagFilterValidator.Validate("Alpha,Beta,Alpha", "Core, Platform");
        r.IsValid.Should().BeTrue();
        r.Errors.Should().BeEmpty();
        r.NormalizedAny.Should().Be("Alpha,Beta");
        r.NormalizedAll.Should().Be("Core,Platform");
    }

    [Fact]
    public void Validate_TooManyAnyTags_Fails()
    {
        var any = string.Join(",", Enumerable.Range(1, 13).Select(i => $"T{i}"));
        var r = TagFilterValidator.Validate(any, null);
        r.IsValid.Should().BeFalse();
        r.Errors.Should().ContainSingle(e => e.Contains("anyTags: too many tags"));
    }

    [Fact]
    public void Validate_TagTooLong_Fails()
    {
        var longTag = new string('x', 41);
        var r = TagFilterValidator.Validate(longTag, null);
        r.IsValid.Should().BeFalse();
        r.Errors.Should().Contain(e => e.Contains("exceeds 40"));
    }

    [Fact]
    public void Validate_OnlyDelimiters_FailsNoValidParsed()
    {
        var r = TagFilterValidator.Validate(",,,", ", ,");
        r.IsValid.Should().BeFalse();
        r.Errors.Should().Contain(e => e.StartsWith("anyTags: no valid tags"));
        r.Errors.Should().Contain(e => e.StartsWith("allTags: no valid tags"));
    }

    [Fact]
    public void Validate_MixedErrors_AggregatesAll()
    {
        var longTag = new string('y', 50);
        var any = string.Join(",", Enumerable.Range(1, 13).Select(i => $"T{i}"));
        var r = TagFilterValidator.Validate(any, longTag);
        r.IsValid.Should().BeFalse();
        r.Errors.Should().HaveCountGreaterOrEqualTo(2);
        r.Errors.Any(e => e.Contains("too many tags")).Should().BeTrue();
        r.Errors.Any(e => e.Contains("exceeds 40")).Should().BeTrue();
    }
}
