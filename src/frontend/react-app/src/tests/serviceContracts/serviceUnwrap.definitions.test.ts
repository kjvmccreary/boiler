import { describe, it, expect, afterEach, beforeAll } from 'vitest';
import * as defs from '../../services/workflowDefinitionsService';
import {
  installApiClientMocks,
  enqueueOk,
  enqueueErr,
  resetApiQueue,
  assertQueueDrained
} from './_testUtils/mockApiClient';

function assertNoEnvelope(obj: any) {
  expect(obj).not.toHaveProperty('success');
  // Paged objects legitimately have items/totalCount etc; top-level 'data' should be absent
  if (obj && typeof obj === 'object') {
    expect(Object.prototype.hasOwnProperty.call(obj, 'data')).toBe(false);
  }
}

beforeAll(() => installApiClientMocks());
afterEach(() => {
  assertQueueDrained();
  resetApiQueue();
});

describe('Definitions service unwrap contracts', () => {
  it('getDefinitionsPaged', async () => {
    enqueueOk('get', { items: [{ id: 7, name: 'Flow' }], totalCount: 1, page: 1, pageSize: 25 });
    const res = await defs.getDefinitionsPaged({ page: 1, pageSize: 25 });
    expect(res.items).toHaveLength(1);
    expect(res.totalCount).toBe(1);
    assertNoEnvelope(res);
  });

  it('getDefinitions returns items array', async () => {
    enqueueOk('get', { items: [{ id: 11, name: 'DefA' }], totalCount: 1, page: 1, pageSize: 50 });
    const res = await defs.getDefinitions({ page: 1, pageSize: 50 });
    expect(Array.isArray(res)).toBe(true);
    expect(res[0].id).toBe(11);
  });

  it('getDefinition', async () => {
    enqueueOk('get', { id: 5, name: 'SingleDef' });
    const res = await defs.getDefinition(5);
    expect(res.id).toBe(5);
    assertNoEnvelope(res);
  });

  it('createDraft', async () => {
    enqueueOk('post', { id: 21, name: 'Draft' });
    const res = await defs.createDraft({ name: 'Draft', jsonDefinition: '{}' });
    expect(res.id).toBe(21);
  });

  it('updateDefinition', async () => {
    enqueueOk('put', { id: 21, name: 'DraftUpdated' });
    const res = await defs.updateDefinition(21, { name: 'DraftUpdated', jsonDefinition: '{}' });
    expect(res.name).toBe('DraftUpdated');
  });

  it('publishDefinition', async () => {
    enqueueOk('post', { id: 42, isPublished: true, publishNotes: 'ok' });
    const res = await defs.publishDefinition(42, { publishNotes: 'ok' });
    expect(res.isPublished).toBe(true);
  });

  it('validateDefinitionJson', async () => {
    enqueueOk('post', { isValid: true, errors: [], warnings: [] });
    const res = await defs.validateDefinitionJson('{ "nodes": [] }');
    expect(res.success).toBe(true);
  });

  it('validateDefinitionById', async () => {
    enqueueOk('get', { isValid: true, errors: [], warnings: [] });
    const res = await defs.validateDefinitionById(99);
    expect(res.success).toBe(true);
  });

  it('createNewVersion', async () => {
    enqueueOk('post', { id: 50, version: 3 });
    // CreateNewVersionRequestDto does not define 'reason'; pass empty object (shape‑agnostic)
    const res = await defs.createNewVersion(50, {} as any);
    expect(res.version).toBe(3);
  });

  it('revalidateDefinition', async () => {
    enqueueOk('post', { isValid: true, errors: [], warnings: [] });
    const res = await defs.revalidateDefinition(50);
    expect(res.isValid).toBe(true);
  });

  it('archiveDefinition', async () => {
    enqueueOk('post', { id: 77, isArchived: true });
    const res = await defs.archiveDefinition(77);
    expect(res.isArchived).toBe(true);
  });

  it('unpublishDefinition', async () => {
    enqueueOk('post', { id: 80, isPublished: false });
    const res = await defs.unpublishDefinition(80);
    expect(res.isPublished).toBe(false);
  });

  it('terminateDefinitionInstances', async () => {
    enqueueOk('post', { terminated: 3 });
    const res = await defs.terminateDefinitionInstances(90);
    expect(res.terminated).toBe(3);
  });

  it('deleteDefinition', async () => {
    enqueueOk('delete', true);
    const res = await defs.deleteDefinition(21);
    expect(res).toBe(true);
  });

  it('publishDefinition error unwrap', async () => {
    enqueueErr('post', 'Graph invalid', [{ code: 'DEF_INVALID', message: 'Graph invalid' }]);
    await expect(defs.publishDefinition(999, { publishNotes: 'bad' })).rejects.toThrow(/graph invalid/i);
  });
});
