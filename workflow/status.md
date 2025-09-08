Below is the updated backend ↔ frontend contract status (Phase 5 in progress). Completed items are marked ✅, partials 🔶, skipped tests 💤.

--------------------------------------------------
## 1. High‑impact mismatches
--------------------------------------------------
### ✅ A Definitions list response shape
Unified unwrap (paged + items). FE & BE aligned.

### ✅ B Added / changed fields
IsArchived, ArchivedAt, PublishNotes, VersionNotes, ParentDefinitionId, Tags, ActiveInstanceCount, IsPublished, PublishedAt all travel and are surfaced.

### ✅ C Start Instance navigation shape
Callers use workflowService.startInstance() → result.id.

### ✅ D Instance completion status mismatch
Completion sets Status=Completed, CompletedAt, clears CurrentNodeIds, emits exactly one 100% progress event (dedupe + final guard).

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
D Progress terminal event duplication ✅ (deduped + completion guard)  

--------------------------------------------------
## 4. Concrete recent frontend changes
- Advanced tag filtering UI (AllTags=AND, AnyTags=OR) + persistence (localStorage) + removable chips.
- Backend request updated to supply `allTags` / `anyTags`.
- Added helper adoption for validation endpoints.
- Added final progress guard for auto-complete flows.
- Existing publish/unpublish/archive terminate helper usage retained.

--------------------------------------------------
## 5. Quick diff checklist
| Concern                                | Status       | Notes |
|----------------------------------------|--------------|-------|
| Definitions response envelope          | ✅ COMPLETE  | Unwrap verified |
| New definition fields surfaced (UI)    | ✅ COMPLETE  | Detail panel |
| Start instance id access               | ✅ COMPLETE  | Flattened DTO |
| Stale instance after final task        | ✅ COMPLETE  | Live updates |
| Permissions naming                     | ✅ COMPLETE  | |
| Archived filtering                     | ✅ COMPLETE  | |
| Paging metadata                        | ✅ COMPLETE  | |
| SignalR InstanceUpdated push           | ✅ COMPLETE  | |
| SignalR InstanceProgress push          | ✅ COMPLETE  | |
| Status badge polling fallback          | ✅ COMPLETE  | |
| Join timeout tests                     | 🔶 PARTIAL   | 5 scenarios; 3 skipped |
| Progress finalization accuracy         | ✅ COMPLETE  | |
| Duplicate final progress events        | ✅ COMPLETE  | Guarded |
| Service unwrapping audit               | ✅ COMPLETE  | |
| StartInstance usage audit              | ✅ COMPLETE  | |
| FE contract test suite                 | ✅ COMPLETE  | All active tests passing |
| PublishDefinition error surfacing      | ✅ COMPLETE  | Helper |
| Tag delimiter policy                   | ✅ COMPLETE  | Comma-only |
| Advanced tag AND/OR filtering          | ✅ COMPLETE  | anyTags / allTags + UI |
| Validation endpoints helper adoption   | ✅ COMPLETE  | validate JSON & by Id |

--------------------------------------------------
## 6. Action order (current)
1. Decide join timeout activation tests (fix vs retire).
2. Legacy `tags` param deprecation decision (document precedence).
3. Optional: Column toggles / saved grid prefs.
4. Optional: Add activeTasksCount to InstanceUpdated payload.
5. Optional: Progress rate metric (events/min).
6. Collapse test adapters to direct service imports.

--------------------------------------------------
## 7. Instance finalization verification (recap)
- Status → Completed  
- CompletedAt populated  
- CurrentNodeIds cleared  
- Single 100% Progress event (guard for duplicates & early auto-complete)  
- Event order stable  

--------------------------------------------------
## 8. Completed summary
✅ Permissions normalization  
✅ Instance lifecycle push + deduped progress  
✅ Definitions metadata surfaced  
✅ Archived filtering  
✅ Paged envelope integration  
✅ Service unwrapping & StartInstance audits  
✅ Contract tests (all active)  
✅ Terminal progress dedupe + guard  
✅ Standardized API error parsing  
✅ Advanced AND/OR tag filtering (backend + UI + persistence)  
✅ Validation endpoints helper adoption  

--------------------------------------------------
## 9. Decision points
- Join timeout test strategy.
- Legacy `tags` parameter lifecycle (warn & remove?).
- activeTasksCount timing.

--------------------------------------------------
## 10. Next recommended steps
1. Document precedence (anyTags/allTags > legacy tags) & add deprecation notice.
2. Decide join timeout test plan / retirement note.
3. (Optional) Persist grid column visibility & sort preferences.
4. (Optional) Add activeTasksCount to InstanceUpdated & UI.

--------------------------------------------------
## 11. Observations / minor technical debt
- Adapters still present (test indirection).
- Few endpoints still use ad-hoc error handling (non-critical).
- Legacy `tags` param may cause confusion alongside new params.
- Join timeout worker scenarios still partially validated.

--------------------------------------------------
## 12. Backlog (non‑blocking)
- activeTasksCount in InstanceUpdated.
- Event burst coalescing.
- Role usage reporting UI.
- Structured error classification (401/403/409/422).
- Modularize workflow types.
- Progress rate metric.
- Deprecation banner / console warning for legacy `tags`.

--------------------------------------------------
## 13. Skipped tests note
Skipped (join timeout activation variants):
- JoinTimeout_FailAction_ShouldFailInstance
- JoinTimeout_ForceAction_ShouldAddJoinNodeToActive
- JoinTimeout_RouteAction_ShouldAddTargetNodeOnly
Reason: Worker activation / context scaffolding mismatch; deferred.

--------------------------------------------------
End of current status – Updated 2025‑09‑08
