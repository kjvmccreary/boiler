import type { DslDefinition, DslNode, ValidationResult } from './dsl.types';

/**
 * Validates a workflow definition according to current builder rules (extended for gateway & join strategies).
 */
export function validateDefinition(definition: DslDefinition): ValidationResult {
  const errors: string[] = [];
  const warnings: string[] = [];

  // Rule 1: Must have exactly one Start node
  const startNodes = definition.nodes.filter(n => n.type === 'start');
  if (startNodes.length === 0) {
    errors.push('Workflow must have exactly one Start node');
  } else if (startNodes.length > 1) {
    errors.push('Workflow can only have one Start node');
  }

  // Rule 2: Must have at least one End node
  const endNodes = definition.nodes.filter(n => n.type === 'end');
  if (endNodes.length === 0) {
    errors.push('Workflow must have at least one End node');
  }

  // Rule 3: Duplicate node IDs
  const idCounts: Record<string, number> = {};
  for (const n of definition.nodes) idCounts[n.id] = (idCounts[n.id] || 0) + 1;
  const duplicates = Object.entries(idCounts)
    .filter(([, c]) => c > 1)
    .map(([id]) => id);
  if (duplicates.length > 0) errors.push(`Duplicate node IDs: ${duplicates.join(', ')}`);

  // Rule 4: Reachability from Start
  if (startNodes.length === 1) {
    const reachableNodes = findReachableNodes(definition, startNodes[0].id);
    const unreachable = definition.nodes.filter(n =>
      n.type !== 'start' && !reachableNodes.has(n.id)
    );
    if (unreachable.length > 0) {
      errors.push(`Unreachable nodes: ${unreachable.map(n => n.label || n.id).join(', ')}`);
    }
  }

  // Rule 5: Edges must connect existing nodes
  for (const edge of definition.edges) {
    const fromNode = definition.nodes.find(n => n.id === edge.from);
    const toNode = definition.nodes.find(n => n.id === edge.to);
    if (!fromNode) errors.push(`Edge ${edge.id} references non-existent source node: ${edge.from}`);
    if (!toNode) errors.push(`Edge ${edge.id} references non-existent target node: ${edge.to}`);
  }

  // Rule 6: Gateway validation (strategy-aware)
  for (const node of definition.nodes) {
    if (node.type === 'gateway') {
      const gw: any = node;
      const inferredStrategy: string = gw.strategy
        ? gw.strategy
        : (gw.condition ? 'conditional' : 'exclusive');

      if (!gw.strategy) {
        gw.strategy = inferredStrategy;
      }

      if (inferredStrategy === 'conditional') {
        if (!gw.condition || typeof gw.condition !== 'string' || gw.condition.trim() === '') {
          errors.push(`Gateway node "${node.label || node.id}" must have a condition`);
        } else {
          try { JSON.parse(gw.condition); }
          catch { errors.push(`Gateway node "${node.label || node.id}" condition is not valid JSON`); }
        }
      } else if (inferredStrategy === 'parallel') {
        const outgoing = definition.edges.filter(e => e.from === node.id);
        if (outgoing.length < 2) {
          warnings.push(`Parallel gateway "${node.label || node.id}" should have at least 2 outgoing branches`);
        }
      }
    }
  }

  // Rule 7: Join validation (base)
  for (const node of definition.nodes) {
    if (node.type === 'join') {
      const j: any = node;
      const mode = j.mode || 'all';
      switch (mode) {
        case 'count':
          if (j.thresholdCount == null || j.thresholdCount <= 0) {
            errors.push(`Join "${node.label || node.id}" (count) requires positive thresholdCount`);
          }
          break;
        case 'quorum':
          if (j.thresholdPercent == null || j.thresholdPercent <= 0 || j.thresholdPercent > 100) {
            errors.push(`Join "${node.label || node.id}" (quorum) requires thresholdPercent between 1 and 100`);
          }
          break;
        case 'expression':
          if (!j.expression || typeof j.expression !== 'string' || !j.expression.trim()) {
            errors.push(`Join "${node.label || node.id}" (expression) requires a non-empty expression`);
          } else {
            try { JSON.parse(j.expression); }
            catch { errors.push(`Join "${node.label || node.id}" expression is not valid JSON`); }
          }
          break;
        case 'all':
        case 'any':
          // no extra requirements
          break;
        default:
          errors.push(`Join "${node.label || node.id}" has invalid mode "${mode}"`);
      }
    }
  }

  // Rule 8: HumanTask advisory (warning)
  for (const node of definition.nodes) {
    if (node.type === 'humanTask') {
      const ht: any = node;
      if (!ht.assigneeRoles || ht.assigneeRoles.length === 0) {
        warnings.push(`HumanTask node "${node.label || node.id}" should have assignee roles`);
      }
    }
  }

  // Rule 9 (M2): Parallel ↔ Join structural coherence
  applyParallelJoinStructuralValidation(definition, errors, warnings);

  return {
    isValid: errors.length === 0,
    errors,
    warnings
  };
}

