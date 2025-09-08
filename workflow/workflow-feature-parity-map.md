# Workflow Feature Parity Map (Backend ↔ Frontend)

| Backend Feature | Endpoint(s) / Concern | Frontend Surface (Current) | Gap Summary | Priority |
|-----------------|-----------------------|----------------------------|-------------|----------|
| Start/End Nodes | Definitions JSON      | Supported in Builder       | -           | Done |
| HumanTask Node  | Definitions JSON / Tasks endpoints | Supported (basic) | Advanced role-based assignment UI missing | High |
| Automatic Node  | Runtime executor      | Shown as generic node      | Property panel for action/webhook not implemented | Medium |
| Gateway Exclusive/Conditional | Definitions; runtime decisions | Basic gateway present | No strategy toggle / expression editor | Critical |
| Gateway Parallel Fan-Out | Same as above | Not represented | No parallel mode UI | Critical |
| Join Node & Modes | Runtime join logic | Not represented | Cannot configure joins | Critical |
| Timer Node Config | Definitions; TimerWorker | Basic presence | No duration/date editor / validation | High |
| Task Assign / Cancel / Reset | assign, cancel, admin reset | Not in UI | Missing action buttons & forms | High |
| Instance Suspend / Resume | suspend/resume | Not in UI | Controls absent | High |
| Bulk Cancel Instances | admin bulk-cancel | Not in UI | Missing selection workflow | High |
| Terminate Running Definition Instances | terminate-running | Not in UI | Missing confirmation flow | High |
| Revalidate Definition | revalidate | Not in UI | No validation trigger / display | Medium |
| Definition New Version | new-version | Possibly via publish flow only | Explicit “Create New Version” action | Medium |
| Runtime Snapshot (parallelGroups, context) | runtime-snapshot | Not surfaced | No diagnostics panel | High |
| Event Timeline (WorkflowEvents) | admin/events or per instance | Not present | No timeline | High |
| Progress Dedupe Awareness | progress events | Simple progress only | No duplicate avoidance indicator / last event meta | Low |
| Tags Server Validation | definitions update | Frontend-only validation | Back-end enforcement + error surfacing missing | Medium |
| JsonLogic Expressions | condition, join expression modes | No editor | Expression builder / validator | Medium |
| Join Timeout Visibility | JoinTimeoutWorker | Not visible | Health / status panel | Low |
| Outbox Queue Visibility | Outbox messages | Not visible | Ops status panel | Medium |
| ActiveTasksCount (planned) | Instance progress payload | Not displayed | Column / badge | Medium |
| Simulation / Dry Run | (internal) | Not available | Path simulator | Medium |
| Version Diff | definitions versions | Not available | Visual diff UI | Medium |
| Metrics (lag, durations) | future observability | Not available | Basic metrics dashboard | Low |
