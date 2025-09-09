// Core DSL types for Workflow JSON definitions
export type NodeType = 'start' | 'end' | 'humanTask' | 'automatic' | 'gateway' | 'timer' | 'join';
export type NodeId = string;
export type EdgeId = string;

// Gateway strategy union
export type GatewayStrategy = 'exclusive' | 'conditional' | 'parallel';

// Join mode union
export type JoinMode = 'all' | 'any' | 'count' | 'quorum' | 'expression';

/* ================= HumanTask Assignment (H1) =================
   New richer assignment model (additive – legacy fields retained):
   - assignment.mode: users | roles | expression | hybrid
   - users / roles: explicit lists
   - expression: JsonLogic string returning { users?:[], roles?:[] }
   - sla: optional SLA timings
   - escalation: optional escalation target afterMinutes (stub for future)
   Backward compatibility:
   - Legacy assigneeRoles & dueInMinutes remain (deprecated).
   - Migration layer (future) may map legacy roles -> assignment { mode:'roles', roles:[...] }.
*/
export type HumanTaskAssignmentMode = 'users' | 'roles' | 'expression' | 'hybrid';

export interface HumanTaskAssignmentSla {
  targetMinutes: number;
  softWarningMinutes?: number;
}

export interface HumanTaskAssignmentEscalation {
  escalateToRole: string;
  afterMinutes: number;
}

export interface HumanTaskAssignment {
  mode: HumanTaskAssignmentMode;
  users?: string[];
  roles?: string[];
  expression?: string; // JsonLogic source
  sla?: HumanTaskAssignmentSla;
  escalation?: HumanTaskAssignmentEscalation;
}

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
  /* Legacy (pre-H1) */
  assigneeRoles?: string[];
  dueInMinutes?: number;
  formSchema?: unknown;
  /* New (H1) */
  assignment?: HumanTaskAssignment;
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
 * Join node
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
