# Workflow Test Suites

Folders:
- dsl: Pure DSL serialization/validation tests.
- service: REST client / API abstraction layer tests.
- builder: ReactFlow builder component & node editing tests.
- operations: Definitions / Instances / Tasks UI tests. (Currently skipped pending AppTestWrapper)
- integration-lite: Narrow vertical slice tests (draft->publish->start->complete using MSW).

Use shared helpers from: src/test/utils/test-utils.tsx
DSL JSON fixtures: src/test/fixtures/workflow/

## TODO (Deferred Enhancements)
- AppTestWrapper (see src/test/utils/AppTestWrapper.tsx)
- Permission mocking helper (see src/test/utils/mockPermissionContext.ts)
- Fixture expansion (see src/test/fixtures/workflow/README.md)
- Workflow-only coverage script:
  Add to package.json scripts:
  "test:workflow:coverage": "vitest run --coverage src/features/workflow/tests"
- Re-enable operations tests after wrapper: restore definitions & tasks tests from git history.
