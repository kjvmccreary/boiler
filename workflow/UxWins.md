# WorkflowService — Tier 1 UX Wins Dev Plan (Epics → Stories → Tasks)

*Last updated: 2025-09-05*

This plan implements the six Tier‑1 capabilities for SMART Works’ **WorkflowService** using the architecture and constraints from `context.md` (multi‑tenant, immutable published definitions, deterministic idempotency keys, Outbox pattern, Validation services, and strict tenant filters). It is formatted for direct copy/use inside Visual Studio + GitHub Copilot Chat.

---

## Sprint Map (suggested)

* **Sprint 1: SLA & Notifications + Decision Table (DMN‑lite) + ActionKinds Foundations (Integration/Audit/Workflow Control)**
* **Sprint 2: Wait for Condition + Message In (event resume) + ActionKinds: DocGen + E‑Sign + Search**
* **Sprint 3: Call Activity (subflow) + AITask (guardrailed) + ActionKinds: Files/DMS + Security + Retention**

> Each sprint below contains Epics → Stories (with Acceptance Criteria) → Tasks. Include test stories for xUnit.

---

## Sprint 1 — SLA & Notifications + Decision Table

### Epic S1‑E1: Notification Node + SLA Policies & Boundary Timers

**Goal:** First‑class notifications with reusable SLA policies and boundary timers to reduce “black holes.”

#### Domain/Schema Impacts

* Add `SlaPolicy` aggregate (in‑definition policy bag) and `NotificationNode` type in DSL.
* Add `TaskSla` runtime record: `{ TaskId, PolicyId, DueAt, Status, LastReminderAt, EscalationStep }`.
* Extend `WorkflowTask` with `SlaPolicyId` and `BoundaryEvents[]`.

#### Stories

1. **S1‑E1‑S1: Define SLA policies in workflow definitions**

   * **AC:**

     * Can declare `policies.sla[]` in JSON (id, dueRule, escalations\[] with after/to/notifyTemplateId).
     * `ValidationService` enforces schema; publish fails if malformed.
     * `DefinitionService.PublishAsync` persists policies; published JSON immutable.
   * **Tasks:**

     * Update JSON schema + parser to hydrate `SlaPolicy` into definition model.
     * Extend `GraphValidationService.Validate(strict)` with SLA policy checks.
     * Unit tests: draft vs publish validation; immutability guard.

2. **S1‑E1‑S2: Attach SLA to HumanTask and support Timer boundary**

   * **AC:**

     * HumanTask may reference `slaPolicyId` (optional).
     * On task creation, engine materializes `TaskSla` with computed `DueAt` (supports business days later).
     * Boundary Timer triggers escalate/reminder branch when overdue.
   * **Tasks:**

     * Add `DueAtCalculator` with business calendar hook (default: calendar‑agnostic; inject later).
     * Extend task creation pipeline to persist `TaskSla` and schedule a `TimerEvent` (row + outbox event).
     * Implement boundary evaluation during token tick.
     * Unit tests: create task → due computed → boundary branch fires.

3. **S1‑E1‑S3: Notification Node executor**

   * **AC:**

     * Node accepts `{ templateId, channel, targets[], vars }`.
     * Executes by emitting `workflow.notification.requested` to Outbox with deterministic key.
     * No provider coupling here (NotificationService handles send + retries).
   * **Tasks:**

     * Implement `NotificationExecutor` (Automatic subclass or dedicated node executor).
     * Integrate with `EventPublisher` via OutboxWriter (deterministic idempotency key).
     * Unit tests: deterministic key stable; cross‑tenant independence; outbox row created once under contention.

4. **S1‑E1‑S4: SLA Reminder/Escalation worker**

   * **AC:**

     * Background worker scans `TaskSla` for reminders/escalations.
     * Emits Notification node (or direct outbox event) per escalation step; idempotent under retries.
   * **Tasks:**

     * Add `SlaWorker` with backoff strategy; respects tenant scoping.
     * Record `LastReminderAt`, increment `EscalationStep`, prevent duplicate sends via deterministic keys.
     * Unit tests: overdue → reminder once per step; tenant isolation.

