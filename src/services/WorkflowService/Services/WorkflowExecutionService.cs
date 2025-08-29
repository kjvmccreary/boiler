using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorkflowService.Persistence;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using WorkflowService.Services.Interfaces;

// Avoid ambiguity with System.Threading.Tasks.TaskStatus
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Services
{
    public class WorkflowExecutionService : IWorkflowExecutionService
    {
        private readonly WorkflowDbContext _db;
        private readonly ILogger<WorkflowExecutionService> _logger;

        public WorkflowExecutionService(WorkflowDbContext db, ILogger<WorkflowExecutionService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task AdvanceAfterTaskCompletionAsync(WorkflowTask completedTask)
        {
            var instance = await _db.WorkflowInstances.FirstOrDefaultAsync(i => i.Id == completedTask.WorkflowInstanceId);
            if (instance == null)
            {
                _logger.LogWarning("Advance: Instance {Id} not found", completedTask.WorkflowInstanceId);
                return;
            }

            var definition = await _db.WorkflowDefinitions.FirstOrDefaultAsync(d =>
                d.Id == instance.WorkflowDefinitionId && d.Version == instance.DefinitionVersion);

            if (definition == null)
            {
                _logger.LogError("Advance: Definition {Def}/{Ver} not found", instance.WorkflowDefinitionId, instance.DefinitionVersion);
                return;
            }

            WorkflowBuilderDsl dsl;
            try
            {
                dsl = WorkflowBuilderDsl.Parse(definition.JSONDefinition);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse definition JSON for instance {InstanceId}", instance.Id);
                return;
            }

            var fromNode = completedTask.NodeId;
            if (string.IsNullOrWhiteSpace(fromNode))
            {
                _logger.LogWarning("Completed task {TaskId} missing NodeId", completedTask.Id);
                return;
            }

            var outgoing = dsl.Edges.Where(e => e.from == fromNode).ToList();
            if (!outgoing.Any())
            {
                await FinalizeIfNoActiveAsync(instance);
                return;
            }

            var newActive = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var edge in outgoing)
            {
                await TraverseAsync(dsl, instance, edge.to, newActive, 0);
            }

            instance.CurrentNodeIds = string.Join(",", newActive);

            if (newActive.Count == 0)
            {
                instance.Status = InstanceStatus.Completed;
                instance.CompletedAt = DateTime.UtcNow;
            }

            instance.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        private async Task TraverseAsync(
            WorkflowBuilderDsl dsl,
            WorkflowInstance instance,
            string nodeId,
            HashSet<string> active,
            int depth)
        {
            if (depth > 50)
            {
                _logger.LogWarning("Traverse depth exceeded for instance {InstanceId}", instance.Id);
                return;
            }

            var node = dsl.Nodes.FirstOrDefault(n => n.id == nodeId);
            if (node == null)
            {
                _logger.LogWarning("Node {NodeId} not in DSL for instance {InstanceId}", nodeId, instance.Id);
                return;
            }

            switch (node.type)
            {
                case "end":
                    return;
                case "humanTask":
                    CreateHumanTask(instance, node, active);
                    return;
                case "automatic":
                    foreach (var e in dsl.Edges.Where(e => e.from == node.id))
                        await TraverseAsync(dsl, instance, e.to, active, depth + 1);
                    return;
                case "gateway":
                    foreach (var e in dsl.Edges.Where(e => e.from == node.id))
                        await TraverseAsync(dsl, instance, e.to, active, depth + 1);
                    return;
                case "timer":
                    CreateTimerTask(instance, node, active);
                    return;
                case "start":
                    return;
                default:
                    _logger.LogInformation("Unhandled node type {Type} treated as terminal", node.type);
                    return;
            }
        }

        private void CreateHumanTask(WorkflowInstance instance, WorkflowNode node, HashSet<string> active)
        {
            var task = new WorkflowTask
            {
                WorkflowInstanceId = instance.Id,
                NodeId = node.id,
                TaskName = node.label ?? "Task",
                Status = WorkflowTaskStatus.Created,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (node.assigneeRoles?.Count == 1)
            {
                task.AssignedToRole = node.assigneeRoles.First();
                task.Status = WorkflowTaskStatus.Assigned;
            }

            if (node.dueInMinutes.HasValue)
                task.DueDate = DateTime.UtcNow.AddMinutes(node.dueInMinutes.Value);

            _db.WorkflowTasks.Add(task);
            active.Add(node.id);
        }

        private void CreateTimerTask(WorkflowInstance instance, WorkflowNode node, HashSet<string> active)
        {
            var task = new WorkflowTask
            {
                WorkflowInstanceId = instance.Id,
                NodeId = node.id,
                TaskName = node.label ?? "Timer",
                Status = WorkflowTaskStatus.Created,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DueDate = node.delayMinutes.HasValue ? DateTime.UtcNow.AddMinutes(node.delayMinutes.Value) : null
            };
            _db.WorkflowTasks.Add(task);
            active.Add(node.id);
        }

        private async Task FinalizeIfNoActiveAsync(WorkflowInstance instance)
        {
            var anyActive = await _db.WorkflowTasks
                .AnyAsync(t => t.WorkflowInstanceId == instance.Id &&
                               t.Status != WorkflowTaskStatus.Completed &&
                               t.Status != WorkflowTaskStatus.Cancelled &&
                               t.Status != WorkflowTaskStatus.Failed);
            if (!anyActive)
            {
                instance.Status = InstanceStatus.Completed;
                instance.CompletedAt = DateTime.UtcNow;
                instance.CurrentNodeIds = string.Empty;
                instance.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }
    }

    // Lightweight DSL structures
    public class WorkflowBuilderDsl
    {
        public List<WorkflowNode> Nodes { get; set; } = new();
        public List<WorkflowEdge> Edges { get; set; } = new();

        public static WorkflowBuilderDsl Parse(string json) =>
            System.Text.Json.JsonSerializer.Deserialize<WorkflowBuilderDsl>(json,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new WorkflowBuilderDsl();
    }

    public class WorkflowNode
    {
        public string id { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string? label { get; set; }
        public int? dueInMinutes { get; set; }
        public int? delayMinutes { get; set; }
        public List<string>? assigneeRoles { get; set; }
        public WorkflowNodeAction? action { get; set; }
    }

    public class WorkflowNodeAction
    {
        public string? kind { get; set; }
        public Dictionary<string, object>? config { get; set; }
    }

    public class WorkflowEdge
    {
        public string id { get; set; } = string.Empty;
        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;
        public string? label { get; set; }
    }
}
