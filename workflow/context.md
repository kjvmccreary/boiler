Application Context Document (Current State Snapshot – Updated Phase 5)

1. High-Level Overview
Multi-service, multi-tenant workflow platform (.NET 9 / C# 13) centered on WorkflowService (definitions, instances, tasks, events, publish lifecycle). Frontend (React + MUI + DataGrid Premium + emerging ReactFlow builder) consumes a consistent envelope (ApiResponseDto<T>) with normalization helpers. Recent focus: definition pagination, archived filtering, live instance/task updates via SignalR, UI toggles with local persistence, contract stabilization, and test alignment.

2. Tech Stack
* Runtime: .NET 9, C# 13
* Data: PostgreSQL (EF Core 9) + InMemory for unit/integration tests
* Messaging: Outbox pattern (relational table, deterministic idempotency keys) – RabbitMQ not yet integrated
* Auth: JWT + permission claims (string-based “permission” claim)
* Frontend: React 18, Vite, TypeScript, MUI, @mui/x-data-grid-premium, SignalR client
* Graph builder: Planned / partial (EditorWorkflowDefinition types + mapping utilities)
* Logging: Serilog
* Real-time: SignalR hubs (tasks & instances)
* Background workers: Outbox dispatcher, Backfill, TimerWorker, JoinTimeoutWorker (pending fix), future event optimizations

3. Projects / Assemblies
* WorkflowService (core workflow API & engine)
* DTOs (transport contracts)
* Contracts (shared service/repository interfaces)
* Common (cross-cutting: tenancy, auditing, deterministic hashing)
* ApiGateway (reverse proxy / discovery middleware)
* Frontend (react-app)
* Tests: unit + integration (WorkflowService.Tests), coverage includes controllers/services

4. Domain Model (Key Entities)
WorkflowDefinition:
* Immutable after publish (JSONDefinition locked)
* Version chain via ParentDefinitionId
* Fields: IsPublished, PublishedAt, PublishNotes, VersionNotes, IsArchived, ArchivedAt, Tags, ActiveInstanceCount
WorkflowInstance:
* DefinitionVersion, Status (Running, Suspended, Completed, Cancelled, Failed)
* CurrentNodeIds (string or JSON), Context (JSON), StartedAt, CompletedAt
WorkflowTask:
* Human or automatic node tasks; Status transitions (Created → Assigned → Claimed → InProgress → Completed/Cancelled/Failed)
* Role/user assignment; optional DueDate
WorkflowEvent:
* Timeline trace (OccurredAt, EventType, Name, Data)
OutboxMessage:
* Deterministic hash key (TenantId + logical payload), RetryCount, ProcessedAt, Error state

5. Security & Tenancy
* Tenant resolution via ITenantProvider (header X-Tenant-ID preferred; claim fallback)
* Permissions constants normalized (Permissions.Workflow.*)
* Role/permission enforcement integrated with controllers ([Authorize]/custom attributes)
* Multi-tenant isolation enforced at service layer; some queries manually constrained (explicit Where on TenantId)

6. Workflow Engine Architecture
* Runtime orchestrator + node executors (Start/End, HumanTask, Automatic, Gateway, Timer, Join)
* Gateway strategies (Feature flag, A/B) with pluggable registry (currently minimal/no external provider wiring)
* Validation layers:
  - GraphValidationService (structure)
  - Publish-time validator (graph rules, JSON parse via BuilderDefinitionAdapter)
* Extensible automatic action executors (Noop + placeholder webhook pattern)
* Diagnostics scaffolding (enrichment of gateway edges, conditional logging)

7. Services Layer (Selected Behavior)
DefinitionService:
* Draft lifecycle, publish (immutability enforcement), version creation, usage stats, includeArchived support
* Paging implemented (PagedResultDto)
InstanceService (implied / partial): start, signal, status, runtime snapshot, suspend/resume, terminate
TaskService: claim, assign, complete, reset (admin), cancel
RoleService: tenant-bound role management, usage lookup (role-in-workflows analysis placeholders)
Outbox services: deterministic production + dispatch loop

8. Outbox Pattern
* Deterministic hashing to avoid duplicates
* Unique constraint on (TenantId, IdempotencyKey)
* Retry / deadletter escalation controlled by simple retry increments
* Background dispatcher & optional backfill worker
* Health checks propagate latency/deadletter concerns (future dashboard candidate)

9. Recent / Notable Hardening & Changes
* Unified paged envelope for definitions (replacing legacy flat list)
* Added includeArchived filtering end-to-end + toggle in UI
* LocalStorage persistence for toggles: Hide Completed (instances/tasks), Show Archived (definitions)
* Instance finalization validated (completion status correctness)
* Type system stabilization (workflow.ts with builder/editor/usage types + optional snapshot fields)
* Controller tests updated to paged contract; removed legacy list-based assumptions

10. Backend ↔ Frontend Contract Adjustments (Current State)
* Envelope: ApiResponseDto<T> consistent for workflow endpoints
* Definitions GET: ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>> (frontend unwraps data.items)
* includeArchived param implemented (default false)
* StartInstance returns ApiResponseDto<WorkflowInstanceDto>; frontend uses data.id
* Snapshot includes tasks, events, visited/traversed metadata (some optional)
* Builder/editor mapping utilities expect EditorWorkflowDefinition ←→ Graph nodes/edges

11. Background Worker Status
JoinTimeoutWorker:
* Still logging nullable access exceptions (root cause unresolved)
* Temporarily tolerated (does not block execution path)
TimerWorker & Outbox workers operational
Optimization opportunity: progress event deduplication (see Section 17)

12. Instance Completion Status (Resolved)
* Verified transitions produce:
  - Status=Completed
  - CompletedAt populated
  - CurrentNodeIds empty
  - Terminal progress ≥ one 100% event
* Frontend now reflects completion rapidly via SignalR push, minimal polling fallback

13. Permissions
* Controllers standardized on Permissions.Workflow.* constants
* Legacy literal permission strings replaced (reduces mismatch risk)
* Claims: “permission” claim holds values—tokens must include needed workflow permissions

14. Dependency Injection & Lifetime Overview
Registered (not exhaustive):
* IDefinitionService / IInstanceService / ITaskService (scoped)
* Validation: IWorkflowGraphValidator, IGraphValidationService, IWorkflowPublishValidator
* Runtime executors: multiple INodeExecutor
* Feature/gateway registries
* Outbox: writer, dispatcher, backfill
* SignalR hubs + notifications dispatch
* Tenant services (ITenantProvider), auditing, hashing
Potential enhancement: split builder-specific services from runtime concerns to tighten hosting footprint

15. Configuration / Environment Variables
* ConnectionStrings:DefaultConnection
* X-Tenant-ID header required (missing → 401/tenant error path)
* WF_ISO_DIAG=1 enables isolation diagnostics
* Workflow:Outbox / Workflow:JoinTimeouts section options
* Optional Docker flags (NO_CACHE_DOCKER etc.)

16. Testing Overview
* Definitions controller tests aligned to paged envelope
* Archived filtering integration tests (includeArchived true/false) passing
* Publish path tests include validation error shaping
* Some tests skipped (join timeout scenarios)
* Frontend tests minimal (task/definition service behavior mostly untested client-side)
* Type safety improved; builder utilities compile with unified types

17. Real-Time & Eventing
SignalR:
* Task events push list refresh
* InstanceUpdated & InstanceProgress events update live status/progress badges
Current Issue:
* Multiple (duplicate) Progress(100%) terminal events (low impact) → potential guard to emit only first terminal event
Potential Enhancements:
* Delta-only payloads for large volume workflows
* Batch compression for high-frequency automatic nodes

18. Diagnostics / Observability
* Outbox logging (FETCH / DISPATCH / DEADLETTER)
* Instance / task mutation logs via service layers
* Health endpoint includes tenant & outbox indicators
* Performance measurements not yet centralized (no metrics aggregator)
* Consider: structured event classification for timeline UI

19. API Summary (Key Endpoints)
Definitions:
  GET /api/workflow/definitions?includeArchived=bool&search=&published=&tags=&sortBy=&desc=&page=&pageSize=
  GET /api/workflow/definitions/{id}
  POST /api/workflow/definitions/draft
  PUT /api/workflow/definitions/{id}
  POST /api/workflow/definitions/{id}/publish
  POST /api/workflow/definitions/{id}/unpublish
  POST /api/workflow/definitions/{id}/new-version
  POST /api/workflow/definitions/{id}/archive
  POST /api/workflow/definitions/{id}/terminate-running
  POST /api/workflow/definitions/{id}/revalidate
  GET  /api/workflow/definitions/{id}/usage
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
Tasks (selection):
  GET /api/workflow/tasks
  GET /api/workflow/tasks/mine
  GET /api/workflow/tasks/{id}
  POST /api/workflow/tasks/{id}/claim
  POST /api/workflow/tasks/{id}/complete
  POST /api/workflow/tasks/{id}/assign
  POST /api/workflow/tasks/{id}/cancel
  POST /api/workflow/admin/tasks/{id}/reset
Admin / Stats:
  GET /api/workflow/tasks/statistics
  POST /api/workflow/admin/instances/bulk-cancel
Health:
  GET /health

20. Response Envelope (Canonical)
ApiResponseDto<T>:
{
  success: bool,
  message?: string,
  errors?: [{ code?: string, message?: string }],
  data?: T
}
Paged variant: data: { items: TItem[], totalCount, page, pageSize }

21. Frontend Integration State
Implemented:
* Definitions paged fetch + archived toggle
* Instances & My Tasks hide-completed toggles (persisted)
* Instance runtime diagram & state badges
* Task status chips + claim/complete flows
Pending / Enhancements:
* Display additional definition metadata (PublishNotes, VersionNotes, Tags, ActiveInstanceCount)
* Progress event deduplication (UI stabilization)
* Formal service method audit doc for unwrap consistency
* Central error classification (401 vs 403 vs 409 vs 422 mapping)

22. Open Follow-Ups
1. Final audit: all startInstance usages rely on data.id (no stale patterns)
2. Service unwrapping matrix (document each method’s expectation)
3. Optional: expose metadata fields in definition detail or expanded row
4. Implement terminal progress event dedupe (cache lastPercent per instance)
5. Decide fate of skipped join-timeout tests (reactivate or deprecate with note)
6. Role usage reporting endpoint completion (if UI to display role impact)
7. Optional: add activeTasksCount to InstanceUpdated events

23. Feature Flags / Extension Points
* Gateway strategies pluggable (feature flag / A-B)
* Automatic action registry (future: webhook, script, integration)
* Publish validation extensible
* Event stream shaping (future: classification, severity)

24. Risk & Debt Areas
* Duplicate terminal progress events (noise)
* JoinTimeoutWorker exceptions – potential hidden missed transitions
* Unexposed metadata fields reduce admin visibility (PublishNotes/VersionNotes)
* Sparse frontend automated test coverage
* Builder/editor type sprawl (all in single workflow.ts) – consider modularization

25. Quick Action Checklist
| Area | Action |
|------|--------|
| Service unwrap audit | Produce method matrix & confirm conformity |
| StartInstance usage | Grep & verify all use response.data.id |
| Definition metadata UI | Decide: add columns or details panel |
| Progress dedupe | Implement single terminal emission |
| Join timeout tests | Reactivate or retire with rationale |
| Role usage endpoints | Complete server implementation if needed by UI |
| Frontend error handling | Standardize toast messaging by HTTP status |

26. Environment / Headers Required
* Authorization: Bearer {JWT}
* X-Tenant-ID: {tenantId}
Missing / invalid tenant context → 401 or controlled error envelope. Ensure consistent 401 vs 403 semantics (permission vs absence).

27. Suggested Immediate Next Steps
1. Produce and store “service unwrap audit” markdown (ensures future regressions are obvious).
2. Add optional definition metadata to a flyout or expandable grid row.
3. Implement simple progress dedupe (cache last emitted non-terminal + block duplicate 100%).
4. Decide on re-enabling join timeout logic (fix nullable root cause first).
5. Add minimal Jest or React Testing Library coverage for workflowService error paths.

28. Builder / Editor Model (Current State)
* Types: EditorWorkflowDefinition / EditorWorkflowNode / EditorWorkflowEdge + mapping utilities (definitionMapper.ts)
* Conversion ensures gateway branches normalized (true/false/else)
* RFNodeData/RFEdgeData types relaxed (optional fields for resilience)
* Future: validation + palette integration pending

29. Real-Time UX Integration
* useTaskHub & useInstanceHub hooks throttle refresh
* Status badge polls only if push missing (avoids noisy network)
* Potential optimization: disable polling after first push per instance

30. Documentation / Developer Experience
* Swagger XML comments recommended for key endpoints (definitions GetAll documented pattern prepared)
* status.md maintains delta-focused contract; context.md (this file) is full snapshot
* README updates pending for includeArchived usage & paged envelope adoption

31. Testing Gaps / Opportunities
* No direct test for publish with tags/version notes interplay
* Missing integration covering instance suspend → resume → completion chain
* Lack of load tests for high-frequency automatic node scenarios
* No test validating outbox idempotency under concurrent publish

32. Performance Considerations
* Definition queries now paged – indexing advisable: (TenantId, IsArchived, IsPublished, UpdatedAt DESC)
* Outbox dispatcher linear scan – consider batching or time-slice if volume grows
* Progress events can be coalesced (client only needs latest for each instance)

33. Observability Enhancements (Proposed)
* Add metrics counters: active instances, task throughput, publish latency
* Track event emission counts (definition.instance.updated vs progress)
* Central structured logging for permission denial

34. Future Roadmap Seeds (From Backlog)
* Task SLA / aging dashboard (needs activeTasksCount / dueSoon push)
* Definition diff view for version comparisons
* Human task form schema rendering (currently only stored)
* Workflow simulation / dry-run harness
* Webhook automatic action executor full implementation

35. Summary State Snapshot
Core engine stable, definitions/instances lifecycle functional, archived filtering and pagination implemented, live updates in place. Remaining items are polish (metadata exposure, event dedupe, worker stabilization) and observability/test depth improvements.

36. Other
* User / Tenant relationship is many to many. A user can belong to multiple tenants with different roles/permissions in each.
* You (AI engine) are to act as a senior developer familiar with this codebase. 
    * You are to provide full code files rather than snippets.
    * Do not expect me to provide missing context; use best judgment.
    * You are responsible for ensuring accuracy and relevance of your responses.
    * All of your responses must take into account the rest of the code base and the overall architecture.
---
If you need a condensed “executive brief” version or a change-log delta from prior snapshot, request: “Provide condensed context” or “Provide delta summary.”
