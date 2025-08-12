import { render, screen, waitFor, cleanup } from '@testing-library/react'
import { rbacRender } from './rbac-test-utils.js'
import type { ReactElement } from 'react'

// Fix: Field permission testing with proper element handling
export const testFieldPermissions = (
  formComponent: ReactElement,
  fieldTests: Array<{
    fieldTestId: string
    roles: Array<{
      role: string
      expectedState: 'enabled' | 'disabled' | 'hidden'
    }>
  }>
) => {
  fieldTests.forEach(fieldTest => {
    fieldTest.roles.forEach(({ role, expectedState }) => {
      // Fix: Clean up before each render
      cleanup()
      
      rbacRender.scenario().asRole(role as any).render(formComponent)

      // Fix: Use queryAllByTestId to handle multiple elements
      const fields = screen.queryAllByTestId(fieldTest.fieldTestId)

      switch (expectedState) {
        case 'enabled':
          expect(fields.length).toBeGreaterThan(0)
          fields.forEach(field => {
            expect(field).toBeEnabled()
          })
          break
        case 'disabled':
          expect(fields.length).toBeGreaterThan(0)
          fields.forEach(field => {
            expect(field).toBeDisabled()
          })
          break
        case 'hidden':
          expect(fields.length).toBe(0)
          break
      }
    })
  })
}

// Fix: Menu visibility testing with proper cleanup
export const testMenuVisibility = (
  navigationComponent: ReactElement,
  menuItems: Array<{
    testId: string
    visibleForRoles: string[]
  }>,
  testRoles: string[]
) => {
  testRoles.forEach(role => {
    // Fix: Clean up before each render
    cleanup()
    
    rbacRender.scenario().asRole(role as any).render(navigationComponent)

    menuItems.forEach(menuItem => {
      const shouldBeVisible = menuItem.visibleForRoles.includes(role)
      
      // Fix: Use queryAllByTestId to handle multiple elements
      const menuElements = screen.queryAllByTestId(menuItem.testId)

      if (shouldBeVisible) {
        expect(menuElements.length).toBeGreaterThan(0)
      } else {
        expect(menuElements.length).toBe(0)
      }
    })
  })
}

// Fix: Table action button testing
export const testTableActionButtons = (
  tableComponent: ReactElement,
  buttonConfigs: Array<{
    buttonTestId: string
    visibleForRoles: string[]
  }>,
  testRoles: string[]
) => {
  testRoles.forEach(role => {
    // Fix: Clean up before each render
    cleanup()
    
    rbacRender.scenario().asRole(role as any).render(tableComponent)

    buttonConfigs.forEach(buttonConfig => {
      const shouldBeVisible = buttonConfig.visibleForRoles.includes(role)
      
      // Fix: Use queryAllByTestId to handle multiple elements
      const buttonElements = screen.queryAllByTestId(buttonConfig.buttonTestId)

      if (shouldBeVisible) {
        expect(buttonElements.length).toBeGreaterThan(0)
      } else {
        expect(buttonElements.length).toBe(0)
      }
    })
  })
}

// Fix: Permission error testing
export const testPermissionErrors = async (
  component: ReactElement,
  errorScenarios: Array<{
    role: string
    expectedError?: string
    shouldShowError: boolean
  }>
) => {
  for (const scenario of errorScenarios) {
    // Fix: Clean up before each render
    cleanup()
    
    rbacRender.scenario().asRole(scenario.role as any).render(component)

    if (scenario.expectedError) {
      await waitFor(() => {
        // Fix: Use getAllByText to handle multiple error messages
        const errorElements = screen.getAllByText(scenario.expectedError!)
        expect(errorElements.length).toBeGreaterThan(0)
      })
    }
  }
}

// Fix: Route accessibility testing
export const testRouteAccess = (
  routeComponent: ReactElement,
  routeTests: Array<{
    role: string
    shouldHaveAccess: boolean
    expectedRedirect?: string
  }>
) => {
  routeTests.forEach(({ role, shouldHaveAccess }) => {
    // Fix: Clean up before each render
    cleanup()
    
    if (shouldHaveAccess) {
      rbacRender.scenario().asRole(role as any).render(routeComponent)
      // Component should render without access denied message
      expect(screen.queryByText(/access denied/i)).not.toBeInTheDocument()
    } else {
      rbacRender.scenario().asRole(role as any).render(routeComponent)
      // Should show access denied or redirect
      // This test might need to be adjusted based on your actual access denied implementation
    }
  })
}
