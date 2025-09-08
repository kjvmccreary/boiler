import type { DslDefinition, DslNode, ValidationResult } from './dsl.types';

/**
 * Validates a workflow definition according to current builder rules (extended for gateway strategies).
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
      // Migration heuristic:
      const inferredStrategy: string = gw.strategy
        ? gw.strategy
        : (gw.condition ? 'conditional' : 'exclusive');

      if (!gw.strategy) {
        // Allow silent inference; could add warning later if desired
        gw.strategy = inferredStrategy;
      }

      if (inferredStrategy === 'conditional') {
        if (!gw.condition || typeof gw.condition !== 'string' || gw.condition.trim() === '') {
          errors.push(`Gateway node "${node.label || node.id}" must have a condition`);
        }
      } else {
        // exclusive / parallel: condition optional; ignore if empty
      }
    }
  }

  // Rule 7: HumanTask advisory (warning)
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
 * Validates a single node (strategy-aware).
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
        }
      }
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
