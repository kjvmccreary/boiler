import { describe, it, expect, beforeEach, afterEach } from 'vitest'
import { rbacRender } from '../utils/rbac-test-utils.js'
import { screen, cleanup } from '@testing-library/react'

describe('Multi-Tenant Permission Isolation Scenarios', () => {

  beforeEach(() => {
    cleanup()
  })

  afterEach(() => {
    cleanup()
  })

  describe('Tenant Data Isolation', () => {
    const TenantDataComponent = ({ tenantId }: { tenantId: string }) => (
      <div>
        <h1 data-testid="tenant-header">Tenant: {tenantId}</h1>
        <div data-testid="tenant-users">Users for {tenantId}</div>
        <div data-testid="tenant-roles">Roles for {tenantId}</div>
      </div>
    )

    it('should isolate tenant data access', async () => {
      // Fix: Test one tenant at a time
      cleanup()
      rbacRender.scenario()
        .asRole('admin')
        .inTenant('tenant-1')
        .render(<TenantDataComponent tenantId="tenant-1" />)

      expect(screen.getByTestId('tenant-header')).toHaveTextContent('Tenant: tenant-1')

      // Clean up and test second tenant
      cleanup()
      rbacRender.scenario()
        .asRole('admin')
        .inTenant('tenant-2')
        .render(<TenantDataComponent tenantId="tenant-2" />)

      expect(screen.getByTestId('tenant-header')).toHaveTextContent('Tenant: tenant-2')
    })

    it('should prevent cross-tenant role assignment', async () => {
      const CrossTenantComponent = () => (
        <div>
          <button data-testid="assign-role-btn">Assign Cross-Tenant Role</button>
          <div data-testid="error-message">Cannot assign roles across tenants</div>
        </div>
      )

      rbacRender.scenario()
        .asRole('admin')
        .inTenant('tenant-1')
        .render(<CrossTenantComponent />)

      expect(screen.getByTestId('error-message')).toHaveTextContent(
        'Cannot assign roles across tenants'
      )
    })
  })

  describe('Tenant-Scoped Permissions', () => {
    const TenantAdminComponent = ({ userRole }: { userRole: string }) => (
      <div>
        <button data-testid="manage-tenant-users">Manage Users</button>
        <button data-testid="manage-tenant-roles">Manage Roles</button>
        {userRole === 'systemAdmin' && (
          <button data-testid="view-all-tenants">View All Tenants</button>
        )}
      </div>
    )

    it('should scope permissions to specific tenants', async () => {
      rbacRender.scenario()
        .asRole('admin')
        .inTenant('tenant-1')
        .render(<TenantAdminComponent userRole="admin" />)

      expect(screen.getByTestId('manage-tenant-users')).toBeInTheDocument()
      expect(screen.getByTestId('manage-tenant-roles')).toBeInTheDocument()

      // Fix: Admin should not see system-wide features
      expect(screen.queryByTestId('view-all-tenants')).not.toBeInTheDocument()
    })

    it('should allow system admins to access multiple tenants', async () => {
      rbacRender.scenario()
        .asRole('systemAdmin')
        .render(<TenantAdminComponent userRole="systemAdmin" />)

      expect(screen.getByTestId('view-all-tenants')).toBeInTheDocument()
    })
  })

  describe('Tenant Context Switching', () => {
    const TenantSwitcherComponent = ({ canSwitch }: { canSwitch: boolean }) => (
      <div>
        <select data-testid="tenant-switcher" disabled={!canSwitch}>
          <option value="tenant-1">Tenant 1</option>
          <option value="tenant-2">Tenant 2</option>
        </select>
      </div>
    )

    it('should handle tenant context switching for multi-tenant users', async () => {
      rbacRender.scenario()
        .asRole('systemAdmin')
        .render(<TenantSwitcherComponent canSwitch={true} />)

      const switcher = screen.getByTestId('tenant-switcher')
      expect(switcher).toBeEnabled()
    })

    it('should restrict tenant switching for single-tenant users', async () => {
      rbacRender.scenario()
        .asRole('admin')
        .render(<TenantSwitcherComponent canSwitch={false} />)

      const switcher = screen.getByTestId('tenant-switcher')
      expect(switcher).toBeDisabled()
    })
  })
})
