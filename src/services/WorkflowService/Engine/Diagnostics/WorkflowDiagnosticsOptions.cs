namespace WorkflowService.Engine.Diagnostics;

/// <summary>
/// Options controlling in-memory workflow diagnostics.
/// Placed under config section: Workflow:Diagnostics
/// </summary>
public class WorkflowDiagnosticsOptions
{
    /// <summary>
    /// Enable verbose automatic node trace buffering & logging augmentation.
    /// </summary>
    public bool EnableAutomaticTrace { get; set; } = false;

    /// <summary>
    /// Ring buffer size for recent automatic diagnostics entries.
    /// </summary>
    public int AutomaticBufferSize { get; set; } = 100;
}
