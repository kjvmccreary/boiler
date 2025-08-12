import { screen, waitFor } from '@testing-library/react'
import { rbacRender, rbacUserEvent } from './rbac-test-utils.js'

// 🔧 NEW: Form Testing with RBAC
export const rbacFormHelpers = {
  // Test form field permissions
  async testFieldPermissions(
    formComponent: React.ReactElement,
    fieldTests: Array<{
      fieldTestId: string
      permission: string
      expectedStates: {
        [role: string]: 'enabled' | 'disabled' | 'hidden'
      }
    }>
  ) {
    for (const fieldTest of fieldTests) {
      for (const [role, expectedState] of Object.entries(fieldTest.expectedStates)) {
        rbacRender.scenario().asRole(role as any).render(formComponent)
        
        const field = screen.queryByTestId(fieldTest.fieldTestId)
        
        switch (expectedState) {
          case 'hidden':
            expect(field).not.toBeInTheDocument()
            break
          case 'disabled':
            expect(field).toBeInTheDocument()
            expect(field).toBeDisabled()
            break
          case 'enabled':
            expect(field).toBeInTheDocument()
            expect(field).toBeEnabled()
            break
        }
      }
    }
  },

  // Test form submission permissions
  async testSubmitPermissions(
    formComponent: React.ReactElement,
    submitButtonTestId: string,
    requiredPermission: string
  ) {
    const roles = ['superAdmin', 'admin', 'manager', 'user', 'viewer'] as const
    
    for (const role of roles) {
      const { user } = rbacUserEvent.setupForRole(role)
      rbacRender.scenario().asRole(role).render(formComponent)
      
      const submitButton = screen.getByTestId(submitButtonTestId)
      const context = rbacRender.scenario().asRole(role).getPermissionContext()
      
      if (context.hasPermission(requiredPermission)) {
        expect(submitButton).toBeEnabled()
        await user.click(submitButton)
        // Add assertions for successful submission
      } else {
        expect(submitButton).toBeDisabled()
      }
    }
  }
}

// 🔧 NEW: Navigation Testing with RBAC
export const rbacNavigationHelpers = {
  // Test menu item visibility
  async testMenuVisibility(
    menuComponent: React.ReactElement,
    menuItems: Array<{
      testId: string
      requiredPermission?: string
      requiredRole?: string
      visibleForRoles: string[]
    }>
  ) {
    const allRoles = ['superAdmin', 'systemAdmin', 'admin', 'manager', 'user', 'viewer'] as const
    
    for (const role of allRoles) {
      rbacRender.scenario().asRole(role).render(menuComponent)
      
      for (const menuItem of menuItems) {
        const shouldBeVisible = menuItem.visibleForRoles.includes(role)
        const menuElement = screen.queryByTestId(menuItem.testId)
        
        if (shouldBeVisible) {
          expect(menuElement).toBeInTheDocument()
        } else {
          expect(menuElement).not.toBeInTheDocument()
        }
      }
    }
  }
}

// 🔧 NEW: Data Table Testing with RBAC
export const rbacTableHelpers = {
  // Test action button visibility in tables
  async testTableActionButtons(
    tableComponent: React.ReactElement,
    actionButtons: Array<{
      testId: string
      permission: string
      visibleForRoles: string[]
    }>
  ) {
    const allRoles = ['superAdmin', 'systemAdmin', 'admin', 'manager', 'user', 'viewer'] as const
    
    for (const role of allRoles) {
      rbacRender.scenario().asRole(role).render(tableComponent)
      
      for (const button of actionButtons) {
        const shouldBeVisible = button.visibleForRoles.includes(role)
        const buttonElements = screen.queryAllByTestId(button.testId)
        
        if (shouldBeVisible) {
          expect(buttonElements.length).toBeGreaterThan(0)
        } else {
          expect(buttonElements.length).toBe(0)
        }
      }
    }
  },

  // Test bulk actions
  async testBulkActions(
    tableComponent: React.ReactElement,
    bulkActions: Array<{
      testId: string
      permission: string
      minimumRole: string
    }>
  ) {
    const roleHierarchy = ['viewer', 'user', 'manager', 'admin', 'systemAdmin', 'superAdmin']
    
    for (const action of bulkActions) {
      const minimumRoleIndex = roleHierarchy.indexOf(action.minimumRole)
      
      for (let i = 0; i < roleHierarchy.length; i++) {
        const role = roleHierarchy[i]
        rbacRender.scenario().asRole(role as any).render(tableComponent)
        
        const actionElement = screen.queryByTestId(action.testId)
        
        if (i >= minimumRoleIndex) {
          expect(actionElement).toBeInTheDocument()
        } else {
          expect(actionElement).not.toBeInTheDocument()
        }
      }
    }
  }
}

// 🔧 NEW: Modal/Dialog Testing with RBAC
export const rbacModalHelpers = {
  // Test modal access permissions
  async testModalAccess(
    triggerComponent: React.ReactElement,
    triggerTestId: string,
    modalTestId: string,
    requiredPermission: string
  ) {
    const roles = ['superAdmin', 'admin', 'manager', 'user', 'viewer'] as const
    
    for (const role of roles) {
      const { user, can } = rbacUserEvent.setupForRole(role)
      rbacRender.scenario().asRole(role).render(triggerComponent)
      
      const trigger = screen.getByTestId(triggerTestId)
      
      if (can(requiredPermission)) {
        expect(trigger).toBeEnabled()
        await user.click(trigger)
        
        await waitFor(() => {
          expect(screen.getByTestId(modalTestId)).toBeInTheDocument()
        })
      } else {
        expect(trigger).toBeDisabled()
      }
    }
  }
}

// 🔧 NEW: Error State Testing with RBAC
export const rbacErrorHelpers = {
  // Test permission-based error messages
  async testPermissionErrors(
    component: React.ReactElement,
    scenarios: Array<{
      role: string
      expectedError?: string
      expectedSuccess?: boolean
    }>
  ) {
    for (const scenario of scenarios) {
      rbacRender.scenario().asRole(scenario.role as any).render(component)
      
      if (scenario.expectedError) {
        await waitFor(() => {
          expect(screen.getByText(scenario.expectedError!)).toBeInTheDocument()
        })
      } else if (scenario.expectedSuccess) {
        await waitFor(() => {
          expect(screen.queryByText(/error|forbidden|unauthorized/i)).not.toBeInTheDocument()
        })
      }
    }
  }
}

// 🔧 NEW: Export all helpers
export const rbacTestHelpers = {
  form: rbacFormHelpers,
  navigation: rbacNavigationHelpers,
  table: rbacTableHelpers,
  modal: rbacModalHelpers,
  error: rbacErrorHelpers
}

export default rbacTestHelpers
