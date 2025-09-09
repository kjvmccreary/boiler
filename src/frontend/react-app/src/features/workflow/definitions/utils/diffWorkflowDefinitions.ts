/**
 * diffWorkflowDefinitions
 * Baseline DSL diff (PR1)
 * - Compares two workflow definition JSON payloads (current vs previous version).
 * - Detects added / removed / modified nodes and added / removed edges.
 * - Node identity: node.id (case-sensitive); if missing, skipped.
 * - Edge identity: edge.id OR fallback `${from}->${to}` (from may be 'source' / 'from').
 * - Modified node: signature of key properties changed.
 *
 * NOTE: Keeps logic defensive (malformed JSON tolerated).
 */

export interface WorkflowDslNode {
  id: string;
  type?: string;
  label?: string;
  [k: string]: any;
}

export interface WorkflowDslEdge {
  id?: string;
  from?: string;
  to?: string;
  source?: string;
  target?: string;
  [k: string]: any;
}

export interface ChangedField {
  field: string;
  before: any;
  after: any;
}

export interface NodeModification {
  id: string;
  type?: string;
  label?: string;
  changedKeys: string[];              // legacy quick list
  changedFields: ChangedField[];      // field-level before/after (flattened)
  before: Record<string, any>;
  after: Record<string, any>;
}

export interface DiffResult {
  addedNodes: WorkflowDslNode[];
  removedNodes: WorkflowDslNode[];
  modifiedNodes: NodeModification[];
  addedEdges: WorkflowDslEdge[];
  removedEdges: WorkflowDslEdge[];
  summary: {
    addedNodes: number;
    removedNodes: number;
    modifiedNodes: number;
    addedEdges: number;
    removedEdges: number;
  };
  error?: string;
}

// Baseline scalar fields always compared
const BASE_FIELDS = ['type', 'label'];

// Per node-type extraction rules (flatten to scalar signature fields)
function extractNodeSignature(n: WorkflowDslNode): Record<string, any> {
  const out: Record<string, any> = {};
  for (const k of BASE_FIELDS) {
    if (n[k] !== undefined) out[k] = n[k];
  }

  switch (n.type) {
    case 'gateway':
      if (n.strategy !== undefined) out.strategy = n.strategy;
      if (n.condition !== undefined) out.condition = n.condition;
      break;
    case 'join':
      if (n.mode !== undefined) out.mode = n.mode;
      if (n.thresholdCount !== undefined) out.thresholdCount = n.thresholdCount;
      if (n.thresholdPercent !== undefined) out.thresholdPercent = n.thresholdPercent;
      if (n.expression !== undefined) out.expression = n.expression;
      if (n.cancelRemaining !== undefined) out.cancelRemaining = n.cancelRemaining;
      break;
    case 'humanTask':
      if (n.assignment) {
        if (n.assignment.mode !== undefined) out.assignmentMode = n.assignment.mode;
        if (Array.isArray(n.assignment.roles)) out.assignmentRolesCount = n.assignment.roles.length;
        if (Array.isArray(n.assignment.users)) out.assignmentUsersCount = n.assignment.users.length;
        if (n.assignment.expression !== undefined) out.assignmentExpressionHash = hashSmall(n.assignment.expression);
      }
      break;
    case 'timer':
      if (n.delaySeconds !== undefined) out.delaySeconds = n.delaySeconds;
      if (n.delayMinutes !== undefined) out.delayMinutes = n.delayMinutes;
      if (n.untilIso !== undefined) out.untilIso = n.untilIso;
      break;
    case 'automatic':
      if (n.action) {
        if (n.action.kind !== undefined) out.actionKind = n.action.kind;
        if (n.action.retryPolicy?.maxAttempts !== undefined) out.retryMaxAttempts = n.action.retryPolicy.maxAttempts;
      }
      break;
    default:
      // no-op for other types
      break;
  }
  return out;
}

function hashSmall(v: string) {
  // cheap non-cryptographic hash for large expressions to avoid noise in diff panel
  let h = 0;
  for (let i = 0; i < v.length; i++) {
    h = (h * 31 + v.charCodeAt(i)) | 0;
  }
  return `h${Math.abs(h)}`;
}

function safeParse(json?: string | null): any {
  if (!json) return null;
  try {
    return JSON.parse(json);
  } catch {
    return null;
  }
}

