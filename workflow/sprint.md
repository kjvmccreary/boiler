# Workflow Frontend Parity & Sprint Plan (Maintained Document)
Estimated % Completion: 4%

> Purpose: Track alignment between backend WorkflowService capabilities and frontend implementation, and manage sprint execution.  
> Update Cadence: After each story refinement / completion.  
> Owner: Workflow Feature Team  
> Last Updated: 2025-09-08

---

## 1. Scope & Intent

This document ensures all backend workflow features (nodes, lifecycle operations, execution diagnostics, admin operations) are properly represented and operable in the frontend UI.  
Primary focus this sprint: Close critical parity gaps (Gateway strategies, Parallel + Join authoring, lifecycle/admin ops, diagnostics panels).

---

## 2. Backend → Frontend Feature Parity Matrix

Progress Legend:  
- [ ] Not Started  
- [~] In Progress  
- [x] Complete  
- [D] Deferred / Explicitly Out of Scope  

| Feature | Backend Status | Frontend Status | Gap Summary | Priority | Progress |
|---------|----------------|-----------------|-------------|----------|----------|
| Start / End Nodes | Stable | Implemented | — | Done | [x] |
| HumanTask Node | Stable | Basic node only | Missing enhanced assignment UX | High | [ ] |
| Automatic Node | Stable | Basic node | No action/webhook config panel | Medium | [ ] |
| Gateway Strategy (exclusive / conditional / parallel) | Supported | Strategy selector, panel | Expr validation pending | Critical | [~] |
| Conditional Expressions (JsonLogic) | Supported | Not editable | Editor/validation pending | Critical | [ ] |
| Parallel Fan-Out | Supported | Basic visualization added | Join + advanced validation pending | Critical | [~] |
| Join Node & Modes | Supported (all/any/count/quorum/expression) | Base + config panel + JSON parse validation | Expression semantics & structural join/parallel checks (M2) | Critical | [~] |
| Join Mode Config (threshold / expression) | Supported | Mode select + inputs + editor (JSON parse) | Advanced validation & builder UI enhancements pending | Critical | [~] |
| Timer Node (due / duration) | Supported (worker pending) | Minimal | Needs duration/datetime inputs + validation | High | [ ] |
| Task Actions: claim/complete | Implemented | Present | OK | Done | [x] |
| Task Assign | Implemented | Missing | Assign dialog absent | High | [ ] |
| Task Cancel | Implemented | Missing | No cancel action | High | [ ] |
| Task Admin Reset | Implemented | Missing | Permission-gated control absent | High | [ ] |
| Instance Suspend / Resume | Implemented | Missing | No controls | High | [ ] |
| Bulk Cancel Instances | Implemented | Missing | Multi-select + confirm | Critical | [ ] |
| Definition Terminate-Running | Implemented | Missing | Action + summary missing | Critical | [ ] |
| Definition Revalidate | Implemented | Missing | No manual validator trigger | Medium | [ ] |
| New Version (draft) | Implemented | Implicit publish only | Dedicated action button needed | Medium | [ ] |
| Runtime Snapshot (active, parallelGroups, joins) | Implemented | Missing | Diagnostics panel | Critical | [ ] |
| Event Timeline (WorkflowEvents) | Implemented | Missing | Timeline UI w/ filtering | Critical | [ ] |
| Progress Bar (dedupe) | Implemented | Partial | No context about dedupe state | Low | [ ] |
| Tags Filtering (ANY/ALL) | Implemented | Implemented | Backend validation guard missing | Medium | [ ] |
| Tags Server-Side Validation | Missing guard | Client-only | Add backend checks | Medium | [ ] |
| JsonLogic Expression Builder (gateway/join) | Engine ready | Basic text editor with JSON parse validation | Backend semantic validation endpoint pending | High | [~] |
| Join Timeout Visibility | Experimental | Missing | Display status if retained | Low | [ ] |
| Outbox Visibility | Persist only | Missing | Health widget + counts | Medium | [ ] |
| ActiveTasksCount (planned) | Pending backend enrich | Missing | Display when available / compute fallback | Medium | [ ] |
| Simulation / Dry-Run | Not implemented | Missing | Path enumeration tool | Medium | [ ] |
| Version Diff Viewer | Data available | Missing | Node-level diff view | Medium | [ ] |
| Event Stream Coalescing (UI) | Not needed now | Missing | Debounce progress events | Low | [ ] |
| Metrics (lag, SLA, durations) | Planned | Missing | Basic widget stub | Low | [ ] |

---

## 3. Sprint Story Backlog (Triaged)

### 3.1 Critical Stories

| ID | Story | Definition of Done | Owner | Status |
|----|-------|--------------------|-------|--------|
| C1 | Gateway Strategy Selector | strategy persisted (exclusive/conditional/parallel); DSL round-trips |  | [~] |
| C2 | Parallel Gateway Visualization | Parallel edges annotated; warning if no downstream join |  | [~] |
| C3 | Join Node Type (Base) | Join node palette + serialization |  | [x] |
| C4 | Join Configuration Panel | Modes + dynamic fields + validation (frontend) |  | [~] |
| C5 | Timer Node Property Panel Upgrade | Supports duration & absolute due; validation errors surfaced |  | [ ] |
| C6 | Task Extended Actions (assign/cancel/reset) | Task detail drawer + permission guard |  | [ ] |
| C7 | Instance Suspend/Resume | Buttons + state refresh |  | [ ] |
| C8 | Definition Terminate-Running Action | Modal + post-action refresh |  | [ ] |
| C9 | Bulk Cancel Instances | Multi-select + batch feedback |  | [ ] |
| C10 | Runtime Snapshot Panel | Active nodes/parallelGroups/join meta display |  | [ ] |
| C11 | Event Timeline (Instance) | Paginated, grouped, type-badged |  | [ ] |

### 3.2 High Priority (excerpt)

| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| H1 | JsonLogic Expression Builder (Gateway/Join) | Text editor + JSON validity check |  | [~] |

## 11. Story Progress Log

| Date | Story | Status Update | Blockers | Next Action |
|------|-------|---------------|----------|------------|
| 2025-09-08 | H1 | Expression editor integrated (gateway conditional & join expression); local JSON validation | No backend validator yet | Add backend validator endpoint / semantic checks |

## 13. Change Log

| Date | Change | Author |
|------|--------|--------|
| 2025-09-08 | Added ExpressionEditor, integrated with gateway & join; updated validation & H1 progress | Team |
