import { apiClient } from './api.client';
import {
  InstanceStatus,
  TASK_STATUSES,
  type TaskStatus
} from '@/types/workflow';
import type {
  WorkflowDefinitionDto,
  CreateWorkflowDefinitionDto,
  UpdateWorkflowDefinitionDto,
  PublishDefinitionRequestDto,
  WorkflowInstanceDto,
  StartInstanceRequestDto,
  SignalInstanceRequestDto,
  TerminateInstanceRequestDto,
  RetryInstanceRequestDto,
  MoveToNodeRequestDto,
  WorkflowTaskDto,
  TaskSummaryDto,
  CompleteTaskRequestDto,
  ClaimTaskRequestDto,
  AssignTaskRequestDto,
  ResetTaskRequestDto,
  WorkflowEventDto,
  WorkflowStatsDto,
  BulkCancelInstancesRequestDto,
  BulkOperationResultDto,
  TaskStatisticsDto,
  PagedResultDto,
  InstanceRuntimeSnapshotDto,
  ValidationResultDto,
  CreateNewVersionRequestDto
} from '@/types/workflow';

export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  errors?: string[] | { code?: string; message?: string }[] | Record<string, unknown>;
  data: T;
}

export interface InstanceStatusDto {
  instanceId: number;
  status: InstanceStatus | string | number;
  currentNodeIds: string;
  currentNodeNames: string[];
  progressPercentage: number;
  lastUpdated: string;
  runtime: string;
  activeTasksCount: number;
  errorMessage?: string | null;
}

function unwrap<T>(payload: unknown): T {
  if (payload && typeof payload === 'object' && 'success' in payload && 'data' in payload) {
    const r = payload as ApiResponse<T>;
    if (!r.success) {
      const errs = Array.isArray(r.errors)
        ? r.errors.map(e => (typeof e === 'string' ? e : (e as any).message ?? JSON.stringify(e)))
        : r.message
          ? [r.message]
          : ['Request failed'];
      throw new Error(errs.join('; '));
    }
    return r.data;
  }
  return payload as T;
}

// --- Status normalization maps ---
const INSTANCE_STATUS_NUM_MAP: Record<number, InstanceStatus> = {
  1: InstanceStatus.Running,
  2: InstanceStatus.Completed,
  3: InstanceStatus.Cancelled,
  4: InstanceStatus.Failed,
  5: InstanceStatus.Suspended
};
const TASK_STATUS_NUM_MAP: Record<number, TaskStatus> = {
  1: 'Created',
  2: 'Assigned',
  3: 'Claimed',
  4: 'InProgress',
  5: 'Completed',
  6: 'Cancelled',
  7: 'Failed'
};

function normalizeInstanceStatus(v: unknown): InstanceStatus {
  if (typeof v === 'string' && Object.values(InstanceStatus).includes(v as InstanceStatus)) return v as InstanceStatus;
  if (typeof v === 'number') return INSTANCE_STATUS_NUM_MAP[v] ?? InstanceStatus.Running;
  if (typeof v === 'string') {
    const low = v.toLowerCase();
    return Object.values(InstanceStatus).find(s => s.toLowerCase() === low) ?? InstanceStatus.Running;
  }
  return InstanceStatus.Running;
}
function normalizeTaskStatus(v: unknown): TaskStatus {
  if (typeof v === 'string') {
    const match = TASK_STATUSES.find(s => s.toLowerCase() === v.toLowerCase());
    if (match) return match;
  }
  if (typeof v === 'number') return TASK_STATUS_NUM_MAP[v] ?? 'Created';
  return 'Created';
}

function mapInstance(i: WorkflowInstanceDto): WorkflowInstanceDto {
  return { ...i, status: normalizeInstanceStatus(i.status) };
}
function mapInstances(list: WorkflowInstanceDto[]) {
  return list.map(mapInstance);
}
function mapTask(t: WorkflowTaskDto): WorkflowTaskDto {
  return { ...t, status: normalizeTaskStatus(t.status) };
}
function mapTasks(list: WorkflowTaskDto[]) {
  return list.map(mapTask);
}

