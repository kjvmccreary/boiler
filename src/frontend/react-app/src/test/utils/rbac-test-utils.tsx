import { type ReactElement } from 'react'
import { render, type RenderOptions, screen, waitFor } from '@testing-library/react'
import { BrowserRouter, MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import userEvent from '@testing-library/user-event'
import { AuthProvider } from '@/contexts/AuthContext.js'
import type { User } from '@/types/index.js'
import { mockUsers, mockRoles, createMockPermissionContext } from './test-utils.js'

// 🔧 NEW: Role-Based Test Configuration
export interface RoleTestConfig {
  role: keyof typeof mockRoles | 'multiRole'
  user?: User
  customPermissions?: string[]
  customRoles?: string[]
  tenantId?: string
  authState?: 'authenticated' | 'unauthenticated' | 'loading'
}

// 🔧 NEW: Test Scenario Builder
export class RBACTestScenarioBuilder {
  private config: RoleTestConfig = {
    role: 'user',
    authState: 'authenticated',
    tenantId: 'tenant-1'
  }

  static create() {
    return new RBACTestScenarioBuilder()
  }

  asRole(role: keyof typeof mockRoles | 'multiRole') {
    this.config.role = role
    return this
  }

  withUser(user: User) {
    this.config.user = user
    return this
  }

  withCustomPermissions(permissions: string[]) {
    this.config.customPermissions = permissions
    return this
  }

  withCustomRoles(roles: string[]) {
    this.config.customRoles = roles
    return this
  }

  inTenant(tenantId: string) {
    this.config.tenantId = tenantId
    return this
  }

  unauthenticated() {
    this.config.authState = 'unauthenticated'
    return this
  }

  loading() {
    this.config.authState = 'loading'
    return this
  }

  render(ui: ReactElement, options?: RenderOptions) {
    return renderWithRoleConfig(ui, this.config, options)
  }

  getPermissionContext() {
    return createMockPermissionContext(
      this.config.role, 
      this.config.customPermissions, 
      this.config.customRoles
    )
  }

  build() {
    return this.config
  }
}

// 🔧 NEW: Test Wrapper with Full RBAC Context
interface TestWrapperProps {
  children: React.ReactNode
  config: RoleTestConfig
  queryClient?: QueryClient
  initialEntries?: string[]
}

function RBACTestWrapper({ 
  children, 
  config,
  queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: 0 },
      mutations: { retry: false }
    }
  }),
  initialEntries = ['/']
}: TestWrapperProps) {
  const RouterComponent = initialEntries.length > 1 || initialEntries[0] !== '/' 
    ? ({ children }: { children: React.ReactNode }) => (
        <MemoryRouter initialEntries={initialEntries}>
          {children}
        </MemoryRouter>
      )
    : BrowserRouter

  const resolvedUser = config.user || (
    config.role === 'multiRole' 
      ? mockUsers.multiRole 
      : mockUsers[config.role as keyof typeof mockUsers]
  )

  return (
    <QueryClientProvider client={queryClient}>
      <RouterComponent>
        <AuthProvider 
          mockUser={resolvedUser}
          mockAuthState={config.authState}
          testMode={true}
        >
          {children}
        </AuthProvider>
      </RouterComponent>
    </QueryClientProvider>
  )
}

// 🔧 NEW: Role-Based Render Function
function renderWithRoleConfig(
  ui: ReactElement,
  config: RoleTestConfig,
  options?: RenderOptions
) {
  return render(ui, {
    wrapper: ({ children }) => (
      <RBACTestWrapper config={config}>
        {children}
      </RBACTestWrapper>
    ),
    ...options,
  })
}

