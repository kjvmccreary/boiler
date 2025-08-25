using System.ComponentModel.DataAnnotations;

namespace DTOs.Workflow;

public class WorkflowTaskDto
{
    public int Id { get; set; }
    public int WorkflowInstanceId { get; set; }
    public string NodeId { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public DTOs.Workflow.Enums.TaskStatus Status { get; set; } // ✅ FIXED: Fully qualified
    public int? AssignedToUserId { get; set; }
    public string? AssignedToRole { get; set; }
    public DateTime? DueDate { get; set; }
    public string Data { get; set; } = string.Empty;
    public DateTime? ClaimedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletionData { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class TaskSummaryDto
{
    public int Id { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public DTOs.Workflow.Enums.TaskStatus Status { get; set; } // ✅ FIXED: Fully qualified
    public string WorkflowDefinitionName { get; set; } = string.Empty;
    public int WorkflowInstanceId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CompleteTaskRequestDto
{
    public string? CompletionData { get; set; } = "{}";
    
    [StringLength(1000)]
    public string? CompletionNotes { get; set; }
}

public class ClaimTaskRequestDto
{
    [StringLength(500)]
    public string? ClaimNotes { get; set; }
}
