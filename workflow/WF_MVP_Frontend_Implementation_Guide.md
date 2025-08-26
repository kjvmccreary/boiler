# Workflow MVP Frontend Implementation Guide

*A detailed, step‑by‑step plan to implement the Workflow (WF) MVP UI with React, TypeScript, MUI Pro, and ReactFlow. Use this as an in‑IDE “source of truth” while you build.*

---

## 1) Scope & Success Criteria (MVP)
**Goal:** Author, publish, and run simple workflows using a basic builder and operational screens.

**What ships in MVP UI**
- **Builder** (ReactFlow): sidebar palette, canvas, property panel, connectable edges, save/load JSON DSL, publish validation.
- **Operations**: Definitions list (publish/draft), Instance details, My Tasks (list/claim/complete), minimal toasts and error handling.
- **Security**: Honor `workflow.read|write|admin` (hide/disable UI appropriately).
- **Reliability**: Client‑side validation before publish; graceful network error states.

**Out of scope (post‑MVP)**: simulation, version diff, parallel/join nodes, AI nodes, websockets, RabbitMQ live updates.

---

## 2) Frontend Folder Structure
```
frontend/react-app/src/
  services/
    workflowService.ts           // REST client wrappers
  features/workflow/
    builder/
      BuilderPage.tsx
      BuilderCanvas.tsx
      NodePalette.tsx
      PropertyPanel.tsx
      dsl/
        dsl.types.ts
        dsl.serialize.ts
        dsl.deserialize.ts
        dsl.validate.ts
    definitions/
      DefinitionsPage.tsx
    instances/
      InstanceDetailsPage.tsx
    tasks/
      MyTasksPage.tsx
    components/
      FormField.tsx
      ConfirmDialog.tsx
    hooks/
      usePermissions.ts
      useToast.ts
  routes/
    workflow.routes.tsx
```

---

## 3) Shared Types (DSL & DTOs)
**DSL Types** (`dsl.types.ts`):
```ts
export type NodeType = 'start'|'end'|'humanTask'|'automatic'|'gateway'|'timer';
export type NodeId = string;

export interface DslNodeBase { id: NodeId; type: NodeType; label?: string; x: number; y: number; }
export interface StartNode extends DslNodeBase { type: 'start'; }
export interface EndNode extends DslNodeBase { type: 'end'; }
export interface HumanTaskNode extends DslNodeBase {
  type: 'humanTask'; assigneeRoles?: string[]; dueInMinutes?: number; formSchema?: unknown;
}
export interface AutomaticNode extends DslNodeBase { type: 'automatic'; action?: { kind: 'webhook'|'noop'; config?: Record<string, unknown> } }
export interface GatewayNode extends DslNodeBase { type: 'gateway'; condition: string; }
export interface TimerNode extends DslNodeBase { type: 'timer'; delayMinutes?: number; untilIso?: string; }

export type DslNode = StartNode|EndNode|HumanTaskNode|AutomaticNode|GatewayNode|TimerNode;
export interface DslEdge { id: string; from: NodeId; to: NodeId; label?: string; }
export interface DslDefinition { key: string; version?: number; nodes: DslNode[]; edges: DslEdge[]; }
```

**DTO Types** (client mirrors server contracts):
```ts
export interface WorkflowDefinitionDto { id: string; key: string; version: number; status: 'Draft'|'Published'; json: DslDefinition; createdAt: string; }
export interface PublishDefinitionRequestDto { definitionId: string; }
export interface StartInstanceRequestDto { definitionKey: string; definitionVersion?: number; input?: Record<string, unknown>; }
export interface WorkflowInstanceDto { id: string; definitionKey: string; definitionVersion: number; status: 'Running'|'Completed'|'Terminated'; currentNodeIds: string[]; startedAt: string; completedAt?: string; }
export interface TaskSummaryDto { id: string; instanceId: string; nodeId: string; label?: string; status: 'Open'|'Claimed'|'Completed'; dueDate?: string; }
export interface CompleteTaskRequestDto { outcome?: Record<string, unknown>; }
```

---

## 4) API Client – `workflowService.ts`
Wrap REST endpoints:
- **Definitions**: draft (POST), list (GET), publish (POST)
- **Instances**: start (POST), get by id (GET)
- **Tasks**: list mine (GET), claim (POST), complete (POST)

Include tenant headers & auth, throw typed errors, surface via toasts.

---

## 5) Builder UI – ReactFlow Setup
**BuilderCanvas.tsx**
- Initialize ReactFlow with `nodes`, `edges`, `onNodesChange`, `onEdgesChange`, `onConnect`.
- Enable snapToGrid, fitView.

**NodePalette.tsx**
- Sidebar with draggable node types.

**PropertyPanel.tsx**
- Right drawer editing node props:
  - humanTask: label, roles, due date
  - gateway: condition (JsonLogic)
  - timer: delay/until
  - automatic: action kind/config

**Serialization/Validation**
- serialize(): ReactFlow → DSL JSON
- deserialize(): DSL JSON → ReactFlow
- validate(): check Start/End/Reachability, required props.

**Publish Flow**
- serialize → validate → POST publish

---

## 6) Operations Screens
**DefinitionsPage.tsx**
- List definitions, status, publish/start actions.

**InstanceDetailsPage.tsx**
- Show instance metadata, timeline/events, current node(s).

**MyTasksPage.tsx**
- List tasks, claim/complete actions.

---

## 7) Permissions & Routing
- Hook `usePermissions.ts` for workflow.read/write/admin checks.
- Guard routes in `workflow.routes.tsx`; hide unauthorized buttons.

---

## 8) UX & A11y
- MUI Pro components for inputs, dialogs, datagrid.
- Keyboard shortcuts (delete node, save, publish).
- Toasts/snackbars for feedback.

---

## 9) Testing Strategy
- Unit: serialize/deserialize, validate.
- Component: BuilderCanvas edge creation, PropertyPanel updates, MyTasks claim/complete.
- Manual E2E: author simple approval workflow → publish → start → complete.

---

## 10) Minimal DSL Example
```json
{
  "key": "approval",
  "nodes": [
    { "id": "n1", "type": "start", "label": "Start", "x": 80, "y": 120 },
    { "id": "n2", "type": "humanTask", "label": "Approve Request", "assigneeRoles": ["Manager"] , "x": 300, "y": 120 },
    { "id": "n3", "type": "gateway", "label": "Approved?", "condition": "{\"==\": [ {\"var\": \"approve\" }, true ]}", "x": 540, "y": 120 },
    { "id": "n4", "type": "end", "label": "Approved", "x": 780, "y": 40 },
    { "id": "n5", "type": "end", "label": "Denied", "x": 780, "y": 200 }
  ],
  "edges": [
    { "id": "e1", "from": "n1", "to": "n2" },
    { "id": "e2", "from": "n2", "to": "n3" },
    { "id": "e3", "from": "n3", "to": "n4", "label": "true" },
    { "id": "e4", "from": "n3", "to": "n5", "label": "false" }
  ]
}
```

---

## 11) Manual Acceptance
- [ ] Author above DSL in builder; save.
- [ ] Publish passes validation.
- [ ] Start instance; task appears in My Tasks.
- [ ] Complete task; instance ends in Approved.
- [ ] Timer node advances after due time.
