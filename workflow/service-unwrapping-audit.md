# Workflow Frontend Service – API Unwrapping & Contract Audit (Phase 5)

Status: COMPLETE (All core unwrap + normalization test coverage in place).  
Last Updated: 2025-09-08 (includes advanced tag filter enhancements)

## 1. Canonical Envelope
`interface ApiResponse<T> { success: boolean; message?: string; errors?: (string | { code?: string; message?: string })[] | any; data: T }`

All service methods route through unified `unwrap<T>()`. Custom errors now centralized via `extractApiErrors` (publish / unpublish / archive / terminateDefinitionInstances / validateDefinitionJson / validateDefinitionById).

## 2. Definitions API Matrix (Current)
| Method | Path | Raw | Returns (post-unwrap) | Notes |
|--------|------|-----|-----------------------|-------|
| getDefinitionsPaged | /api/workflow/definitions | ApiResponse<PagedResultDto<Definition>> | PagedResultDto<WorkflowDefinitionDto> | Supports query params: `search`, `published`, `includeArchived`, legacy `tags` (OR), `anyTags` (OR), `allTags` (AND), paging & sort |
| getDefinitions | same | same | WorkflowDefinitionDto[] | Convenience wrapper (defaults: newest first, pageSize=100) |
| getDefinition | /definitions/{id} | ApiResponse<Definition> | WorkflowDefinitionDto |  |
| createDraft | /definitions/draft | ApiResponse<Definition> | WorkflowDefinitionDto |  |
| updateDefinition | /definitions/{id} | ApiResponse<Definition> | WorkflowDefinitionDto |  |
| publishDefinition | /definitions/{id}/publish | ApiResponse<Definition> | WorkflowDefinitionDto | Helper-based error extraction |
| validateDefinitionJson | /definitions/validate | ApiResponse<ValidationResultDto> | GraphValidationResult | Uses helper on failure |
| validateDefinitionById | /definitions/{id}/validate | ApiResponse<ValidationResultDto> | GraphValidationResult | Uses helper on failure |
| validateDefinition | /definitions/validate | ApiResponse<ValidationResultDto> | ValidationResultDto | Direct unwrap |
| createNewVersion | /definitions/{id}/new-version | ApiResponse<Definition> | WorkflowDefinitionDto |  |
| revalidateDefinition | /definitions/{id}/revalidate | ApiResponse<ValidationResultDto> | ValidationResultDto |  |
| deleteDefinition | /definitions/{id} (DELETE) | ApiResponse<boolean> | boolean |  |
| unpublishDefinition | /definitions/{id}/unpublish | ApiResponse<Definition> | WorkflowDefinitionDto | Helper |
| archiveDefinition | /definitions/{id}/archive | ApiResponse<Definition> | WorkflowDefinitionDto | Helper |
| terminateDefinitionInstances | /definitions/{id}/terminate-running | ApiResponse<{terminated:number}> | { terminated:number } | Helper |

Metadata surfaced (detail panel only): PublishNotes, VersionNotes, Tags, ActiveInstanceCount, ParentDefinitionId, IsArchived, ArchivedAt, IsPublished, PublishedAt.

### Tag Filtering Enhancements
- Legacy `tags` query param retained (OR semantics) for backward compatibility.
- New `anyTags` (OR) & `allTags` (AND) allow combined boolean logic (All ∧ (Any set)).
- Backend normalizes stored & filter tags (comma-only, case-insensitive, de-duplicated, multi-word preserved).
- Frontend persists last-used `anyTags` / `allTags` in `localStorage`, provides chip deletion & explicit Apply/Reset.

## 3. Instances API Matrix
(unchanged)

