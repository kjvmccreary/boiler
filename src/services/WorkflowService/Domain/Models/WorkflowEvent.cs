using System.ComponentModel.DataAnnotations;
using DTOs.Entities;

namespace WorkflowService.Domain.Models;

public class WorkflowEvent : BaseEntity
{
    [Required]
    public int WorkflowInstanceId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Type { get; set; } = string.Empty; // e.g., "NodeEntered", "TaskCompleted", "InstanceStarted"
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Data { get; set; } = "{}"; // JSON event data
    
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    
    public int? UserId { get; set; }
    
    // Navigation properties
    public virtual WorkflowInstance WorkflowInstance { get; set; } = null!;
}
