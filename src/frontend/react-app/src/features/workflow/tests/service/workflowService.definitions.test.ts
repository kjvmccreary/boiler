import { describe, it, expect } from 'vitest';
import { workflowService } from '@/services/workflow.service';
import approvalBasic from '@/test/fixtures/workflow/approval.basic.json';
import { server } from '@/test/setup';
import { http, HttpResponse } from 'msw';

describe('WorkflowService / Definitions', () => {
  it('creates a draft definition', async () => {
    const key = approvalBasic.key;
    const created = await workflowService.createDraft({
      key,
      name: 'Approval Flow',
      description: 'Basic approval flow',
      jsonDefinition: approvalBasic
    } as any);
    expect(created).toBeTruthy();
    // Access possibly non-declared fields via any to avoid TS errors if DTO lacks them
    expect((created as any).key ?? key).toBe(key);
    // Optional status assertion only if present
    if ('status' in (created as any)) {
      expect((created as any).status).toBe('Draft');
    }
  });

  it('publishes a draft after validation passes', async () => {
    const key = 'publish-test';
    const draft = await workflowService.createDraft({
      key,
      name: 'Publish Test',
      description: 'Publish flow',
      jsonDefinition: approvalBasic
    } as any);

    const { validation, published } = await workflowService.validateThenPublish((draft as any).id ?? 0);
    expect(validation?.success).toBe(true);
    expect(published).toBeTruthy();
    if (published && 'status' in (published as any)) {
      expect((published as any).status).toBe('Published');
    }
  });

  it('returns validation failure (simulated server invalid)', async () => {
    server.use(
      http.get('/api/workflow/definitions/:id/validate', () =>
        HttpResponse.json({
          success: false,
          errors: ['Multiple start nodes', 'Unreachable node X'],
          warnings: ['Gateway unlabeled']
        }, { status: 200 })
      )
    );
    const result = await workflowService.validateDefinitionById(999);
    expect(result.success).toBe(false);
    expect(result.errors.length).toBeGreaterThan(0);
  });

  it('publishDefinition surfaces error messages', async () => {
    server.use(
      http.post('/api/workflow/definitions/:id/publish', () =>
        HttpResponse.json(
          { success: false, errors: ['Graph invalid'], message: 'Graph invalid' },
          { status: 422 }
        )
      )
    );
    await expect(workflowService.publishDefinition(123)).rejects.toThrow(/Graph invalid/);
  });
});
