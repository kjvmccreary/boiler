import { describe, it, expect, beforeAll, afterEach } from 'vitest';
import * as inst from '../../services/workflowInstancesService';
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

describe('Instance status normalization', () => {
  it('numeric status normalized to string', async () => {
    // service normalizes; backend returns number 2 (Completed)
    enqueueOk('get', { id: 55, status: 2, definitionVersion: 1 });
    const res = await inst.getInstance(55);
    expect(typeof res.status === 'string').toBe(true);
  });
});
