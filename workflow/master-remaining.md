# 4. Recommended Next Sprint (Priority Order)
### 1. TimerWorker Implementation
* <s>Hosted service every 30–60s: ```SELECT``` due timer tasks (Status in Created/Assigned, DueDate ≤ now, NodeType timer).</s>
* <s>Complete them via ```runtime.CompleteTaskAsync(autoCommit:false)``` in batches; single UoW commit.</s>
* <s>Add concurrency guard (```SKIP LOCKED``` or update status to ```InProgressTimer``` before processing).</s>
### 2. Graph Validation Service
* <s>On publish: assert exactly one Start, ≥1 End, all nodes reachable from Start, no unreachable End, no duplicate IDs, no isolated islands. Present any exceptions / errors in UI friendly manner to user.</s>
### 3. Outbox Dispatcher (MVP)
* <s>Hosted worker polls unprocessed messages, logs or POSTs to a placeholder endpoint, sets ```IsProcessed + ProcessedAt/RetryCount```.</s>
* <s>Add ```IdempotencyKey (Guid)``` + unique index to ```OutboxMessage```.</s>
### 4. Automated Tests
* <s>```GatewayCondition_ShouldSelectTruePathOnly```.</s>
* <s>```TimerWorker_ShouldAdvanceAfterDue```.</s>
* <s>```Runtime_SaveChanges_BatchCount_ShouldEqual1 for claim/complete/start```.</s>
* <s>```Validation_ShouldRejectMultipleStartNodes```.</s>
### 5. RBAC & Permissions Audit
* <s>Ensure seed contains: ```workflow.view_tasks```, ```workflow.claim_tasks```, ```workflow.complete_tasks```, ```workflow.admin```, ```workflow.view_instances```, ```workflow.start_instances```.</s>
* <s>Map each controller action to a permission doc.</s>
### 6. Automatic Action Executor Abstraction
* <s>Interface + registry keyed by ```action.kind```.</s>
* <s>Implement noop + future webhook placeholder.</s>
* <s>Guard with try/catch + failure event emission.</s>
### 7. Definition Immutability Enforcement
* <s>Block updates to ```JSONDefinition``` after publish (require new version).
* Disallow unpublish if active running instances (or add rule).
### 8. Tenant Audit & Guard Tests
* Unit/integration test that cross-tenant tasks / instances are not retrievable.</s>
### 9. Outbox Idempotency & Schema Migration
* Add columns: ```IdempotencyKey (uuid)```, ```ProcessedAt (timestamp)```, ```Error (text)```.
* Add unique index ```(TenantId, IdempotencyKey)```.
### 10. Documentation & Developer Onboarding
* “How to start a workflow from another service” one-pager.
* Example conditional gateway definition JSON.

# 5. Quick Wins (Low Effort, High Clarity)
* Add logging prefix TIMER_WORKER / OUTBOX_WORKER for future operations.
* Add HealthCheck tag grouping (e.g., readiness vs liveness).
* Add simple /api/workflow/definitions/{id}/validate endpoint returning the new graph validator output before publish.
# 6. Suggested Metrics (Post-Refactor)

| Metric                                       | Target     |
| -------------------------------------------- | ------------- |
| SaveChanges per Claim/Complete	           | 1          |
| Timer cycle latency                          |	< 60s (configurable) |
| Gateway evaluation error fallback rate | Near 0  |
| Outbox processing lag | < 2 minutes in MVP |
| Validation failures at publish | Surfaced with actionable error list |

# 7. Proposed Branching Strategy (Optional)
* ```feature/timer-worker```
* ```feature/graph-validation```
* ```feature/outbox-dispatch```
* ```feature/auto-action-registry```


