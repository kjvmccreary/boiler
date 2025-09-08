# Workflow Frontend Parity & Sprint Plan (Maintained Document)
Estimated % Completion: 3%  *(1 completed / 32 scoped stories — recomputed after adding H5 & M7)*

> Purpose: Track alignment between backend WorkflowService capabilities and frontend implementation, and manage sprint execution.  
> Update Cadence: After each story refinement / completion.  
> Owner: Workflow Feature Team  
> Last Updated: 2025-09-08

---

## 1. Scope & Intent

This document ensures all backend workflow features (nodes, lifecycle operations, execution diagnostics, admin operations) are properly represented and operable in the frontend UI.  
Primary focus this sprint: Close critical parity gaps (Gateway strategies, Parallel + Join authoring, lifecycle/admin ops, diagnostics panels).

---

## 2. Backend → Frontend Feature Parity Matrix (Excerpt of Changed Rows Only)

| Feature | Backend Status | Frontend Status | Gap Summary | Priority | Progress |
|---------|----------------|-----------------|-------------|----------|----------|
| Timer Node (due / duration) | Supported (worker pending) | Panel (relative/absolute) implemented | Advanced scheduling validation & UX polish | High | [~] |
| JsonLogic Expression Builder (gateway/join) | Engine ready | JSON parse + backend semantic validation integrated | Monaco (H5), opt‑in toggle (L6), operator assist pending | High | [~] |
| Expression Validation Tests (backend) | Not present | Missing | Add unit tests for expression service/controller (M6) | Medium | [ ] |
| Timer Backend Validation Tests | Not present | Missing | Add unit tests for timer scheduling + edge cases (M7) | Medium | [ ] |
| Monaco Editor Integration | N/A | Not implemented | Rich editing (H5) | High | [ ] |
| Semantic Validation Opt-In | N/A | Not implemented | Toggle to reduce API calls (L6) | Low | [ ] |

(Other matrix rows unchanged.)

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
| H1 | JsonLogic Expression Builder (Gateway/Join) | Text editor + JSON & semantic validation |  | [~] |
| H2 | New Draft Version & Revalidate UI | Buttons + validation result panel |  | [ ] |
| H3 | ActiveTasksCount Column/Badge | Column with fallback compute |  | [ ] |
| H4 | Health Widget (Outbox & Timers) | Shows counts & status colors |  | [ ] |
| H5 | Monaco Expression Editor Upgrade | Monaco-based editor: syntax highlight, operator snippets, squiggle diagnostics, lazy load bundle, opt-in fallback |  | [ ] |

### 3.3 Medium Priority

| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| M1 | Backend Tags Validation Guard | Server rejects invalid tags; UI surfaces errors |  | [ ] |
| M2 | Advanced Graph Validation (Join↔Parallel coherence) | Publish blocked on structural errors |  | [ ] |
| M3 | Simulation / Path Preview Tool | Enumerates distinct paths (static) |  | [ ] |
| M4 | Version Diff Viewer | Added/removed/changed nodes highlighted |  | [ ] |
| M5 | Task Completion Data Viewer | JSON viewer with syntax highlight |  | [ ] |
| M6 | Expression Validation Backend Tests | Unit tests: happy path, invalid JSON, disallowed op, warning scenarios |  | [ ] |
| M7 | Timer Backend Validation Tests | Unit tests: relative vs absolute exclusivity, negative values, zero delay, past timestamp rejection |  | [ ] |

### 3.4 Low Priority

| ID | Story | DoD | Owner | Status |
|----|-------|-----|-------|--------|
| L1 | Progress Event Debounce | Prevent redundant renders |  | [ ] |
| L2 | Timeline Filters | Client-side type/time filtering |  | [ ] |
| L3 | Outbox Message Drill-Down Prep | Placeholder link (no dispatcher) |  | [ ] |
| L4 | Join Timeout Banner | Conditional display if logic active |  | [ ] |
| L5 | Metrics Stub | Basic placeholders (instances started/sec) |  | [ ] |
| L6 | Semantic Validation Opt-In Toggle | UI toggle + persisted preference; disables backend expression calls when off |  | [ ] |

### 3.5 Spikes

| ID | Spike | Goal | Deliverable | Status |
|----|-------|------|-------------|--------|
| S1 | Endpoint Usage Audit | Confirm in-use endpoints vs surfaced UI | Updated parity map | [ ] |
| S2 | Join/Parallel Test Seed | Provide sample definitions for QA | JSON seed file | [ ] |
| S3 | Expression Validation Contract | Decide reuse vs new endpoint | ADR mini note | [ ] |

---

## 11. Story Progress Log (New Entries Only)

