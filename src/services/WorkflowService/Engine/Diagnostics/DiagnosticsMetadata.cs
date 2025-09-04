namespace WorkflowService.Engine.Diagnostics;

/// Central place for diagnostics schema versioning & constants.
public static class DiagnosticsMetadata
{
    /// Increment when the shape of gateway decision diagnostics changes.
    /// v1: original (no diagnosticsVersion field)
    /// v2: adds diagnosticsVersion + (optional) strategyConfigHash
    public const int GatewayDecisionDiagnosticsVersion = 2;
}
