PHASE 3 STORY BACKLOG (TRIAGED)
A. A/B TEST / FEATURE FLAG STRATEGY
~~A1. Strategy Kind “abTest” (Deterministic Variant Selection)
•	Intent: Enable a gateway to fan out to one of N target branches based on stable hashing (e.g., userId, tenantId, or custom key in context).
•	Scope:
•	New strategy kind: abTest
•	Properties: { kind:"abTest", config:{ keyPath:"user.id", variants:[{weight:70,target:"pathA"},{weight:30,target:"pathB"}] } }
•	Deterministic hash: (key + gatewayId + definitionVersion) → bucket → weighted variant selection.
•	Always returns exactly one variant unless allowMultiple = true (out of scope here).
•	Acceptance:
•	Same instance context / key always yields same variant.
•	Weight distribution approximates target ratios across large sample (integration smoke with synthetic simulation).
•	Decision logged in _gatewayDecisions diagnostics: { strategy:"abTest", variant, weight, key, hash }.
•	Dependencies: None beyond existing strategy registry.
•	Risk: Low.
•	Triage: MVP (Critical).~~
~~A2. External Feature Flag Provider Plug-In
•	Intent: Allow strategy to ask an injected provider whether a flag is on → pick branch A (flag on) else branch B.
•	Scope:
•	config: { flag:"newFlowX", onTarget:"nodeA", offTarget:"nodeB", required=true }
•	IFeatureFlagProvider abstraction + default Noop (always false).
•	Fallback if required=true and provider unavailable → offTarget (logged).
•	Acceptance:
•	If provider returns true/false decisions reflect it.
•	Outage path produces deterministic fallback and an event FeatureFlagFallback.
•	Dependencies: DI registration.
•	Risk: Medium (external latency).
•	Triage: MVP (Useful for progressive rollout).~~
~~A3. Experiment Enrollment Snapshot (Persist Enrollment ID)
•	Intent: Persist chosen experiment metadata in context to keep consistent if key changes mid-flow.
•	Scope:
•	_experiments[gatewayId]: { variant:"A", assignedHash:"...", keyValue:"123" }
•	On re-evaluation, reuse stored variant (do not recompute).
•	Acceptance: Changing context key after decision does not alter variant.
•	Risk: Low.
•	Triage: MVP (Consistency).~~
~~A4. Override / Forced Variant via Context
•	Intent: Permit forced override (e.g., QA or targeted user) with context path _overrides.gateway[gatewayId]="variantName".
•	Acceptance: Override logged, variant respected even if weight mismatch.
•	Risk: Low.
•	Triage: Post-MVP (Optional).
A5. Metrics Hook / Outbox Event “ExperimentAssigned”
•	Intent: Emit outbox message for experiment telemetry.
•	Acceptance: Event contains: gatewayId, variant, definitionId, instanceId, tenantId, hash.
•	Risk: Low.
•	Triage: Post-MVP.~~
A6. Multi-Variant (Top-K) Selection (allowMultiple)
•	Intent: Select K distinct variants (e.g., multi-arm test).
•	Acceptance: When allowMultiple:true & topK:N, returns up to N unique targets deterministically stable.
•	Risk: Medium (complexity vs need).
•	Triage: Post-MVP.
A7. Runtime Rebalance (Dynamic Weight Refresh)
•	Intent: Periodically reload variant weights from provider (e.g. remote config).
•	Acceptance: Weight refresh does not change already-assigned variant for existing instances.
•	Risk: Medium/High (consistency).
•	Triage: Post-MVP.
A8. Sampling Guard / Minimum Traffic Threshold
•	Intent: Skip test (fallback variant) if insufficient sample size expectation (e.g., tenant with < M users).
•	Acceptance: Condition documented & logged.
•	Risk: Low.
•	Triage: Post-MVP.
B. ADVANCED JOIN / WAIT (CONVERGENCE) STRATEGIES
~~B1. Convergence Policy: “quorum”
•	Intent: Flow continues when X% (or fraction) of parallel branches arrive.
•	Scope:
•	join.mode = "quorum", config: { thresholdPercent: 60 } or thresholdCount override.
•	Satisfied when arrivals / total ≥ threshold.
•	Acceptance: Event includes quorumSatisfied:true + threshold data.
•	Risk: Low.
•	Triage: MVP (Common partial convergence use case).~~
~~B2. Convergence Timeout
•	Intent: If join not satisfied within duration, escalate (complete with partial, fail, or route to timeoutTarget).
•	Scope:
•	join.timeout: { seconds: 900, onTimeout:"route" | "fail" | "force" , target:"timeoutHandler" }
•	Background scanner: detect pending joins past timeout; produce ParallelJoinTimeout event.
•	Acceptance: Deterministic action; context updates join.timeoutTriggered:true.
•	Dependencies: Timer/worker reuse or new lightweight query.
•	Risk: Medium (operational timing).
•	Triage: MVP (Prevents indefinite hang).~~
B3. Partial Cancellation Policy Extensions
•	Intent: Fine-grained control after satisfaction.
•	cancelRemainingMode: "immediate" | "graceful" | "none"
•	graceful → mark branches as soft-cancel (allow open tasks to finish but no new edges).
•	Acceptance: Markers in context & events for soft-cancel.
•	Risk: Medium.
•	Triage: Post-MVP (Refinement).
B4. Late Arrival Handling Mode
•	Intent: A branch arrives after join satisfied.
•	lateArrival = "ignore" | "error" | "side-channel" (send to alt node) | "append"
•	Acceptance: Setting enforced, event emitted.
•	Risk: Low.
•	Triage: Post-MVP.
B5. Expression Enhancements (Access to Branch Metadata)
•	Intent: Provide richer variables: per-branch task statuses, durations.
•	Acceptance: _joinEval enriched; tests confirm expression referencing e.g. branchTaskCounts.
•	Risk: Low.
•	Triage: Post-MVP.
B6. Branch Weighting with Weighted Quorum
•	Intent: Some branches count more toward quorum.
•	join.weights: { branchNodeId: weight }
•	Satisfied when sum(arrivedWeights) >= thresholdWeight.
•	Acceptance: Weighted math correct.
•	Risk: Medium.
•	Triage: Post-MVP.
B7. Replay / Dry-Run Join Simulation API
•	Intent: Given hypothetical arrivals set, show wouldSatisfy + variant route.
•	Acceptance: Non-mutating endpoint returns structured decision preview.
•	Risk: Low.
•	Triage: Post-MVP.
B8. Audit / Drift Detector for Joins
•	Intent: Background job ensures no join stuck beyond maximum allowed aging SLA.
•	Acceptance: Report or event for each stale join.
•	Risk: Low.
•	Triage: Post-MVP.
C. CROSS-CUTTING FOUNDATIONAL STORIES
C1. Context Size Guard / Pruning Strategy
•	Intent: Avoid unbounded growth of _gatewayDecisions & _parallelGroups.
•	Acceptance: Configurable max history per gateway; oldest pruned; event GatewayDecisionPruned.
•	Risk: Medium (data retention vs diagnostics).
•	Triage: MVP (Prevent memory bloat early).
C2. Unified Strategy Diagnostic Schema Versioning
•	Intent: Add _diagnosticsVersion to each decision for forward compatibility.
•	Acceptance: Version increments on shape change; fallback parsers unaffected.
•	Risk: Low.
•	Triage: MVP (Future-proofing).
C3: deterministic hash utility (DONE ✅)
~~C4: strategy config validation (DONE ✅)~~
C5. Metrics / Telemetry Hook Points
•	Intent: Abstract IWorkflowTelemetrySink; emit counters: gateway.strategy.count, join.wait.time.
•	Acceptance: No-op default; easy to plug Prometheus/App Insights later.
•	Risk: Low.
•	Triage: Post-MVP (Optional until ops asks).
C6. Documentation / Schema Contracts
•	Intent: Machine-readable JSON Schema for gateway strategies & join config (expose via diagnostics endpoint).
•	Acceptance: Schema served at /workflow/meta/strategy-schema (if authorized).
•	Risk: Low.
•	Triage: Post-MVP.
D. TEST / QUALITY STORIES
D1. Deterministic Variant Stability Test
•	Acceptance: 1000 simulated contexts → 100% stable assignment reruns.
•	Triage: MVP.
D2. Distribution Sanity Test
•	Acceptance: 50k assignments → variance within ±3% of specified weights (statistical tolerance).
•	Triage: Post-MVP (optional depth).
D3. Quorum Join Behavior Matrix
•	Cases: exact threshold, below threshold, above threshold, late arrival after satisfaction.
•	Triage: MVP.
D4. Timeout Path Test
•	Simulate join pending beyond timeout; ensure chosen action executes exactly once.
•	Triage: MVP.
D5. Context Prune Test
•	Populate > max decisions; assert oldest pruned & event emitted.
•	Triage: MVP.
D6. Validation Failure Tests (Invalid abTest config) (DONE ✅)
D7. Expression Mode Safety Tests (malformed expressions)
•	Triage: Post-MVP.
------------------------------------------------------------------
MVP CUT (RECOMMENDED FOR INITIAL PHASE 3 INCREMENT)
Critical (ship first):
•	A1: abTest strategy core (DONE ✅)
•	A2: Feature flag provider (basic on/off) (DONE ✅)
•	A3: Enrollment snapshot persistence (DONE ✅)
•	B1: quorum join mode (DONE ✅)
•	B2: join timeout (DONE ✅)
•	C1: context history pruning (DONE ✅)
•	C2: diagnostics version tag (DONE ✅)
•	C3: deterministic hash utility (DONE ✅)
•	C4: strategy config validation (DONE ✅)
•	D1, D3, D4, D5 (pending), D6 (DONE ✅)
Nice-to-have but deferrable:
•	Override (A4) (DONE ✅)
•	Experiment assigned outbox event (A5) (DONE ✅)
•	Partial cancellation refinements (B3)
•	Weighted quorum (B6)
•	Telemetry sink (C5)
•	Distribution deep test (D2)
Deferred / Advanced:
•	Multi-variant selection (A6)
•	Runtime rebalance (A7)
•	Sampling guard (A8)
•	Late arrival routing (B4)
•	Enhanced expression variables (B5)
•	Replay simulation API (B7)
•	Stale join auditor (B8)
•	JSON schema endpoint (C6)
•	Expression safety deep tests (D7)
------------------------------------------------------------------
RECOMMENDED SEQUENCE
1.	Foundations: C3 (Hasher) → C4 (Validation) → C2 (Diagnostics version) → C1 (Pruning)
2.	A/B Core: A1 → A3 → A2 (flag provider) → minimal tests (D1, D6)
3.	Join Extensions: B1 (quorum) → B2 (timeout) → tests (D3, D4)
4.	Hardening: pruning test (D5), integration with existing diagnostics
5.	Optional stretch (end of sprint): A4 or A5 if capacity
------------------------------------------------------------------
ACCEPTANCE SUMMARY (MVP EXIT CHECK)
•	Gateways of kind abTest select stable variant, recorded with diagnostics version.
•	Feature flag strategy resolves and can fallback gracefully.
•	Join supports quorum + timeout producing correct events.
•	Context growth bounded by pruning (configurable).
•	Validation blocks invalid weighting configs.
•	Tests green for deterministic, quorum, timeout, pruning, validation.
------------------------------------------------------------------
RISKS / WATCHPOINTS
•	Immediate SaveChanges for waiting tasks (recent fix) → ensure not regressed by new strategy tasks.
•	Hash algorithm consistency: pick one and do not change seeds mid-production (store algorithm + seed in decision).
•	Timeout processing concurrency: ensure idempotency (mark processed join to avoid double actions).
•	Pruning: must never remove the most recent decision in active logic (prune oldest only).
------------------------------------------------------------------
METADATA TO ENRICH DECISION OBJECT (PHASE 3 ADDITIONS) Add fields for new strategies:
•	strategyConfigHash
•	diagnosticsVersion (increment to e.g. 2)
•	experiment: { variant, weightsHash, key, assignedAtUtc }
•	join: add quorumThreshold / timeoutApplied flags where relevant.
