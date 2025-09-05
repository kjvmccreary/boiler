Application Context Document (Current State Snapshot)
1. High-Level Overview

1. Multi-service solution
(.NET 9 / C# 13) with a dedicated WorkflowService implementing a multi-tenant workflow engine (definitions, instances, tasks, events) plus RBAC integration shared with Auth/User services. Backend hardened recently: stricter tenant isolation, immutability enforcement, deterministic outbox/event idempotency, refined DI registration, and service-layer abstraction.

2. Tech Stack
* Runtime: .NET 9, C# 13
* Data: PostgreSQL (EF Core 9), some tests use InMemory provider
* Messaging pattern: Outbox (table + dispatcher, future RabbitMQ)
* Auth: JWT + permission claims (string-based)
* Frontend: React (MUI DataGrid Premium), ReactFlow planned for builder
* Logging: Serilog
* Background workers: OutboxBackgroundWorker, OutboxIdempotencyBackfillWorker, TimerWorker, JoinTimeoutWorker (currently throwing due to nullable access), JoinTimeoutWorker, Task notifications via SignalR
3. Projects / Key Assemblies
* WorkflowService (primary microservice)
* Common (shared infra: TenantProvider, AuditService, hashing, repositories)
* Contracts (interfaces for repositories/services)
* DTOs (transport types)
* ApiGateway (service discovery, reverse proxy behavior)
* Frontend (React app)
4. Domain Model (Core Entities)
WorkflowDefinition:
* Immutable once published (JSONDefinition locked)
* Versioned (Version int, ParentDefinitionId chain)
* Fields: IsPublished, PublishedAt, PublishNotes, IsArchived, ArchivedAt, Tags, VersionNotes WorkflowInstance:
* Tracks DefinitionVersion + Status (Running | Suspended | Completed | Cancelled | etc.)
* CurrentNodeIds (string JSON or CSV), Context (JSON), StartedAt, CompletedAt WorkflowTask:
* Human/automatic tasks per node, AssignedTo role/user, Status & optional DueDate WorkflowEvent:
* Type, Name, Data (JSON), OccurredAt (canonical execution trace) OutboxMessage:
* EventType, EventData, TenantId, IdempotencyKey (deterministic), ProcessedAt, RetryCount, Error
5. Security & Tenancy
* Tenant scoping via ITenantProvider (resolves from header X-Tenant-ID or claims)
* Global query filters (in some repositories/entities)
* RBAC: Role, Permission, UserRole; RoleService enforces tenant and system role boundaries
* Permissions used by workflow endpoints are a mix of constants (Permissions.Workflow.*) and literal strings (e.g. "workflow.view_instances") → normalization recommended
6. Workflow Engine Architecture
* Execution runtime: IWorkflowRuntime + node executors (StartEndExecutor, HumanTaskExecutor, AutomaticExecutor, GatewayEvaluator, TimerExecutor, JoinExecutor)
* Gateways: FeatureFlagGatewayStrategy, AbTestGatewayStrategy (pluggable via registry)
* Validation:
* GraphValidationService (structural, JSON DSL parsing via BuilderDefinitionAdapter)
* JsonLogic conditions for transitions
* Publish-time validator (Noop currently if none supplied)
* Automatic actions & diagnostics: AutomaticActionRegistry, NoopAutomaticActionExecutor, optional diagnostic ring buffer (AutomaticDiagnosticsBuffer)
7. Services Layer (Selected)
DefinitionService:
* Draft create/update with draft validation
* Publish: strict validation, immutability enforcement, publish events
* Unpublish: optional force terminate & events
* New version creation, usage stats, active instance counts (refactored to avoid EF composite Contains translation issue) InstanceService (registered; not all code shown):
* Start, Signal, Suspend, Resume, Terminate, Status retrieval, History TaskService (registered):
* Human task lifecycle (complete, claim, etc.) RoleService:
* Hardened for tenant boundaries, system role protection, concurrency mitigations, deterministic logging + audits
8. Outbox Pattern
* Deterministic keys via DeterministicHasher / DeterministicOutboxKey (Instance/Task/Definition events)
* Dispatcher polls unprocessed messages, sets ProcessedAt or increments RetryCount
* Idempotency enforced by unique (TenantId, IdempotencyKey)
* Health check registered (outbox)
9. Recent Hardening Changes
* Added missing DI registrations (definitions/instances/tasks/services + validators)
* Replaced unsupported EF composite anonymous Contains query in DefinitionService.GetAllAsync with a 2-step approach (IDs + in-memory grouping)
* Normalized creation/update immutability logic
* Added isolation diagnostics toggled by WF_ISO_DIAG=1
* Improved audit + security logging in RoleService
* Added defensive mapping for DefinitionService gateway edge enrichment
* Added no-cache Docker build capability in start script
10. Frontend ↔ Backend Contract Adjustments (Completed / Pending)
Resolved / Identified:
* Definitions list now returns ApiResponseDto<List<WorkflowDefinitionDto>> (controller unwraps Items)
* Frontend previously assumed plain array; must use response.data.data
* Start instance: frontend used response.id but response is ApiResponseDto<WorkflowInstanceDto> → must use response.data.id
* Archived definitions currently NOT filtered in backend; UI may need to filter or add query param Pending:
* Instance completion mismatch (see Section 12)
* Permissions naming inconsistencies may cause silent 403s
* Potential need to expose pagination metadata instead of flattening Items
11. Known Background Worker Issue
JoinTimeoutWorker repeatedly logs: System.InvalidOperationException: Nullable object must have a value Likely due to a nullable field accessed (e.g., a Join timeout column or CompletedAt). Temporary mitigations:
* Comment out AddHostedService<JoinTimeoutWorker>()
* Add null guards inside ScanAsync Root cause not yet patched.
12. Instance Completion Status Issue (Open)
Observed Symptom:
* Task grid shows last task completed
* Instance detail still shows Running Likely Causes:
* HumanTaskExecutor or TaskService.CompleteTaskAsync not marking instance Status=Completed when no remaining active tasks and End node reached
* Missing call to event publisher (instance.completed)
* Frontend not refetching instance after task completion (stale cache) Next Required Inputs (not yet reviewed):
* TasksController completion endpoint (file open at line ~151)
* HumanTaskExecutor.cs
* DB row dump after final task (Status, CompletedAt) Pending Patch:
* Add instance finalization logic + API contract confirmation
13. Permissions (Observed)
Used constants: Permissions.Workflow.ViewDefinitions, .CreateDefinitions, .EditDefinitions, .PublishDefinitions, .ManageInstances Used literals: "workflow.view_instances", "workflow.start_instances", "workflow.manage_instances" Recommendation:
* Single source (constants) or consistent naming; ensure token claim "permission" includes new forms.
14. DI Registrations (Current Core)
Scoped / Singleton of note:
* IDefinitionService, IInstanceService, ITaskService
* IUserContext, IUnitOfWork
* IWorkflowRuntime, IConditionEvaluator
* IGraphValidationService, IWorkflowGraphValidator, IWorkflowPublishValidator (may be Noop)
* Node executors (multiple INodeExecutor registrations)
* Outbox services (writer, dispatcher, transport, metrics)
* Background workers (TimerWorker, JoinTimeoutWorker, Outbox)
* SignalR hub (TaskNotificationsHub) + dispatcher
15. Configuration & Environment Variables
* Connection string: DefaultConnection
* Tenancy: Tenancy section bound to TenantSettings
* Workflow:Outbox, Workflow:Outbox:Backfill, Workflow:JoinTimeouts for options
* WF_ISO_DIAG=1 → extra ISO:* logs
* NO_CACHE_DOCKER=1 or script -NoCache for rebuild
* X-Tenant-ID header expected; missing leads to "Tenant context required"
16. Testing
* Unit tests green (including Controllers: DefinitionsControllerTests, InstancesControllerTests, RoleServiceTests)
* DefinitionService tests rely on in-memory provider (in-memory semantics differ from PostgreSQL; translation issues hidden before refactor)
* Coverage for frontend workflow definitions file currently 0% (Istanbul report)
* Isolation tests rely on deterministic idempotency keys
17. Docker & Startup
* start-full-stack.ps1 now supports:
* -NoCache (compose build --no-cache)
* -Recreate (compose down && up)
* Health wait loop (expects 7 healthy services)
* DataProtection keys warning expected (ephemeral volume)
18. Diagnostics / Observability
Logs of interest:
* OUTBOX_WORKER_FETCH / DISPATCH / DEADLETTER
* TIMER_WORKER_NO_DUE
* UNPUBLISH_FORCE_TERMINATE, ISO:* (when WF_ISO_DIAG=1)
* JoinTimeoutWorker scan failure (open issue) Health endpoints:
* /health (includes outbox check) Hubs:
* /hubs/tasks (SignalR)
19. API Summary (Current Key Endpoints)
Definitions:
* GET /api/workflow/definitions
* GET /api/workflow/definitions/{id}
* POST /api/workflow/definitions/draft
* PUT /api/workflow/definitions/{id}
* POST /api/workflow/definitions/{id}/publish
* POST /api/workflow/definitions/{id}/unpublish
* POST /api/workflow/definitions/{id}/new-version
* POST /api/workflow/definitions/{id}/archive
* POST /api/workflow/definitions/{id}/terminate-running
* GET /api/workflow/definitions/{id}/usage
* POST /api/workflow/definitions/{id}/revalidate Instances:
* GET /api/workflow/instances
* GET /api/workflow/instances/{id}
* GET /api/workflow/instances/{id}/status
* GET /api/workflow/instances/{id}/history
* GET /api/workflow/instances/{id}/runtime-snapshot
* POST /api/workflow/instances (start)
* POST /api/workflow/instances/{id}/signal
* POST /api/workflow/instances/{id}/suspend
* POST /api/workflow/instances/{id}/resume
* DELETE /api/workflow/instances/{id} (terminate) Tasks (not fully listed here; includes claim/complete/mine endpoints) Health:
* /health
20. Response Envelope
All workflow endpoints standardize on: ApiResponseDto<T>: { success: bool, message?: string, data?: T, errors?: [{ code, message }] }
Frontend must consistently unwrap data.
21. Known Frontend Adjustments Still Needed
* Update workflowService methods to unwrap ApiResponseDto and use .data
* Handle archived definitions (filter or annotate)
* Fix navigation after startInstance (use returned data.id)
* Ensure instance detail page refetches after task completion or subscribe to notifications
* Add error classification: 401 (tenant), 403 (permission), 409 (immutability), 422 (validation optional), fallback 500
22. Open Follow-Ups
1.	Provide HumanTaskExecutor & TasksController completion code → finalize completion logic patch.
2.	Fix JoinTimeoutWorker null handling.
3.	Normalize permissions.
4.	Optional: implement pagination metadata in definitions response.
5.	Implement role usage check integration (currently a stub in RoleService.CheckRoleUsageInWorkflowsAsync).
6.	Add integration tests for final task → instance completion → outbox instance.completed event.
23. Flags / Feature Toggles
* FeatureFlagGatewayStrategy currently wired with NoopFeatureFlagProvider
* Experiment assignment emitter present (A/B gateway strategy)
* Gateway pruning & diagnostics enabled structurally (pruner singleton) but feature flags are inert
24. Risk Areas
* Mixed permission naming may create intermittent access issues
* Manual fallback logic in DefinitionsController (unpublish/archive) bypasses service invariants if service path fails
* Archived definitions not filtered could confuse UX
* Task completion path may not guarantee instance completion (pending verification)
* JoinTimeoutWorker exceptions could conceal legitimate timeout operations
25. Quick Reference (Action Checklist)

| Area | Action | 
|------|--------| 
| Frontend definitions | Adjust getDefinitions() to treat data.data as array | 
| Instance start | Use response.data.id | 
| Completion issue | Inspect HumanTaskExecutor + DB row after final task | 
| Permissions | Standardize naming (constants or all literal) | 
| Archived defs | Filter in query or add includeArchived param | 
| JoinTimeoutWorker | Add null guards or temporarily disable | 
| Outbox monitoring | Watch DEADLETTER / RETRY growth |

26. Environment / Headers Required
* Authorization: Bearer {JWT}
* X-Tenant-ID: {tenantId} (or valid tenantId claim) Missing tenant ID → "Tenant context required" → currently returns 500 in some paths; adjust to 401/400 if desired.
27. Suggested Immediate Next Steps
1.	Supply HumanTaskExecutor + task completion endpoint for finalization patch.
2.	Implement frontend service refactors (definitions, startInstance).
3.	Decide on archived filtering behavior; implement backend param if needed.
4.	Normalize permissions + update token claims issuance.
5.	Add completion integration test (create simple definition: Start → HumanTask → End, complete task, assert instance Completed + outbox event).
6.	Patch JoinTimeoutWorker.
---
If you need this distilled into a shorter “startup briefing” variant, ask and I’ll compress into a one-page version.
