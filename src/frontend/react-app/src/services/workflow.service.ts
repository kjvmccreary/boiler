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
  PagedResultDto
} from '@/types/workflow';

// ======================================
// Enum normalization (numeric or string)
// ======================================
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
    return (Object.values(InstanceStatus).find(s => s.toLowerCase() === low)) ?? InstanceStatus.Running;
  }
  return InstanceStatus.Running;
}

function normalizeTaskStatus(v: unknown): TaskStatus {
  if (typeof v === 'string' && Object.values(TaskStatus).includes(v as TaskStatus)) return v as TaskStatus;
  if (typeof v === 'number') return TASK_STATUS_NUM_MAP[v] ?? TaskStatus.Created;
  if (typeof v === 'string') {
    const low = v.toLowerCase();
    return (Object.values(TaskStatus).find(s => s.toLowerCase() === low)) ?? TaskStatus.Created;
  }
  return TaskStatus.Created;
}

// Mappers
function mapInstance(i: WorkflowInstanceDto): WorkflowInstanceDto {
  return { ...i, status: normalizeInstanceStatus(i.status) };
}
function mapInstances(list: WorkflowInstanceDto[]) { return list.map(mapInstance); }
function mapTask(t: WorkflowTaskDto): WorkflowTaskDto { return { ...t, status: normalizeTaskStatus(t.status) }; }
function mapTasks(list: WorkflowTaskDto[]) { return list.map(mapTask); }

// Filters
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

export class WorkflowService {
  // -------- Definitions --------
  async getDefinitions(filters?: WorkflowDefinitionsFilters): Promise<WorkflowDefinitionDto[]> {
    const params = new URLSearchParams();
    if (filters?.published !== undefined) params.append('published', String(filters.published));
    const resp = await apiClient.get<WorkflowDefinitionDto[]>(`/api/workflow/definitions${params.size ? `?${params.toString()}` : ''}`);
    return resp.data;
  }

  async getDefinition(id: number): Promise<WorkflowDefinitionDto> {
    return (await apiClient.get<WorkflowDefinitionDto>(`/api/workflow/definitions/${id}`)).data;
  }

  async createDraft(request: CreateWorkflowDefinitionDto) {
    return (await apiClient.post<WorkflowDefinitionDto>('/api/workflow/definitions/draft', request)).data;
  }

  async updateDefinition(id: number, request: UpdateWorkflowDefinitionDto) {
    return (await apiClient.put<WorkflowDefinitionDto>(`/api/workflow/definitions/${id}`, request)).data;
  }

  async publishDefinition(id: number, request: PublishDefinitionRequestDto) {
    return (await apiClient.post<WorkflowDefinitionDto>(`/api/workflow/definitions/${id}/publish`, request)).data;
  }

  async deleteDefinition(id: number): Promise<boolean> {
    // Endpoint likely returns ApiResponseDto<bool> → unwrapped to bool already OR plain bool; treat truthy as success.
    return (await apiClient.delete<any>(`/api/workflow/definitions/${id}`)).data === true;
  }

  async unpublishDefinition(id: number): Promise<WorkflowDefinitionDto> {
    return (await apiClient.post<WorkflowDefinitionDto>(`/api/workflow/definitions/${id}/unpublish`, {})).data;
  }
  async archiveDefinition(id: number): Promise<WorkflowDefinitionDto> {
    return (await apiClient.post<WorkflowDefinitionDto>(`/api/workflow/definitions/${id}/archive`, {})).data;
  }
  async terminateDefinitionInstances(id: number): Promise<{ terminated: number }> {
    // backend returns { terminated: number }
    return (await apiClient.post<{ terminated: number }>(`/api/workflow/definitions/${id}/terminate-running`, {})).data;
  }

