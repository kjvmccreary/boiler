// Core DSL types for Workflow JSON definitions (restored / consolidated)

export type NodeType =
  | 'start'
  | 'end'
  | 'humanTask'
  | 'automatic'
  | 'gateway'
  | 'timer'
  | 'join';

export type NodeId = string;
export type EdgeId = string;

export interface ValidationResult {
  isValid: boolean;
  errors: string[];
  warnings: string[];
}

/* ---------- Gateway & Join strategy enums ---------- */
export type GatewayStrategy = 'exclusive' | 'conditional' | 'parallel';
export type JoinMode = 'all' | 'any' | 'count' | 'quorum' | 'expression';

/* ---------- HumanTask Assignment (H1) ---------- */
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
  expression?: string; // JsonLogic
  sla?: HumanTaskAssignmentSla;
  escalation?: HumanTaskAssignmentEscalation;
}

/* ---------- Base Node ---------- */
export interface DslNodeBase {
  id: NodeId;
  type: NodeType;
  label?: string;
  x: number;
  y: number;
}

/* ---------- Specific Nodes ---------- */
export interface StartNode extends DslNodeBase { type: 'start'; }
export interface EndNode extends DslNodeBase { type: 'end'; }

export interface HumanTaskNode extends DslNodeBase {
  type: 'humanTask';
  assigneeRoles?: string[];     // legacy
  dueInMinutes?: number;        // legacy
  formSchema?: unknown;
  formSchemaSource?: string;
  assignmentHistory?: {
    ts: string;
    mode: string;
    roles?: string[];
    users?: string[];
    slaTarget?: number;
  }[];
  assignment?: HumanTaskAssignment;
}

export interface AutomaticNode extends DslNodeBase {
  type: 'automatic';
  action?: AutomaticAction;
}

export interface GatewayNode extends DslNodeBase {
  type: 'gateway';
  strategy?: GatewayStrategy;       // explicit strategy
  condition?: string;               // JsonLogic (when conditional)
}

export interface TimerNode extends DslNodeBase {
  type: 'timer';
  delaySeconds?: number;
  delayMinutes?: number;
  untilIso?: string;
}

export interface JoinNode extends DslNodeBase {
  type: 'join';
  mode?: JoinMode;
  thresholdCount?: number;
  thresholdPercent?: number;
  expression?: string;   // JsonLogic for expression mode
  cancelRemaining?: boolean;
}

/* ---------- Automatic Action (A1) ---------- */
export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

export interface AutomaticActionNoop {
  kind: 'noop';
}

export interface AutomaticActionWebhook {
  kind: 'webhook';
  url: string;
  method: HttpMethod;
  headers?: Record<string, string>;
  bodyTemplate?: string;
  retryPolicy?: {
    maxAttempts: number;
    backoffSeconds: number;
  };
}

export type AutomaticAction = AutomaticActionNoop | AutomaticActionWebhook;

/* ---------- Unions ---------- */
export type DslNode =
  | StartNode
  | EndNode
  | HumanTaskNode
  | AutomaticNode
  | GatewayNode
  | TimerNode
  | JoinNode;

/* ---------- Edge & Definition ---------- */
export interface DslEdge {
  id: EdgeId;
  from: NodeId;
  to: NodeId;
  label?: string;
  /**
   * Optional originating handle identifier.
   * Used in builder to preserve which boolean / branch handle an edge came from
   * (e.g., conditional gateway true/false branches) so that re‑serialization
   * can keep implicit labels stable.
   */
  fromHandle?: string;
}

export interface DslDefinition {
  key: string;
  name?: string;
  description?: string;
  version?: number;
  nodes: DslNode[];
  edges: DslEdge[];
  // Additional metadata fields can be added here (e.g., tags)
}

/* ---------- Palette (builder UI helper) ---------- */
export interface NodePaletteItem {
  type: NodeType;
  label: string;
  icon?: string;
  description?: string;
  /**
   * Optional default node data applied when the user drags this palette item
   * onto the canvas (e.g., starter coordinates, initial strategy fields, etc.)
   */
  defaultData?: Partial<DslNode>;
}

/* ---------- Helper Guards ---------- */
export function isGatewayNode(n: DslNode): n is GatewayNode {
  return n.type === 'gateway';
}
export function isJoinNode(n: DslNode): n is JoinNode {
  return n.type === 'join';
}
export function isTimerNode(n: DslNode): n is TimerNode {
  return n.type === 'timer';
}
export function isAutomaticNode(n: DslNode): n is AutomaticNode {
  return n.type === 'automatic';
}
export function isHumanTaskNode(n: DslNode): n is HumanTaskNode {
  return n.type === 'humanTask';
}
