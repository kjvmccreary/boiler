Below is the updated backend ↔ frontend contract status (Phase 5 in progress). Completed items are marked ✅, partials 🔶, skipped tests 💤.

--------------------------------------------------
## 1. High‑impact mismatches
--------------------------------------------------
### ✅ A Definitions list response shape
Backend & frontend aligned: ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>> → frontend unwraps (paged helper + items shortcut).

### ✅ B Added / changed fields
New backend fields: IsArchived, ArchivedAt, PublishNotes, VersionNotes, ParentDefinitionId, Tags, ActiveInstanceCount, IsPublished, PublishedAt.  
All travel end‑to‑end. UI now surfaces PublishNotes, VersionNotes, Tags, ActiveInstanceCount, ParentDefinitionId in the Definitions detail panel (intentionally not all as base columns).

### ✅ C Start Instance navigation shape
All callers use workflowService.startInstance() → instance.id (no legacy response.data.id usages). Multi‑tenant test updated.

### ✅ D Instance completion status mismatch
Completion sets Status=Completed, CompletedAt, clears CurrentNodeIds, emits ≥ one 100% progress event.

### ✅ Permissions naming inconsistencies
Controllers use Permissions.* constants.

--------------------------------------------------
## 2. Frontend service unwrapping audit
Goal: uniform ApiResponse<T> handling.  
Result: ✅ COMPLETE. Documented in service-unwrapping-audit.md (2025‑09‑08). No envelope leakage outside service layer.

--------------------------------------------------
## 3. Recommended backend improvements (optional)
A Paging metadata for definitions ✅  
B Archived filtering includeArchived=false ✅  
C Instance finalization logic ✅  
D Progress terminal event duplication – OPEN (still optional; low impact)

--------------------------------------------------
## 4. Concrete recent frontend changes
- Definitions detail panel now exposes PublishNotes / VersionNotes / Tags / ActiveInstanceCount / ParentDefinitionId.
- Multi‑tenant test corrected (removed legacy definitionKey).
- DataGrid activeInstanceCount column fixed (removed invalid valueGetter).
- StartInstance usage sweep & audit completed.

--------------------------------------------------
## 5. Quick diff checklist
| Concern                                | Status      | Notes |
|----------------------------------------|-------------|-------|
| Definitions response envelope          | ✅ COMPLETE | Unified unwrap |
| New definition fields surfaced (UI)    | ✅ COMPLETE | Shown in detail panel |
| Start instance id access               | ✅ COMPLETE | No legacy patterns |
| Stale instance after final task        | ✅ COMPLETE | Live updates |
| Permissions naming                     | ✅ COMPLETE | |
| Archived filtering                     | ✅ COMPLETE | Toggle + param |
| Paging metadata                        | ✅ COMPLETE | PagedResultDto |
| SignalR InstanceUpdated push           | ✅ COMPLETE | |
| SignalR InstanceProgress push          | ✅ COMPLETE | |
| Status badge polling fallback          | ✅ COMPLETE | |
| Join timeout tests                     | 💤 SKIPPED  | Decision pending |
| Progress finalization accuracy         | ✅ COMPLETE | |
| Duplicate final progress events        | OPEN        | Dedupe guard pending |
| Service unwrapping audit               | ✅ COMPLETE | Matrix stored |
| StartInstance usage audit              | ✅ COMPLETE | Documented |

--------------------------------------------------
## 6. Action order (current)
1. Implement terminal progress dedupe guard (single 100% emission).
2. Add FE tests for unwrap + status normalization.
3. Decide fate of join timeout tests (reinstate or deprecate with rationale).
4. (Optional) Tag delimiter policy (commas-only vs current comma/space split).
5. (Optional) Add column toggles / user prefs for metadata fields.
6. (Optional) Backend: add activeTasksCount to InstanceUpdated payload.

--------------------------------------------------
## 7. Instance finalization verification (recap)
- Status → Completed
- CompletedAt populated
- CurrentNodeIds cleared
- Progress 100% event emitted
- Order: Task completion → Progress → Instance completed events

--------------------------------------------------
## 8. Completed summary
✅ Permissions normalization  
✅ Instance lifecycle push + progress events  
✅ Definitions metadata surfaced (detail panel)  
✅ Archived filtering (API + UI)  
✅ Paged envelope integration  
✅ Service unwrapping & StartInstance audits  
✅ Multi-tenant definition/task isolation test aligned  

--------------------------------------------------
## 9. Decision points
- Progress dedupe: implement now or defer?
- Join timeout tests: resurrect vs retire (add explicit ticket).
- Tag splitting: allow multi-word tags (switch to comma-only) or retain current regex?

--------------------------------------------------
## 10. Next recommended steps
1. Add progress dedupe (cache last emitted percentage & suppress duplicate terminal).
2. Write vitest suite: publishDefinition error mapping, numeric → enum normalization, paged unwrap integrity.
3. Add explicit decision note on join timeout tests in backlog.
4. If multi-word tags needed: change splitTags to comma-only & update contribution doc.
5. Optional: Add activeTasksCount metric to InstanceUpdated to reduce polling.

--------------------------------------------------
## 11. Observations / minor technical debt
- Duplicate terminal Progress events (noise).
- Tag regex splits multi-word phrases unintentionally.
- Validation endpoints have dual adaptive paths (can be consolidated later).
- Sparse FE automated tests for workflow service (risk for future contract drift).
- Builder/editor types monolithic (consider modularizing if growth continues).

--------------------------------------------------
## 12. Backlog (non‑blocking)
- Progress event consolidation & terminal dedupe.
- Extend InstanceUpdated payload (activeTasksCount).
- Event delta coalescing for high-frequency paths.
- Role usage reporting UI.
- Structured error classification (401/403/409/422) for consistent toasts.
- Modularize workflow.ts type definitions.

--------------------------------------------------
## 13. Skipped tests note
Join timeout tests remain skipped; create an explicit tracking ticket to avoid silent regression.

--------------------------------------------------
End of current status (Phase 5 in progress) – Updated 2025‑09‑08
