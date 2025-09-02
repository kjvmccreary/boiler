using System.Text.Json;
using DTOs.Workflow;
using WorkflowService.Domain.Dsl;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Services;

public class GraphValidationService : IGraphValidationService
{
    private readonly ILogger<GraphValidationService> _logger;

    public GraphValidationService(ILogger<GraphValidationService> logger)
    {
        _logger = logger;
    }

    public ValidationResultDto Validate(string jsonDefinition, bool strict)
    {
        try
        {
            var wf = WorkflowDefinitionJson.FromJson(jsonDefinition);

            var errors = new List<string>();
            var warnings = new List<string>();

            // Node & edge ID sets
            var nodeIdSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var duplicateNodeIds = new List<string>();
            foreach (var n in wf.Nodes)
            {
                if (!nodeIdSet.Add(n.Id))
                    duplicateNodeIds.Add(n.Id);
            }
            if (duplicateNodeIds.Any())
                errors.Add($"Duplicate node IDs: {string.Join(",", duplicateNodeIds.Distinct())}");

            var edgeIdSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var duplicateEdgeIds = new List<string>();
            foreach (var e in wf.Edges)
            {
                if (!edgeIdSet.Add(e.Id))
                    duplicateEdgeIds.Add(e.Id);
            }
            if (duplicateEdgeIds.Any())
                errors.Add($"Duplicate edge IDs: {string.Join(",", duplicateEdgeIds.Distinct())}");

            // Start / End constraints
            var starts = wf.Nodes.Where(n => n.Type.Equals("start", StringComparison.OrdinalIgnoreCase)).ToList();
            var ends = wf.Nodes.Where(n => n.Type.Equals("end", StringComparison.OrdinalIgnoreCase)).ToList();

            if (starts.Count == 0) errors.Add("Exactly one Start node required (found 0).");
            else if (starts.Count > 1) errors.Add($"Exactly one Start node required (found {starts.Count}).");
            if (ends.Count == 0) errors.Add("At least one End node required.");

            // Reachability (if a single start exists)
            HashSet<string> reachable = new(StringComparer.OrdinalIgnoreCase);
            if (starts.Count == 1)
                reachable = GetReachable(wf, starts[0].Id);

            // Unreachable nodes
            var unreachableNodes = wf.Nodes
                .Where(n => !n.Type.Equals("start", StringComparison.OrdinalIgnoreCase) && !reachable.Contains(n.Id))
                .Select(n => n.Id)
                .ToList();

            if (unreachableNodes.Any())
                errors.Add($"Unreachable nodes: {string.Join(",", unreachableNodes)}");

            // Edges referencing unknown nodes
            foreach (var e in wf.Edges)
            {
                if (!wf.Nodes.Any(n => n.Id == e.EffectiveSource))
                    errors.Add($"Edge {e.Id} source '{e.EffectiveSource}' does not exist");
                if (!wf.Nodes.Any(n => n.Id == e.EffectiveTarget))
                    errors.Add($"Edge {e.Id} target '{e.EffectiveTarget}' does not exist");
            }

            // Unreachable end nodes (subset of unreachable nodes)
            var unreachableEnds = ends.Where(e => !reachable.Contains(e.Id)).Select(e => e.Id).ToList();
            if (unreachableEnds.Any())
                errors.Add($"End nodes unreachable from Start: {string.Join(",", unreachableEnds)}");

            // Optional: isolated islands (nodes neither reachable nor used as an edge endpoint)
            // Already covered by unreachable if start reachable analysis is strict.

            // Gateway quality warnings (existing domain warnings reused)
            var domainValidation = wf.Validate();
            warnings.AddRange(domainValidation.Warnings.Where(w => !warnings.Contains(w)));

            return new ValidationResultDto
            {
                IsValid = errors.Count == 0,
                Errors = errors,
                Warnings = warnings,
                Metadata = new Dictionary<string, object>
                {
                    ["nodeCount"] = wf.Nodes.Count,
                    ["edgeCount"] = wf.Edges.Count,
                    ["startCount"] = starts.Count,
                    ["endCount"] = ends.Count,
                    ["unreachableNodeIds"] = unreachableNodes,
                    ["duplicateNodeIds"] = duplicateNodeIds,
                    ["duplicateEdgeIds"] = duplicateEdgeIds,
                    ["nodeTypes"] = wf.Nodes.GroupBy(n => n.Type).ToDictionary(g => g.Key, g => g.Count())
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Graph validation failed");
            return new ValidationResultDto
            {
                IsValid = false,
                Errors = new List<string> { $"Invalid JSON: {ex.Message}" }
            };
        }
    }

    private static HashSet<string> GetReachable(WorkflowDefinitionJson wf, string startId)
    {
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var queue = new Queue<string>();
        queue.Enqueue(startId);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!visited.Add(current)) continue;
            var next = wf.Edges
                .Where(e => e.EffectiveSource == current)
                .Select(e => e.EffectiveTarget);
            foreach (var n in next)
                if (!visited.Contains(n))
                    queue.Enqueue(n);
        }
        return visited;
    }
}
