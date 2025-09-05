Project overview
•	Tech stack: C# 13, .NET 9, EF Core, Visual Studio 2022.
•	Domain: WorkflowService with multi-tenant workflow definitions, instances, tasks, and events.
•	Key goals: Strong tenant isolation, immutable published definitions, reliable eventing via Outbox pattern with deterministic idempotency keys.
Core projects and components
•	WorkflowService (primary service)
•	Domain models: WorkflowDefinition, WorkflowInstance, WorkflowTask, WorkflowEvent, OutboxMessage.
•	Services:
•	DefinitionService: create/update/publish/unpublish definitions; enforces immutability; emits lifecycle events.
•	EventPublisher: raises domain events via the Outbox; uses deterministic keys for idempotency.
•	OutboxWriter/OutboxDispatcher: durable enqueue + async dispatch, retries, error tracking.
•	Validation:
•	GraphValidationService.Validate(json, strict): parses DSL (WorkflowDefinitionJson), checks start/end nodes, reachability, duplicates, orphaned edges; draft vs publish modes.
•	Deterministic idempotency
•	DeterministicOutboxKey (canonical source): SHA-256 over normalized, colon-joined parts → first 16 bytes as Guid.
•	Instance(tenantId, instanceId, phase, definitionVersion)
•	Task(tenantId, taskId, phase)
•	Definition(tenantId, definitionId, phase, version)
•	DefinitionPublished/Unpublished, Custom(...)
•	OutboxIdempotency: Obsolete; forwards to DeterministicOutboxKey for compatibility.
Persistence and tenancy
•	WorkflowDbContext
•	Tenant scoping: global query filters per-entity (TenantId == current), with test-aware toggles.
•	SaveChanges:
•	SetTenantIdForNewEntities: stamps TenantId only when 0.
•	UpdateTimestamps: CreatedAt/UpdatedAt.
•	EnforceDefinitionJsonImmutability: throws if JSONDefinition mutates once published.
•	Provider-specific constraints (non-test): jsonb types, CHECK constraints aligning TenantId with current_setting('app.tenant_id') for Postgres.
Definition lifecycle and immutability
•	Draft creation/updating validates DSL (draft rules), applies edge backfill for gateways.
•	PublishAsync:
•	Validates DSL (strict), publish-time validation hooks, sets IsPublished.
•	ForcePublish (already published):
•	Idempotent success only if JSONDefinition unchanged.
•	If JSONDefinition changed, returns error “Force publish blocked: definition JSONDefinition was modified after publish”.
•	UnpublishAsync:
•	Optionally force-cancels active instances, emits events.
•	Strict tenant filtering across all queries.
Outbox pattern
•	Schema: OutboxMessage has TenantId, EventType, EventData, IdempotencyKey (unique with TenantId), ProcessedAt, RetryCount, Error, DeadLetter (flag).
•	OutboxWriter.TryAddAsync:
•	Inserts with provided key (deterministic) or logs warning if missing for workflow.* events.
•	On unique key conflict: re-queries and returns (AlreadyExisted=true).
•	OutboxDispatcher:
•	Polls unprocessed rows, delivers, sets ProcessedAt on success.
•	Retries with backoff options; truncates error; optional dead-lettering via options.
•	Logs: OUTBOX_WORKER_FETCH/DISPATCH_SUCCESS/DISPATCH_FAIL/DEADLETTER.
Events and keys (examples)
•	workflow.instance.started/completed/failed/force_cancelled → DeterministicOutboxKey.Instance(...)
•	workflow.task.created/completed/assigned → DeterministicOutboxKey.Task(...)
•	workflow.definition.published/unpublished → DeterministicOutboxKey.DefinitionPublished/Unpublished(...)
Diagnostics and logging
•	DefinitionService diagnostics (opt-in): set WF_ISO_DIAG=1 to emit ISO:… logs.
•	Test-readable diagnostics: DefinitionService.IsolationDiag (ConcurrentQueue<string>) mirrors ISO lines.
•	Visual Studio test output: use xUnit ITestOutputHelper via custom XunitOutputLoggerProvider and (optionally) Outbox.runsettings with LogConsoleOutput=true.
Testing conventions
•	DefinitionServiceBuilder:
•	Recreates DbContext per tenant (TestDbContextFactory.Create(dbName, tenantId)) to align tenant filters and persistence.
•	WithOutput(_output) to bridge logs to xUnit.
•	Isolation tests:
•	Multi-tenant unpublish force-cancel only touches same-tenant instances (confirmed via diagnostics).
•	Outbox tests:
•	Deterministic key stability and differentiation now assert DeterministicOutboxKey outputs.
•	Concurrency/isolation (O12): SQLite-backed tests ensure (TenantId, IdempotencyKey) uniqueness and cross-tenant independence.
Recent work (complete)
•	O11 Deterministic Key Strategy:
•	Implemented DeterministicOutboxKey; migrated producers/tests; legacy helper forwards; writer warns if keys missing.
•	O12 Concurrency & Isolation:
•	Parallel idempotent insert test (single row under contention).
•	Cross-tenant same logical event → separate rows confirmed.
Key environment and settings
•	WF_ISO_DIAG=1: enable ISO diagnostics in DefinitionService.
•	Outbox.runsettings: <LogConsoleOutput>true</LogConsoleOutput> to surface console logs during tests.
•	ENABLE_TENANT_FILTERS_IN_TESTS: can control tenant filters in test contexts (DbContext also caches tenant id at construction).
Known guarantees and constraints
•	Tenant isolation enforced everywhere (queries, outbox uniqueness).
•	Published workflows’ JSONDefinition immutable (service-level guard + DbContext runtime check).
•	Deterministic keys are stable across restarts; normalization is lowercase + trimmed + colon-delimited; SHA-256 first 16 bytes.
