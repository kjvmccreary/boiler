import { Edge } from 'reactflow';
import type { DslDefinition, DslNode, DslEdge } from './dsl.types';

function extractBranch(edge: Edge): 'true' | 'false' | undefined {
  const sh = edge.sourceHandle;
  if (sh === 'true' || sh === 'false') return sh;
  const d = (edge as any).data?.fromHandle;
  if (d === 'true' || d === 'false') return d;
  const lbl = typeof edge.label === 'string' ? edge.label.toLowerCase() : undefined;
  if (lbl === 'true' || lbl === 'false') return lbl as 'true' | 'false';
  return undefined;
}

export function serializeToDsl(
  nodes: any[],
  edges: Edge[],
  key: string,
  version?: number
): DslDefinition {
  const dslNodes: DslNode[] = nodes.map(n => {
    const base = n.data as DslNode;
    return {
      ...base,
      id: base.id || n.id,
      x: n.position.x,
      y: n.position.y
    };
  });

  const dslEdges: DslEdge[] = edges.map(e => {
    const branch = extractBranch(e);
    return {
      id: e.id,
      from: e.source,
      to: e.target,
      label: branch || (typeof e.label === 'string' ? e.label : undefined),
      fromHandle: branch || undefined
    };
  });

  // Deduplicate true/false per source
  const byFrom: Record<string, DslEdge[]> = {};
  for (const e of dslEdges) (byFrom[e.from] ||= []).push(e);

  const filtered: DslEdge[] = [];
  for (const list of Object.values(byFrom)) {
    let trueSeen = false;
    let falseSeen = false;
    for (const e of list) {
      if (e.label === 'true') {
        if (trueSeen) continue;
        trueSeen = true;
      } else if (e.label === 'false') {
        if (falseSeen) continue;
        falseSeen = falseSeen ? falseSeen : true; // explicit
      }
      filtered.push(e);
    }
  }

  // eslint-disable-next-line no-console
  console.debug('[DSL][serialize] edges', filtered.map(e => ({
    id: e.id, from: e.from, to: e.to, label: e.label, fromHandle: (e as DslEdge).fromHandle
  })));

  return { key, version, nodes: dslNodes, edges: filtered };
}

export function deserializeFromDsl(def: DslDefinition): {
  nodes: any[];
  edges: Edge[];
} {
  const flowNodes = def.nodes.map(n => {
    // Migration note: ensure gateway strategy defaults (not mutating original object)
    if (n.type === 'gateway') {
      const g: any = { ...n };
      if (!g.strategy) {
        // Heuristic: legacy definitions with condition => treat as conditional, else exclusive
        g.strategy = g.condition ? 'conditional' : 'exclusive';
      }
      return {
        id: g.id,
        type: g.type,
        position: { x: g.x, y: g.y },
        data: g
      };
    }
    return {
      id: n.id,
      type: n.type,
      position: { x: n.x, y: n.y },
      data: { ...n }
    };
  });

  const flowEdges: Edge[] = def.edges.map(e => {
    const branch = e.label === 'true' || e.label === 'false'
      ? (e.label as 'true' | 'false')
      : e.fromHandle === 'true' || e.fromHandle === 'false'
        ? (e.fromHandle as 'true' | 'false')
        : undefined;

    return {
      id: e.id,
      source: e.from,
      target: e.to,
      sourceHandle: branch,
      label: branch,
      data: branch ? { fromHandle: branch } : undefined,
      type: 'default'
    } as Edge;
  });

  // eslint-disable-next-line no-console
  console.debug('[DSL][deserialize] edges',
    flowEdges.map(e => ({
      id: e.id, sourceHandle: e.sourceHandle, label: e.label
    }))
  );

  return { nodes: flowNodes, edges: flowEdges };
}
