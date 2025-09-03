# Operations UI Tests (Temporarily Skipped)

These tests were intentionally skipped after MVP validation because they require a full application provider harness (Auth, Tenant, Permission, Theme, QueryClient).

TODO(WF-OPS):
1. Implement `AppTestWrapper` (src/test/utils/AppTestWrapper.tsx).
2. Provide permission mocking helper (src/test/utils/mockPermissionContext.ts).
3. Restore original scenarios:
   - Definitions list: empty, list + publish success, publish error
   - MyTasks: list tasks, claim, complete (with optimistic UI verification)
4. Add negative cases (claim conflict, permission denial, missing permission button visibility).
5. Add coverage script (see main workflow README).

Do not delete the placeholder specs; they document intended coverage.
