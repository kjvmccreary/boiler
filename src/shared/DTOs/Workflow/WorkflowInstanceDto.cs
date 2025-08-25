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
}

public class SignalInstanceRequestDto
{
    [Required]
    public string SignalName { get; set; } = string.Empty;
    
    public string? SignalData { get; set; } = "{}";
}
