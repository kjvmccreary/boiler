import { nanoid } from 'nanoid';
import {
  EditorWorkflowDefinition,
  EditorWorkflowEdge,
  EditorWorkflowNode,
  RFEdgeData,
  RFNodeData
} from '../../../types/workflow';
import { Edge as RFEdge, Node } from 'reactflow';

// Helper to coerce editor node type to a safe string
const coerceNodeType = (t: any): string =>
  typeof t === 'string' && t.length ? t : 'humanTask';

function normalize(v?: string | null): string | undefined {
  if (!v) return undefined;
  const l = v.trim().toLowerCase();
  if (l === 'yes') return 'true';
  if (l === 'no') return 'false';
  if (l === 'default' || l === 'else') return 'else';
  if (['true', 'false'].includes(l)) return l;
  return undefined;
}

function infer(edge: EditorWorkflowEdge): string | undefined {
  return (
    normalize(edge.fromHandle || undefined) ||
    normalize(edge.label || undefined) ||
    (() => {
      const id = edge.id.toLowerCase();
      if (id.includes('true')) return 'true';
      if (id.includes('false')) return 'false';
      if (id.includes('else')) return 'else';
      return undefined;
    })()
  );
}

function binaryCollapse(edges: EditorWorkflowEdge[]) {
  const groups = edges.reduce<Record<string, EditorWorkflowEdge[]>>((acc, e) => {
    (acc[e.from] ||= []).push(e);
    return acc;
  }, {});
  for (const list of Object.values(groups)) {
    const trues = list.filter(e => infer(e) === 'true');
    const falses = list.filter(e => infer(e) === 'false');
    const elses = list.filter(e => infer(e) === 'else');

    if (elses.length) {
      if (!trues.length && !falses.length) {
        elses.forEach(e => {
          e.fromHandle = 'false';
          e.label = 'false';
        });
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
    type: coerceNodeType(n.type),
    position: { x: (n as any).x ?? 0, y: (n as any).y ?? 0 },
    data: {
      nodeId: n.id,
      type: coerceNodeType(n.type),
      label: n.label,
      dueInMinutes: (n as any).dueInMinutes,
      assigneeRoles: (n as any).assigneeRoles,
      condition: (n as any).condition,
      action: (n as any).action
    }
  }));

  const edges: RFEdge<RFEdgeData>[] = working.map(e => {
    const logical = infer(e);
    const physical =
      logical === 'true'
        ? 'out_true'
        : logical === 'false'
          ? 'out_false'
          : undefined;

    return {
      id: e.id || nanoid(),
      source: e.from,
      target: e.to,
      sourceHandle: physical,
      data: {
        branch: (e as any).fromHandle,
        from: e.from,
        to: e.to,
        edgeId: e.id
      },
      label: logical,
      type: 'straight',
      style: {
        stroke:
          logical === 'true'
            ? '#16a34a'
            : logical === 'false'
              ? '#dc2626'
              : '#64748b',
        strokeWidth: 2
      }
    } as RFEdge<RFEdgeData>;
  });

  return { nodes, edges };
}

export function toDefinition(
  key: string,
  nodes: Node<RFNodeData>[],
  edges: RFEdge<RFEdgeData>[],
  extras: Partial<EditorWorkflowDefinition> = {}
): EditorWorkflowDefinition {
  const defNodes: EditorWorkflowNode[] = nodes.map(n => ({
    id: n.id,
    type: coerceNodeType(n.data?.type ?? n.type),
    label: (n.data as any)?.label ?? (n as any).label,
    x: typeof n.position?.x === 'number' ? n.position.x : 0,
    y: typeof n.position?.y === 'number' ? n.position.y : 0,
    dueInMinutes: (n.data as any)?.dueInMinutes as number | undefined,
    assigneeRoles: (n.data as any)?.assigneeRoles as string[] | undefined,
    condition: (n.data as any)?.condition as string | undefined,
    action: (n.data as any)?.action
  }));

  const defEdges: EditorWorkflowEdge[] = edges.map(e => {
    const logical =
      (e.data as any)?.fromHandle ||
      (typeof e.label === 'string' ? normalize(e.label) : undefined) ||
      (e.sourceHandle ? normalize(e.sourceHandle) : undefined);

    const finalLogical =
      logical === 'true' || logical === 'false'
        ? logical
        : (e.sourceHandle === 'true' || e.sourceHandle === 'false'
          ? (e.sourceHandle as 'true' | 'false')
          : undefined);

    return {
      id: e.id,
      from: e.source,
      to: e.target,
      fromHandle: finalLogical ?? null,
      label: finalLogical ?? null
    };
  });

  return {
    key,
    nodes: defNodes,
    edges: defEdges,
    ...extras
  } as EditorWorkflowDefinition;
}
