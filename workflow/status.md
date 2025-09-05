OUTBOX & IDEMPOTENCY CONTEXT (NEXT SPRINT PREP)

Scope
Add idempotent, observable, and retry-safe delivery semantics to the existing Outbox pattern by introducing new columns, enforcing a tenant-scoped uniqueness guarantee, and enhancing dispatcher + producer logic.

Current State (Baseline)
- OutboxMessages table (assumed existing): Id (PK), TenantId, EventType / Name, Payload (JSON), CreatedAt, RetryCount (if present), IsProcessed (bool?) or similar.
- Events are written during domain operations (publish/unpublish, instance lifecycle, gateway decisions, task events).
- Dispatcher (MVP) either missing or only logs.
- No column enforcing uniqueness or dedupe across retries or producer replays.
- No timestamp marking actual dispatch completion.
- No persistent error detail for failed attempts.
- No deterministic idempotency key logic.

Target Schema (Delta)
Add three nullable columns initially:
1. IdempotencyKey (uuid) – required for new rows, backfilled for legacy rows.
2. ProcessedAt (timestamp with time zone) – null until successfully dispatched.
3. Error (text) – last failure (truncated), cleared on success.

Indexes:
- Unique index UX_Outbox_Tenant_IdempotencyKey (TenantId, IdempotencyKey).
- Supporting non-unique index IDX_Outbox_Unprocessed (ProcessedAt NULL) to accelerate dispatcher polling.

(OPTIONAL LATER) Add DeadLetter (bool), NextAttemptAt (timestamp), or Status enum if complexity increases.

Idempotency Strategy
Deterministic Key (preferred when a natural semantic identity exists):
Guid = Hash(UUID namespace + $"{tenant}:{domainType}:{entityId}:{eventKind}:{version|timestampBucket|correlation}")
If no natural key: generate Guid.NewGuid().
Producer contract: Always supply or derive a stable key for logical duplicates (e.g., re-publish event triggered twice due to retry).
On uniqueness violation: treat as idempotent success (do NOT fail business transaction).

Dispatcher Behavior (Target)
Loop:
1. SELECT batch WHERE ProcessedAt IS NULL ORDER BY CreatedAt LIMIT N.
2. For each message:
   - Attempt delivery (e.g., HTTP POST / internal bus / log stub).
   - On success: set ProcessedAt=UtcNow, Error=NULL.
   - On transient failure: increment RetryCount, set Error, leave ProcessedAt NULL.
   - Respect MaxRetries (config). If exceeded → still leave ProcessedAt NULL (MVP) OR (future) mark as dead letter.
3. Metrics/logging for each cycle.

Configuration (Add strongly-typed options)
OutboxOptions:
- PollIntervalSeconds (default: 5–15)
- BatchSize (default: 50–200)
- MaxRetries (default: 5)
- BaseRetryDelaySeconds (default: 10) – constant or backoff
- UseExponentialBackoff (bool) – if true, Delay = Base * 2^RetryCount ± jitter
- MaxErrorTextLength (e.g., 2000)

Logging Prefixes
- OUTBOX_WORKER_FETCH count=…
- OUTBOX_WORKER_DISPATCH_SUCCESS id=… key=…
- OUTBOX_WORKER_DISPATCH_FAIL id=… key=… retry=… max=…
- OUTBOX_WORKER_DUPLICATE tenant=… key=…
- OUTBOX_WORKER_SKIP_PROCESSED id=…

Observability / Metrics
Derive:
- BacklogSize = COUNT(ProcessedAt IS NULL)
- FailedPending = COUNT(ProcessedAt IS NULL AND RetryCount > 0)
- OldestAgeSeconds = MAX(NOW - CreatedAt WHERE ProcessedAt IS NULL)
- Throughput = processed / interval
- FailureRatio = (failures per cycle) / batch size

API (Optional Admin)
GET /api/workflow/outbox?status=pending|failed|processed&page=x&pageSize=y
Response includes minimal fields (Id, EventType, CreatedAt, RetryCount, ProcessedAt, LastErrorSnippet).

Migration Plan (Zero-Downtime)
1. Add nullable columns + indexes (online).
2. Backfill IdempotencyKey for existing rows (script or UPDATE with generated UUID).
3. Deploy application version generating IdempotencyKey.
4. After monitoring, (optional) enforce NOT NULL on IdempotencyKey.
5. (Optional future) Remove legacy IsProcessed if replaced by ProcessedAt semantics.

