## Stories for “Definition Immutability Enforcement” (Backend + Frontend + Tests)
<s>### 1.	Backend: Service-Level Unpublish Rule
* Add IDefinitionService.UnpublishAsync(definitionId, request) with rule: reject if any WorkflowInstances (Status Running or Suspended) exist for (DefinitionId, Version).
* Return error with counts (running, suspended, completed).
* Emit workflow.definition.unpublished outbox event on success.
* Remove direct DB mutation logic from DefinitionsController.Unpublish and delegate to service.
* Add UnpublishDefinitionRequestDto { bool ForceTerminateAndUnpublish }.
* If ForceTerminateAndUnpublish = true: atomically cancel all running/suspended instances (Status → Cancelled, CompletedAt=UtcNow) then unpublish; produce per-instance cancellation events.

### 2.	Backend: JSONDefinition Mutation Guard (Persistence Layer)
* In WorkflowDbContext.SaveChangesAsync scan Modified WorkflowDefinition entries where IsPublished was true before modification; reject if JSONDefinition changed.
* Allow edits to metadata fields (Description, Tags, PublishNotes, VersionNotes, Name?) — clarify scope: enforce only JSONDefinition immutability per requirement.
* Return a domain exception with a clear message (“Published workflow JSONDefinition is immutable. Create a new version.”).

### 3.	Backend: Defensive Validation in UpdateDraftAsync
* Keep existing “cannot modify published” check.
* Harden: verify JSONDefinition cannot be null/empty when provided; ensure gateway enrichment only on drafts.
* Remove ForcePublish semantics (currently allows confusing re‑publish). If retained, enforce: ForcePublish only allowed when JSONDefinition hash unchanged; otherwise require new version.

### 4.	Backend: New Endpoint – GET /api/workflow/definitions/{id}/usage
* Returns { definitionId, version, activeInstanceCount, runningCount, suspendedCount, completedCount, latestVersion }.
* Used by UI to decide whether to show/enable Unpublish action.

### 5.	Backend: Extend WorkflowDefinitionDto
* Add ActiveInstanceCount (running + suspended).
* Populate in Get / list endpoints via subquery (efficient: projection with correlated COUNT on statuses IN (Running, Suspended)).
* Frontend uses to badge counts.