// 🔧 NEW: Quick Role-Based Render Functions
export const rbacRender = {
  // Single role renders
  asSuperAdmin: (ui: ReactElement, options?: RenderOptions) =>
    RBACTestScenarioBuilder.create().asRole('superAdmin').render(ui, options),

  asSystemAdmin: (ui: ReactElement, options?: RenderOptions) =>
    RBACTestScenarioBuilder.create().asRole('systemAdmin').render(ui, options),

  asAdmin: (ui: ReactElement, options?: RenderOptions) =>
    RBACTestScenarioBuilder.create().asRole('admin').render(ui, options),

  asManager: (ui: ReactElement, options?: RenderOptions) =>
    RBACTestScenarioBuilder.create().asRole('manager').render(ui, options),

  asUser: (ui: ReactElement, options?: RenderOptions) =>
    RBACTestScenarioBuilder.create().asRole('user').render(ui, options),

  asViewer: (ui: ReactElement, options?: RenderOptions) =>
    RBACTestScenarioBuilder.create().asRole('viewer').render(ui, options),

  asMultiRole: (ui: ReactElement, options?: RenderOptions) =>
    RBACTestScenarioBuilder.create().asRole('multiRole').render(ui, options),

  // Special states
  unauthenticated: (ui: ReactElement, options?: RenderOptions) =>
    RBACTestScenarioBuilder.create().unauthenticated().render(ui, options),

  loading: (ui: ReactElement, options?: RenderOptions) =>
    RBACTestScenarioBuilder.create().loading().render(ui, options),

  // Custom scenarios
  withPermissions: (permissions: string[], ui: ReactElement, options?: RenderOptions) =>
    RBACTestScenarioBuilder.create().withCustomPermissions(permissions).render(ui, options),

  withRoles: (roles: string[], ui: ReactElement, options?: RenderOptions) =>
    RBACTestScenarioBuilder.create().withCustomRoles(roles).render(ui, options),

  // Builder pattern access
  scenario: () => RBACTestScenarioBuilder.create()
}

// 🔧 NEW: Permission Assertion Utilities
export const rbacAssert = {
  // Element visibility based on permissions
  async expectElementIfPermission(
    permission: string, 
    role: keyof typeof mockRoles | 'multiRole', 
    elementTestId: string
  ) {
    const context = createMockPermissionContext(role)
    const hasPermission = context.hasPermission(permission)
    
    if (hasPermission) {
      await waitFor(() => {
        expect(screen.getByTestId(elementTestId)).toBeInTheDocument()
      })
    } else {
      expect(screen.queryByTestId(elementTestId)).not.toBeInTheDocument()
    }
  },

  // Element visibility based on roles
  async expectElementIfRole(
    roleName: string, 
    userRole: keyof typeof mockRoles | 'multiRole', 
    elementTestId: string
  ) {
    const context = createMockPermissionContext(userRole)
    const hasRole = context.hasRole(roleName)
    
    if (hasRole) {
      await waitFor(() => {
        expect(screen.getByTestId(elementTestId)).toBeInTheDocument()
      })
    } else {
      expect(screen.queryByTestId(elementTestId)).not.toBeInTheDocument()
    }
  },

  // Multiple role checking
  async expectElementIfAnyRole(
    roleNames: string[], 
    userRole: keyof typeof mockRoles | 'multiRole', 
    elementTestId: string
  ) {
    const context = createMockPermissionContext(userRole)
    const hasAnyRole = context.hasAnyRole(roleNames)
    
    if (hasAnyRole) {
      await waitFor(() => {
        expect(screen.getByTestId(elementTestId)).toBeInTheDocument()
      })
    } else {
      expect(screen.queryByTestId(elementTestId)).not.toBeInTheDocument()
    }
  },

  // Button interaction permissions
  async expectButtonEnabled(testId: string, shouldBeEnabled: boolean) {
    const button = screen.getByTestId(testId)
    if (shouldBeEnabled) {
      expect(button).toBeEnabled()
    } else {
      expect(button).toBeDisabled()
    }
  }

  // 🔧 REMOVED: expectRouteAccessible method to fix unused parameter error
}

// 🔧 NEW: User Event Utilities with RBAC Context
export const rbacUserEvent = {
  // Setup user event with role context
  setupForRole(role: keyof typeof mockRoles | 'multiRole') {
    const context = createMockPermissionContext(role)
    const user = userEvent.setup()
    
    return {
      user,
      context,
      can: (permission: string) => context.hasPermission(permission),
      hasRole: (roleName: string) => context.hasRole(roleName),
      isAdmin: () => context.isAdmin(),
      
      // Conditional interactions
      async clickIfAllowed(element: HTMLElement, permission: string) {
        if (context.hasPermission(permission)) {
          await user.click(element)
          return true
        }
        return false
      },

      async typeIfAllowed(element: HTMLElement, text: string, permission: string) {
        if (context.hasPermission(permission)) {
          await user.type(element, text)
          return true
        }
        return false
      }
    }
  }
}

