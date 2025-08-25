import { describe, it, expect } from 'vitest'
import { screen } from '@testing-library/react'
import { rbacRender } from '../utils/rbac-test-utils.tsx' // ✅ Explicit extension
import { createMockPermissionContext } from '../utils/test-utils.tsx' // ✅ Explicit extension

describe('RBAC Usage Examples', () => {
  describe('Basic Permission Checks', () => {
    it('should demonstrate basic permission checking', () => {
      const adminContext = createMockPermissionContext('admin')
      const userContext = createMockPermissionContext('user')

      // Admin should have user management permissions
      expect(adminContext.hasPermission('users.all')).toBe(true)
      expect(adminContext.hasPermission('roles.all')).toBe(true)

      // Regular user should not have admin permissions
      expect(userContext.hasPermission('users.all')).toBe(false)
      expect(userContext.hasPermission('roles.all')).toBe(false)

      // Both should have basic view permissions
      expect(adminContext.hasPermission('users.view')).toBe(true)
      expect(userContext.hasPermission('users.view')).toBe(true)
    })

    it('should demonstrate role hierarchy checks', () => {
      const adminContext = createMockPermissionContext('admin')
      const managerContext = createMockPermissionContext('manager')
      const userContext = createMockPermissionContext('user')

      // Test hierarchy levels
      expect(adminContext.getRoleHierarchy()).toBe(3)
      expect(managerContext.getRoleHierarchy()).toBe(2)
      expect(userContext.getRoleHierarchy()).toBe(1)

      // Test admin checks
      expect(adminContext.isAdmin()).toBe(true)
      expect(managerContext.isAdmin()).toBe(false)
      expect(userContext.isAdmin()).toBe(false)
    })

    it('should demonstrate multi-role user handling', () => {
      const multiRoleContext = createMockPermissionContext('multiRole')
      const userRoles = multiRoleContext.getUserRoles()

      // Should have multiple roles
      expect(Array.isArray(userRoles)).toBe(true)
      expect(userRoles).toContain('Admin')
      expect(userRoles).toContain('User')

      // Should have admin-level permissions
      expect(multiRoleContext.isAdmin()).toBe(true)
      expect(multiRoleContext.hasPermission('users.all')).toBe(true)
    })
  })

  describe('Component Testing Examples', () => {
    const TestComponent = () => (
      <div>
        <div data-testid="public-content">Public Content</div>
        <div data-testid="admin-content">Admin Only Content</div>
        <div data-testid="user-content">User Content</div>
      </div>
    )

    it('should render components with different roles', () => {
      // Test as admin
      rbacRender.asAdmin(<TestComponent />)
      expect(screen.getByTestId('public-content')).toBeInTheDocument()
      expect(screen.getByTestId('admin-content')).toBeInTheDocument()
      expect(screen.getByTestId('user-content')).toBeInTheDocument()
    })

    it('should render components as regular user', () => {
      rbacRender.asUser(<TestComponent />)
      expect(screen.getByTestId('public-content')).toBeInTheDocument()
      expect(screen.getByTestId('user-content')).toBeInTheDocument()
      // Admin content would be hidden in real components with permission checks
    })

    it('should render components as viewer', () => {
      rbacRender.asViewer(<TestComponent />)
      expect(screen.getByTestId('public-content')).toBeInTheDocument()
      // Other content would be hidden in real components with permission checks
    })
  })

  describe('System vs Tenant Role Examples', () => {
    it('should distinguish between system and tenant roles', () => {
      const systemAdminContext = createMockPermissionContext('systemAdmin')
      const tenantAdminContext = createMockPermissionContext('admin')

      // System admin should be marked as system role
      expect(systemAdminContext.isSystemAdmin()).toBe(true)
      expect(tenantAdminContext.isSystemAdmin()).toBe(false)

      // Both should be admins, but different levels
      expect(systemAdminContext.isAdmin()).toBe(true)
      expect(tenantAdminContext.isAdmin()).toBe(true)

      // System admin should have higher hierarchy
      expect(systemAdminContext.getRoleHierarchy()).toBeGreaterThan(
        tenantAdminContext.getRoleHierarchy()
      )
    })

    it('should handle super admin permissions', () => {
      const superAdminContext = createMockPermissionContext('superAdmin')

      expect(superAdminContext.isSuperAdmin()).toBe(true)
      expect(superAdminContext.isSystemAdmin()).toBe(true)
      expect(superAdminContext.isAdmin()).toBe(true)

      // Should have the highest hierarchy level
      expect(superAdminContext.getRoleHierarchy()).toBe(5)
    })
  })
})
