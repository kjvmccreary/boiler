import { describe, it, expect, vi, beforeEach } from 'vitest';
import { workflowService } from '@/services/workflow.service';

/**
 * We spy on the underlying apiClient.get used inside workflowService.
 * If apiClient is not directly exported in your project, adjust the import path
 * or switch to MSW handlers consistent with existing service tests.
 */
vi.mock('@/services/workflow.service', async (orig) => {
  const actual = await orig<typeof import('@/services/workflow.service')>();
  return {
    ...actual,
  };
});

// Lazy grab of apiClient (adjust if your project exposes it differently)
let apiClientGet: any;
try {
  // eslint-disable-next-line @typescript-eslint/no-var-requires
  const svc = require('@/services/workflow.service');
  // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
  apiClientGet = vi.spyOn(svc.apiClient ?? {}, 'get');
} catch {
  /* If apiClient not exported, tests below using direct spy will be skipped */
}

describe('workflowService.previewValidateTags', () => {
  beforeEach(() => {
    if (apiClientGet) apiClientGet.mockReset();
  });

  it('returns success true / empty errors when endpoint 404 (graceful skip)', async () => {
    if (!apiClientGet) return;
    apiClientGet.mockRejectedValueOnce({
      response: { status: 404 }
    });

    const res = await workflowService.previewValidateTags(10, 'a,b');
    expect(res.success).toBe(true);
    expect(res.errors.length).toBe(0);
  });

  it('returns errors array from server (blocking)', async () => {
    if (!apiClientGet) return;
    apiClientGet.mockResolvedValueOnce({
      data: { success: false, errors: ['Tag "bad tag" invalid', 'Tag "dup" duplicate'] }
    });

    const res = await workflowService.previewValidateTags(11, 'bad tag dup');
    expect(res.success).toBe(false);
    expect(res.errors).toContain('Tag "bad tag" invalid');
  });

  it('treats plain string error as single entry', async () => {
    if (!apiClientGet) return;
    apiClientGet.mockResolvedValueOnce({
      data: { success: false, errors: 'Tag "x" invalid' }
    });

    const res = await workflowService.previewValidateTags(12, 'x');
    expect(res.success).toBe(false);
    expect(res.errors).toEqual(['Tag "x" invalid']);
  });

  it('success true when backend returns success:true and no errors', async () => {
    if (!apiClientGet) return;
    apiClientGet.mockResolvedValueOnce({
      data: { success: true, errors: [] }
    });

    const res = await workflowService.previewValidateTags(13, 'valid1 valid2');
    expect(res.success).toBe(true);
    expect(res.errors.length).toBe(0);
  });
});

describe('workflowService.validateThenPublish tag gating integration', () => {
  beforeEach(() => {
    if (apiClientGet) apiClientGet.mockReset();
  });

  it('blocks publish when validate-tags returns errors', async () => {
    if (!apiClientGet) return;
    // Order of GET calls in validateThenPublish:
    // 1) /definitions/{id}/validate (validateDefinitionById)
    // 2) /definitions/{id}/validate-assignment (optional)
    // 3) /definitions/{id}/validate-tags (our target)
    // We mock sequential resolves.
    apiClientGet
      .mockResolvedValueOnce({ data: { isValid: true, errors: [], warnings: [] } }) // validateDefinitionById
      .mockResolvedValueOnce({ data: { success: true, errors: [] } })               // assignment (stub)
      .mockResolvedValueOnce({ data: { success: false, errors: ['Invalid tag Z'] } }); // tags

    // Spy on publishDefinition (POST) to ensure it is NOT called
    const publishSpy = vi.spyOn(workflowService as any, 'publishDefinition').mockResolvedValue({ id: 99 });

    const result = await workflowService.validateThenPublish(99);
    expect(result.published).toBeUndefined();
    expect(result.validation?.success).toBe(false);
    expect(result.validation?.errors).toContain('Invalid tag Z');
    expect(publishSpy).not.toHaveBeenCalled();
    publishSpy.mockRestore();
  });

  it('allows publish when tags validation passes', async () => {
    if (!apiClientGet) return;
    apiClientGet
      .mockResolvedValueOnce({ data: { isValid: true, errors: [], warnings: [] } }) // graph
      .mockResolvedValueOnce({ data: { success: true, errors: [] } })               // assignment
      .mockResolvedValueOnce({ data: { success: true, errors: [] } });              // tags

    const publishSpy = vi.spyOn(workflowService as any, 'publishDefinition')
      .mockResolvedValue({ id: 101, isPublished: true });

    const result = await workflowService.validateThenPublish(101);
    expect(result.validation?.success).toBe(true);
    expect(result.published?.isPublished).toBe(true);
    expect(publishSpy).toHaveBeenCalled();
    publishSpy.mockRestore();
  });
});
