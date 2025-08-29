import { apiClient } from './api.client';
import { InstanceStatus, TaskStatus } from '@/types/workflow';
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
  InstanceRuntimeSnapshotDto
} from '@/types/workflow';

// -------------------- API Response Wrapper --------------------
interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: unknown;
}

// Gracefully unwrap either ApiResponse<T> or raw T
function unwrap<T>(payload: any): T {
  if (payload && typeof payload === 'object' && 'success' in payload && 'data' in payload) {
    return (payload as ApiResponse<T>).data;
  }
  return payload as T;
}

// -------------------- Enum Normalization --------------------
const INSTANCE_STATUS_NUM_MAP: Record<number, InstanceStatus> = {
  1: InstanceStatus.Running,
  2: InstanceStatus.Completed,
  3: InstanceStatus.Cancelled,
  4: InstanceStatus.Failed,
  5: InstanceStatus.Suspended
};
const TASK_STATUS_NUM_MAP: Record<number, TaskStatus> = {
  1: TaskStatus.Created,
  2: TaskStatus.Assigned,
  3: TaskStatus.Claimed,
  4: TaskStatus.InProgress,
  5: TaskStatus.Completed,
  6: TaskStatus.Cancelled,
  7: TaskStatus.Failed
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
  if (typeof v === 'string' && Object.values(TaskStatus).includes(v as TaskStatus)) return v as TaskStatus;
  if (typeof v === 'number') return TASK_STATUS_NUM_MAP[v] ?? TaskStatus.Created;
  if (typeof v === 'string') {
    const low = v.toLowerCase();
    return Object.values(TaskStatus).find(s => s.toLowerCase() === low) ?? TaskStatus.Created;
  }
  return TaskStatus.Created;
}

// -------------------- Mapping Helpers --------------------
function mapInstance(i: WorkflowInstanceDto): WorkflowInstanceDto {
  return { ...i, status: normalizeInstanceStatus(i.status) };
}
function mapInstances(list: WorkflowInstanceDto[]) { return list.map(mapInstance); }
function mapTask(t: WorkflowTaskDto): WorkflowTaskDto { return { ...t, status: normalizeTaskStatus(t.status) }; }
function mapTasks(list: WorkflowTaskDto[]) { return list.map(mapTask); }

// -------------------- Filter Interfaces --------------------
export interface WorkflowDefinitionsFilters { published?: boolean; }
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

// -------------------- Service --------------------
export class WorkflowService {
  // ===== Definitions =====
  async getDefinitions(filters?: WorkflowDefinitionsFilters): Promise<WorkflowDefinitionDto[]> {
    const params = new URLSearchParams();
    if (filters?.published !== undefined) params.append('published', String(filters.published));
    const resp = await apiClient.get(`/api/workflow/definitions${params.size ? `?${params}` : ''}`);
    return unwrap<WorkflowDefinitionDto[]>(resp.data);
  }

  async getDefinition(id: number): Promise<WorkflowDefinitionDto> {
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

  async publishDefinition(id: number, request: PublishDefinitionRequestDto) {
    const resp = await apiClient.post(`/api/workflow/definitions/${id}/publish`, request);
    return unwrap<WorkflowDefinitionDto>(resp.data);
  }

  async deleteDefinition(id: number): Promise<boolean> {
    const resp = await apiClient.delete(`/api/workflow/definitions/${id}`);
    const val = unwrap<boolean>(resp.data);
    return !!val;
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

  // ===== Instances =====
  async getInstances(filters?: WorkflowInstancesFilters): Promise<WorkflowInstanceDto[]> {
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
    return paged.items;
  }

  async getInstancesPaged(filters?: WorkflowInstancesFilters): Promise<PagedResultDto<WorkflowInstanceDto>> {
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
    const dto = unwrap<WorkflowInstanceDto>(resp.data);
    return mapInstance(dto);
  }

  async startInstance(request: StartInstanceRequestDto) {
    const resp = await apiClient.post('/api/workflow/instances', request);
    return mapInstance(unwrap<WorkflowInstanceDto>(resp.data));
  }

  async signalInstance(id: number, request: SignalInstanceRequestDto) {
    const resp = await apiClient.post(`/api/workflow/instances/${id}/signal`, request);
    return mapInstance(unwrap<WorkflowInstanceDto>(resp.data));
  }

  async terminateInstance(id: number): Promise<boolean> {
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

  // ===== Tasks =====
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
    const arr = unwrap<WorkflowTaskDto[]>(resp.data);
    return mapTasks(arr);
  }

  async getMyTasks(status?: TaskStatus): Promise<TaskSummaryDto[]> {
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

  // ===== Events / Admin =====
  async getWorkflowEvents(filters?: WorkflowEventsFilters): Promise<WorkflowEventDto[]> {
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

  // ===== Metrics =====
  async getTaskStatistics() {
    const resp = await apiClient.get('/api/workflow/tasks/statistics');
    return unwrap<TaskStatisticsDto>(resp.data);
  }

  async validateDefinition(jsonDefinition: string): Promise<{ isValid: boolean; errors: string[] }> {
    const resp = await apiClient.post('/api/workflow/definitions/validate', { jsonDefinition });
    return unwrap<{ isValid: boolean; errors: string[] }>(resp.data);
  }

  // ===== Runtime Snapshot =====
  async getRuntimeSnapshot(instanceId: number): Promise<InstanceRuntimeSnapshotDto> {
    const resp = await apiClient.get(`/api/workflow/instances/${instanceId}/runtime-snapshot`);
    const wrapper = unwrap<InstanceRuntimeSnapshotDto>(resp.data);
    return wrapper;
  }
}

export const workflowService = new WorkflowService();
