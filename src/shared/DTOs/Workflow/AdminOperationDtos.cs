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

// 🔧 ADD: Missing DTOs needed by AdminService
public class ForceCompleteRequestDto
{
    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    public string? FinalContext { get; set; } = "{}";

    [StringLength(100)]
    public string? CompletionNodeId { get; set; }
}

public class GetAnalyticsRequestDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? WorkflowDefinitionId { get; set; }
    public string? GroupBy { get; set; } = "day"; // day, week, month
}

public class WorkflowAnalyticsDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public int TotalInstances { get; set; }
    public int CompletedInstances { get; set; }
    public int FailedInstances { get; set; }
    public int RunningInstances { get; set; }
    
    public double AverageCompletionTime { get; set; }
    public double SuccessRate { get; set; }
    
    public Dictionary<string, int> InstancesByStatus { get; set; } = new();
    public Dictionary<string, int> InstancesByDefinition { get; set; } = new();
    public Dictionary<DateTime, int> InstancesByDate { get; set; } = new();
    
    public List<WorkflowPerformanceDto> TopBottlenecks { get; set; } = new();
}

public class WorkflowPerformanceDto
{
    public string NodeId { get; set; } = string.Empty;
    public string NodeName { get; set; } = string.Empty;
    public string NodeType { get; set; } = string.Empty;
    public double AverageTime { get; set; }
    public int InstanceCount { get; set; }
}

public class WorkflowSystemHealthDto
{
    public string Status { get; set; } = string.Empty; // Healthy, Degraded, Unhealthy
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    
    public int ActiveInstances { get; set; }
    public int PendingTasks { get; set; }
    public int BackgroundWorkerStatus { get; set; } // 0=Stopped, 1=Running, 2=Error
    
    public Dictionary<string, object> SystemMetrics { get; set; } = new();
    public List<string> Issues { get; set; } = new();
}

public class BulkInstanceOperationRequestDto
{
    [Required]
    public string Operation { get; set; } = string.Empty; // cancel, retry, terminate

    public List<int>? InstanceIds { get; set; }
    
    // OR bulk criteria:
    public int? WorkflowDefinitionId { get; set; }
    public WorkflowInstanceStatus? Status { get; set; }
    public DateTime? StartedBefore { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;
}

public class GetAuditTrailRequestDto
{
    public int? InstanceId { get; set; }
    public int? UserId { get; set; }
    public string? Action { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class WorkflowAuditEntryDto
{
    public int Id { get; set; }
    public int? InstanceId { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
