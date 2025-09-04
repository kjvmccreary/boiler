## Summary (Current Coverage) Present:
### MultiTenantIsolationTests only cover:
1.  TenantA_Cannot_Unpublish_TenantB_Definition
2.  ForceTerminate_Does_Not_Cancel_OtherTenant_Instances Missing (re: “cross-tenant tasks / instances are not retrievable”):
* No tests asserting a tenant cannot read another tenant’s:
* WorkflowInstances (list/detail)
* WorkflowTasks (list/my-tasks/claim/complete)
* Definitions list (another tenant’s definitions filtered out)
* Events / admin bulk operations
* No negative tests for cross-tenant StartInstance, ClaimTask, CompleteTask, TerminateInstance.
* No guard that cross-tenant ID probing returns 404/Forbidden. Important Gap: In WorkflowDbContext, tenant query filters are intentionally DISABLED in test environment (isTestEnvironment short‑circuits). So current tests do NOT validate the real production filter behavior. We need explicit scenarios or a way to re-enable filters for integration tests.

## Proposed Story Set: Tenant Audit & Guard Tests
### ✅ DONE!  Story T1: Re-enable Tenant Filters for Integration Mode
* ✅ DONE! Allow an override (e.g., env var TEST_ENABLE_TENANT_FILTERS=true) so integration tests run with real global filters.
* ✅ DONE! Add helper to spin context with filters ON vs OFF to prove difference.
### ✅ DONE! Story T2: Definition List Isolation
* ✅ DONE! Seed tenant 1 & tenant 2 definitions.
* ✅ DONE! Under tenant 1 context (controller + auth claims), GET /api/workflow/definitions should not include tenant 2 definitions.
* ✅ DONE! Same for tenant 2.
### ✅ DONE! Story T3: Instance Retrieval Isolation
* ✅ DONE! Seed one instance per tenant using same DefinitionId value (different tenant rows).
* ✅ DONE! GET /api/workflow/instances?workflowDefinitionId=… as tenant 1 should only return its own.
* ✅ DONE! GET /api/workflow/instances/{id} for other tenant → 404.
### ✅ DONE! Story T4: Start Instance Cross-Tenant Guard
* ✅ DONE! Attempt StartInstance specifying definition ID belonging to different tenant → 404 or Forbidden (define expected).
* ✅ DONE! Confirm success with same-tenant definition.
### ✅ DONE! Story T5: Task List + My Tasks Isolation
* ✅ DONE! Seed tasks across both tenants.
* ✅ DONE! Tenant 1: GET /api/workflow/tasks → only tenant 1 tasks.
* ✅ DONE! Tenant 1: GET /api/workflow/tasks/my → only tasks assigned to tenant 1 users.
* ✅ DONE! Cross-tenant task ID access → 404.
### ✅ DONE! Story T6: Task Mutation Isolation
* ✅ DONE! Claim / Complete / Cancel / Reassign using another tenant’s task ID → 404 (or Forbidden).
* ✅ DONE! Ensure no row changed in other tenant.
### ✅ DONE! Story T7: Admin Operations Isolation
* ✅ DONE! Admin bulk cancel endpoint with definitionId from another tenant → 404.
* ✅ DONE! MoveInstance / RetryInstance given other tenant instance ID → 404.
### ✅ DONE! Story T8: Unpublish + ForceTerminate Isolation (Already Partially Covered)
* ✅ DONE! Add retrieval assertions: after tenant 2 force-terminates its own instances, tenant 1 still cannot see tenant 2 cancelled instances by probing IDs.
### ✅ DONE! Story T9: Events & Usage Endpoint Isolation
* ✅ DONE! /api/workflow/definitions/{id}/usage for other tenant’s definition → 404.
* ✅ DONE! Attempt enumeration of events (/api/workflow/admin/events if exists) returns only in-tenant events.
### ⏳ Story T10: Direct EF Defensive Unit Tests (Service Layer)
* Create DefinitionService with tenantProvider returning tenant 1; manually insert tenant 2 rows; service GetAllAsync must exclude tenant 2 when filters enabled (configure context with filters ON).
* Repeat for InstanceService / TaskService.
### Story T11 (Optional): Race Condition Guard
* Simulate rapid alternating tenant contexts (two scoped DbContexts) creating tasks / reading counts; ensure no leakage / cross contamination in aggregate counts.
### Story T12 (Optional): Fuzzed ID Probing
* Randomly generate 50 nonexistent IDs (belonging to other tenant) for instances/tasks; all must return 404 and not log unhandled exceptions.

## Support Tasks
### Support S1: Test Utilities Update
* Add TestTenantContextFactory(enableFilters: bool).
* Add TenantScopedHttpClient helper that injects X-Tenant-ID header + token.
### Support S2: Assertion Helpers
* AssertContainsOnlyTenantEntities<T>(IEnumerable<T>, tenantId).
* AssertNotFound(ActionResult) convenience wrapper.
### Support S3: Seed Enhancements
* Unified MultiTenantWorkflowSeeder producing:
* N definitions per tenant
* M instances referencing each definition
* K tasks per instance (mix statuses)
* Distinct user IDs per tenant

## Acceptance Criteria
* All tenant isolation stories T2–T9 implemented.
* Each access attempt across tenants returns consistent 404 (not 403) unless security policy dictates otherwise (decide & document).
* No direct modifications observed in other tenant data after mutation attempts.
* Global filter ON integration run passes (tracked via a single test toggling the env var).
* Documentation section added: “Tenant Isolation Guarantees & Test Coverage”.

### Open Decisions (Clarify Before Implementing)
1.  Expected status code for cross-tenant resource probe: 404 (security through obscurity) vs 403 (explicit denial)?
2.  Re-enable filters in all tests or only integration suite?
3.  Do we treat definitions as discoverable names across tenants (shared library) or strictly isolated? (Assumed isolated.)
4.  Should events endpoint (if any) intentionally allow system/central admin cross-tenant view?

If you confirm these stories & decisions, I can generate the test scaffolding next (integration + unit variants) and optional context toggle.
Next Step Suggestion Confirm status code choice (404 vs 403) and whether to add filter re-enable switch. Reply with:
* “Use 404” or “Use 403”
* “Enable filters in all integration tests” or “Selective enable” Then I’ll generate code.
