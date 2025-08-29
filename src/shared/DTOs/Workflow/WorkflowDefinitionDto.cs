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
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
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
    
    // 🔧 ADD: Tags support
    public string? Tags { get; set; }
}

public class UpdateWorkflowDefinitionDto
{
    [StringLength(255, MinimumLength = 1)]
    public string? Name { get; set; }
    
    public string? JSONDefinition { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    // 🔧 ADD: Tags support
    public string? Tags { get; set; }
}

public class PublishDefinitionRequestDto
{
    public string? PublishNotes { get; set; }
    
    // 🔧 ADD: Force publish option
    public bool ForcePublish { get; set; } = false;
}

// 🔧 ADD: Missing DTOs needed by DefinitionService
public class GetWorkflowDefinitionsRequestDto
{
    public string? SearchTerm { get; set; }
    public bool? IsPublished { get; set; }
    public string? Tags { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}

public class ValidateDefinitionRequestDto
{
    [Required]
    public string JSONDefinition { get; set; } = string.Empty;
}

public class ValidationResultDto
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class CreateNewVersionRequestDto
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public string JSONDefinition { get; set; } = string.Empty;

    [StringLength(500)]
    public string? VersionNotes { get; set; }

    public string? Tags { get; set; }
}
