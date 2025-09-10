# Workflow Frontend Parity & Sprint Plan (Maintained Document)
Estimated % Completion: 59%  *(22 completed / 37 scoped stories)*

> Purpose: Track alignment between backend WorkflowService capabilities and frontend implementation, and manage sprint execution.  
> Update Cadence: After each story refinement / completion.  
> Owner: Workflow Feature Team  
> Last Updated: 2025-09-10

---

## 1. Scope & Intent
This document ensures all backend workflow features (nodes, lifecycle operations, execution diagnostics, admin operations) are properly represented and operable in the frontend UI.  
Primary focus this sprint: Close critical parity gaps (Gateway strategies, Parallel + Join authoring, lifecycle/admin ops, diagnostics panels).

---

## 2. Backend → Frontend Feature Parity Matrix
Progress Legend: [ ] Not Started · [~] In Progress · [x] Complete · [D] Deferred

| Feature | Backend Status | Frontend Status | Gap Summary | Priority | Progress |
|---------|----------------|-----------------|-------------|----------|----------|
| Start / End Nodes | Stable | Implemented | — | Done | [x] |
| HumanTask Node | Stable | Enhanced assignment UX (modes, SLA, escalation, form schema, auth val stub) | — | High | [x] |
| Automatic Node | Stable | Webhook/Noop action config + validation + retry + telemetry | — | Medium | [x] |
| Gateway Strategy | Supported | Strategy selector + condition editor + hints (C1) | Further semantic enrich | Critical | [x] |
| Conditional Expressions | Supported | Monaco + semantic + dynamic vars | Advanced scopes | Critical | [x] |
| Parallel Fan-Out | Supported | Branch coloring + diagnostics overlay (C2) | Full dominance algo | Critical | [x] |
| Join Node & Modes | Supported | Panel + semantic & mode validation (C4) | Dominance refinement | Critical | [x] |
| Join Mode Config | Supported | Inputs & expression editor (C4) | UI polish | Critical | [x] |
| Timer Node | Supported | Advanced panel (presets, preview) | Worker automation | High | [x] |
| Task Actions: claim/complete | Implemented | Present | — | Done | [x] |
| Task Assign | Implemented | Dialog + guards | User/role search | High | [x] |
| Task Cancel | Implemented | Single cancel | Improved confirmations | High | [x] |
| Task Admin Reset | Implemented | Drawer reset (admin) | Reason auditing UI | High | [x] |
| Instance Suspend / Resume | Implemented | Row actions (C7) | Enhanced status detail | High | [x] |
| Bulk Cancel Instances | Implemented | Multi-select + reason dialog (C9) | Result breakdown UI | Critical | [x] |
| Definition Terminate-Running | Implemented | Modal + count + result (C8) | Deeper audit surfacing | Critical | [x] |
| Definition Revalidate | Implemented | Button + result panel | Audit surfacing later | Medium | [x] |
| New Version (draft) | Implemented | Action button (create draft) | Diff viewer later | Medium | [x] |
| Runtime Snapshot | Implemented | Snapshot panel (C10) | Additional graph insights | Critical | [x] |
| Event Timeline | Implemented | Timeline panel (C11) | Advanced stream coalescing | Critical | [x] |
| Progress Bar (dedupe) | Implemented | Variance aggregation + summary telemetry (PR3) | (Optional) mini chart | Low | [x] |
| Tags Filtering (ANY/ALL) | Implemented | Guard + telemetry + tests (PR1–PR2) | Optional: analytics dashboard | Medium | [x] |
| Tags Server-Side Validation | Missing guard | Publish pre-check + live UI validation + tests (PR3) | — | Medium | [x] |
| JsonLogic Expression Builder | Engine ready | Monaco + semantic + vars | Examples library | High | [x] |
| Monaco Editor Integration | N/A | MVP done | Tests + optimization | High | [x] |
| Monaco Telemetry & Theming | N/A | Implemented | Charts/export | High | [x] |
| Monaco Variable Assist | N/A | Dynamic vars + docs | Categorized grouping | Medium | [x] |
| Monaco Frontend Tests | N/A | Telemetry assertions added (PR3) | Optional: coverage on timer & parallel diagnostics | Medium | [x] |
| Semantic Validation Opt-In | N/A | Toggle + badge | Analytics detail | Low | [x] |
| Monaco Bundle Optimization | N/A | PR3: slim core + json/editor workers + parse debounce + telemetry | Optional: on-demand JS lang, exceljs dynamic import, lite JSON mode | Low | [~] |
| Parallel→Join Structural Validation | N/A | Heuristic + refinement + strict toggle (M2) | Dominance completeness metrics | Critical | [x] |
| Join Timeout Visibility | Experimental | Missing | Conditional banner | Low | [ ] |
| Outbox Visibility | Persist only | Missing | Health widget | Medium | [ ] |
| ActiveTasksCount | Pending enrich | Missing | Column/badge fallback | Medium | [ ] |
| Simulation / Dry-Run | Enhanced | PR3: path enum + parallel cartesian + join-mode semantics | Edge conditions, probabilities, weighting | Medium | [x] |
| Version Diff Viewer | Data available | Field-level diff + visual overlay (PR3 in progress) | Arbitrary compare selector | Medium | [~] |
| Event Stream Coalescing | Not needed now | Missing | Debounce layer | Low | [ ] |
| Metrics (lag, SLA) | Planned | Missing | Metrics stub | Low | [ ] |
| Expression Validation Tests (backend) | Partial | Initial tests added | Broader negative set | Medium | [x] |
| Timer Backend Validation Tests | Not present | Missing | Add tests (M7) | Medium | [ ] |

