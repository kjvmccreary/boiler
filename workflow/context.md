Application Context Document (Comprehensive Snapshot – Phase 5+ With Recent UI & Runtime Enhancements)

1. High‑Level Overview
A multi-tenant workflow orchestration platform built on .NET 9 (C# 13) with a React 18 (TypeScript) frontend. Core capabilities: versioned workflow definitions (JSON DSL), instance execution engine (human, automatic, gateway, timer, join), task lifecycle, event stream persistence, publish/version lifecycle, and real-time progress / task notifications (SignalR). Recent work delivered contract stabilization (paged envelopes, archived filtering), advanced AND/OR tag filtering with persistence, tag editing & validation in both Builder and Definitions management UI, final progress event dedupe guard, and enriched definition metadata surface.

2. Tech Stack
Backend: .NET 9, C# 13, ASP.NET Core, EF Core 9 (PostgreSQL), Serilog.
Frontend: React 18, Vite, TypeScript, MUI 5, @mui/x-data-grid-premium, ReactFlow (builder), SignalR client.
Auth & Security: JWT + permission claims (“permission” claim), multi-tenant via ITenantProvider (X-Tenant-ID header).
Persistence: PostgreSQL (WorkflowDefinitions, WorkflowInstances, WorkflowTasks, WorkflowEvents, OutboxMessages).
Messaging Pattern: Outbox table (dispatcher not yet fully implemented).
Background Workers (current / planned): TimerWorker (pending), OutboxDispatcher (pending), JoinTimeoutWorker (problematic), future anomaly/progress workers.
Testing: xUnit + FluentAssertions + Moq (unit/integration); frontend minimal runtime contract reliance (no deep UI test suite yet).

3. Projects / Assemblies
WorkflowService (API + engine)
DTOs (transport contracts)
Contracts (interfaces for tenancy, notifications, conditions)
Common (utility/auditing helpers)
ApiGateway (reverse proxy / multi-service routing)
Frontend (react-app)
WorkflowService.Tests (unit/integration)
Misc: Potential administrative or support libraries (role/permission seeding).

4. Core Domain Entities
WorkflowDefinition
  Fields: Id, Name, Version, JSONDefinition, IsPublished, PublishedAt, PublishNotes, VersionNotes, IsArchived, ArchivedAt, ParentDefinitionId, Tags (comma-separated, normalized), ActiveInstanceCount.
  Immutable after publish (JSONDefinition should not change; new version path).
WorkflowInstance
  Fields: Id, WorkflowDefinitionId, DefinitionVersion, Status (Running | Suspended | Completed | Cancelled | Failed), CurrentNodeIds (JSON or list), Context (JSON), StartedAt, CompletedAt, ErrorMessage.
  Tracks active and visited nodes via context arrays (_visited, _parallelGroups, join metadata).
WorkflowTask
  Fields: Id, WorkflowInstanceId, NodeId, TaskName, NodeType (human/timer/auto), Status (Created→...→Completed/Cancelled/Failed), AssignedToUserId, AssignedToRole, DueDate, CompletionData, CompletedAt.
WorkflowEvent
  Timeline record: WorkflowInstanceId, Type (Instance | Task | Edge | Gateway | Parallel | Node | Signal), Name, Data (JSON), OccurredAt, UserId (optional).
OutboxMessage
  Fields: Id, TenantId, EventType, EventData, IsProcessed, RetryCount, (Planned) IdempotencyKey, ProcessedAt, Error.
  Future: unique index on (TenantId, IdempotencyKey).

5. Security & Tenancy Model
Tenant isolation enforced at query layer and runtime caching:
  _instanceTenantCache keyed by InstanceId.
Permissions:
  Granular constants (workflow.view_instances, workflow.start_instances, workflow.view_tasks, workflow.claim_tasks, workflow.complete_tasks, workflow.admin, etc.).
  Controllers decorated with permission checks.
AuthZ Determination:
  JWT contains permission claims; X-Tenant-ID header used to scope all operations.
Risk:
  Some deep runtime queries rely on cached TenantId; audit coverage pending for tenant leakage.

6. DSL & Builder Model
DSL (WorkflowDefinition JSON):
  nodes[]: { id, type, properties{} }, edges[]: { id, source/from, target/to, (optional labels / branch handles) }.
Supported Node Types: Start, End, HumanTask, Automatic, Gateway (conditional, parallel, exclusive semantics), Timer, Join (gateway join), plus potential placeholders.
Builder:
  serializeToDsl / deserializeFromDsl mapping ReactFlow nodes/edges ↔ DSL.
  Gateway branch normalization (true/false).
  Tag entry integrated (workflowTags state; validation + normalization via normalizeTags).
  Publish path runs Graph + DSL validation before runtime publish call.

7. Execution Engine (WorkflowRuntime)
Overview:
  StartWorkflowAsync: creates instance, marks start visited, invokes ContinueWorkflowAsync, emits initial or terminal progress (guard for immediate auto-complete flows).
  ContinueWorkflowAsync: loops active node execution until yielding (waiting tasks) or completion or safety counter (5000).
  Node Execution: delegating to INodeExecutor implementations (HumanTask, Automatic, Gateway, Timer placeholder, Start/End fallback).
  Progress Emission:
    ComputeAndMaybeEmitProgressAsync calculates percentage = visitedCount / total countable nodes (excludes Start).
    Dedupe: _progressCache prevents duplicate 100% events; start-of-flow guard ensures final 100% appears even for auto flows.
  Task Completion:
    CompleteTaskAsync transitions task, updates context (task_{nodeId}), removes node from active, handles parallel/join logic, may auto-advance further.
Parallel & Join Handling:
  _parallelGroups structured JSON in Context maintains branches, remaining, completed, join metadata.
  Join modes supported (all, any, count, quorum thresholdCount/thresholdPercent, expression).
  Expression evaluation executed via IConditionEvaluator (JsonLogic or domain expression).
Error Handling:
  Node failures can suspend instance or mark failed; emit both Node Failed and Instance Failed events.
Immutability / Save:
  MarkDirty + SaveIfDirtyAsync pattern ensures EF context flush control (batching operations).
Critical Guards:
  Progress dedupe
  Safety break on runaway loops
  Late task completion on completed/cancelled instances triggers automatic task cancellation event rather than exception.

8. Gateway Processing
Gateway Evaluation:
  Strategies: exclusive (default), parallel (branch fan-out), condition-driven selection (via _gatewayDecisions in context), fallback heuristics when missing decisions.
Events:
  GatewayEvaluated with strategy, outgoingEdges, selected.
Edges traversed emit EdgeTraversed events with mode (AutoAdvance, TaskCompletionAdvance, AutoAdvanceParallel).
Parallel groups create markers for join evaluation.
Missing Enhancements:
  Weighted routing, feature-flag integration, dynamic condition injection not fully implemented.

9. Progress & Context Internals
Context JSON Special Keys:
  _visited: JsonArray of node IDs visited.
  _progress: { lastPercent } store.
  _parallelGroups: grouping object with branch tracking + join metadata.
  _gatewayDecisions: optional structure for predetermined branching diagnostics.
  task_{nodeId}: per-task completion data (raw JSON or wrapped).
Progress Calculation:
  ShouldCountForProgress excludes Start; Completed instance forces all counted nodes as visited (ensures 100%).
Open Considerations:
  Potential addition of partial branch weighting; currently linear count.

10. Tags System (Backend & Frontend)
Normalization:
  normalizeTags(raw) -> canonicalQuery (comma-separated, trimmed, lowercase stable or original preserved per implementation), normalized[] list.
Filtering:
  anyTags (OR semantics), allTags (AND semantics). Legacy tags param remains (OR) for backward compatibility and can be deprecated.
Persistence:
  Definitions.Tags stored as canonical string; tag modifications through updateDefinition or createDraft.
UI:
  Definitions Page:
    Advanced Filters panel: All Tags (AND), Any Tags (OR).
    Chips preview; apply/reset with persistence via localStorage keys (wf.definitions.anyTags / wf.definitions.allTags).
    Sortable Tags column with top 3 chips + overflow indicator.
    Tag Edit Dialog with validation (max 12 tags, each ≤ 40 chars).
  Builder Page:
    Tag input integrated directly in top toolbar; validation identical; normalized on create/update.
Validation & Limits:
  Controlled solely in frontend currently; backend normalization tolerant (should implement server-side guard for defense-in-depth).
Deprecation Plan (Pending):
  Console warning & status document; precedence rule: anyTags/allTags override legacy tags.

11. Frontend Architecture Summary
State Patterns:
  Local React state + ephemeral fetch; minimal global store (no Redux).
Persistence:
  localStorage for showArchived, tag filters, (similar toggles for hideCompleted in other views).
Data Grid:
  @mui/x-data-grid-premium for definitions, with detail panel for metadata (publish notes, version notes, tags, active instance count, timings).
Builder:
  ReactFlow-based visual editor (nodes/edges editing, property panel, validation workflow, publish flow).
Services:
  workflow.service.ts handles all API interactions, unwrapping ApiResponse envelope, applying status normalization & tag normalization.
Error Handling:
  extractApiErrors centralizes picking first actionable message; used for publish/unpublish/archive/terminate/validation operations.

12. Service Layer (Frontend)
Unwrap Logic:
  Generic unwrap<T>() enforces success field gating; errors aggregated.
Definitions:
  getDefinitionsPaged (paging query param builder incl. tags), getDefinitions convenience wrapper defaults page=1 pageSize=100 sorted newest.
Instances:
  getInstancesPaged normalizes status; runtime snapshot retrieval included.
Tasks:
  getTasks/mine with status normalization; action methods (claim/assign/complete/reset/cancel).
Events/Stats:
  getWorkflowEvents, getTaskStatistics, getWorkflowStats, bulkCancelInstances.
Validation:
  validateDefinitionJson / validateDefinitionById integrate helper error mapping; GraphValidationResult computed.
SignalR (Elsewhere):
  Not shown here but referenced: startTaskHub / instance progress subscription (progress + updated events).

13. Response Envelope (Backend)
ApiResponse<T>:
  { success: bool, message?: string, errors?: array/mixed, data: T }
Paged:
  data: { items, totalCount, page, pageSize }
Frontend strips envelope and returns typed DTOs; startInstance flattened (used as result.id).

14. Real-Time & Live Updates
Current:
  Progress events push via workflow progress notifier (NotifyInstanceProgressAsync).
  Instance listing refresh triggered post-mutation; tasks listing via task notifier events.
Client Behavior:
  DataGrid refresh triggered after actions (publish/unpublish/archive/duplicate).
Enhancements (Potential):
  Active tasks count on instance progress/snapshot.
  Broadcast minimal diff for heavy payload reduction.

15. Background Workers (Current vs Pending)
Implemented in concept (partially in codebase):
  TimerWorker (NOT wired): should poll due timer tasks and auto-complete.
  OutboxDispatcher (MVP placeholder): selects unprocessed messages; actual transport integration absent.
  JoinTimeoutWorker: logging exceptions (join timeout logic not stable).
Planned:
  AnomalyDetector (progress stall alerts), EventBurstCoalescer.

16. Validation Layers
Graph Publish Validator:
  Single Start, ≥1 End, reachability, no orphan ends, no duplicate IDs.
Runtime Defensive Validation:
  Safe parse context, fallback event emission when nodes missing.
Frontend Pre-Publish:
  validateDefinitionById + local DSL validation + auto-save re-run.
Missing:
  UI-level structural validation for join configurations (parallel+join pairs).
Opportunity:
  Add offline “simulate path” tool.

17. Test Coverage Overview
Covered:
  Publish/unpublish/archive/terminate endpoints with error handling.
  Definitions paging + archived filtering.
  Instance lifecycle (start, signal, completion).
  Gateway selection (some scenarios).
  Progress finalization (100% appears).
  Task claim/complete baseline.
Partial/Skipped:
  Join timeout activation test variants (fail, force, route).
  Timer auto-fire (depends on TimerWorker).
  Outbox dispatch idempotency under concurrency.
Frontend:
  No comprehensive component test coverage; relies on manual verification and backend tests enforcing contract shape.

18. Performance & Scaling Considerations
Queries:
  Definitions list uses paging—indexes recommended: (TenantId, IsArchived, IsPublished, UpdatedAt DESC).
Concurrency:
  Runtime loops may thrash if large parallel expansions; safety counter protects runaway loops.
Progress Frequency:
  Coalescing not implemented; potential for event spam on large automatic chains (dedupe only addresses terminal duplicates).
Outbox:
  Dispatcher absent—message table can grow unbounded (needs retention policy).

19. Observability & Diagnostics
Logging:
  Structured Serilog; engine logs WF_* prefixed messages (WF_START, WF_NODE_EXEC_START, etc.).
Progress:
  Cache prevents repeated 100% emission (reduces noise).
Events:
  Rich event stream supports future timeline visualization.
Pending:
  Metrics counters (tasks per minute, average instance completion time).
  Health probe expansion (outbox lag, timer backlog).
Recommended:
  Add structured log enrichment for tenant + instance tracing.

20. Risk / Debt Inventory
High Priority:
  TimerWorker missing → timers rely on manual completion.
  Outbox dispatcher missing → no external event propagation.
  JoinTimeout logic unreliable.
Medium:
  Multi-tenant enforcement not fully tested for deep edge cases.
  Definition immutability enforcement needs explicit validation & negative tests.
  Automatic executors minimal (no robust webhook integration).
Low:
  Duplicate 100% progress issue resolved; low residual risk.
  Lack of UI tests; manual regression passes needed.

21. UI Enhancements (Recent)
Definitions Page:
  Advanced tag filtering (All=AND, Any=OR) with localStorage persistence.
  Tags column + overflow indicator, metadata detail panel expanded.
  Edit Tags dialog (validation + preview chips) for draft/published.
Builder Page:
  Inline tag input + validation (shared policy).
Progress UI:
  Terminal dedupe reduces flicker for completed instances.
Filter Persistence:
  showArchived, anyTags, allTags stored under stable localStorage keys.
Tooltips:
  Tag semantics (AND vs OR) explained in detail panel.
Chip Interaction:
  Removal updates filter state but requires explicit Apply (reduces query spam).

22. Open Items Backlog (Condensed Jira-Ready)
1. TimerWorker implementation (auto-complete due timer tasks).
2. OutboxDispatcher + idempotency key & processed auditing.
3. Join timeout test strategy (implement vs retire) + fix worker.
4. Legacy tags parameter deprecation (precedence documentation + console warning).
5. Multi-tenant enforcement test suite (negative leakage tests).
6. Definition immutability hardening (guard PUT after publish).
7. ActiveTasksCount in InstanceUpdated payload & UI column/badge.
8. Grid personalization (columns, sort, page size, density persistence).
9. Standardize error helper across remaining admin / events endpoints.
10. Progress rate metric & anomaly detection (stalled flows).
11. Event burst coalescing (batch or debounce progress/task events).
12. Automatic action registry extensions (webhook executor).
13. Role usage reporting (aggregate role distribution across defs).
14. Structured error classification (401/403/409/422 friendly surfacing).
15. Test coverage: timer auto-fire, outbox idempotency, parallel/join edge cases.
16. Builder simulation / dry-run harness.
17. Deprecation banner for legacy tags.
18. Performance baseline & regression check for filtered definition queries.
19. Reusable TagInput component (DRY across builder + dialogs).
20. Instance history timeline UI using stored WorkflowEvents.

23. API Surface (Key Endpoints Recap)
Definitions:
  GET /api/workflow/definitions (paged; filters: search, published, includeArchived, tags, anyTags, allTags, sortBy, desc, page, pageSize)
  GET /api/workflow/definitions/{id}
  POST /api/workflow/definitions/draft
  PUT /api/workflow/definitions/{id}
  POST /api/workflow/definitions/{id}/publish
  POST /api/workflow/definitions/{id}/unpublish
  POST /api/workflow/definitions/{id}/archive
  POST /api/workflow/definitions/{id}/new-version
  POST /api/workflow/definitions/{id}/terminate-running
  POST /api/workflow/definitions/{id}/revalidate
Instances:
  GET /api/workflow/instances (+ filters)
  GET /api/workflow/instances/{id}
  GET /api/workflow/instances/{id}/status
  GET /api/workflow/instances/{id}/runtime-snapshot
  POST /api/workflow/instances
  POST /api/workflow/instances/{id}/signal
  POST /api/workflow/instances/{id}/suspend
  POST /api/workflow/instances/{id}/resume
  DELETE /api/workflow/instances/{id}
Tasks:
  GET /api/workflow/tasks
  GET /api/workflow/tasks/mine
  GET /api/workflow/tasks/{id}
  POST /api/workflow/tasks/{id}/claim
  POST /api/workflow/tasks/{id}/complete
  POST /api/workflow/tasks/{id}/assign
  POST /api/workflow/tasks/{id}/cancel
  POST /api/workflow/admin/tasks/{id}/reset
Admin / Stats / Events:
  GET /api/workflow/tasks/statistics
  POST /api/workflow/admin/instances/bulk-cancel
  GET /api/workflow/admin/events
Health:
  GET /health

24. DTO / Envelope Conventions
PagedResultDto<T> { items: T[], totalCount, page, pageSize }
WorkflowDefinitionDto includes tags (string | null).
WorkflowInstanceDto includes normalized status (InstanceStatus).
WorkflowTaskDto includes status normalization & completion metadata.
GraphValidationResult (frontend abstraction) merges backend envelope variants.

25. Builder / Editor Type Summary
EditorWorkflowDefinition { nodes, edges, metadata? }
EditorWorkflowNode { id, type, label?, x, y, roles?, action?, condition?, metadata }
EditorWorkflowEdge { id, from, to, label?, fromHandle? }
Mapping ensures gateway boolean edges consistent (true/false).
Potential future metadata expansions: SLA, form schema, data-binding.

26. Runtime Internals (Selected Methods)
StartWorkflowAsync:
  Creates instance, marks start node visited, triggers continuation loop, emits initial or force terminal progress.
ContinueWorkflowAsync:
  Pulls active nodes; executes each unless waiting task present; attempts completion if active set drained.
CompleteTaskAsync:
  Status transition, context injection, active set mutation, emits parallel/join events, may recursively continue.
ComputeAndMaybeEmitProgressAsync:
  Derives visitedCount vs total countable nodes; persists lastPercent; dedupes duplicate 100%.
TryCompleteInstanceAsync:
  Cancels open tasks, sets Completed, emits final events, forces progress recalculation.

27. Join / Parallel Mechanics
Parallel:
  Gateway with parallel strategy populates _parallelGroups[gatewayNodeId].branches.
Join:
  Arrival events tracked in joinMeta (“arrivals” array).
  Mode strategies: all, any, count, quorum(percent/count), expression (custom expression evaluation with overlay _joinEval).
Cancellation:
  cancelRemaining supports aggressively pruning remaining branches when join satisfied.

28. Error Handling & Resilience
Execution errors:
  Node failure paths can suspend or fail instance; events persisted.
Validation failures:
  Graph validation returns actionable error list; publish blocked until resolved.
Late Task Completion:
  On already completed/cancelled instance → task auto-cancel + event note.
Silent / Suppressed Exceptions:
  Some progress emission & notifier exceptions swallowed to avoid halting engine (trade-off: diagnostic visibility).

29. Frontend UX Patterns
Explicit Apply for advanced filters to avoid excessive network calls.
Chip-based removal workflow (mutates input, requires reapply).
Detail panel shows extended metadata (publish notes, version notes, tags, timestamps).
Dialogs for destructive / lifecycle operations (Publish, Unpublish, Archive, Terminate Instances, Edit Tags).
Normalization performed just-in-time before persistence (tags, statuses).
Consistent toast feedback patterns (success/failure contextual messaging).

30. Local Storage Keys (Current)
wf.definitions.showArchived
wf.definitions.anyTags
wf.definitions.allTags
(Elsewhere probable: wf.instances.hideCompleted, wf.tasks.hideCompleted)

31. Performance Considerations
Hot Paths:
  ContinueWorkflowAsync loops; may benefit from micro-batching events.
  Progress emission frequency could be reduced by only emitting on percent change (already partially enforced).
Indexes (Recommended):
  WorkflowDefinitions: (TenantId, IsArchived, IsPublished, UpdatedAt DESC)
  WorkflowInstances: (TenantId, Status, StartedAt DESC)
  WorkflowTasks: (WorkflowInstanceId, Status, DueDate)
Potential:
  Event table growth; add archival or partitioning strategy.

32. Observability Backlog
Planned Metrics:
  InstancesStarted/sec, TasksCompleted/sec, AverageDuration, TimerLag, OutboxLag.
Event Classification:
  Consider severity tagging (info / warn / error) for future UI timeline filtering.

33. Security / Guarding
Immutability:
  PUT after publish should reject (needs full enforcement + tests).
Tenant Leaks:
  Validate cross-reference queries (joins on definitions/instances/tasks incorporate TenantId).
Role Enforcement:
  Admin endpoints strictly require workflow.admin permission (confirmed).
Data Sanitization:
  Task completion data stored raw (if JSON parse fails) → consider size limit / sanitization.

34. Known Gaps / Decisions Needed
TimerWorker: implement or clarify manual policy.
Legacy tags param: timeline for removal.
Join timeout semantics: either finalize or disable feature for MVP.
Outbox externalization: choose transport (e.g., RabbitMQ / Kafka) or continue logging fallback.
Simulation / test harness: needed for complex gateways / joins reliability.

35. Immediate Recommendations
1. Implement TimerWorker to remove manual timer task friction.
2. Add OutboxDispatcher (idempotent lock, batch process, backoff).
3. Multi-tenant negative tests to certify isolation.
4. Deprecate legacy tags param (warning + documentation).
5. Add minimal metrics hook (avoid scope creep but seed observability).
6. Confirm definition immutability enforcement with regression test.

36. Summary Snapshot
Stable core (definitions, instances, task lifecycle, gateway routing, progress tracking). Significant UX advances (tags system & filtering, metadata surfacing). Operational readiness partially hindered by absent TimerWorker & Outbox dispatch. Testing resilient for core paths but lacking for timed / join/timeouts and externalization scenarios. System is feature-complete for foundational workflow orchestration; next focus: automation maturity (timers/outbox), validation depth, and operational observability.

37. Glossary
ANY Tags: OR filtering (match at least one).
ALL Tags: AND filtering (must contain all).
Active Set: CurrentNodeIds list deserialized for engine iteration.
Visited Nodes: _visited context array marking execution history.
Parallel Group: Structure under _parallelGroups tracking branches & join state.
Terminal Progress: 100% event signifying all countable nodes visited or instance completion.

38. Appendices (Potential Future Sections)
A. Migration Strategy for Definition Versioning
B. Webhook Automatic Action Execution Design
C. Role Usage Analytical Queries
D. Event Timeline UI Specification (future)

39. Monaco & Editor Optimization (Recent Delta)
 - Loader Refactor: ensureMonaco() service (single-flight) + idle prefetch (reason: 'idle' | 'heuristic').
 - Deferred Load: Optional focus-triggered initialization reducing initial bundle execution cost.
 - Slim Build: Switched to 'monaco-editor/esm/vs/editor/editor.api' with only json & editor workers; removed bulk language contributions.
 - JSON Language Service: Dynamically imported; potential future “lite mode” to drop schema validation worker for further size savings.
 - Telemetry Added: monaco.load.start / complete / failed (cold + slim flag), monaco.prefetch.trigger, monaco.reload.attempt / success / failed, monaco.concurrent, monaco.models.cleaned, monaco.local.parse.ms (bucketed performance), monaco.slim.enabled.
 - Local Parse Optimization: 150ms debounce + duplicate suppression for JSON.parse; semantic validation triggered only after successful parse.
 - Memory Hygiene: Model cleanup after threshold; beforeunload cleanup invocation for long sessions.
 - Theme Optimization: applyThemeIfChanged prevents redundant theme reapplication.
 - Bundle Guard: Script-based (check-bundle-size.cjs) baseline enforcement for main/entry chunk growth regression.

40. Simulation Capability (Dry-Run PR1–PR2)
 - Purpose: Enumerate terminal execution paths to assist authors/operators in understanding branch explosion and join behavior before runtime.
 - Implementation: Depth-first traversal with caps (maxPaths, maxDepth, maxVisitsPerNode). Conditional gateways evaluate JsonLogic; parallel gateways either linearized or exploded.
 - PR2 Enhancements: Parallel branch fragment exploration with naive join merge detection and capped cartesian product (maxBranchCartesian). Partial convergence flagged (partialParallelMerge).
 - Telemetry: simulation.run.start / simulation.run.complete (pathCount, truncated, maxLength, cartesianCapped), simulation.truncated (reasons[]), simulation.path.select, simulation.drawer.open/close.
 - UI: SimulationDrawer (context JSON input, config controls, path list with metadata, highlight selected path on DefinitionDiagram / Instance diagram).
 - Truncation Reasons: 'maxDepth', 'visitCap', 'maxPaths', 'cartesianCap' (deduplicated).
 - Limitations (Next PR Targets): Join mode semantics (all/any/count/quorum/expression), edge-level condition evaluation, probability weighting, performance caching per context, ghost path comparison vs historical runs.

41. Version Diff Visual Overlay
 - Overlay Highlights: Added nodes (green glow), modified nodes (amber border). Removed nodes surfaced via legend counts (ghost rendering deferred).
 - Contexts: Definitions diff drawer & InstanceDetails runtime diagram (diff vs previous version).
 - Toggle: diff.viewer.overlay.toggle event with payload { enabled, context, added, modified, removed }.
 - Future: Ghost removed nodes (semi-transparent red), edge diff styling (added/removed/changed), arbitrary version compare (vN ↔ vM), overlay accessibility (aria-live change summaries).

42. Extended Telemetry Inventory (Additions)
 - Diff: diff.viewer.overlay.toggle, (planned: diff.viewer.modified.field).
 - Simulation: simulation.run.start, simulation.run.complete, simulation.truncated, simulation.path.select, simulation.drawer.open/close.
 - Editor: monaco.local.parse.ms (bucket, ms, error flag), monaco.defer.used, monaco.reload.*, monaco.prefetch.trigger (reason), monaco.slim.enabled.

43. Upcoming Focus Items (Planned PRs)
 1. Simulation PR3: Join-mode semantics (respect count/quorum/expression thresholds) + branch pruning after satisfied join; integrate join path annotations.
 2. Diff Overlay PR3b/PR4: Ghost removed nodes + edge change visualization; arbitrary version picker.
 3. Monaco Optimization PR4: Optional JS/YAML language on-demand + dynamic exceljs import + lite JSON mode flag.
 4. ActiveTasksCount Fallback: UI badge with lazy backend enrichment; potential hub event extension.
 5. Outbox & Timer Operationalization: Worker enablement + observability (lag metrics, retries).

44. Risk / Mitigation Updates (Additive)
 - Simulation Fidelity Gap: Without join semantics, some enumerated paths may over-approximate; mitigation: PR3 planned with mode-aware branching and pruning.
 - Bundle Drift Risk: Future feature additions may erode slim gains; mitigation: enforced bundle-size baseline + telemetry on load durations.
 - Visual Diff Accessibility: Overlay reliant on color; mitigation: planned aria summaries + pattern/dashed outlines.

45. Developer Quick Reference (New Capabilities)
 - Run Simulation: Open instance details → Simulate → Adjust caps → Select path to highlight.
 - Enable Diff Overlay: Open definition diff drawer or toggle on instance runtime diagram when previous version exists.
 - Measure Editor Parse Performance: Inspect telemetry stream for monaco.local.parse.ms buckets (<1,<2,<5,<10,<25,<50,50+).
 - Enforce Bundle Guard: npm run build:ci (fails if entry chunk growth > baseline + delta).

---
For a condensed executive summary: request “Provide condensed context”.
For delta from previous snapshot: request “Provide delta summary”.
