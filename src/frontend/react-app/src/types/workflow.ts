// =====================================
// WORKFLOW TYPES
// =====================================

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
// ENUMS / STRING UNIONS
// =====================================

export enum InstanceStatus {
  Running = 'Running',
  Completed = 'Completed',
  Failed = 'Failed',
  Cancelled = 'Cancelled',
  Suspended = 'Suspended'
}

// Converted from enum → string union for simpler literal usage
export type TaskStatus =
  | 'Created'
  | 'Assigned'
  | 'Claimed'
  | 'InProgress'
  | 'Completed'
  | 'Cancelled'
  | 'Failed';

// Central canonical list (runtime array) for validation / normalization
export const TASK_STATUSES: TaskStatus[] = [
  'Created',
  'Assigned',
  'Claimed',
  'InProgress',
  'Completed',
  'Cancelled',
  'Failed'
];

// =====================================
// DEFINITIONS
// =====================================

export interface WorkflowDefinitionDto {
  id: number;
  name: string;
  version: number;
  jsonDefinition: string;
  isPublished: boolean;
  description?: string;
  publishedAt?: string;
  publishedByUserId?: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateWorkflowDefinitionDto {
  name: string;
  jsonDefinition: string;
  description?: string;
  tags?: string;
}

export interface UpdateWorkflowDefinitionDto {
  name?: string;
  jsonDefinition?: string;
  description?: string;
  tags?: string;
}

export interface PublishDefinitionRequestDto {
  publishNotes?: string;
  forcePublish?: boolean;
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
// INSTANCES
// =====================================

export interface WorkflowInstanceDto {
  id: number;
  workflowDefinitionId: number;
  workflowDefinitionName: string;
  definitionVersion: number;
  status: InstanceStatus;
  currentNodeIds: string; // raw json array or csv
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

export interface TerminateInstanceRequestDto {
  reason: string;
  forceTerminate?: boolean;
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

export interface InstanceStatusDto {
  instanceId: number;
  status: InstanceStatus;
  currentNodeIds: string;
  currentNodeNames: string[];
  progressPercentage: number;
  lastUpdated: string;
  runtime: string;
  activeTasksCount: number;
  errorMessage?: string;
}

// =====================================
// TASKS
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
  status: TaskStatus | string; // backend might send raw string; normalized client-side
  workflowDefinitionName: string;
  workflowInstanceId: number;
  dueDate?: string;
  createdAt: string;
  nodeId?: string;
  nodeType?: string;
}

export interface CompleteTaskRequestDto {
  completionData?: string;
  completionNotes?: string;
}

export interface ClaimTaskRequestDto {
  claimNotes?: string;
}

export interface AssignTaskRequestDto {
  assignedToUserId?: number;
  assignedToRole?: string;
  assignmentNotes?: string;
}

export interface ResetTaskRequestDto {
  newStatus: TaskStatus;
  assignToUserId?: number;
  errorMessage?: string;
  reason?: string;
}

// =====================================
// EVENTS
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
// SNAPSHOT
// =====================================

export interface InstanceRuntimeSnapshotDto {
  instance: WorkflowInstanceDto;
  definitionJson: string;
  tasks: TaskSummaryDto[];
  events: WorkflowEventDto[];
  traversedEdgeIds: string[];
  visitedNodeIds: string[];
  currentNodeIds: string[];
}

// =====================================
// ROLE USAGE
// =====================================

export interface WorkflowNodeUsageDto {
  nodeId: string;
  nodeName: string;
  nodeType: string;
}

export interface WorkflowDefinitionUsageDto {
  definitionId: number;
  definitionName: string;
  version: number;
  isPublished: boolean;
  usageCount: number;
  usedInNodes: WorkflowNodeUsageDto[];
}

export interface RoleUsageInWorkflowsDto {
  isUsedInWorkflows: boolean;
  usedInDefinitions: WorkflowDefinitionUsageDto[];
  totalUsageCount: number;
  roleName: string;
  message: string;
}

export interface CheckRoleUsageRequestDto {
  roleName: string;
}

// =====================================
// ADMIN / ANALYTICS
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

// =====================================
// BUILDER (subset)
// =====================================

export interface EditorWorkflowNode {
  id: string;
  type: string;
  label?: string;
  x: number;
  y: number;
  dueInMinutes?: number;
  assigneeRoles?: string[];
  condition?: string;
  action?: any;
  [key: string]: any;
}

export interface EditorWorkflowEdge {
  id: string;
  from: string;
  to: string;
  fromHandle?: string | null;
  label?: string | null;
}

export interface EditorWorkflowDefinition {
  key?: string;
  nodes: EditorWorkflowNode[];
  edges: EditorWorkflowEdge[];
  [key: string]: any;
}

export interface RFNodeData {
  label?: string;
  dueInMinutes?: number;
  assigneeRoles?: string[];
  condition?: string;
  action?: any;
}

export interface RFEdgeData {
  fromHandle?: string | null;
}
