# Workflow Frontend Parity & Sprint Plan (Maintained Document)
Estimated % Completion: 14%  *(5 completed / 37 scoped stories)*

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
| Conditional Expressions (JsonLogic) | Supported | Monaco + semantic + variable assist (basic) | Dynamic context (S3) | Critical | [~] |
| Parallel Fan-Out | Supported | Basic visualization added | Join + advanced validation pending | Critical | [~] |
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
| JsonLogic Expression Builder (gateway/join) | Engine ready | Monaco core + semantic + variable assist baseline | Dynamic enrichment (H7-ext) | High | [x] |
| Monaco Editor Integration | N/A | MVP complete | Tests (M8) & enrichment | High | [x] |
| Monaco Telemetry & Theming | N/A | Theme selector + load/semantic telemetry | Charts/export later | High | [x] |
| Monaco Variable Assist | N/A | Basic vars + snippets + hover docs | Dynamic runtime var source | Medium | [~] |
| Monaco Frontend Tests | N/A | Missing | Fallback, race, completion depth | Medium | [ ] |
| Semantic Validation Opt-In | N/A | Toggle + status chip (badge) | Analytics drill-down later | Low | [x] |
| Monaco Bundle Optimization | N/A | Full editor & all languages bundled | JSON-only slimming (L7) | Low | [ ] |
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
| H7 | Monaco Variable Assist & Rich Docs | Variable/operator completions, snippets, markdown hover docs (baseline) |  | [~] |

### 3.3 Medium Priority
| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| M1 | Backend Tags Validation Guard | Server rejects invalid tags; UI surfaces errors |  | [ ] |
| M2 | Advanced Graph Validation (Join↔Parallel coherence) | Publish blocked on structural errors |  | [ ] |
| M3 | Simulation / Path Preview Tool | Enumerates distinct paths (static) |  | [ ] |
| M4 | Version Diff Viewer | Added/removed/changed nodes highlighted |  | [ ] |
| M5 | Task Completion Data Viewer | JSON viewer with syntax highlight |  | [ ] |
| M6 | Expression Validation Backend Tests | Unit tests: happy path, invalid JSON, disallowed op, warnings |  | [ ] |
| M7 | Timer Backend Validation Tests | Unit tests: relative vs absolute, negative/zero, past timestamp |  | [ ] |
| M8 | Monaco Frontend Test Coverage | Tests: fallback path, race guard, completion depth, semantic debounce |  | [ ] |
| H7-ext | (Follow-up) Dynamic Variable Context | Resolve runtime var list from backend context (depends S3) |  | [~] |

### 3.4 Low Priority
| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| L1 | Progress Event Debounce | Prevent redundant renders |  | [ ] |
| L2 | Timeline Filters | Client-side type/time filtering |  | [ ] |
| L3 | Outbox Message Drill-Down Prep | Placeholder link (no dispatcher) |  | [ ] |
| L4 | Join Timeout Banner | Conditional display if logic active |  | [ ] |
| L5 | Metrics Stub | Basic placeholders (instances started/sec) |  | [ ] |
| L6 | Semantic Validation Opt-In Toggle | Persisted preference (localStorage) + status chip in editor |  | [x] |
| L7 | Monaco Bundle Optimization | Reduce shipped languages & features; JSON-only + dynamic exceljs |  | [ ] |

### 3.5 Spikes
| ID | Spike | Goal | Deliverable | Status |
|----|-------|------|-------------|--------|
| S1 | Endpoint Usage Audit | Confirm in-use endpoints vs surfaced UI | Updated parity map | [ ] |
| S2 | Join/Parallel Test Seed | Provide sample definitions for QA | JSON seed file | [ ] |
| S3 | Expression Validation Contract | Decide reuse vs new endpoint | ADR mini note | [ ] |

---

## 4. Sequencing & Dependencies

1. C1 → C2 → enables C3/C4  
2. M2 waits for C2 & C4 (graph metadata coherence)  
3. H7-ext depends on S3 for final variable contract  
4. M8 after H5/H6 stable baseline  
5. L7 deferred until Monaco enrichment stabilized  

---

## 5. Risk Register

| Risk | Impact | Mitigation | Status |
|------|--------|------------|--------|
| Parallel gateway without join (dead branches) | High | Structural validator (M2) | Open |
| Complex join misconfig stalls execution | High | Pre-publish structural checks (M2) | Open |
| Expression authoring confusion | Medium | Variable assist (H7/H7-ext) | In Progress |
| Editor bundle size inflation | Medium | L7 optimization (deferred) | Open |
| Excess semantic validation calls | Medium | L6 (completed) + telemetry | Mitigated |
| Missing backend tag validation | Medium | Implement guard (M1) | Open |
| Lack of diagnostics (snapshot/events) | High | C10/C11 | Open |

---

## 6. DSL Schema Additions (Current Highlights)

