import { Node, Edge } from 'reactflow';
import type { DslDefinition, DslNode, DslEdge } from './dsl.types';

export function serializeToDsl(
  nodes: Node[],
  edges: Edge[],
  key: string,
  version?: number
): DslDefinition {
  const dslNodes: DslNode[] = nodes.map(node => ({
    ...node.data,
    x: node.position.x,
    y: node.position.y
  }));

  const dslEdges: DslEdge[] = edges.map(edge => ({
    id: edge.id,
    from: edge.source,
    to: edge.target,
    label: edge.label as string
  }));

  return {
    key,
    version,
    nodes: dslNodes,
    edges: dslEdges
  };
}

export function deserializeFromDsl(definition: DslDefinition): { nodes: Node[]; edges: Edge[] } {
  const nodes: Node[] = definition.nodes.map(dslNode => ({
    id: dslNode.id,
    type: dslNode.type,
    position: { x: dslNode.x, y: dslNode.y },
    data: dslNode
  }));

  const edges: Edge[] = definition.edges.map(dslEdge => ({
    id: dslEdge.id,
    source: dslEdge.from,
    target: dslEdge.to,
    label: dslEdge.label
  }));

  return { nodes, edges };
}
