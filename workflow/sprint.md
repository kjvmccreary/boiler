# Workflow Frontend Parity & Sprint Plan (Maintained Document)
Estimated % Completion: 19%  *(7 completed / 37 scoped stories)*

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
| Gateway Strategy (exclusive / conditional / parallel) | Supported | Strategy selector, panel | Expr semantic enrichment (operator hints) | Critical | [~] |
| Conditional Expressions (JsonLogic) | Supported | Monaco + semantic + dynamic variables | Further runtime scope categories | Critical | [x] |
| Parallel Fan-Out | Supported | Basic visualization added | Advanced structural validation (M2) | Critical | [~] |
| Join Node & Modes | Supported | Base + config panel + semantic checks | Structural parallel→join validation (M2) | Critical | [~] |
| Join Mode Config (threshold / expression) | Supported | Mode select + inputs + Monaco/JSON | Advanced validation & builder UI enhancements pending | Critical | [~] |
| Timer Node (due / duration) | Supported (worker pending) | Panel (relative/absolute) implemented | Advanced scheduling validation & UX polish | High | [~] |
| Task Actions: claim/complete | Implemented | Present | — | Done | [x] |
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
| JsonLogic Expression Builder (gateway/join) | Engine ready | Monaco core + semantic + dynamic vars | Additional examples library | High | [x] |
| Monaco Editor Integration | N/A | MVP complete | Tests (M8) & perf slimming | High | [x] |
| Monaco Telemetry & Theming | N/A | Theme selector + load/semantic telemetry | Charts/export later | High | [x] |
| Monaco Variable Assist | N/A | Dynamic vars + snippets + markdown docs | Context categories / filtering | Medium | [x] |
| Monaco Frontend Tests | N/A | Missing | Fallback, race, completion depth | Medium | [ ] |
| Semantic Validation Opt-In | N/A | Toggle + status chip | Analytics drill-down later | Low | [x] |
| Monaco Bundle Optimization | N/A | Full editor & all languages bundled | JSON-only slimming (L7) | Low | [ ] |
| Parallel→Join Structural Validation | N/A | Implemented (branch convergence heuristics; warnings & errors) | Partial algorithm (future refinement) | Critical | [~] |
| Join Timeout Visibility | Experimental | Missing | Display status if retained | Low | [ ] |
| Outbox Visibility | Persist only | Missing | Health widget + counts | Medium | [ ] |
| ActiveTasksCount (planned) | Pending enrich | Missing | Display when available / compute fallback | Medium | [ ] |
| Simulation / Dry-Run | Not implemented | Missing | Path enumeration tool | Medium | [ ] |
| Version Diff Viewer | Data available | Missing | Node-level diff view | Medium | [ ] |
| Event Stream Coalescing (UI) | Not needed now | Missing | Debounce progress events | Low | [ ] |
| Metrics (lag, SLA, durations) | Planned | Missing | Basic widget stub | Low | [ ] |
| Expression Validation Tests (backend) | Not present | Missing | Add unit tests (M6) | Medium | [ ] |
| Timer Backend Validation Tests | Not present | Missing | Add unit tests (M7) | Medium | [ ] |

---

## 3. Sprint Story Backlog (Triaged)

### 3.1 Critical Stories
| ID | Story | Definition of Done | Owner | Status |
|----|-------|--------------------|-------|--------|
| C1 | Gateway Strategy Selector | strategy persisted (exclusive/conditional/parallel); DSL round-trips |  | [~] |
| C2 | Parallel Gateway Visualization | Parallel edges annotated; warning if no downstream join |  | [~] |
| C3 | Join Node Type (Base) | Join node palette + serialization |  | [x] |
| C4 | Join Configuration Panel | Modes + dynamic fields + validation (frontend) |  | [~] |
| C5 | Timer Node Property Panel Upgrade | Relative & absolute inputs + validation + serialization |  | [~] |
| C6 | Task Extended Actions (assign/cancel/reset) | Task detail drawer + permission guard |  | [ ] |
| C7 | Instance Suspend/Resume | Buttons + state refresh |  | [ ] |
| C8 | Definition Terminate-Running Action | Modal + post-action refresh |  | [ ] |
| C9 | Bulk Cancel Instances | Multi-select + batch feedback |  | [ ] |
| C10 | Runtime Snapshot Panel | Active nodes/parallelGroups/join meta display |  | [ ] |
| C11 | Event Timeline (Instance) | Paginated, grouped, type-badged |  | [ ] |

### 3.2 High Priority
| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| H1 | JsonLogic Expression Builder (Gateway/Join) | Text editor + JSON & semantic validation |  | [x] |
| H2 | New Draft Version & Revalidate UI | Buttons + validation result panel |  | [ ] |
| H3 | ActiveTasksCount Column/Badge | Column with fallback compute |  | [ ] |
| H4 | Health Widget (Outbox & Timers) | Shows counts & status colors |  | [ ] |
| H5 | Monaco Expression Editor Upgrade | Lazy load, completion, hover, format, fallback, a11y, race guard |  | [x] |
| H6 | Monaco Telemetry & Theming | Theme selector + load & semantic telemetry recorded |  | [x] |
| H7 | Monaco Variable Assist & Rich Docs | Dynamic vars, operator & variable snippets, markdown hover, refresh & semantic re-run |  | [x] |
| M2 | Advanced Graph Validation (Parallel↔Join) | Parallel→join structural checks (warnings/errors) integrated |  | [~] |