Backfill Strategy
- Single statement if data volume low:
  UPDATE OutboxMessages SET "IdempotencyKey" = gen_random_uuid() WHERE "IdempotencyKey" IS NULL;
- For large tables: chunked updates by CreatedAt range.

Producer Integration Changes
Add helper OutboxWriter.AddAsync(tenantId, eventType, payload, optionalDeterministicKey).
Internally:
- Compute key (if empty).
- Try insert.
- On unique violation -> SELECT existing row (idempotent success).
Return row Id + IdempotencyKey to caller (tracking/logging).

Duplicate Handling Flow
try Insert (TenantId, IdempotencyKey)
catch DbUpdateException (unique index):
  - Query for existing row (TenantId + IdempotencyKey)
  - Return existing row (Success = true, AlreadyExisted = true)

Testing Matrix

| Area | Test | Goal |
|------|------|------|
| Schema | Migration_Adds_Columns | Columns exist post-migration |
| Schema | Migration_Backfills_IdempotencyKey | Legacy rows have non-null key |
| Entity | OutboxMessage_DefaultValues | IdempotencyKey auto, ProcessedAt null |
| Insert | Insert_Duplicate_SameTenant_Idempotent | Single row only |
| Insert | Insert_SameKey_DifferentTenant | Two rows allowed |
| Producer | DeterministicKey_SameInput_SameGuid | Stability |
| Producer | DeterministicKey_DifferentInput_DifferentGuid | Dedupe correctness |
| Dispatcher | Dispatcher_Marks_Processed | Sets ProcessedAt |
| Dispatcher | Dispatcher_Retry_OnFailure | RetryCount++, Error set |
| Dispatcher | Dispatcher_Clears_Error_OnSuccess | Error null after success |
| Retry | Retry_Stops_After_MaxRetries | No further attempt (check elapsed attempts) |
| Backoff | Backoff_Delay_Grows_Exponentially | Verify delay schedule function |
| Logging | Logs_On_Success_And_Failure | Contains expected prefixes |
| Metrics | Metrics_Computation_Accurate | Derived values correct with known dataset |
| Concurrency | Parallel_Inserts_SingleRow | Race-safe insertion |
| Concurrency | Parallel_Dispatch_SingleProcessPerMessage | (Optional) row-level concurrency guard |
| Error Handling | Error_Truncated_When_TooLong | Enforces MaxErrorTextLength |
| Admin (optional) | Admin_List_Pending | Only unprocessed rows returned |
| Admin (optional) | Admin_Filter_Failed | Failed subset matches criteria |

Open Decisions
1. Dead letter semantics: Defer or implement (DeadLetter bool)?
2. Hard TTL for processed rows purge? (Not in MVP; retention config later.)
3. Deterministic key base: Use deterministic namespace GUID or stable hash→Guid conversion?
4. Backoff: Exponential vs fixed for MVP?
5. Should dispatcher be single-instance (leader election) or allow multi-node with DB-side filtering? (Current design: safe multi-node; rely on SKIP LOCKED or row version update—if provider supports. For InMemory, single node only.)
6. Payload size / compression needed? (Probably not MVP.)
7. Health check integration? (Expose backlog + oldest age via IHealthCheck.)

Implementation Order (Recommended)
1. Migration + EF model updates.
2. Deterministic key utility + producer updates (no-op dispatcher can still run).
3. Dispatcher enhancements (mark ProcessedAt).
4. Duplicate handling + concurrency test.
5. Retry/backoff logic.
6. Observability (logging/metrics).
7. Optional admin endpoint.
8. Hardening (error truncation, dead letter future).

Deterministic Key Utility (Concept)
OutboxIdempotency.CreateKey(tenantId, category, entityId, kind, versionOrSeq?)
- Canonical string normalized (lowercase, ‘:’ delimiter)
- Compute SHA-256 → take first 16 bytes → Guid
- Ensures portability without reliance on Guid namespace algorithms.

Sample Pseudocode (Key Generation)
ComputeString = $"{tenantId}:{category}:{entityId}:{kind}:{version ?? 0}";
Hash = SHA256(UTF8(ComputeString));
Guid = new Guid(first16BytesOf(Hash));

Dispatcher Pseudocode
while (!stopping)
{
  rows = SELECT ... WHERE ProcessedAt IS NULL ORDER BY CreatedAt LIMIT BatchSize;
  if (rows.Empty) delay(PollInterval); continue;

  foreach(row in rows)
  {
    try {
      Deliver(row);
      UPDATE set ProcessedAt=UtcNow, Error=NULL WHERE Id=...
      log success
    } catch(ex) {
      UPDATE set RetryCount=RetryCount+1, Error=Truncate(ex.Message) WHERE Id=...
      log failure
    }
  }

  delay(PollInterval);
}