---

## 3. Sprint Story Backlog (Triaged)

### 3.1 Critical Stories
| ID | Story | Definition of Done | Status |
|----|-------|--------------------|--------|
| C1 | Gateway Strategy Selector | Persist strategies; DSL round-trip | [x] |
| C2 | Parallel Gateway Visualization | Fan-out labeling & warnings | [x] |
| C3 | Join Node Type (Base) | Palette + serialization | [x] |
| C4 | Join Configuration Panel | Modes + dynamic fields + validation | [x] |
| C5 | Timer Node Property Panel Upgrade | Advanced panel + validation + presets + preview | [x] |
| C6 | Task Extended Actions | Drawer + permission guards + actions wired | [x] |
| C7 | Instance Suspend/Resume | Row controls + refresh | [x] |
| C8 | Definition Terminate-Running Action | Modal + summary | [x] |
| C9 | Bulk Cancel Instances | Multi-select + confirmation + reason | [x] |
| C10 | Runtime Snapshot Panel | Active nodes/parallelGroups/join meta display | [x] |
| C11 | Event Timeline (Instance) | Paginated, grouped, type-badged | [x] |

### 3.2 High / Medium / Low Priority
Remaining matrix items (Medium/Low) to pick up after dominance refinement & revalidate actions.

#### 3.2.1 Medium / Low Stories Matrix (Restored – Non‑Critical Backlog)
| Key | Story / Capability | Current Frontend Status | Gap / Next Increment | Priority | Notes / Dependencies |
|-----|--------------------|-------------------------|----------------------|----------|----------------------|
| M-A1 | JsonLogic Examples Library | Editor present | Curated snippet / examples palette | Medium | After Expression stability |
| M-A2 | HumanTask: Form Schema Rendering | Stub (spec only) | Dynamic form preview & validation | Medium | Depends future form engine |
| M-A3 | Automatic Node Webhook Telemetry Deep-Dive | Basic telemetry | Size buckets → charts/export | Low | After metrics infra |
| M-T1 | Tags Analytics Dashboard | Not started | Aggregation & charts | Medium | Depends metrics plumbing |
| M-T2 | Tags Legacy Param Deprecation | Pending | Warning banner + docs link | Medium | Coordinate with API deprecation |
| M-V1 | Version Diff Viewer PR3b | Visual overlay ghost removed nodes | Ghost nodes & edge diff styling | Medium | Needs prior layout caching |
| M-V2 | Version Diff Arbitrary Compare | Sequential only | Pick any previous version | Medium | Requires versions list API (if not cached) |
| M-V3 | Diff Telemetry Expansion | Basic toggle/open | Field-level expansion events | Low | After overlay stabilizes |
| M-SIM3 | Simulation Join-Mode Semantics | Implemented (all/any/count/quorum/expression) | Edge conditions (M-SIM6), probability layer (M-SIM4) | Medium | Δ join modes done |
| M-SIM4 | Simulation Probability Layer | Not started | Path likelihood (historical weighting) | Low | Needs historical stats aggregation |
| M-SIM5 | Simulation Export / Persist Scenarios | Not started | Save context + options & reload | Low | Optional UX enhancer |
| M-SIM6 | Simulation Edge Condition Support | Node-level only | Per-edge JsonLogic evaluation | Medium | Requires DSL edge condition schema |
| M-J1 | Join Timeout Visibility | Missing | Conditional banner & countdown | Low | Requires timeout metadata surfaced |
| M-O1 | Outbox Visibility Widget | Missing | Health widget (lag, retry) | Medium | Needs outbox metrics endpoint |
| M-ATC | ActiveTasksCount Fallback | Missing | Column/badge until backend enrich | Medium | Depends instance snapshot ext |
| L-ESC | Event Stream Coalescing | Missing | Debounce / grouping layer | Low | After telemetry baseline |
| L-MET1 | Metrics (lag, SLA) Stub | Missing | Basic runtime metrics panel | Low | Needs backend counters |
| B-EVT | Expression Validation Tests (Backend) | Not present | Implement M6 test plan | Medium | Improves confidence |
| B-TIM | Timer Backend Validation Tests | Not present | Implement M7 test plan | Medium | Prior to TimerWorker rollout |
| M-MON4 | Monaco On-Demand Languages | Slim core only | Lazy load JS/YAML as needed | Low | Post slim baseline |
| M-MON5 | Monaco exceljs Dynamic Import | Static import now | Code split heavy lib | Low | Size reduction target |
| M-MON6 | Monaco Lite JSON Mode Flag | Always JSON service | Toggle off schema worker | Low | Perf on constrained devices |
| L-UX1 | Mini Progress Sparkline | Not started | Compact trend next to badge | Low | Optional visual enhancement |
| L-UX2 | Role Usage Impact Preview | Missing | Change impact simulation | Low | Needs role usage aggregation |
| L-AUD | Audit Surfacing (Terminate/Cancel Reasons) | Missing UI | Show reasons in timeline/events | Medium | Events already capture reason |
| L-ACC | Overlay Accessibility | Color only now | Pattern / aria summaries | Low | After visual diff stabilization |
| L-OPS | Outbox Dispatcher & Lag Metric Surfacing | Backend pending | FE widget consumption | Medium | Wait for dispatcher implementation |
| L-IMP | Definition Immutability Guard (UI Notice) | Implicit | Banner / disabled editing cues | Low | Clarify edit vs new version |
| L-ERR | Structured Error Classification UI | Generic toast | Consistent 401/403/409/422 surfaces | Medium | Map backend codes |
| DOC-1 | Developer Guide: Simulation Internals | Not started | MDX / wiki page | Low | After SIM PR3 |
| DOC-2 | Bundle Optimization Playbook | Not started | Document approach & thresholds | Low | Post Monaco PR4 |

