import type { Node, Edge } from 'reactflow';
import type { DslDefinition, DslNode, DslEdge } from './dsl.types';

/**
 * Converts ReactFlow nodes and edges to DSL format
 */
export function serializeToDsl(
  nodes: Node[],
  edges: Edge[],
  workflowKey: string,
  version?: number
): DslDefinition {
  const dslNodes: DslNode[] = nodes.map(node => ({
    id: node.id,
    type: node.type as any, // We'll ensure type safety in the UI
    label: node.data?.label || '',
    x: node.position.x,
    y: node.position.y,
    // Spread any additional type-specific data
    ...node.data
  }));

  const dslEdges: DslEdge[] = edges.map(edge => ({
    id: edge.id,
    from: edge.source,
    to: edge.target,
    label: edge.label as string
  }));

  return {
    key: workflowKey,
    version,
    nodes: dslNodes,
    edges: dslEdges
  };
}

/**
 * Converts DSL format to ReactFlow nodes and edges
 */
export function deserializeFromDsl(dsl: DslDefinition): { nodes: Node[]; edges: Edge[] } {
  const nodes: Node[] = dsl.nodes.map(dslNode => ({
    id: dslNode.id,
    type: dslNode.type,
    position: { x: dslNode.x, y: dslNode.y },
    data: {
      label: dslNode.label || getDefaultLabel(dslNode.type),
      ...getNodeTypeData(dslNode)
    }
  }));

  const edges: Edge[] = dsl.edges.map(dslEdge => ({
    id: dslEdge.id,
    source: dslEdge.from,
    target: dslEdge.to,
    label: dslEdge.label,
    type: 'default'
  }));

  return { nodes, edges };
}

/**
 * Get default label for node type
 */
function getDefaultLabel(nodeType: string): string {
  switch (nodeType) {
    case 'start': return 'Start';
    case 'end': return 'End';
    case 'humanTask': return 'Human Task';
    case 'automatic': return 'Automatic';
    case 'gateway': return 'Gateway';
    case 'timer': return 'Timer';
    default: return 'Node';
  }
}

/**
 * Extract type-specific data from DSL node
 */
function getNodeTypeData(dslNode: DslNode): Record<string, any> {
  const base = { 
    label: dslNode.label || getDefaultLabel(dslNode.type)
  };

  switch (dslNode.type) {
    case 'humanTask':
      const humanTask = dslNode as any;
      return {
        ...base,
        assigneeRoles: humanTask.assigneeRoles || [],
        dueInMinutes: humanTask.dueInMinutes,
        formSchema: humanTask.formSchema
      };
      
    case 'automatic':
      const automatic = dslNode as any;
      return {
        ...base,
        action: automatic.action || { kind: 'noop' }
      };
      
    case 'gateway':
      const gateway = dslNode as any;
      return {
        ...base,
        condition: gateway.condition || ''
      };
      
    case 'timer':
      const timer = dslNode as any;
      return {
        ...base,
        delayMinutes: timer.delayMinutes,
        untilIso: timer.untilIso
      };
      
    default:
      return base;
  }
}