| Date | Story | Status Update | Blockers | Next Action |
|------|-------|---------------|----------|------------|
| 2025-09-08 | H5 | Planning stub created (scope, approach, risks) | None | Implement after H1 stabilizes |
| 2025-09-08 | M7 | Added test coverage goals for timer validation logic | None | Draft test cases |

---

## 12. Testing Plan

| Area | Layer | Planned Coverage | Related Story |
|------|-------|------------------|---------------|
| Expression Validation | Backend Unit | JSON parse errors, disallowed operator, warning generation, success AST shape | M6 |
| Timer Validation | Backend Unit | Relative vs absolute mutual exclusivity, past absolute time, zero/negative delay rejection | M7 |
| Join Mode Logic | Backend Unit (Future) | Mode-specific parameter validation (count/quorum/expression) | M2 |
| Gateway Strategy | Frontend Unit | Strategy persistence + handle rendering per strategy | C1/C2 |
| Timer Panel UI | Frontend Unit | Mode switch retains values, validation messages appear appropriately | C5 |
| Expression Editor | Frontend Unit | JSON parse fail → invalid state; semantic toggle (future L6) | H1/L6 |

---

## 13. Change Log (New Entries Only)

| Date | Change | Author |
|------|--------|--------|
| 2025-09-08 | Added H5 Monaco stub, M7 timer backend tests, L6 semantic opt-in; testing plan section | Team |

---
(End of Document)

---

# H5 Planning Stub (Monaco Expression Editor Upgrade)

Goal:
Replace simple TextField-based JsonLogic editor with a lazy-loaded Monaco editor providing syntax highlighting, operator completions, inline diagnostics (squiggles), and future variable suggestion hooks.

Scope (In H5):
1. Lazy dynamic import of Monaco bundle (no blocking initial app load).
2. JsonLogic “language” registration:
   - Treat as JSON with custom completion provider for top-level operators.
   - Basic token styling (operators, var, numbers, strings).
3. Diagnostics integration:
   - Local JSON parse errors surfaced as red squiggles.
   - Backend semantic validation warnings mapped to warning markers (yellow).
4. Editor features:
   - Format shortcut (Ctrl/Cmd+Shift+F).
   - Read-only fallback if Monaco fails to load.
   - Prop: compact vs full height variant.
5. Accessibility:
   - ARIA label hook.
   - High contrast theme fallback (reuse monaco ‘vs-dark’ / ‘hc-black’ mapping).
6. Fallback:
   - If network offline or dynamic import fails → revert to existing TextField mode automatically.

Out of Scope (Defer):
- Variable context introspection (needs backend context endpoint).
- Live operator documentation hover.
- Monaco worker bundling optimization beyond basic dynamic import.

Technical Approach:
- Create ExpressionMonaco.tsx (dynamic import inside React.Suspense).
- Use separate chunk: import('monaco-editor').then(...)
- Register completion provider (provide operator suggestions: ==, !=, and, or, >, <, >=, <=, var, in, missing, missing_some).
- On content change debounce:
  - Local parse → monaco.editor.setModelMarkers (owner: 'jsonlogic-local')
  - If semantic opt‑in (L6) active → call backend; map errors/warnings to markers (owner: 'jsonlogic-semantic')
- Maintain adapter wrapper so existing ExpressionEditor can swap implementation via prop (e.g. useMonaco={true} once story complete).

Data Flow:
User input → Monaco model → debounce → local parse → optional backend validate → markers → parent onChange/value.

Dependencies / Order:
- Requires H1 stable (existing semantic validation method).
- L6 (opt-in toggle) can follow or precede; stub hooking can read from a context.

Risks:
| Risk | Mitigation |
|------|------------|
| Bundle size regression | Dynamic import & only load when editor first mounts |
| Race conditions between local & semantic markers | Namespace markers (different owner keys) |
| Operator completion noise for nested objects | Provide context-aware suggestions only at depth=1 initially |
| JsonLogic syntax drift | Keep operator list centralized (single exported array) |
| Accessibility regressions | Fallback to TextField; add aria-label & ensure tab focus retains selection |

Acceptance Criteria:
- Monaco loads only when user focuses an expression field (network waterfall visible).
- Invalid JSON shows squiggle within 300ms of pause.
- Backend semantic warning appears as yellow squiggle & in tooltip.
- Switching between multiple editors retains independent markers.
- Graceful fallback if import fails (console.warn + TextField).

Telemetry (Optional Future):
- Track load time of Monaco chunk.
- Count semantic validation calls vs opt-out sessions.

Next Steps After H5:
- L6: user preference toggle controlling semantic validation calls.
- Operator docs hover (H6 potential).
- Variable autocomplete (requires context API).

Let me know when to proceed with actual H5 implementation or begin M6/M7 test scaffolds. Reply with: 
- “Implement H5” 
- or “Start M6” / “Start M7” 
- or another story ID.
