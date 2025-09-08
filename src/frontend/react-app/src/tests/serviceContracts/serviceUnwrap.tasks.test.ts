import { describe, it, expect, beforeAll, afterEach } from 'vitest';
import * as tasks from '../../services/workflowTasksService';
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

describe('Tasks service unwrap contracts', () => {
  it('getTask', async () => {
    enqueueOk('get', { id: 200, taskName: 'Approve', status: 'Created' });
    const t = await tasks.getTask(200);
    expect(t.taskName).toBe('Approve');
  });

  it('getTasksPaged synthetic', async () => {
    enqueueOk('get', [
      { id: 201, taskName: 'A', status: 'Created' },
      { id: 202, taskName: 'B', status: 'Created' }
    ]);
    const page = await tasks.getTasksPaged({ page: 1, pageSize: 50 });
    expect(page.items.length).toBe(2);
  });

  it('claimTask', async () => {
    enqueueOk('post', { id: 203, status: 'Claimed' });
    const t = await tasks.claimTask(203);
    expect(t.status).toBe('Claimed');
  });

  it('completeTask', async () => {
    enqueueOk('post', { id: 204, status: 'Completed' });
    const t = await tasks.completeTask(204, '{}');
    expect(t.status).toBe('Completed');
  });

  it('assignTask', async () => {
    enqueueOk('post', { id: 205, status: 'Assigned', assignedToUserId: 5 });
    const t = await tasks.assignTask(205, { userId: 5 } as any);
    expect(t.assignedToUserId).toBe(5);
  });
});
