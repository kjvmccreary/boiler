using System.ComponentModel.DataAnnotations;
using DTOs.Workflow.Enums; // ✅ FIXED: Use shared enum namespace

namespace DTOs.Workflow;

public class WorkflowInstanceDto
{
    public int Id { get; set; }
    public int WorkflowDefinitionId { get; set; }
    public string WorkflowDefinitionName { get; set; } = string.Empty;
    public int DefinitionVersion { get; set; }
    public InstanceStatus Status { get; set; }
    public string CurrentNodeIds { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? StartedByUserId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class StartInstanceRequestDto
{
    [Required]
    public int WorkflowDefinitionId { get; set; }
    
    public string? InitialContext { get; set; } = "{}";
    
    public string? StartNotes { get; set; }
    
    // 🔧 ADD: Enhanced options for starting instances
    public int? WorkflowVersion { get; set; }
    
    [StringLength(255)]
    public string? InstanceName { get; set; }

    public Dictionary<string, object> Variables { get; set; } = new();
}

public class SignalInstanceRequestDto
{
    [Required]
    public string SignalName { get; set; } = string.Empty;
    
    public string? SignalData { get; set; } = "{}";
}

// 🔧 ADD: Missing DTOs needed by InstanceService
public class GetInstancesRequestDto
{
    public int? WorkflowDefinitionId { get; set; }
    public InstanceStatus? Status { get; set; }
    public DateTime? StartedAfter { get; set; }
    public DateTime? StartedBefore { get; set; }
    public int? StartedByUserId { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "StartedAt";
    public bool SortDescending { get; set; } = true;
}

public class TerminateInstanceRequestDto
{
    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;

    public bool ForceTerminate { get; set; } = false;
}

public class InstanceStatusDto
{
    public int InstanceId { get; set; }
    public InstanceStatus Status { get; set; }
    public string CurrentNodeIds { get; set; } = string.Empty;
    public List<string> CurrentNodeNames { get; set; } = new();
    public double ProgressPercentage { get; set; }
    public DateTime LastUpdated { get; set; }
    public TimeSpan Runtime { get; set; }
    public int ActiveTasksCount { get; set; }
    public string? ErrorMessage { get; set; }
}

// ✅ CONSOLIDATED: Complete WorkflowEventDto with audit fields
public class WorkflowEventDto
{
    public int Id { get; set; }
    public int WorkflowInstanceId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
