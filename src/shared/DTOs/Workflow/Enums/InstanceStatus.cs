namespace DTOs.Workflow.Enums;

/// <summary>
/// Status of a workflow instance
/// </summary>
public enum InstanceStatus
{
    /// <summary>
    /// Instance is currently running
    /// </summary>
    Running = 1,
    
    /// <summary>
    /// Instance completed successfully
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Instance was cancelled
    /// </summary>
    Cancelled = 3,
    
    /// <summary>
    /// Instance failed with an error
    /// </summary>
    Failed = 4,
    
    /// <summary>
    /// Instance is suspended/paused
    /// </summary>
    Suspended = 5
}
