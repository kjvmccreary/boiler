# Workflow Frontend Parity & Sprint Plan (Maintained Document)
Estimated % Completion: 35%  *(13 completed / 37 scoped stories)*

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
| HumanTask Node | Stable | Basic node only | Enhanced assignment UX | High | [ ] |
| Automatic Node | Stable | Basic node | Webhook/action config | Medium | [ ] |
| Gateway Strategy | Supported | Selector + persistence | Operator hint enrichment | Critical | [~] |
| Conditional Expressions | Supported | Monaco + semantic + dynamic vars | Advanced scopes | Critical | [x] |
| Parallel Fan-Out | Supported | Visualization + heuristic warnings | Structural refinement | Critical | [~] |
| Join Node & Modes | Supported | Panel + semantic & mode validation | Dominance refinement | Critical | [~] |
| Join Mode Config | Supported | Inputs & expression editor | UI polish | Critical | [~] |
| Timer Node | Supported | Advanced panel (presets, preview) | Worker automation | High | [x] |
| Task Actions: claim/complete | Implemented | Present | — | Done | [x] |
| Task Assign | Implemented | Dialog + guards | User/role search | High | [x] |
| Task Cancel | Implemented | Single cancel | Improved confirmations | High | [x] |
| Task Admin Reset | Implemented | Drawer reset (admin) | Reason auditing UI | High | [x] |
| Instance Suspend / Resume | Implemented | Row actions (C7) | Enhanced status detail | High | [x] |
| Bulk Cancel Instances | Implemented | Multi-select + reason dialog (C9) | Result breakdown UI | Critical | [x] |
| Definition Terminate-Running | Implemented | Modal + count + result (C8) | Deeper audit surfacing | Critical | [x] |
| Definition Revalidate | Implemented | Missing | Manual trigger UI | Medium | [ ] |
| New Version (draft) | Implemented | Missing button | Explicit action flow | Medium | [ ] |
| Runtime Snapshot | Implemented | Snapshot panel (C10) | Additional graph insights | Critical | [x] |
| Event Timeline | Implemented | Missing UI | Timeline + filters | Critical | [ ] |
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
| Parallel→Join Structural Validation | N/A | Heuristic + refinement | Full dominance algo | Critical | [~] |
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
| C1 | Gateway Strategy Selector | Persist strategies; DSL round-trip | [~] |
| C2 | Parallel Gateway Visualization | Fan-out labeling & warnings | [~] |
| C3 | Join Node Type (Base) | Palette + serialization | [x] |
| C4 | Join Configuration Panel | Modes + dynamic fields + validation | [~] |
| C5 | Timer Node Property Panel Upgrade | Advanced panel + validation + presets + preview | [x] |
| C6 | Task Extended Actions | Drawer + permission guards + actions wired | [x] |
| C7 | Instance Suspend/Resume | Row controls + refresh | [x] |
| C8 | Definition Terminate-Running Action | Modal + summary | [ ] |
| C9 | Bulk Cancel Instances | Multi-select + confirmation + reason | [x] |
| C10 | Runtime Snapshot Panel | Active nodes/parallelGroups/join meta display | [ ] |
| C11 | Event Timeline (Instance) | Paginated, grouped, type-badged | [ ] |

### 3.2 High / Medium / Low Priority (unchanged beyond status updates)
(See matrix above for current state.)

---

## 4. Sequencing & Dependencies
1. C1 → C2 → C3/C4  
2. C6, C7, C9 independent after services ready  
3. M2 refinement after C2/C4  
4. M8 after H7  
5. L7 after usage telemetry

---

## 5. Risk Register
| Risk | Impact | Mitigation | Status |
|------|--------|------------|--------|
| Parallel gateway no convergence | High | M2 heuristic error | Mitigated |
| Partial branch merges | High | M2 subset warnings | Open |
| Timer worker absent | High | Plan next sprint | Open |
| Bulk cancel misuse | Medium | Confirm dialog + reason | Mitigated |
| Missing tag validation | Medium | Implement M1 | Open |
| Large Monaco bundle | Medium | L7 optimization | Open |

---

## 6. DSL Schema Additions (Highlights)
(unchanged)

---

## 7. Validation Strategy
(unchanged – reflects structural + timer + semantic layers)

---

## 8. UI / UX Guidelines
(unchanged – add: Bulk operations require explicit confirmation)

---

## 9. Permissions & Security
(unchanged)

---

## 10. Telemetry
| Metric | Source | Status |
|--------|--------|--------|
| Bulk cancel count | Instances page (future hook) | Planned |
| Suspend/Resume usage | Instances page | Planned |
| Structural validation results | Publish flow | Planned |

---

## 11. Story Progress Log
| Date | Story | Status Update | Blockers | Next Action |
|------|-------|---------------|----------|------------|
| 2025-09-09 | C10 | Runtime Snapshot panel (metrics, auto refresh) complete | None | C11 timeline |
| 2025-09-09 | C8 | Terminate-running modal delivered (counts + summary) | None | Optional audit surfacing |
| 2025-09-08 | C9 | Bulk cancel (multi-select + reason) complete | None | Optional result breakdown |
| 2025-09-08 | C7 | Suspend / Resume UI complete | None | C8 terminate-running |
| 2025-09-08 | C6 | Task drawer & actions available | None | UX polish |
| 2025-09-08 | M2 | Heuristic & subset convergence checks | Full dominance optional | Decide depth |
| 2025-09-08 | C5 | Advanced timer panel complete | Worker missing | M7 tests |

---

## 12. Testing Plan
| Area | Layer | Coverage Goal | Story |
|------|-------|--------------|-------|
| Runtime Snapshot Panel | Frontend Unit | Progress% accurate; auto-refresh toggles; disabled terminal | C10 |
| Bulk Cancel | Frontend Unit | Only cancellable IDs sent; reason optional | C9 |
| Suspend / Resume | Frontend Unit | Status transitions reflected | C7 |
| Timer Panel | Frontend Unit | Presets & validation states | C5 |
| Task Actions Drawer | Frontend Unit | Guard matrix | C6 |

---

## 13. Change Log
| Date | Change | Author |
|------|--------|--------|
| 2025-09-09 | Added runtime snapshot panel & terminate-running UI (C10, C8) | Team |
| 2025-09-08 | Added suspend/resume & bulk cancel (C7, C9) | Team |
| 2025-09-08 | Structural validation refinement | Team |
| 2025-09-08 | Advanced timer panel | Team |

---

## 14. Open Questions
| Question | Owner | Target |
|----------|-------|--------|
| Dominance algorithm need now? | Arch | Mid-sprint |
| Bulk cancellation audit metrics? | Backend | Before GA |
| Expose cancel/terminate reasons in timeline? | Product | C11 design |

---

## 15. Stretch Candidates
(unchanged)

---

## 16. Decisions (Summary)
| Decision | Date | Notes |
|----------|------|-------|
| Promote non-converging parallel to error | 2025-09-08 | Prevent dead paths |
| Ship heuristic before full dominance | 2025-09-08 | Time-box complexity |
| Bulk cancel unified endpoint usage | 2025-09-08 | Consistent auditing |

---

## 17. Glossary
(unchanged)

---

(End of Document)
