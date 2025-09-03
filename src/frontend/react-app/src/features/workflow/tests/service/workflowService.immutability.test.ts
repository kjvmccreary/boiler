import { describe, it, expect } from 'vitest';
import { workflowService } from '@/services/workflow.service';
import { server } from '@/test/setup';
import { http, HttpResponse } from 'msw';

function api<T>(data: T, success = true, message?: string) {
  return { success, data, message, errors: success ? [] : ['err'] };
}

describe.skip('WorkflowService definition immutability (SKIPPED - enable when update rules enforced)', () => {
  it('prevents updating a published definition (placeholder)', async () => {
    // TODO: When /api/workflow/definitions/:id (PUT) handler is available for published check
    server.use(
      http.get('/api/workflow/definitions/901', () =>
        HttpResponse.json(api({
          id: 901,
            key: 'immutable_def',
            version: 1,
            status: 'Published',
            json: { key: 'immutable_def', nodes: [], edges: [] },
            createdAt: new Date().toISOString(),
            publishedAt: new Date().toISOString()
        }))
      )
    );
    expect(true).toBe(true);
  });
});
