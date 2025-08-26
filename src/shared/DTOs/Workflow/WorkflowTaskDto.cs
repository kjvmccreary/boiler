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

// 🔧 ADD: Missing DTOs needed by TaskService
public class GetTasksRequestDto
{
    public DTOs.Workflow.Enums.TaskStatus? Status { get; set; }
    public int? WorkflowDefinitionId { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? AssignedToRole { get; set; }
    public DateTime? DueBefore { get; set; }
    public DateTime? DueAfter { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}

public class ReassignTaskRequestDto
{
    public int? AssignToUserId { get; set; }

    [StringLength(100)]
    public string? AssignToRole { get; set; }

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;
}

public class TaskStatisticsDto
{
    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public Dictionary<string, int> TasksByType { get; set; } = new();
    public Dictionary<DTOs.Workflow.Enums.TaskStatus, int> TasksByStatus { get; set; } = new();
    public double AverageCompletionTime { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