Risk & Mitigation
| Risk | Impact | Mitigation |
|------|--------|------------|
| Duplicate message under retry storm | Downstream duplication | IdempotencyKey dedupe |
| Long backlog grows unbounded | Storage pressure | Future retention policy + metrics alerts |
| Dispatcher crash mid-flight | Partial progress | ProcessedAt atomic update ensures retry-only unprocessed |
| Large error payloads | Table bloat | Truncate to configured length |
| Multi-node race on same message | Double delivery | Add SELECT ... FOR UPDATE / row-level version OR rely on relational locking; not fully addressed in InMemory |
| Missing deterministic key when possible | Reduced dedupe value | Enforce helper usage, code reviews |

Rollout Checklist
- Apply migration in staging with high event volume simulation.
- Verify no null IdempotencyKey after backfill (SELECT COUNT WHERE IdempotencyKey IS NULL).
- Deploy producers (writes new key).
- Deploy dispatcher update.
- Monitor backlog + error metrics.
- (Optional) Add NOT NULL + default constraint once confirmed stable.

Glossary
- Outbox Message: Durable record representing a domain/integration event awaiting dispatch.
- IdempotencyKey: Logical unique fingerprint for an event, enabling deduplication.
- ProcessedAt: Timestamp marking successful completion of side-effect dispatch.
- Backlog: Set of messages with ProcessedAt IS NULL.
- Dead Letter: (Future) Message exceeding retry policy requiring manual intervention.

Next Step
Confirm open decisions (dead letter, retry strategy, key algorithm). Then proceed with Migration + EF model code generation (Story O1/O2).


Outbox Idempotency & Schema Migration — Explanation
The Outbox pattern guarantees reliable, exactly-once (or at-least-once with dedupe) publication of domain events / integration messages by persisting them in the same transaction as business state changes, then dispatching them asynchronously.
Adding an IdempotencyKey and processing metadata formalizes safe re-delivery, retry, and observability.
Key additions:
* IdempotencyKey (Guid): Stable identifier supplied (preferably) by the producer (e.g., logical workflow event identity) or generated server-side. Used to prevent duplicate logical messages (e.g., retry from client, race across nodes).
* ProcessedAt (timestamp / UTC): Set once dispatcher finishes the side-effect (HTTP call, enqueue, etc.). Enables lag metrics and cleanup.
* Error (text / nullable): Last failure detail (truncated/sanitized) for inspection & alerting.
* Unique index (TenantId, IdempotencyKey): Enforces tenant-scoped idempotent insert. (Tenant isolation preserved; same key can exist across tenants.)
* Dispatcher logic uses IsProcessed OR ProcessedAt as terminal flag to skip further handling.
* RetryCount (if already present) increments; on success Error cleared.
Migration concerns:
1.	Backfill existing rows: New columns must allow NULL initially. IdempotencyKey assigned for legacy rows (deterministic new Guid per row) so uniqueness is satisfied.
2.	Online deployment: Add columns (NULLable), backfill batch-wise if large, then create unique partial index. If small, single migration fine.
3.	Code path must not assume non-null until after migration deployed (feature flag or null-safe code).
4.	Concurrency: Inserting duplicate key should catch DbUpdateException; convert to application-level idempotent success (return existing message or treat as no-op).
Idempotency generation strategy:
* For workflow events derived from underlying entity (InstanceId + EventType + Sequence), prefer hash-based deterministic Guid (e.g., GuidUtility.CreateV5(namespace, $"{tenant}:{eventType}:{entityId}:{version}:{optionalCorrelation}")).
* If no natural key, generate Guid.NewGuid().
Failure recording:
* On transient failure: increment RetryCount, set Error, leave ProcessedAt null.
* On poison failing after MaxRetries (config), optionally mark Error + DeadLetter=True (future column) or still leave as retriable.
Metrics enabled:
* Outbox backlog = COUNT where ProcessedAt IS NULL.
* Old failures = COUNT where Error NOT NULL AND RetryCount >= threshold.
* Processing latency = NOW() - CreatedAt for unprocessed items.
* Throughput = processed per interval.


## Story & Test Plan

