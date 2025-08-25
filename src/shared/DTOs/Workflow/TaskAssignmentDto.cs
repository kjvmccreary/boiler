using System.ComponentModel.DataAnnotations;

namespace DTOs.Workflow;

public class AssignTaskRequestDto
{
    public int? AssignedToUserId { get; set; }
    
    [StringLength(100)]
    public string? AssignedToRole { get; set; }
    
    [StringLength(500)]
    public string? AssignmentNotes { get; set; }
}