/**
 * Find all nodes reachable from a starting node
 */
function findReachableNodes(definition: DslDefinition, startNodeId: string): Set<string> {
  const reachable = new Set<string>();
  const toVisit = [startNodeId];
  while (toVisit.length > 0) {
    const currentId = toVisit.pop()!;
    if (reachable.has(currentId)) continue;
    reachable.add(currentId);
    const outgoing = definition.edges.filter(e => e.from === currentId);
    for (const edge of outgoing) {
      if (!reachable.has(edge.to)) toVisit.push(edge.to);
    }
  }
  return reachable;
}

/**
 * Advanced structural validation (M2):
 * - Parallel gateway should eventually converge at a join (warn if not).
 * - Each join should have >= 2 incoming edges (error).
 * - Join should merge at least two distinct branches diverged from some earlier parallel gateway (warn if not detected).
 * - If a parallel gateway's branches converge at multiple joins or partial merges (some branches skip), issue warnings.
 */
function applyParallelJoinStructuralValidation(def: DslDefinition, errors: string[], warnings: string[]) {
  const nodeMap: Record<string, DslNode> = {};
  for (const n of def.nodes) nodeMap[n.id] = n;

  const edgesFrom: Record<string, string[]> = {};
  const edgesTo: Record<string, string[]> = {};
  for (const e of def.edges) {
    (edgesFrom[e.from] ||= []).push(e.to);
    (edgesTo[e.to] ||= []).push(e.from);
  }

  // --- Join basic in-degree rule
  for (const j of def.nodes.filter(n => n.type === 'join')) {
    const incoming = edgesTo[j.id] || [];
    if (incoming.length < 2) {
      errors.push(`Join "${j.label || j.id}" must have at least 2 incoming edges`);
    }
  }

  // Helper: BFS forward until first join(s) encountered; return the first join(s) for that branch
  const FIRST_JOIN_LIMIT = 200; // safety
  function findFirstJoinDownstream(start: string): string[] {
    const visited = new Set<string>();
    const queue: { id: string; depth: number }[] = [{ id: start, depth: 0 }];
    const found: string[] = [];
    let foundDepth: number | undefined;

    while (queue.length > 0) {
      const { id, depth } = queue.shift()!;
      if (visited.has(id)) continue;
      visited.add(id);
      if (depth > FIRST_JOIN_LIMIT) break;

      const n = nodeMap[id];
      if (n && n.type === 'join') {
        if (foundDepth === undefined) foundDepth = depth;
        if (depth === foundDepth) {
          found.push(id);
          continue; // still allow picking up other joins at same depth
        } else if (depth > foundDepth) {
          // deeper than first join depth – stop exploring further
          continue;
        }
      }

      // Only keep exploring if we haven't locked to join depth
      if (foundDepth === undefined) {
        const outs = edgesFrom[id] || [];
        for (const to of outs) queue.push({ id: to, depth: depth + 1 });
      }
    }

    return found;
  }

  // Track branch -> join hits per parallel gateway
  for (const gw of def.nodes.filter(n => n.type === 'gateway')) {
    const gwAny: any = gw;
    const strategy = gwAny.strategy || (gwAny.condition ? 'conditional' : 'exclusive');
    if (strategy !== 'parallel') continue;

    const outgoing = edgesFrom[gw.id] || [];
    if (outgoing.length < 2) {
      // Already warned earlier; skip deeper analysis
      continue;
    }

    // For each outgoing branch start node, find first downstream join(s)
    const branchJoins: Record<string, Set<string>> = {};
    for (const target of outgoing) {
      const firstJoins = findFirstJoinDownstream(target);
      branchJoins[target] = new Set(firstJoins);
    }

    const allJoinIds = new Set<string>();
    Object.values(branchJoins).forEach(s => s.forEach(j => allJoinIds.add(j)));

    if (allJoinIds.size === 0) {
      warnings.push(`Parallel gateway "${gw.label || gw.id}" has no downstream join (branches never reconverge)`);
      continue;
    }

    // Determine coverage: how many branches reach each join
    const joinCoverage: Record<string, number> = {};
    for (const j of allJoinIds) joinCoverage[j] = 0;
    for (const br in branchJoins) {
      for (const j of branchJoins[br]) {
        joinCoverage[j] += 1;
      }
    }

    const totalBranches = outgoing.length;
    const fullMergeJoins = Object.entries(joinCoverage)
      .filter(([, c]) => c === totalBranches)
      .map(([j]) => j);

    // Warn if no single join covers all branches (partial merges)
    if (fullMergeJoins.length === 0) {
      warnings.push(`Parallel gateway "${gw.label || gw.id}" branches do not all merge at a single join (partial convergence detected)`);
    } else if (fullMergeJoins.length > 1) {
      warnings.push(`Parallel gateway "${gw.label || gw.id}" branches fully merge at multiple joins (${fullMergeJoins.join(', ')})`);
    }

    // Branches that fail to reach any join
    for (const br of outgoing) {
      if (branchJoins[br].size === 0) {
        warnings.push(`Parallel gateway "${gw.label || gw.id}" branch starting at "${br}" does not reach a join`);
      }
    }
  }

  // For each join, attempt to detect if it merges branches from a parallel gateway
  for (const j of def.nodes.filter(n => n.type === 'join')) {
    const incoming = edgesTo[j.id] || [];
    if (incoming.length < 2) continue; // already errored if <2

    // Simple heuristic: if any ancestor path (depth-limited) includes a parallel gateway with >=2 outgoing edges
    const ancestorHasParallel = ancestorSearchForParallel(def, j.id, nodeMap, edgesTo);
    if (!ancestorHasParallel) {
      warnings.push(`Join "${j.label || j.id}" does not appear to merge branches from a parallel gateway`);
    }
  }
}

