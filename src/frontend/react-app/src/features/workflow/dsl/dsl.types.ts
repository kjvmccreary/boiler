// Core DSL types for Workflow JSON definitions
export type NodeType = 'start' | 'end' | 'humanTask' | 'automatic' | 'gateway' | 'timer' | 'join';
export type NodeId = string;
export type EdgeId = string;

// Gateway strategy union
export type GatewayStrategy = 'exclusive' | 'conditional' | 'parallel';

// Join mode union (will be expanded in C4 configuration story)
export type JoinMode = 'all' | 'any' | 'count' | 'quorum' | 'expression';

// Base node interface
export interface DslNodeBase {
  id: NodeId;
  type: NodeType;
  label?: string;
  x: number;
  y: number;
}

// Specific node types
export interface StartNode extends DslNodeBase { type: 'start'; }
export interface EndNode extends DslNodeBase { type: 'end'; }

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
  strategy?: GatewayStrategy;
  condition?: string;
}

export interface TimerNode extends DslNodeBase {
  type: 'timer';
  delayMinutes?: number;
  delaySeconds?: number;
  untilIso?: string;
  dueDate?: string;
}

/**
 * Join node (base form – detailed configuration arrives in C4)
 * Defaults:
 *  - mode 'all' (wait for all incoming branches)
 *  - thresholdCount / thresholdPercent used for count/quorum modes (C4)
 *  - cancelRemaining indicates pruning of unfinished branches once satisfied
 */
export interface JoinNode extends DslNodeBase {
  type: 'join';
  mode?: JoinMode;
  thresholdCount?: number;
  thresholdPercent?: number;
  expression?: string;
  cancelRemaining?: boolean;
}

// Union type for all nodes
export type DslNode =
  | StartNode
  | EndNode
  | HumanTaskNode
  | AutomaticNode
  | GatewayNode
  | TimerNode
  | JoinNode;

// Edge definition
export interface DslEdge {
  id: EdgeId;
  from: NodeId;
  to: NodeId;
  label?: string;
  fromHandle?: string;
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

export interface NodePaletteItem {
  type: NodeType;
  label: string;
  icon: string;
  description: string;
  defaultData: Partial<DslNode>;
}
