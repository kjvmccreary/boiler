namespace DTOs.Workflow.Enums;

/// <summary>
/// Status of a workflow task
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// Task is created and available for assignment
    /// </summary>
    Created = 1,
    
    /// <summary>
    /// Task has been assigned to a user
    /// </summary>
    Assigned = 2,
    
    /// <summary>
    /// Task has been claimed by a user
    /// </summary>
    Claimed = 3,
    
    /// <summary>
    /// Task is in progress
    /// </summary>
    InProgress = 4,
    
    /// <summary>
    /// Task completed successfully
    /// </summary>
    Completed = 5,
    
    /// <summary>
    /// Task was cancelled
    /// </summary>
    Cancelled = 6,
    
    /// <summary>
    /// Task failed with an error
    /// </summary>
    Failed = 7
}
