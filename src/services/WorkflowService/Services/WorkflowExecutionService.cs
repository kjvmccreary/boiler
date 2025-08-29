using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorkflowService.Persistence;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using WorkflowService.Services.Interfaces;
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
            _logger.LogInformation("ADVANCE_START TaskId={TaskId} NodeId={NodeId} InstanceId={InstanceId}",
                completedTask.Id, completedTask.NodeId, completedTask.WorkflowInstanceId);

            if (string.IsNullOrWhiteSpace(completedTask.NodeId))
            {
                _logger.LogWarning("ADVANCE_ABORT TaskId={TaskId} empty NodeId", completedTask.Id);
                return;
            }

            var instance = await _db.WorkflowInstances.FirstOrDefaultAsync(i => i.Id == completedTask.WorkflowInstanceId);
            if (instance == null) { _logger.LogWarning("ADVANCE_ABORT Instance {Id} missing", completedTask.WorkflowInstanceId); return; }

            var definition = await _db.WorkflowDefinitions.FirstOrDefaultAsync(d =>
                d.Id == instance.WorkflowDefinitionId && d.Version == instance.DefinitionVersion);
            if (definition == null) { _logger.LogError("ADVANCE_ABORT Definition missing for Instance {Id}", instance.Id); return; }

            WorkflowBuilderDsl dsl;
            try { dsl = WorkflowBuilderDsl.Parse(definition.JSONDefinition); }
            catch (Exception ex) { _logger.LogError(ex, "Parse failure"); return; }

            var fromNode = completedTask.NodeId;
            var outgoing = dsl.Edges.Where(e => e.from == fromNode).ToList();
            _logger.LogInformation("ADVANCE_EDGES From={From} Count={Count}", fromNode, outgoing.Count);

            var currentSet = ParseCurrentNodeIds(instance.CurrentNodeIds);
            currentSet.Remove(fromNode);

            // RECORD: edge traversal / start of advancement
            if (outgoing.Count == 0)
            {
                await RecordEvent(instance.Id, "Edge", "NoOutgoing",
                    new { from = fromNode });
            }

            var newlyActive = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var edge in outgoing)
            {
                await RecordEvent(instance.Id, "Edge", "EdgeTraversed",
                    new { from = edge.from, to = edge.to, edgeId = edge.id });
                await TraverseAsync(dsl, instance, edge.to, newlyActive, 0);
            }

            foreach (var n in newlyActive) currentSet.Add(n);

            if (currentSet.Count == 0)
            {
                var unfinished = await _db.WorkflowTasks.AnyAsync(t =>
                    t.WorkflowInstanceId == instance.Id &&
                    t.Status != WorkflowTaskStatus.Completed &&
                    t.Status != WorkflowTaskStatus.Cancelled &&
                    t.Status != WorkflowTaskStatus.Failed);

                if (!unfinished)
                {
                    instance.Status = InstanceStatus.Completed;
                    instance.CompletedAt = DateTime.UtcNow;
                    await RecordEvent(instance.Id, "Instance", "InstanceCompleted",
                        new { instanceId = instance.Id, completedAt = instance.CompletedAt });
                }
            }

            instance.CurrentNodeIds = JsonSerializer.Serialize(currentSet.OrderBy(s => s));
            instance.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _logger.LogInformation("ADVANCE_DONE InstanceId={InstanceId} Current={Current}",
                instance.Id, instance.CurrentNodeIds);
        }

        private async Task TraverseAsync(
            WorkflowBuilderDsl dsl,
            WorkflowInstance instance,
            string nodeId,
            HashSet<string> active,
            int depth)
        {
            if (depth > 50) { _logger.LogWarning("DEPTH_EXCEEDED"); return; }

            var node = dsl.Nodes.FirstOrDefault(n => n.id == nodeId);
            if (node == null) { _logger.LogWarning("NODE_MISSING {Node}", nodeId); return; }

            _logger.LogInformation("TRAVERSE Node={Node} Type={Type}", node.id, node.type);

            switch (node.type)
            {
                case "end":
                    await RecordEvent(instance.Id, "Node", "EndReached", new { nodeId = node.id, label = node.label });
                    return;

                case "humanTask":
                    CreateHumanTask(instance, node, active);
                    await RecordEvent(instance.Id, "Node", "NodeActivated",
                        new { nodeId = node.id, type = node.type, label = node.label, assigneeRoles = node.assigneeRoles });
                    return;

                case "automatic":
                    await RecordEvent(instance.Id, "Node", "NodeAutoExecuted",
                        new { nodeId = node.id, type = node.type, label = node.label, action = node.action });
                    foreach (var e in dsl.Edges.Where(e => e.from == node.id))
                    {
                        await RecordEvent(instance.Id, "Edge", "EdgeTraversed",
                            new { from = e.from, to = e.to, edgeId = e.id });
                        await TraverseAsync(dsl, instance, e.to, active, depth + 1);
                    }
                    return;

                case "gateway":
                    await RecordEvent(instance.Id, "Node", "GatewayEvaluated",
                        new { nodeId = node.id, type = node.type, label = node.label });
                    foreach (var e in dsl.Edges.Where(e => e.from == node.id))
                    {
                        await RecordEvent(instance.Id, "Edge", "EdgeTraversed",
                            new { from = e.from, to = e.to, edgeId = e.id });
                        await TraverseAsync(dsl, instance, e.to, active, depth + 1);
                    }
                    return;

                case "timer":
                    CreateTimerTask(instance, node, active);
                    await RecordEvent(instance.Id, "Node", "NodeActivated",
                        new { nodeId = node.id, type = node.type, label = node.label, delayMinutes = node.delayMinutes });
                    return;

                case "start":
                    // Usually not re-entered; ignore
                    return;

                default:
                    await RecordEvent(instance.Id, "Node", "UnhandledNodeType",
                        new { nodeId = node.id, type = node.type });
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

        private async Task RecordEvent(int instanceId, string type, string name, object data)
        {
            try
            {
                var ev = new WorkflowEvent
                {
                    WorkflowInstanceId = instanceId,
                    Type = type,
                    Name = name,
                    Data = JsonSerializer.Serialize(data),
                    OccurredAt = DateTime.UtcNow
                };
                _db.WorkflowEvents.Add(ev);
                // Intentionally defer SaveChanges; caller batches
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record workflow event {Type}:{Name}", type, name);
            }
        }

        private static HashSet<string> ParseCurrentNodeIds(string raw)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(raw)) return set;
            try
            {
                if (raw.TrimStart().StartsWith("["))
                {
                    var arr = JsonSerializer.Deserialize<List<string>>(raw);
                    if (arr != null) foreach (var s in arr) if (!string.IsNullOrWhiteSpace(s)) set.Add(s);
                    return set;
                }
            }
            catch { /* ignore */ }
            foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                set.Add(part);
            return set;
        }
    }

    // DSL structures unchanged...
    public class WorkflowBuilderDsl
    {
        public List<WorkflowNode> Nodes { get; set; } = new();
        public List<WorkflowEdge> Edges { get; set; } = new();
        public static WorkflowBuilderDsl Parse(string json) =>
            JsonSerializer.Deserialize<WorkflowBuilderDsl>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
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
