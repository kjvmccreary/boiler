import { apiClient } from './api.client';
import type { 
  WorkflowDefinitionDto,
  CreateWorkflowDefinitionDto,
  UpdateWorkflowDefinitionDto,
  PublishDefinitionRequestDto,
  WorkflowInstanceDto,
  StartInstanceRequestDto,
  SignalInstanceRequestDto,
  TerminateInstanceRequestDto,
  WorkflowTaskDto,
  TaskSummaryDto,
  CompleteTaskRequestDto,
  ClaimTaskRequestDto,
  AssignTaskRequestDto,
  WorkflowStatsDto,
  RetryInstanceRequestDto,
  MoveToNodeRequestDto,
  ResetTaskRequestDto,
  BulkCancelInstancesRequestDto,
  BulkOperationResultDto,
  WorkflowEventDto,
  TaskStatisticsDto,
  ApiResponseDto,
  PagedResultDto,
  InstanceStatus,
  TaskStatus
} from '@/types/workflow';

export interface WorkflowDefinitionsFilters {
  published?: boolean;
}

export interface WorkflowInstancesFilters {
  status?: InstanceStatus;
  workflowDefinitionId?: number;
  startedByUserId?: number;
  page?: number;
  pageSize?: number;
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
  // =====================================
  // WORKFLOW DEFINITIONS
  // =====================================

  /**
   * Get all workflow definitions
   */
  async getDefinitions(filters?: WorkflowDefinitionsFilters): Promise<WorkflowDefinitionDto[]> {
    console.log('🔍 WorkflowService: getDefinitions called with filters:', filters);
    
    try {
      const params = new URLSearchParams();
      if (filters?.published !== undefined) {
        params.append('published', filters.published.toString());
      }
      
      const url = `/api/workflow/definitions${params.toString() ? `?${params.toString()}` : ''}`;
      const response = await apiClient.get<ApiResponseDto<WorkflowDefinitionDto[]>>(url);
      
      console.log('✅ WorkflowService: getDefinitions successful', response.data);
      return response.data.data; // ✅ Extract data from wrapper
    } catch (error) {
      console.error('❌ WorkflowService: getDefinitions failed:', error);
      throw error;
    }
  }

  /**
   * Get workflow definition by ID
   */
  async getDefinition(id: number): Promise<WorkflowDefinitionDto> {
    console.log('🔍 WorkflowService: getDefinition called with id:', id);
    
    try {
      // ✅ FIX: Handle ApiResponseDto wrapper
      const response = await apiClient.get<ApiResponseDto<WorkflowDefinitionDto>>(`/api/workflow/definitions/${id}`);
      console.log('✅ WorkflowService: getDefinition successful');
      return response.data.data; // ✅ Extract data from wrapper
    } catch (error) {
      console.error('❌ WorkflowService: getDefinition failed:', error);
      throw error;
    }
  }

  /**
   * Create a new workflow definition draft
   */
  async createDraft(request: CreateWorkflowDefinitionDto): Promise<WorkflowDefinitionDto> {
    console.log('🔍 WorkflowService: createDraft called with request:', request);
    
    try {
      const response = await apiClient.post<ApiResponseDto<WorkflowDefinitionDto>>('/api/workflow/definitions/draft', request);
      console.log('✅ WorkflowService: createDraft successful', response.data);
      return response.data.data; // ✅ Extract data from wrapper
    } catch (error) {
      console.error('❌ WorkflowService: createDraft failed:', error);
      throw error;
    }
  }

  /**
   * Update a workflow definition
   */
  async updateDefinition(id: number, request: UpdateWorkflowDefinitionDto): Promise<WorkflowDefinitionDto> {
    console.log('🔍 WorkflowService: updateDefinition called with id:', id, 'request:', request);
    
    try {
      const response = await apiClient.put<ApiResponseDto<WorkflowDefinitionDto>>(`/api/workflow/definitions/${id}`, request);
      console.log('✅ WorkflowService: updateDefinition successful', response.data);
      return response.data.data; // ✅ Extract data from wrapper
    } catch (error) {
      console.error('❌ WorkflowService: updateDefinition failed:', error);
      throw error;
    }
  }

  /**
   * Publish a workflow definition (makes it immutable)
   */
  async publishDefinition(id: number, request: PublishDefinitionRequestDto): Promise<WorkflowDefinitionDto> {
    console.log('🔍 WorkflowService: publishDefinition called with id:', id);
    
    try {
      const response = await apiClient.post<ApiResponseDto<WorkflowDefinitionDto>>(`/api/workflow/definitions/${id}/publish`, request);
      console.log('✅ WorkflowService: publishDefinition successful');
      return response.data.data; // ✅ Extract data from wrapper
    } catch (error) {
      console.error('❌ WorkflowService: publishDefinition failed:', error);
      throw error;
    }
  }

