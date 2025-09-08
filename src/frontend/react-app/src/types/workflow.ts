// ================= Core Enums & Constants =================
export enum InstanceStatus {
  Running = 'Running',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
  Failed = 'Failed',
  Suspended = 'Suspended'
}

export type TaskStatus =
  | 'Created'
  | 'Assigned'
  | 'Claimed'
  | 'InProgress'
  | 'Completed'
  | 'Cancelled'
  | 'Failed';

export const TASK_STATUSES: TaskStatus[] = [
  'Created',
  'Assigned',
  'Claimed',
  'InProgress',
  'Completed',
  'Cancelled',
  'Failed'
];

// ================= Definitions =================
export interface WorkflowDefinitionDto {
  id: number;
  name: string;
  version: number;
  jsonDefinition: string;
  description?: string | null;
  isPublished: boolean;
  publishedAt?: string | null;
  publishedByUserId?: number | null;
  createdAt: string;
  updatedAt: string;
  isArchived: boolean;
  archivedAt?: string | null;
  activeInstanceCount: number;
  publishNotes?: string | null;
  versionNotes?: string | null;
  parentDefinitionId?: number | null;
  tags?: string | null;
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

export interface CreateNewVersionRequestDto {
  name: string;
  jsonDefinition: string;
  description?: string;
  versionNotes?: string;
  tags?: string;
}

// ================= Instances =================
export interface WorkflowInstanceDto {
  id: number;
  workflowDefinitionId: number;
  workflowDefinitionName?: string;
  definitionVersion: number;
  status: InstanceStatus | string | number;
  currentNodeIds: string;
  startedAt: string;
  completedAt?: string;
  createdAt?: string;
  updatedAt?: string;
  startedByUserId?: number;
  errorMessage?: string | null;
  context?: string;
}

// ================= Tasks =================
export interface WorkflowTaskDto {
  id: number;
  workflowInstanceId: number;
  workflowDefinitionId: number;
  taskName: string;
  status: TaskStatus | string | number;
  nodeId?: string;
  nodeType?: string;
  dueDate?: string;
  assignedToUserId?: number;
  assignedToRole?: string;
  createdAt: string;
  updatedAt: string;
  completionData?: Record<string, unknown>;
  completionNotes?: string;
  completedAt?: string;
  data?: string;
  errorMessage?: string;
}

export interface TaskSummaryDto {
  id: number;
  taskName: string;
  workflowInstanceId: number;
  workflowDefinitionId: number;
  workflowDefinitionName: string;
  status: TaskStatus;
  dueDate?: string;
  assignedToUserId?: number;
  assignedToRole?: string;
  nodeType?: string;
  createdAt: string;
  nodeId?: string;
}

// ================= Task Actions =================
export interface ClaimTaskRequestDto { claimNotes?: string; }
export interface CompleteTaskRequestDto { completionData?: string; completionNotes?: string; }
export interface AssignTaskRequestDto { userId?: number; role?: string; assignNotes?: string; }
export interface ResetTaskRequestDto { reason?: string; }

// ================= Instance Actions =================
export interface StartInstanceRequestDto {
  workflowDefinitionId: number;
  initialContext?: string;
  startNotes?: string;
}
export interface SignalInstanceRequestDto { signal: string; payload?: unknown; }
export interface RetryInstanceRequestDto { retryReason?: string; }
export interface MoveToNodeRequestDto { targetNodeId: string; reason?: string; }
export interface TerminateInstanceRequestDto { reason?: string; }

// ================= Bulk / Admin =================
export interface BulkCancelInstancesRequestDto {
  workflowDefinitionId?: number;
  olderThanMinutes?: number;
  statusIn?: InstanceStatus[];
  reason?: string;
}
export interface BulkOperationResultDto {
  totalMatched: number;
  totalAffected: number;
  message?: string;
}

// ================= Events & Stats =================
export interface WorkflowEventDto {
  id: number;
  workflowInstanceId: number;
  eventType: string;
  name?: string;
  data?: Record<string, unknown>;
  occurredAt: string;
  userId?: number;
}
export interface WorkflowStatsDto {
  activeInstances: number;
  runningTasks: number;
  completedInstances24h: number;
  failedInstances24h: number;
  averageDurationMinutes?: number;
}
export interface TaskStatisticsDto {
  total: number;
  created: number;
  assigned: number;
  claimed: number;
  inProgress: number;
  completed: number;
  cancelled: number;
  failed: number;
}

// ================= Paging =================
export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

// ================= Validation =================
export interface ValidationResultDto {
  isValid: boolean;
  errors: string[];
  warnings: string[];
  metadata?: Record<string, unknown>;
}

// ================= Runtime Snapshot =================
// All optional so augmentation / legacy fields don't conflict
export interface InstanceRuntimeSnapshotDto {
  instanceId: number;
  definitionVersion: number;
  currentNodeIds: string[];
  visitedNodeIds?: string[];
  progressPercentage?: number;
  tasks?: WorkflowTaskDto[];
  status: InstanceStatus | string;
  lastUpdated: string;
  instance?: WorkflowInstanceDto | null;
  events?: WorkflowEventDto[];
  definitionJson?: string | null;
  traversedEdgeIds?: string[];
}

// ================= Role Usage / Definition Usage =================
export interface WorkflowNodeUsageDto {
  nodeId: string;
  nodeType: string;
  nodeLabel?: string;
  nodeName?: string;
  rolesReferenced?: string[];
}

export interface WorkflowDefinitionUsageDto {
  id: number;
  name: string;
  version: number;
  isPublished: boolean;
  isArchived: boolean;
  usedInNodes: WorkflowNodeUsageDto[];
  definitionId?: number;
  definitionName?: string;
  usageCount?: number;
}

export interface RoleUsageInWorkflowsDto {
  roleName: string;
  definitionCount: number;
  usedInDefinitions: WorkflowDefinitionUsageDto[];
  isUsedInWorkflows?: boolean;
}

export interface CheckRoleUsageRequestDto {
  roleName: string;
}

// ================= Builder / Editor Types =================
export type EditorNodeType =
  | 'start'
  | 'end'
  | 'humanTask'
  | 'automatic'
  | 'gateway'
  | 'timer';

export interface EditorWorkflowNode {
  id: string;
  type: EditorNodeType | string;
  label?: string;
  x: number;
  y: number;
  assigneeRoles?: string[];
  dueInMinutes?: number;
  formSchema?: unknown;
  action?: { kind: 'webhook' | 'noop'; config?: Record<string, unknown> };
  condition?: string;
  delayMinutes?: number;
  untilIso?: string;
  metadata?: Record<string, unknown>;
}

export interface EditorWorkflowEdge {
  id: string;
  from: string;
  to: string;
  label?: string;
  fromHandle?: string;
  enriched?: boolean;
}

export interface EditorWorkflowDefinition {
  id?: string;
  key?: string;
  name?: string;
  version?: number;
  nodes: EditorWorkflowNode[];
  edges: EditorWorkflowEdge[];
  metadata?: Record<string, unknown>;
}

// React Flow node/edge data (all optional to avoid conflicts)
export interface RFNodeData {
  nodeId?: string;
  label?: string;
  type?: EditorNodeType | string;
  roles?: string[];
  status?: string;
  [k: string]: unknown;
}

export interface RFEdgeData {
  edgeId?: string;
  from?: string;
  to?: string;
  branch?: string;
  [k: string]: unknown;
}
