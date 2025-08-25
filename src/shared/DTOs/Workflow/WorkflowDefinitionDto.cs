using System.ComponentModel.DataAnnotations;

namespace DTOs.Workflow;

public class WorkflowDefinitionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Version { get; set; }
    public string JSONDefinition { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public string? Description { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int? PublishedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateWorkflowDefinitionDto
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string JSONDefinition { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? Description { get; set; }
}

public class UpdateWorkflowDefinitionDto
{
    [StringLength(255, MinimumLength = 1)]
    public string? Name { get; set; }
    
    public string? JSONDefinition { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }
}

public class PublishDefinitionRequestDto
{
    public string? PublishNotes { get; set; }
}