Clarification: This matrix restores non‑critical items previously implied but not explicitly enumerated in section 3.2. No critical scope changed. Future additions should append rows rather than replacing this table.

---

## 4. Sequencing & Dependencies
1. C1 → C2 → C3/C4  
2. C6, C7, C9 independent once task/instance endpoints stable  
3. M2 refinement after C2/C4 (dominance / strict structural)  
4. Revalidate + New Version (draft) after diagnostics stable  
5. Test coverage & optimization after feature freeze

---

## 5. Risk Register
| Risk | Impact | Mitigation | Status |
|------|--------|------------|--------|
| Parallel gateway no convergence | High | Validation error (C2) | Mitigated |
| Partial branch merges | High | Subset warnings (C2) | Open |
| Timer worker absent | High | Plan worker deployment next sprint | Open |
| Bulk cancel misuse | Medium | Reason dialog + counts | Mitigated |
| Missing tag validation | Medium | Server guard (M1) | Open |
| Large Monaco bundle | Medium | Bundle split story (L7) | Open |
| Dominance false positives | Medium | Keep heuristic + toggle strict | Planned |
| Low test coverage builder | Medium | Add unit tests post-C4 | Open |

---

## 6. DSL Schema Additions (Highlights)
| Area | Additions / Changes |
|------|---------------------|
| Gateway | strategy: 'exclusive' | 'conditional' | 'parallel'; condition (JsonLogic string) |
| Parallel Diagnostics | structural metadata (branches, commonJoins, subsetJoins, orphanBranches) generated client-side |
| Join Node | mode: all | any | count | quorum | expression; thresholdCount; thresholdPercent; expression; cancelRemaining |
| Timer | delaySeconds, delayMinutes, untilIso (precedence: untilIso > seconds > minutes) |
| Task | future: formSchema, action.webhook config (placeholder) |
| Validation Output | diagnostics.parallelGateways, diagnostics.joins extended in result |

---

## 7. Validation Strategy
Layered approach executed client-side before publish; server still authoritative.

1. Syntactic  
   - Node & edge presence, uniqueness, reachability, edge endpoints.
2. Node-Level Semantics  
   - Gateway: conditional requires valid JsonLogic; parallel requires ≥2 branches.  
   - Join: per-mode parameter validation.  
   - Timer: exclusive delay vs untilIso + future check.
3. Structural (Heuristic)  
   - Parallel→Join convergence mapping (common, subset, orphan) with errors & warnings.
4. Contextual Advisories  
   - HumanTask role presence, small quorum edge cases, any-mode low branch count.
5. Diagnostics Packaging  
   - ExtendedValidationResult { errors, warnings, diagnostics } consumed by property panels & overlay.
6. Publish Gate  
   - Block on errors; allow warnings with user confirmation (planned confirm dialog).
7. Future Strict Mode (M2)  
   - Dominance/complete convergence algorithm behind toggle to compare heuristic vs strict.

---

## 8. UI / UX Guidelines
| Area | Guideline |
|------|-----------|
| Property Panel | Context-sensitive sections: strategy editor (gateway), join panel, timer mode toggles |
| Feedback | Inline chips & alerts for issues/warnings; no modal interruptions during editing |
| Accessibility | All inputs label-associated; keyboard deletion support on canvas; focus trap in drawers |
| Bulk Operations | Always require explicit confirmation + optional reason (stored for audit) |
| Edge Labeling | Conditional gateway auto-suggest labels (future) but never overwrite user edits |
| Visual Diagnostics | Parallel branch colors reserved palette (max 6 before cycling) |
| Error vs Warning | Error blocks publish; warning allows proceed; consistent color coding (MUI severity) |
| Auto-Refresh (runtime) | Suspended/terminal states disable auto-refresh to reduce noise |
| Consistency | Enumerations persist explicitly (no inference on save) for clarity & diff stability |

