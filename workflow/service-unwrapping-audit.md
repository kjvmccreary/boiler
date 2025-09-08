# Workflow Frontend Service – API Unwrapping & Contract Audit (Phase 5)

Status: COMPLETE (Items 2 & 3 from action list closed).  
Purpose: Enforce uniform ApiResponse<T> unwrapping, highlight edge cases, and provide a baseline for future test assertions.

## 1. Envelope Contract (Canonical)
All workflow HTTP endpoints (except SignalR hub pushes) return:
`interface ApiResponse<T> { success: boolean; message?: string; errors?: { code?: string; message?: string }[] | string[] | any; data?: T; }`  
Paged variant: `data: { items: TItem[]; totalCount: number; page: number; pageSize: number }`

Helper: `unwrap<T>(resp.data)` throws aggregated Error when `success === false`.

## 2. Definitions API Matrix
| Method | HTTP | Path | Body | Raw Shape | Unwrap Type | Paged? | Notes |
|--------|------|------|------|-----------|-------------|--------|-------|
| getDefinitionsPaged | GET | /api/workflow/definitions | Query | ApiResponse<PagedResultDto<WorkflowDefinitionDto>> | PagedResultDto<WorkflowDefinitionDto> | Yes | Grid source |
| getDefinitions | GET | /api/workflow/definitions | Query | Same | WorkflowDefinitionDto[] | Yes | Convenience (items only) |
| getDefinition | GET | /api/workflow/definitions/{id} | – | ApiResponse<WorkflowDefinitionDto> | WorkflowDefinitionDto | No | |
| createDraft | POST | /api/workflow/definitions/draft | CreateWorkflowDefinitionDto | ApiResponse<WorkflowDefinitionDto> | WorkflowDefinitionDto | No | |
| updateDefinition | PUT | /api/workflow/definitions/{id} | UpdateWorkflowDefinitionDto | ApiResponse<WorkflowDefinitionDto> | WorkflowDefinitionDto | No | |
| publishDefinition | POST | /api/workflow/definitions/{id}/publish | {publishNotes,forcePublish} | ApiResponse<WorkflowDefinitionDto> | WorkflowDefinitionDto | No | Custom error mapping |
| validateDefinitionJson | POST | /api/workflow/definitions/validate | { JSONDefinition } | ApiResponse<ValidationResultDto> | GraphValidationResult | No | Adaptive legacy tolerance |
| validateDefinitionById | GET | /api/workflow/definitions/{id}/validate | – | ApiResponse<ValidationResultDto> | GraphValidationResult | No | |
| validateDefinition | POST | /api/workflow/definitions/validate | { JSONDefinition } | ApiResponse<ValidationResultDto> | ValidationResultDto | No | Direct unwrap |
| createNewVersion | POST | /api/workflow/definitions/{id}/new-version | CreateNewVersionRequestDto | ApiResponse<WorkflowDefinitionDto> | WorkflowDefinitionDto | No | |
| revalidateDefinition | POST | /api/workflow/definitions/{id}/revalidate | {} | ApiResponse<ValidationResultDto> | ValidationResultDto | No | |
| deleteDefinition | DELETE | /api/workflow/definitions/{id} | – | ApiResponse<boolean> | boolean | No | |
| unpublishDefinition | POST | /api/workflow/definitions/{id}/unpublish | {} | ApiResponse<WorkflowDefinitionDto> | WorkflowDefinitionDto | No | |
| archiveDefinition | POST | /api/workflow/definitions/{id}/archive | {} | ApiResponse<WorkflowDefinitionDto> | WorkflowDefinitionDto | No | |
| terminateDefinitionInstances | POST | /api/workflow/definitions/{id}/terminate-running | {} | ApiResponse<{terminated:number}> | { terminated:number } | No | |

Definition metadata exposure: UI now surfaces PublishNotes, VersionNotes, Tags, ActiveInstanceCount, ParentDefinitionId in the detail panel (DefinitionsPage) – not in base columns (intentionally).