| Method | Path | Returns | Post-processing |
|--------|------|---------|-----------------|
| getInstancesPaged | /instances | PagedResultDto<WorkflowInstanceDto> | Status normalization |
| getInstances | /instances | WorkflowInstanceDto[] | Status normalization |
| getInstance | /instances/{id} | WorkflowInstanceDto | Status normalization |
| getInstanceStatus | /instances/{id}/status | InstanceStatusDto | Status normalization |
| startInstance | /instances | WorkflowInstanceDto | Flattened id |
| signalInstance | /instances/{id}/signal | WorkflowInstanceDto | Status normalization |
| terminateInstance | /instances/{id} (DELETE) | boolean |  |
| suspendInstance | /instances/{id}/suspend | WorkflowInstanceDto |  |
| resumeInstance | /instances/{id}/resume | WorkflowInstanceDto |  |
| retryInstance | /admin/instances/{id}/retry | WorkflowInstanceDto |  |
| moveInstanceToNode | /admin/instances/{id}/move-to-node | WorkflowInstanceDto |  |
| getRuntimeSnapshot | /instances/{id}/runtime-snapshot | InstanceRuntimeSnapshotDto |  |

## 4. Tasks API (Verified)
No changes; unwrap & status normalization intact.

## 5. Events & Admin
Still pending helper standardization for some admin/event endpoints (non-blocking).

## 6. Real-Time Progress
Terminal 100% dedupe guard active. Additional guard added in `StartWorkflowAsync` for immediate auto-complete flows (prevents lingering 50% as last event).

## 7. Start Instance Usage Audit
PASS – no envelope leakage.

## 8. Error Handling Improvements
| Area | Previous | Now |
|------|----------|-----|
| publishDefinition | Generic text | First specific error surfaced |
| unpublish/archive/terminate | Basic unwrap | Helper-based |
| validateDefinitionJson / ById | Manual divergent parsing | Helper unified |
| Remaining minor endpoints | Manual | Optional future alignment |

## 9. Edge Case Coverage
| Concern | Status | Notes |
|---------|--------|-------|
| Numeric → enum normalization | Covered | Tests |
| Paged integrity | Covered |  |
| Error envelope propagation | Covered | Helper paths |
| StartInstance flattening | Covered |  |
| Validation tolerant mapping | Accepted | Unified output |
| Tag boolean logic (AND/OR) | NEW Covered | Any + All filter tests backend |

## 10. Test Coverage Summary
(unchanged except new tag filter tests added)

- Added backend AND / OR tag filter unit tests (AllTags, AnyTags, Combined).
- Frontend persistence not unit-tested (acceptable for initial dev).

## 11. Outstanding / Follow-Up
| Item | Type | Action |
|------|------|--------|
| Standardize remaining event/admin errors | Consistency | Optional |
| Remove test adapters | Cleanup | Pending |
| Join timeout activation tests | Engine | Partial (3 skipped) |
| Legacy `tags` deprecation path | Product | Decide timeline |

## 12. Verification Checklist (Updated)
- [x] Uniform unwrap logic
- [x] Error mapping (definitions critical paths + validation)
- [x] Progress dedupe & completion guard
- [x] Status normalization
- [x] Paging integrity
- [x] Tag normalization (storage + filters)
- [x] AND/OR tag filter semantics (backend + UI)
- [ ] Remaining minor endpoints helper adoption (optional)

## 13. Risk / Impact Notes
- Mixing legacy `tags` with `anyTags/allTags` could confuse callers; documented precedence (legacy only used when AnyTags absent).
- Tag normalization reduces accidental partial matches; risk of unexpected stricter results for prior substring assumptions (acceptable in early dev).
- Retained adapter layer adds minimal complexity—removal lowers maintenance.

## 14. Recommendations (Immediate)
1. Document precedence: if `anyTags` provided, ignore legacy `tags`. (Backend already does.)
2. Decide on timeline to deprecate legacy `tags`.
3. Optional: add OR/AND tag chips → clickable to refine (UI enhancement).
4. Consolidate remaining admin/event error handling into helper for uniform telemetry.

Audit Updated with advanced tag filter support & validation helpers integration.
