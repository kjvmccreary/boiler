🥇 MVP Phase (Weeks 1–4)
1. Entities & Storage

WorkflowDefinition

TenantId, Name, Version, JSONDefinition, IsPublished, CreatedAt

WorkflowInstance

TenantId, DefinitionId+Version, Status, CurrentNodeIds[], Context (JSON), StartedAt, CompletedAt

WorkflowTask

InstanceId, NodeId, AssignedToUser/Role, Status, DueDate, Data (JSON)

WorkflowEvent

InstanceId, Type, Name, Data (JSON), OccurredAt

👉 Store definitions as immutable JSON (nodes + edges) instead of normalizing steps/transitions.

2. Supported Node Types (MVP)

Start

End

HumanTask (manual, approval)

Automatic (system call / script placeholder)

Gateway (exclusive branch)

Timer (delay until X or after Y minutes)

(Everything else — AI, loops, parallel joins — postponed until post-MVP.)

3. API Surface

Definitions

POST /workflow/definitions/draft

POST /workflow/definitions/{id}/publish

GET /workflow/definitions

Instances

POST /workflow/instances (start)

GET /workflow/instances/{id}

POST /workflow/instances/{id}:signal (advance external event)

Tasks

GET /workflow/tasks?mine=...

POST /workflow/tasks/{id}:claim

POST /workflow/tasks/{id}:complete

Admin (protected by workflow.admin)

POST /workflow/instances/{id}:retry

POST /workflow/instances/{id}:moveToNode

4. Execution Model

State machine via Stateless — transitions evaluated inside engine.

Condition language: JsonLogic (safe, lightweight) for transitions.

Timers: store due dates, background worker checks them; in MVP can just poll DB.

5. Events & Outbox

Every action writes to WorkflowEvent and Outbox table.

MVP: outbox just logs → console.

Post-MVP: RabbitMQ consumer(s) for async propagation.

6. React Builder (Basic)

Use ReactFlow with:

Sidebar with Start, End, Human, Automatic, Gateway, Timer.

Canvas to connect nodes, edit node properties.

Save/export to JSON → stored as WorkflowDefinition.

Keep validation minimal: must have Start and End, all nodes reachable.


MVP scope guardrails (so you stay fast)

Node set (MVP): Start, End, HumanTask, Automatic, Gateway (exclusive), Timer.

Definitions: versioned & immutable once published (JSON blob, jsonb column).

Conditions: safe evaluator (JsonLogic/CEL), not raw JS.

Timers: DB‑polled TimerWorker (RabbitMQ comes post‑MVP).

Events: persist to WorkflowEvent + OutboxMessage (even if you only log them now).

Security: add workflow.read, workflow.write, workflow.admin to your RBAC seed and protect AdminController accordingly, staying consistent with your RBAC design in the system spec