  /**
   * Delete a workflow definition (draft only)
   */
  async deleteDefinition(id: number): Promise<boolean> {
    console.log('🔍 WorkflowService: deleteDefinition called with id:', id);
    
    try {
      const response = await apiClient.delete<ApiResponseDto<boolean>>(`/api/workflow/definitions/${id}`);
      console.log('✅ WorkflowService: deleteDefinition successful');
      return response.data.data; // ✅ Extract data from wrapper
    } catch (error) {
      console.error('❌ WorkflowService: deleteDefinition failed:', error);
      throw error;
    }
  }

  // =====================================
  // WORKFLOW INSTANCES
  // =====================================

  /**
   * Get workflow instances with filters
   */
  async getInstances(filters?: WorkflowInstancesFilters): Promise<WorkflowInstanceDto[]> {
    console.log('🔍 WorkflowService: getInstances called with filters:', filters);
    
    try {
      const params = new URLSearchParams();
      if (filters?.status) params.append('status', filters.status);
      if (filters?.workflowDefinitionId) params.append('workflowDefinitionId', filters.workflowDefinitionId.toString());
      if (filters?.startedByUserId) params.append('startedByUserId', filters.startedByUserId.toString());
      if (filters?.page) params.append('page', filters.page.toString());
      if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
      
      const url = `/api/workflow/instances${params.toString() ? `?${params.toString()}` : ''}`;
      const response = await apiClient.get<WorkflowInstanceDto[]>(url);
      
      console.log('✅ WorkflowService: getInstances successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: getInstances failed:', error);
      throw error;
    }
  }

  /**
   * Get workflow instance by ID
   */
  async getInstance(id: number): Promise<WorkflowInstanceDto> {
    console.log('🔍 WorkflowService: getInstance called with id:', id);
    
    try {
      const response = await apiClient.get<WorkflowInstanceDto>(`/api/workflow/instances/${id}`);
      console.log('✅ WorkflowService: getInstance successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: getInstance failed:', error);
      throw error;
    }
  }

  /**
   * Start a new workflow instance
   */
  async startInstance(request: StartInstanceRequestDto): Promise<WorkflowInstanceDto> {
    console.log('🔍 WorkflowService: startInstance called');
    
    try {
      const response = await apiClient.post<WorkflowInstanceDto>('/api/workflow/instances', request);
      console.log('✅ WorkflowService: startInstance successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: startInstance failed:', error);
      throw error;
    }
  }

  /**
   * Send a signal to a workflow instance
   */
  async signalInstance(id: number, request: SignalInstanceRequestDto): Promise<WorkflowInstanceDto> {
    console.log('🔍 WorkflowService: signalInstance called with id:', id);
    
    try {
      const response = await apiClient.post<WorkflowInstanceDto>(`/api/workflow/instances/${id}/signal`, request);
      console.log('✅ WorkflowService: signalInstance successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: signalInstance failed:', error);
      throw error;
    }
  }

  /**
   * Terminate a workflow instance
   */
  async terminateInstance(id: number): Promise<boolean> {
    console.log('🔍 WorkflowService: terminateInstance called with id:', id);
    
    try {
      const response = await apiClient.delete<boolean>(`/api/workflow/instances/${id}`);
      console.log('✅ WorkflowService: terminateInstance successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: terminateInstance failed:', error);
      throw error;
    }
  }

  /**
   * Suspend a workflow instance
   */
  async suspendInstance(id: number): Promise<WorkflowInstanceDto> {
    console.log('🔍 WorkflowService: suspendInstance called with id:', id);
    
    try {
      const response = await apiClient.post<WorkflowInstanceDto>(`/api/workflow/instances/${id}/suspend`, {});
      console.log('✅ WorkflowService: suspendInstance successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: suspendInstance failed:', error);
      throw error;
    }
  }

  /**
   * Resume a suspended workflow instance
   */
  async resumeInstance(id: number): Promise<WorkflowInstanceDto> {
    console.log('🔍 WorkflowService: resumeInstance called with id:', id);
    
    try {
      const response = await apiClient.post<WorkflowInstanceDto>(`/api/workflow/instances/${id}/resume`, {});
      console.log('✅ WorkflowService: resumeInstance successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: resumeInstance failed:', error);
      throw error;
    }
  }

