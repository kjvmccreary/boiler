# Workflow Frontend Parity & Sprint Plan (Maintained Document)
Estimated % Completion: 54%  *(20 completed / 37 scoped stories)*

> Purpose: Track alignment between backend WorkflowService capabilities and frontend implementation, and manage sprint execution.  
> Update Cadence: After each story refinement / completion.  
> Owner: Workflow Feature Team  
> Last Updated: 2025-09-09

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
| HumanTask Node | Stable | Assignment model PR1–PR5 (UI + validation) | Remaining: tests & telemetry | High | [~] |
| Automatic Node | Stable | Basic node | Webhook/action config | Medium | [ ] |
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
| Progress Bar (dedupe) | Implemented | Partial | Dedupe context missing | Low | [ ] |
| Tags Filtering (ANY/ALL) | Implemented | Implemented | Backend validation guard | Medium | [ ] |
| Tags Server-Side Validation | Missing guard | Client-only | Add guard | Medium | [ ] |
| JsonLogic Expression Builder | Engine ready | Monaco + semantic + vars | Examples library | High | [x] |
| Monaco Editor Integration | N/A | MVP done | Tests + optimization | High | [x] |
| Monaco Telemetry & Theming | N/A | Implemented | Charts/export | High | [x] |
| Monaco Variable Assist | N/A | Dynamic vars + docs | Categorized grouping | Medium | [x] |
| Monaco Frontend Tests | N/A | Missing | Coverage scenarios | Medium | [ ] |
| Semantic Validation Opt-In | N/A | Toggle + badge | Analytics detail | Low | [x] |
| Monaco Bundle Optimization | N/A | Full bundle | Slim JSON-only | Low | [ ] |
| Parallel→Join Structural Validation | N/A | Heuristic + refinement + strict toggle (M2) | Dominance completeness metrics | Critical | [x] |
| Join Timeout Visibility | Experimental | Missing | Conditional banner | Low | [ ] |
| Outbox Visibility | Persist only | Missing | Health widget | Medium | [ ] |
| ActiveTasksCount | Pending enrich | Missing | Column/badge fallback | Medium | [ ] |
| Simulation / Dry-Run | Not implemented | Missing | Path enumeration | Medium | [ ] |
| Version Diff Viewer | Data available | Missing | Node diff UI | Medium | [ ] |
| Event Stream Coalescing | Not needed now | Missing | Debounce layer | Low | [ ] |
| Metrics (lag, SLA) | Planned | Missing | Metrics stub | Low | [ ] |
| Expression Validation Tests (backend) | Not present | Missing | Add tests (M6) | Medium | [ ] |
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

---

## 14. Open Questions
| Question | Owner | Target |
|----------|-------|--------|
| Dominance algorithm need now? | Arch | Mid-sprint |
| Bulk cancellation audit metrics? | Backend | Before GA |
| Expose cancel/terminate reasons in timeline? | Product | C11 design |
| When to introduce strict structural toggle UI? | FE/Arch | After M2 spike |
| ActiveTasksCount backend enrichment ETA? | Backend | Align w/ instance list refresh |

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

---

## 16. Decisions (Summary)
| Decision | Date | Notes |
|----------|------|-------|
| Promote non-converging parallel to error | 2025-09-08 | Prevent dead paths |
| Ship heuristic before full dominance | 2025-09-08 | Time-box complexity |
| Bulk cancel unified endpoint usage | 2025-09-08 | Consistent auditing |
| Explicit persistence of gateway strategy | 2025-09-09 | Avoid inference drift |
| Provide diagnostics object in validation | 2025-09-09 | Single source for overlays |

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
 | Status | In Progress |
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
| softWarningMinutes >= targetMinutes | Error | “Soft warning must be less than target.” |
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
| SLA soft >= target | Error |
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
 - Form schema integration (future story)
 - Audit / timeline event for assignment changes
 - Backend authoritative validation (server rejects invalid assignment)
 
 (End of Story Specification)

(End of Document)
