import React from 'react'
import { describe, it, expect, vi } from 'vitest'
import { screen, waitFor } from '@testing-library/react'
import { rbacRender, rbacAssert, rbacBatch, rbacScenarios } from '../utils/rbac-test-utils.js'
import { rbacTestHelpers } from '../utils/rbac-component-helpers.js'

// Example component imports (replace with your actual components)
// import { UserList } from '@/components/users/UserList.js'
// import { RoleEditor } from '@/components/roles/RoleEditor.js'
// import { CanAccess } from '@/components/authorization/CanAccess.js'

describe('RBAC Test Utilities - Usage Examples', () => {
  
  // 🔧 EXAMPLE 1: Basic Role-Based Rendering
  describe('Basic Role Rendering', () => {
    it('should render differently for different roles', () => {
      const TestComponent = () => (
        <div>
          <button data-testid="admin-button">Admin Only</button>
          <button data-testid="user-button">All Users</button>
        </div>
      )

      // Test as admin
      rbacRender.asAdmin(<TestComponent />)
      expect(screen.getByTestId('admin-button')).toBeInTheDocument()
      expect(screen.getByTestId('user-button')).toBeInTheDocument()

      // Test as regular user
      rbacRender.asUser(<TestComponent />)
      expect(screen.getByTestId('user-button')).toBeInTheDocument()
      // Admin button would be hidden by CanAccess component
    })
  })

  // 🔧 EXAMPLE 2: Scenario Builder Pattern
  describe('Scenario Builder Pattern', () => {
    it('should support complex scenarios', () => {
      const TestComponent = () => <div data-testid="test">Test</div>

      // Complex scenario with custom permissions
      rbacRender.scenario()
        .asRole('manager')
        .withCustomPermissions(['users.delete', 'roles.create'])
        .inTenant('special-tenant')
        .render(<TestComponent />)

      expect(screen.getByTestId('test')).toBeInTheDocument()
    })
  })

  // 🔧 EXAMPLE 3: Permission Assertions
  describe('Permission Assertions', () => {
    it('should assert element visibility based on permissions', async () => {
      const TestComponent = () => (
        <div>
          <button data-testid="delete-button">Delete</button>
        </div>
      )

      rbacRender.asAdmin(<TestComponent />)
      
      // Assert that admin can see delete button
      await rbacAssert.expectElementIfPermission(
        'users.delete', 
        'admin', 
        'delete-button'
      )
    })
  })

  // 🔧 EXAMPLE 4: Batch Testing All Roles
  describe('Batch Role Testing', () => {
    it('should test all roles against component', async () => {
      const TestComponent = (role: string) => (
        <div>
          <span data-testid="role-display">{role}</span>
          <button data-testid="admin-action">Admin Action</button>
          <button data-testid="user-action">User Action</button>
        </div>
      )

      await rbacBatch.testAllRoles(
        (role) => <TestComponent role={role} />,
        [
          {
            role: 'admin',
            expectVisible: ['role-display', 'admin-action', 'user-action'],
            expectHidden: []
          },
          {
            role: 'user',
            expectVisible: ['role-display', 'user-action'],
            expectHidden: ['admin-action']
          },
          {
            role: 'viewer',
            expectVisible: ['role-display'],
            expectHidden: ['admin-action', 'user-action']
          }
        ]
      )
    })
  })

  // 🔧 EXAMPLE 5: Permission Matrix Testing
  describe('Permission Matrix Testing', () => {
    it('should test permission matrix', () => {
      rbacBatch.testPermissionMatrix([
        {
          permission: 'users.delete',
          roles: [
            { role: 'superAdmin', shouldHave: true },
            { role: 'admin', shouldHave: true },
            { role: 'manager', shouldHave: false },
            { role: 'user', shouldHave: false },
            { role: 'viewer', shouldHave: false }
          ]
        },
        {
          permission: 'users.view',
          roles: [
            { role: 'superAdmin', shouldHave: true },
            { role: 'admin', shouldHave: true },
            { role: 'manager', shouldHave: true },
            { role: 'user', shouldHave: true },
            { role: 'viewer', shouldHave: true }
          ]
        }
      ])
    })
  })

  // 🔧 EXAMPLE 6: Form Testing with RBAC
  describe('Form Testing', () => {
    it('should test form field permissions', async () => {
      const TestForm = () => (
        <form>
          <input data-testid="name-field" />
          <input data-testid="admin-field" />
          <button data-testid="submit-button">Submit</button>
        </form>
      )

      await rbacTestHelpers.form.testFieldPermissions(
        <TestForm />,
        [
          {
            fieldTestId: 'name-field',
            permission: 'users.edit',
            expectedStates: {
              admin: 'enabled',
              manager: 'enabled',
              user: 'disabled',
              viewer: 'disabled'
            }
          },
          {
            fieldTestId: 'admin-field',
            permission: 'admin.settings',
            expectedStates: {
              admin: 'enabled',
              manager: 'hidden',
              user: 'hidden',
              viewer: 'hidden'
            }
          }
        ]
      )
    })
  })

  // 🔧 EXAMPLE 7: Navigation Testing
  describe('Navigation Testing', () => {
    it('should test menu visibility', async () => {
      const TestMenu = () => (
        <nav>
          <a data-testid="users-link">Users</a>
          <a data-testid="roles-link">Roles</a>
          <a data-testid="settings-link">Settings</a>
        </nav>
      )

      await rbacTestHelpers.navigation.testMenuVisibility(
        <TestMenu />,
        [
          {
            testId: 'users-link',
            requiredPermission: 'users.view',
            visibleForRoles: ['admin', 'manager', 'user']
          },
          {
            testId: 'roles-link',
            requiredPermission: 'roles.view',
            visibleForRoles: ['admin', 'manager']
          },
          {
            testId: 'settings-link',
            requiredPermission: 'admin.settings',
            visibleForRoles: ['admin']
          }
        ]
      )
    })
  })

  // 🔧 EXAMPLE 8: Common Scenarios
  describe('Common Scenarios', () => {
    it('should use predefined scenarios', () => {
      const deleteScenario = rbacScenarios.createPermissionScenario('users.delete')
      
      expect(deleteScenario.permission).toBe('users.delete')
      expect(deleteScenario.testCases).toHaveLength(6) // All role types
      
      // Test that admins have delete permission
      const adminCase = deleteScenario.testCases.find(tc => tc.role === 'admin')
      expect(adminCase?.hasPermission).toBe(true)
      
      // Test that viewers don't have delete permission
      const viewerCase = deleteScenario.testCases.find(tc => tc.role === 'viewer')
      expect(viewerCase?.hasPermission).toBe(false)
    })
  })

  // 🔧 EXAMPLE 9: Multi-Role User Testing
  describe('Multi-Role User Testing', () => {
    it('should test users with multiple roles', () => {
      const TestComponent = () => (
        <div>
          <span data-testid="manager-feature">Manager Feature</span>
          <span data-testid="user-feature">User Feature</span>
        </div>
      )

      // Test user with both Manager and User roles
      rbacRender.asMultiRole(<TestComponent />)
      
      expect(screen.getByTestId('manager-feature')).toBeInTheDocument()
      expect(screen.getByTestId('user-feature')).toBeInTheDocument()
    })
  })

  // 🔧 EXAMPLE 10: Error State Testing
  describe('Error State Testing', () => {
    it('should test permission-based errors', async () => {
      const TestComponent = ({ role }: { role: string }) => (
        <div>
          {role === 'viewer' ? (
            <div>Access Denied: Insufficient permissions</div>
          ) : (
            <div>Welcome, {role}!</div>
          )}
        </div>
      )

      await rbacTestHelpers.error.testPermissionErrors(
        <TestComponent role="viewer" />,
        [
          {
            role: 'admin',
            expectedSuccess: true
          },
          {
            role: 'viewer',
            expectedError: 'Access Denied: Insufficient permissions'
          }
        ]
      )
    })
  })
})
