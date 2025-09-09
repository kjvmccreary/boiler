/**
 * Simulation (PR2) – path enumeration with optional parallel branch cartesian merge.
 * Enumerates terminal paths from start nodes with caps for depth, visits, and path count.
 * Parallel gateways:
 *   - If parallelExplode=false: linearize (take first branch).
 *   - Else: explore each branch to first join(s). If all reach same first join -> cartesian merge (capped).
 * Conditional gateways:
 *   - Evaluates JsonLogic condition if present; otherwise explores all outgoing edges.
 * Exclusive gateways:
 *   - Chooses first edge (placeholder for future scoring).
 *
 * NOTE: 'json-logic-js' package not required at build here; a minimal evaluator is inlined.
 * Replace applyJsonLogic with json-logic-js if full feature set needed.
 */

// ---- Minimal JsonLogic subset evaluator (==, ===, and, or, var) ----
type JL = any;
function applyJsonLogic(expr: JL, data: Record<string, any>): any {
  if (expr == null || typeof expr !== 'object' || Array.isArray(expr)) return expr;
  const keys = Object.keys(expr);
  if (keys.length !== 1) return undefined;
  const op = keys[0];
  const val = (expr as any)[op];

  const resolve = (x: any) => {
    if (x && typeof x === 'object' && !Array.isArray(x) && Object.prototype.hasOwnProperty.call(x, 'var')) {
      return data[(x as any).var];
    }
    return x;
  };

  switch (op) {
    case '==':
    case '===':
      if (Array.isArray(val) && val.length === 2) return resolve(val[0]) == resolve(val[1]);
      return false;
    case 'and':
      if (Array.isArray(val)) return val.every(v => !!applyJsonLogic(v, data));
      return false;
    case 'or':
      if (Array.isArray(val)) return val.some(v => !!applyJsonLogic(v, data));
      return false;
    default:
      return undefined;
  }
}

export interface SimulatedPath {
  id: string;
  nodes: string[];
  length: number;
  terminalNodeId: string;
  meta?: {
    mergedAtJoinId?: string;
    partialParallelMerge?: boolean;
  };
}

export interface SimulationResult {
  paths: SimulatedPath[];
  truncated: boolean;
  reasons: string[];
  cartesianCapped?: boolean;
}

interface WFNode {
  id: string;
  type?: string;
  label?: string;
  strategy?: string;
  condition?: any;
}

interface WFEdge {
  id?: string;
  from: string;
  to: string;
  label?: string;
  fromHandle?: string;
}

interface WFDefinition {
  nodes: WFNode[];
  edges: WFEdge[];
}

export interface SimulationOptions {
  context?: Record<string, any>;
  maxPaths?: number;
  maxDepth?: number;
  maxVisitsPerNode?: number;
  parallelExplode?: boolean;
  maxBranchCartesian?: number;
}

type BranchFrag = { endNode: string; path: string[]; mergeJoin?: string };

