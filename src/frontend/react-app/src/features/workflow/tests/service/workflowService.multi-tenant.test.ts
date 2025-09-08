import { describe, it, expect } from 'vitest';
import { workflowService } from '@/services/workflow.service';
import approvalBasic from '@/test/fixtures/workflow/approval.basic.json';

/**
 * Multi-tenant isolation test:
 * Relies on apiClient test-mode logic reading localStorage 'test_tenant_id'
 * and injecting X-Tenant-Id header.
 *
 * Verifies:
 *  - Definitions are isolated per tenant
 *  - Tasks created in one tenant are not visible in another
 */
describe('WorkflowService / Multi-Tenant Isolation', () => {
  async function definitionNames(): Promise<string[]> {
    const defs = await workflowService.getDefinitions({ includeArchived: true });
    return defs.map(d => d.name).sort();
  }

  async function taskCount(): Promise<number> {
    const tasks = await workflowService.getTasks({ pageSize: 50 });
    return tasks.length;
  }

  it('isolates definitions and tasks between tenants', async () => {
    // ---------- Tenant A ----------
    localStorage.setItem('test_tenant_id', 'tenantA');

    const draftA = await workflowService.createDraft({
      name: 'TenantA Flow',
      description: 'Tenant A Definition',
      jsonDefinition: JSON.stringify(approvalBasic)
    });

    await workflowService.publishDefinition(draftA.id);
    await workflowService.startInstance({ workflowDefinitionId: draftA.id });

    const defsA1 = await definitionNames();
    expect(defsA1).toEqual(['TenantA Flow']);

    const tasksA1 = await taskCount();
    expect(tasksA1).toBeGreaterThan(0);

    // ---------- Tenant B (should be isolated) ----------
    localStorage.setItem('test_tenant_id', 'tenantB');

    const defsB0 = await definitionNames();
    expect(defsB0).toEqual([]);

    const draftB = await workflowService.createDraft({
      name: 'TenantB Flow',
      description: 'Tenant B Definition',
      jsonDefinition: JSON.stringify(approvalBasic)
    });

    await workflowService.publishDefinition(draftB.id);
    await workflowService.startInstance({ workflowDefinitionId: draftB.id });

    const defsB1 = await definitionNames();
    expect(defsB1).toEqual(['TenantB Flow']);

    const tasksB = await taskCount();
    expect(tasksB).toBeGreaterThan(0);

    // ---------- Back to Tenant A (unchanged) ----------
    localStorage.setItem('test_tenant_id', 'tenantA');

    const defsA2 = await definitionNames();
    expect(defsA2).toEqual(['TenantA Flow']);

    const tasksA2 = await taskCount();
    expect(tasksA2).toBe(tasksA1); // Should not have been affected by Tenant B operations
  });
});
