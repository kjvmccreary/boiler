import { describe, it, expect } from 'vitest';
import { workflowService } from '@/services/workflow.service';
import approvalBasic from '@/test/fixtures/workflow/approval.basic.json';

describe('WorkflowService / Instances & Tasks', () => {
  let draftKey: string;

  it('creates & publishes definition, then starts instance', async () => {
    draftKey = 'service-inst';
    const draft = await workflowService.createDraft({
      key: draftKey,
      name: 'Service Inst',
      description: 'Service inst test',
      jsonDefinition: approvalBasic
    } as any);

    await workflowService.publishDefinition((draft as any).id ?? 0);

    const instance = await workflowService.startInstance({
      definitionKey: draftKey
    } as any);

    expect(instance).toBeTruthy();
    // If definitionKey not on DTO type, skip strict typing
    if ('definitionKey' in (instance as any)) {
      expect((instance as any).definitionKey).toBe(draftKey);
    }
  });

  it('lists tasks (mine filter)', async () => {
    const tasks = await workflowService.getTasks({ mine: true });
    expect(Array.isArray(tasks)).toBe(true);
  });

  it('gracefully handles validateDefinitionJson (happy path)', async () => {
    const json = JSON.stringify(approvalBasic);
    const res = await workflowService.validateDefinitionJson(json);
    expect(res.success).toBe(true);
  });
});
