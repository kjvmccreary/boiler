Below is the updated backend ↔ frontend contract status (Phase 5 in progress). Completed items are marked ✅, partials 🔶, skipped tests 💤.

--------------------------------------------------
## 1. High‑impact mismatches
--------------------------------------------------
### ✅ A Definitions list response shape
Backend: Returns ApiResponseDto<List<WorkflowDefinitionDto>> (items unwrapped).
Frontend: Service method adjusted (final audit still pending in Section 2).

### 🔶 B Added / changed fields (IsArchived, ArchivedAt, PublishNotes, VersionNotes, ParentDefinitionId, Tags, ActiveInstanceCount, IsPublished, PublishedAt)
Archived filtering still not implemented (decision pending). UI does not yet expose “Show Archived” toggle.

### 🔶 C Start Instance navigation shape
Need final confirmation everywhere navigation uses unwrapped instance.id (not raw response.id). Mark COMPLETE after audit.

### ✅ D Instance completion status mismatch
Verified end‑to‑end: human task completion -> active set drained -> Instance.Status=Completed, CompletedAt set, CurrentNodeIds empty, events + final progress(100%) emitted. (See snapshot in wflogs.txt.)

### ✅ Permissions naming inconsistencies
All controllers use Permissions.* constants.

--------------------------------------------------
## 2. Frontend service refactors (audit)
Goal: uniform unwrapping of ApiResponseDto (.data.data).
Items to audit (some already correct):
- getDefinitions()
- startInstance()
- getInstance()
- getInstanceStatus()
- getRuntimeSnapshot()
- terminateInstance()
- signalInstance()
- unpublishDefinition()
- archiveDefinition()
- terminateDefinitionInstances()

Status: 🔶 PARTIAL (formal pass still OPEN).

--------------------------------------------------
## 3. Recommended backend improvements (optional)
A Paging metadata for definitions (OPEN)  
B Archived filtering param includeArchived=false (OPEN)  
C Instance finalization logic ✅ COMPLETE (Human task path validated)  
D (New) Progress event duplication optimization (see Section 11) – OPTIONAL

--------------------------------------------------
## 4. Concrete frontend code changes
Recent:
- SignalR integration for InstanceUpdated + InstanceProgress.
- InstanceStatusBadge consumes push (falls back to polling).
- Progress bar added (percentage + visited/total).

--------------------------------------------------
## 5. Quick diff checklist
| Concern | Status |
|---------|--------|
| Definitions unwrapped array | 🔶 PARTIAL |
| New definition fields surfaced in UI | OPEN |
| Start instance id access | 🔶 PARTIAL |
| Stale instance after final task | ✅ Resolved (push + refetch) |
| Permissions naming | ✅ COMPLETE |
| Archived filtering | OPEN |
| Paging metadata | OPEN |
| SignalR InstanceUpdated push | ✅ COMPLETE |
| SignalR InstanceProgress push | ✅ COMPLETE |
| Status badge polling fallback | ✅ COMPLETE |
| Join timeout tests | 💤 Skipped (temporary) |
| Progress finalization accuracy | ✅ 100% verified |
| Duplicate final progress events | OPEN (optimize) |

--------------------------------------------------
## 6. Action order (revised)
1. Audit workflowService.ts (unwrap consistency) – OPEN  
2. Confirm all startInstance navigations use unwrapped id – OPEN  
3. Implement includeArchived (backend) + toggle (frontend) – OPEN  
4. Optimize duplicate final progress emissions (optional) – OPEN  
5. Re‑enable & stabilize join timeout tests – OPEN (deferred)  
6. Definitions paging metadata (if product needs) – OPEN  
7. (Optional) Disable polling once first push seen per instance – OPTIONAL  

--------------------------------------------------
## 7. Instance finalization verification (COMPLETE)
Snapshot (wflogs.txt) confirms:
- Status=Completed, CompletedAt set
- CurrentNodeIds = []
- Progress lastPercent=100
- Task Completed event emitted
- Instance Completed + final Progress events present
No further changes required for core finalization.

--------------------------------------------------
## 8. Completed summary
✅ Permissions normalization  
✅ Instance lifecycle push (InstanceUpdated)  
✅ Progress push (InstanceProgress)  
✅ Human task completion -> instance finalization validated  
✅ Context tracking (_visited, _progress)  
✅ Frontend badge + progress bar integration  

--------------------------------------------------
## 9. Pending decision points
- Server-side archived filtering now or later?
- Need paging or infinite scroll soon?
- Keep multiple Progress(100%) events or dedupe?
- When to re-activate join timeout tests?

--------------------------------------------------
## 10. Next recommended steps (immediate)
1. Perform service unwrapping audit (Section 2).  
2. Add includeArchived param (default false) & UI toggle.  
3. Dedupe final progress events (emit single 100% after completion).  
4. Re-introduce timeout tests post refactor or mark permanently replaced by new coverage.  

--------------------------------------------------
## 11. Observations / minor technical debt
- Duplicate final Progress events (3 identical 100% entries) — low risk; can suppress by caching last emitted percentage + a terminal guard.  
- NodeActivated events appear twice for human task (different payload shape); could consolidate later.  
- Assigned task event before claim is present; ensure ordering consistent with UX expectations if building timeline view.

--------------------------------------------------
## 12. Backlog (not blocking)
- Progress event consolidation (single terminal emission).
- Optional: Include activeTasksCount / openHumanTasks in InstanceUpdated.
- Streaming of only deltas (reduce event volume).
- Per-tenant throttling / coalescing of high-frequency automatic nodes.

--------------------------------------------------
## 13. Skipped tests note
Join timeout tests (force / fail / route) temporarily skipped while join timeout scaffolding & metadata creation are refined. Track reactivation to avoid silent regressions.

--------------------------------------------------
End of current status (Phase 5 in progress)
