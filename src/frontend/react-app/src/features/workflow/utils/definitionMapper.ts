import { nanoid } from 'nanoid';
import {
  EditorWorkflowDefinition,
  EditorWorkflowEdge,
  EditorWorkflowNode,
  RFEdgeData,
  RFNodeData
} from '../../../types/workflow';
import { Edge, Node } from 'reactflow';

function rawInfer(edge: EditorWorkflowEdge): string | null {
  if (edge.label) return edge.label;
  if (edge.fromHandle) return edge.fromHandle;
  const idLower = edge.id.toLowerCase();
  if (idLower.includes('true')) return 'true';
  if (idLower.includes('false')) return 'false';
  if (idLower.includes('else')) return 'else';
  return null;
}

function assignElsePerGateway(edges: EditorWorkflowEdge[]) {
  const bySource = edges.reduce<Record<string, EditorWorkflowEdge[]>>((acc, e) => {
    acc[e.from] = acc[e.from] || [];
    acc[e.from].push(e);
    return acc;
  }, {});
  for (const group of Object.values(bySource)) {
    if (group.length < 2) continue;
    const labels = group.map(e => e.fromHandle || e.label);
    const hasTrue = labels.includes('true');
    const hasFalse = labels.includes('false');
    const hasElse = labels.includes('else');
    const unlabeled = group.filter(e => !e.fromHandle && !e.label);
    if ((hasTrue || hasFalse) && !hasElse && unlabeled.length === 1) {
      unlabeled[0].fromHandle = 'else';
      unlabeled[0].label = 'else';
    }
  }
}

export function toGraph(def: EditorWorkflowDefinition) {
  const nodes: Node<RFNodeData>[] = def.nodes.map(n => ({
    id: n.id,
    type: n.type === 'gateway' ? 'gateway' : n.type,
    position: { x: n.x ?? 0, y: n.y ?? 0 },
    data: {
      label: n.label,
      dueInMinutes: (n as any).dueInMinutes,
      assigneeRoles: (n as any).assigneeRoles,
      condition: (n as any).condition,
      action: (n as any).action
    }
  }));

  const working = def.edges.map(e => ({ ...e }));
  assignElsePerGateway(working);

  const edges: Edge<RFEdgeData>[] = working.map(e => {
    const label = e.label ?? rawInfer(e);
    const handle = e.fromHandle ?? label ?? undefined;
    const edge: Edge<RFEdgeData> = {
      id: e.id || nanoid(),
      source: e.from,
      target: e.to,
      sourceHandle: handle,
      data: { fromHandle: handle },
      label: label || undefined,
      type: 'default'
    };
    return edge;
  });

  // Debug log
  // eslint-disable-next-line no-console
  console.debug('[WF][toGraph] edges hydrated',
    edges.map(e => ({ id: e.id, source: e.source, target: e.target, sourceHandle: e.sourceHandle, label: e.label }))
  );

  return { nodes, edges };
}

export function toDefinition(
  key: string,
  nodes: Node<RFNodeData>[],
  edges: Edge<RFEdgeData>[],
  extras: Partial<EditorWorkflowDefinition> = {}
): EditorWorkflowDefinition {
  const defNodes: EditorWorkflowNode[] = nodes.map(n => ({
    id: n.id,
    type: n.type ?? 'humanTask',
    label: n.data?.label,
    x: n.position.x,
    y: n.position.y,
    dueInMinutes: n.data?.dueInMinutes,
    assigneeRoles: n.data?.assigneeRoles,
    condition: n.data?.condition,
    action: n.data?.action
  }));

  const defEdges: EditorWorkflowEdge[] = edges.map(e => {
    const fromHandle =
      e.sourceHandle ||
      e.data?.fromHandle ||
      (e.label && ['true', 'false', 'else'].includes(e.label) ? e.label : undefined);

    return {
      id: e.id,
      from: e.source,
      to: e.target,
      fromHandle,
      label: fromHandle
    };
  });

  assignElsePerGateway(defEdges);

  return {
    key,
    nodes: defNodes,
    edges: defEdges,
    ...extras
  };
}
