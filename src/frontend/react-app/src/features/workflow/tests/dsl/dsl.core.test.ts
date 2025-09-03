import { describe, it, expect } from 'vitest';
import { serializeToDsl, deserializeFromDsl } from '@/features/workflow/dsl/dsl.serialize';
import { validateDefinition, validateNode } from '@/features/workflow/dsl/dsl.validate';
import type {
  DslDefinition,
  DslNode,
  TimerNode,
  GatewayNode,
  HumanTaskNode
} from '@/features/workflow/dsl/dsl.types';
import type { Edge } from 'reactflow';

// Helper to build React Flow style nodes
function rfNode(n: DslNode) {
  return {
    id: n.id,
    type: n.type,
    position: { x: n.x, y: n.y },
    data: { ...n }
  };
}

// Helper to create minimal Edge objects
function edge(params: Partial<Edge> & { id: string; source: string; target: string }): Edge {
  return {
    id: params.id,
    source: params.source,
    target: params.target,
    sourceHandle: params.sourceHandle,
    label: params.label,
    data: params.data,
    type: params.type || 'default'
  } as Edge;
}

describe('DSL serialize/deserialize core', () => {

  it('deduplicates true/false branch edges per source keeping first occurrences', () => {
    const nodes: DslNode[] = [
      { id: 'g1', type: 'gateway', label: 'G1', x: 10, y: 10, condition: 'x>1' } as GatewayNode,
      { id: 'a', type: 'automatic', label: 'A', x: 100, y: 10 },
      { id: 'b', type: 'automatic', label: 'B', x: 150, y: 10 },
      { id: 'c', type: 'automatic', label: 'C', x: 200, y: 10 },
      { id: 'd', type: 'automatic', label: 'D', x: 250, y: 10 }
    ];

    const edges: Edge[] = [
      edge({ id: 'e1', source: 'g1', target: 'a', sourceHandle: 'true', label: 'true' }),
      edge({ id: 'e2', source: 'g1', target: 'b', sourceHandle: 'true', label: 'true' }), // dup true
      edge({ id: 'e3', source: 'g1', target: 'c', sourceHandle: 'false', label: 'false' }),
      edge({ id: 'e4', source: 'g1', target: 'd', sourceHandle: 'false', label: 'false' }), // dup false
      edge({ id: 'e5', source: 'g1', target: 'd', label: 'else' }) // else not deduped
    ];

    const dsl = serializeToDsl(nodes.map(rfNode), edges, 'wf1', 1);

    const gatewayEdges = dsl.edges.filter(e => e.from === 'g1');
    // Expect: first true, first false, else => 3
    expect(gatewayEdges.length).toBe(3);
    const labels = gatewayEdges.map(e => e.label).sort();
    expect(labels).toEqual(['else', 'false', 'true']);
    // Ensure kept edges are the first occurrences (e1, e3)
    expect(gatewayEdges.find(e => e.label === 'true')?.id).toBe('e1');
    expect(gatewayEdges.find(e => e.label === 'false')?.id).toBe('e3');
  });

  it('round-trips nodes & edges (excluding deduped duplicates)', () => {
    const definition: DslDefinition = {
      key: 'wf-round',
      version: 2,
      nodes: [
        { id: 'start', type: 'start', x: 0, y: 0, label: 'Start' },
        {
          id: 't1',
          type: 'timer',
          x: 120,
          y: 0,
          label: 'Wait',
          delayMinutes: 5,
          delaySeconds: 30,
          untilIso: undefined
        },
        {
          id: 'g1',
          type: 'gateway',
          x: 240,
          y: 0,
          label: 'Check',
          condition: '{ ">" : [ { "var": "value" }, 10 ] }'
        } as GatewayNode,
        {
          id: 'h1',
          type: 'humanTask',
          x: 360,
          y: 0,
          label: 'Approve',
          assigneeRoles: ['manager']
        } as HumanTaskNode,
        { id: 'end', type: 'end', x: 480, y: 0, label: 'Done' }
      ],
      edges: [
        { id: 's_to_t', from: 'start', to: 't1' },
        { id: 't_to_g', from: 't1', to: 'g1' },
        { id: 'g_true', from: 'g1', to: 'h1', label: 'true' },
        { id: 'g_false', from: 'g1', to: 'end', label: 'false' }
      ]
    };

    // Build react-flow nodes/edges (simulate UI)
    const rfNodes = definition.nodes.map(rfNode);
    const rfEdges: Edge[] = definition.edges.map(e =>
      edge({
        id: e.id,
        source: e.from,
        target: e.to,
        label: e.label,
        sourceHandle: e.label === 'true' || e.label === 'false' ? e.label : undefined
      })
    );

    const serialized = serializeToDsl(rfNodes, rfEdges, definition.key, definition.version);
    // Compare core sets (sort for stability)
    expect(serialized.key).toBe(definition.key);
    expect(serialized.version).toBe(definition.version);
    expect(serialized.nodes.map(n => n.id).sort()).toEqual(definition.nodes.map(n => n.id).sort());
    expect(serialized.edges.map(e => e.id).sort()).toEqual(definition.edges.map(e => e.id).sort());

    // Timer fields preserved
    const originalTimer = definition.nodes.find(n => n.id === 't1') as TimerNode;
    const serializedTimer = serialized.nodes.find(n => n.id === 't1') as TimerNode;
    expect(serializedTimer.delayMinutes).toBe(originalTimer.delayMinutes);
    expect(serializedTimer.delaySeconds).toBe(originalTimer.delaySeconds);

    // Now deserialize and confirm branch handles are restored
    const deserialized = deserializeFromDsl(serialized);
    const gTrue = deserialized.edges.find(e => e.id === 'g_true');
    const gFalse = deserialized.edges.find(e => e.id === 'g_false');
    expect(gTrue?.sourceHandle).toBe('true');
    expect(gFalse?.sourceHandle).toBe('false');
  });

  it('preserves arbitrary non-branch edge labels', () => {
    const nodes: DslNode[] = [
      { id: 'n1', type: 'start', x: 0, y: 0 },
      { id: 'n2', type: 'automatic', x: 100, y: 0 },
      { id: 'n3', type: 'end', x: 200, y: 0 }
    ];
    const edges: Edge[] = [
      edge({ id: 'e1', source: 'n1', target: 'n2', label: 'INIT' }),
      edge({ id: 'e2', source: 'n2', target: 'n3', label: 'FINALIZE' })
    ];
    const dsl = serializeToDsl(nodes.map(rfNode), edges, 'wf-labels');
    const labels = dsl.edges.map(e => e.label);
    expect(labels).toContain('INIT');
    expect(labels).toContain('FINALIZE');
  });
});

