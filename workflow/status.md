Below is the updated backend ↔ frontend contract status (Phase 5 in progress). Completed items are marked ✅, partials 🔶, skipped tests 💤.

--------------------------------------------------
## 1. High‑impact mismatches
--------------------------------------------------
### ✅ A Definitions list response shape
Unified unwrap (paged + items). FE & BE aligned.

### ✅ B Added / changed fields
IsArchived, ArchivedAt, PublishNotes, VersionNotes, ParentDefinitionId, Tags, ActiveInstanceCount, IsPublished, PublishedAt all travel and are surfaced (detail panel, not all grid columns).

### ✅ C Start Instance navigation shape
Callers use workflowService.startInstance() → result.id (no legacy response.data.id).

### ✅ D Instance completion status mismatch
Completion sets Status=Completed, CompletedAt, clears CurrentNodeIds, emits exactly one 100% progress event (dedupe guard active).

### ✅ Permissions naming inconsistencies
Controllers use Permissions.* constants.

--------------------------------------------------
## 2. Frontend service unwrapping audit
Result: ✅ COMPLETE. Matrix in service-unwrapping-audit.md (2025‑09‑08). No envelope leakage beyond service layer.

--------------------------------------------------
## 3. Recommended backend improvements (optional)
A Paging metadata for definitions ✅  
B Archived filtering includeArchived=false ✅  
C Instance finalization logic ✅  
D Progress terminal event duplication ✅ (deduped single 100%)  

--------------------------------------------------
## 4. Concrete recent frontend changes
- Added comprehensive contract test suite (definitions / instances / tasks / status normalization).
- Error path test for publishDefinition temporarily skipped pending helper refactor.
- Terminal progress dedupe implemented (in-memory lastPercent + suppression of duplicate 100%).
- Adapters added to re-export consolidated workflow.service.ts for tests.

--------------------------------------------------
## 5. Quick diff checklist
| Concern                                | Status       | Notes |
|----------------------------------------|--------------|-------|
| Definitions response envelope          | ✅ COMPLETE  | Unwrap verified by tests |
| New definition fields surfaced (UI)    | ✅ COMPLETE  | Detail panel |
| Start instance id access               | ✅ COMPLETE  | Flattened DTO |
| Stale instance after final task        | ✅ COMPLETE  | Live updates |
| Permissions naming                     | ✅ COMPLETE  | |
| Archived filtering                     | ✅ COMPLETE  | |
| Paging metadata                        | ✅ COMPLETE  | |
| SignalR InstanceUpdated push           | ✅ COMPLETE  | |
| SignalR InstanceProgress push          | ✅ COMPLETE  | |
| Status badge polling fallback          | ✅ COMPLETE  | |
| Join timeout tests                     | 💤 SKIPPED   | Decision pending |
| Progress finalization accuracy         | ✅ COMPLETE  | |
| Duplicate final progress events        | ✅ COMPLETE  | Guarded |
| Service unwrapping audit               | ✅ COMPLETE  | |
| StartInstance usage audit              | ✅ COMPLETE  | |
| FE contract test suite                 | 🔶 PARTIAL   | One skipped publishDefinition error test |
| PublishDefinition error surfacing      | 🔶 PARTIAL   | Helper pending |
| Tag delimiter policy                   | OPEN         | Decision needed |

--------------------------------------------------
## 6. Action order (current)
1. (In Progress) Add extractApiErrors helper + refactor publish/unpublish/archive (Micro-step 1).  
2. Re-enable skipped publishDefinition error test (Micro-step 2).  
3. Decide fate of join timeout tests (reinstate vs retire with rationale).  
4. Tag delimiter policy decision (commas-only vs current splitter) & doc update.  
5. Optional: Column toggles / user prefs.  
6. Optional: Add activeTasksCount to InstanceUpdated payload.  
7. Optional: Progress rate metric (events/min) for anomaly detection.

--------------------------------------------------
## 7. Instance finalization verification (recap)
- Status → Completed
- CompletedAt populated
- CurrentNodeIds cleared
- Single 100% Progress event
- Event order stable

--------------------------------------------------
## 8. Completed summary
✅ Permissions normalization  
✅ Instance lifecycle push + deduped progress  
✅ Definitions metadata surfaced  
✅ Archived filtering  
✅ Paged envelope integration  
✅ Service unwrapping & StartInstance audits  
✅ Contract tests (core paths)  
✅ Terminal progress dedupe  

--------------------------------------------------
## 9. Decision points
- Join timeout tests: reinstate vs formally retire.
- Tag splitting: allow multi-word tags (comma-only) or retain current regex?
- activeTasksCount metric addition timing.

--------------------------------------------------
## 10. Next recommended steps
1. Finish error helper + unskip test.
2. Document join timeout test decision.
3. Tag policy + status.md update.
4. (Optional) Add progress rate metric.
5. (Optional) Extend InstanceUpdated payload.

--------------------------------------------------
## 11. Observations / minor technical debt
- publishDefinition / archive / unpublish share ad-hoc error parsing (helper pending).
- One skipped test indicates mock consistency gap (legacy fetch vs axios spy).
- Tag splitting may break multi-word phrases.

--------------------------------------------------
## 12. Backlog (non‑blocking)
- activeTasksCount in InstanceUpdated.
- Event burst coalescing.
- Role usage reporting UI.
- Structured error classification (401/403/409/422).
- Modularize workflow.ts types.
- Progress rate metric.

--------------------------------------------------
## 13. Skipped tests note
Join timeout tests + one publishDefinition error handling test (temporarily skipped).

--------------------------------------------------
End of current status – Updated 2025‑09‑08
