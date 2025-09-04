WORKFLOW SERVICE CONTEXT (PHASE 3 STATE)
Tech Stack:
•	.NET 9, C# (modern features but raw strings avoided in tests for compatibility).
•	EF Core (WorkflowDbContext) with PostgreSQL (UseNpgsql) + Outbox pattern (WorkflowEvents + OutboxMessages).
•	Serilog logging. JSON-based workflow model.
Core Domain Concepts:
•	WorkflowDefinition: JSONDefinition holds a node/edge graph.
•	WorkflowInstance: runtime state (Context JSON, CurrentNodeIds, Status, timestamps, Version).
•	Nodes (types): start, end, gateway, automatic, humanTask, timer, join.
•	Edges: directional links; gateway edges determine branching.
•	Tasks: Human and timer nodes create WorkflowTask entries; human tasks pause until completion.
Execution Engine (WorkflowRuntime):
•	StartWorkflowAsync: initializes instance at start node, auto-progresses.
•	ContinueWorkflowAsync: loop executes active nodes until waiting or done; safety counter to prevent runaway loops.
•	ExecuteNodeInternalAsync: resolves node executor, records events, handles gateway branching.
•	CompleteTaskAsync: resumes progression after human/timer task completion (now includes join arrival logic for quorum/timeouts).
•	Parallel groups: _parallelGroups[gatewayNodeId] tracks branches, remaining, completed, join metadata.
•	Join handling: join.mode supports all|any|count|quorum|expression; quorum adds thresholdPercent/thresholdCount; timeout metadata optional.
Gateway Strategies (IGatewayStrategy): Implemented:
1.	exclusive (default / fallback)
2.	parallel
3.	abTest (A1)
4.	featureFlag (A2)
abTest Strategy:
•	Deterministic variant selection: hash(keyValue, gatewayId, definitionVersion) → bucket → weight cumulative distribution.
•	Snapshot persistence (A3): _experiments[gatewayId] = { variant, assignedHash, keyValue, assignedAtUtc } ensures stability if key changes mid-flow.
•	Override support (A4): context path _overrides.gateway[gatewayId] forces variant (no snapshot write).
•	Experiment assignment event (A5): WorkflowEvent + Outbox event ExperimentAssigned emitted on first assignment or override (not on pure snapshot reuse).
•	Diagnostics include hash, normalizedRoll, variantTable, snapshotReuse/overrideApplied flags.
Feature Flag Strategy (A2):
•	Config: { flag, onTarget, offTarget, required }
•	Uses IFeatureFlagProvider (default Noop → false).
•	On provider exception: if required=true → fallback to offTarget + FeatureFlagFallback event via IFeatureFlagFallbackEmitter; if required=false treat as off silently.
•	Tests currently cover on/off; outage fallback test still a gap.
Context & Diagnostics:
•	_gatewayDecisions: per-gateway decision history (array). Each decision has diagnosticsVersion (>=2), strategyConfigHash, selectedTargets, diagnostics object.
•	Pruning (C1): WorkflowContextPruner enforces max history per gateway; emits GatewayDecisionPruned via IGatewayPruningEventEmitter.
•	Hashing (C3): IDeterministicHasher abstraction (algorithm + seed recorded indirectly in diagnostics).
•	Strategy config hash: canonical JSON of config hashed and stored.
Join Enhancements:
•	Quorum (B1): join.mode="quorum" with thresholdPercent or thresholdCount; satisfaction when arrivals >= effective threshold.
•	Timeout (B2): join timeout metadata (timeout.seconds, onTimeout route|fail|force, target). Background JoinTimeoutWorker scans running instances; on timeout:
•	force: mark join satisfied, activate join node.
•	route: activate timeoutTarget directly.
•	fail: mark instance Failed. Emits ParallelJoinTimeout event once (timeoutTriggered flag).
•	Human task completion now triggers join arrival logic (fix added to CompleteTaskAsync).
Timeout Worker:
•	JoinTimeoutWorker (hosted service) scans instances whose context contains join timeout metadata; idempotent via timeoutTriggered.
Validation (C4):
•	WorkflowPublishValidator validates:
•	Automatic actions (presence of action.kind, webhook https URL).
•	Gateway abTest strategy: keyPath required, at least 2 variants, positive weights, weights sum to 100 ± tolerance, no duplicate targets.
•	Anonymous strategy objects normalized via serialization fallback.
Events & Outbox:
•	WorkflowEvents: major lifecycle (Instance Started/Completed/Failed/Suspended/Resumed/Cancelled), GatewayEvaluated, EdgeTraversed, ParallelJoinArrived/Satisfied/Timeout, Task events, ExperimentAssigned, FeatureFlagFallback, pruning events.
•	OutboxMessages mirror events for external dispatch (eventType workflow..).
Testing Coverage (Achieved):
•	abTest core + snapshot + stability + override + assignment event (A1/A3/A4/A5/D1).
•	Feature flag on/off (partial A2; outage fallback test missing).
•	Validation negative scenarios (C4, D6).
•	Quorum joins exact + below threshold + late arrivals (B1, D3).
•	Join timeout (force/route/fail) (B2, D4).
•	Context pruning & diagnosticsVersion (C1, C2, D5).
•	Deterministic stability (D1).
•	Experiment assignment emission logic.
•	Human task join arrival fix validated implicitly by quorum tests.
Remaining Not Covered / Potential Next Tests:
•	Feature flag provider exception path (required=true → fallback event).
•	Distribution sanity test (D2) for abTest weight distribution (statistical, optional).
•	Expression mode join malformed expression safety (D7).
•	Timeout idempotency re-scan after trigger & late arrival post-timeout (edge hardening).
•	Hasher explicit test (record algorithm + seed in decisions if required later).
•	Weighted quorum, partial cancellation, multi-variant selection (future stories not implemented).
Key JSON Context Structures:
•	_gatewayDecisions: { gatewayId: [ { diagnosticsVersion, strategy, selectedTargets, diagnostics {...} } ] }
•	_experiments: { gatewayId: { variant, assignedHash, keyValue, assignedAtUtc } }
•	_parallelGroups: { gatewayId: { branches, remaining, completed, join: { nodeId, mode, arrivals[], satisfied, thresholdCount/Percent, timeoutSeconds, timeoutAtUtc, timeoutTriggered, ... } } }
Important Integration Points:
•	Deterministic hashing influences abTest; changing seed/algorithm would alter assignments (avoid in production).
•	Snapshot ensures stability across context key changes.
•	Timeout worker relies on textual search for "timeoutSeconds" in Context; context integrity critical.
•	Pruning must not remove most recent decision; pruner enforces by trimming from oldest.
Recent Fixes/Adjustments This Session:
•	Added abTest snapshot, override, experiment assignment emission.
•	Added featureFlag strategy + fallback event emitter.
•	Added quorum join + timeout logic + background worker.
•	Fixed join arrival on human task completion (CompleteTaskAsync).
•	Added validation fallback for anonymous strategy objects.
•	Added experiment assignment emission call in GatewayEvaluator after decision recording.
•	Added join arrival handling during task completion for quorum satisfaction.
•	Comprehensive unit tests added (AbTestAdvancedTests, QuorumAndTimeoutTests, GatewayPruningTests, FeatureFlagGatewayStrategyTests, validation tests).
Known Gaps/Risks:
•	Unhandled provider outage test path.
•	Statistical distribution not validated.
•	Join expression and timeout re-run edge cases not yet covered.
Use this document to quickly restore mental model for future enhancements or test additions.
--- End of context ---