// --- Filters ---
export interface WorkflowDefinitionsFilters {
  search?: string;
  published?: boolean;
  tags?: string;
  includeArchived?: boolean;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  desc?: boolean;
}
export interface WorkflowInstancesFilters {
  status?: InstanceStatus;
  workflowDefinitionId?: number;
  startedByUserId?: number;
  page?: number;
  pageSize?: number;
  searchTerm?: string;
}
export interface WorkflowTasksFilters {
  mine?: boolean;
  status?: TaskStatus;
  workflowInstanceId?: number;
  assignedToUserId?: number;
  assignedToRole?: string;
  overdue?: boolean;
  page?: number;
  pageSize?: number;
}
export interface WorkflowEventsFilters {
  instanceId?: number;
  eventType?: string;
  userId?: number;
  from?: Date;
  to?: Date;
  page?: number;
  pageSize?: number;
}

export interface GraphValidationResult {
  success: boolean;
  errors: string[];
  warnings: string[];
}

export class WorkflowService {
  // ----------------- Definitions -----------------
  async getDefinitions(filters?: WorkflowDefinitionsFilters): Promise<WorkflowDefinitionDto[]> {
    const paged = await this.getDefinitionsPaged(filters);
    return paged.items;
  }

  async getDefinitionsPaged(filters?: WorkflowDefinitionsFilters): Promise<PagedResultDto<WorkflowDefinitionDto>> {
    const params = new URLSearchParams();
    if (filters?.search) params.append('search', filters.search);
    if (filters?.published !== undefined) params.append('published', String(filters.published));
    if (filters?.tags) params.append('tags', filters.tags);
    if (filters?.includeArchived !== undefined) params.append('includeArchived', String(filters.includeArchived));
    if (filters?.page) params.append('page', String(filters.page));
    if (filters?.pageSize) params.append('pageSize', String(filters.pageSize));
    if (filters?.sortBy) params.append('sortBy', filters.sortBy);
    if (filters?.desc !== undefined) params.append('desc', String(filters.desc));
    const resp = await apiClient.get(`/api/workflow/definitions${params.size ? `?${params}` : ''}`);
    return unwrap<PagedResultDto<WorkflowDefinitionDto>>(resp.data);
  }

  async getDefinition(id: number) {
    const resp = await apiClient.get(`/api/workflow/definitions/${id}`);
    return unwrap<WorkflowDefinitionDto>(resp.data);
  }

  async createDraft(request: CreateWorkflowDefinitionDto) {
    const resp = await apiClient.post('/api/workflow/definitions/draft', request);
    return unwrap<WorkflowDefinitionDto>(resp.data);
  }

  async updateDefinition(id: number, request: UpdateWorkflowDefinitionDto) {
    const resp = await apiClient.put(`/api/workflow/definitions/${id}`, request);
    return unwrap<WorkflowDefinitionDto>(resp.data);
  }

  async validateDefinitionJson(jsonDefinition: string): Promise<GraphValidationResult> {
    try {
      const resp = await apiClient.post('/api/workflow/definitions/validate', { JSONDefinition: jsonDefinition });
      const d: any = resp.data;
      const payload = d.data ?? d;
      return {
        success: (payload.isValid ?? payload.success ?? true) && ((payload.errors?.length ?? 0) === 0),
        errors: payload.errors ?? [],
        warnings: payload.warnings ?? []
      };
    } catch (err: any) {
      const d = err?.response?.data;
      let errors: string[] = [];
      let warnings: string[] = [];
      if (Array.isArray(d?.errors)) {
        errors = (d.errors as any[]).map(e => (typeof e === 'string' ? e : e.message ?? JSON.stringify(e)));
      } else if (Array.isArray(d?.data?.errors)) {
        errors = d.data.errors;
      } else if (d?.message) {
        errors = [d.message];
      } else {
        errors = ['Validation failed (unknown server response).'];
      }
      if (Array.isArray(d?.warnings)) warnings = d.warnings;
      return { success: false, errors, warnings };
    }
  }

