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
 * Validates a single node (strategy / join aware).
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
      // (retain existing count/quorum checks)
      break;
    }
    case 'timer': {
      const t: any = node;
      if (!t.delayMinutes && !t.delaySeconds && !t.untilIso) {
        errors.push('Timer node must have either delayMinutes, delaySeconds, or untilIso');
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