  // =====================================
  // WORKFLOW TASKS
  // =====================================

  /**
   * Get workflow tasks with filters
   */
  async getTasks(filters?: WorkflowTasksFilters): Promise<WorkflowTaskDto[]> {
    console.log('🔍 WorkflowService: getTasks called with filters:', filters);
    
    try {
      const params = new URLSearchParams();
      if (filters?.mine !== undefined) params.append('mine', filters.mine.toString());
      if (filters?.status) params.append('status', filters.status);
      if (filters?.workflowInstanceId) params.append('workflowInstanceId', filters.workflowInstanceId.toString());
      if (filters?.assignedToUserId) params.append('assignedToUserId', filters.assignedToUserId.toString());
      if (filters?.assignedToRole) params.append('assignedToRole', filters.assignedToRole);
      if (filters?.overdue !== undefined) params.append('overdue', filters.overdue.toString());
      if (filters?.page) params.append('page', filters.page.toString());
      if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
      
      const url = `/api/workflow/tasks${params.toString() ? `?${params.toString()}` : ''}`;
      const response = await apiClient.get<WorkflowTaskDto[]>(url);
      
      console.log('✅ WorkflowService: getTasks successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: getTasks failed:', error);
      throw error;
    }
  }

  /**
   * Get my tasks (simplified view)
   */
  async getMyTasks(status?: TaskStatus): Promise<TaskSummaryDto[]> {
    console.log('🔍 WorkflowService: getMyTasks called with status:', status);
    
    try {
      const params = status ? `?status=${status}` : '';
      const response = await apiClient.get<TaskSummaryDto[]>(`/api/workflow/tasks/mine${params}`);
      
      console.log('✅ WorkflowService: getMyTasks successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: getMyTasks failed:', error);
      throw error;
    }
  }

  /**
   * Get workflow task by ID
   */
  async getTask(id: number): Promise<WorkflowTaskDto> {
    console.log('🔍 WorkflowService: getTask called with id:', id);
    
    try {
      const response = await apiClient.get<WorkflowTaskDto>(`/api/workflow/tasks/${id}`);
      console.log('✅ WorkflowService: getTask successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: getTask failed:', error);
      throw error;
    }
  }

  /**
   * Claim a workflow task
   */
  async claimTask(id: number, request: ClaimTaskRequestDto): Promise<WorkflowTaskDto> {
    console.log('🔍 WorkflowService: claimTask called with id:', id);
    
    try {
      const response = await apiClient.post<WorkflowTaskDto>(`/api/workflow/tasks/${id}/claim`, request);
      console.log('✅ WorkflowService: claimTask successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: claimTask failed:', error);
      throw error;
    }
  }

  /**
   * Complete a workflow task
   */
  async completeTask(id: number, request: CompleteTaskRequestDto): Promise<WorkflowTaskDto> {
    console.log('🔍 WorkflowService: completeTask called with id:', id);
    
    try {
      const response = await apiClient.post<WorkflowTaskDto>(`/api/workflow/tasks/${id}/complete`, request);
      console.log('✅ WorkflowService: completeTask successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: completeTask failed:', error);
      throw error;
    }
  }

  /**
   * Assign a task to a specific user (admin operation)
   */
  async assignTask(id: number, request: AssignTaskRequestDto): Promise<WorkflowTaskDto> {
    console.log('🔍 WorkflowService: assignTask called with id:', id);
    
    try {
      const response = await apiClient.post<WorkflowTaskDto>(`/api/workflow/tasks/${id}/assign`, request);
      console.log('✅ WorkflowService: assignTask successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: assignTask failed:', error);
      throw error;
    }
  }

  /**
   * Cancel a workflow task (admin operation)
   */
  async cancelTask(id: number): Promise<WorkflowTaskDto> {
    console.log('🔍 WorkflowService: cancelTask called with id:', id);
    
    try {
      const response = await apiClient.post<WorkflowTaskDto>(`/api/workflow/tasks/${id}/cancel`, {});
      console.log('✅ WorkflowService: cancelTask successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: cancelTask failed:', error);
      throw error;
    }
  }

  // =====================================
  // ADMIN OPERATIONS
  // =====================================

  /**
   * Get workflow system statistics
   */
  async getWorkflowStats(): Promise<WorkflowStatsDto> {
    console.log('🔍 WorkflowService: getWorkflowStats called');
    
    try {
      const response = await apiClient.get<WorkflowStatsDto>('/api/workflow/admin/stats');
      console.log('✅ WorkflowService: getWorkflowStats successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: getWorkflowStats failed:', error);
      throw error;
    }
  }