describe('DSL validateDefinition invalid fixtures', () => {
  const baseNodes: DslNode[] = [
    { id: 'start', type: 'start', x: 0, y: 0 },
    { id: 'end', type: 'end', x: 200, y: 0 }
  ];

  it('flags multiple start nodes', () => {
    const def: DslDefinition = {
      key: 'multi-start',
      nodes: [...baseNodes, { id: 'start2', type: 'start', x: 10, y: 100 }],
      edges: []
    };
    const res = validateDefinition(def);
    expect(res.errors).toContain('Workflow can only have one Start node');
  });

  it('flags missing end node', () => {
    const def: DslDefinition = {
      key: 'no-end',
      nodes: [{ id: 'start', type: 'start', x: 0, y: 0 }],
      edges: []
    };
    const res = validateDefinition(def);
    expect(res.errors).toContain('Workflow must have at least one End node');
  });

  it('flags unreachable nodes', () => {
    const def: DslDefinition = {
      key: 'unreachable',
      nodes: [...baseNodes, { id: 'island', type: 'automatic', x: 400, y: 400 }],
      edges: [{ id: 'e1', from: 'start', to: 'end' }]
    };
    const res = validateDefinition(def);
    expect(res.errors.some(e => e.includes('Unreachable nodes'))).toBe(true);
  });

  it('flags edge referencing unknown node', () => {
    const def: DslDefinition = {
      key: 'bad-edge',
      nodes: baseNodes,
      edges: [{ id: 'e1', from: 'start', to: 'ghost' }]
    };
    const res = validateDefinition(def);
    expect(res.errors).toContain('Edge e1 references non-existent target node: ghost');
  });

  it('flags duplicate node IDs', () => {
    const def: DslDefinition = {
      key: 'dup-nodes',
      nodes: [
        { id: 'start', type: 'start', x: 0, y: 0 },
        { id: 'start', type: 'automatic', x: 50, y: 50 },
        { id: 'end', type: 'end', x: 100, y: 0 }
      ],
      edges: []
    };
    const res = validateDefinition(def);
    expect(res.errors).toContain('Duplicate node IDs: start');
  });

  it('flags gateway without condition', () => {
    const def: DslDefinition = {
      key: 'gw-missing-cond',
      nodes: [...baseNodes, { id: 'gw1', type: 'gateway', x: 100, y: 50, label: 'Decision', condition: '' } as any],
      edges: []
    };
    const res = validateDefinition(def);
    expect(res.errors).toContain('Gateway node "Decision" must have a condition');
  });
});

describe('DSL validateNode', () => {
  it('accepts timer with delaySeconds only', () => {
    const timer: TimerNode = {
      id: 't1',
      type: 'timer',
      x: 0,
      y: 0,
      label: 'Timer',
      delaySeconds: 90
    };
    const res = validateNode(timer);
    expect(res.isValid).toBe(true);
    expect(res.errors).toHaveLength(0);
  });

  it('errors on timer with no delayMinutes, delaySeconds, or untilIso', () => {
    const timer: TimerNode = {
      id: 't2',
      type: 'timer',
      x: 0,
      y: 0,
      label: 'BadTimer'
    };
    const res = validateNode(timer);
    expect(res.isValid).toBe(false);
    expect(res.errors[0]).toMatch(/Timer node must have either delayMinutes, delaySeconds, or untilIso/);
  });

  it('errors on gateway without condition', () => {
    const gw: GatewayNode = {
      id: 'gw',
      type: 'gateway',
      x: 0,
      y: 0,
      label: 'Decision',
      condition: ''
    };
    const res = validateNode(gw);
    expect(res.isValid).toBe(false);
    expect(res.errors).toContain('Gateway node must have a condition');
  });

  it('warns on humanTask without assignee roles', () => {
    const ht: HumanTaskNode = {
      id: 'h1',
      type: 'humanTask',
      x: 0,
      y: 0,
      label: 'Review'
    };
    const res = validateNode(ht);
    expect(res.isValid).toBe(true);
    expect(res.warnings).toContain('HumanTask should have assignee roles');
  });
});