  async validateDefinitionById(id: number): Promise<GraphValidationResult> {
    try {
      const resp = await apiClient.get(`/api/workflow/definitions/${id}/validate`);
      const d: any = resp.data;
      return {
        success: !!d.success && ((d.errors?.length ?? 0) === 0),
        errors: d.errors ?? [],
        warnings: d.warnings ?? []
      };
    } catch (err: any) {
      const d = err?.response?.data;
      return {
        success: false,
        errors: d?.errors ?? [d?.message ?? 'Validation request failed'],
        warnings: d?.warnings ?? []
      };
    }
  }

  async publishDefinition(id: number, opts?: PublishDefinitionRequestDto) {
    try {
      const resp = await apiClient.post(`/api/workflow/definitions/${id}/publish`, {
        publishNotes: opts?.publishNotes,
        forcePublish: opts?.forcePublish ?? false
      });
      return unwrap<WorkflowDefinitionDto>(resp.data);
    } catch (err: any) {
      const d = err?.response?.data;
      // Extract meaningful messages
      let errors: string[] = [];
      if (Array.isArray(d?.errors)) {
        errors = d.errors.map((e: any) =>
          typeof e === 'string'
            ? e
            : (e?.message ?? e?.code ?? JSON.stringify(e)));
      } else if (d?.errors) {
        errors = [typeof d.errors === 'string' ? d.errors : JSON.stringify(d.errors)];
      }
      if (errors.length === 0 && typeof d?.message === 'string') {
        errors = [d.message];
      }
      if (errors.length === 0) {
        errors = ['Publish failed'];
      }

      // Harden: if we ended with a generic placeholder but original axios error had a better message, prefer it.
      const genericSet = /^(publish failed|operation failed)$/i;
      const rawAxiosMessage: string | undefined = err?.message;
      if (genericSet.test(errors[0]) && typeof d?.message === 'string') {
        errors[0] = d.message;
      } else if (genericSet.test(errors[0]) && rawAxiosMessage && !genericSet.test(rawAxiosMessage)) {
        errors[0] = rawAxiosMessage;
      }

      const e = new Error(errors[0]);
      (e as any).errors = errors;
      throw e;
    }
  }

  async validateThenPublish(id: number, opts?: PublishDefinitionRequestDto) {
    const vr = await this.validateDefinitionById(id);
    if (!vr.success) return { validation: vr, published: undefined };
    try {
      const published = await this.publishDefinition(id, opts);
      return { validation: vr, published };
    } catch (e: any) {
      return { validation: vr, published: undefined, error: e };
    }
  }

  async deleteDefinition(id: number) {
    const resp = await apiClient.delete(`/api/workflow/definitions/${id}`);
    return !!unwrap<boolean>(resp.data);
  }

  async unpublishDefinition(id: number) {
    const resp = await apiClient.post(`/api/workflow/definitions/${id}/unpublish`, {});
    return unwrap<WorkflowDefinitionDto>(resp.data);
  }

  async archiveDefinition(id: number) {
    const resp = await apiClient.post(`/api/workflow/definitions/${id}/archive`, {});
    return unwrap<WorkflowDefinitionDto>(resp.data);
  }

  async terminateDefinitionInstances(id: number) {
    const resp = await apiClient.post(`/api/workflow/definitions/${id}/terminate-running`, {});
    return unwrap<{ terminated: number }>(resp.data);
  }

  async validateDefinition(jsonDefinition: string): Promise<ValidationResultDto> {
    const resp = await apiClient.post('/api/workflow/definitions/validate', { JSONDefinition: jsonDefinition });
    return unwrap<ValidationResultDto>(resp.data);
  }

  async createNewVersion(id: number, request: CreateNewVersionRequestDto) {
    const resp = await apiClient.post(`/api/workflow/definitions/${id}/new-version`, request);
    return unwrap<WorkflowDefinitionDto>(resp.data);
  }

  async revalidateDefinition(id: number) {
    const resp = await apiClient.post(`/api/workflow/definitions/${id}/revalidate`, {});
    return unwrap<ValidationResultDto>(resp.data);
  }

