Context Summary: A workflow definition with a gateway node intended to run in parallel (JSON includes either: "properties": { "strategy": { "kind": "parallel" } } or "properties": { "gatewayType": "parallel" } ) is parsed by BuilderDefinitionAdapter.Parse. After parsing, the resulting WorkflowNode.Properties loses (or never exposes) a usable “strategy” / “gatewayType” entry in a form that GatewayEvaluator and WorkflowRuntime can detect. Consequently, gateway strategy detection always defaults to "exclusive".
Temporary Mitigation Implemented: We added a heuristic in WorkflowRuntime.ExecuteNodeAsync:
•	If a gateway has more than one outgoing edge and no edge is labeled (true/false/else), we assume it is parallel.
•	We then fan out all outgoing paths, emit EdgeTraversed events for each, and continue.
•	This makes the ParallelGateway test pass but bypasses proper declared strategy logic.
•	GatewayEvaluator still logs GATEWAY_STRATEGY_DEFAULT (exclusive), and no GatewayEvaluated event is created for heuristic parallel fan-out (GATEWAY_EVENTS = 0 in test logs).
Observed Logs When Running Test:
•	GATEWAY_STRATEGY_DEFAULT Node=gw Kind=exclusive
•	WF_GATEWAY_PARALLEL_HEURISTIC ...
•	WF_GATEWAY_PARALLEL_DISCOVERY / MAP / FORCE_PARALLEL / PARALLEL_EMIT
•	No “GatewayEvaluated” event recorded (because evaluation bypassed GatewayEvaluator for the heuristic branch).
Root Problem (to solve in story):
1.	BuilderDefinitionAdapter.Parse likely strips or flattens the nested “strategy” object so node.Properties does not surface it as:
•	JsonElement (Object) containing { kind: "parallel" }
•	IDictionary<string,object> with key "kind"
•	Raw string "parallel"
2.	GatewayEvaluator.ExtractStrategy (and runtime TryGetDeclaredGatewayStrategy) cannot detect intended parallel strategy.
3.	As a result, we rely on a heuristic that may mis-classify multi-branch exclusive gateways without labels as parallel.
4.	GatewayEvaluated events are not emitted for heuristic-driven parallel fan-out (instrumentation gap).
Acceptance Criteria for Proper Fix:
•	When definition JSON includes: "properties": { "strategy": { "kind": "parallel", "config": { ... } } } OR "properties": { "gatewayType": "parallel" } then GatewayEvaluator logs GATEWAY_EXEC Node=... Strategy=parallel (not “exclusive”).
•	GatewayEvaluator emits a GatewayEvaluated event (Type=Gateway, Name=GatewayEvaluated) with selected including all outgoing targets for parallel.
•	WorkflowRuntime no longer needs the parallel heuristic for unlabeled multi-edge gateways (remove or guard it behind a fallback flag).
•	Existing tests updated (or an additional assertion added) to verify a GatewayEvaluated event for the parallel gateway.
•	EdgeTraversed events still emitted for each branch (e2 & e3).
•	Backward compatibility: legacy exclusive gateways still behave the same.
•	Optional: Add a test where one branch has a label but strategy=parallel still fans out all branches.
Technical Tasks:
1.	Inspect BuilderDefinitionAdapter.Parse to confirm how node.Properties is populated and why nested objects are lost or transformed.
2.	Ensure nested JSON objects in "properties" are preserved as either JsonElement or a strongly-typed structure (e.g. Dictionary<string,object>) so strategy detection works.
3.	Simplify runtime overrides: remove heuristic or make it conditional on a missing explicit strategy & feature flag.
4.	Guarantee consistent GatewayEvaluated event generation for both exclusive and parallel strategies.
5.	Add a regression test capturing the strategy object form (strategy.kind) and gatewayType alias.
Risks / Considerations:
•	Removing heuristic before parser fix will re-break tests.
•	Need to ensure no performance regression from deeper property materialization.
•	Confirm that adding nested object preservation does not affect other node types that might expect flattened properties.
Artifacts / Evidence:
•	Current passing test uses heuristic (logs show GATEWAY_PARALLEL_HEURISTIC).
•	GATEWAY_EVENTS empty in test output.
•	Manual observation: strategy parsing paths never execute (no GATEWAY_STRATEGY_DETECTED logs).
Definition JSON Example (target format): { "id":"gw", "type":"gateway", "properties":{ "strategy": { "kind":"parallel", "config": { "foo": 1 } } } }
Desired Post-Fix Log Snippets: GATEWAY_STRATEGY_DETECTED Node=gw Kind=parallel (JsonElement|Dictionary) GATEWAY_EXEC Node=gw Strategy=parallel WF_GATEWAY_SELECT Instance=... Node=gw Result=True Selected=[a,b] Phase=inline-exec EDGE_EVENTS includes e2 & e3
Summary: We currently mask a parsing gap with a heuristic. Story is to restore intentional configuration-driven parallel gateway execution and instrumentation, then remove or demote the heuristic.
Use this context to request: “Implement proper parsing and detection of parallel gateway strategy; remove heuristic once fixed.”
