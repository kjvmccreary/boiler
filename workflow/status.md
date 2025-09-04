# Workflow Gateway Strategy Status

The runtime now relies solely on declared gateway strategy:
- strategy.kind = "parallel"
- strategy = "parallel" (string form)
- gatewayType = "parallel"

Heuristic fan-out based on multiple unlabeled outgoing edges has been removed.

Current behavior (verified by tests):
- Parallel gateways emit a GatewayEvaluated event with all outgoing targets listed in selected.
- Each branch emits an EdgeTraversed event with mode = AutoAdvanceParallel.
- Exclusive (default) gateways still evaluate condition / labeled edges and emit GatewayEvaluated with a single selected branch.

Remaining enhancements (future work, not part of heuristic removal):
1. True parallel completion semantics (wait for all branches instead of early End-node completion).
2. Optional gateway strategy abstraction (IGatewayStrategy).
3. Context enrichment (_gatewayDecisions) for diagnostics & replay.
4. Barrier / join node design (if required by later stories).

Status: Heuristic removal complete and test coverage added for all three declaration variants (object, gatewayType, string).
