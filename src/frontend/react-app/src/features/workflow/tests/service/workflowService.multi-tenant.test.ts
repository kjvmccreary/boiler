import { describe, it, expect } from 'vitest';
import { workflowService } from '@/services/workflow.service';
import { apiClient } from '@/services/api.client';
import approvalBasic from '@/test/fixtures/workflow/approval.basic.json';

/**
 * Multi-tenant isolation test:
 * Uses localStorage test_tenant_id -> injected as X-Tenant-Id header (ApiClient test-mode logic).
 * Ensures definitions, instances, tasks do not leak across tenants.
 */
describe('WorkflowService / Multi-Tenant Isolation', () => {
  async function listDefinitions(): Promise<any[]> {
    const resp = await apiClient.get('/api/workflow/definitions');
    return resp.data as any[];
  }

  async function listTasks(): Promise<any[]> {
    const resp = await apiClient.get('/api/workflow/tasks');
    return resp.data as any[];
  }

  it('isolates definitions and tasks between tenants', async () => {
    // Tenant A create + publish + start
    localStorage.setItem('test_tenant_id', 'tenantA');
    const draftA = await workflowService.createDraft({
      key: 'defA',
      name: 'Def A',
      description: 'Tenant A',
      jsonDefinition: approvalBasic
    } as any);
    await workflowService.publishDefinition((draftA as any).id);
    await workflowService.startInstance({ definitionKey: 'defA' } as any);

    const defsA1 = await listDefinitions();
    expect(defsA1.map(d => d.key)).toEqual(['defA']);
    const tasksA1 = await listTasks();
    expect(tasksA1.length).toBeGreaterThan(0);

    // Tenant B should see nothing initially
    localStorage.setItem('test_tenant_id', 'tenantB');
    const defsB0 = await listDefinitions();
    expect(defsB0.length).toBe(0);

    // Create in B
    const draftB = await workflowService.createDraft({
      key: 'defB',
      name: 'Def B',
      description: 'Tenant B',
      jsonDefinition: approvalBasic
    } as any);
    await workflowService.publishDefinition((draftB as any).id);
    await workflowService.startInstance({ definitionKey: 'defB' } as any);

    const defsB1 = await listDefinitions();
    expect(defsB1.map(d => d.key)).toEqual(['defB']);
    const tasksB = await listTasks();
    expect(tasksB.length).toBeGreaterThan(0);

    // Back to A: original state unchanged by B
    localStorage.setItem('test_tenant_id', 'tenantA');
    const defsA2 = await listDefinitions();
    expect(defsA2.map(d => d.key)).toEqual(['defA']);
    const tasksA2 = await listTasks();
    expect(tasksA2.length).toBe(tasksA1.length);
  });
});
