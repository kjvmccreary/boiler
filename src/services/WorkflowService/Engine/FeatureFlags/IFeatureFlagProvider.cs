namespace WorkflowService.Engine.FeatureFlags;

/// <summary>
/// Abstraction for external feature flag evaluation (MVP scope).
/// Implementations must be deterministic for same inputs in a short window
/// and SHOULD cache upstream results where appropriate.
/// </summary>
public interface IFeatureFlagProvider
{
    /// <summary>
    /// Returns true if the flag is ON (enabled) else false.
    /// Must not throw for normal "flag not found" (return false instead).
    /// May throw for infrastructure/unexpected errors (caught by strategy).
    /// </summary>
    Task<bool> IsEnabledAsync(string flag, CancellationToken cancellationToken = default);
}

/// <summary>
/// Default no-op provider (always returns false).
/// </summary>
public sealed class NoopFeatureFlagProvider : IFeatureFlagProvider
{
    public Task<bool> IsEnabledAsync(string flag, CancellationToken cancellationToken = default)
        => Task.FromResult(false);
}
