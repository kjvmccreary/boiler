using System.ComponentModel.DataAnnotations;
using DTOs.Workflow.Enums;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;
using WorkflowInstanceStatus = DTOs.Workflow.Enums.InstanceStatus; // ✅ ADD: Alias for InstanceStatus

namespace DTOs.Workflow;

public class WorkflowStatsDto
{
    public int TotalDefinitions { get; set; }
    public int PublishedDefinitions { get; set; }
    
    public int TotalInstances { get; set; }
    public int RunningInstances { get; set; }
    public int CompletedInstances { get; set; }
    public int FailedInstances { get; set; }
    public int SuspendedInstances { get; set; }
    
    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int ActiveTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    
    public int TotalEvents { get; set; }
    public int EventsLast24Hours { get; set; }
}

public class RetryInstanceRequestDto
{
    [StringLength(500)]
    public string? RetryReason { get; set; }
    
    [StringLength(100)]
    public string? ResetToNodeId { get; set; }
}

public class MoveToNodeRequestDto
{
    [Required]
    [StringLength(100)]
    public string TargetNodeId { get; set; } = string.Empty;
    
    public string? ContextUpdates { get; set; }
    
    [StringLength(500)]
    public string? Reason { get; set; }
}

public class ResetTaskRequestDto
{
    [Required]
    public WorkflowTaskStatus NewStatus { get; set; }
    
    public int? AssignToUserId { get; set; }
    
    [StringLength(1000)]
    public string? ErrorMessage { get; set; }
    
    [StringLength(500)]
    public string? Reason { get; set; }
}

public class BulkCancelInstancesRequestDto
{
    public int? WorkflowDefinitionId { get; set; }
    
    public WorkflowInstanceStatus? Status { get; set; } // ✅ FIXED: Use the alias
    
    public DateTime? StartedBefore { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;
}

public class BulkOperationResultDto
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public int TotalCount { get; set; }
    public string OperationType { get; set; } = string.Empty;
}
