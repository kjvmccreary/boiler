import { describe, it, expect, beforeAll, afterEach } from 'vitest';
import { startInstance } from '../../services/workflowInstancesService';
import {
  installApiClientMocks,
  enqueueOk,
  resetApiQueue,
  assertQueueDrained
} from './_testUtils/mockApiClient';

beforeAll(() => installApiClientMocks());
afterEach(() => {
  assertQueueDrained();
  resetApiQueue();
});

describe('startInstance unwrap', () => {
  it('returns flattened instance DTO (no envelope)', async () => {
    enqueueOk('post', { id: 123, status: 'Running', definitionVersion: 2 });
    const inst = await startInstance({ workflowDefinitionId: 5, initialContext: '{}' });
    expect(inst.id).toBe(123);
    expect((inst as any).success).toBeUndefined();
    expect((inst as any).data).toBeUndefined();
  });
});