---

## 9. Permissions & Security
| Permission | UI Impact |
|------------|-----------|
| workflow.read | View definitions, instances, tasks (baseline) |
| workflow.write | Edit draft definitions, create new versions, publish attempt |
| workflow.admin | Bulk cancel, terminate-running, task admin reset, structural diagnostics overrides (future) |
| role.* (backend) | Role selection in HumanTask assignment filtered to tenant roles with workflow.* permissions |
| Future Enhancements | Audit surfacing for terminate + bulk cancel reasons in timeline |

---

## 10. Telemetry
| Metric | Source Hook | Status |
|--------|-------------|--------|
| gateway.strategy.changed | GatewayStrategyEditor | Stub TBD |
| parallel.diagnostic.opened | PropertyPanel (gateway selected) | Stub TBD |
| join.mode.changed | JoinConfigurationPanel | Stub TBD |
| validation.publish.attempt | Publish flow | Planned |
| bulk.cancel.count | Instances bulk action | Planned |
| suspend.resume.usage | Instance list actions | Planned |
| simulation.run.start / complete / truncated | SimulationDrawer | Implemented |
| simulation.path.select | SimulationDrawer | Implemented |
| diff.viewer.overlay.toggle | Diff Drawer & Instance Diagram | Implemented |
| monaco.local.parse.ms | Editor (JsonLogic) | Implemented |

---

## 11. Story Progress Log
| Date | Story | Status Update | Blockers | Next Action |
|------|-------|---------------|----------|------------|
| 2025-09-09 | C4 | Join configuration panel (modes, validation, diagnostics) complete | None | M2 dominance refinement |
| 2025-09-09 | C2 | Parallel diagnostics (branches/common/subset/orphans) + legend | None | M2 deeper dominance eval |
| 2025-09-09 | C1 | Strategy selector + conditional editor & snippets delivered | None | Minor semantic polish |
| 2025-09-09 | C11 | Event timeline (filters, pagination, grouping, badges) complete | None | Optional infinite streaming |
| 2025-09-09 | C10 | Runtime Snapshot panel (metrics, auto refresh) complete | None | C11 timeline |
| 2025-09-09 | C8 | Terminate-running modal delivered (counts + summary) | None | Optional audit surfacing |
| 2025-09-09 | Revalidate | Frontend button + validation result collapse added | None | Consider audit timeline surfacing |
| 2025-09-09 | New Version Draft | Create new draft version dialog & navigation to builder | None | Later: diff viewer |
| 2025-09-08 | C9 | Bulk cancel (multi-select + reason) complete | None | Optional result breakdown |
| 2025-09-08 | C7 | Suspend / Resume UI complete | None | C8 terminate-running |
| 2025-09-08 | C6 | Task drawer & actions available | None | UX polish |
| 2025-09-08 | M2 | Strict structural (dominance) analysis toggle added; mismatch diff surfaced | None | Evaluate false positives |
| 2025-09-08 | C5 | Advanced timer panel complete | Worker missing | M7 tests |
| 2025-09-09 | H1 | PR1–PR5: types, validation, UI scaffold, hooks, Monaco, publish validation integration | None | PR6 tests |
| 2025-09-09 | H1 | PR6: assignment validation unit tests added (rules + integration) | None | PR7 telemetry stubs |
| 2025-09-09 | H1 | Deferred items delivered: form schema editor, assignment history, auth validation stub | None | — |
| 2025-09-09 | A1 | PR2: Automatic node action editor UI (webhook + retry + headers + body) integrated | None | PR3 telemetry/tests |
| 2025-09-09 | A1 | PR3: automatic action validation unit + integration tests added | None | (Optional) PR4 telemetry polish |
| 2025-09-09 | A1 | PR4: telemetry polish (headers/body size buckets, health transitions, retry state) | None | PR5 finalize + docs |
| 2025-09-09 | A1 | PR5: documentation & JSDoc added; story complete | None | — |
| 2025-09-09 | T1 (Tags Validation) | PR2: Live tag validation field (debounced), error surfacing in Workflow Settings | None | PR3 tests |
| 2025-09-09 | T1 (Tags Validation) | PR3: Unit & integration tests (preview + publish gating) added | None | — |
| 2025-09-09 | PB (Progress Dedupe) | PR2: Added useInstanceProgress deduped metrics (hook-level) | None | PR2b add hook tests |
| 2025-09-09 | PB (Progress Dedupe) | PR3: Variance aggregation + summary telemetry + UI badge | None | (Optional) sparkline |
| 2025-09-09 | MFT (Monaco Tests) | PR1: Added component tests (strategy, join, assignment, action) with editor mock | None | PR2 expand scenarios |
| 2025-09-09 | MFT (Monaco Tests) | PR2: Added extended validation tests (SLA, quorum, expression invalid JSON, headers/body/retry/discard) | None | PR3 telemetry assertions |
| 2025-09-09 | MFT (Monaco Tests) | PR3: Telemetry event assertion coverage (gateway.strategy, join.mode, assignment.mode/expression, action.* set) | None | — |
| 2025-09-09 | TF (Tags Filtering Guard) | PR1: Backend 422 validation + frontend error surfacing added | None | PR2 tests & telemetry |
| 2025-09-09 | TF (Tags Filtering Guard) | PR2: Added backend/unit tests + frontend UI tests + telemetry events | None | — |
| 2025-09-09 | VDV (Version Diff Viewer) | PR1: Baseline drawer with node/edge add/remove/modify diff implemented | None | PR2 field-level key detail refinements |
| 2025-09-09 | VDV (Version Diff Viewer) | PR2: Field-level modified node diff + tests + expansion telemetry | None | PR3 visual overlay |
| 2025-09-09 | VDV (Version Diff Viewer) | PR3: Initial visual overlay highlighting added/modified nodes (legend + toggle) | None | PR3b removed ghost layout |
| 2025-09-09 | MBO (Monaco Bundle Optimization) | PR1: Added optimized dynamic loader, telemetry, idle prefetch | None | PR2 on-demand languages |
| 2025-09-09 | MBO (Monaco Bundle Optimization) | PR2: Defer-on-focus load, retry fallback, theme skip, model cleanup, heuristic prefetch, concurrency & size guard script | None | PR3 language on-demand |
| 2025-09-09 | MBO (Monaco Bundle Optimization) | PR3: Switched to editor.api, removed bulk languages, custom json/editor workers, telemetry (slim) | None | Validate size drop / optional JS language |
| 2025-09-09 | MBO (Monaco Bundle Optimization) | PR3b: Added 150ms debounced parse, parse time telemetry (bucketed), semantic trigger post-parse | None | PR4 dynamic exceljs / on-demand JS lang |
| 2025-09-09 | Simulation (Dry-Run) | PR1: Added SimulationDrawer, basic DFS path enumeration, conditional gateway eval, diagram path highlight | None | PR2 parallel expansion |
| 2025-09-09 | Simulation (Dry-Run) | PR2: Parallel cartesian expansion (capped), join merge detection, truncation reasons & controls (depth/paths/cartesian), telemetry | None | PR3 join-mode semantics |
| 2025-09-10 | M-SIM3 | Join-mode semantics (all/any/count/quorum/expression) implemented in simulation | None | Edge conditions (M-SIM6) |
| 2025-09-10 | Backend Tests | Added JoinSemanticsRuntimeTests parity coverage | None | Negative expr cases |

