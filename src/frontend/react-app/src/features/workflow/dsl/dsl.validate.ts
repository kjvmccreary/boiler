import type { DslDefinition, DslNode, ValidationResult } from './dsl.types'; // ✅ ADD: DslNode import

/**
 * Validates a workflow definition according to MVP rules
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

  // Rule 3: All nodes must be reachable from Start
  if (startNodes.length === 1) {
    const reachableNodes = findReachableNodes(definition, startNodes[0].id);
    const unreachableNodes = definition.nodes.filter(n => 
      n.type !== 'start' && !reachableNodes.has(n.id)
    );
    
    if (unreachableNodes.length > 0) {
      errors.push(`Unreachable nodes: ${unreachableNodes.map(n => n.label || n.id).join(', ')}`);
    }
  }

  // Rule 4: All edges must connect valid nodes
  for (const edge of definition.edges) {
    const fromNode = definition.nodes.find(n => n.id === edge.from);
    const toNode = definition.nodes.find(n => n.id === edge.to);
    
    if (!fromNode) {
      errors.push(`Edge ${edge.id} references non-existent source node: ${edge.from}`);
    }
    if (!toNode) {
      errors.push(`Edge ${edge.id} references non-existent target node: ${edge.to}`);
    }
  }

  // Rule 5: Gateway nodes must have condition
  for (const node of definition.nodes) {
    if (node.type === 'gateway') {
      const gatewayNode = node as any;
      if (!gatewayNode.condition || gatewayNode.condition.trim() === '') {
        errors.push(`Gateway node "${node.label || node.id}" must have a condition`);
      }
    }
  }

  // Rule 6: HumanTask nodes should have assignee roles (warning)
  for (const node of definition.nodes) {
    if (node.type === 'humanTask') {
      const humanTaskNode = node as any;
      if (!humanTaskNode.assigneeRoles || humanTaskNode.assigneeRoles.length === 0) {
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
    
    // Find all edges from this node
    const outgoingEdges = definition.edges.filter(e => e.from === currentId);
    for (const edge of outgoingEdges) {
      if (!reachable.has(edge.to)) {
        toVisit.push(edge.to);
      }
    }
  }
  
  return reachable;
}

/**
 * Validates a single node
 */
export function validateNode(node: DslNode): ValidationResult {
  const errors: string[] = [];
  const warnings: string[] = [];

  // Basic validation
  if (!node.id) {
    errors.push('Node must have an ID');
  }
  
  if (!node.type) {
    errors.push('Node must have a type');
  }

  // Type-specific validation
  switch (node.type) {
    case 'gateway':
      const gatewayNode = node as any;
      if (!gatewayNode.condition) {
        errors.push('Gateway node must have a condition');
      }
      break;
      
    case 'timer':
      const timerNode = node as any;
      if (!timerNode.delayMinutes && !timerNode.untilIso) {
        errors.push('Timer node must have either delayMinutes or untilIso');
      }
      break;
      
    case 'humanTask':
      const humanTaskNode = node as any;
      if (!humanTaskNode.assigneeRoles || humanTaskNode.assigneeRoles.length === 0) {
        warnings.push('HumanTask should have assignee roles');
      }
      break;
  }

  return {
    isValid: errors.length === 0,
    errors,
    warnings
  };
}