# 1. Checklist Alignment (master-mvp-checklist.md)
| Section      | Item                                                  | Status                | Notes                                                                                              |
| ------------ | ----------------------------------------------------- | --------------------- | -------------------------------------------------------------------------------------------------- |
| 0 Prereq     | DB, RBAC, Auth                                        | Partial               | PostgreSQL in place; Redis not used yet (if required).                                             |
| 1 Backend    | Project scaffolding                                   | Done                  | Service, config, DI all present.                                                                   |
| 1 Backend    | Controllers (Definitions / Instances / Tasks / Admin) | Done                  | Functional; some admin edges (moveToNode validation depth) still light.                            |
| 1 Backend    | Domain Models & DSL                                   | Done                  | Versioned definitions, nodes, events, tasks, outbox.                                               |
| 1 Backend    | Runtime Engine                                        | Partial → Improved    | Conditional gateway logic now implemented; timer auto-advance missing.                             |
| 1 Backend    | Executors (Human, Automatic, Gateway, Timer)          | Partial               | Timer executor creates task; no automatic firing worker. Automatic executor is minimal.            |
| 1 Backend    | Background TimerWorker                                | Missing               | Required for SLA/timer progression.                                                                |
| 1 Backend    | Services Layer                                        | Done                  | Definition / Instance / Task / Admin / EventPublisher in place.                                    |
| 1 Backend    | Security Policies                                     | Done                  | Permission attributes applied. Need final seed audit.                                              |
| 1 Backend    | Health Checks                                         | Done                  | Health endpoints mapped in Program.cs.                                                             |
| 2 Shared     | DTOs                                                  | Done                  | Wide DTO coverage; some may be broader than MVP.                                                   |
| 2 Shared     | Contracts                                             | Partial               | Only internal runtime + event publisher; optional external contracts not formalized.               |
| 2 Shared     | RBAC Seed (workflow.\*)                               | Partial               | Code assumes fine-grained perms (view, claim, complete, admin). Confirm seed script.               |
| 3 Gateway    | Routes                                                | Assumed Done          | Not revalidated here; verify workflow routes in ocelot.json.                                       |
| 4 Docker     | Dockerfile / Compose                                  | Partial               | Not shown; ensure image, ports (5003/7003), healthcheck, migration on startup.                     |
| 5 Frontend   | REST client                                           | Done                  | `workflow.service.ts` robust; status normalization present.                                        |
| 5 Frontend   | Builder (ReactFlow)                                   | Done (MVP)            | Save/load/publish minimal validation only.                                                         |
| 5 Frontend   | Task Inbox & Actions                                  | Done                  | Claim / Complete / Filters / Overdue highlighting.                                                 |
| 5 Frontend   | Timer UX                                              | Partial               | Due date rendering works; no live auto-advance.                                                    |
| 6 Events     | Event persistence                                     | Done                  | Extensive event logging + outbox rows.                                                             |
| 6 Events     | Outbox dispatch                                       | Missing               | No dispatcher/transport; logging only.                                                             |
| 6 Events     | Idempotency keys                                      | Missing               | OutboxMessage lacks unique key column.                                                             |
| 7 Guardrails | Node set limited                                      | Done                  | Only listed types used.                                                                            |
| 7 Guardrails | Definition immutability post-publish                  | Partial               | Need enforcement/hardening & tests; unpublish/archive semantics unclear.                           |
| 7 Guardrails | Safe condition evaluation                             | Done                  | JsonLogic evaluator integrated; defensive error fallback = true.                                   |
| 7 Guardrails | Timer via DB polling                                  | Missing               | Worker absent.                                                                                     |
| 7 Guardrails | Multi-tenant enforcement                              | Partial               | Most queries filter; audit internal runtime queries & event creation paths for tenant correctness. |
| 7 Guardrails | Admin perms enforced                                  | Done                  | `workflow.admin` decorated endpoints.                                                              |
| 8 Acceptance | Full publish/start/complete flow                      | Partial → Mostly Done | All but auto timer advancement & deeper validation.                                                |
| 8 Acceptance | Timer auto-fire                                       | Missing               | Requires TimerWorker.                                                                              |
| 8 Acceptance | Outbox event chain                                    | Partial               | Persisted only; no external emission.                                                              |
| 9 Smoke Flow | Conditional gateway path                              | Done                  | Condition evaluation now functional.                                                               |
| 9 Smoke Flow | Timer branch                                          | Partial               | Must manually complete timer task today.                                                           |

