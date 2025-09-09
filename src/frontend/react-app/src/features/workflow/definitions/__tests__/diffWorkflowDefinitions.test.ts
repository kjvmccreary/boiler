import { describe, it, expect } from 'vitest';
import { diffWorkflowDefinitions } from '../utils/diffWorkflowDefinitions';

function wrap(nodes: any[], edges: any[] = []) {
  return JSON.stringify({ nodes, edges });
}

describe('diffWorkflowDefinitions (PR2)', () => {
  it('detects added node', () => {
    const prev = wrap([{ id: 'a', type: 'start' }]);
    const curr = wrap([{ id: 'a', type: 'start' }, { id: 'b', type: 'end' }]);
    const diff = diffWorkflowDefinitions(curr, prev);
    expect(diff.addedNodes.map((n: any) => n.id)).toContain('b');
    expect(diff.summary.addedNodes).toBe(1);
  });

  it('detects removed node', () => {
    const prev = wrap([{ id: 'a', type: 'start' }, { id: 'b', type: 'end' }]);
    const curr = wrap([{ id: 'a', type: 'start' }]);
    const diff = diffWorkflowDefinitions(curr, prev);
    expect(diff.removedNodes.map((n: any) => n.id)).toContain('b');
    expect(diff.summary.removedNodes).toBe(1);
  });

  it('detects modified gateway strategy', () => {
    const prev = wrap([{ id: 'g1', type: 'gateway', strategy: 'exclusive' }]);
    const curr = wrap([{ id: 'g1', type: 'gateway', strategy: 'parallel' }]);
    const diff = diffWorkflowDefinitions(curr, prev);
    expect(diff.modifiedNodes.length).toBe(1);
    expect(diff.modifiedNodes[0].changedKeys).toContain('strategy');
  });

  it('detects join mode and threshold change', () => {
    const prev = wrap([{ id: 'j1', type: 'join', mode: 'all' }]);
    const curr = wrap([{ id: 'j1', type: 'join', mode: 'count', thresholdCount: 2 }]);
    const diff = diffWorkflowDefinitions(curr, prev);
    expect(diff.modifiedNodes[0].changedKeys).toEqual(expect.arrayContaining(['mode', 'thresholdCount']));
  });

  it('detects humanTask assignment role count change', () => {
    const prev = wrap([{ id: 't1', type: 'humanTask', assignment: { mode: 'roles', roles: ['Ops'] } }]);
    const curr = wrap([{ id: 't1', type: 'humanTask', assignment: { mode: 'roles', roles: ['Ops', 'QA'] } }]);
    const diff = diffWorkflowDefinitions(curr, prev);
    expect(diff.modifiedNodes[0].changedKeys).toContain('assignmentRolesCount');
  });

  it('detects automatic action kind change', () => {
    const prev = wrap([{ id: 'a1', type: 'automatic', action: { kind: 'noop' } }]);
    const curr = wrap([{ id: 'a1', type: 'automatic', action: { kind: 'webhook', retryPolicy: { maxAttempts: 3 } } }]);
    const diff = diffWorkflowDefinitions(curr, prev);
    expect(diff.modifiedNodes[0].changedKeys).toEqual(expect.arrayContaining(['actionKind', 'retryMaxAttempts']));
  });

  it('detects edge add/remove', () => {
    const prev = wrap([{ id: 's', type: 'start' }, { id: 'e', type: 'end' }], [{ id: 'se', from: 's', to: 'e' }]);
    const curr = wrap([{ id: 's', type: 'start' }, { id: 'm', type: 'automatic' }, { id: 'e', type: 'end' }],
      [{ id: 'sm', from: 's', to: 'm' }, { id: 'me', from: 'm', to: 'e' }]);
    const diff = diffWorkflowDefinitions(curr, prev);
    expect(diff.addedEdges.length).toBe(2);
    expect(diff.removedEdges.length).toBe(1);
  });

  it('no differences yields empty summary', () => {
    const prev = wrap([{ id: 'a', type: 'start' }, { id: 'b', type: 'end' }]);
    const curr = wrap([{ id: 'a', type: 'start' }, { id: 'b', type: 'end' }]);
    const diff = diffWorkflowDefinitions(curr, prev);
    expect(diff.summary).toMatchObject({
      addedNodes: 0,
      removedNodes: 0,
      modifiedNodes: 0,
      addedEdges: 0,
      removedEdges: 0
    });
  });
});
