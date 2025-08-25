Workflow MVP – Implementation Checklist
0) Prereqs

 Confirm PostgreSQL and Redis containers are up (as in your ports panel).

 Ensure API Gateway is healthy and routing Auth/User (baseline sanity).

1) Backend service (sibling microservice)

src/services/WorkflowService/…

 Create project: WorkflowService.csproj, Program.cs, appsettings*.json.

 Add Controllers/

 DefinitionsController – draft, publish, list/get.

 InstancesController – start, get, signal/terminate.

 TasksController – list (mine/all), claim, complete.

 AdminController (protected) – retry, moveToNode.

 Add Domain/

 DSL: WorkflowDefinitionJson.cs, NodeTypes.cs (Start, End, HumanTask, Automatic, Gateway, Timer).

 Models: WorkflowDefinition, WorkflowInstance, WorkflowTask, WorkflowEvent, OutboxMessage.

 Enums: InstanceStatus, TaskStatus.

 Add Persistence/

 WorkflowDbContext.cs with jsonb mappings for definition/contexts.

 Configurations for indexes (TenantId, Instance status, Task status, DueDate).

 EF Migrations (initial).

 Add Engine/

 WorkflowRuntime (Stateless-driven transitions).

 ConditionEvaluator (JsonLogic/CEL — safe; no raw JS).

 Executors: AutomaticExecutor, HumanTaskExecutor, TimerExecutor, GatewayEvaluator.

 Add Background/

 TimerWorker (poll DB for due timers; mark fire/advance).

 Add Services/

 DefinitionService, InstanceService, TaskService, AdminService.

 EventPublisher (persist WorkflowEvent + OutboxMessage).

 Add Security/

 Policies.cs and endpoint authorization (workflow.read|write|admin).

 Health check endpoint.

(Structure mirrors your “sibling service + gateway” pattern.)

Acceptance criteria

 POST /workflow/definitions/draft stores a draft JSON.

 …/publish creates immutable version N.

 POST /workflow/instances starts instance and positions at Start.

 GET /workflow/tasks?mine=true returns tasks scoped by TenantId.

 POST /workflow/tasks/{id}:complete advances along Gateway rules.

 TimerWorker wakes due timers and advances instances.

 Events written to WorkflowEvent + OutboxMessage.

2) Shared libraries (DTOs & Contracts)

src/shared/DTOs/Workflow/…
src/shared/Contracts/Workflow/…

 DTOs: WorkflowDefinitionDto, PublishDefinitionRequestDto, StartInstanceRequestDto, WorkflowInstanceDto, TaskSummaryDto, CompleteTaskRequestDto.

 Contracts: IWorkflowReadService (optional cross‑service reads), IWorkflowEvents (event names/payloads).

 Add permissions to seed list: workflow.read, workflow.write, workflow.admin.
(Parallels how Auth/User keep types in shared.)

Acceptance criteria

 DTOs referenced in controllers; appear in Swagger.

 Permissions enforce access (admin ops blocked without workflow.admin).

3) API Gateway

src/services/ApiGateway/ocelot.json

 Add routes:

 /workflow/definitions/* → WorkflowService:5003

 /workflow/instances/* → WorkflowService:5003

 /workflow/tasks/* → WorkflowService:5003

 Reload gateway; verify reachability.
(Consistent with your gateway + services layout and ports table.)

Acceptance criteria

 GET /workflow/definitions via gateway returns 200.

4) Docker & env

docker/services/WorkflowService.Dockerfile
docker/docker-compose.yml

 Add service workflowservice mapping 5003 (dev) / 7003 (https).

 Env: DB connection string, JWT authority (AuthService), Redis (optional).

 Add healthcheck.
(Aligns to your compose + services pattern and the existing infra panel.)

Acceptance criteria

 docker compose up brings WorkflowService healthy.

 Gateway resolves /workflow/* to container.

5) Frontend (thin stubs to exercise APIs)

frontend/react-app/src/services/workflowService.ts
frontend/react-app/src/components/workflow/…

 Service file with calls: list/publish definitions; start instance; get instance; list/claim/complete tasks.

 Components: DefinitionsTable, InstanceDetails, MyTasks (read‑only + basic actions).
(Plugs into the SPA you already have scaffolded.)

Acceptance criteria

 From UI: publish a simple definition; start an instance; complete a task.

6) Tests

tests/unit/WorkflowService.Tests/…

 WorkflowDefinitionsTests – draft→publish immutability; versioning.

 WorkflowInstancesTests – start; gateway routing; timer advance (mock clock).

 WorkflowTasksTests – list mine; claim; complete; permission checks.

 WorkflowAdminOpsTests – retry; moveToNode (guarded).
(Keep parity with your existing unit test layout.)

Acceptance criteria

 All tests pass locally; CI job green.

7) Guardrails for MVP scope

 Node set limited to: Start, End, HumanTask, Automatic, Gateway(exclusive), Timer.

 Definitions stored as immutable JSON (versioned).

 Conditions via JsonLogic/CEL (no arbitrary JS).

 Timers via DB‑polled TimerWorker (messaging later).

 Events persisted (+ outbox), even if only logged for now.

 RBAC: workflow.* permissions enforced at controller level.

8) Quick “smoke flow” you can run Monday

 Create Definition v1 (Start → HumanTask → Gateway[approve/deny] → End).

 Publish v1; start instance; confirm Task appears in “My Tasks”.

 Complete Task with approve=true; instance ends at “Approved End”.

 Add a Timer node; set due+5m; verify TimerWorker advances.
