import React from 'react'
import { describe, it, expect, beforeEach } from 'vitest'
import { render, screen, cleanup } from '@testing-library/react'
import { createMockPermissionContext } from '@/test/utils/test-utils'

// Helper function to test with all roles individually
const testWithAllRoles = (testFn: (roleName: string, context: any) => void) => {
  const roles = ['superAdmin', 'systemAdmin', 'admin', 'manager', 'user', 'viewer', 'multiRole'] as const

  roles.forEach(role => {
    const context = createMockPermissionContext(role)
    testFn(role, context)
    cleanup() // ✅ Clean up DOM between role tests
  })
}

describe('RBAC Test Utilities - Usage Examples', () => {
  beforeEach(() => {
    cleanup() // ✅ Ensure clean state
  })

  describe('Basic Role Rendering', () => {
    it('should render differently for different roles', () => {
      testWithAllRoles((roleName, context) => {
        const TestComponent = () => (
          <div data-testid={`role-test-${roleName}`}> {/* ✅ Unique test ID */}
            <h1>Dashboard</h1>
            {context.isAdmin() && <div data-testid={`admin-panel-${roleName}`}>Admin Panel</div>}
            {context.hasPermission('users.view') && <div data-testid={`users-section-${roleName}`}>Users</div>}
            <div data-testid={`user-role-${roleName}`}>{roleName}</div>
          </div>
        )

        render(<TestComponent />)

        // ✅ Use role-specific selectors
        expect(screen.getByTestId(`role-test-${roleName}`)).toBeInTheDocument()
        expect(screen.getByTestId(`user-role-${roleName}`)).toHaveTextContent(roleName)

        if (context.isAdmin()) {
          expect(screen.getByTestId(`admin-panel-${roleName}`)).toBeInTheDocument()
        } else {
          expect(screen.queryByTestId(`admin-panel-${roleName}`)).not.toBeInTheDocument()
        }
      })
    })
  })

  describe('Batch Role Testing', () => {
    it('should test all roles against component', () => {
      const results: Record<string, boolean> = {}

      testWithAllRoles((roleName, context) => {
        results[roleName] = context.hasPermission('users.view')
      })

      // ✅ These should all be true now with updated permissions
      expect(results.superAdmin).toBe(true)
      expect(results.systemAdmin).toBe(true)
      expect(results.admin).toBe(true)
      expect(results.manager).toBe(true)
      expect(results.user).toBe(true)
      expect(results.viewer).toBe(true)
      expect(results.multiRole).toBe(true)
    })
  })

  describe('Form Testing', () => {
    it('should test form field permissions', () => {
      testWithAllRoles((roleName, context) => {
        const TestForm = () => (
          <div data-testid={`form-test-${roleName}`}> {/* ✅ Unique container */}
            <form>
              <input data-testid={`name-field-${roleName}`} />
              {context.hasPermission('users.edit') && (
                <input data-testid={`admin-field-${roleName}`} />
              )}
              {context.hasPermission('users.delete') && (
                <button data-testid={`delete-button-${roleName}`}>Delete</button>
              )}
            </form>
          </div>
        )

        render(<TestForm />)

        // ✅ Use role-specific selectors
        expect(screen.getByTestId(`name-field-${roleName}`)).toBeInTheDocument()

        if (context.hasPermission('users.edit')) {
          expect(screen.getByTestId(`admin-field-${roleName}`)).toBeInTheDocument()
        } else {
          expect(screen.queryByTestId(`admin-field-${roleName}`)).not.toBeInTheDocument()
        }
      })
    })
  })

  describe('Navigation Testing', () => {
    it('should test menu visibility', () => {
      const navigationItems = [
        { id: 'dashboard', label: 'Dashboard', requiredPermission: 'dashboard.view' },
        { id: 'users', label: 'Users', requiredPermission: 'users.view' },
        { id: 'admin', label: 'Admin', requiredPermission: 'users.all' }
      ]

      testWithAllRoles((roleName, context) => {
        const TestNavigation = () => (
          <div data-testid={`nav-test-${roleName}`}> {/* ✅ Unique container */}
            <nav>
              {navigationItems.map(item =>
                context.hasPermission(item.requiredPermission) ? (
                  <a key={item.id} data-testid={`${item.id}-link-${roleName}`} href={`/${item.id}`}>
                    {item.label}
                  </a>
                ) : null
              )}
            </nav>
          </div>
        )

        render(<TestNavigation />)

        for (const item of navigationItems) {
          // ✅ Use role-specific selectors
          const menuElement = screen.queryByTestId(`${item.id}-link-${roleName}`)

          if (context.hasPermission(item.requiredPermission)) {
            expect(menuElement).toBeInTheDocument()
          } else {
            expect(menuElement).not.toBeInTheDocument()
          }
        }
      })
    })
  })

  describe('Error State Testing', () => {
    it('should test permission-based errors', () => {
      testWithAllRoles((roleName, context) => {
        const TestComponent = () => {
          const canDelete = context.hasPermission('users.delete')

          return (
            <div data-testid={`error-test-${roleName}`}> {/* ✅ Unique container */}
              {!canDelete && (
                <div data-testid={`permission-error-${roleName}`}>
                  You don't have permission to delete users
                </div>
              )}
              {canDelete && (
                <button data-testid={`delete-action-${roleName}`}>Delete User</button>
              )}
            </div>
          )
        }

        render(<TestComponent />)

        // ✅ Use role-specific selectors
        if (context.hasPermission('users.delete')) {
          expect(screen.getByTestId(`delete-action-${roleName}`)).toBeInTheDocument()
          expect(screen.queryByTestId(`permission-error-${roleName}`)).not.toBeInTheDocument()
        } else {
          expect(screen.getByTestId(`permission-error-${roleName}`)).toBeInTheDocument()
          expect(screen.queryByTestId(`delete-action-${roleName}`)).not.toBeInTheDocument()
        }
      })
    })
  })
})
