namespace Contracts.Services;

/// Deterministic, non-cryptographic 64-bit hashing service (stable across deployments).
/// Used for variant bucketing (A/B tests, gateway strategies, etc.).
public interface IDeterministicHasher
{
    /// Algorithm name (e.g., "XxHash64").
    string Algorithm { get; }

    /// Seed used for hashing (surface for diagnostics / auditing).
    ulong Seed { get; }

    /// Compute 64-bit hash for a UTF-8 string (null/empty -> 0).
    ulong Hash(string? value);

    /// Compute 64-bit hash for raw bytes.
    ulong Hash(ReadOnlySpan<byte> data);

    /// Reduced bucket (0 <= bucket < bucketCount).
    int ToBucket(ulong hash, int bucketCount);

    /// Convenience: hash a composite key with separators (no allocations for null segments).
    ulong HashComposite(params string?[] parts);
}
