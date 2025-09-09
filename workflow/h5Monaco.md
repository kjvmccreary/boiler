# H5: Monaco Expression Editor Upgrade (JsonLogic) – Status Report (MVP Complete)

## Objective
Replace the previous textarea/TextField JSON expression input with a lazy‑loaded Monaco editor delivering: syntax highlight, structural validation, semantic (backend) validation markers, operator completions, formatting, accessibility, and resilient fallback.

## Status Summary
Core Monaco MVP is COMPLETE and integrated (Gateway + Join conditional/expression panels) with hybrid fallback and semantic validation. Remaining polish items have been spun out into follow‑on stories (H6, H7, M8, L6) so Monaco can “shine” while keeping scope transparent.

---

## In‑Scope Items (Original Plan) – Completion Breakdown

| # | Item | Details | Status |
|---|------|---------|--------|
| 1 | Lazy load | Dynamic import & loader skeleton | ✅ Done |
| 2 | Base language support | JSON mode, completion provider (top-level), hover docs | ✅ Done |
| 3 | Markers / validation | Local parse markers (jsonlogic-local) + semantic (jsonlogic-semantic), debounce 500ms | ✅ Done |
| 4 | UX features | Format button, status chip, live region (a11y), manual revalidate | ✅ Done |
| 5 | Fallback | Automatic fallback to legacy ExpressionEditor on load error | ✅ Done |
| 6 | Config props | value, onChange, kind, semantic flag, height, readOnly | ✅ Done |
| 7 | Telemetry hooks (stub) | onLoad / onSemanticValidation callbacks | ❌ Not Implemented (H6) |
| 8 | Documentation snippet | README / internal MD snippet | ❌ Not Implemented (rolled into H7) |

### Additional Delivered (Not Explicitly in Original List)
| Feature | Notes |
|---------|-------|
| Semantic race guard | Versioned semantic responses prevent stale markers |
| Accessible live region | Announces first error/warning / validity state |
| Completion & hover modularization | jsonlogicOperators + registerJsonLogicLanguage |
| Hybrid wrapper | Seamless opt-in to Monaco via useMonaco flag |
| Format integration | Uses built-in JSON formatter |

---

## Deferred / Enhancement Items (Now New Stories)

| New Story | Scope | Rationale | Priority |
|-----------|-------|-----------|----------|
| H6 Monaco Telemetry & Theming | Load & semantic timing metrics; optional dark/high-contrast theme selection; basic instrumentation hooks | Observability & polish | High |
| H7 Monaco Variable & Operator Assist | Variable placeholder completion (future context API), richer multi-line operator docs, cursor snippet insertion | Author productivity | Medium |
| M8 Monaco Editor Test Coverage | Frontend tests: fallback path, race guard, completion depth heuristic, semantic debounce | Regression safety | Medium |
| L6 Semantic Validation Opt-In Toggle | Persisted user toggle (localStorage) to disable backend semantic calls (offline / perf) | Control / governance | Low |

---

## Implementation Artifacts (Delivered)

| File | Purpose |
|------|---------|
| `monaco/useMonacoLoader.ts` | Singleton async loader with error capture |
| `monaco/jsonlogicOperators.ts` | Operator metadata + documentation |
| `monaco/registerJsonLogicLanguage.ts` | Completion & hover registration |
| `components/MonacoExpressionEditor.tsx` | Standalone Monaco powered editor |
| `components/HybridExpressionEditor.tsx` | Wrapper: chooses Monaco vs legacy |
| Updates to Gateway / Join panels | Monaco integration via `useMonaco` prop |

---

## Technical Notes

- **Completion Heuristic**: Shallow brace depth (<=1) only, avoiding noise in nested arrays/objects.
- **Marker Namespaces**: `jsonlogic-local` (parse) and `jsonlogic-semantic` (backend) kept separate to avoid overwrites.
- **Race Guard**: A monotonic semanticVersion ensures outdated async responses cannot clobber recent edits.
- **Fallback Behavior**: If import fails, user still retains valid editing with basic validation.
- **Performance**: Monaco loaded only after first mount. No prefetch yet (can be added in H6 if metrics encourage).

---

## Remaining / Enhancement Acceptance Criteria (Mapped to New Stories)

| Story | Acceptance Criteria |
|-------|---------------------|
| H6 | Telemetry callbacks fire; load time & semantic latency tracked; optional theme switch (auto + dark + light + high contrast) |
| H7 | Operator docs extended (multi-line examples); snippet insert (tab stops); variable completion stub (even if static) |
| M8 | Test suite covers: fallback path, race guard ignoring stale responses, completions suppressed at depth>1, semantic debounce timing |
| L6 | Toggle stored in localStorage; when off, no semantic calls fire; UI indicator shows “Semantic validation off” |

---

## Risks & Mitigations (Updated)

| Risk | Current Mitigation | Follow-up |
|------|--------------------|-----------|
| No telemetry on adoption | Add metrics (H6) | Enable data-driven tuning |
| Missing variable suggestions | Provide stub + extensible provider (H7) | Improve authoring speed |
| Overcalling backend validation | Debounce now; add persistent opt-in (L6) | User-controlled load |
| Limited a11y beyond basics | Live region + labels done | Evaluate keyboard audit post-H7 |

---

## Definition of Done (H5 – Met)

- Core editing, parsing, semantic validation, fallback, accessibility, formatting, completions, and hover shipped without regressions.
- Hybrid adoption path (optional activation) present.

---

## Next Steps (Recommended Order)

1. L6 – implement semantic validation toggle (quick win; reduces backend chatter).
2. H6 – telemetry + theming (inform future performance & polish decisions).
3. M8 – lock in baseline tests before expanding complexity.
4. H7 – variable assist & richer docs (add real author value).

---

## Summary

Monaco MVP is production‑ready and integrated. Remaining enhancements have been transparently re-scoped into discrete stories to prevent scope creep while enabling focused polish for the MVP demo.

(End of Document)
