# Workflow Frontend Parity & Sprint Plan (Maintained Document)
Estimated % Completion: 11%  *(4 completed / 37 scoped stories)*

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
| JsonLogic Expression Builder (gateway/join) | Engine ready | Monaco core complete (parse, semantic, format, fallback) + var assist hook | Variable enrichment (H7/S3) | High | [x] |
| Monaco Editor Integration | N/A | MVP complete | Tests (M8) & variable assist expansion | High | [x] |
| Monaco Telemetry & Theming | N/A | Theme selector + load/semantic telemetry implemented | Additional charts/export later | High | [x] |
| Monaco Variable Assist | N/A | Basic variable list & snippets integrated | Dynamic enrichment & docs | Medium | [~] |
| Monaco Frontend Tests | N/A | Missing | Fallback, race, completion depth | Medium | [ ] |
| Semantic Validation Opt-In | N/A | Toggle implemented (UI + persistence) | Badge / analytics refinement | Low | [~] |
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
| H7-ext | (Follow-up) Dynamic Variable Context | Resolve runtime var list from backend context (depends S3) |  | [ ] |

### 3.4 Low Priority
| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| L1 | Progress Event Debounce | Prevent redundant renders |  | [ ] |
| L2 | Timeline Filters | Client-side type/time filtering |  | [ ] |
| L3 | Outbox Message Drill-Down Prep | Placeholder link (no dispatcher) |  | [ ] |
| L4 | Join Timeout Banner | Conditional display if logic active |  | [ ] |
| L5 | Metrics Stub | Basic placeholders (instances started/sec) |  | [ ] |
| L6 | Semantic Validation Opt-In Toggle | Persisted preference (localStorage) gating semantic calls |  | [~] |
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
3. H7 variable assist extension depends on S3 (runtime context contract)  
4. M8 after H5/H6 (editor stable)  
5. L7 can defer until Monaco polish complete  
6. H7-ext scheduled after S3 & baseline H7 sign-off  

---

## 5. Risk Register

| Risk | Impact | Mitigation | Status |
|------|--------|------------|--------|
| Parallel gateway without join (dead branches) | High | Structural validator (M2) | Open |
| Complex join misconfig stalls execution | High | Pre-publish structural checks (M2) | Open |
| Expression authoring confusion | Medium | Variable assist (H7), examples | In Progress |
| Editor bundle size inflation | Medium | L7 optimization (deferred) | Open |
| Excess semantic validation calls | Medium | Opt-in toggle (L6) + telemetry (H6) | In Progress |
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
| join.expression | JsonLogic expression for expression mode | Implemented |
| timer.delayMinutes / delaySeconds | Relative delay | Implemented |
| timer.untilIso | Absolute ISO timestamp (exclusive) | Implemented |
| node.properties (compat) | Transitional metadata store | Supported |

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
| Tags validity | Client only (normalize) | Partial |

---

## 8. UI / UX Guidelines

| Area | Guideline |
|------|-----------|
| Property Panels | Specialized editors per node; fallback text otherwise |
| Node Badges | Strategy/mode chips for clarity |
| Validation Feedback | Errors block publish; warnings inline |
| Expression Editor | Lazy Monaco, fallback safe |
| Variable Assist | Predictable, non-intrusive suggestions |
| Timer UX | Clear mode switch (relative vs absolute) |
| Accessibility | aria-live status + keyboard focus retention |
| Theming | User-selectable (light/dark/system/hc) persisted |

---

## 9. Permissions & Security

| Action | Permission | Notes |
|--------|------------|-------|
| Read definitions | workflow.read | Standard |
| Modify / Publish | workflow.write | Draft & publish |
| Admin ops (terminate/move/reset) | workflow.admin | Restricted UI |
| Task reset | workflow.admin | C6 |
| Instance suspend/resume | workflow.admin | C7 |

---

## 10. Telemetry (Planned / Implemented)

| Metric | Source | State |
|--------|--------|-------|
| Monaco load time | useMonacoLoader | Implemented |
| Semantic validation latency | HybridExpressionEditor | Implemented |
| Variable assist usage (accept rate) | (Future) Editor callbacks | Planned (H7-ext) |
| Publish failure categories | Backend logs | Planned (H2) |
| Timer scheduling anomalies | Timer worker | Planned (M7) |
| Structural validation failures | Validation service | Planned (M2) |

---

## 11. Story Progress Log

