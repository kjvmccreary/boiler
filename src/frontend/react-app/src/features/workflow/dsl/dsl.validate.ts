import type { DslDefinition, DslNode, ValidationResult } from './dsl.types';

/**
 * Validates a workflow definition according to current builder rules (extended & refined for gateway & join strategies).
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
  const duplicates = Object.entries(idCounts).filter(([, c]) => c > 1).map(([id]) => id);
  if (duplicates.length > 0) errors.push(`Duplicate node IDs: ${duplicates.join(', ')}`);

  // Rule 4: Reachability from Start
  if (startNodes.length === 1) {
    const reachableNodes = findReachableNodes(definition, startNodes[0].id);
    const unreachable = definition.nodes.filter(n => n.type !== 'start' && !reachableNodes.has(n.id));
    if (unreachable.length > 0) {
      errors.push(`Unreachable nodes: ${unreachable.map(n => n.label || n.id).join(', ')}`);
    }
  }

  // Rule 5: Edges connect existing nodes
  for (const edge of definition.edges) {
    const fromNode = definition.nodes.find(n => n.id === edge.from);
    const toNode = definition.nodes.find(n => n.id === edge.to);
    if (!fromNode) errors.push(`Edge ${edge.id} references non-existent source node: ${edge.from}`);
    if (!toNode) errors.push(`Edge ${edge.id} references non-existent target node: ${edge.to}`);
  }

  // Rule 6: Gateways
  for (const node of definition.nodes) {
    if (node.type === 'gateway') {
      const gw: any = node;
      const inferredStrategy: string = gw.strategy
        ? gw.strategy
        : (gw.condition ? 'conditional' : 'exclusive');
      if (!gw.strategy) gw.strategy = inferredStrategy;

      if (inferredStrategy === 'conditional') {
        if (!gw.condition || typeof gw.condition !== 'string' || gw.condition.trim() === '') {
          errors.push(`Gateway "${node.label || node.id}" (conditional) must have a condition`);
        } else {
          try { JSON.parse(gw.condition); }
          catch { errors.push(`Gateway "${node.label || node.id}" condition is not valid JSON`); }
        }
      } else if (inferredStrategy === 'parallel') {
        const outgoing = definition.edges.filter(e => e.from === node.id);
        if (outgoing.length < 2) {
          warnings.push(`Parallel gateway "${node.label || node.id}" should have at least 2 outgoing branches`);
        }
      }
    }
  }

  // Rule 7: Joins base
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
          break;
        default:
          errors.push(`Join "${node.label || node.id}" has invalid mode "${mode}"`);
      }
    }
  }

  // Rule 8: HumanTask warning
  for (const node of definition.nodes) {
    if (node.type === 'humanTask') {
      const ht: any = node;
      if (!ht.assigneeRoles || ht.assigneeRoles.length === 0) {
        warnings.push(`HumanTask "${node.label || node.id}" should have assignee roles`);
      }
    }
  }

  // Rule 9: Parallel / Join structural
  applyParallelJoinStructuralValidation(definition, errors, warnings);

  return { isValid: errors.length === 0, errors, warnings };
}

function findReachableNodes(def: DslDefinition, startId: string): Set<string> {
  const seen = new Set<string>();
  const stack = [startId];
  while (stack.length) {
    const id = stack.pop()!;
    if (seen.has(id)) continue;
    seen.add(id);
    for (const e of def.edges.filter(x => x.from === id)) stack.push(e.to);
  }
  return seen;
}

function applyParallelJoinStructuralValidation(def: DslDefinition, errors: string[], warnings: string[]) {
  const nodeMap: Record<string, DslNode> = {};
  def.nodes.forEach(n => { nodeMap[n.id] = n; });

  const edgesFrom: Record<string, string[]> = {};
  const edgesTo: Record<string, string[]> = {};
  for (const e of def.edges) {
    (edgesFrom[e.from] ||= []).push(e.to);
    (edgesTo[e.to] ||= []).push(e.from);
  }

  // Join in-degree
  for (const j of def.nodes.filter(n => n.type === 'join')) {
    const incoming = edgesTo[j.id] || [];
    if (incoming.length < 2) errors.push(`Join "${j.label || j.id}" must have at least 2 incoming edges`);
  }

  const MAX_SCAN = 400;
  function forwardScan(start: string) {
    const q: { id: string; d: number }[] = [{ id: start, d: 0 }];
    const visited = new Set<string>();
    const firstJoins: Record<string, number> = {};
    while (q.length) {
      const { id, d } = q.shift()!;
      if (visited.has(id) || d > MAX_SCAN) continue;
      visited.add(id);
      const n = nodeMap[id];
      if (n?.type === 'join') {
        if (!(id in firstJoins)) firstJoins[id] = d;
        continue;
      }
      for (const nxt of edgesFrom[id] || []) q.push({ id: nxt, d: d + 1 });
    }
    return { joins: Object.keys(firstJoins) };
  }

  for (const gw of def.nodes.filter(n => n.type === 'gateway')) {
    const anyGw: any = gw;
    const strat = anyGw.strategy || (anyGw.condition ? 'conditional' : 'exclusive');
    if (strat !== 'parallel') continue;
    const branches = edgesFrom[gw.id] || [];
    if (branches.length < 2) continue;

    const branchSets: Record<string, Set<string>> = {};
    branches.forEach(b => {
      const { joins } = forwardScan(b);
      branchSets[b] = new Set(joins);
    });

    const all = new Set<string>();
    Object.values(branchSets).forEach(s => s.forEach(j => all.add(j)));

    if (all.size === 0) {
      errors.push(`Parallel "${gw.label || gw.id}" has no downstream join reconverging its branches`);
      continue;
    }

    let intersection: Set<string> | null = null;
    for (const b of branches) {
      const current = branchSets[b]; // Set<string>
      if (intersection == null) {
        intersection = new Set<string>(current);
      } else {
        const narrowed = intersection as Set<string>;
        intersection = new Set<string>([...narrowed].filter(x => current.has(x)));
      }
    }

    if (!intersection || intersection.size === 0) {
      warnings.push(`Parallel "${gw.label || gw.id}" branches partially converge (no single join reached by all branches)`);
    } else if (intersection.size > 1) {
      warnings.push(`Parallel "${gw.label || gw.id}" branches have multiple candidate convergence joins: ${[...intersection].join(', ')}`);
    }

    for (const b of branches) {
      if (branchSets[b].size === 0) {
        warnings.push(`Parallel "${gw.label || gw.id}" branch starting at "${b}" never reaches a join`);
      }
    }

    const subset = [...all].filter(j =>
      branches.some(br => branchSets[br].has(j)) &&
      branches.some(br => !branchSets[br].has(j))
    );
    if (subset.length) {
      warnings.push(`Parallel "${gw.label || gw.id}" has subset convergence join(s): ${subset.join(', ')}`);
    }
  }
}

export function validateNode(node: DslNode): ValidationResult {
  const errors: string[] = [];
  const warnings: string[] = [];

  if (!node.id) errors.push('Node must have an ID');
  if (!node.type) errors.push('Node must have a type');

  switch (node.type) {
    case 'gateway': {
      const gw: any = node;
      const strategy = gw.strategy || (gw.condition ? 'conditional' : 'exclusive');
      if (strategy === 'conditional') {
        if (!gw.condition || typeof gw.condition !== 'string' || !gw.condition.trim()) {
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
      const hasRel = (t.delayMinutes && t.delayMinutes > 0) || (t.delaySeconds && t.delaySeconds > 0);
      const hasAbs = !!t.untilIso;
      if (!hasRel && !hasAbs) errors.push('Timer node must specify a relative delay or an absolute untilIso');
      if (hasRel && hasAbs) errors.push('Timer node cannot have both relative delay and absolute untilIso');
      if (hasRel) {
        if (t.delayMinutes && t.delayMinutes < 0) errors.push('Timer delayMinutes must be >= 0');
        if (t.delaySeconds && t.delaySeconds < 0) errors.push('Timer delaySeconds must be >= 0');
        if ((t.delayMinutes || 0) === 0 && (t.delaySeconds || 0) === 0) {
          errors.push('Timer relative delay must be > 0 total');
        }
      }
      if (hasAbs) {
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

  return { isValid: errors.length === 0, errors, warnings };
}