---

## 12. Testing Plan
| Area | Layer | Coverage Goal | Story |
|------|-------|--------------|-------|
| Gateway Strategy Editor | Frontend Unit | Strategy switch preserves/removes condition appropriately | C1 |
| Parallel Diagnostics | Frontend Unit | Diagnostics sets (common/subset/orphan) computed as expected | C2 |
| Join Configuration Panel | Frontend Unit | Mode field rules & dynamic validation | C4 |
| Event Timeline | Frontend Unit | Filters & pagination produce consistent counts | C11 |
| Runtime Snapshot Panel | Frontend Unit | Progress% accurate; auto-refresh toggles; disabled terminal | C10 |
| Bulk Cancel | Frontend Unit | Only cancellable IDs sent; reason optional | C9 |
| Suspend / Resume | Frontend Unit | Status transitions reflected | C7 |
| Timer Panel | Frontend Unit | Presets & validation states | C5 |
| Task Actions Drawer | Frontend Unit | Guard matrix | C6 |
| Automatic Node Action Editor | Frontend Unit | Webhook + retry + headers + body fields | A1 |

---

## 13. Change Log
| Date | Change | Author |
|------|--------|--------|
| 2025-09-09 | Added join configuration panel (C4) | Team |
| 2025-09-09 | Added gateway strategy editor & parallel diagnostics (C1, C2) | Team |
| 2025-09-09 | Added event timeline (C11) | Team |
| 2025-09-09 | Added runtime snapshot panel & terminate-running UI (C10, C8) | Team |
| 2025-09-08 | Added suspend/resume & bulk cancel (C7, C9) | Team |
| 2025-09-08 | Structural validation refinement | Team |
| 2025-09-08 | Advanced timer panel | Team |
| 2025-09-09 | Added definition revalidate action & panel | Team |
| 2025-09-09 | Added new version (draft) creation flow | Team |
| 2025-09-09 | Automatic node action config (A1 complete) | Team |
| 2025-09-09 | PR1: Added server-side tags validation hook in publish flow (silent 404 pass) | Team |
| 2025-09-09 | Added simulation path enumeration (PR1–PR2) | Team |
| 2025-09-09 | Added diff visual overlay (definitions / instances) | Team |

---