5. **S1‑E1‑S5: Observability and metrics**

   * **AC:**

     * Counters: `tasks_sla_on_time`, `tasks_sla_overdue`, `sla_escalations_sent`.
     * Logs: `SLA_WORKER_SCAN/REMINDER_SENT/ESCALATE_SENT` with tenant + task IDs.
   * **Tasks:**

     * Wire structured logging; export metrics via `EventCounters`/OpenTelemetry.

---

### Epic S1‑E2: Decision Table (DMN‑lite) feeding Gateways

**Goal:** Declarative routing and approver roles with predictable outcomes.

#### Domain/Schema Impacts

* Add `DecisionTable` node to DSL with `rulesetId`, `inputs[]`, `rules[]`, `default`.
* Add `Ruleset` repository (JSON stored with definition; cache per version).

#### Stories

1. **S1‑E2‑S1: Ruleset storage & validation**

   * **AC:**

     * `rulesetId` JSON accepts `inputs[]`, `rules[]` with `when` (expression) and `set` (vars patch).
     * Publish fails if references unknown inputs/outputs.
   * **Tasks:**

     * Extend definition schema; parser persists `Ruleset`.
     * Validator ensures referenced outputs are declared in node `outputs`.

2. **S1‑E2‑S2: Expression evaluator**

   * **AC:**

     * Support `${var}` substitutions, logical ops, numeric/string comparisons.
     * Deterministic evaluation; guard against `NullReference` via safe accessors.
   * **Tasks:**

     * Implement small expression engine (interpreter) or integrate open‑source with sandbox.
     * Unit tests across types and edge cases.

3. **S1‑E2‑S3: DecisionTable executor → Gateway**

   * **AC:**

     * Node evaluates rules, patches instance variables, then jumps to Gateway using `expr = ${path}`.
   * **Tasks:**

     * Implement executor; mutation scoped to instance `Vars`.
     * Tests: rule match order, default path, variable patch visibility.

4. **S1‑E2‑S4: Tooling & examples**

   * **AC:**

     * Provide sample rulesets: approval routing by `total/risk`, audit sampling tiers, EDC eligibility.
   * **Tasks:**

     * Add `samples/decisiontables/*.json`; docs in `/docs/decision-table.md`.

---

### Epic S1‑E3: ActionKinds Foundations (Integration/Audit/Workflow Control)

**Goal:** Provide a stable Automatic‑node ActionKind registry with idempotent execution and core, pod‑agnostic actions.

#### Domain/Schema Impacts

* Add `ActionKindRegistry` with JSON‑schema per kind and optional `compensatorKind`.
* Add `AutomaticExecutor` pipeline using `(TenantId, InstanceId, NodeId, Kind, ArgsHash)` idempotency key.

#### Stories

1. **S1‑E3‑S1: ActionKind registry & validation**
   **AC:** Registry lists kinds with arg schemas; publish‑time validation rejects invalid `action.args`.
   **Tasks:** Registry model + in‑memory cache; JSON‑schema validators; docs.

2. **S1‑E3‑S2: Idempotent execution & error mapping**
   **AC:** Duplicate invocations (same key) do not re‑execute; failures map to `Error` boundary with stable codes.
   **Tasks:** Idempotency store; exception taxonomy; unit tests for replay.

3. **S1‑E3‑S3: Implement integration kinds**
   **Kinds:** `http.call`, `queue.publish`, `audit.append`.
   **AC:** Timeouts/retries on `http.call`; headers/body templating; RabbitMQ publish with headers; audit writes via `AuditService`.
   **Tasks:** HTTP client with retry/circuit breaker; MQ publisher; AuditService adapter; tests.

4. **S1‑E3‑S4: Implement workflow‑control kinds**
   **Kinds:** `wf.setVars`, `wf.copyVar`, `wf.setSLA`, `wf.defer`.
   **AC:** Vars patch/copy visible to downstream nodes; `wf.setSLA` recalculates due; `wf.defer` registers an event wait.
   **Tasks:** Vars patcher; SLA recompute; defer → `EventWaitIndex` row; tests.

---

## Sprint 2 — Wait for Condition + Message In

### Epic S2‑E1: Wait for Condition (event/poll)