  // -------- Instances --------
  async getInstances(filters?: WorkflowInstancesFilters): Promise<WorkflowInstanceDto[]> {
    const params = new URLSearchParams();
    if (filters?.status) params.append('status', filters.status);
    if (filters?.workflowDefinitionId) params.append('workflowDefinitionId', String(filters.workflowDefinitionId));
    if (filters?.startedByUserId) params.append('startedByUserId', String(filters.startedByUserId));
    if (filters?.page) params.append('page', String(filters.page));
    if (filters?.pageSize) params.append('pageSize', String(filters.pageSize));
    if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);
    const resp = await apiClient.get<PagedResultDto<WorkflowInstanceDto>>(`/api/workflow/instances${params.size ? `?${params.toString()}` : ''}`);
    resp.data.items = mapInstances(resp.data.items);
    return resp.data.items;
  }

  async getInstancesPaged(filters?: WorkflowInstancesFilters): Promise<PagedResultDto<WorkflowInstanceDto>> {
    const params = new URLSearchParams();
    if (filters?.status) params.append('status', filters.status);
    if (filters?.workflowDefinitionId) params.append('workflowDefinitionId', String(filters.workflowDefinitionId));
    if (filters?.startedByUserId) params.append('startedByUserId', String(filters.startedByUserId));
    if (filters?.page) params.append('page', String(filters.page));
    if (filters?.pageSize) params.append('pageSize', String(filters.pageSize));
    if (filters?.searchTerm) params.append('searchTerm', filters.searchTerm);
    const resp = await apiClient.get<PagedResultDto<WorkflowInstanceDto>>(`/api/workflow/instances${params.size ? `?${params.toString()}` : ''}`);
    resp.data.items = mapInstances(resp.data.items);
    return resp.data;
  }

  async getInstance(id: number) {
    return mapInstance((await apiClient.get<WorkflowInstanceDto>(`/api/workflow/instances/${id}`)).data);
  }

  async startInstance(request: StartInstanceRequestDto) {
    return mapInstance((await apiClient.post<WorkflowInstanceDto>('/api/workflow/instances', request)).data);
  }

  async signalInstance(id: number, request: SignalInstanceRequestDto) {
    return mapInstance((await apiClient.post<WorkflowInstanceDto>(`/api/workflow/instances/${id}/signal`, request)).data);
  }

  async terminateInstance(id: number): Promise<boolean> {
    return (await apiClient.delete<any>(`/api/workflow/instances/${id}`)).data === true;
  }

  // Placeholders (will 404 until implemented server-side)
  async suspendInstance(id: number) {
    return mapInstance((await apiClient.post<WorkflowInstanceDto>(`/api/workflow/instances/${id}/suspend`, {})).data);
  }
  async resumeInstance(id: number) {
    return mapInstance((await apiClient.post<WorkflowInstanceDto>(`/api/workflow/instances/${id}/resume`, {})).data);
  }

  async retryInstance(id: number, request: RetryInstanceRequestDto) {
    return mapInstance((await apiClient.post<WorkflowInstanceDto>(`/api/workflow/admin/instances/${id}/retry`, request)).data);
  }

  async moveInstanceToNode(id: number, request: MoveToNodeRequestDto) {
    return mapInstance((await apiClient.post<WorkflowInstanceDto>(`/api/workflow/admin/instances/${id}/move-to-node`, request)).data);
  }

  // -------- Tasks --------
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
    const resp = await apiClient.get<WorkflowTaskDto[]>(`/api/workflow/tasks${params.size ? `?${params.toString()}` : ''}`);
    return mapTasks(resp.data);
  }

  async getMyTasks(status?: TaskStatus): Promise<TaskSummaryDto[]> {
    const resp = await apiClient.get<TaskSummaryDto[]>(`/api/workflow/tasks/mine${status ? `?status=${status}` : ''}`);
    return resp.data.map(t => ({ ...t, status: normalizeTaskStatus(t.status) }));
  }

  async getTask(id: number) {
    return mapTask((await apiClient.get<WorkflowTaskDto>(`/api/workflow/tasks/${id}`)).data);
  }

  async claimTask(id: number, request: ClaimTaskRequestDto) {
    return mapTask((await apiClient.post<WorkflowTaskDto>(`/api/workflow/tasks/${id}/claim`, request)).data);
  }

  async completeTask(id: number, request: CompleteTaskRequestDto) {
    return mapTask((await apiClient.post<WorkflowTaskDto>(`/api/workflow/tasks/${id}/complete`, request)).data);
  }

  async assignTask(id: number, request: AssignTaskRequestDto) {
    return mapTask((await apiClient.post<WorkflowTaskDto>(`/api/workflow/tasks/${id}/assign`, request)).data);
  }

  async cancelTask(id: number) {
    return mapTask((await apiClient.post<WorkflowTaskDto>(`/api/workflow/tasks/${id}/cancel`, {})).data);
  }

  async resetTask(id: number, request: ResetTaskRequestDto) {
    return mapTask((await apiClient.post<WorkflowTaskDto>(`/api/workflow/admin/tasks/${id}/reset`, request)).data);
  }

  // -------- Events / Admin --------
  async getWorkflowEvents(filters?: WorkflowEventsFilters): Promise<WorkflowEventDto[]> {
    const params = new URLSearchParams();
    if (filters?.instanceId) params.append('instanceId', String(filters.instanceId));
    if (filters?.eventType) params.append('eventType', filters.eventType);
    if (filters?.userId) params.append('userId', String(filters.userId));
    if (filters?.from) params.append('from', filters.from.toISOString());
    if (filters?.to) params.append('to', filters.to.toISOString());
    if (filters?.page) params.append('page', String(filters.page));
    if (filters?.pageSize) params.append('pageSize', String(filters.pageSize));
    const resp = await apiClient.get<WorkflowEventDto[]>(`/api/workflow/admin/events${params.size ? `?${params.toString()}` : ''}`);
    return resp.data;
  }

  async getWorkflowStats() {
    return (await apiClient.get<WorkflowStatsDto>('/api/workflow/admin/stats')).data;
  }

  async bulkCancelInstances(request: BulkCancelInstancesRequestDto) {
    return (await apiClient.post<BulkOperationResultDto>('/api/workflow/admin/instances/bulk-cancel', request)).data;
  }

  // -------- Metrics / Utilities --------
  async getTaskStatistics() {
    return (await apiClient.get<TaskStatisticsDto>('/api/workflow/tasks/statistics')).data;
  }

  async validateDefinition(jsonDefinition: string): Promise<{ isValid: boolean; errors: string[] }> {
    return (await apiClient.post<{ isValid: boolean; errors: string[] }>('/api/workflow/definitions/validate', { jsonDefinition })).data;
  }
}

export const workflowService = new WorkflowService();