## 14. Open Questions
| Question | Owner | Target |
|----------|-------|--------|
| Dominance algorithm need now? | Arch | Mid-sprint |
| Bulk cancellation audit metrics? | Backend | Before GA |
| Expose cancel/terminate reasons in timeline? | Product | C11 design |
| When to introduce strict structural toggle UI? | FE/Arch | After M2 spike |
| ActiveTasksCount backend enrichment ETA? | Backend | Align w/ instance list refresh |
| Simulation join-mode semantics approach (respect quorum/count/expression)? | Arch/FE | Before Simulation PR3 |
| Probability / weighting source for simulation (historical vs static)? | Product/Arch | Post PR3 |

---

## 15. Stretch Candidates
| Item | Rationale |
|------|-----------|
| Simulation / Dry-Run | Improves author confidence (path enumeration) |
| Version Diff Viewer | Change intelligence between versions |
| Metrics (lag, SLA) | Operational transparency |
| Event Stream Coalescing | Reduce noise under bursts |
| Monaco Bundle Slim Mode | Faster initial load |
| Outbox Visibility Widget | Operational health indicator |
| Debounced Live Timeline | Lower render churn |
| Role Usage Impact Preview | Safer role refactors |
| Simulation Join-Mode Semantics (PR3) | Higher fidelity modeling |
| Simulation Probability Layer | Risk / path likelihood insights |
| Diff Overlay Ghost Removed Nodes | Full visual diff clarity |

---

## 16. Decisions (Summary)
| Decision | Date | Notes |
|----------|------|-------|
| Promote non-converging parallel to error | 2025-09-08 | Prevent dead paths |
| Ship heuristic before full dominance | 2025-09-08 | Time-box complexity |
| Bulk cancel unified endpoint usage | 2025-09-08 | Consistent auditing |
| Explicit persistence of gateway strategy | 2025-09-09 | Avoid inference drift |
| Provide diagnostics object in validation | 2025-09-09 | Single source for overlays |
| Defer Monaco load until focus (editor) | 2025-09-09 | Reduced initial bundle/TTI |
| Slim Monaco to json/editor only | 2025-09-09 | Bundle reduction baseline |
| Add simulation path enumeration before join semantics | 2025-09-09 | Incremental delivery |

---

## 17. Glossary
| Term | Definition |
|------|------------|
| Gateway Strategy | Execution behavior: exclusive (single path), conditional (JsonLogic driven), parallel (fan-out) |
| Parallel Convergence | Point(s) where parallel branches reconverge via join(s) |
| Subset Join | Join reached by only a portion of parallel branches |
| Orphan Branch | Parallel branch that reaches no join |
| Join Mode | Policy determining when join fires (all/any/count/quorum/expression) |
| Cancel Remaining | Option to prune other branches once join condition satisfied |
| Snapshot (Runtime) | Polled instance state: active nodes, visited nodes, tasks, events |
| Structural Diagnostics | Validation metadata describing graph topology issues |
| Dominance (future) | Formal guarantee each branch must pass a specific node (strict structural analysis) |
| JsonLogic | Declarative JSON-based logic DSL used for conditions |

---

## 18. Story Specification – HumanTask Node Enhanced Assignment UX (New)
 
 | Field | Detail |
 |-------|--------|
 | Story ID | H1 (HumanTask Enhanced Assignment) |
 | Goal | Allow authors to configure richer assignment semantics for HumanTask nodes (users, roles, dynamic expressions, fallback & SLA metadata) directly in the builder property panel with validation + preview. |
 | Priority | High |
 | Status | Complete |
 | Owner | Workflow Feature Team |
 | Dependencies | Existing role/user directory endpoints; expression validation service; publish validation hook. |
 
 ### 18.1 Scope (In-Scope vs Out-of-Scope)
 - In-Scope:
   - Property Panel section for HumanTask node (replaces basic fields).
   - Assignment modes: 
     1. Direct User(s) (multi-select) 
     2. Role(s) (multi-select) 
     3. Dynamic Expression (JsonLogic returning userIds/role codes) 
     4. Hybrid (roles + expression fallback)
   - Optional fallback: escalateToRole (single role) on SLA breach (stub).
   - SLA config: targetMinutes (integer) + optional softWarningMinutes (< target).
   - Validation rules (see section 18.4).
   - Display of computed “effective assignment summary” (static evaluation where possible).
   - Expression validation via existing validate expression endpoint (kind='gateway' reuse or extend to 'task' if backend allows).
   - Telemetry stubs (no charts yet).
   - Draft JSON DSL mutation (persist structured object).
 - Out-of-Scope (defer):
   - Full SLA runtime enforcement UI.
   - Escalation workflow builder.
   - Per-branch overrides.
   - Form schema integration.
 
 ### 18.2 DSL Additions
 Augment HumanTask node object with (proposed):
 ```json
 { "type": "humanTask", "assignment": 
    { "mode": "users|roles|expression|hybrid", "users": ["userId1","userId2"], "roles": ["RoleA","RoleB"], 
    "expression": "{ /* JsonLogic returning { users?:[], roles?:[] } */ }", "escalation": { "escalateToRole": "Supervisors", "afterMinutes": 120 }, "sla": 
    { "targetMinutes": 240, "softWarningMinutes": 180 } 
  } 
 }

 ```
 - Omit arrays/objects when empty.
