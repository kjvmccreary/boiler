using System.ComponentModel.DataAnnotations;
using DTOs.Entities;
using DTOs.Workflow.Enums; // ✅ FIXED: Use shared enums

namespace WorkflowService.Domain.Models;

public class WorkflowInstance : BaseEntity
{
    [Required]
    public int TenantId { get; set; }
    
    [Required]
    public int WorkflowDefinitionId { get; set; }
    
    [Required]
    public int DefinitionVersion { get; set; }
    
    [Required]
    public InstanceStatus Status { get; set; } = InstanceStatus.Running;
    
    [Required]
    public string CurrentNodeIds { get; set; } = string.Empty; // JSON array of current node IDs
    
    [Required]
    public string Context { get; set; } = "{}"; // JSON context data
    
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletedAt { get; set; }
    
    public int? StartedByUserId { get; set; }
    
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
    
    // Navigation properties
    public virtual WorkflowDefinition WorkflowDefinition { get; set; } = null!;
    public virtual ICollection<WorkflowTask> Tasks { get; set; } = new List<WorkflowTask>();
    public virtual ICollection<WorkflowEvent> Events { get; set; } = new List<WorkflowEvent>();
}
