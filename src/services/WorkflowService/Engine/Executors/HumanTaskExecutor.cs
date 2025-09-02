using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using Contracts.Services;
using WorkflowService.Services.Interfaces;
using WorkflowService.Persistence;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Executor for human task nodes (idempotent: will not create a second open task
/// for the same instance+node if one is already pending).
/// </summary>
public class HumanTaskExecutor : INodeExecutor
{
    private readonly ILogger<HumanTaskExecutor> _logger;
    private readonly IRoleService _roleService;
    private readonly ITaskNotificationDispatcher _notificationDispatcher;
    private readonly WorkflowDbContext _db;

    public string NodeType => NodeTypes.HumanTask;

    public HumanTaskExecutor(
        ILogger<HumanTaskExecutor> logger,
        IRoleService roleService,
        ITaskNotificationDispatcher notificationDispatcher,
        WorkflowDbContext db)
    {
        _logger = logger;
        _roleService = roleService;
        _notificationDispatcher = notificationDispatcher;
        _db = db;
    }

    public bool CanExecute(WorkflowNode node) => node.IsHumanTask();

    public async Task<NodeExecutionResult> ExecuteAsync(
        WorkflowNode node,
        WorkflowInstance instance,
        string context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var assignedRole   = GetAssignedRole(node);
            var assignedUserId = GetAssignedUserId(node);
            var taskName       = GetTaskName(node);
            var dueInMinutes   = GetDueInMinutes(node);

            _logger.LogInformation(
                "Creating human task: TaskName='{TaskName}', AssignedRole='{Role}', AssignedUserId='{UserId}', DueInMinutes={DueInMinutes}",
                taskName, assignedRole, assignedUserId, dueInMinutes);

            // Validate role (best effort)
            if (!string.IsNullOrWhiteSpace(assignedRole))
            {
                try
                {
                    var roleExists = !await _roleService.IsRoleNameAvailableAsync(assignedRole, null, cancellationToken);
                    if (!roleExists)
                        _logger.LogWarning("Assigned role '{Role}' does not exist in tenant {TenantId} (node {NodeId})",
                            assignedRole, instance.TenantId, node.Id);
                    else
                        _logger.LogInformation("Validated role '{Role}' exists in tenant {TenantId}",
                            assignedRole, instance.TenantId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Role validation failed for '{Role}' (node {NodeId})", assignedRole, node.Id);
                }
            }

            // Idempotency guard: do NOT create a second open task for this node.
            var existing = await _db.WorkflowTasks
                .Where(t =>
                    t.WorkflowInstanceId == instance.Id &&
                    t.NodeId == node.Id &&
                    (t.Status == WorkflowTaskStatus.Created
                     || t.Status == WorkflowTaskStatus.Assigned
                     || t.Status == WorkflowTaskStatus.Claimed
                     || t.Status == WorkflowTaskStatus.InProgress))
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existing != null)
            {
                _logger.LogDebug("WF_HUMANTASK_DUP_SKIP Instance={InstanceId} Node={NodeId} ExistingTaskId={TaskId}",
                    instance.Id, node.Id, existing.Id);

                return new NodeExecutionResult
                {
                    IsSuccess   = true,
                    ShouldWait  = true,
                    CreatedTask = null,
                    NextNodeIds = new List<string>() // remain waiting
                };
            }

            var task = new WorkflowTask
            {
                TenantId             = instance.TenantId,
                WorkflowInstanceId   = instance.Id,
                NodeId               = node.Id,
                TaskName             = taskName,
                Status               = WorkflowTaskStatus.Created,
                NodeType             = "human",
                Data = JsonSerializer.Serialize(new
                {
                    FormSchema  = GetFormSchema(node),
                    Instructions= GetInstructions(node),
                    Priority    = GetPriority(node),
                    NodeLabel   = node.Name,
                    NodeId      = node.Id
                }),
                DueDate    = dueInMinutes.HasValue ? DateTime.UtcNow.AddMinutes(dueInMinutes.Value) : null,
                CreatedAt  = DateTime.UtcNow,
                UpdatedAt  = DateTime.UtcNow
            };

            // Assignment precedence: explicit user > role
            if (!string.IsNullOrEmpty(assignedUserId) && int.TryParse(assignedUserId, out var userId))
            {
                task.AssignedToUserId = userId;
                task.Status           = WorkflowTaskStatus.Assigned;
                _logger.LogInformation("Task assigned to user {UserId}", userId);
            }
            else if (!string.IsNullOrWhiteSpace(assignedRole))
            {
                task.AssignedToRole = assignedRole;
                task.Status         = WorkflowTaskStatus.Assigned;
                _logger.LogInformation("Task assigned to role '{Role}'", assignedRole);
            }
            else
            {
                _logger.LogInformation("Task created unassigned (claimable).");
            }

            _logger.LogInformation(
                "Successfully created human task '{TaskName}' for workflow instance {InstanceId} (Tenant {TenantId})",
                taskName, instance.Id, instance.TenantId);

            // Notify (TaskBell / summaries)
            await _notificationDispatcher.NotifyTenantAsync(instance.TenantId, cancellationToken);

            return new NodeExecutionResult
            {
                IsSuccess   = true,
                ShouldWait  = true,          // human tasks always pause progression
                CreatedTask = task,
                NextNodeIds = new List<string>() // advanced after completion
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error executing human task node {NodeId} for instance {InstanceId}",
                node.Id, instance.Id);

            return new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    // ---------- Property Extraction Helpers ----------

    private string GetAssignedRole(WorkflowNode node)
    {
        if (!node.Properties.TryGetValue("assigneeRoles", out var value) || value == null)
            return FallbackRole(node);

        try
        {
            if (value is string[] arr && arr.Length > 0)
                return SafeFirst(arr);

            if (value is IEnumerable<object> objEnum)
            {
                var first = objEnum.OfType<string>().FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(first)) return first.Trim();
            }

            if (value is JsonElement je)
            {
                if (je.ValueKind == JsonValueKind.Array && je.GetArrayLength() > 0)
                {
                    var first = je[0].GetString();
                    if (!string.IsNullOrWhiteSpace(first)) return first.Trim();
                }
                if (je.ValueKind == JsonValueKind.String)
                {
                    var s = je.GetString();
                    if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                }
            }

            if (value is string single && !string.IsNullOrWhiteSpace(single))
                return single.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to parse assigneeRoles for node {NodeId}", node.Id);
        }

        return FallbackRole(node);

        static string SafeFirst(string[] a) =>
            a.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?.Trim() ?? string.Empty;
    }

    private string FallbackRole(WorkflowNode node) =>
        node.GetProperty<string>("assignedToRole")?.Trim()
        ?? node.GetProperty<string>("assigneeRole")?.Trim()
        ?? string.Empty;

    private string GetAssignedUserId(WorkflowNode node) =>
        node.GetProperty<string>("assignedToUserId")?.Trim()
        ?? node.GetProperty<string>("assigneeUserId")?.Trim()
        ?? string.Empty;

    private string GetTaskName(WorkflowNode node)
    {
        if (node.Properties.TryGetValue("label", out var labelValue) && labelValue != null)
        {
            if (labelValue is string s && !string.IsNullOrWhiteSpace(s)) return s;
            if (labelValue is JsonElement je && je.ValueKind == JsonValueKind.String)
                return je.GetString() ?? "Human Task";
        }

        return node.GetProperty<string>("taskName")
            ?? node.Name
            ?? "Human Task";
    }

    private int? GetDueInMinutes(WorkflowNode node)
    {
        if (!node.Properties.TryGetValue("dueInMinutes", out var raw) || raw == null)
            return node.GetProperty<int?>("timeoutMinutes")
                ?? node.GetProperty<int?>("dueDateMinutes");

        try
        {
            return raw switch
            {
                int i => i,
                long l => (int)l,
                string s when int.TryParse(s, out var parsed) => parsed,
                JsonElement je when je.ValueKind == JsonValueKind.Number && je.TryGetInt32(out var v) => v,
                _ => node.GetProperty<int?>("timeoutMinutes") ?? node.GetProperty<int?>("dueDateMinutes")
            };
        }
        catch
        {
            return node.GetProperty<int?>("timeoutMinutes")
                ?? node.GetProperty<int?>("dueDateMinutes");
        }
    }

    private string GetFormSchema(WorkflowNode node) =>
        node.GetProperty<string>("formSchema") ?? "{}";

    private string GetInstructions(WorkflowNode node) =>
        node.GetProperty<string>("instructions")
        ?? node.GetProperty<string>("description")
        ?? string.Empty;

    private string GetPriority(WorkflowNode node) =>
        node.GetProperty<string>("priority") ?? "normal";
}