export function simulateWorkflow(defJson: string | null, opts: SimulationOptions = {}): SimulationResult {
  if (!defJson) return { paths: [], truncated: false, reasons: [] };
  let def: WFDefinition;
  try {
    def = JSON.parse(defJson);
  } catch {
    return { paths: [], truncated: false, reasons: [] };
  }

  const {
    context = {},
    maxPaths = 200,
    maxDepth = 250,
    maxVisitsPerNode = 5,
    parallelExplode = true,
    maxBranchCartesian = 64
  } = opts;

  const nodeMap = new Map(def.nodes.map(n => [n.id, n]));
  const outEdges = new Map<string, WFEdge[]>();
  for (const e of def.edges) {
    if (!outEdges.has(e.from)) outEdges.set(e.from, []);
    outEdges.get(e.from)!.push(e);
  }

  const startNodes = def.nodes.filter(n => n.type === 'start');
  if (startNodes.length === 0) return { paths: [], truncated: false, reasons: [] };

  const paths: SimulatedPath[] = [];
  let truncated = false;
  const reasons: string[] = [];
  let cartesianCapped = false;
  let pathCounter = 0;

  const visitCounts: Record<string, number> = {};

  function evaluateCondition(node: WFNode): boolean | undefined {
    if (!node.condition) return undefined;
    try {
      const parsed = typeof node.condition === 'string' ? JSON.parse(node.condition) : node.condition;
      return !!applyJsonLogic(parsed, context);
    } catch {
      return undefined;
    }
  }

  function dfs(currentId: string, stack: string[]) {
    if (stack.length > maxDepth) {
      truncated = true;
      reasons.push('maxDepth');
      return;
    }
    visitCounts[currentId] = (visitCounts[currentId] || 0) + 1;
    if (visitCounts[currentId] > maxVisitsPerNode) {
      truncated = true;
      reasons.push('visitCap');
      return;
    }

    const node = nodeMap.get(currentId);
    if (!node) return;

    const nextEdges = outEdges.get(currentId) || [];
    if (nextEdges.length === 0) {
      paths.push({
        id: `p${++pathCounter}`,
        nodes: [...stack, currentId],
        length: stack.length + 1,
        terminalNodeId: currentId
      });
      return;
    }

    if (node.type === 'gateway') {
      switch (node.strategy) {
        case 'parallel': {
          if (!parallelExplode) {
            dfs(nextEdges[0].to, [...stack, currentId]);
            return;
          }
          const branchFrags: BranchFrag[][] = [];
          for (const edge of nextEdges) {
            const fragments: BranchFrag[] = [];
            exploreBranch(edge.to, [...stack, currentId], fragments, 0);
            branchFrags.push(fragments);
            if (truncated) return;
          }
          const firstJoin = commonFirstJoin(branchFrags);
          if (firstJoin) {
            const combos: string[][] = [];
            buildCartesian(branchFrags, 0, [], combos, firstJoin, maxBranchCartesian);
            if (combos.length >= maxBranchCartesian) {
              cartesianCapped = true;
              reasons.push('cartesianCap');
            }
            for (const combo of combos.slice(0, maxBranchCartesian)) {
              const last = combo[combo.length - 1];
              const contEdges = outEdges.get(last) || [];
              if (contEdges.length === 0) {
                paths.push({
                  id: `p${++pathCounter}`,
                  nodes: combo,
                  length: combo.length,
                  terminalNodeId: last,
                  meta: { mergedAtJoinId: firstJoin }
                });
              } else {
                for (const ce of contEdges) {
                  dfs(ce.to, combo);
                  if (paths.length >= maxPaths) { truncated = true; reasons.push('maxPaths'); return; }
                }
              }
            }
            return;
          }
          // partial convergence
          for (const fragList of branchFrags) {
            for (const frag of fragList) {
              const last = frag.endNode;
              const cont = outEdges.get(last) || [];
              if (cont.length === 0) {
                paths.push({
                  id: `p${++pathCounter}`,
                  nodes: frag.path,
                  length: frag.path.length,
                  terminalNodeId: last,
                  meta: { partialParallelMerge: true }
                });
              } else {
                for (const ce of cont) {
                  dfs(ce.to, frag.path);
                  if (paths.length >= maxPaths) { truncated = true; reasons.push('maxPaths'); return; }
                }
              }
            }
          }
          return;
        }
        case 'conditional': {
          const cond = evaluateCondition(node);
          const trueEdges = nextEdges.filter(e =>
            e.label?.toLowerCase() === 'true' || e.fromHandle === 'true'
          );
          const falseEdges = nextEdges.filter(e =>
            e.label?.toLowerCase() === 'false' || e.fromHandle === 'false'
          );
          if (cond === true && trueEdges.length) {
            for (const e of trueEdges) {
              dfs(e.to, [...stack, currentId]);
              if (paths.length >= maxPaths) { truncated = true; reasons.push('maxPaths'); return; }
            }
            return;
          }
            if (cond === false && falseEdges.length) {
              for (const e of falseEdges) {
                dfs(e.to, [...stack, currentId]);
                if (paths.length >= maxPaths) { truncated = true; reasons.push('maxPaths'); return; }
              }
              return;
            }
          for (const e of nextEdges) {
            dfs(e.to, [...stack, currentId]);
            if (paths.length >= maxPaths) { truncated = true; reasons.push('maxPaths'); return; }
          }
          return;
        }
        case 'exclusive':
        default:
          dfs(nextEdges[0].to, [...stack, currentId]);
          return;
      }
    } else {
      for (const e of nextEdges) {
        dfs(e.to, [...stack, currentId]);
        if (paths.length >= maxPaths) { truncated = true; reasons.push('maxPaths'); return; }
      }
    }
  }

  for (const start of startNodes) {
    dfs(start.id, []);
    if (paths.length >= maxPaths) {
      truncated = true;
      reasons.push('maxPaths');
      break;
    }
  }

  return {
    paths,
    truncated,
    reasons: Array.from(new Set(reasons)),
    cartesianCapped
  };

  // ---------------- Helpers ----------------

  function exploreBranch(
    startId: string,
    base: string[],
    frags: BranchFrag[],
    depth: number
  ) {
    if (depth > maxDepth) { truncated = true; reasons.push('maxDepth'); return; }
    const node = nodeMap.get(startId);
    if (!node) return;
    const path = [...base, startId];
    const outs = outEdges.get(startId) || [];
    if (node.type === 'join') {
      frags.push({ endNode: startId, path, mergeJoin: startId });
      return;
    }
    if (outs.length === 0) {
      frags.push({ endNode: startId, path });
      return;
    }
    if (outs.length === 1 && node.type !== 'gateway') {
      exploreBranch(outs[0].to, path, frags, depth + 1);
      return;
    }
    for (const oe of outs) {
      exploreBranch(oe.to, path, frags, depth + 1);
      if (frags.length > maxPaths) { truncated = true; reasons.push('maxPaths'); return; }
    }
  }

  function commonFirstJoin(branchFrags: BranchFrag[][]): string | null {
    const joinSets = branchFrags.map(list =>
      new Set(list.filter(f => f.mergeJoin).map(f => f.mergeJoin!))
    );
    if (joinSets.some(s => s.size === 0)) return null;
    const first = joinSets[0];
    for (const cand of first) {
      if (joinSets.every(s => s.has(cand))) return cand;
    }
    return null;
  }

  function buildCartesian(
    parts: BranchFrag[][],
    idx: number,
    acc: string[][],
    out: string[][],
    joinId: string,
    cap: number
  ) {
    if (out.length >= cap) return;
    if (idx === parts.length) {
      const merged: string[] = [];
      for (const fragment of acc) {
        for (const n of fragment) {
          if (merged.length === 0 || merged[merged.length - 1] !== n) {
            merged.push(n);
          }
        }
      }
      if (merged[merged.length - 1] !== joinId) merged.push(joinId);
      out.push(merged);
      return;
    }
    for (const frag of parts[idx]) {
      if (frag.mergeJoin !== joinId) continue;
      acc.push(frag.path);
      buildCartesian(parts, idx + 1, acc, out, joinId, cap);
      acc.pop();
      if (out.length >= cap) return;
    }
  }
}