# 2. “Remaining” Document Items (master-remaining.md) — Updated Status
| Item                              | Original Status          | Current Status   | Notes                                                                                                                             |
| --------------------------------- | ------------------------ | ---------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| 1 Gateway logic                   | Missing                  | Done             | Conditional selection implemented with evaluation + edge label inference.                                                         |
| 2 Timer behavior                  | Missing                  | Still Missing    | Need worker to auto-complete due timer tasks & advance.                                                                           |
| 3 Timer worker                    | Missing                  | Missing          | Add hosted background service (db poll).                                                                                          |
| 4 Outbox dispatch                 | Missing                  | Missing          | Add dispatcher + processed flag updates + optional integration stub.                                                              |
| 5 Validation depth                | Missing                  | Missing          | Implement graph validator (reachability, single start, at least one end, orphan edges, duplicate IDs).                            |
| 6 RBAC seed completeness          | Unverified               | Still Unverified | Enumerate actual seeded claims vs required: view\_tasks, claim\_tasks, complete\_tasks, admin, view\_instances, start\_instances. |
| 7 Health check endpoint           | Potentially Missing      | Done             | Present in Program.cs.                                                                                                            |
| 8 Docker updates                  | Unverified               | Still Unverified | Confirm presence of service in compose, migrations on container start.                                                            |
| 9 Tests update                    | Missing                  | Missing          | Need gateway condition test, timer worker test, runtime snapshot test, UoW save-count tests.                                      |
| 10 Unpublish / archive endpoints  | Partially stubbed        | Partial          | Backend appears; front-end wiring & rules not enforced.                                                                           |
| 11 Automatic action plugin        | Missing                  | Missing          | Create `IAutomaticActionExecutor` registry; support e.g. webhook/noop.                                                            |
| 12 Multi-tenant enforcement audit | Needed                   | Still Needed     | Add guard tests verifying tenant isolation & injection.                                                                           |
| 13 CurrentNodeIds consistency     | Optional                 | Optional         | Legacy CSV normalization migration optional; logic tolerant.                                                                      |
| 14 Progress calculation           | Heuristic                | Acceptable       | Can defer; ensure it's not used for core logic.                                                                                   |
| Added Since Doc                   | SignalR real-time counts | New              | Implemented (Task bell).                                                                                                          |
| Added Since Doc                   | UoW / Save batching      | New              | Implemented; runtime autoCommit flag + TaskService defers commit to controller.                                                   |

# 3. Technical Risk and Debt Hotspots
| Area                                | Risk                                     | Impact                                         |
| ----------------------------------- | ---------------------------------------- | ---------------------------------------------- |
| Timer automation                    | Users must manually complete timer tasks | Incorrect SLA timing & stalled process paths   |
| Validation                          | Invalid/degenerate graphs could publish  | Runtime errors / orphan tasks                  |
| Outbox dispatch absent              | No external event integration            | Limits cross-service orchestration             |
| RBAC seed audit                     | Silent permission mismatches             | Unexpected access denials or overexposure      |
| Lack of tests (gateway/timer/UoW)   | Regressions undetected                   | Stability risk during future changes           |
| Definition immutability enforcement | Post-publish mutation risk               | Historical audit & reproducibility compromised |
| Automatic executor extensibility    | Tight coupling of “automatic” behavior   | Hard to add domain actions later               |
| Tenant enforcement audit            | Potential data leakage vector            | Security / compliance issue                    |


Integrate sequentially to reduce merge complexity.
***
## TL;DR

Core workflow< (definitions, instances, human & automatic tasks, conditional gateways, real-time task notifications, consolidated persistence) is solid.
Major remaining MVP blockers per the original specification are: timer automation, stronger definition validation, and minimal outbox dispatch.

Add tests and RBAC/immutability hardening next to stabilize before expanding feature breadth.

Specify which item you want first (e.g., “timer worker code” or “graph validation code”) and I’ll produce full implementation files.