  /**
   * Retry a failed workflow instance
   */
  async retryInstance(id: number, request: RetryInstanceRequestDto): Promise<WorkflowInstanceDto> {
    console.log('🔍 WorkflowService: retryInstance called with id:', id);
    
    try {
      const response = await apiClient.post<WorkflowInstanceDto>(`/api/workflow/admin/instances/${id}/retry`, request);
      console.log('✅ WorkflowService: retryInstance successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: retryInstance failed:', error);
      throw error;
    }
  }

  /**
   * Move an instance to a specific node (advanced admin operation)
   */
  async moveInstanceToNode(id: number, request: MoveToNodeRequestDto): Promise<WorkflowInstanceDto> {
    console.log('🔍 WorkflowService: moveInstanceToNode called with id:', id);
    
    try {
      const response = await apiClient.post<WorkflowInstanceDto>(`/api/workflow/admin/instances/${id}/move-to-node`, request);
      console.log('✅ WorkflowService: moveInstanceToNode successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: moveInstanceToNode failed:', error);
      throw error;
    }
  }

  /**
   * Reset a task to a different status (admin operation)
   */
  async resetTask(id: number, request: ResetTaskRequestDto): Promise<WorkflowTaskDto> {
    console.log('🔍 WorkflowService: resetTask called with id:', id);
    
    try {
      const response = await apiClient.post<WorkflowTaskDto>(`/api/workflow/admin/tasks/${id}/reset`, request);
      console.log('✅ WorkflowService: resetTask successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: resetTask failed:', error);
      throw error;
    }
  }

  /**
   * Get workflow events for debugging and audit
   */
  async getWorkflowEvents(filters?: WorkflowEventsFilters): Promise<WorkflowEventDto[]> {
    console.log('🔍 WorkflowService: getWorkflowEvents called with filters:', filters);
    
    try {
      const params = new URLSearchParams();
      if (filters?.instanceId) params.append('instanceId', filters.instanceId.toString());
      if (filters?.eventType) params.append('eventType', filters.eventType);
      if (filters?.userId) params.append('userId', filters.userId.toString());
      if (filters?.from) params.append('from', filters.from.toISOString());
      if (filters?.to) params.append('to', filters.to.toISOString());
      if (filters?.page) params.append('page', filters.page.toString());
      if (filters?.pageSize) params.append('pageSize', filters.pageSize.toString());
      
      const url = `/api/workflow/admin/events${params.toString() ? `?${params.toString()}` : ''}`;
      const response = await apiClient.get<WorkflowEventDto[]>(url);
      
      console.log('✅ WorkflowService: getWorkflowEvents successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: getWorkflowEvents failed:', error);
      throw error;
    }
  }

  /**
   * Bulk cancel instances by criteria (emergency operation)
   */
  async bulkCancelInstances(request: BulkCancelInstancesRequestDto): Promise<BulkOperationResultDto> {
    console.log('🔍 WorkflowService: bulkCancelInstances called');
    
    try {
      const response = await apiClient.post<BulkOperationResultDto>('/api/workflow/admin/instances/bulk-cancel', request);
      console.log('✅ WorkflowService: bulkCancelInstances successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: bulkCancelInstances failed:', error);
      throw error;
    }
  }

  // =====================================
  // UTILITY METHODS
  // =====================================

  /**
   * Get task statistics for dashboard
   */
  async getTaskStatistics(): Promise<TaskStatisticsDto> {
    console.log('🔍 WorkflowService: getTaskStatistics called');
    
    try {
      // Note: This endpoint would need to be implemented in TaskService/Controller
      const response = await apiClient.get<TaskStatisticsDto>('/api/workflow/tasks/statistics');
      console.log('✅ WorkflowService: getTaskStatistics successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: getTaskStatistics failed:', error);
      throw error;
    }
  }

  /**
   * Validate workflow definition JSON
   */
  async validateDefinition(jsonDefinition: string): Promise<{isValid: boolean; errors: string[]}> {
    console.log('🔍 WorkflowService: validateDefinition called');
    
    try {
      // Note: This endpoint would need to be implemented in DefinitionsController
      const response = await apiClient.post<{isValid: boolean; errors: string[]}>('/api/workflow/definitions/validate', { jsonDefinition });
      console.log('✅ WorkflowService: validateDefinition successful');
      return response.data;
    } catch (error) {
      console.error('❌ WorkflowService: validateDefinition failed:', error);
      throw error;
    }
  }
}

// Export singleton instance - following your existing pattern
export const workflowService = new WorkflowService();