/**
 * Reverse search up to detect a parallel gateway ancestor (depth limited).
 */
function ancestorSearchForParallel(
  def: DslDefinition,
  startId: string,
  nodeMap: Record<string, DslNode>,
  edgesTo: Record<string, string[]>,
  depthLimit = 200
): boolean {
  const visited = new Set<string>();
  const stack: { id: string; depth: number }[] = [{ id: startId, depth: 0 }];
  while (stack.length) {
    const { id, depth } = stack.pop()!;
    if (visited.has(id)) continue;
    visited.add(id);
    if (depth > depthLimit) break;
    const parents = edgesTo[id] || [];
    for (const p of parents) {
      const pn = nodeMap[p];
      if (pn && pn.type === 'gateway') {
        const anyPn: any = pn;
        const strat = anyPn.strategy || (anyPn.condition ? 'conditional' : 'exclusive');
        if (strat === 'parallel') {
          const outs = (def.edges.filter(e => e.from === pn.id) || []).length;
            if (outs >= 2) return true;
        }
      }
      stack.push({ id: p, depth: depth + 1 });
    }
  }
  return false;
}

/**
 * Validates a single node (strategy / join / timer aware).
 */
export function validateNode(node: DslNode): ValidationResult {
  const errors: string[] = [];
  const warnings: string[] = [];

  if (!node.id) errors.push('Node must have an ID');
  if (!node.type) errors.push('Node must have a type');

  switch (node.type) {
    case 'gateway': {
      const gw: any = node;
      const inferredStrategy: string = gw.strategy
        ? gw.strategy
        : (gw.condition ? 'conditional' : 'exclusive');
      if (inferredStrategy === 'conditional') {
        if (!gw.condition || typeof gw.condition !== 'string' || gw.condition.trim() === '') {
          errors.push('Gateway node must have a condition');
        } else {
          try { JSON.parse(gw.condition); }
          catch { errors.push('Gateway node condition is not valid JSON'); }
        }
      }
      break;
    }
    case 'join': {
      const j: any = node;
      const mode = j.mode || 'all';
      if (mode === 'expression') {
        if (!j.expression || !j.expression.trim()) {
          errors.push('Join (expression) requires expression');
        } else {
          try { JSON.parse(j.expression); }
          catch { errors.push('Join (expression) invalid JSON'); }
        }
      }
      if (mode === 'count' && (j.thresholdCount == null || j.thresholdCount <= 0)) {
        errors.push('Join (count) requires positive thresholdCount');
      }
      if (mode === 'quorum' &&
        (j.thresholdPercent == null || j.thresholdPercent <= 0 || j.thresholdPercent > 100)) {
        errors.push('Join (quorum) requires thresholdPercent between 1 and 100');
      }
      break;
    }
    case 'timer': {
      const t: any = node;
      const hasRelative = (t.delayMinutes && t.delayMinutes > 0) || (t.delaySeconds && t.delaySeconds > 0);
      const hasAbsolute = !!t.untilIso;
      if (!hasRelative && !hasAbsolute) {
        errors.push('Timer node must specify a relative delay (minutes/seconds) or an absolute untilIso');
      }
      if (hasRelative && hasAbsolute) {
        errors.push('Timer node cannot have both relative delay and absolute untilIso');
      }
      if (hasRelative) {
        if (t.delayMinutes && t.delayMinutes < 0) errors.push('Timer delayMinutes must be >= 0');
        if (t.delaySeconds && t.delaySeconds < 0) errors.push('Timer delaySeconds must be >= 0');
        if ((t.delayMinutes || 0) === 0 && (t.delaySeconds || 0) === 0) {
          errors.push('Timer relative delay must be > 0 total');
        }
      }
      if (hasAbsolute) {
        const dt = new Date(t.untilIso);
        if (isNaN(dt.getTime())) errors.push('Timer untilIso must be a valid ISO datetime');
        else if (dt.getTime() <= Date.now()) errors.push('Timer untilIso must be in the future');
      }
      break;
    }
    case 'humanTask': {
      const ht: any = node;
      if (!ht.assigneeRoles || ht.assigneeRoles.length === 0) {
        warnings.push('HumanTask should have assignee roles');
      }
      break;
    }
  }

  return {
    isValid: errors.length === 0,
    errors,
    warnings
  };
}