**Goal:** Remove manual polling; the workflow advances itself.

#### Domain/Schema Impacts

* Add `WaitForCondition` node with `predicate.kind = event|poll`.
* Add `CorrelationKey` tuple support `(tenantId, entityType, entityId)` in DSL `events[]`.

#### Stories

1. **S2‑E1‑S1: Event predicate support**

   * **AC:**

     * Node subscribes to `events[]` declared on definition.
     * On matching event (name + correlation tuple), token resumes to `next`.
   * **Tasks:**

     * Implement `EventWaitIndex` table `{ TenantId, EventName, CorrelationHash, TokenId }`.
     * On entering node, register wait row; on event arrival, resume atomically.
     * DeterministicOutboxKey used for emitted `workflow.instance.resumed`.
     * Tests: multiple tokens waiting; cross‑tenant isolation; idempotent resumes.

2. **S2‑E1‑S2: Poll predicate support (minimal)**

   * **AC:**

     * Node with `predicate.kind=poll, intervalSec, expr` wakes periodically and evaluates.
   * **Tasks:**

     * `PollWorker` scheduled scan; compile `expr` using same evaluator.
     * Tests: backoff, cancellation when token moves.

3. **S2‑E1‑S3: Safety and timeouts**

   * **AC:**

     * Optional timeout → boundary Timer or fallback branch.
   * **Tasks:**

     * Add `timeout` to node schema; integrate with boundary events.

---

### Epic S2‑E2: Message In (event resume)

**Goal:** Resume tokens when domain/integration events occur (RabbitMQ/Bus).

#### Stories

1. **S2‑E2‑S1: Inbound event API + bus adapter**

   * **AC:**

     * `POST /wf/instances/{id}/signal` accepts `{ name, correlation, payload }` (API path already planned).
     * Bus adapter listens to `workflow.external.*` and translates to the same internal signal call.
   * **Tasks:**

     * Implement controller endpoint with tenant validation; re‑use `EventPublisher` for audit events.
     * Create `BusListener` (RabbitMQ) module → controller call.
     * Tests: bad tenant, unknown token, duplicate signals.

2. **S2‑E2‑S2: Correlation hashing & index**

   * **AC:**

     * Correlation normalized and hashed for index lookups; stable across restarts.
   * **Tasks:**

     * `CorrelationHash = SHA256(lowercase(json-canon(correlation)))[:16]`.
     * Migrations + unique constraints with TenantId.

3. **S2‑E2‑S3: Idempotent event handling**

   * **AC:**

     * Duplicate external events do not double‑resume tokens.
   * **Tasks:**

     * Store `ReceivedEvents` with `(TenantId, ExternalEventId)` unique; skip duplicates.

---

### Epic S2‑E3: ActionKinds — DocGen + E‑Sign + Search

**Goal:** Enable high‑value business actions used across pods.

#### Stories

1. **S2‑E3‑S1: DocGen adapter**
   **Kinds:** `doc.generateFromTemplate` (v1), `doc.merge` (optional), `doc.compareAndRedline` (optional).
   **AC:** Calls DocGenService; returns `generatedDocId`; idempotent on same `templateId+dataRefHash`.
   **Tasks:** DocGen client; args hashing; unit tests + sample templates.

2. **S2‑E3‑S2: E‑Sign minimal**
   **Kinds:** `esign.sendEnvelope`, `esign.fetchStatus` (optional for poll).
   **AC:** Sends envelope via EsignService/provider; returns `envelopeId`; emits `MessageOut` `EsignEnvelopeSent`; integrates with `WaitForCondition(event)` on completion.
   **Tasks:** Esign adapter; provider error mapping; samples.

3. **S2‑E3‑S3: Search indexing**
   **Kind:** `search.index`.
   **AC:** Indexes arbitrary payloads (docType/id/bodyRef) to SearchService; retries on 429/5xx.
   **Tasks:** Search client; backoff policy; tests.

4. **S2‑E3‑S4: Webhook notifications**
   **Kind:** `notify.webhook` (thin wrapper over `http.call` + templating).
   **AC:** Posts templated payloads to CMS/portals; redacts secrets in logs.
   **Tasks:** Template expansion; secret masking; tests.