### ✅ DONE O1: Schema Migration
#### Tasks:
* Alter OutboxMessage table: add columns
* IdempotencyKey uuid NULL
* ProcessedAt timestamptz NULL
* Error text NULL
* Backfill: update all existing rows set IdempotencyKey = gen_random_uuid() (or NEWID() / uuid_generate_v4()).
* Add unique index: UX_Outbox_Tenant_Idem (TenantId, IdempotencyKey).
* (Optional) Add supporting index on ProcessedAt IS NULL for dispatcher scan. Tests:
* Migration_DDL_Contains_NewColumns (if using migration inspection).
* Outbox_Insert_DuplicateIdempotencyKey_SameTenant_ShouldFailDB (unit/integration) – asserts uniqueness enforced.
* Outbox_Insert_SameKey_DifferentTenant_ShouldSucceed.

### ✅ DONE O2: Domain / Model Update
#### Tasks:
* Update OutboxMessage entity + EF configuration (HasIndex composite unique).
* Ensure property types: Guid IdempotencyKey (non-null in code after insert), DateTime? ProcessedAt, string? Error.
* Ensure SaveChanges interceptor or repository assigns IdempotencyKey if default(Guid). Tests:
* OutboxMessage_Defaults_Assigned_OnAdd (IdempotencyKey auto-populated, ProcessedAt null).
* OutboxMessage_Roundtrip_Persists_Values.

### ✅ DONE O3: Producer Integration
## Tasks:
* Update all places creating outbox rows (e.g., event publisher):
* Accept optional idempotency seed parameters.
* Provide deterministic key where logical duplication risk exists (e.g., publish definition published event multiple times).
* Overload PublishX methods to accept IdempotencyKey (optional). Tests:
* PublishDefinition_Event_Uses_Deterministic_IdempotencyKey (two publish attempts produce same key if logically same event).
* PublishInstanceForceCancelled_Events_Have_UniqueKeys (different instance events differ).
### ✅ DONE O4: Dispatcher Enhancements
Tasks:
* Modify dispatcher query: SELECT * FROM OutboxMessages WHERE ProcessedAt IS NULL ORDER BY CreatedAt LIMIT N.
* On success: set ProcessedAt=UtcNow, clear Error.
* On failure: set Error (truncated), increment RetryCount.
* Add logging prefixes OUTBOX_WORKER. Tests:
* Dispatcher_Sets_ProcessedAt_OnSuccess.
* Dispatcher_Sets_Error_And_Increments_Retry_OnFailure (mock transport throw).
* Dispatcher_Skips_AlreadyProcessed.
### ✅ DONE O5 Idempotent Insert Handling
Tasks:
* Wrap outbox write in try/catch; if uniqueness violation on (TenantId, IdempotencyKey), treat as idempotent success (return existing row or ignore).
* Provide helper TryAddOutboxAsync(TenantId, key, factory) that re-queries on conflict. Tests:
* TryAddOutbox_Twice_SameKey_SingleRowExists.
* TryAddOutbox_Parallel_Inserts_SingleRow (multi-thread simulation / race).
### ✅ DONE 06 Retry / Backoff Policy
Tasks:
* Add configuration: MaxRetries, BaseDelay, Jitter.
* Implement backoff evaluation (e.g., exponential for next attempt).
* Dispatcher respects next-attempt time (optional: add NextAttemptAt column later; for MVP simple constant delay). Tests:
* Dispatcher_Retry_Until_MaxRetries_Reached.
* Dispatcher_NoRetry_After_MaxRetries (still unprocessed but flagged).
### 🚧 (in progress) O7 Observability & Metrics
Tasks:
* Add log lines:
* OUTBOX_WORKER_FETCH count=X
* OUTBOX_WORKER_DISPATCH_SUCCESS id=...
* OUTBOX_WORKER_DISPATCH_FAIL id=... retry=...
* OUTBOX_WORKER_DUPLICATE_KEY tenant=... key=...
* Add health/diagnostics endpoint or log summary every N cycles. Tests:
* (Log Assertion) Outbox_Dispatch_Emits_Success_Log (optional).
* Metrics_Snapshot_Computation (unit of helper).
O8: API / Admin Visibility (Optional)
Tasks:
* Add admin endpoint GET /api/workflow/outbox?status=pending|failed|processed for observability.
* Support pagination, filtering by EventType. Tests:
* Outbox_Admin_List_Pending.
* Outbox_Admin_Filter_By_Status.
O9: Backfill Script Safety (Deployment)
Tasks:
* Ensure migration is additive & backward compatible.
* Confirm dispatcher code tolerant to NULL IdempotencyKey until post-migration (guard generation before use).
* Document zero-downtime rollout steps. Tests:
* LegacyRow_Backfill_Adds_Generated_IdempotencyKey (integration using manual insert without key).
O10: Failure Scenarios & Poison Handling (Deferred if not MVP)
Tasks:
* Decide on threshold: if RetryCount >= MaxRetries mark a status (future column) or continue indefinite.
* Decide if Error length should be truncated (e.g., 2000 chars). Tests:
* Outbox_Error_Truncated_When_TooLong.
O11: Deterministic Key Strategy (Design)
Tasks:
* Utility: OutboxIdempotency.CreateDeterministicKey(params) → Guid (namespace-based v5 or hash → Guid).
* Centralize to avoid drift across producers. Tests:
* DeterministicKey_SameInputs_SameGuid.
* DeterministicKey_DifferentInputs_DifferentGuid.
O12: Concurrency & Isolation Tests
Tasks:
* Simulate multiple threads adding same idempotent event (Task.WhenAll).
* Use barrier to align execution and assert single row created. Tests:
* Concurrency_IdempotentInsert_Race_Produces_SingleRow.
---
Test Coverage Matrix (Summary)
| Story | Unit Tests | Integration Tests | Notes | |-------|------------|-------------------|-------| | O1 | Migration DDL reflection (optional) | Backfill legacy rows | May mock relational provider | | O2 | Entity default assignment | Roundtrip via real DB | | | O3 | Deterministic key generation | Duplicate logical event path | | | O4 | Dispatcher success/failure logic | End-to-end dispatch cycle | Mock transport | | O5 | Duplicate insert handling | Parallel insert scenario | | | O6 | Retry progression logic | Max retry boundary | Delay calc pure unit | | O7 | Metrics helper pure funcs | (Optional) log capture | | | O8 | (If implemented) endpoint query shape | API response filter tests | Optional MVP | | O9 | Legacy row upgrade | Full migration path | | | O10 | Error truncation | — | Optional | | O11 | Key generation determinism | — | Utility only | | O12 | Race handling | High-contention test | Use in-memory or sqlite |
---
Implementation Order (Recommended)
1.	O1/O2: Schema & model (migration first).
2.	O3: Producers emit idempotent keys.
3.	O4/O5: Dispatcher logic + conflict handling.
4.	O6: Retry/backoff (simple constant first).
5.	O11: Deterministic key utility integrated.
6.	O7: Logging & metrics.
7.	O12: Concurrency assurance.
8.	Optional: O8 admin view / O10 truncation.
---
Rollout / Deployment Notes
1.	Deploy migration adding nullable columns & unique index (after backfill of IdempotencyKey to avoid null duplicates).
2.	Release code that:
* Always sets IdempotencyKey for new messages.
* Skips processing when ProcessedAt != null.
3.	After verifying backlog stable, optionally add NOT NULL constraint to IdempotencyKey in later hardening migration.