- Validate that hybrid requires at least one static roles entry plus expression.

### 18.3 UI / UX Details
- Property Panel Section Title: “Assignment”
- Mode selector (radio group).
- Multi-select autocompletes:
  - Users: search (debounced) by displayName/email.
  - Roles: search local cached list (fetched once per builder session).
- Expression editor: Monaco JsonLogic pre-configured (reuse existing variables assist).
- SLA inputs grouped with helper text:
  - targetMinutes required for any SLA.
  - softWarningMinutes optional; must be < targetMinutes.
- Escalation (collapsed/accordion) – disabled until SLA provided.
- Summary chip row (live): “Users(3), Roles(2), Expr ✓ validated, SLA: 4h”.
- Error surfacing inline per field; publish blocked if structural + assignment errors exist.

### 18.4 Validation Rules
| Rule | Severity | Message |
|------|----------|---------|
| mode selected | Error | “Assignment mode is required.” |
| users mode & empty users[] | Error | “At least one user must be selected.” |
| roles mode & empty roles[] | Error | “At least one role must be selected.” |
| expression mode & missing expression | Error | “Expression is required for expression mode.” |
| expression invalid (syntax) | Error | “Assignment expression invalid: <detail>.” |
| hybrid mode & missing roles | Error | “Hybrid mode requires at least one role.” |
| hybrid mode & missing expression | Error | “Hybrid mode requires an expression.” |
| duplicate user ids | Warning | “Duplicate users removed.” (auto-dedupe) |
| duplicate roles | Warning | “Duplicate roles removed.” |
| SLA targetMinutes < 5 | Warning | “Very low SLA target may be unrealistic.” |
| softWarningMinutes ≥ targetMinutes | Error | “Soft warning must be less than target.” |
| escalation.afterMinutes <= targetMinutes (if both) | Warning | “Escalation occurs before or at SLA target.” |

### 18.5 Backend / API Touchpoints
- Endpoint (assumed existing / to confirm):
  - GET /api/users?search= (paged)
  - GET /api/roles
  - POST /api/workflow/expressions/validate (reuse with kind='task' if backend extended; fallback kind='gateway')
- No publish endpoint change required (fields pass-through).
- Add server validation (future) to reject empty assignment per mode.

### 18.6 Frontend Tasks
| Task | Type | Est |
|------|------|-----|
| Add DSL Type augmentation (TypeScript) | Dev | 0.5d |
| PropertyPanel: detect humanTask node & render AssignmentSection | Dev | 1d |
| Users autocomplete (debounced search, 300ms) | Dev | 0.5d |
| Roles fetch & cache (context or hook) | Dev | 0.25d |
| Mode radio group + dynamic field rendering | Dev | 0.5d |
| Expression Monaco integration (reuse existing config) | Dev | 0.5d |
| SLA + Escalation subform | Dev | 0.5d |
| Local validation hook (assignmentRules.ts) | Dev | 0.5d |
| Integrate into existing validateDefinition (adds assignment errors) | Dev | 0.5d |
| Summary chips component | Dev | 0.25d |
| Telemetry stubs (console or no-op) | Dev | 0.25d |
| Unit tests (parsing, validation cases) | QA/Dev | 1d |
| Documentation (sprint.md + inline JSDoc) | Dev | 0.25d |

### 18.7 Telemetry (Stubs)
| Event | Trigger | Payload |
|-------|---------|---------|
| assignment.mode.changed | Mode change | { mode } |
| assignment.expression.validated | Successful validation | { durationMs } |
| assignment.sla.configured | target set | { targetMinutes, softWarningMinutes? } |

### 18.8 Risks & Mitigations
| Risk | Impact | Mitigation |
|------|--------|-----------|
| User search latency | UX lag | Debounce + loading indicator |
| Expression complexity | Author confusion | Provide inline examples shortcut |
| Over-validation noise | Author friction | Only block on error severity |
| Hybrid misconfiguration | Incorrect routing | Explicit paired validation & inline hints |

### 18.9 Test Plan (Additions)
| Case | Expected |
|------|----------|
| Users mode no users | Error displayed; publish blocked |
| Roles mode duplicates | Deduped silently; warning present |
| Hybrid missing expression | Error |
| Expression invalid JSON | Syntax error surfaced |
| SLA soft ≥ target | Error |
| Escalation without SLA | Escalation section disabled |
| Successful publish with hybrid | No assignment errors in final validation |