### 3.3 Medium Priority
| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| M1 | Backend Tags Validation Guard | Server rejects invalid tags; UI surfaces errors |  | [ ] |
| M4 | Version Diff Viewer | Added/removed/changed nodes highlighted |  | [ ] |
| M5 | Task Completion Data Viewer | JSON viewer with syntax highlight |  | [ ] |
| M6 | Expression Validation Backend Tests | Unit tests: happy path, invalid JSON, disallowed op, warnings |  | [ ] |
| M7 | Timer Backend Validation Tests | Unit tests: relative vs absolute, negative/zero, past timestamp |  | [ ] |
| M8 | Monaco Frontend Test Coverage | Tests: fallback path, race guard, completion depth, semantic debounce, dynamic vars refresh |  | [ ] |

### 3.4 Low Priority
| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| L1 | Progress Event Debounce | Prevent redundant renders |  | [ ] |
| L2 | Timeline Filters | Client-side type/time filtering |  | [ ] |
| L3 | Outbox Message Drill-Down Prep | Placeholder link (no dispatcher) |  | [ ] |
| L4 | Join Timeout Banner | Conditional display if logic active |  | [ ] |
| L5 | Metrics Stub | Basic placeholders (instances started/sec) |  | [ ] |
| L6 | Semantic Validation Opt-In Toggle | Persisted preference + status chip |  | [x] |
| L7 | Monaco Bundle Optimization | Reduce shipped languages & features; JSON-only + dynamic exceljs |  | [ ] |

### 3.5 Spikes
| ID | Spike | Goal | Deliverable | Status |
|----|-------|------|-------------|--------|
| S1 | Endpoint Usage Audit | Confirm in-use endpoints vs surfaced UI | Updated parity map | [ ] |
| S2 | Join/Parallel Test Seed | Provide sample definitions for QA | JSON seed file | [ ] |
| S3 | Expression Validation Contract | Decide reuse vs new endpoint | ADR mini note | [ ] |

---

## 4. Sequencing & Dependencies

1. C1 → C2 → C3/C4  
2. M2 (structural) refines after basic parallel/join nodes exist  
3. M8 after H7 (stable editor)  
4. L7 after Monaco usage observed  

---

## 5. Risk Register

| Risk | Impact | Mitigation | Status |
|------|--------|------------|--------|
| Parallel gateway without join | High | M2 structural validation (partial) | Mitigated (warn) |
| Join misconfiguration | High | M2 rules (incoming edges, ancestry) | In Progress |
| Large editor bundle | Medium | L7 optimization | Open |
| Missing tag validation | Medium | M1 | Open |

---

## 6. DSL Schema Additions (Highlights)
(unchanged – see prior version)

---

## 7. Validation Strategy
(unchanged – parallel→join pending M2)

---

## 8. UI / UX Guidelines
(unchanged)

---

## 9. Permissions & Security
(unchanged)

---

## 10. Telemetry
(unchanged – add variable assist metrics after instrumentation decision)

---

## 11. Story Progress Log

| Date | Story | Status Update | Blockers | Next Action |
|------|-------|---------------|----------|------------|
| 2025-09-08 | H7 | Dynamic variable context loading + semantic auto refresh completed | None | M8 tests |
| 2025-09-08 | L6 | Semantic badge + toggle complete | None | Optional analytics |
| 2025-09-08 | H6 | Telemetry baseline done | None | Potential dashboards |
| 2025-09-08 | M2 | Added structural validation: parallel→join convergence, join ancestry & in-degree checks | Heuristic only (no full dominance analysis) | Consider refinement pass |
| 2025-09-08 | H7 | Dynamic variable context loading + semantic auto refresh completed | None | M8 tests |

(Prior entries retained)

---

## 12. Testing Plan (Additions)

| Area | Layer | Coverage Goal | Story |
|------|-------|--------------|-------|
| Dynamic Var Refresh | Frontend Unit | Fetch invoked on kind/deps change; completions updated | M8 |
| Variable Fallback | Frontend Unit | Fallback list used on network error | M8 |
| Parallel→Join Validation | Frontend Unit | Cases: no join, partial merge, multiple full merges, single correct join | M2 |
| Join Ancestry Warning | Frontend Unit | Join with >=2 inputs but no upstream parallel marks warning | M2 |

---

## 13. Change Log

| Date | Change | Author |
|------|--------|--------|
| 2025-09-08 | H7 completed (dynamic vars & assist) | Team |
| 2025-09-08 | Updated completion % (7/37) | Team |
| 2025-09-08 | Added M2 structural validation heuristics | Team |

---

## 14. Open Questions

| Question | Owner | Target |
|----------|-------|--------|
| Variable categories (system/user/runtime) for grouping? | H7 follow-up | Pre M8 |
| Provide example library / snippet gallery? | Product | Post MVP |

---

## 15. Stretch Candidates
(unchanged)

---

## 16. Decisions (Summary)

| Decision | Date | Notes |
|----------|------|-------|
| Finish H7 with dynamic frontend stub | 2025-09-08 | Backend endpoint future-proofed |
| Use GET /api/workflow/expressions/variables | 2025-09-08 | Expected contract: string[] |

---

## 17. Glossary
(unchanged)

---

(End of Document)