Effort comparison (assuming you just paste code I generate; your “manual” time is review/integration only):

| Aspect | Option 2 (Minimal Completion) | Option 3 (Full Expansion incl. Prometheus) | 
|--------|------------------------------|--------------------------------------------| 
| Scope | Snapshot + FailureRatio + Throughput, HealthCheck endpoint (/health) + /api/.../metrics JSON | Everything in Option 2 plus Prometheus /metrics exposition, rolling window stats, optional counters/gauges, (maybe) dead-letter ready hooks | 
| New Classes | +1 HealthCheck, +1 Controller (Metrics), small extension of existing MetricsProvider | + HealthCheck + Controller + PrometheusExporter (or adapter), RollingWindowAggregator, MetricFormatter | 
| Code Size (approx LOC) | 250–320 | 550–750 (nearly 2–2.5x) | 
| Test Additions | 3–4 tests (health OK/Degraded/Unhealthy, snapshot fields) | 6–9 tests (those + Prometheus format, rolling throughput, counters increment correctness) | 
| Config Additions | Health thresholds section | Same plus optional Prometheus enable flag & rolling window length | 
| Operational Impact | Basic readiness / liveness monitoring + simple JSON metrics (good enough for dashboards via scraper adapter) | First-class integration with Prometheus/Grafana; lower future rework if observability maturity is desired | 
| Risk / Complexity | Low (all synchronous, no streaming) | Moderate (format correctness, potential cardinality concerns, concurrency in rolling stats) | 
| Extensibility Later | Can still add Prometheus later with minimal refactor (wrap existing provider) | Already “done” – future changes are incremental | 
| Added Dependencies | None (stick to built-in HealthChecks) | Optional prometheus-net (if chosen) or custom plain-text writer (no external dep) |