  // ----------------- Instances -----------------
  async getInstances(filters?: WorkflowInstancesFilters) {
    const paged = await this.getInstancesPaged(filters);
    return paged.items;
  }

  async getInstancesPaged(filters?: WorkflowInstancesFilters) {
    const params = new URLSearchParams();
    if (filters?.status) params.append('status', filters.status);
    if (filters?.workflowDefinitionId) params.append('workflowDefinitionId', String(filters.workflowDefinitionId));
    if (filters?.startedByUserId) params.append('startedByUserId', String(filters.startedByUserId));
    if (filters?.page) params.append('page', String(filters.page));
    if (filters?.pageSize) params.append('pageSize', String(filters.pageSize));
    if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);
    const resp = await apiClient.get(`/api/workflow/instances${params.size ? `?${params}` : ''}`);
    const paged = unwrap<PagedResultDto<WorkflowInstanceDto>>(resp.data);
    paged.items = mapInstances(paged.items);
    return paged;
  }

  async getInstance(id: number) {
    const resp = await apiClient.get(`/api/workflow/instances/${id}`);
    return mapInstance(unwrap<WorkflowInstanceDto>(resp.data));
  }

  async getInstanceStatus(id: number): Promise<InstanceStatusDto> {
    const resp = await apiClient.get(`/api/workflow/instances/${id}/status`);
    const dto = unwrap<InstanceStatusDto>(resp.data);
    return { ...dto, status: normalizeInstanceStatus(dto.status) };
  }

  async startInstance(request: StartInstanceRequestDto) {
    const resp = await apiClient.post('/api/workflow/instances', request);
    return mapInstance(unwrap<WorkflowInstanceDto>(resp.data)); // caller use result.id
  }

  async signalInstance(id: number, request: SignalInstanceRequestDto) {
    const resp = await apiClient.post(`/api/workflow/instances/${id}/signal`, request);
    return mapInstance(unwrap<WorkflowInstanceDto>(resp.data));
  }

  async terminateInstance(id: number) {
      // DELETE returns ApiResponse<bool> (termination success)
    const resp = await apiClient.delete(`/api/workflow/instances/${id}`);
    return unwrap<boolean>(resp.data);
  }

  async suspendInstance(id: number) {
    const resp = await apiClient.post(`/api/workflow/instances/${id}/suspend`, {});
    return mapInstance(unwrap<WorkflowInstanceDto>(resp.data));
  }

  async resumeInstance(id: number) {
    const resp = await apiClient.post(`/api/workflow/instances/${id}/resume`, {});
    return mapInstance(unwrap<WorkflowInstanceDto>(resp.data));
  }

  async retryInstance(id: number, request: RetryInstanceRequestDto) {
    const resp = await apiClient.post(`/api/workflow/admin/instances/${id}/retry`, request);
    return mapInstance(unwrap<WorkflowInstanceDto>(resp.data));
  }

  async moveInstanceToNode(id: number, request: MoveToNodeRequestDto) {
    const resp = await apiClient.post(`/api/workflow/admin/instances/${id}/move-to-node`, request);
    return mapInstance(unwrap<WorkflowInstanceDto>(resp.data));
  }

  // ----------------- Tasks -----------------
  async getTasks(filters?: WorkflowTasksFilters): Promise<WorkflowTaskDto[]> {
    const params = new URLSearchParams();
    if (filters?.mine !== undefined) params.append('mine', String(filters.mine));
    if (filters?.status) params.append('status', filters.status);
    if (filters?.workflowInstanceId) params.append('workflowInstanceId', String(filters.workflowInstanceId));
    if (filters?.assignedToUserId) params.append('assignedToUserId', String(filters.assignedToUserId));
    if (filters?.assignedToRole) params.append('assignedToRole', filters.assignedToRole);
    if (filters?.overdue !== undefined) params.append('overdue', String(filters.overdue));
    if (filters?.page) params.append('page', String(filters.page));
    if (filters?.pageSize) params.append('pageSize', String(filters.pageSize));
    const resp = await apiClient.get(`/api/workflow/tasks${params.size ? `?${params}` : ''}`);
    return mapTasks(unwrap<WorkflowTaskDto[]>(resp.data));
  }

  async getMyTasks(status?: TaskStatus) {
    const resp = await apiClient.get(`/api/workflow/tasks/mine${status ? `?status=${status}` : ''}`);
    const arr = unwrap<TaskSummaryDto[]>(resp.data);
    return arr.map(t => ({ ...t, status: normalizeTaskStatus(t.status) }));
  }

  async getTask(id: number) {
    const resp = await apiClient.get(`/api/workflow/tasks/${id}`);
    return mapTask(unwrap<WorkflowTaskDto>(resp.data));
  }

  async claimTask(id: number, request: ClaimTaskRequestDto) {
    const resp = await apiClient.post(`/api/workflow/tasks/${id}/claim`, request);
    return mapTask(unwrap<WorkflowTaskDto>(resp.data));
  }

  async completeTask(id: number, request: CompleteTaskRequestDto) {
    const resp = await apiClient.post(`/api/workflow/tasks/${id}/complete`, request);
    return mapTask(unwrap<WorkflowTaskDto>(resp.data));
  }

  async assignTask(id: number, request: AssignTaskRequestDto) {
    const resp = await apiClient.post(`/api/workflow/tasks/${id}/assign`, request);
    return mapTask(unwrap<WorkflowTaskDto>(resp.data));
  }

  async cancelTask(id: number) {
    const resp = await apiClient.post(`/api/workflow/tasks/${id}/cancel`, {});
    return mapTask(unwrap<WorkflowTaskDto>(resp.data));
  }

  async resetTask(id: number, request: ResetTaskRequestDto) {
    const resp = await apiClient.post(`/api/workflow/admin/tasks/${id}/reset`, request);
    return mapTask(unwrap<WorkflowTaskDto>(resp.data));
  }

  // ----------------- Events / Admin -----------------
  async getWorkflowEvents(filters?: WorkflowEventsFilters) {
    const params = new URLSearchParams();
    if (filters?.instanceId) params.append('instanceId', String(filters.instanceId));
    if (filters?.eventType) params.append('eventType', filters.eventType);
    if (filters?.userId) params.append('userId', String(filters.userId));
    if (filters?.from) params.append('from', filters.from.toISOString());
    if (filters?.to) params.append('to', filters.to.toISOString());
    if (filters?.page) params.append('page', String(filters.page));
    if (filters?.pageSize) params.append('pageSize', String(filters.pageSize));
    const resp = await apiClient.get(`/api/workflow/admin/events${params.size ? `?${params}` : ''}`);
    return unwrap<WorkflowEventDto[]>(resp.data);
  }

  async getWorkflowStats() {
    const resp = await apiClient.get('/api/workflow/admin/stats');
    return unwrap<WorkflowStatsDto>(resp.data);
  }

  async bulkCancelInstances(request: BulkCancelInstancesRequestDto) {
    const resp = await apiClient.post('/api/workflow/admin/instances/bulk-cancel', request);
    return unwrap<BulkOperationResultDto>(resp.data);
  }

  async getTaskStatistics() {
    const resp = await apiClient.get('/api/workflow/tasks/statistics');
    return unwrap<TaskStatisticsDto>(resp.data);
  }

  async getRuntimeSnapshot(instanceId: number): Promise<InstanceRuntimeSnapshotDto> {
    const resp = await apiClient.get(`/api/workflow/instances/${instanceId}/runtime-snapshot`);
    return unwrap<InstanceRuntimeSnapshotDto>(resp.data);
  }
}

export const workflowService = new WorkflowService();

export interface TaskCountsDto {
  available: number;
  assignedToMe: number;
  assignedToMyRoles: number;
  claimed: number;
  inProgress: number;
  completedToday: number;
  overdue: number;
  failed: number;
  totalActionable: number;
}

export async function getMyTaskSummary(): Promise<TaskCountsDto> {
  const res = await apiClient.get<ApiResponse<TaskCountsDto>>('/api/workflow/tasks/mine/summary');
  return unwrap<TaskCountsDto>(res.data);
}
