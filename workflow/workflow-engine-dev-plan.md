🚀 Workflow Engine Development Plan (Revised)
Guiding Principles

Microservice sibling: WorkflowService lives beside AuthService and UserService, with its own DB and API Gateway routes.

JSON-first definitions: Workflow definitions stored and versioned as immutable JSON (nodes + edges).

Small node taxonomy: Start with ~6 primitives, expand later.

Event-first: Every action emits an event (to outbox → RabbitMQ eventually).

Multi-tenant & RBAC-aware: All entities scoped by TenantId, secured by workflow.* permissions.

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

🏆 Post-MVP Enhancements (Weeks 5+)
Workflow Engine

Parallel & Join nodes

Sub-workflows (call workflow B from workflow A)

Compensation/rollback (for sagas)

Pluggable step executors (register custom node types via config).

AI & Automation

AIProcessing node (prompt → AI service, output variables).

AIReview node (AI suggests, human confirms).

AI-powered decision gateways.

Cost tracking + provider adapters (OpenAI, Anthropic, Azure).

Builder UX

Rich property panel (prompt editing, API config).

Variable management (define workflow vars, types, validation).

Version diff viewer (see changes between v1, v2).

Preview/debug mode (simulate execution).

Ops & Integration

RabbitMQ integration for outbox → event-driven comms.

SignalR/WebSockets for live task updates in UI.

Metrics: avg workflow duration, SLA misses, stuck tasks.

Migration tool: move in-flight instances from v1 → v2 of workflow definition.

🎯 Summary

MVP:

JSON-based definitions with 6 node types.

Publish/version definitions.

Start workflows, create tasks, complete tasks.

Admin “retry/move” ops.

React builder that can draft → publish definitions.

Post-MVP:

Rich step library (AI, parallel, subflows).

Advanced builder features (validation, preview, version diffs).

Event-driven with RabbitMQ, real-time updates.

Observability & SLA features.