// 🔧 NEW: Batch Role Testing Utilities
export const rbacBatch = {
  // Test all roles against a component
  async testAllRoles(
    componentFn: (role: keyof typeof mockRoles) => ReactElement,
    expectations: Array<{
      role: keyof typeof mockRoles
      expectVisible?: string[]
      expectHidden?: string[]
      expectEnabled?: string[]
      expectDisabled?: string[]
    }>
  ) {
    for (const expectation of expectations) {
      const { role, expectVisible, expectHidden, expectEnabled, expectDisabled } = expectation
      
      // Render component as this role
      rbacRender.scenario().asRole(role).render(componentFn(role))
      
      // Check visibility expectations
      if (expectVisible) {
        for (const testId of expectVisible) {
          await waitFor(() => {
            expect(screen.getByTestId(testId)).toBeInTheDocument()
          })
        }
      }
      
      if (expectHidden) {
        for (const testId of expectHidden) {
          expect(screen.queryByTestId(testId)).not.toBeInTheDocument()
        }
      }
      
      // Check enabled/disabled state
      if (expectEnabled) {
        for (const testId of expectEnabled) {
          expect(screen.getByTestId(testId)).toBeEnabled()
        }
      }
      
      if (expectDisabled) {
        for (const testId of expectDisabled) {
          expect(screen.getByTestId(testId)).toBeDisabled()
        }
      }
    }
  },

  // Permission matrix testing
  testPermissionMatrix(testCases: Array<{
    permission: string
    roles: Array<{ 
      role: keyof typeof mockRoles | 'multiRole'
      shouldHave: boolean 
    }>
  }>) {
    testCases.forEach(testCase => {
      testCase.roles.forEach(roleTest => {
        const context = createMockPermissionContext(roleTest.role)
        const hasPermission = context.hasPermission(testCase.permission)
        
        expect(hasPermission).toBe(roleTest.shouldHave)
      })
    })
  },

  // Role hierarchy testing
  testRoleHierarchy() {
    const hierarchyTests = [
      { role: 'superAdmin' as const, expectedLevel: 0 },
      { role: 'systemAdmin' as const, expectedLevel: 1 },
      { role: 'admin' as const, expectedLevel: 2 },
      { role: 'manager' as const, expectedLevel: 3 },
      { role: 'user' as const, expectedLevel: 4 },
      { role: 'viewer' as const, expectedLevel: 5 }
    ]

    hierarchyTests.forEach(({ role, expectedLevel }) => {
      const context = createMockPermissionContext(role)
      expect(context.getRoleHierarchy()).toBe(expectedLevel)
    })
  }
}

// 🔧 NEW: Common Test Scenarios
export const rbacScenarios = {
  // Standard permission scenarios
  createPermissionScenario(permission: string) {
    return {
      permission,
      testCases: Object.keys(mockRoles).map(roleKey => ({
        role: roleKey as keyof typeof mockRoles,
        user: mockUsers[roleKey as keyof typeof mockUsers],
        hasPermission: createMockPermissionContext(roleKey as keyof typeof mockRoles).hasPermission(permission)
      }))
    }
  },

  // Common UI patterns
  adminOnlyButton: {
    visible: ['superAdmin', 'systemAdmin', 'admin'],
    hidden: ['manager', 'user', 'viewer']
  },

  managerAndAbove: {
    visible: ['superAdmin', 'systemAdmin', 'admin', 'manager'],
    hidden: ['user', 'viewer']
  },

  authenticatedOnly: {
    visible: ['superAdmin', 'systemAdmin', 'admin', 'manager', 'user', 'viewer'],
    hidden: []
  },

  // Form field scenarios
  systemAdminFields: {
    enabled: ['superAdmin', 'systemAdmin'],
    disabled: ['admin', 'manager', 'user', 'viewer']
  }
}

export default rbacRender
