// Non-destructive legacy compatibility augmentations.
// This file restores optional properties older components/tests still reference.

import type {
  WorkflowDefinitionUsageDto,
  WorkflowNodeUsageDto,
  RoleUsageInWorkflowsDto,
  EditorWorkflowDefinition,
  RFNodeData,
  RFEdgeData,
  InstanceRuntimeSnapshotDto,
  WorkflowTaskDto
} from './workflow';

// Extend interfaces via declaration merging (all optional to avoid conflicts).
declare module './workflow' {
  interface WorkflowNodeUsageDto {
    nodeName?: string;            // legacy display label
  }

  interface WorkflowDefinitionUsageDto {
    definitionId?: number;        // legacy alias for id
    definitionName?: string;      // legacy alias for name
    usageCount?: number;          // length of usedInNodes
  }

  interface RoleUsageInWorkflowsDto {
    isUsedInWorkflows?: boolean;  // legacy flag
  }

  interface EditorWorkflowDefinition {
    key?: string;                 // legacy builder key
  }

  interface RFNodeData {
    nodeId?: string;
    type?: string;                // loosen to generic string
  }

  interface RFEdgeData {
    edgeId?: string;
    from?: string;
    to?: string;
  }

  interface InstanceRuntimeSnapshotDto {
    // Make these optional but known to silence undefined property errors.
    instance?: import('./workflow').WorkflowInstanceDto | null;
    events?: import('./workflow').WorkflowEventDto[];
    definitionJson?: string | null;
    traversedEdgeIds?: string[];
  }

  interface WorkflowTaskDto {
    // Legacy raw field sometimes used directly
    data?: string;
  }
}
