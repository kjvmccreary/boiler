WORKFLOW PLATFORM CONTEXT (CONDENSED – FEED THIS TO START A SESSION)

Purpose
Multi-tenant workflow execution service (.NET 9) with:
- Versioned workflow definitions (graph-based JSON DSL)
- Runtime engine (nodes, gateways, joins, timers, human & automatic tasks)
- Event + Outbox persistence for external integration (idempotency upgrade pending)
- Strong RBAC + per-tenant data isolation
- Background workers (current: join timeout; upcoming: timer auto-advance, outbox dispatcher)

Core Domain Objects
WorkflowDefinition
- Fields: Id, TenantId, Name, Version, JSONDefinition, IsPublished, IsArchived, PublishedAt, ArchivedAt, UpdatedAt
- Immutability: JSONDefinition cannot change after publish (enforced at DbContext SaveChanges)
- New versions created via CreateNewVersion → Version++ (draft)

WorkflowInstance
- Fields: Id, TenantId, WorkflowDefinitionId, DefinitionVersion, Status (Running, Suspended, Completed, Failed, Cancelled), Context (JSON), CurrentNodeIds (JSON array), StartedAt/UpdatedAt/CompletedAt/ErrorMessage
- Progress orchestrated by WorkflowRuntime

WorkflowTask
- Human or timer tasks represent execution wait states
- Fields: Status (Created, Assigned, Claimed, InProgress, Completed, Cancelled), NodeType (human/timer), DueDate (timer), AssignedToUserId
- Timer tasks currently require manual completion (TimerWorker still pending)

OutboxMessage
- Mirrors WorkflowEvents for external dispatch (Outbox Dispatcher MVP exists; idempotent enhancements planned: IdempotencyKey, ProcessedAt, Error, unique (TenantId, IdempotencyKey))

WorkflowEvent
- Fine-grained trace: instance lifecycle, node execution, gateway decisions, joins (arrive/satisfy/timeout), task events, experiment assignment, feature flag fallback, pruning

Node Types
- start, end, humanTask, timer, automatic, gateway, join
- Gateways: exclusive, parallel, abTest, featureFlag
- Joins: all|any|count|quorum|expression (quorum + timeout implemented)

Gateway Strategies
abTest
- Deterministic assignment via hash(seed: keyPath value + gatewayId + definitionVersion)
- Weighted variants (weights sum ~100)
- Snapshot stored to preserve stability if context changes
- Overrides via _overrides.gateway[gatewayId]
featureFlag
- Uses IFeatureFlagProvider (default false)
- required=true + provider failure → fallback event + offTarget route

Join Enhancements
- Quorum join satisfaction based on thresholdCount or thresholdPercent
- Timeout metadata (force|route|fail) handled by JoinTimeoutWorker (background)
- Human task completion triggers join arrival update

Diagnostics / Context Structures (JSON inside WorkflowInstance.Context)
- _gatewayDecisions: gateway decision history (bounded & pruned)
- _experiments: abTest snapshot assignments
- _parallelGroups: branch bookkeeping + join metadata
- Timeout fields: timeoutSeconds, timeoutAtUtc, timeoutTriggered

Validation
Graph / Publish Validation:
- Single start, ≥1 end, all nodes reachable from start, no unreachable end nodes, no duplicate IDs, no isolated islands
Strategy validation:
- abTest: ≥2 variants, valid weights, keyPath required
- Automatic actions (action.kind presence & (future) webhook URL checks)
Immutability:
- JSONDefinition change post-publish → InvalidOperationException (guarded in DbContext)

Security & RBAC
- Per-tenant isolation (query filters optionally enabled in tests via ENABLE_TENANT_FILTERS_IN_TESTS)
- Permissions: workflow.view_tasks, claim_tasks, complete_tasks, admin, view_instances, start_instances (seed audited)
- Cross-tenant access returns 404 (not 403) by design

Background Workers
Implemented:
- JoinTimeoutWorker (join timeout actions)
Pending / Partial:
- TimerWorker (auto-complete due timer tasks → advance runtime)
- OutboxDispatcher (poll unprocessed messages, apply retries + idempotency once enhanced)

