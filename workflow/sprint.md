# Workflow Frontend Parity & Sprint Plan (Maintained Document)

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
| Gateway Strategy (exclusive / conditional / parallel) | Supported | Strategy selector & panel added | Parallel viz (C2) & expr validation (H1) pending | Critical | [~] |
| Conditional Expressions (JsonLogic) | Supported | Not editable | No editor/validation | Critical | [ ] |
| Parallel Fan-Out | Supported | Not representable | No parallel strategy UI visualization | Critical | [ ] |
| Join Node & Modes | Supported (all/any/count/quorum/expression) | Missing | No node type or config | Critical | [ ] |
| Join Mode Config (threshold / expression) | Supported | Missing | No inputs / validation | Critical | [ ] |
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
| JsonLogic Expression Builder (gateway/join) | Engine ready | Missing | UI + backend validation endpoint usage | High | [ ] |
| Join Timeout Visibility | Experimental | Missing | Display status if retained | Low | [ ] |
| Outbox Visibility | Persist only | Missing | Health widget + counts | Medium | [ ] |
| ActiveTasksCount (planned) | Pending backend enrich | Missing | Display when available / compute fallback | Medium | [ ] |
| Simulation / Dry-Run | Not implemented | Missing | Path enumeration tool | Medium | [ ] |
| Version Diff Viewer | Data available | Missing | Node-level diff view | Medium | [ ] |
| Event Stream Coalescing (UI) | Not needed now | Missing | Debounce progress events | Low | [ ] |
| Metrics (lag, SLA, durations) | Planned | Missing | Basic widget stub | Low | [ ] |

---

## 3. Sprint Story Backlog (Triaged)

Order = execution sequence within sprint. Use IDs for tracking.

### 3.1 Critical Stories

| ID | Story | Definition of Done | Owner | Status |
|----|-------|--------------------|-------|--------|
| C1 | Gateway Strategy Selector | strategy persisted (exclusive/conditional/parallel); DSL round-trips |  | [~] |
| C2 | Parallel Gateway Visualization | Parallel edges annotated; warning if no downstream join |  | [ ] |
| C3 | Join Node Type (Base) | Join node palette + serialization |  | [ ] |
| C4 | Join Configuration Panel | Modes + dynamic fields + validation |  | [ ] |
| C5 | Timer Node Property Panel Upgrade | Supports duration & absolute due; validation errors surfaced |  | [ ] |
| C6 | Task Extended Actions (assign/cancel/reset) | Task detail drawer + permission guard |  | [ ] |
| C7 | Instance Suspend/Resume | Buttons + state refresh |  | [ ] |
| C8 | Definition Terminate-Running Action | Modal + post-action refresh |  | [ ] |
| C9 | Bulk Cancel Instances | Multi-select + batch feedback |  | [ ] |
| C10 | Runtime Snapshot Panel | Active nodes/parallelGroups/join meta display |  | [ ] |
| C11 | Event Timeline (Instance) | Paginated, grouped, type-badged |  | [ ] |

### 3.2 High Priority

| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| H1 | JsonLogic Expression Builder (Gateway/Join) | Text + inline validation |  | [ ] |
| H2 | New Draft Version & Revalidate UI | Buttons + validation result panel |  | [ ] |
| H3 | ActiveTasksCount Column/Badge | Column with fallback compute |  | [ ] |
| H4 | Health Widget (Outbox & Timers) | Shows counts & status colors |  | [ ] |

### 3.3 Medium Priority

| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| M1 | Backend Tags Validation Guard | Server rejects invalid tags; UI surfaces errors |  | [ ] |
| M2 | Advanced Graph Validation (Join↔Parallel coherence) | Publish blocked on structural errors |  | [ ] |
| M3 | Simulation / Path Preview Tool | Enumerates distinct paths (static) |  | [ ] |
| M4 | Version Diff Viewer | Added/removed/changed nodes highlighted |  | [ ] |
| M5 | Task Completion Data Viewer | JSON viewer with syntax highlight |  | [ ] |

### 3.4 Low Priority

| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| L1 | Progress Event Debounce | Prevent redundant renders |  | [ ] |
| L2 | Timeline Filters | Client-side type/time filtering |  | [ ] |
| L3 | Outbox Message Drill-Down Prep | Placeholder link (no dispatcher) |  | [ ] |
| L4 | Join Timeout Banner | Conditional display if logic active |  | [ ] |
| L5 | Metrics Stub | Basic placeholders (instances started/sec) |  | [ ] |

### 3.5 Spikes

| ID | Spike | Goal | Deliverable | Status |
|----|-------|------|-------------|--------|
| S1 | Endpoint Usage Audit | Confirm in-use endpoints vs surfaced UI | Updated parity map | [ ] |
| S2 | Join/Parallel Test Seed | Provide sample definitions for QA | JSON seed file | [ ] |
| S3 | Expression Validation Contract | Decide reuse vs new endpoint | ADR mini note | [ ] |

---

## 4. Sequencing & Dependencies

1. C1 → C2 → (enables) C3/C4 (gateway strategy before join referencing parallel origins)  
2. C5 (Timer) can proceed independently.  
3. C6–C9 (operations) share scaffolding in task & instance detail views—coordinate API service updates first.  
4. C10 + C11 require event & snapshot service wrappers (build after base instance detail scaffolding).  
5. H1 depends on minimal expression persistence from C1/C4.  
6. M2 depends on C1–C4 completion (structural graph metadata).  

---

## 5. Risk Register

| Risk | Impact | Mitigation | Owner | Status |
|------|--------|------------|-------|--------|
| DSL schema extension breaks existing drafts | High | Add migration defaults; version key in JSON |  | Open |
| Complex join misconfiguration leads to runtime stalls | High | Pre-publish validation (M2) |  | Open |
| Expression errors reduce adoption | Medium | Inline validation + examples |  | Open |
| Timeline payload size grows | Medium | Pagination + lazy load early |  | Open |
| Parallel gateway used w/o join | Medium | Warning & publish blocker if unresolved |  | Open |
| Outbox backlog undetected | Medium | Health widget (H4) |  | Open |

---

## 6. DSL Schema Additions (Proposed)
