using System.ComponentModel.DataAnnotations;
using DTOs.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowService.Domain.Models;

public class WorkflowDefinition : BaseEntity
{
    [Required]
    public int TenantId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public int Version { get; set; } = 1;
    
    [Required]
    public string JSONDefinition { get; set; } = string.Empty; // jsonb column
    
    public bool IsPublished { get; set; } = false;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    public DateTime? PublishedAt { get; set; }
    
    public int? PublishedByUserId { get; set; }

    // Add these properties:
    public string? Tags { get; set; }
    public string? PublishNotes { get; set; }
    public string? VersionNotes { get; set; }
    
    // ✅ TEMPORARY FIX: Remove this field from database operations
    [NotMapped]
    public int? ParentDefinitionId { get; set; }
    
    // Navigation properties
    public virtual ICollection<WorkflowInstance> Instances { get; set; } = new List<WorkflowInstance>();
}