Testing (High Coverage Areas)
- Gateway strategies: abTest (assignment stability, override), featureFlag (on/off)
- Quorum joins & timeout actions
- Definition immutability rules (update, unpublish with/without force)
- Multi-tenant isolation:
  * Definition list / instance retrieval / task operations / admin operations
  * Force terminate isolation (ensures tenant 2 unpublish doesn’t affect tenant 1)
- Validation: negative publish scenarios, multiple start detection, gateway branching
- Context pruning (diagnosticsVersion, bounded decision history)
- UoW batching (SaveChanges count assertions for task operations)
Gaps / Remaining Tests (Optional):
- Feature flag provider outage fallback
- Statistical abTest distribution (optional)
- Join expression mode edge cases
- Timer automation end-to-end (post TimerWorker)
- Outbox idempotency & duplicate handling (upcoming)
- Race / fuzz ID probing (optional hardening)
- Dead-letter / retry exhaustion (future)

Observability & Logging
Prefixes / Patterns:
- UNPUBLISH_BLOCKED / UNPUBLISH_FORCE_TERMINATE / UNPUBLISH_SUCCESS
- IMMUTABILITY_VIOLATION (DbContext guard)
- Gateway + automatic executor structured logs
Planned:
- TIMER_WORKER / OUTBOX_WORKER structured logs + metrics snapshot
Metrics Targets (post-refactor):
- SaveChanges per claim/complete: 1
- Timer cycle latency: <60s configurable
- Outbox processing lag: <2 min
- Validation errors surfaced with clear actionable list

Remaining Priority Work (High-Level)
1. TimerWorker Implementation (auto-fire due timer tasks)
2. Outbox Idempotency & Dispatcher upgrade (IdempotencyKey, ProcessedAt, Error)
3. Graph Validation depth expansion (already largely implemented—confirm completeness)
4. Documentation additions (developer onboarding, external workflow trigger guide)
5. TaskService / InstanceService deeper isolation race tests (optional)
6. Optional: Admin observability endpoints (outbox status, backlog metrics)
7. Future: Webhook automatic executor + retry policies

Outbox Idempotency Upgrade (Planned)
Add:
- IdempotencyKey (Guid) + unique (TenantId, IdempotencyKey)
- ProcessedAt (Nullable timestamp)
- Error (text)
Behavior:
- Producers supply deterministic keys when logical duplicates possible
- Dispatcher sets ProcessedAt on success; increments RetryCount + sets Error on failure
- Duplicate insert (same TenantId + IdempotencyKey) → treat as idempotent success (no throw)
- Metrics: backlog size, oldest unprocessed age, failure ratio

Design Conventions
- JSONDefinition normalized (gateway edge enrichment) when saving drafts
- Context mutated minimally; large diagnostics trimmed by pruning
- Return shape: ApiResponseDto<T> with Success / Message / Errors[] (Code + Message)
- 404 for cross-tenant resource probes; avoid leaking existence
- EF InMemory nuances handled with GUID-suffixed DB names in tests to avoid bleed
- SaveChanges batching: controllers commit via deferred UoW when runtime invoked

Key Risks / Watchpoints
- Missing TimerWorker delays SLA-timer progression
- Outbox without idempotency risks duplicate external emissions when dispatcher introduced
- Graph validation regressions could allow unreachable nodes → runtime dead paths
- Multi-tenant filter toggle: ensure production always enables filters; test toggles scoped
- Concurrent unpublish + start edge cases (acceptable race; design allows instance start if observed published state earlier)

Quick Start Mental Model
Definitions (immutable JSON after publish) → Instances (progress through engine) → Tasks (wait states) → Events (persist) → Outbox (external integration) → Background workers (advance timeouts & timers, dispatch outbox).

Use This Document To
- Rehydrate architectural understanding quickly
- Prioritize remaining MVP blockers (TimerWorker, Outbox Idempotency)
- Identify untested or partially tested edge cases to solidify before expansion
- Drive next sprint scoping and test story creation

End of condensed context.
