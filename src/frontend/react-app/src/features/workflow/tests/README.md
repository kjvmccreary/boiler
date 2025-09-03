# Workflow Test Suites

Folders:
- dsl: Pure DSL serialization/validation tests.
- service: REST client / API abstraction layer tests.
- builder: ReactFlow builder component & node editing tests.
- operations: Definitions / Instances / Tasks UI tests.
- integration-lite: Narrow vertical slice tests (e.g. draft->publish->start->complete using MSW).

Use shared helpers from: src/test/utils/testHelpers.tsx
DSL JSON fixtures: src/test/fixtures/workflow/
