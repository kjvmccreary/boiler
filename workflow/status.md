Below is a focused backend ↔ frontend contract diff and concrete refactor list. This will let you align the React calls with the hardened backend and also explains why an instance page might still show “Running” after the last task completes.
--------------------------------------------------
## 1.	High‑impact mismatches (do these first)
--------------------------------------------------
### A Definitions list response shape Backend (DefinitionsController.GetAll): Returns ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>> with Data.Items (paged Wrapper). Frontend (workflowService.getDefinitions used by DefinitionsPage) expects a plain array (sets definitions = response). Result: definitions array will actually be the entire ApiResponseDto object or undefined logic → UI inconsistencies.

#### Fix (frontend service getDefinitions):
* Parse response.data.data.items (depending on casing; from server: SuccessResult(resp.Data.Items) so outer object shape is: { success: true, message: "", data: [ WorkflowDefinitionDto ... ]  // Because controller wraps Items directly, NOT the PagedResultDto } Important: Your controller intentionally unwraps Items (ApiResponseDto<List<WorkflowDefinitionDto>>). So service must expect data to be an array under data, NOT paged container.
#### Action: 
* Update getDefinitions to treat response.data.data as array (no pagination info available with current controller unwrap). If you want pagination later, change controller to return the paged object (Data.Items + TotalCount) and update UI accordingly.
### B Added / changed fields WorkflowDefinition now includes:
* IsArchived, ArchivedAt, PublishNotes, VersionNotes, ParentDefinitionId, Tags, ActiveInstanceCount (on DTO), IsPublished, PublishedAt. React grid currently ignores IsArchived; you might want a filter or hide archived items. If archiving should remove from list, DefinitionService.GetAllAsync currently does not filter IsArchived=false. Add filter on backend or filter client-side.
#### Backend currently: query = _context.WorkflowDefinitions.Where(d => d.TenantId == tenantId) No IsArchived filter => Archived definitions will appear. If that’s not desired, add condition or add a query param archived=false.

### C
Start Instance navigation Start instance endpoint returns ApiResponseDto<WorkflowInstanceDto>, not a raw { id }. Frontend uses response.id. Should use response.data.id. Fix handleStartInstance: const instance = await workflowService.startInstance(...); navigate(/app/workflow/instances/${instance.data.id});

### D. Instance completion status mismatch Likely causes:
1.	Backend engine not marking WorkflowInstance.Status=Completed (maybe last human task executor doesn’t call PublishInstanceCompletedAsync or doesn’t set CompletedAt / Status).
2.	React instance detail page may fetch status from /api/workflow/instances/{id} (returns ApiResponseDto<WorkflowInstanceDto>) but uses stale cached state after completing last task (no refetch).
3.	Tasks completion POST might return updated task but instance list not refreshed.
### Checklist to isolate:
* After final task completion, inspect DB row WorkflowInstances.Status and CompletedAt.
* Call GET /api/workflow/instances/{id}/status directly and see if status there is Completed. If DB shows Completed but the instance page shows Running, frontend caching / parsing issue. If DB shows Running, engine didn’t finalize. Check task completion executor pipeline for final node detection.
### E
Permissions naming Controllers use literal strings: RequiresPermission("workflow.view_instances"), "workflow.start_instances", "workflow.manage_instances" DefinitionsController uses Permissions.Workflow.ViewDefinitions etc (constants). Ensure token has BOTH forms (or unify) if your dynamic permission builder expects consistent naming (e.g. workflow.view_definitions vs workflow.view_instances). Mismatch = 403 then masked by generic error handling.

---

## 2.	Frontend service refactors (proposed shapes)

Assumed backend wrapper: ApiResponseDto<T> => { success: bool, message: string|null, data: T|null, errors?: [{code,message,...}] }

Adjust each:

* getDefinitions() Current: expects array New: const resp = await http.get('/api/workflow/definitions'); if (!resp.data?.success) throw ... return resp.data.data; // array of WorkflowDefinitionDto

* publishDefinition(id, body) Expect resp.data.success.

* startInstance() Return resp.data.data (WorkflowInstanceDto)

* getInstance(id) GET /api/workflow/instances/{id} Return resp.data.data

* getInstanceStatus(id) Return resp.data.data (InstanceStatusDto)

* getRuntimeSnapshot(id) Return resp.data.data (InstanceRuntimeSnapshotDto)

* terminateInstance(id) DELETE returns ApiResponseDto<bool>

* signalInstance(id, dto) POST id/signal, returns ApiResponseDto<WorkflowInstanceDto>

* unpublishDefinition(id) POST /api/workflow/definitions/{id}/unpublish => ApiResponseDto<WorkflowDefinitionDto>

* archiveDefinition(id) POST /api/workflow/definitions/{id}/archive => ApiResponseDto<WorkflowDefinitionDto>

* terminateDefinitionInstances(id) POST /api/workflow/definitions/{id}/terminate-running returns ApiResponseDto<object> with data { terminated: number } Return resp.data.data.terminated.

## 3.	Recommended backend improvements (optional but clarifying)

### A
DefinitionsController.GetAll Return full paging metadata (TotalCount) instead of just Items: return Ok(resp); // keep ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>> Frontend then can show counts and pagination server-side. OR add query param flatten=true to keep current behavior.
### B
Filter archived drafts Add query param includeArchived (default false): if (!request.IncludeArchived) query = query.Where(d => !d.IsArchived);
### C
Instance finalization Verify in HumanTaskExecutor (or TaskService.CompleteTaskAsync) you call:
* Update instance: if no remaining active tasks and current node is End => set Status=Completed, CompletedAt=UtcNow, UpdatedAt=UtcNow
* PublishInstanceCompletedAsync
If missing, implement.

## 4.	Concrete code changes (frontend service example)

(Provide only if you want; request if needed. Not adding unless you confirm.)


## 5.	Quick diff checklist (copyable)

Frontend expects | Backend returns | Fix Array (definitions) | ApiResponseDto<List<WorkflowDefinitionDto>> | Use data.data response.id (start) | ApiResponseDto<WorkflowInstanceDto> | data.data.id No archived filter | Includes archived | Filter client or backend Status stale | Instance maybe completed | Refetch or fix completion code Permission names mixed | Mixed constant/raw strings | Normalize permission naming

## 6.	Action order

1. Fix workflowService response parsing.
2.	Fix startInstance navigation (use data.id).
3.	Verify engine sets instance completion.
4.	Add archived filter if desired.
5.	Normalize permission claim names.
6.	(Optional) Return proper paging metadata.


## 7.	Need from you (to finish instance issue)

Send:
* Task completion endpoint code (controller + service).
    * See boiler\src\services\WorkflowService\Controllers\TasksController.cs, line 151 for Task Completion method.
* HumanTaskExecutor or equivalent.
    * See C:\Users\mccre\dev\boiler\src\services\WorkflowService\Engine\Executors\HumanTaskExecutor.cs
* WorkflowInstance row after last task completion (Status, CurrentNodeIds, CompletedAt).
    * I'll provide when we get this info.

I’ll then give the minimal patch to mark completion properly.