### 6.	Backend: New Version Safety
* On CreateNewVersionAsync ensure ParentDefinitionId set (already) and copy Tags / Description optionally if request omits them.
* Return header (e.g., X-Workflow-New-Version: vN) for UI hints (optional).
### 7.	Backend: Events & Outbox
* Add PublishDefinitionUnpublishedAsync(definition) in EventPublisher.
* Add events for ForceTerminate path: workflow.instance.forceCancelledReason=“unpublish”.
* Ensure idempotency key use (already Outbox has IdempotencyKey).
### 8.	Backend: Migrations (Only if adding new columns)
* If adding ActiveInstanceCount is purely computed—no migration.
* No schema change required for immutability enforcement.
</s>
### 9.	Frontend: Disable/Inform Unpublish Action
* Before showing “Unpublish” dialog, call usage endpoint.
* If activeInstanceCount > 0:
* Show counts + two options: (a) Cancel, (b) Force terminate & unpublish (checkbox toggles ForceTerminateAndUnpublish).
* If user lacks workflow.admin (or ManageInstances permission), disable force option.
## FRONTEND MODS / ENHANCEMENTS
### 10.	Frontend: Remove Unpublish Option When Already Unpublished
* Keep Archive distinct (only allowed after unpublish OR still allowed on published? Clarify: normally archive when no longer used; we will require unpublished first for clarity and hide Archive if IsPublished = true).
### 11.	Frontend: Visual Badge
* Show chip “Active: N” for published definitions with active instances.
* Tooltip clarifies unpublish rule.
## OTHER
### 12.	Observability: Logging
* Log warning when an unpublish attempt blocked due to active instances with counts.
* Log information when ForceTerminate path cancels instances (include count).
* Log error when immutability violation attempt detected in DbContext.
### 13.	Error Model Consistency
* Standardize ApiResponseDto errors for unpublish/immutability to use code = “ImmutabilityViolation” or “ActiveInstancesPresent”.
### 14.	Documentation Update (context.md + master-remaining.md)
* After implementation, strike through “Definition Immutability Enforcement” in master-remaining.md.
* Add section to context.md describing immutability + unpublish lifecycle.
---
<s>## Test Stories (Unit / Integration)
### A. DefinitionImmutabilityTests
1.	Should_UpdateDraft_When_NotPublished
2.	Should_Fail_UpdateDraft_When_Published
3.	Should_Throw_On_Attempted_JSONDefinition_Modification_PostPublish (simulate direct EF modification to verify DbContext guard)
4.	Should_Allow_Metadata_Update_When_Published (Description/Tags only)
5.	Should_Prevent_ForcePublish_When_JSONDefinition_Changed (if ForcePublish retained)
### B. PublishAndVersioningTests
1.	Publish_Sets_IsPublished_And_PublishedAt
2.	CreateNewVersion_Increments_Version_And_IsDraft
3.	ExistingInstances_Retain_OriginalVersion_After_NewVersion_Published
### C. UnpublishRuleTests
1.	Unpublish_Succeeds_When_No_Running_Or_Suspended_Instances
2.	Unpublish_Fails_When_RunningInstances_Exist (returns counts)
3.	ForceTerminateAndUnpublish_Cancels_Running_Then_Unpublishes (instances moved to Cancelled)
4.	ForceTerminate_PartialFailure_IsAtomic (wrap in transaction; verify rollback on simulated exception)
5.	Unpublish_After_Instances_Complete_Succeeds
### D. UsageEndpointTests
1.	Usage_Returns_Correct_Counts (seed multiple statuses)
2.	Usage_Reflects_New_Instance_After_Start
3.	Usage_Reflects_Zero_After_ForceTerminateAndUnpublish
### E. EventEmissionTests
1.	Definition_Publish_Emits_Outbox_Event
2.	Definition_Unpublish_Emits_Outbox_Event
3.	ForceTerminateAndUnpublish_Emits_InstanceCancellation_Events_CountMatchesRunning
### F. MultiTenantIsolationTests (overlap)
1.	TenantA_Cannot_Unpublish_TenantB_Definition
2.	ForceTerminate_Does_Not_Cancel_OtherTenant_Instances
### G. Performance / Query Tests (Optional)
1.	ListDefinitions_Includes_ActiveInstanceCount_With_Single_Query (use interceptor or logging to assert query count)
### H. ConcurrencyTests (Optional)
1.	Concurrent_Unpublish_And_NewInstance_Start: Unpublish Should Fail After Instance Start (or instance start fails if definition unpublished mid-flight—define chosen invariant)
</s>
---
## Non-Functional Acceptance Criteria
* No direct controller mutation of WorkflowDefinition publish state (only via service).
* Attempted post-publish JSON changes rejected with 400 (or 409) and consistent error envelope.
* Unpublish endpoint latency target: < 1s for up to 500 running instances (bulk cancel done in single transaction).
* All new tests green; coverage added around immutability branches.
---
Open Decisions (Clarify Before Coding)
1.	Allow updating Name/Description/Tags after publish? (Current plan: Yes, content JSON only immutable).
2.	Keep or remove ForcePublish? (Recommend remove to reduce ambiguity.)
3.	Archive semantics: require definition to be unpublished first? (Recommend yes.)
4.	Should starting a new instance race with unpublish? (Define rule: if IsPublished read + start within same transaction; unpublish after start still allowed; race acceptable.)
Provide answers and I’ll incorporate into implementation files next.
Let me know if you want the code changes next; once implemented I will return updated master-remaining.md with “Definition Immutability Enforcement” struck through.
