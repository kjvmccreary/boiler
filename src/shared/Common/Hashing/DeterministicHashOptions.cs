namespace Common.Hashing;

public class DeterministicHashOptions
{
    /// Optional explicit seed (stable). If null, DefaultSeed is used.
    public ulong? Seed { get; set; }

    /// Optional algorithm override (reserved for future; only XxHash64 supported now).
    public string? Algorithm { get; set; }

    /// Default hard-coded seed (DO NOT CHANGE once in production).
    public const ulong DefaultSeed = 0xC0FFEE_BABE_F00DUL;
}
