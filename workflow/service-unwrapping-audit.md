# Workflow Frontend Service – API Unwrapping & Contract Audit (Phase 5)

Status: COMPLETE (All core unwrap + normalization test coverage in place).  
Last Updated: 2025-09-08

## 1. Canonical Envelope
`interface ApiResponse<T> { success: boolean; message?: string; errors?: (string | { code?: string; message?: string })[] | any; data: T }`

All service methods route through a single `unwrap<T>()` except specialized error handlers (publish/unpublish/archive/terminateDefinitionInstances now refactored via `extractApiErrors`).

## 2. Definitions API Matrix (Current)
| Method | Path | Raw | Returns (post-unwrap) | Notes |
|--------|------|-----|-----------------------|-------|
| getDefinitionsPaged | /api/workflow/definitions | ApiResponse<PagedResultDto<Definition>> | PagedResultDto<WorkflowDefinitionDto> | Grid |
| getDefinitions | same | same | WorkflowDefinitionDto[] | Convenience |
| getDefinition | /definitions/{id} | ApiResponse<Definition> | WorkflowDefinitionDto | |
| createDraft | /definitions/draft | ApiResponse<Definition> | WorkflowDefinitionDto | |
| updateDefinition | /definitions/{id} | ApiResponse<Definition> | WorkflowDefinitionDto | |
| publishDefinition | /definitions/{id}/publish | ApiResponse<Definition> | WorkflowDefinitionDto | Custom error parsing (helper) |
| validateDefinitionJson | /definitions/validate | ApiResponse<ValidationResultDto> | GraphValidationResult | Legacy tolerant mapping |
| validateDefinitionById | /definitions/{id}/validate | ApiResponse<ValidationResultDto> | GraphValidationResult | |
| validateDefinition | /definitions/validate | ApiResponse<ValidationResultDto> | ValidationResultDto | Direct unwrap |
| createNewVersion | /definitions/{id}/new-version | ApiResponse<Definition> | WorkflowDefinitionDto | |
| revalidateDefinition | /definitions/{id}/revalidate | ApiResponse<ValidationResultDto> | ValidationResultDto | |
| deleteDefinition | /definitions/{id} (DELETE) | ApiResponse<boolean> | boolean | |
| unpublishDefinition | /definitions/{id}/unpublish | ApiResponse<Definition> | WorkflowDefinitionDto | Helper |
| archiveDefinition | /definitions/{id}/archive | ApiResponse<Definition> | WorkflowDefinitionDto | Helper |
| terminateDefinitionInstances | /definitions/{id}/terminate-running | ApiResponse<{terminated:number}> | { terminated:number } | Helper |

Metadata surfaced (detail panel only): PublishNotes, VersionNotes, Tags, ActiveInstanceCount, ParentDefinitionId, IsArchived, ArchivedAt, IsPublished, PublishedAt.

## 3. Instances API Matrix
| Method | Path | Returns | Post-processing |
|--------|------|---------|-----------------|
| getInstancesPaged | /instances | PagedResultDto<WorkflowInstanceDto> | Status normalization |
| getInstances | /instances | WorkflowInstanceDto[] | Status normalization |
| getInstance | /instances/{id} | WorkflowInstanceDto | Status normalization |
| getInstanceStatus | /instances/{id}/status | InstanceStatusDto | Status normalization |
| startInstance | /instances | WorkflowInstanceDto | Flattened id usage |
| signalInstance | /instances/{id}/signal | WorkflowInstanceDto | Status normalization |
| terminateInstance | /instances/{id} (DELETE) | boolean | |
| suspendInstance | /instances/{id}/suspend | WorkflowInstanceDto | |
| resumeInstance | /instances/{id}/resume | WorkflowInstanceDto | |
| retryInstance | /admin/instances/{id}/retry | WorkflowInstanceDto | |
| moveInstanceToNode | /admin/instances/{id}/move-to-node | WorkflowInstanceDto | |
| getRuntimeSnapshot | /instances/{id}/runtime-snapshot | InstanceRuntimeSnapshotDto | |

## 4. Tasks API (Verified)
All task methods unwrap arrays or single DTOs and normalize task status (`Created/Assigned/...`). No envelope leakage.

## 5. Events & Admin
- getWorkflowEvents returns unwrapped array (non-paged).
- bulkCancelInstances, getWorkflowStats, getTaskStatistics all unwrap cleanly.
- No custom error helper applied yet (future consistency task).

## 6. Real-Time Progress
Duplicate terminal (100%) progress events: RESOLVED (dedupe guard active). Previous “Open” item closed.

## 7. Start Instance Usage Audit
Confirmed: no direct `response.data` property access outside service. All consumers rely on flattened DTO. PASS.

## 8. Error Handling Improvements
| Area | Previous | Now |
|------|----------|-----|
| publishDefinition | Generic “[object Object]” / “Publish failed” | Specific first error (message/code) |
| unpublish/archive/terminateDefinitionInstances | Basic unwrap | Standardized helper |
| Remaining (validateDefinitionJson etc.) | Mixed manual parsing | Candidate for helper adoption |

## 9. Edge Case Coverage
| Concern | Status | Notes |
|---------|--------|-------|
| Numeric → enum normalization (instances/tasks) | Covered | Tests assert conversion |
| Paged integrity (definitions/instances) | Covered | Contract tests |
| Error envelope propagation | Covered | publishDefinition + generic unwrap failures |
| StartInstance flattening | Covered | Tests confirm absence of `success/data` |
| Validation tolerant mapping | Accepted | Unified refactor optional |

## 10. Test Coverage Summary
- Definitions: paged, single, create/update/publish, error path.
- Instances: paged, start, signal, retry, move, status normalization, error path.
- Tasks: get, paged synthetic wrapper, claim/assign/complete.
- Unwrapping error propagation: explicit tests.
- Skipped (outside unwrap scope): join timeout activation (engine timing) – tracked separately.

## 11. Outstanding / Follow-Up
| Item | Type | Action |
|------|------|--------|
| Apply helper to validateDefinitionJson & events/admin endpoints | Consistency | Pending |
| Tag parsing decision | Product | Pending |
| Remove adapters in favor of direct workflow.service imports | Cleanup | Pending |
| Join timeout activation tests (engine) | Separate | Partial / 3 skipped |

## 12. Verification Checklist (Updated)
- [x] Uniform unwrap logic
- [x] Error mapping improved for key definition actions
- [x] Progress dedupe in place
- [x] Status normalization tested
- [x] StartInstance envelope removed & tested
- [x] Paged extraction integrity
- [ ] Helper adoption across all remaining error catch blocks (optional)
- [ ] Tag parsing policy finalized

## 13. Risk / Impact Notes
- Remaining ad-hoc catches (validateDefinitionJson) could diverge if backend error shapes change.
- Tag splitting ambiguity may produce inconsistent filter behavior for multi-word tags.
- Adapter layer adds minimal overhead; safe to remove post-cleanup.

## 14. Recommendations (Immediate)
1. Apply `extractApiErrors` to validateDefinitionJson / validateDefinitionById for uniformity.
2. Decide tag policy & document in status.md + builder UI hint.
3. Remove service adapters (direct imports in tests) to reduce churn.
4. Optionally add an assertion helper to centralize “no envelope” expectations for future tests.

Audit Updated: All previously “Pending” test items now closed; only consistency & product decisions remain.
