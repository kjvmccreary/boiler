import { describe, it, expect, beforeAll, afterEach } from 'vitest';
import * as inst from '../../services/workflowInstancesService';
import {
  installApiClientMocks,
  enqueueOk,
  enqueueErr,
  resetApiQueue,
  assertQueueDrained
} from './_testUtils/mockApiClient';

function assertNoEnvelope(o: any) {
  expect(o).not.toHaveProperty('success');
  expect(o).not.toHaveProperty('data');
}

beforeAll(() => installApiClientMocks());
afterEach(() => {
  assertQueueDrained();
  resetApiQueue();
});

describe('Instances service unwrap contracts', () => {
  it('getInstancesPaged', async () => {
    enqueueOk('get', { items: [{ id: 1, status: 'Running' }], totalCount: 1, page: 1, pageSize: 25 });
    const res = await inst.getInstancesPaged({ page: 1, pageSize: 25 });
    expect(res.items[0].id).toBe(1);
  });

  it('getInstances', async () => {
    enqueueOk('get', { items: [{ id: 2, status: 'Completed' }], totalCount: 1, page: 1, pageSize: 25 });
    const res = await inst.getInstances({ page: 1 });
    expect(Array.isArray(res)).toBe(true);
    assertNoEnvelope(res);
  });

  it('getInstance', async () => {
    enqueueOk('get', { id: 3, status: 'Running' });
    const res = await inst.getInstance(3);
    expect(res.status).toBe('Running');
    assertNoEnvelope(res);
  });

  it('getInstanceStatus', async () => {
    enqueueOk('get', { id: 3, status: 'Running' });
    const res = await inst.getInstanceStatus(3);
    expect(res.status).toBe('Running');
    assertNoEnvelope(res);
  });

  it('startInstance', async () => {
    enqueueOk('post', { id: 10, status: 'Running', definitionVersion: 2 });
    const res = await inst.startInstance({ workflowDefinitionId: 5, initialContext: '{}' });
    expect(res.id).toBe(10);
    assertNoEnvelope(res);
  });

  it('signalInstance', async () => {
    enqueueOk('post', { id: 10, status: 'Running', lastSignal: 'poke' });
    const res = await inst.signalInstance(10, { signal: 'poke', payload: '{}' } as any);
    expect((res as any).lastSignal).toBe('poke');
  });

  it('terminateInstance', async () => {
    enqueueOk('delete', true);
    const res = await inst.terminateInstance(10);
    expect(res).toBe(true);
  });

  it('suspendInstance', async () => {
    enqueueOk('post', { id: 11, status: 'Suspended' });
    const res = await inst.suspendInstance(11);
    expect(res.status).toBe('Suspended');
  });

  it('resumeInstance', async () => {
    enqueueOk('post', { id: 11, status: 'Running' });
    const res = await inst.resumeInstance(11);
    expect(res.status).toBe('Running');
  });

  it('retryInstance', async () => {
    enqueueOk('post', { id: 12, status: 'Running' });
    const res = await inst.retryInstance(12, { retryReason: 'reset' });
    expect(res.id).toBe(12);
  });

  it('moveInstanceToNode', async () => {
    enqueueOk('post', { id: 13, status: 'Running', currentNodeIds: ['manualReset'] });
    const res = await inst.moveInstanceToNode(13, { targetNodeId: 'manualReset' } as any);
    expect(res.currentNodeIds?.includes('manualReset')).toBe(true);
  });

  it('getRuntimeSnapshot', async () => {
    enqueueOk('get', { instanceId: 10, activeNodes: ['x'], visited: [] });
    const res = await inst.getRuntimeSnapshot(10);
    expect(res.instanceId).toBe(10);
    assertNoEnvelope(res);
  });

  it('error path startInstance', async () => {
    enqueueErr('post', 'Definition not published');
    await expect(
      inst.startInstance({ workflowDefinitionId: 999, initialContext: '{}' })
    ).rejects.toThrow(/definition not published/i);
  });
});
