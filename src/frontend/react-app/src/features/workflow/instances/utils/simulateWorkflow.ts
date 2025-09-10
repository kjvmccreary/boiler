/**
 * Simulation (PR3 – Join Mode Semantics)
 * Enhancements over PR2:
 *  - Join node modes respected: all | any | count | quorum | expression
 *  - Subset / threshold subset enumeration for parallel → join convergence
 *  - CancelRemaining semantics (any/count with cancelRemaining=true) modeled (other branches pruned)
 *  - Expression mode evaluation with expanded minimal JsonLogic (var, ==, ===, !=, >, >=, <, <=, and, or, !)
 *
 * Caps / Guards:
 *  - Global path cap: maxPaths
 *  - Depth cap: maxDepth
 *  - Node revisit cap: maxVisitsPerNode
 *  - Cartesian / combination cap: maxBranchCartesian (applies to subset/combination enumeration too)
 *
 * Existing Limitations:
 *  - For any/count/quorum/expression when cancelRemaining = false we do not currently enumerate BOTH
 *    “early join” and “all branches to join then continue” variants (would explode paths).
 *    We favor early satisfaction subset paths only (gives conservative, earlier completion outlook).
 *  - Expression mode relies on minimal JsonLogic subset; extend if richer operators needed.
 */

type JL = any;
function applyJsonLogic(expr: JL, data: Record<string, any>): any {
  if (expr == null) return expr;
  if (typeof expr !== 'object' || Array.isArray(expr)) return expr;
  const keys = Object.keys(expr);
  if (keys.length !== 1) return undefined;
  const op = keys[0];
  const val = (expr as any)[op];

  const resolveVar = (path: string) => {
    if (!path) return undefined;
    const parts = path.split('.');
    let cur: any = data;
    for (const p of parts) {
      if (cur == null) return undefined;
      cur = cur[p];
    }
    return cur;
  };

  const resolve = (x: any) => {
    if (x && typeof x === 'object' && !Array.isArray(x) && Object.prototype.hasOwnProperty.call(x, 'var')) {
      const v = (x as any).var;
      if (typeof v === 'string') return resolveVar(v);
      return undefined;
    }
    if (x && typeof x === 'object' && !Array.isArray(x)) {
      // nested expression
      return applyJsonLogic(x, data);
    }
    return x;
  };

  const bin = (fn: (a: any, b: any) => any) => {
    if (Array.isArray(val) && val.length === 2) return fn(resolve(val[0]), resolve(val[1]));
    return false;
  };

  switch (op) {
    case 'var':
      return resolveVar(val);
    case '==':
    case '===':
      return bin((a, b) => a == b);
    case '!=':
      return bin((a, b) => a != b);
    case '>':
      return bin((a, b) => Number(a) > Number(b));
    case '>=':
      return bin((a, b) => Number(a) >= Number(b));
    case '<':
      return bin((a, b) => Number(a) < Number(b));
    case '<=':
      return bin((a, b) => Number(a) <= Number(b));
    case 'and':
      if (Array.isArray(val)) return val.every(v => !!applyJsonLogic(v, data));
      return false;
    case 'or':
      if (Array.isArray(val)) return val.some(v => !!applyJsonLogic(v, data));
      return false;
    case '!':
      return !applyJsonLogic(val, data);
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
    joinMode?: string;
    arrivedBranches?: number;
    totalBranches?: number;
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
  mode?: string;
  count?: number;
  thresholdCount?: number;
  thresholdPercent?: number;
  expression?: string;
  cancelRemaining?: boolean;
  // Added to support legacy / wrapped DSL structures
  properties?: Record<string, any>;
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

  const startNodes = def.nodes.filter(n => (n.type || '').toLowerCase() === 'start');
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

  function evaluateJoinExpression(expr: string | undefined, overlay: any): boolean {
    if (!expr) return false;
    try {
      const parsed = typeof expr === 'string' ? JSON.parse(expr) : expr;
      return !!applyJsonLogic(parsed, overlay);
    } catch {
      return false;
    }
  }

  function dfs(currentId: string, stack: string[]) {
    if (paths.length >= maxPaths) return;
    if (stack.length > maxDepth) {
      truncated = true; reasons.push('maxDepth'); return;
    }
    visitCounts[currentId] = (visitCounts[currentId] || 0) + 1;
    if (visitCounts[currentId] > maxVisitsPerNode) {
      truncated = true; reasons.push('visitCap'); return;
    }

    const node = nodeMap.get(currentId);
    if (!node) return;

    const nextEdges = outEdges.get(currentId) || [];
    if (nextEdges.length === 0) {
      emitPath([...stack, currentId], currentId, undefined);
      return;
    }

    if ((node.type || '').toLowerCase() === 'gateway') {
      switch ((node.strategy || 'exclusive').toLowerCase()) {
        case 'parallel':
          handleParallel(node, nextEdges, stack);
          return;
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
              if (truncateIfFull()) return;
            }
            return;
          }
          if (cond === false && falseEdges.length) {
            for (const e of falseEdges) {
              dfs(e.to, [...stack, currentId]);
              if (truncateIfFull()) return;
            }
            return;
          }
          for (const e of nextEdges) {
            dfs(e.to, [...stack, currentId]);
            if (truncateIfFull()) return;
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
        if (truncateIfFull()) return;
      }
    }
  }

  function handleParallel(gateway: WFNode, nextEdges: WFEdge[], stack: string[]) {
    if (!parallelExplode) {
      dfs(nextEdges[0].to, [...stack, gateway.id]);
      return;
    }

    // Gather branch fragments to first join (or terminal)
    const branchFrags: BranchFrag[][] = [];
    for (const edge of nextEdges) {
      const fragments: BranchFrag[] = [];
      exploreBranch(edge.to, [...stack, gateway.id], fragments, 0);
      branchFrags.push(fragments);
      if (truncated) return;
    }

    const firstJoin = commonFirstJoin(branchFrags);
    if (!firstJoin) {
      // partial convergence scenario
      for (const fragList of branchFrags) {
        for (const frag of fragList) {
          const last = frag.endNode;
          const cont = outEdges.get(last) || [];
          if (cont.length === 0) {
            emitPath(frag.path, last, { partialParallelMerge: true });
          } else {
            for (const ce of cont) {
              dfs(ce.to, frag.path);
              if (truncateIfFull()) return;
            }
          }
        }
      }
      return;
    }

    // JOIN semantics
    const joinNode = nodeMap.get(firstJoin);
    const joinMode = ((joinNode?.mode ?? joinNode?.properties?.mode) || 'all').toLowerCase();
    const cancelRemaining = (joinNode?.cancelRemaining ?? joinNode?.properties?.cancelRemaining) || false;
    const totalBranches = branchFrags.length;

    // Root branch identifiers
    const branchIds = nextEdges.map(e => e.to);

    // Filter fragments reaching the join
    const perBranchJoinFrags = branchFrags.map(frags => frags.filter(f => f.mergeJoin === firstJoin));
    if (perBranchJoinFrags.some(list => list.length === 0) && joinMode === 'all') {
      // fallback if some branch never reaches join
      for (const fragList of branchFrags) {
        for (const frag of fragList) {
            const last = frag.endNode;
            const cont = outEdges.get(last) || [];
            if (cont.length === 0) {
              emitPath(frag.path, last, { partialParallelMerge: true });
            } else {
              for (const ce of cont) {
                dfs(ce.to, frag.path);
                if (truncateIfFull()) return;
              }
            }
        }
      }
      return;
    }

    let combos: string[][] = [];

    const thresholdData = computeThreshold(joinMode, joinNode, totalBranches);
    const thresholdCount = thresholdData.thresholdCount;
    const effectiveMode = thresholdData.effectiveMode;

    switch (effectiveMode) {
      case 'all':
        combos = buildAllCombos(perBranchJoinFrags, firstJoin, maxBranchCartesian);
        break;
      case 'any':
        combos = buildAnyCombos(perBranchJoinFrags, firstJoin, maxBranchCartesian);
        break;
      case 'count':
      case 'quorum':
        combos = buildSubsetCountCombos(
          perBranchJoinFrags,
          firstJoin,
          branchIds,
          maxBranchCartesian,
          thresholdCount,
          false
        );
        break;
      case 'expression':
        combos = buildExpressionCombos(
          perBranchJoinFrags,
          firstJoin,
          joinNode?.expression ?? joinNode?.properties?.expression,
          joinMode,
          branchIds,
          maxBranchCartesian
        );
        break;
      default:
        combos = buildAllCombos(perBranchJoinFrags, firstJoin, maxBranchCartesian);
        break;
    }

    if (combos.length >= maxBranchCartesian) {
      cartesianCapped = true;
      reasons.push('cartesianCap');
    }

    for (const combo of combos.slice(0, maxBranchCartesian)) {
      const last = combo[combo.length - 1];
      const contEdges = outEdges.get(last) || [];
      if (contEdges.length === 0) {
        emitPath(combo, last, {
          mergedAtJoinId: firstJoin,
          joinMode: joinMode,
          arrivedBranches: deriveArrivedCount(combo, firstJoin, branchIds),
          totalBranches
        });
      } else {
        for (const ce of contEdges) {
          dfs(ce.to, combo);
          if (truncateIfFull()) return;
        }
      }
      if (truncateIfFull()) return;
    }
  }

  function emitPath(nodes: string[], terminal: string, meta?: Partial<SimulatedPath['meta']>) {
    if (paths.length >= maxPaths) return;
    paths.push({
      id: `p${++pathCounter}`,
      nodes,
      length: nodes.length,
      terminalNodeId: terminal,
      meta: meta ? { ...meta } : undefined
    });
    if (paths.length >= maxPaths) {
      truncated = true;
      reasons.push('maxPaths');
    }
  }

  function truncateIfFull() {
    if (paths.length >= maxPaths) {
      truncated = true;
      if (!reasons.includes('maxPaths')) reasons.push('maxPaths');
      return true;
    }
    return false;
  }

  // DFS from all starts
  for (const start of startNodes) {
    dfs(start.id, []);
    if (truncateIfFull()) break;
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
    if ((node.type || '').toLowerCase() === 'join') {
      frags.push({ endNode: startId, path, mergeJoin: startId });
      return;
    }
    if (outs.length === 0) {
      frags.push({ endNode: startId, path });
      return;
    }
    if (outs.length === 1 && (node.type || '').toLowerCase() !== 'gateway') {
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

  function mergeFragmentPaths(fragments: BranchFrag[], joinId: string): string[] {
    const merged: string[] = [];
    for (const frag of fragments) {
      for (const n of frag.path) {
        if (merged.length === 0 || merged[merged.length - 1] !== n) {
          merged.push(n);
        }
      }
    }
    if (merged[merged.length - 1] !== joinId) merged.push(joinId);
    return merged;
  }

  function buildAllCombos(perBranchFrags: BranchFrag[][], joinId: string, cap: number): string[][] {
    const out: string[][] = [];
    const selection: BranchFrag[] = [];
    function rec(idx: number) {
      if (out.length >= cap) return;
      if (idx === perBranchFrags.length) {
        out.push(mergeFragmentPaths(selection, joinId));
        return;
      }
      for (const frag of perBranchFrags[idx]) {
        if (frag.mergeJoin !== joinId) continue;
        selection.push(frag);
        rec(idx + 1);
        selection.pop();
        if (out.length >= cap) return;
      }
    }
    rec(0);
    return out;
  }

  function buildAnyCombos(perBranchFrags: BranchFrag[][], joinId: string, cap: number): string[][] {
    const out: string[][] = [];
    for (let i = 0; i < perBranchFrags.length; i++) {
      for (const frag of perBranchFrags[i]) {
        if (frag.mergeJoin !== joinId) continue;
        out.push(mergeFragmentPaths([frag], joinId));
        if (out.length >= cap) return out;
      }
    }
    return out;
  }

  function buildSubsetCountCombos(
    perBranchFrags: BranchFrag[][],
    joinId: string,
    branchIds: string[],
    cap: number,
    minCount: number,
    allowAllFallback: boolean
  ): string[][] {
    const out: string[][] = [];
    const branchCount = perBranchFrags.length;

    function choose(start: number, picked: number[]) {
      if (out.length >= cap) return;
      if (picked.length >= minCount) {
        buildSubsetCartesian(picked, 0, [], joinId);
        if (out.length >= cap) return;
      }
      for (let i = start; i < branchCount; i++) {
        picked.push(i);
        choose(i + 1, picked);
        picked.pop();
        if (out.length >= cap) return;
      }
    }

    function buildSubsetCartesian(subset: number[], idx: number, acc: BranchFrag[], joinId: string) {
      if (out.length >= cap) return;
      if (idx === subset.length) {
        out.push(mergeFragmentPaths(acc, joinId));
        return;
      }
      const branchIndex = subset[idx];
      for (const frag of perBranchFrags[branchIndex]) {
        if (frag.mergeJoin !== joinId) continue;
        acc.push(frag);
        buildSubsetCartesian(subset, idx + 1, acc, joinId);
        acc.pop();
        if (out.length >= cap) return;
      }
    }

    choose(0, []);

    if (allowAllFallback && out.length === 0 && minCount > branchCount) {
      return buildAllCombos(perBranchFrags, joinId, cap);
    }
    return out;
  }

  function buildExpressionCombos(
    perBranchFrags: BranchFrag[][],
    joinId: string,
    expression: string | undefined,
    mode: string,
    branchIds: string[],
    cap: number
  ): string[][] {
    const out: string[][] = [];
    const n = perBranchFrags.length;

    function choose(start: number, picked: number[]) {
      if (out.length >= cap) return;
      if (picked.length > 0) {
        const arrivedIds = picked.map(i => branchIds[i]);
        const overlay = {
          ...context,
          _joinEval: {
            mode,
            total: n,
            arrived: picked.length,
            remaining: n - picked.length,
            arrivedIds,
            branchIds
          }
        };
        if (evaluateJoinExpression(expression, overlay)) {
          buildSubsetCartesian(picked, 0, [], joinId);
          if (out.length >= cap) return;
        }
      }
      for (let i = start; i < n; i++) {
        picked.push(i);
        choose(i + 1, picked);
        picked.pop();
        if (out.length >= cap) return;
      }
    }

    function buildSubsetCartesian(subset: number[], idx: number, acc: BranchFrag[], joinId: string) {
      if (out.length >= cap) return;
      if (idx === subset.length) {
        out.push(mergeFragmentPaths(acc, joinId));
        return;
      }
      const branchIndex = subset[idx];
      for (const frag of perBranchFrags[branchIndex]) {
        if (frag.mergeJoin !== joinId) continue;
        acc.push(frag);
        buildSubsetCartesian(subset, idx + 1, acc, joinId);
        acc.pop();
        if (out.length >= cap) return;
      }
    }

    choose(0, []);
    return out;
  }

  function computeThreshold(mode: string, joinNode: WFNode | undefined, totalBranches: number) {
    const m = mode.toLowerCase();
    if (m === 'quorum') {
      let tCount = joinNode?.thresholdCount ?? joinNode?.properties?.thresholdCount ?? 0;
      const tPercent = joinNode?.thresholdPercent ?? joinNode?.properties?.thresholdPercent ?? 0;
      if (tCount <= 0 && tPercent > 0) {
        tCount = Math.ceil(totalBranches * (tPercent / 100.0));
      }
      if (tCount <= 0) tCount = totalBranches;
      return { thresholdCount: tCount, effectiveMode: 'quorum' as const };
    }
    if (m === 'count') {
      let c = joinNode?.count ?? joinNode?.properties?.count ?? 0;
      if (c <= 0) c = totalBranches;
      return { thresholdCount: c, effectiveMode: 'count' as const };
    }
    if (m === 'any') return { thresholdCount: 1, effectiveMode: 'any' as const };
    if (m === 'expression') return { thresholdCount: 0, effectiveMode: 'expression' as const };
    return { thresholdCount: totalBranches, effectiveMode: 'all' as const };
  }

  function deriveArrivedCount(path: string[], joinId: string, branchIds: string[]): number {
    const set = new Set<string>();
    for (const b of branchIds) {
      if (path.includes(b)) set.add(b);
    }
    if (!path.includes(joinId)) return set.size;
    return set.size;
  }
}
