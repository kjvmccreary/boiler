import { describe, it, expect, beforeEach } from 'vitest';
import { workflowService } from '@/services/workflow.service';
import approvalBasic from '@/test/fixtures/workflow/approval.basic.json';
import { server } from '@/test/setup';
import { http, HttpResponse } from 'msw';

function api<T>(data: T, success = true) {
  return { success, data, errors: success ? [] : ['err'] };
}

describe('WorkflowService / API call batching (single network per action)', () => {
  let claimCalls = 0;
  let completeCalls = 0;
  let tasksCalls = 0;

  beforeEach(() => {
    claimCalls = 0;
    completeCalls = 0;
    tasksCalls = 0;
    server.resetHandlers();

    server.use(
      // Create draft
      http.post('/api/workflow/definitions/draft', async ({ request }) => {
        const body = await request.json() as any;
        return HttpResponse.json(api({
          id: 'def-batch',
          key: body.key,
          version: 1,
          status: 'Draft',
          json: body.jsonDefinition,
          createdAt: new Date().toISOString()
        }), { status: 201 });
      }),
      // Publish
      http.post('/api/workflow/definitions/def-batch/publish', () =>
        HttpResponse.json(api({
          id: 'def-batch',
          key: 'batch_def',
          version: 1,
          status: 'Published',
          json: approvalBasic,
          createdAt: new Date().toISOString(),
          publishedAt: new Date().toISOString()
        }))
      ),
      // Start instance
      http.post('/api/workflow/instances', async ({ request }) => {
        const body = await request.json() as any;
        return HttpResponse.json(api({
          id: 'inst-batch',
            definitionKey: body.definitionKey,
            definitionVersion: 1,
            status: 'Running',
            currentNodeIds: ['n2'],
            startedAt: new Date().toISOString(),
            context: {}
        }), { status: 201 });
      }),
      // Tasks list
      http.get('/api/workflow/tasks', () => {
        tasksCalls++;
        return HttpResponse.json(api([
          {
            id: 'task-batch',
            workflowInstanceId: 'inst-batch',
            nodeId: 'n2',
            taskName: 'Approve Request',
            status: 1,
            createdAt: new Date().toISOString()
          }
        ]));
      }),
      // Claim
      http.post('/api/workflow/tasks/task-batch/claim', () => {
        claimCalls++;
        return HttpResponse.json(api({
          id: 'task-batch',
          workflowInstanceId: 'inst-batch',
          nodeId: 'n2',
          taskName: 'Approve Request',
          status: 3,
          createdAt: new Date().toISOString()
        }));
      }),
      // Complete
      http.post('/api/workflow/tasks/task-batch/complete', () => {
        completeCalls++;
        return HttpResponse.json(api({
          id: 'task-batch',
          workflowInstanceId: 'inst-batch',
          nodeId: 'n2',
          taskName: 'Approve Request',
          status: 5,
          createdAt: new Date().toISOString()
        }));
      })
    );
  });

  it('claim + complete each perform exactly one POST to their endpoints', async () => {
    await workflowService.createDraft({
      key: 'batch_def',
      name: 'Batch Def',
      description: 'Batch test',
      jsonDefinition: approvalBasic
    } as any);
    await workflowService.publishDefinition('def-batch' as any);
    await workflowService.startInstance({ definitionKey: 'batch_def' } as any);

    const tasks = await workflowService.getTasks({ mine: true });
    expect(tasks.length).toBe(1);
    const task = tasks[0];

    const claimed = await workflowService.claimTask((task as any).id, {} as any);
    expect(claimed.status).toBe('Claimed');
    expect(claimCalls).toBe(1);

    const completed = await workflowService.completeTask((task as any).id, { completionData: '{}' } as any);
    expect(completed.status).toBe('Completed');
    expect(completeCalls).toBe(1);

    expect(tasksCalls).toBeGreaterThan(0);
  });
});