| Date | Story | Status Update | Blockers | Next Action |
|------|-------|---------------|----------|------------|
| 2025-09-08 | C1 | Strategy selector + panel committed; build fix | None | H1 UX refinements |
| 2025-09-08 | C2 | Parallel visualization & warnings implemented | Await structural validator | M2 planning |
| 2025-09-08 | C3 | Base join node completed | None | C4 |
| 2025-09-08 | C4 | Join config panel (modes/thresholds/expression) added | Structural checks pending | M2 |
| 2025-09-08 | C5 | Timer panel (relative/absolute) + validation added | None | Edge case tests (M7) |
| 2025-09-08 | H1 | Semantic validation endpoint integrated | None | H5 polish |
| 2025-09-08 | H5 | Monaco MVP complete (format, fallback, a11y, race guard) | None | H6 |
| 2025-09-08 | H6 | Theme selector + telemetry integrated | None | H7 |
| 2025-09-08 | L6 | Semantic opt-in toggle implemented | None | Badge / analytics refinement |
| 2025-09-08 | H7 | Variable assist baseline (vars + snippets + hover docs) | Needs S3 decision for dynamic vars | Add dynamic context (H7-ext) |
| 2025-09-08 | L7 | Added optimization story (bundle slimming deferred) | Await focus | Schedule post-core parity |

---

## 12. Testing Plan

| Area | Layer | Coverage Goal | Story |
|------|-------|--------------|-------|
| Expression Validation Service | Backend Unit | JSON errors, disallowed op, warnings, AST shape | M6 |
| Timer Validation | Backend Unit | Relative/absolute exclusivity, past, zero, negative | M7 |
| Join Mode Config | Backend Unit | Param enforcement per mode | M2 |
| Gateway Strategy Handles | Frontend Unit | Correct handles per strategy | C1/C2 |
| Parallel Branch Labeling | Frontend Unit | b1,b2… unique generation | C2 |
| Join Panel Mode Switch | Frontend Unit | Clears irrelevant fields | C4 |
| Timer Panel UI | Frontend Unit | Mode persist + validation prompts | C5 |
| Monaco Editor Core | Frontend Unit | Fallback path, race guard | M8 |
| Variable Completion | Frontend Unit | Variables appear & sort order | H7 |
| Snippet Insertion | Frontend Unit | Tab stops work for operators | H7 |
| Hover Docs | Frontend Unit | Markdown render for operators & vars | H7 |
| Semantic Toggle | Frontend Unit | OFF suppresses calls & clears markers | L6 |
| Bundle Optimization | Build Metric | Size delta pre/post (target <150kB gz editor) | L7 |

---

## 13. Change Log

| Date | Change | Author |
|------|--------|--------|
| 2025-09-08 | Added H7 variable assist (baseline) & updated matrix | Team |
| 2025-09-08 | Added L7 (bundle optimization) | Team |
| 2025-09-08 | H6 completed (telemetry + theming) | Team |
| 2025-09-08 | Opt-in toggle (L6) integrated | Team |
| 2025-09-08 | Join configuration & timer panel added | Team |

---

## 14. Open Questions

| Question | Owner | Resolution Target |
|----------|-------|-------------------|
| Dynamic variable context endpoint shape? | S3 | Before H7-ext |
| Enforce parallel→join pre-publish or warn only? | Arch | M2 |
| Timer absolute timezone (force UTC)? | Backend | C5 finalize |
| Diff viewer approach (graph-level vs raw JSON)? | UI/Arch | Before M4 |
| Bundle optimization timing (impact on demo)? | PM | Mid-sprint review |

---

## 15. Upcoming (Stretch Candidates)

| Candidate | Rationale |
|-----------|-----------|
| Path simulation tool | Author confidence |
| Version diff viewer | Safer iteration |
| Variable autocomplete enrichment | Faster expression authoring |
| Publish dry-run | Early structural error detection |
| Inline structural fix suggestions | Reduce author friction |

---

## 16. Decisions (Summary)

| Decision | Date | Notes |
|----------|------|-------|
| Pure JsonLogic (no custom DSL) | 2025-09-08 | Simplifies validation |
| Lazy-load Monaco | 2025-09-08 | Performance |
| Timer mutual exclusivity (rel vs abs) | 2025-09-08 | Clear mental model |
| Defer bundle optimization (L7) | 2025-09-08 | Focus on parity first |

---

## 17. Glossary

| Term | Meaning |
|------|--------|
| DSL | Workflow definition JSON schema |
| JsonLogic | JSON-based rule expression grammar |
| Semantic Validation | Backend rule validation beyond JSON parse |
| Parallel Gateway | Fan-out node executing all outgoing branches |
| Join Node | Synchronization node for parallel branches |
| Variable Assist | Editor feature surfacing available context variables |

---

(End of Document)