function changedKeys(prev: Record<string, any>, curr: Record<string, any>): string[] {
  const keys = new Set<string>([...Object.keys(prev), ...Object.keys(curr)]);
  const out: string[] = [];
  for (const k of keys) {
    const a = prev[k];
    const b = curr[k];
    const eq = typeof a === 'object' && typeof b === 'object'
      ? JSON.stringify(a) === JSON.stringify(b)
      : a === b;
    if (!eq) out.push(k);
  }
  return out;
}

function buildChangedFields(prevSig: Record<string, any>, currSig: Record<string, any>, keys: string[]): ChangedField[] {
  return keys.map(k => ({
    field: k,
    before: prevSig[k],
    after: currSig[k]
  }));
}

export function diffWorkflowDefinitions(currentJson?: string | null, previousJson?: string | null): DiffResult {
  const current = safeParse(currentJson);
  const previous = safeParse(previousJson);

  if (!current || !previous) {
    return {
      addedNodes: [],
      removedNodes: [],
      modifiedNodes: [],
      addedEdges: [],
      removedEdges: [],
      summary: { addedNodes: 0, removedNodes: 0, modifiedNodes: 0, addedEdges: 0, removedEdges: 0 },
      error: 'One or both definitions failed to parse'
    };
  }

  const currentNodes: WorkflowDslNode[] = Array.isArray(current.nodes) ? current.nodes : [];
  const prevNodes: WorkflowDslNode[] = Array.isArray(previous.nodes) ? previous.nodes : [];
  const currentEdges: WorkflowDslEdge[] = Array.isArray(current.edges) ? current.edges : [];
  const prevEdges: WorkflowDslEdge[] = Array.isArray(previous.edges) ? previous.edges : [];

  const prevNodeMap = new Map<string, WorkflowDslNode>();
  prevNodes.forEach(n => { if (n?.id) prevNodeMap.set(n.id, n); });

  const currNodeMap = new Map<string, WorkflowDslNode>();
  currentNodes.forEach(n => { if (n?.id) currNodeMap.set(n.id, n); });

  const addedNodes: WorkflowDslNode[] = [];
  const removedNodes: WorkflowDslNode[] = [];
  const modifiedNodes: NodeModification[] = [];

  // Additions & modifications
  for (const [id, cn] of currNodeMap.entries()) {
    if (!prevNodeMap.has(id)) {
      addedNodes.push(cn);
      continue;
    }
    const pn = prevNodeMap.get(id)!;
    const prevSig = extractNodeSignature(pn);
    const currSig = extractNodeSignature(cn);
    const diffs = changedKeys(prevSig, currSig);
    if (diffs.length > 0) {
      modifiedNodes.push({
        id,
        type: cn.type || pn.type,
        label: cn.label || pn.label,
        changedKeys: diffs,
        changedFields: buildChangedFields(prevSig, currSig, diffs),
        before: prevSig,
        after: currSig
      });
    }
  }

  // Removals
  for (const [id, pn] of prevNodeMap.entries()) {
    if (!currNodeMap.has(id)) removedNodes.push(pn);
  }

  // Edges (use identity fallback)
  const edgeId = (e: WorkflowDslEdge) => e.id || `${e.from || e.source}->${e.to || e.target}`;
  const prevEdgeMap = new Map<string, WorkflowDslEdge>();
  prevEdges.forEach(e => {
    const id = edgeId(e);
    if (id) prevEdgeMap.set(id, e);
  });
  const currEdgeMap = new Map<string, WorkflowDslEdge>();
  currentEdges.forEach(e => {
    const id = edgeId(e);
    if (id) currEdgeMap.set(id, e);
  });

  const addedEdges: WorkflowDslEdge[] = [];
  const removedEdges: WorkflowDslEdge[] = [];

  for (const [id, ce] of currEdgeMap.entries()) {
    if (!prevEdgeMap.has(id)) addedEdges.push(ce);
  }
  for (const [id, pe] of prevEdgeMap.entries()) {
    if (!currEdgeMap.has(id)) removedEdges.push(pe);
  }

  return {
    addedNodes,
    removedNodes,
    modifiedNodes,
    addedEdges,
    removedEdges,
    summary: {
      addedNodes: addedNodes.length,
      removedNodes: removedNodes.length,
      modifiedNodes: modifiedNodes.length,
      addedEdges: addedEdges.length,
      removedEdges: removedEdges.length
    }
  };
}
