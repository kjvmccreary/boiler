namespace DTOs.Workflow;

/// <summary>
/// Request to check role usage in workflow definitions before renaming/deleting
/// </summary>
public class CheckRoleUsageRequestDto
{
    public string RoleName { get; set; } = string.Empty;
}

/// <summary>
/// Response showing how a role is used in workflow definitions
/// </summary>
public class RoleUsageInWorkflowsDto
{
    public bool IsUsedInWorkflows { get; set; }
    public List<WorkflowDefinitionUsageDto> UsedInDefinitions { get; set; } = new();
    public int TotalUsageCount { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Information about how a role is used in a specific workflow definition
/// </summary>
public class WorkflowDefinitionUsageDto
{
    public int DefinitionId { get; set; }
    public string DefinitionName { get; set; } = string.Empty;
    public int Version { get; set; }
    public bool IsPublished { get; set; }
    public List<WorkflowNodeUsageDto> UsedInNodes { get; set; } = new();
    public int UsageCount { get; set; }
    public DateTime LastModified { get; set; }
}

/// <summary>
/// Information about how a role is used in a specific node
/// </summary>
public class WorkflowNodeUsageDto
{
    public string NodeId { get; set; } = string.Empty;
    public string NodeName { get; set; } = string.Empty;
    public string NodeType { get; set; } = string.Empty;
}

/// <summary>
/// Request to update role name with workflow impact validation
/// </summary>
public class UpdateRoleWithValidationRequestDto
{
    public string NewName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Permissions { get; set; } = new();
    public bool ForceUpdate { get; set; } = false;
    public bool UpdateWorkflowDefinitions { get; set; } = false;
}
