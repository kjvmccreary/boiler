1) Remaining MVP Checklist Items
From the original workflow/master-mvp-checklist.md, here is what is (based on current code) DONE vs OUTSTANDING / PARTIAL:
Completed (or sufficiently in place):
•	Core entities: Definition, Instance, Task, Event.
•	JSON-first definition storage and versioning.
•	Builder (ReactFlow) with save, publish, validation stub.
•	InstancesController, TasksController, Admin pieces (retry / move exist).
•	Execution engine can advance through start → human → automatic → end.
•	Automatic node executes immediately; human tasks created with role assignment.
•	Event emission (enhanced) for path audit.
•	Read-only definition and runtime diagram with overlays.
•	Runtime snapshot endpoint.
•	Suspend / Resume / Terminate endpoints.
•	Task claiming / completion flow.
Partial / Needs attention for MVP polish:
1.	Gateway logic: Currently unconditional fan‑out (no condition evaluation). DSL has gateway.condition but execution ignores it.
2.	Timer behavior: Timer tasks are created with DueDate but no background worker to fire them (no auto-advancement). untilIso not honored; only delayMinutes (mapped from delayMinutes or DSL property).
3.	Background worker / timer polling: Missing (so timers never advance unless manually completed or you implement a job).
4.	Outbox pattern / message broker integration: You persist events, but no outbox dispatch (if that’s part of MVP “event-first” vision).
5.	Validation depth: Only basic builder checks; no reachability/ uniqueness/ required-type field validation beyond what was mentioned.
6.	Security / RBAC seed confirmation: Ensure workflow.view_instances, workflow.start_instances, workflow.manage_instances, workflow.admin are all seeded and enforced uniformly.
7.	Health check endpoint: If not present, add simple /health or ASP.NET health checks.
8.	Docker / compose updates: Ensure WorkflowService image updated after new endpoints & EF migrations.
9.	Tests update:
•	Unit tests referencing old controller constructor signatures or DTO shapes (NodeId added).
•	Snapshot / runtime diagram integration may need mocks or disabled for unit harness.
•	Timer / gateway tests currently absent.
•	Adjust tests that previously expected plain DTO responses (now ApiResponse wrappers on some routes).
10.	Definition unpublish / archive endpoints: If planned in MVP and not yet exposed in UI or validated (they appear partially stubbed).
11.	Automatic node action plug‑in architecture: Right now only “noop”; if MVP expects a placeholder for webhook or service call, add interface & safe execution wrapper.
12.	Multi-tenant enforcement: Double-check every query includes TenantId (some internal service paths do direct context queries).
13.	Consistent CurrentNodeIds format: New code serializes JSON array; legacy rows may still be CSV—parsing logic handles both (acceptable), but consider a migration or a one-time normalizer (optional).
14.	Progress calculation: Currently heuristic; optional for MVP (fine to leave).
Recommended “MVP close-out” checklist:
•	Implement conditional gateway evaluation.
•	Implement timer advancement worker.
•	Add tests for both.
•	Add a short “How to integrate a domain module” doc (see answer 3 below).
•	Ensure OpenAPI / minimal docs for consumers.
---
2) Testing Gateway & Timer Nodes (Current State and What To Add)
Current behavior in WorkflowExecutionService:
•	gateway: Traverses ALL outgoing edges immediately (no condition check).
•	timer: Creates a WorkflowTask with DueDate = now + delayMinutes; stays “Created” (or “Assigned” not relevant) and remains active node until completed—no code auto-completes it.
•	automatic: Executes inline and continues (works as expected).
Properties:
•	DSL gateway.condition: ignored.
•	DSL timer.delayMinutes used if present (delayMinutes), untilIso not used.
So today:
•	Gateway test = just ensure multiple downstream nodes become active tasks after completing prior node.
•	Timer test = confirm task row created with DueDate; must manually complete to advance.
Implementing gateway conditions (simple next step): Option: Evaluate gateway.condition as JsonLogic against the instance context JSON. Pseudo diff inside case "gateway"::
```
case "gateway":
    // If condition is empty -> fan out
    var outgoingEdges = dsl.Edges.Where(e => e.from == node.id);
    if (!string.IsNullOrWhiteSpace(node.condition))
    {
        foreach (var edge in outgoingEdges)
        {
            // Evaluate condition; optionally include edge label-specific constraints
            if (EvaluateCondition(node.condition, instance.Context))
                await TraverseAsync(dsl, instance, edge.to, active, depth + 1);
        }
    }
    else
    {
        foreach (var edge in outgoingEdges)
            await TraverseAsync(dsl, instance, edge.to, active, depth + 1);
    }
    return;
```
Implementing timer progression: Add a BackgroundHostedService (TimerWorker):
•	Query due timer tasks: Status in (Created, Assigned) AND DueDate <= UtcNow.
•	For each: mark as Completed (or a dedicated status) then call AdvanceAfterTaskCompletionAsync(task).
•	Interval: every 30–60 seconds (configurable).
•	Add concurrency guard (e.g., FOR UPDATE SKIP LOCKED in PostgreSQL or random distribution if scaling out; MVP: simple query with update).
“Until” semantics:
•	If DSL includes untilIso, compute DueDate = DateTime.Parse(untilIso) (ignore delayMinutes). Priority rule:
1.	If untilIso defined -> use that.
2.	Else if delayMinutes defined -> now + delayMinutes.
Tests to add:
•	Gateway conditional: Instance context sets a variable; definition with one true edge and one false; only true path tasks created.
•	Timer worker: Create timer node, run worker after due, assert next node tasks appear.
---
3) High-Level Integration Strategy for Other Modules (Contracts, Legal Cases, etc.)
Design principle: Keep WorkflowService “application-agnostic”; other domains integrate via:
1.	Definition authoring referencing domain concepts via context variables (e.g., contractId, caseId).
2.	Starting instances from domain modules.
3.	Responding to events or signals.
4.	Using tasks UI generically (tasks carry NodeId + InstanceId + Domain reference in Data/Context).
Core integration patterns:
A) Start workflow from domain action:
•	Contract created → call POST /api/workflow/instances with:
```
{
    "workflowDefinitionId": <id>,
    "initialContext": "{\"contractId\":123, \"initiatorUserId\":42}"
  }
```
•	Store InstanceId back in contract record for cross-linking.
B) Embed workflow state in domain UI:
•	Query runtime snapshot for instance → show diagram or statuses inline in contract detail page.
•	Filter tasks with context containing the domain ID (you may extend Task DTO with context excerpt).
C) Domain-driven signals:
•	If a workflow awaits a signal (e.g., external approval completion), domain service calls:
```
POST /api/workflow/instances/{id}/signal
  {
    "signalName":"ExternalApprovalCompleted",
    "signalData":"{\"approved\":true}"
  }
  ```