## 3. Instances API Matrix
| Method | HTTP | Path | Body | Raw Shape | Unwrap | Paged | Post |
|--------|------|------|------|-----------|--------|-------|------|
| getInstancesPaged | GET | /api/workflow/instances | Query | ApiResponse<PagedResultDto<WorkflowInstanceDto>> | PagedResultDto<WorkflowInstanceDto> | Yes | Status normalized |
| getInstances | GET | /api/workflow/instances | Query | Same | WorkflowInstanceDto[] | Yes | Items only |
| getInstance | GET | /api/workflow/instances/{id} | – | ApiResponse<WorkflowInstanceDto> | WorkflowInstanceDto | No | Status normalized |
| getInstanceStatus | GET | /api/workflow/instances/{id}/status | – | ApiResponse<InstanceStatusDto> | InstanceStatusDto | No | Status normalized |
| startInstance | POST | /api/workflow/instances | StartInstanceRequestDto | ApiResponse<WorkflowInstanceDto> | WorkflowInstanceDto | No | Caller uses id |
| signalInstance | POST | /api/workflow/instances/{id}/signal | SignalInstanceRequestDto | ApiResponse<WorkflowInstanceDto> | WorkflowInstanceDto | No | |
| terminateInstance | DELETE | /api/workflow/instances/{id} | – | ApiResponse<boolean> | boolean | No | |
| suspendInstance | POST | /api/workflow/instances/{id}/suspend | {} | ApiResponse<WorkflowInstanceDto> | WorkflowInstanceDto | No | |
| resumeInstance | POST | /api/workflow/instances/{id}/resume | {} | ApiResponse<WorkflowInstanceDto> | WorkflowInstanceDto | No | |
| retryInstance | POST | /api/workflow/admin/instances/{id}/retry | RetryInstanceRequestDto | ApiResponse<WorkflowInstanceDto> | WorkflowInstanceDto | No | |
| moveInstanceToNode | POST | /api/workflow/admin/instances/{id}/move-to-node | MoveToNodeRequestDto | ApiResponse<WorkflowInstanceDto> | WorkflowInstanceDto | No | |
| getRuntimeSnapshot | GET | /api/workflow/instances/{id}/runtime-snapshot | – | ApiResponse<InstanceRuntimeSnapshotDto> | InstanceRuntimeSnapshotDto | No | |

## 4. Tasks API Matrix
(unchanged – verified)  
All task methods unwrap & normalize status; no envelope leakage.

## 5. Events & Admin
(unchanged – verified)  
getWorkflowEvents currently unpaged; candidate for pagination later.

## 6. Real-Time (SignalR)
Duplicate terminal (100%) progress events still observed (pending dedupe guard).

## 7. Start Instance Usage Audit (FINAL – COMPLETE)
Repository-wide search confirmed no direct use of response.data.id. Multi-tenant test updated to use workflowDefinitionId. Audit stamp: PASS.

## 8. Edge Case Notes
| Concern | Status | Action |
|---------|--------|--------|
| Adaptive validation responses | Accepted | Consolidate after backend uniformity |
| Duplicate 100% progress events | Open | Add runtime/publisher guard |
| Metadata visibility | Addressed | In detail panel (not grid) |
| unwrap permissiveness | Acceptable | Optional stricter assertion deferred |
| Enum normalization | Retained | Defensive resilience |

## 9. Planned Tests (Pending)
1. unwrap error aggregation
2. Paged extraction integrity
3. Status normalization (numeric → enum)
4. PublishDefinition error mapping
5. StartInstance returns normalized DTO (no envelope)

## 10. Action Items (Revised)
| Order | Action | Status |
|-------|--------|--------|
| 1 | Service unwrapping audit | ✅ Complete |
| 2 | StartInstance usage audit | ✅ Complete |
| 3 | Progress dedupe guard | ⏳ Pending |
| 4 | FE unwrap & normalization tests | ⏳ Pending |
| 5 | Decide validateDefinitionJson consolidation | ⏳ Pending |
| 6 | (Optional) Grid column toggles / extra metadata | Deferred |

## 11. Verification Checklist
- [x] Unwrap uniform
- [x] StartInstance consumers clean
- [x] Metadata surfaced (detail panel)
- [ ] Progress dedupe
- [ ] FE tests for unwrap
- [ ] Validation consolidation decision

Audit Updated: Step 2 & 3 (original plan numbering) closed.  
Updated On: 2025-09-08
