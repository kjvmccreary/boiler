using DTOs.Workflow.GatewayDiagnostics;

namespace WorkflowService.Services.Interfaces;

public interface IWorkflowDiagnosticsService
{
    Task<GatewayDecisionHistoryDto?> GetGatewayDecisionHistoryAsync(int instanceId, CancellationToken ct = default);
    Task<GatewayNodeDecisionHistoryDto?> GetGatewayNodeDecisionHistoryAsync(int instanceId, string nodeId, CancellationToken ct = default);
}
