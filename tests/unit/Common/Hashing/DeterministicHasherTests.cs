using Common.Hashing;
using Contracts.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace UnitTests.Common.Hashing;

public class DeterministicHasherTests
{
    private IDeterministicHasher Create(ulong? seed = null)
    {
        var opts = Options.Create(new DeterministicHashOptions { Seed = seed });
        return new DeterministicHasher(opts, NullLogger<DeterministicHasher>.Instance);
    }

    [Fact]
    public void Hash_SameInput_SameOutput()
    {
        var h = Create();
        var a = h.Hash("sample-key");
        var b = h.Hash("sample-key");
        Assert.Equal(a, b);
    }

    [Fact]
    public void Hash_DifferentSeeds_DifferentOutputsLikely()
    {
        var h1 = Create(1);
        var h2 = Create(2);
        var v1 = h1.Hash("x");
        var v2 = h2.Hash("x");
        Assert.NotEqual(v1, v2);
    }

    [Fact]
    public void ToBucket_WithinRange()
    {
        var h = Create();
        var hash = h.Hash("tenant-123/user-456/featureX");
        var bucket = h.ToBucket(hash, 100);
        Assert.InRange(bucket, 0, 99);
    }

    [Fact]
    public void HashComposite_PositionalDifference()
    {
        var h = Create();
        var v1 = h.HashComposite("A", "B", "C");
        var v2 = h.HashComposite("A", "C", "B");
        Assert.NotEqual(v1, v2);
    }

    [Fact]
    public void Hash_Empty_ReturnsZero()
    {
        var h = Create();
        Assert.Equal(0UL, h.Hash(string.Empty));
        Assert.Equal(0UL, h.Hash((string?)null));
    }
}
