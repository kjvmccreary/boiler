using System.Buffers;
using System.Text;
using Contracts.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Common.Hashing;

/// Deterministic, stable, non‑cryptographic hashing service.
/// Implementation: 64‑bit FNV‑1a with an additional seed XOR at start & final mix.
/// NOTE: While not cryptographically secure, distribution quality is adequate for A/B bucketing.
public sealed class DeterministicHasher : IDeterministicHasher
{
    private readonly ILogger<DeterministicHasher> _logger;
    private readonly Encoding _utf8 = new UTF8Encoding(false);
    public string Algorithm { get; }
    public ulong Seed { get; }

    // FNV‑1a 64 constants
    private const ulong FnvOffset = 14695981039346656037UL;
    private const ulong FnvPrime = 1099511628211UL;

    public DeterministicHasher(
        IOptions<DeterministicHashOptions> options,
        ILogger<DeterministicHasher> logger)
    {
        _logger = logger;
        var opts = options.Value ?? new DeterministicHashOptions();
        Seed = opts.Seed ?? DeterministicHashOptions.DefaultSeed;
        Algorithm = "FNV1A64+Seed";

        _logger.LogInformation("DeterministicHasher initialized (Algorithm={Algorithm}, Seed={Seed}). Changing seed breaks variant stability.",
            Algorithm, Seed);
    }

    public ulong Hash(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return 0UL;

        var byteCount = _utf8.GetByteCount(value);
        if (byteCount == 0) return 0UL;

        byte[]? rented = null;
        Span<byte> span = byteCount <= 1024
            ? stackalloc byte[byteCount]
            : (rented = ArrayPool<byte>.Shared.Rent(byteCount)).AsSpan(0, byteCount);

        try
        {
            _utf8.GetBytes(value, span);
            return Hash(span);
        }
        finally
        {
            if (rented != null)
                ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public ulong Hash(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty) return 0UL;

        // Seeded FNV‑1a
        ulong hash = FnvOffset ^ Seed;
        foreach (var b in data)
        {
            hash ^= b;
            hash *= FnvPrime;
        }

        // Additional final avalanche (xorshift mix)
        hash ^= hash >> 33;
        hash *= 0xff51afd7ed558ccdUL;
        hash ^= hash >> 33;
        hash *= 0xc4ceb9fe1a85ec53UL;
        hash ^= hash >> 33;
        // Re-introduce seed for diffusion
        hash ^= Seed;
        return hash;
    }

    public int ToBucket(ulong hash, int bucketCount)
    {
        if (bucketCount <= 0) throw new ArgumentOutOfRangeException(nameof(bucketCount));
        return (int)(hash % (ulong)bucketCount);
    }

    public ulong HashComposite(params string?[] parts)
    {
        if (parts == null || parts.Length == 0)
            return 0UL;

        // Separator byte
        const byte sep = 0x1F;
        ulong hash = FnvOffset ^ Seed;

        foreach (var part in parts)
        {
            if (!string.IsNullOrEmpty(part))
            {
                var byteCount = _utf8.GetByteCount(part);
                byte[]? rented = null;
                Span<byte> span = byteCount <= 1024
                    ? stackalloc byte[byteCount]
                    : (rented = ArrayPool<byte>.Shared.Rent(byteCount)).AsSpan(0, byteCount);

                try
                {
                    _utf8.GetBytes(part, span);
                    foreach (var b in span)
                    {
                        hash ^= b;
                        hash *= FnvPrime;
                    }
                }
                finally
                {
                    if (rented != null) ArrayPool<byte>.Shared.Return(rented);
                }
            }

            // Write separator to preserve positional context
            hash ^= sep;
            hash *= FnvPrime;
        }

        // Final avalanche
        hash ^= hash >> 32;
        hash *= 0xd6e8feb86659fd93UL;
        hash ^= hash >> 32;
        hash ^= Seed;
        return hash;
    }
}