---

## Sprint 3 — Call Activity (subflow) + AITask (guardrailed)

### Epic S3‑E1: Call Activity (sub‑workflow invocation)

**Goal:** Reuse standard subflows (e.g., StandardApproval, PublishPublicNotice) across pods.

#### Domain/Schema Impacts

* Add `CallActivity` node with `{ workflowId, input, wait = none|first|all }`.
* Add parent/child linkage on instances.

#### Stories

1. **S3‑E1‑S1: Spawn child instance(s)**

   * **AC:**

     * Node can start a sub‑workflow (same tenant) with input vars; captures `ChildInstanceIds`.
   * **Tasks:**

     * Implement `InstanceSpawner`; reuse `DefinitionService` lookup (immutable version pinning).
     * Tests: missing workflow; version pinning; tenant mismatch rejected.

2. **S3‑E1‑S2: Wait strategies**

   * **AC:**

     * `wait=none` immediately advances; `first` advances on first child completion; `all` waits for all.
   * **Tasks:**

     * Implement `ChildCompletionIndex`; resume logic; cancellation propagation on parent cancel.

3. **S3‑E1‑S3: Compensation scope (enroll)**

   * **AC:**

     * Optional enrollment so parent compensation cancels/voids children.
   * **Tasks:**

     * Mark scope; on compensation, signal child cancels; tests for idempotency.

---

### Epic S3‑E2: AITask (guardrailed LLM/NLP executor)

**Goal:** First‑class AI steps with cost caps and schema‑validated outputs.

#### Domain/Schema Impacts

* Add `AITask` node with `{ promptId, inputs, schemaId, guardrails }`.
* Persist `AiInvocationLog` for cost/latency telemetry.

#### Stories

1. **S3‑E2‑S1: Provider abstraction**

   * **AC:**

     * `IAIClient` interface with `CompleteJsonAsync(promptId, inputs, schemaId, caps)`; first provider `OpenAI:gpt-5` via AiService.
   * **Tasks:**

     * Implement adapter client; retry policy with exponential backoff.
     * Unit tests: schema conformance, retry/backoff, provider errors map to `Error` boundary.

2. **S3‑E2‑S2: Cost caps & audit**

   * **AC:**

     * Enforce `maxCostUsd`; log `AiInvocationLog(TenantId, NodeId, CostUsd, Tokens, DurationMs)`.
   * **Tasks:**

     * Cost estimator hookup; persist log; metrics for mean cost/duration.

3. **S3‑E2‑S3: Deterministic idempotency for AITask**

   * **AC:**

     * Replays with same args return the same stored result (cache by `(TenantId, InstanceId, NodeId, ArgsHash)`).
   * **Tasks:**

     * Add `AiResultCache`; guard with TTL option; tests for cache hits/misses.

---

### Epic S3‑E3: ActionKinds — Files/DMS + Security + Retention

**Goal:** Round out file handling, DMS filing, and data‑governance actions.

#### Stories

1. **S3‑E3‑S1: File/DMS actions**
   **Kinds:** `files.upload`, `files.copy`, `doc.fileToDMS`.
   **AC:** Upload/copy across storages (S3/Blob/FileService); file to DMS (SharePoint/OpenText) with metadata; emits `MessageOut` upon success.
   **Tasks:** FileService/DMS adapters; large file chunking; tests.

2. **S3‑E3‑S2: Security actions**
   **Kinds:** `security.grant`, `security.revoke`.
   **AC:** Row/document‑level rights applied with optional TTL; audited via `audit.append`.
   **Tasks:** SecurityService adapter; TTL scheduler; tests.

3. **S3‑E3‑S3: Governance actions**
   **Kinds:** `retention.apply`, plus E‑Sign extras `esign.void`, `esign.remind`.
   **AC:** Retention policy attached to records; envelopes can be voided or reminded; compensators wired (e.g., grant↔revoke).
   **Tasks:** Retention adapter; compensator registry wiring; tests.

---

## Cross‑Cutting Work

### CC‑1: DSL & Validation updates

* Update `definitions.schema.json` and `rules.schema.json`.
* Expand `GraphValidationService` to understand new nodes and boundary events.
* Publish‑time validation hooks for: SLA references, ruleset references, events/correlation lists.

