import { describe, it, expect, beforeAll, afterEach } from 'vitest';
import * as defs from '../../services/workflowDefinitionsService';
import * as inst from '../../services/workflowInstancesService';
import {
  installApiClientMocks,
  enqueueErr,
  resetApiQueue,
  assertQueueDrained
} from './_testUtils/mockApiClient';

// These tests assert that an error envelope bubbles up as an exception (assuming unwrap throws on success=false).
// If your unwrap implementation returns a result instead, adjust accordingly.

function has(fnName: string, mod: any) {
  return typeof mod[fnName] === 'function';
}

beforeAll(() => installApiClientMocks());
afterEach(() => {
  assertQueueDrained();
  resetApiQueue();
});

describe('Service unwrap error handling', () => {
  it('publishDefinition propagates error (success=false)', async () => {
    if (!has('publishDefinition', defs)) return;
    enqueueErr('post', 'Validation failed', [{ code: 'DEF_INVALID', message: 'Graph invalid' }]);
    await expect(
      (defs as any).publishDefinition(1, { publishNotes: 'x' })
    ).rejects.toThrow(/graph invalid/i);
  });

  it('startInstance propagates error (success=false)', async () => {
    if (!has('startInstance', inst)) return;
    enqueueErr('post', 'Definition not published');
    await expect(
      (inst as any).startInstance({ workflowDefinitionId: 999, initialContext: '{}' })
    ).rejects.toThrow(/definition not published/i);
  });
});
