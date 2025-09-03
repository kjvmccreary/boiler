import { describe, it, expect } from 'vitest';
import { serializeToDsl, deserializeFromDsl } from '@/features/workflow/dsl/dsl.serialize';
import approvalBasic from '@/test/fixtures/workflow/approval.basic.json';

describe('Builder Serialization (placeholder)', () => {
  it('round-trips basic approval fixture', () => {
    const dsl = approvalBasic; // fixture has: key, nodes, edges (no version)
    const flowNodes = dsl.nodes.map(n => ({
      id: n.id,
      type: n.type,
      position: { x: n.x, y: n.y },
      data: { ...n }
    }));
    const flowEdges = dsl.edges.map(e => ({
      id: e.id,
      source: e.from,
      target: e.to,
      label: e.label
    }));

    // Remove dsl.version (not present in fixture)
    const serialized = serializeToDsl(flowNodes as any, flowEdges as any, dsl.key);
    expect(serialized.nodes.length).toBe(dsl.nodes.length);

    const { nodes, edges } = deserializeFromDsl(serialized);
    expect(nodes.length).toBe(dsl.nodes.length);
    expect(edges.length).toBe(dsl.edges.length);
  });
});

