/*************************************************************************************************
 * mockPermissionContext (TODO)
 *
 * GOAL:
 *  Provide reusable permission + role mocking without duplicating vi.mock blocks
 *  across operations / component tests.
 *
 * INTENDED API (example):
 *  setupMockPermissions({
 *    roles: ['Admin'],
 *    permissions: ['workflow.view', 'workflow.publish']
 *  });
 *
 * IMPLEMENTATION PLAN:
 *  1. Export a function that registers vi.mock('@/contexts/PermissionContext', ...).
 *  2. Accept overrides for hasPermission / hasAny / hasAll return values.
 *  3. Optionally integrate with mockAuth / mockTenant to keep a single call site.
 *
 * STATUS:
 *  Placeholder only. No runtime logic executed now.
 *************************************************************************************************/

export interface MockPermissionConfig {
  roles?: string[]
  permissions?: string[]
  grantAll?: boolean
}

export function setupMockPermissions(_config?: MockPermissionConfig) {
  throw new Error('setupMockPermissions not implemented. See TODO in mockPermissionContext.ts')
}
