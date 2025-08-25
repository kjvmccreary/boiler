using System.ComponentModel.DataAnnotations;
using DTOs.Entities;
using WorkflowTaskStatus = WorkflowService.Domain.Enums.TaskStatus; // Use alias to avoid conflict

namespace WorkflowService.Domain.Models;

public class WorkflowTask : BaseEntity
{
    [Required]
    public int WorkflowInstanceId { get; set; }
    
    // ✅ ADD: Direct TenantId for proper isolation
    [Required]
    public int TenantId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string NodeId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string TaskName { get; set; } = string.Empty;
    
    [Required]
    public WorkflowTaskStatus Status { get; set; } = WorkflowTaskStatus.Pending; // Use alias
    
    public int? AssignedToUserId { get; set; }
    
    [MaxLength(100)]
    public string? AssignedToRole { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    [Required]
    public string Data { get; set; } = "{}"; // JSON task data
    
    public DateTime? ClaimedAt { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    [MaxLength(1000)]
    public string? CompletionData { get; set; } // JSON completion result
    
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
    
    // Navigation properties
    public virtual WorkflowInstance WorkflowInstance { get; set; } = null!;
}
