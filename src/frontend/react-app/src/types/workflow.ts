// =====================================
// WORKFLOW TYPES
// =====================================

// Base API Response Types
export interface ApiResponseDto<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
  traceId?: string;
}

export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

// =====================================
// WORKFLOW ENUMS
// =====================================

export enum InstanceStatus {
  Running = 'Running',
  Completed = 'Completed',
  Failed = 'Failed',
  Cancelled = 'Cancelled',
  Suspended = 'Suspended'
}

export enum TaskStatus {
  Created = 'Created',
  Assigned = 'Assigned', 
  Claimed = 'Claimed',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
  Failed = 'Failed'
}

// =====================================
// WORKFLOW DEFINITION TYPES
// =====================================

export interface WorkflowDefinitionDto {
  id: number;
  name: string;
  version: number;
  jsonDefinition: string; // ✅ KEEP camelCase - our apiClient handles conversion
  isPublished: boolean;
  description?: string;
  publishedAt?: string;
  publishedByUserId?: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateWorkflowDefinitionDto {
  name: string;
  jsonDefinition: string; // ✅ KEEP camelCase 
  description?: string;
  tags?: string;
}

export interface UpdateWorkflowDefinitionDto {
  name?: string;
  jsonDefinition?: string; // ✅ KEEP camelCase
  description?: string;
  tags?: string;
}

export interface PublishDefinitionRequestDto {
  publishNotes?: string;
  forcePublish?: boolean;
}

export interface GetWorkflowDefinitionsRequestDto {
  searchTerm?: string;
  isPublished?: boolean;
  tags?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface ValidateDefinitionRequestDto {
  jsonDefinition: string;
}

export interface ValidationResultDto {
  isValid: boolean;
  errors: string[];
  warnings: string[];
  metadata: Record<string, any>;
}

export interface CreateNewVersionRequestDto {
  name: string;
  description?: string;
  jsonDefinition: string;
  versionNotes?: string;
  tags?: string;
}

// =====================================
// WORKFLOW INSTANCE TYPES  
// =====================================

export interface WorkflowInstanceDto {
  id: number;
  workflowDefinitionId: number;
  workflowDefinitionName: string;
  definitionVersion: number;
  status: InstanceStatus;
  currentNodeIds: string;
  context: string;
  startedAt: string;
  completedAt?: string;
  startedByUserId?: number;
  errorMessage?: string;
  createdAt: string;
  updatedAt: string;
}

export interface StartInstanceRequestDto {
  workflowDefinitionId: number;
  initialContext?: string;
  startNotes?: string;
  workflowVersion?: number;
  instanceName?: string;
  variables?: Record<string, any>;
}

export interface SignalInstanceRequestDto {
  signalName: string;
  signalData?: string;
}

export interface GetInstancesRequestDto {
  workflowDefinitionId?: number;
  status?: InstanceStatus;
  startedAfter?: string;
  startedBefore?: string;
  startedByUserId?: number;
  searchTerm?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface TerminateInstanceRequestDto {
  reason: string;
  forceTerminate?: boolean;
}

export interface InstanceStatusDto {
  instanceId: number;
  status: InstanceStatus;
  currentNodeIds: string;
  currentNodeNames: string[];
  progressPercentage: number;
  lastUpdated: string;
  runtime: string; // TimeSpan as string
  activeTasksCount: number;
  errorMessage?: string;
}

// =====================================
// WORKFLOW TASK TYPES
// =====================================

export interface WorkflowTaskDto {
  id: number;
  workflowInstanceId: number;
  nodeId: string;
  taskName: string;
  status: TaskStatus;
  assignedToUserId?: number;
  assignedToRole?: string;
  dueDate?: string;
  data: string;
  claimedAt?: string;
  completedAt?: string;
  completionData?: string;
  errorMessage?: string;
  createdAt: string;
  updatedAt: string;
}

export interface TaskSummaryDto {
  id: number;
  taskName: string;
  status: TaskStatus;
  workflowDefinitionName: string;
  workflowInstanceId: number;
  dueDate?: string;
  createdAt: string;
}

export interface CompleteTaskRequestDto {
  completionData?: string;
  completionNotes?: string;
}

export interface ClaimTaskRequestDto {
  claimNotes?: string;
}

export interface GetTasksRequestDto {
  status?: TaskStatus;
  workflowDefinitionId?: number;
  assignedToUserId?: number;
  assignedToRole?: string;
  dueBefore?: string;
  dueAfter?: string;
  searchTerm?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface ReassignTaskRequestDto {
  assignToUserId?: number;
  assignToRole?: string;
  reason: string;
}

export interface TaskStatisticsDto {
  totalTasks: number;
  pendingTasks: number;
  inProgressTasks: number;
  completedTasks: number;
  overdueTasks: number;
  tasksByType: Record<string, number>;
  tasksByStatus: Record<TaskStatus, number>;
  averageCompletionTime: number;
  lastUpdated: string;
}

export interface AssignTaskRequestDto {
  assignedToUserId?: number;
  assignedToRole?: string;
  assignmentNotes?: string;
}

// =====================================
// WORKFLOW EVENT TYPES
// =====================================

export interface WorkflowEventDto {
  id: number;
  workflowInstanceId: number;
  type: string;
  name: string;
  data: string;
  occurredAt: string;
  userId?: number;
  createdAt: string;
  updatedAt: string;
}

// =====================================
// ADMIN OPERATION TYPES
// =====================================

export interface WorkflowStatsDto {
  totalDefinitions: number;
  publishedDefinitions: number;
  totalInstances: number;
  runningInstances: number;
  completedInstances: number;
  failedInstances: number;
  suspendedInstances: number;
  totalTasks: number;
  pendingTasks: number;
  activeTasks: number;
  completedTasks: number;
  overdueTasks: number;
  totalEvents: number;
  eventsLast24Hours: number;
}

export interface RetryInstanceRequestDto {
  retryReason?: string;
  resetToNodeId?: string;
}

export interface MoveToNodeRequestDto {
  targetNodeId: string;
  contextUpdates?: string;
  reason?: string;
}

export interface ResetTaskRequestDto {
  newStatus: TaskStatus;
  assignToUserId?: number;
  errorMessage?: string;
  reason?: string;
}

export interface BulkCancelInstancesRequestDto {
  workflowDefinitionId?: number;
  status?: InstanceStatus;
  startedBefore?: string;
  reason: string;
}

export interface BulkOperationResultDto {
  successCount: number;
  failureCount: number;
  totalCount: number;
  operationType: string;
}

export interface ForceCompleteRequestDto {
  reason: string;
  finalContext?: string;
  completionNodeId?: string;
}

export interface GetAnalyticsRequestDto {
  startDate?: string;
  endDate?: string;
  workflowDefinitionId?: number;
  groupBy?: string; // day, week, month
}

export interface WorkflowAnalyticsDto {
  startDate: string;
  endDate: string;
  totalInstances: number;
  completedInstances: number;
  failedInstances: number;
  runningInstances: number;
  averageCompletionTime: number;
  successRate: number;
  instancesByStatus: Record<string, number>;
  instancesByDefinition: Record<string, number>;
  instancesByDate: Record<string, number>;
  topBottlenecks: WorkflowPerformanceDto[];
}

export interface WorkflowPerformanceDto {
  nodeId: string;
  nodeName: string;
  nodeType: string;
  averageTime: number;
  instanceCount: number;
}

export interface WorkflowSystemHealthDto {
  status: string; // Healthy, Degraded, Unhealthy
  checkedAt: string;
  activeInstances: number;
  pendingTasks: number;
  backgroundWorkerStatus: number; // 0=Stopped, 1=Running, 2=Error
  systemMetrics: Record<string, any>;
  issues: string[];
}

export interface BulkInstanceOperationRequestDto {
  operation: string; // cancel, retry, terminate
  instanceIds?: number[];
  workflowDefinitionId?: number;
  status?: InstanceStatus;
  startedBefore?: string;
  reason: string;
}

export interface GetAuditTrailRequestDto {
  instanceId?: number;
  userId?: number;
  action?: string;
  startDate?: string;
  endDate?: string;
  page?: number;
  pageSize?: number;
}

export interface WorkflowAuditEntryDto {
  id: number;
  instanceId?: number;
  userId?: number;
  action: string;
  details: string;
  timestamp: string;
  ipAddress?: string;
  userAgent?: string;
}