•	Execution engine can then advance nodes expecting that signal (future enhancement: signal waiting registry).
D) Automatic node actions:
•	Provide pluggable executors. Introduce IAutomaticActionExecutor keyed by node.action.kind.
•	For contract approvals, define kind = "contract:generatePdf" or "case:assignLawyer".
•	Executor loads necessary domain service via DI.
E) Listening to workflow events:
•	Implement Outbox + message broker (RabbitMQ / Kafka). Worker publishes events externally.
•	Contract module subscribes to “InstanceCompleted” for follow-up (e.g., finalize contract state).
•	Or “NodeActivated” for human tasks that need domain-specific prompts.
F) Cross-module reuse:
•	Use naming convention for definitions: contract-approval-v1, case-intake-v1.
•	Parameterize context rather than embedding domain logic in the workflow itself.
G) Securing multi-tenant multi-module:
•	Always attach tenant identifier on start / tasks queries (already enforced).
•	Domain module ensures it only references instance IDs it owns.
H) Versioning:
•	When updating definitions, keep old versions published for existing instances; new domain triggers pick latest published version automatically.
Immediate additions to enable smoother integration:
1.	Add “automatic” action dispatcher (Strategy pattern).
2.	Add optional context extract onto WorkflowTask (Task.Data could hold domain metadata; or computed view).
3.	Implement condition evaluation (gateway) so domain context variables influence path.
4.	Complete timer worker so SLAs / deadlines become meaningful.
Minimal code additions needed for integration readiness:
•	AutomaticActionExecutor pipeline.
•	JsonLogic (or CEL) evaluation helper for gateway.
•	Timer worker hosted service.
•	Outbox publisher (optional but beneficial).
---
Recommended Next Steps (Ordered)
1.	Implement TimerWorker (HostedService).
2.	Implement gateway condition evaluation (JsonLogic).
3.	Add automatic action executor registry (even if just noop + placeholder).
4.	Add gateway / timer unit tests + runtime snapshot integration tests.
5.	Update existing tests to accommodate NodeId, snapshot endpoint changes.
6.	Document integration recipe (start instance, add signals, context usage).
7.	Add small health endpoint (if missing).
8.	(Optional) Basic outbox dispatch stub.
If you want code patches for any of the above (TimerWorker, gateway conditions, automatic executor skeleton), just say which (e.g., “timer worker code” or “gateway conditions code”) and I’ll generate them.
Let me know which ones you want next.
