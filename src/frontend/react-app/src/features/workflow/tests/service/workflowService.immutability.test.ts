import { describe, it, expect } from 'vitest';
import { workflowService } from '@/services/workflow.service';
import approvalBasic from '@/test/fixtures/workflow/approval.basic.json';

/**
 * Relies on MSW updateDefinitionHandler returning 409 once status=Published.
 * If workflowService.updateDefinition does not exist yet, skip or adjust.
 */
describe('WorkflowService / Definition Immutability', () => {
  it('blocks update after publish', async () => {
    const draft = await workflowService.createDraft({
      key: 'immutable_def',
      name: 'Immutable Def',
      description: 'Test immutability',
      jsonDefinition: approvalBasic
    } as any);

    await workflowService.publishDefinition((draft as any).id);

    let error: any;
    try {
      await workflowService.updateDefinition((draft as any).id, {
        name: 'Changed Name',
        description: 'Should fail',
        jsonDefinition: approvalBasic
      } as any);
    } catch (e: any) {
      error = e;
    }
    expect(error).toBeTruthy();
    expect(String(error.message || error)).toMatch(/Published.*cannot|modified|Immutable/i);
  });
});
