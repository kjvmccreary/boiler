import { describe, it, expect } from 'vitest';
import { workflowService } from '@/services/workflow.service';
import { server } from '@/test/setup';
import { http, HttpResponse } from 'msw';

// Helper to wrap ApiResponse shape
function api<T>(data: T, success = true, message?: string) {
  return { success, data, message, errors: success ? [] : ['err'] };
}

describe('WorkflowService status + error normalization', () => {

  it('normalizes numeric instance status to enum', async () => {
    server.use(
      http.get('/api/workflow/instances/123', () =>
        HttpResponse.json(api({
          id: 123,
          definitionKey: 'k',
          definitionVersion: 1,
          status: 2, // Completed numeric (mapped)
          currentNodeIds: [],
          startedAt: new Date().toISOString()
        }))
      )
    );
    const inst = await workflowService.getInstance(123 as any);
    expect(inst.status).toBe('Completed');
  });

  it('normalizes numeric task statuses in list', async () => {
    server.use(
      http.get('/api/workflow/tasks?mine=true', () =>
        HttpResponse.json(api([
          { id: 1, workflowInstanceId: 10, nodeId: 'n2', taskName: 'T1', status: 5, createdAt: new Date().toISOString() }, // Completed
          { id: 2, workflowInstanceId: 10, nodeId: 'n3', taskName: 'T2', status: 1, createdAt: new Date().toISOString() }  // Created
        ]))
      )
    );
    const tasks = await workflowService.getTasks({ mine: true });
    expect(tasks[0].status).toBe('Completed');
    expect(tasks[1].status).toBe('Created');
  });

  it('validateThenPublish short-circuits when validation fails', async () => {
    server.use(
      http.get('/api/workflow/definitions/55/validate', () =>
        HttpResponse.json({
          success: false,
          errors: ['Unreachable nodes'],
          warnings: []
        }, { status: 200 })
      ),
      http.post('/api/workflow/definitions/55/publish', () => {
        throw new Error('Should not be called');
      })
    );
    const res = await workflowService.validateThenPublish(55);
    expect(res.validation?.success).toBe(false);
    expect(res.published).toBeUndefined();
  });

  it('publishDefinition surfaces server error messages (array)', async () => {
    server.use(
      http.post('/api/workflow/definitions/77/publish', () =>
        HttpResponse.json({ success: false, errors: ['A', 'B'], message: 'A; B' }, { status: 422 })
      )
    );
    await expect(workflowService.publishDefinition(77)).rejects.toThrow(/A; B|A/);
  });

  it('publishDefinition handles single string error shape', async () => {
    server.use(
      http.post('/api/workflow/definitions/88/publish', () =>
        HttpResponse.json({ success: false, errors: 'Invalid', message: 'Invalid' }, { status: 422 })
      )
    );
    await expect(workflowService.publishDefinition(88)).rejects.toThrow(/Invalid/);
  });

  it('validateDefinitionJson returns success false on server 422 with errors', async () => {
    server.use(
      http.post('/api/workflow/definitions/validate', () =>
        HttpResponse.json({
          success: false,
          errors: ['Duplicate start'],
          warnings: []
        }, { status: 422 })
      )
    );
    const res = await workflowService.validateDefinitionJson(JSON.stringify({}));
    expect(res.success).toBe(false);
    expect(res.errors).toContain('Duplicate start');
  });
});
