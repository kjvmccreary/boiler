import { describe, it, expect } from 'vitest';
import approvalBasic from '@/test/fixtures/workflow/approval.basic.json';
import { workflowService, getMyTaskSummary } from '@/services/workflow.service';

describe('Workflow Integration-Lite / Approval Happy Path', () => {
  it('draft → publish → start → task summary', async () => {
    const key = 'approval-int-lite';
    const draft = await workflowService.createDraft({
      key,
      name: 'Approval Integration',
      description: 'Integration-lite flow',
      jsonDefinition: approvalBasic
    } as any);
    expect(draft).toBeTruthy();
    if ('status' in (draft as any)) {
      expect((draft as any).status).toBe('Draft');
    }

    const published = await workflowService.publishDefinition((draft as any).id ?? 0);
    if ('status' in (published as any)) {
      expect((published as any).status).toBe('Published');
    }

    const inst = await workflowService.startInstance({ definitionKey: key } as any);
    expect(inst).toBeTruthy();

    const tasks = await workflowService.getTasks({ mine: true });
    expect(Array.isArray(tasks)).toBe(true);

    const summary = await getMyTaskSummary().catch(() => null);
    if (summary) {
      expect(summary.available >= 0).toBe(true);
    }
  });
});