### 18.10 Acceptance Criteria (Definition of Done)
- All validation rules enforced client-side; publish blocked only on “Error” rules.
- DSL JSON persists assignment object correctly and round-trips into editor.
- Switching modes clears irrelevant fields (with confirm if data loss).
- Expression validate button (or auto) gives success/failure within <750ms typical.
- SLA summary chip appears only when SLA configured.
- No console errors; TypeScript types updated.
- Unit tests cover: each mode validation + SLA rules + hybrid scenario (≥80% lines in assignment module).
 
 ### 18.11 Out-of-Scope Follow-ups
 - Audit / timeline surfacing (server-side) for assignment changes
 - Full runtime SLA enforcement & escalation workflow
 - Backend hard rejection of invalid assignment (current stub only)
 - Per-branch overrides
 - Form-driven task UI rendering (schema consumption)
+

## 19. Story Specification – Version Diff Viewer (New)
+| Field | Detail |
+|-------|--------|
+| Story ID | VDV (Version Diff Viewer) |
+| Goal | Allow authors to quickly see what changed between two sequential workflow definition versions (added/removed/modified nodes & edges) before publishing or when reviewing history. |
+| Priority | Medium |
+| Status | In Progress (PR1) |
+| Owner | Workflow Feature Team |
+| Dependencies | Existing definitions endpoint (historical versions), JSON DSL parser already used in builder. |
+
+### 19.1 Scope (PR1 Baseline)
+Deliver a non-visual diff panel:
+ - Select “Compare to Previous Version” from DefinitionsPage row action (enabled when version > 1).
+ - Fetch prior version JSON + current version JSON.
+ - Compute structured diff:
+   - addedNodes[], removedNodes[]
+   - modifiedNodes[] (id stable; label/type/critical props changed)
+   - addedEdges[], removedEdges[]
+   - metadata summary counts (adds/removals/modifications)
+ - Present in side drawer (or dialog) with chips & grouped lists.
+ - No canvas overlay yet (future PR).
+
+### 19.2 Future PRs
+| PR | Focus |
+|----|-------|
+| PR2 | Node property field-level diff (show changed keys) |
+| PR3 | Visual overlay (ghost previous vs current) |
+| PR4 | Telemetry + unit tests (diff correctness, edge cases) |
+
+### 19.3 Diff Rules (Baseline)
+ - Node identity = id (case-sensitive).
+ - Modified node: exists in both versions & (type OR label OR key properties set { roles, assignment.mode, strategy, mode (join), timer parameters } changed).
+ - Edge identity = id (fallback to from+to if id absent).
+ - Ignore ordering differences.
+ - Whitespace-insensitive comparison for label / textual fields.
+
+### 19.4 UI / UX
+ - Drawer title: “Diff v{current} ↔ v{previous}”
+ - Summary chips: +N nodes, −M nodes, ΔK modified, +E edges, −F edges.
+ - Lists collapsible; each modified node row shows changed keys as small chips (e.g., label, strategy).
+ - Close button returns focus to definitions grid row.
+
+### 19.5 Telemetry (Planned PR4)
+| Event | Payload |
+|-------|---------|
+| diff.viewer.opened | { currentVersion, previousVersion, addNodes, removeNodes, modifiedNodes } |
+| diff.viewer.modified.field | { nodeId, field } (emitted when user expands modified node details) |
+
+### 19.6 Test Plan (PR4)
+| Case | Expected |
+|------|----------|
+| Added node only | appears in addedNodes; counts accurate |
+| Removed node only | appears in removedNodes |
+| Type change | treated as modified (original vs new type captured) |
+| Property label change only | modifiedNodes includes label in changedKeys |
+| Edge added/removed | listed in respective edge sections |
+| No changes | “No differences detected” message |
+
+### 19.7 Acceptance Criteria (PR1)
+ - Action visible for versions > 1.
+ - Diff computation completes < 200ms for typical (≤200 nodes).
+ - Accurate counts for adds/removes/modifications.
+ - No runtime errors on edge cases (empty previous, malformed prior JSON gracefully handled with error banner).
+
+### 19.8 Out-of-Scope (Deferred)
+ - Visual side-by-side graph overlay.
+ - Inline JSON patch presentation.
+ - Multi-version (v1 vs vN) arbitrary compare selector.
+ - Per-field diff for nested complex structures (e.g., deep assignment object).
+
+### 19.9 Risks & Mitigation
+| Risk | Mitigation |
+|------|------------|
+| Large definitions impact diff time | O(n) maps & set ops only; lazy expansion for modified detail |
+| Inconsistent IDs between versions | Fallback: treat as removal + addition |
+| Future DSL field additions break diff | Centralize “key properties” list; add tests |
+
+### 19.10 Implementation Steps (PR1)
+1. Add row action “Compare” in DefinitionsPage (guard version > 1).
+2. Service method getDefinitionVersion(definitionId, version?) if not existing (or reuse existing fetch with id & version param).
+3. diffWorkflowDefinitions util (pure function) + unit tests (optional PR1 or PR4).
+4. DiffDrawer component (MUI Drawer) with summary + lists.
+5. Telemetry stub (log only) placeholder.
+
+### 19.11 Metrics (Later)
+ - Average opened diffs per publish.
+ - Most frequently changed node types between versions.
