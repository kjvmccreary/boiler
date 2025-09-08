import { describe, it, expect } from 'vitest';
import approvalBasic from '@/test/fixtures/workflow/approval.basic.json';
import { workflowService } from '@/services/workflow.service';

describe('Integration-Lite / Claim & Complete Flow', () => {
  it('draft → publish → start → claim → complete task', async () => {
    const draft = await workflowService.createDraft({
      key: 'claim-int',
      name: 'Claim Flow',
      description: 'Claim/Complete path',
      jsonDefinition: approvalBasic
    } as any);
    const published = await workflowService.publishDefinition((draft as any).id);
    expect((published as any).status).toBe('Published');

    const inst = await workflowService.startInstance({ definitionKey: 'claim-int' } as any);
    expect(inst).toBeTruthy();

    const tasks = await workflowService.getTasks({ mine: true });
    expect(tasks.length).toBeGreaterThan(0);
    const first = tasks[0];

    const claimed = await workflowService.claimTask((first as any).id, {} as any);
    expect(String(claimed.status).toLowerCase()).toBe('claimed');

    const completed = await workflowService.completeTask((first as any).id, { completionData: '{}' } as any);
    expect(String(completed.status).toLowerCase()).toBe('completed');
  });
});
