// Core DSL types for Workflow JSON definitions
export type NodeType = 'start' | 'end' | 'humanTask' | 'automatic' | 'gateway' | 'timer';
export type NodeId = string;
export type EdgeId = string;

// Base node interface
export interface DslNodeBase {
  id: NodeId;
  type: NodeType;
  label?: string;
  x: number;
  y: number;
}

// Specific node types
export interface StartNode extends DslNodeBase {
  type: 'start';
}

export interface EndNode extends DslNodeBase {
  type: 'end';
}

export interface HumanTaskNode extends DslNodeBase {
  type: 'humanTask';
  assigneeRoles?: string[];
  dueInMinutes?: number;
  formSchema?: unknown;
}

export interface AutomaticNode extends DslNodeBase {
  type: 'automatic';
  action?: {
    kind: 'webhook' | 'noop';
    config?: Record<string, unknown>;
  };
}

export interface GatewayNode extends DslNodeBase {
  type: 'gateway';
  condition: string; // JsonLogic expression
}

export interface TimerNode extends DslNodeBase {
  type: 'timer';
  delayMinutes?: number;
  untilIso?: string;
}

// Union type for all nodes
export type DslNode = StartNode | EndNode | HumanTaskNode | AutomaticNode | GatewayNode | TimerNode;

// Edge definition
export interface DslEdge {
  id: EdgeId;
  from: NodeId;
  to: NodeId;
  label?: string;
  fromHandle?: string; // OPTIONAL: branch indicator ('true' | 'false')
}

// Complete workflow definition
export interface DslDefinition {
  key: string;
  version?: number;
  nodes: DslNode[];
  edges: DslEdge[];
}

// Validation result
export interface ValidationResult {
  isValid: boolean;
  errors: string[];
  warnings: string[];
}

// Node palette item for drag-and-drop
export interface NodePaletteItem {
  type: NodeType;
  label: string;
  icon: string;
  description: string;
  defaultData: Partial<DslNode>;
}