| Element | Description | Status |
|---------|-------------|--------|
| gateway.strategy | 'exclusive' | 'conditional' | 'parallel' | Implemented |
| gateway.condition | JsonLogic expression (string) | Implemented |
| join.mode | all/any/count/quorum/expression | Implemented |
| join.thresholdCount / thresholdPercent | Mode parameters | Implemented |
| join.expression | JsonLogic expression (expression mode) | Implemented |
| timer.delayMinutes / delaySeconds | Relative delay | Implemented |
| timer.untilIso | Absolute ISO timestamp (exclusive) | Implemented |

---

## 7. Validation Strategy

| Rule | Layer | Enforced |
|------|-------|----------|
| Single Start node | DSL validateDefinition | Yes |
| ≥1 End node | DSL validateDefinition | Yes |
| Reachability | DSL validateDefinition | Yes |
| Gateway conditional JSON valid | DSL + Editor parse | Yes |
| Parallel min branches (warn) | DSL (warning) | Yes |
| Join mode params | DSL validateNode | Yes |
| Timer relative vs absolute exclusivity | DSL validateNode | Yes |
| Expression semantic checks | Backend endpoint | Partial |
| Parallel→Join structure | Planned (M2) | Pending |

---

## 8. UI / UX Guidelines

| Area | Guideline |
|------|-----------|
| Property Panels | Specialized editors per node; fallback text otherwise |
| Node Badges | Strategy/mode chips |
| Validation Feedback | Errors block publish; warnings inline |
| Expression Editor | Monaco lazy load + fallback |
| Variable Assist | Snippets & hover docs; low noise |
| Semantic Toggle | Global preference + per-editor badge |
| Timer UX | Clear relative vs absolute choice |
| Accessibility | aria-live status for expression validity |
| Theming | User-selectable (persisted) |

---

## 9. Permissions & Security

| Action | Permission | Notes |
|--------|------------|-------|
| Read definitions | workflow.read | Standard |
| Modify / Publish | workflow.write | Draft + publish |
| Admin ops (terminate/move/reset) | workflow.admin | Restricted |
| Instance suspend/resume | workflow.admin | C7 |

---

## 10. Telemetry (Planned / Implemented)

| Metric | Source | State |
|--------|--------|-------|
| Monaco load time | useMonacoLoader | Implemented |
| Semantic validation latency | HybridExpressionEditor | Implemented |
| Variable assist usage | Editor (future callbacks) | Planned (H7-ext) |
| Publish failure categories | Backend logs | Planned (H2) |

---

## 11. Story Progress Log

| Date | Story | Status Update | Blockers | Next Action |
|------|-------|---------------|----------|------------|
| 2025-09-08 | L6 | Badge added to editor; opt-in toggle complete | None | Telemetry correlation (optional) |
| 2025-09-08 | H7 | Variable assist baseline delivered (operators + vars + snippets) | S3 pending for dynamic context contract | H7-ext dynamic loading |
| 2025-09-08 | H7-ext | Dynamic variable fetch stub integrated (placeholder service) | Await S3 | Replace stub w/ endpoint |

(Prior entries retained above)

---

## 12. Testing Plan

| Area | Layer | Coverage Goal | Story |
|------|-------|--------------|-------|
| Variable Completion | Frontend Unit | Operators + variables appear; ordering | H7 |
| Snippet Insertion | Frontend Unit | Tab stops insert placeholders | H7 |
| Dynamic Variable Refresh | Frontend Unit | Refresh on context change | H7-ext |
| Semantic Toggle | Frontend Unit | OFF removes semantic markers | L6 |
| Monaco Editor Core | Frontend Unit | Fallback + race guard | M8 |

---

## 13. Change Log

| Date | Change | Author |
|------|--------|--------|
| 2025-09-08 | L6 refinement: semantic status chip added; marked complete | Team |
| 2025-09-08 | H7-ext dynamic vars stub added | Team |
| 2025-09-08 | Variable assist baseline (H7) added | Team |

---

## 14. Open Questions

| Question | Owner | Target |
|----------|-------|--------|
| Dynamic variable endpoint schema? | S3 | Pre H7-ext completion |
| Include user-defined vars (form payload)? | Backend/UI | With S3 |
| Parallel→Join structural enforcement severity? | Arch | M2 |

---

## 15. Upcoming (Stretch Candidates)

| Candidate | Rationale |
|-----------|-----------|
| Path simulation tool | Author confidence |
| Dynamic diff viewer | Safer iteration |
| Expression snippet gallery | Faster authoring |
| Publish dry-run | Early error surfacing |

---

## 16. Decisions (Summary)

| Decision | Date | Notes |
|----------|------|-------|
| Badge for semantic status in editor | 2025-09-08 | Improves transparency |
| Stub dynamic variable fetch (front-end) | 2025-09-08 | Enables early UX |
| Defer bundle optimization | 2025-09-08 | Focus on parity first |

---

## 17. Glossary

| Term | Meaning |
|------|--------|
| Semantic Validation | Backend logic/structure checks |
| Variable Assist | Auto-complete + docs for context vars |
| H7-ext | Dynamic extension of variable assist |

---

(End of Document)
