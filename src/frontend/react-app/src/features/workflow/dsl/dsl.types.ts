// Core DSL types for Workflow JSON definitions
export type NodeType = 'start' | 'end' | 'humanTask' | 'automatic' | 'gateway' | 'timer';
export type NodeId = string;
export type EdgeId = string;

// Gateway strategy union (initial set surfaced in builder)
export type GatewayStrategy = 'exclusive' | 'conditional' | 'parallel';

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

/**
 * Gateway node.
 * Backward compatibility:
 *  - Older drafts have no 'strategy' and always a 'condition' string (required).
 *  - New model: strategy determines whether condition is required.
 *    * conditional => condition required (JsonLogic serialized as string for now)
 *    * exclusive | parallel => condition optional/ignored
 * Migration heuristic (validation): if strategy absent and a condition field exists, treat as 'conditional'; else default 'exclusive'.
 */
export interface GatewayNode extends DslNodeBase {
  type: 'gateway';
  strategy?: GatewayStrategy;      // Newly introduced (optional for legacy)
  condition?: string;              // Required only if strategy=conditional (or legacy with condition field)
}

export interface TimerNode extends DslNodeBase {
  type: 'timer';
  // Legacy relative delay in minutes (can now be fractional)
  delayMinutes?: number;
  // Precise relative delay in seconds (takes precedence over delayMinutes if provided)
  delaySeconds?: number;
  // Absolute override (UTC ISO)
  untilIso?: string;
  // Optional historical alias
  dueDate?: string;
}

// Union type for all nodes
export type DslNode =
  | StartNode
  | EndNode
  | HumanTaskNode
  | AutomaticNode
  | GatewayNode
  | TimerNode;

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
