import { nanoid } from 'nanoid';
import {
  EditorWorkflowDefinition,
  EditorWorkflowEdge,
  EditorWorkflowNode,
  RFEdgeData,
  RFNodeData
} from '../../../types/workflow';
import { Edge, Node } from 'reactflow';

function normalize(v?: string | null): string | undefined {
  if (!v) return undefined;
  const l = v.trim().toLowerCase();
  if (l === 'yes') return 'true';
  if (l === 'no') return 'false';
  if (l === 'default' || l === 'else') return 'else';
  if (['true','false'].includes(l)) return l;
  return undefined;
}

function infer(edge: EditorWorkflowEdge): string | undefined {
  return normalize(edge.fromHandle) ||
         normalize(edge.label) ||
         (() => {
           const id = edge.id.toLowerCase();
           if (id.includes('true')) return 'true';
           if (id.includes('false')) return 'false';
           if (id.includes('else')) return 'else';
           return undefined;
         })();
}

function binaryCollapse(edges: EditorWorkflowEdge[]) {
  const groups = edges.reduce<Record<string, EditorWorkflowEdge[]>>((acc, e) => {
    (acc[e.from] ||= []).push(e);
    return acc;
  }, {});
  for (const list of Object.values(groups)) {
    const trues  = list.filter(e => infer(e) === 'true');
    const falses = list.filter(e => infer(e) === 'false');
    const elses  = list.filter(e => infer(e) === 'else');

    if (elses.length) {
      if (!trues.length && !falses.length) {
        elses.forEach(e => { e.fromHandle = 'false'; e.label = 'false'; });
      } else {
        const ids = new Set(elses.map(e => e.id));
        for (let i = edges.length - 1; i >= 0; i--) {
          if (ids.has(edges[i].id)) edges.splice(i, 1);
        }
      }
    }
    // enforce single true/false
    let seenTrue = false;
    let seenFalse = false;
    for (const e of list) {
      const lg = infer(e);
      if (lg === 'true') {
        if (seenTrue) {
          const idx = edges.findIndex(x => x.id === e.id);
          if (idx >= 0) edges.splice(idx, 1);
        }
        seenTrue = true;
      } else if (lg === 'false') {
        if (seenFalse) {
          const idx = edges.findIndex(x => x.id === e.id);
          if (idx >= 0) edges.splice(idx, 1);
        }
        seenFalse = true;
      }
    }
  }
}

export function toGraph(def: EditorWorkflowDefinition) {
  const working = def.edges.map(e => ({ ...e }));
  binaryCollapse(working);

  const nodes: Node<RFNodeData>[] = def.nodes.map(n => ({
    id: n.id,
    // Force the runtime node type to the new custom type to avoid legacy component
    type: n.type === 'gateway' ? 'wfGateway' : (n.type === 'wfGateway' ? 'wfGateway' : n.type),
    position: { x: (n as any).x ?? 0, y: (n as any).y ?? 0 },
    data: {
      label: n.label,
      dueInMinutes: (n as any).dueInMinutes,
      assigneeRoles: (n as any).assigneeRoles,
      condition: (n as any).condition,
      action: (n as any).action
    }
  }));

  const edges: Edge<RFEdgeData>[] = working.map(e => {
    const logical = infer(e);
    const physical = logical === 'true'
      ? 'out_true'
      : logical === 'false'
      ? 'out_false'
      : undefined;

    return {
      id: e.id || nanoid(),
      source: e.from,
      target: e.to,
      sourceHandle: physical,
      data: { branch: logical },
      label: logical,
      type: 'straight',
      style: {
        stroke: logical === 'true' ? '#16a34a'
             : logical === 'false' ? '#dc2626'
             : '#64748b',
        strokeWidth: 2
      }
    } as Edge<RFEdgeData>;
  });

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
    // Persist original semantic type as "gateway" (optional) or keep 'wfGateway'
    type: n.type === 'wfGateway' ? 'gateway' : (n.type ?? 'humanTask'),
    label: n.data?.label,
    x: n.position.x,
    y: n.position.y,
    dueInMinutes: n.data?.dueInMinutes,
    assigneeRoles: n.data?.assigneeRoles,
    condition: n.data?.condition,
    action: n.data?.action
  }));

  const defEdges: EditorWorkflowEdge[] = edges.map(e => {
    const logical =
      (e.data as any)?.branch ||
      (typeof e.label === 'string' ? normalize(e.label) : undefined) ||
      (e.sourceHandle === 'out_true' ? 'true'
        : e.sourceHandle === 'out_false' ? 'false'
        : undefined);
    return {
      id: e.id,
      from: e.source,
      to: e.target,
      fromHandle: logical ?? null,
      label: logical ?? null
    };
  });

  return { key, nodes: defNodes, edges: defEdges, ...extras };
}