### CC‑2: Outbox & Deterministic Keys

* All new emitted events (`workflow.notification.requested`, `workflow.instance.resumed`, `workflow.child.spawned`, etc.) use `DeterministicOutboxKey` helpers.
* Tests: unique on `(TenantId, IdempotencyKey)` holds under concurrency.

### CC‑3: Persistence & Migrations

* Tables: `TaskSla`, `EventWaitIndex`, `ReceivedEvents`, `ChildCompletionIndex`, `AiInvocationLog`, `AiResultCache`.
* Add CHECK/constraints to bind TenantId (Postgres `current_setting('app.tenant_id')`).

### CC‑4: Public API surface (minimal)

* `POST /wf/instances/{id}/signal` (Message In)
* `GET /wf/tasks/{taskId}` → include SLA status
* `GET /wf/instances/{id}` → include waits/children summary

### CC‑5: Samples & Docs

* `/samples/definitions/*.json` for: Approval Flow, E‑sign Wait, PBC Tracking, Public Notice.
* `/docs/*` for: SLA, Decision Table, Events/Correlation, AITask guardrails.

### CC‑6: ActionKinds Catalog & Discovery

* **Catalog doc:** `/docs/actionkinds.md` enumerating kinds, args schema, compensators, and sample JSON.
* **Discovery API (optional):** `GET /wf/actionkinds` returns registry for tooling.
* **Telemetry:** per‑kind latency/success metrics; top errors by kind; per‑tenant usage counters.

---

## Definition & Snippet Library (ready to paste)

### SLA policy example

```json
{
  "policies": {
    "sla": [
      { "id": "std.approval", "name": "Standard Approval", "dueRule": {"businessDays": 3},
        "escalations": [ {"after": {"businessDays": 2}, "to": "manager", "notifyTemplateId": "tmpl.escalation"} ] }
    ]
  }
}
```

### Notification node example

```json
{ "id": "escalate", "type": "Notification", "templateId": "tmpl.reminder", "channel": "email", "targets": ["${managerEmail}"], "next": "tHuman" }
```

### Decision Table node example

```json
{ "id": "dt1", "type": "DecisionTable", "rulesetId": "rules.approvalRouting",
  "outputs": {"approverRole": "string", "path": "string"}, "next": "gt1" }
```

### Wait for Condition (event) example

```json
{ "id": "waitSig", "type": "WaitForCondition", "predicate": { "kind": "event", "name": "EsignEnvelopeCompleted",
  "correlation": ["tenantId", "entityType", "entityId"] }, "next": "End" }
```

### CallActivity example

```json
{ "id": "callApproval", "type": "CallActivity", "workflowId": "wf.standard.approval.v1", "input": {"entityId": "${id}"}, "wait": "all" }
```

### AITask example

```json
{ "type": "AITask", "promptId": "ai.proposal.section.v2", "inputs": { "rfpTextRef": "$.rfp.text", "section": "Technical Approach" },
  "schemaId": "schema.proposal.section", "guardrails": { "maxCostUsd": 0.25, "retry": {"times": 2, "backoffMs": 5000}, "provider": "OpenAI:gpt-5" } }
```

---

## Test Plan (xUnit high‑level)

* **Tenant Isolation:**

  * Cross‑tenant events/SLAs cannot affect others; verify with `DefinitionServiceBuilder` and per‑tenant DbContexts.
* **Outbox Idempotency:**

  * Concurrency test inserts for the same notification event → single outbox row.
* **Validation:**

  * Publish fails with unknown `slaPolicyId`/`rulesetId`/event name.
* **Wait/Resume:**

  * Event arrival resumes exactly one waiting token; duplicates ignored.
* **AI Guardrails:**

  * Exceeding cost cap aborts node and triggers `Error` boundary; schema mismatch raises validation error.

---

## Done Criteria (per sprint)

* All stories green; migrations applied; sample definitions run in an end‑to‑end smoke test.
* Metrics visible for SLA and AITask; logs show deterministic keys for new events.
* Docs updated; examples copy‑pastable.

---

**End of Plan**
