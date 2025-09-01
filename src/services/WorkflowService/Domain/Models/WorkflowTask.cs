using System.ComponentModel.DataAnnotations;
using DTOs.Entities;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Domain.Models;

public class WorkflowTask : BaseEntity
{
    [Required]
    public int TenantId { get; set; }

    [Required]
    public int WorkflowInstanceId { get; set; }

    [Required, MaxLength(100)]
    public string NodeId { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string TaskName { get; set; } = string.Empty;

    [Required]
    public WorkflowTaskStatus Status { get; set; } = WorkflowTaskStatus.Created;

    [Required, MaxLength(50)]
    public string NodeType { get; set; } = string.Empty; // removed default "human"

    public int? AssignedToUserId { get; set; }
    [MaxLength(100)] public string? AssignedToRole { get; set; }
    public DateTime? DueDate { get; set; }
    [Required] public string Data { get; set; } = "{}";
    public DateTime? ClaimedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletionData { get; set; }
    [MaxLength(1000)] public string? ErrorMessage { get; set; }

    public virtual WorkflowInstance WorkflowInstance { get; set; } = null!;
}
