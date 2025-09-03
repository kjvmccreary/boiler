# Workflow Test Fixtures (TODO Expansion)

Current fixtures:
- approval.basic.json (simple human task + gateway)

Planned additions (TODO):
1. invalid.multi-start.json  
   - Two start nodes; used to assert validation error aggregation.

2. invalid.unreachable.json  
   - Contains an isolated node; drives "Unreachable nodes" path.

3. invalid.missing-end.json  
   - No end node; asserts required terminal node validation.

4. warning.gateway-no-condition.json  
   - Gateway without condition to trigger specific warning path (if warnings separate from errors).

5. timer.basic.json  
   - Includes a start -> timer -> automatic -> end chain validating timer serialization.

6. branching.timer-and-human.json  
   - Parallel edges after a timer (even if engine executes sequentially now) to future-proof edge traversal tests.

Action Items (TODO):
- Add the JSON files above.
- Extend DSL validation tests to load them dynamically instead of inline definitions.
- Use in integration-lite tests for negative publish attempts.

NOTE:
Keep fixtures minimal (remove extraneous properties) to reduce maintenance friction.
