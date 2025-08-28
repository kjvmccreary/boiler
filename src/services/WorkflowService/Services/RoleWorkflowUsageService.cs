using DTOs.Workflow;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Dsl;
using WorkflowService.Persistence;
using Contracts.Services;

namespace WorkflowService.Services;

/// <summary>
/// Service for checking role usage in workflow definitions
/// </summary>
public interface IRoleWorkflowUsageService
{
    Task<RoleUsageInWorkflowsDto> CheckRoleUsageInWorkflowsAsync(string roleName, int tenantId, CancellationToken cancellationToken = default);
}

public class RoleWorkflowUsageService : IRoleWorkflowUsageService
{
    private readonly WorkflowDbContext _context;
    private readonly ILogger<RoleWorkflowUsageService> _logger;

    public RoleWorkflowUsageService(
        WorkflowDbContext context,
        ILogger<RoleWorkflowUsageService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RoleUsageInWorkflowsDto> CheckRoleUsageInWorkflowsAsync(string roleName, int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking role '{RoleName}' usage in workflow definitions for tenant {TenantId}", roleName, tenantId);

            // Get all workflow definitions for current tenant
            var workflowDefinitions = await _context.WorkflowDefinitions
                .Where(d => d.TenantId == tenantId)
                .Select(d => new 
                { 
                    d.Id, 
                    d.Name, 
                    d.Version, 
                    d.IsPublished, 
                    d.JSONDefinition, 
                    d.UpdatedAt 
                })
                .ToListAsync(cancellationToken);

            var usageInfo = new RoleUsageInWorkflowsDto
            {
                RoleName = roleName,
                IsUsedInWorkflows = false,
                UsedInDefinitions = new List<WorkflowDefinitionUsageDto>(),
                TotalUsageCount = 0
            };

            foreach (var definition in workflowDefinitions)
            {
                try
                {
                    // Parse the JSON definition to check for role usage
                    var workflowDef = WorkflowDefinitionJson.FromJson(definition.JSONDefinition);
                    var definitionUsage = new WorkflowDefinitionUsageDto
                    {
                        DefinitionId = definition.Id,
                        DefinitionName = definition.Name,
                        Version = definition.Version,
                        IsPublished = definition.IsPublished,
                        UsedInNodes = new List<WorkflowNodeUsageDto>(),
                        UsageCount = 0,
                        LastModified = definition.UpdatedAt
                    };

                    // Check each node for role usage
                    foreach (var node in workflowDef.Nodes)
                    {
                        if (node.IsHumanTask())
                        {
                            // Check if this human task node uses the role
                            var assignedRole = node.GetAssignedToRole();
                            var assigneeRoles = node.GetProperty<string[]>("assigneeRoles") ?? new string[0];

                            bool usesRole = false;
                            
                            // Check single assigned role
                            if (!string.IsNullOrEmpty(assignedRole) && 
                                assignedRole.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                            {
                                usesRole = true;
                            }
                            
                            // Check assignee roles array
                            if (assigneeRoles.Any(r => r.Equals(roleName, StringComparison.OrdinalIgnoreCase)))
                            {
                                usesRole = true;
                            }

                            if (usesRole)
                            {
                                definitionUsage.UsedInNodes.Add(new WorkflowNodeUsageDto
                                {
                                    NodeId = node.Id,
                                    NodeName = node.Name ?? node.Id,
                                    NodeType = node.Type
                                });
                                definitionUsage.UsageCount++;
                            }
                        }
                    }

                    // If this definition uses the role, add it to results
                    if (definitionUsage.UsageCount > 0)
                    {
                        usageInfo.UsedInDefinitions.Add(definitionUsage);
                        usageInfo.TotalUsageCount += definitionUsage.UsageCount;
                        usageInfo.IsUsedInWorkflows = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse workflow definition {DefinitionId} while checking role usage", definition.Id);
                    // Continue with other definitions
                }
            }

            // Generate helpful message
            if (usageInfo.IsUsedInWorkflows)
            {
                var publishedCount = usageInfo.UsedInDefinitions.Count(d => d.IsPublished);
                var draftCount = usageInfo.UsedInDefinitions.Count(d => !d.IsPublished);
                
                usageInfo.Message = $"Role '{roleName}' is used in {usageInfo.UsedInDefinitions.Count} workflow definition(s): " +
                                   $"{publishedCount} published, {draftCount} draft(s). " +
                                   $"Total usage: {usageInfo.TotalUsageCount} node(s).";
            }
            else
            {
                usageInfo.Message = $"Role '{roleName}' is not used in any workflow definitions.";
            }

            _logger.LogInformation("Role '{RoleName}' usage check complete: {IsUsed} (checked {DefCount} definitions)", 
                roleName, usageInfo.IsUsedInWorkflows, workflowDefinitions.Count);

            return usageInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role '{RoleName}' usage in workflows", roleName);
            return new RoleUsageInWorkflowsDto
            {
                RoleName = roleName,
                IsUsedInWorkflows = false,
                Message = "Error checking workflow usage"
            };
        }
    }
}
