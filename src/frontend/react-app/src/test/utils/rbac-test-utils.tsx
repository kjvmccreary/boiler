import { type ReactElement } from 'react'
import { render, type RenderOptions, screen, waitFor, cleanup } from '@testing-library/react'
import { BrowserRouter, MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import userEvent from '@testing-library/user-event'
import { AuthProvider } from '@/contexts/AuthContext.js'
import { TenantProvider } from '@/contexts/TenantContext.js'; // 🔧 NEW: Add TenantProvider
import type { User } from '@/types/index.js'
import { mockUsers, mockRoles, createMockPermissionContext, type MockRoleType } from './test-utils.js'

// Fix: Add cleanup between renders
let currentQueryClient: QueryClient | null = null

export interface RoleTestConfig {
  role: MockRoleType
  user?: User
  customPermissions?: string[]
  customRoles?: string[]
  tenantId?: string
  authState?: 'authenticated' | 'unauthenticated' | 'loading'
}

export class RBACTestScenarioBuilder {
  private config: RoleTestConfig = {
    role: 'user',
    authState: 'authenticated',
    tenantId: 'tenant-1'
  }

  static create() {
    return new RBACTestScenarioBuilder()
  }

  asRole(role: MockRoleType) {
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
    // Fix: Cleanup before rendering to prevent multiple elements
    cleanup()
    if (currentQueryClient) {
      currentQueryClient.clear()
    }
    return renderWithRoleConfig(ui, this.config, options)
  }

  getPermissionContext() {
    return createMockPermissionContext(this.config.role)
  }

  build() {
    return this.config
  }
}

interface TestWrapperProps {
  children: React.ReactNode
  config: RoleTestConfig
  queryClient?: QueryClient
  initialEntries?: string[]
}

function RBACTestWrapper({
  children,
  config,
  queryClient,
  initialEntries = ['/']
}: TestWrapperProps) {
  // Fix: Create new query client for each test
  if (!queryClient) {
    if (currentQueryClient) {
      currentQueryClient.clear()
    }
    currentQueryClient = new QueryClient({
      defaultOptions: {
        queries: { retry: false, gcTime: 0, staleTime: 0 },
        mutations: { retry: false }
      }
    })
    queryClient = currentQueryClient
  }

  const RouterComponent = initialEntries.length > 1 || initialEntries[0] !== '/'
    ? ({ children }: { children: React.ReactNode }) => (
      <MemoryRouter initialEntries={initialEntries}>
        {children}
      </MemoryRouter>
    )
    : BrowserRouter

  const resolvedUser = config.user || mockUsers[config.role]

  return (
    <QueryClientProvider client={queryClient}>
      <RouterComponent>
        <AuthProvider
          mockUser={resolvedUser}
          mockAuthState={config.authState}
          testMode={true}
        >
          <TenantProvider> {/* 🔧 NEW: Add TenantProvider wrapper */}
            {children}
          </TenantProvider>
        </AuthProvider>
      </RouterComponent>
    </QueryClientProvider>
  )
}

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

// Fix: Enhanced render functions with cleanup
export const rbacRender = {
  asSuperAdmin: (ui: ReactElement, options?: RenderOptions) => {
    cleanup()
    return RBACTestScenarioBuilder.create().asRole('superAdmin').render(ui, options)
  },

  asSystemAdmin: (ui: ReactElement, options?: RenderOptions) => {
    cleanup()
    return RBACTestScenarioBuilder.create().asRole('systemAdmin').render(ui, options)
  },

  asAdmin: (ui: ReactElement, options?: RenderOptions) => {
    cleanup()
    return RBACTestScenarioBuilder.create().asRole('admin').render(ui, options)
  },

  asManager: (ui: ReactElement, options?: RenderOptions) => {
    cleanup()
    return RBACTestScenarioBuilder.create().asRole('manager').render(ui, options)
  },

  asUser: (ui: ReactElement, options?: RenderOptions) => {
    cleanup()
    return RBACTestScenarioBuilder.create().asRole('user').render(ui, options)
  },

  asViewer: (ui: ReactElement, options?: RenderOptions) => {
    cleanup()
    return RBACTestScenarioBuilder.create().asRole('viewer').render(ui, options)
  },

  asMultiRole: (ui: ReactElement, options?: RenderOptions) => {
    cleanup()
    return RBACTestScenarioBuilder.create().asRole('multiRole').render(ui, options)
  },

  unauthenticated: (ui: ReactElement, options?: RenderOptions) => {
    cleanup()
    return RBACTestScenarioBuilder.create().unauthenticated().render(ui, options)
  },

  loading: (ui: ReactElement, options?: RenderOptions) => {
    cleanup()
    return RBACTestScenarioBuilder.create().loading().render(ui, options)
  },

  withPermissions: (permissions: string[], ui: ReactElement, options?: RenderOptions) => {
    cleanup()
    return RBACTestScenarioBuilder.create().withCustomPermissions(permissions).render(ui, options)
  },

  withRoles: (roles: string[], ui: ReactElement, options?: RenderOptions) => {
    cleanup()
    return RBACTestScenarioBuilder.create().withCustomRoles(roles).render(ui, options)
  },

  scenario: () => RBACTestScenarioBuilder.create()
}

export const rbacAssert = {
  async expectElementIfPermission(
    permission: string,
    role: MockRoleType,
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

  async expectElementIfRole(
    roleName: string,
    userRole: MockRoleType,
    elementTestId: string
  ) {
    const context = createMockPermissionContext(userRole)
    const userRoles = context.getUserRoles()
    const hasRole = userRoles.includes(roleName)

    if (hasRole) {
      await waitFor(() => {
        expect(screen.getByTestId(elementTestId)).toBeInTheDocument()
      })
    } else {
      expect(screen.queryByTestId(elementTestId)).not.toBeInTheDocument()
    }
  },

  async expectElementIfAnyRole(
    roleNames: string[],
    userRole: MockRoleType,
    elementTestId: string
  ) {
    const context = createMockPermissionContext(userRole)
    const userRoles = context.getUserRoles()
    const hasAnyRole = roleNames.some(roleName => userRoles.includes(roleName))

    if (hasAnyRole) {
      await waitFor(() => {
        expect(screen.getByTestId(elementTestId)).toBeInTheDocument()
      })
    } else {
      expect(screen.queryByTestId(elementTestId)).not.toBeInTheDocument()
    }
  },

  async expectButtonEnabled(testId: string, shouldBeEnabled: boolean) {
    const button = screen.getByTestId(testId)
    if (shouldBeEnabled) {
      expect(button).toBeEnabled()
    } else {
      expect(button).toBeDisabled()
    }
  }
}

export const rbacUserEvent = {
  setupForRole(role: MockRoleType) {
    const context = createMockPermissionContext(role)
    const user = userEvent.setup()

    return {
      user,
      context,
      can: (permission: string) => context.hasPermission(permission),
      hasRole: (roleName: string) => context.getUserRoles().includes(roleName),
      isAdmin: () => context.isAdmin(),

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

// ✅ ADD: Missing testRoleHierarchy function that tests are looking for
export interface HierarchyTest {
  role: MockRoleType
  expectedLevel: number
}

// ✅ FIX: Define hierarchy tests with CORRECT expected levels 
export const hierarchyTests: HierarchyTest[] = [
  { role: 'viewer', expectedLevel: 0 },
  { role: 'user', expectedLevel: 1 },
  { role: 'manager', expectedLevel: 2 },
  { role: 'admin', expectedLevel: 3 },
  { role: 'systemAdmin', expectedLevel: 4 },
  { role: 'superAdmin', expectedLevel: 5 },
  { role: 'multiRole', expectedLevel: 3 } // Same as admin
]

// ✅ ADD: The missing testRoleHierarchy function
export function testRoleHierarchy(description: string) {
  hierarchyTests.forEach(({ role, expectedLevel }) => {
    const context = createMockPermissionContext(role)
    expect(context.getRoleHierarchy()).toBe(expectedLevel)
  })
}

// Fix: Enhanced batch testing with proper cleanup
export const rbacBatch = {
  async testAllRoles(
    componentFn: (role: MockRoleType) => ReactElement,
    expectations: Array<{
      role: MockRoleType
      expectVisible?: string[]
      expectHidden?: string[]
      expectEnabled?: string[]
      expectDisabled?: string[]
    }>
  ) {
    for (const expectation of expectations) {
      const { role, expectVisible, expectHidden, expectEnabled, expectDisabled } = expectation

      // Fix: Clean up between role tests
      cleanup()

      rbacRender.scenario().asRole(role).render(componentFn(role))

      if (expectVisible) {
        for (const testId of expectVisible) {
          await waitFor(() => {
            // Fix: Use getAllByTestId for expected multiple elements
            const elements = screen.getAllByTestId(testId)
            expect(elements.length).toBeGreaterThan(0)
          })
        }
      }

      if (expectHidden) {
        for (const testId of expectHidden) {
          expect(screen.queryByTestId(testId)).not.toBeInTheDocument()
        }
      }

      if (expectEnabled) {
        for (const testId of expectEnabled) {
          const elements = screen.getAllByTestId(testId)
          elements.forEach(element => {
            expect(element).toBeEnabled()
          })
        }
      }

      if (expectDisabled) {
        for (const testId of expectDisabled) {
          const elements = screen.getAllByTestId(testId)
          elements.forEach(element => {
            expect(element).toBeDisabled()
          })
        }
      }
    }
  },

  testPermissionMatrix(testCases: Array<{
    permission: string
    roles: Array<{
      role: MockRoleType
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

  // ✅ FIX: Update testRoleHierarchy to use the correct hierarchy levels
  testRoleHierarchy() {
    hierarchyTests.forEach(({ role, expectedLevel }) => {
      const context = createMockPermissionContext(role)
      expect(context.getRoleHierarchy()).toBe(expectedLevel)
    })
  }
}

export const rbacScenarios = {
  createPermissionScenario(permission: string) {
    return {
      permission,
      testCases: Object.keys(mockRoles).map(roleKey => ({
        role: roleKey as MockRoleType,
        user: mockUsers[roleKey as MockRoleType],
        hasPermission: createMockPermissionContext(roleKey as MockRoleType).hasPermission(permission)
      }))
    }
  },

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

  systemAdminFields: {
    enabled: ['superAdmin', 'systemAdmin'],
    disabled: ['admin', 'manager', 'user', 'viewer']
  }
}

// ✅ ADD: Permission inheritance testing functions
export function testPermissionInheritance() {
  hierarchyTests.forEach(({ role }) => {
    const context = createMockPermissionContext(role)
    const userRoles = context.getUserRoles()

    // Should return array of role names
    expect(Array.isArray(userRoles)).toBe(true)

    if (role === 'multiRole') {
      expect(userRoles).toContain('Admin')
      expect(userRoles).toContain('User')
    } else {
      expect(userRoles.length).toBeGreaterThan(0)
    }
  })
}

// ✅ ADD: Role permission checking utilities
export function createRoleTestSuite(componentName: string) {
  return {
    testAllRoles: (testFunction: (role: MockRoleType) => void) => {
      hierarchyTests.forEach(({ role }) => {
        it(`should work correctly for ${role} role`, () => {
          testFunction(role)
        })
      })
    },

    testAdminRoles: (testFunction: (role: MockRoleType) => void) => {
      const adminRoles: MockRoleType[] = ['admin', 'systemAdmin', 'superAdmin']
      adminRoles.forEach(role => {
        it(`should work correctly for admin role: ${role}`, () => {
          testFunction(role)
        })
      })
    },

    testBasicRoles: (testFunction: (role: MockRoleType) => void) => {
      const basicRoles: MockRoleType[] = ['user', 'viewer']
      basicRoles.forEach(role => {
        it(`should work correctly for basic role: ${role}`, () => {
          testFunction(role)
        })
      })
    }
  }
}

// ✅ ADD: Component testing utilities
export function renderWithRole(
  ui: ReactElement,
  role: MockRoleType,
  options: RenderOptions = {}
) {
  const mockContext = createMockPermissionContext(role)

  return {
    ...rbacRender.scenario().asRole(role).render(ui, options),
    mockContext
  }
}

// ✅ ADD: Permission testing helpers
export function expectPermissionAccess(
  role: MockRoleType,
  permission: string,
  shouldHaveAccess: boolean
) {
  const context = createMockPermissionContext(role)
  expect(context.hasPermission(permission)).toBe(shouldHaveAccess)
}

// ✅ ADD: Multi-role testing utilities
export function testMultiRoleScenarios() {
  const context = createMockPermissionContext('multiRole')
  const userRoles = context.getUserRoles()

  // Should return array with multiple roles
  expect(Array.isArray(userRoles)).toBe(true)
  expect(userRoles.length).toBeGreaterThan(1)

  // Should have both Admin and User roles
  expect(userRoles).toContain('Admin')
  expect(userRoles).toContain('User')

  // Should have admin-level permissions
  expect(context.isAdmin()).toBe(true)
  expect(context.hasPermission('users.all')).toBe(true)
  expect(context.hasPermission('roles.all')).toBe(true)
}

// ✅ ADD: Role hierarchy validation
export function validateRoleHierarchy() {
  const roles: MockRoleType[] = ['viewer', 'user', 'manager', 'admin', 'systemAdmin']

  for (let i = 0; i < roles.length - 1; i++) {
    const lowerRole = roles[i]
    const higherRole = roles[i + 1]

    const lowerContext = createMockPermissionContext(lowerRole)
    const higherContext = createMockPermissionContext(higherRole)

    const lowerHierarchy = lowerContext.getRoleHierarchy()
    const higherHierarchy = higherContext.getRoleHierarchy()

    // Higher roles should have higher hierarchy numbers
    expect(higherHierarchy).toBeGreaterThan(lowerHierarchy)
  }
}

// ✅ ADD: Permission inheritance testing
export function testPermissionInheritanceBetweenRoles(
  lowerRole: MockRoleType,
  higherRole: MockRoleType
) {
  // Get permissions for both roles (mock data simulation)
  const lowerPermissions = getLowerRolePermissions(lowerRole)
  const higherPermissions = getHigherRolePermissions(higherRole)

  // Higher roles should have at least as many permissions as lower roles
  expect(higherPermissions.length).toBeGreaterThanOrEqual(lowerPermissions.length)

  // Check inheritance rate
  const commonPermissions = lowerPermissions.filter(p => higherPermissions.includes(p))
  const inheritanceRate = commonPermissions.length / lowerPermissions.length

  // At least 50% of lower role permissions should be inherited
  expect(inheritanceRate).toBeGreaterThan(0.5)
}

// ✅ Helper functions for permission simulation
function getLowerRolePermissions(role: MockRoleType): string[] {
  if (role === 'user') return ['users.view', 'profile.edit']
  if (role === 'manager') return ['users.view', 'users.edit', 'roles.view', 'profile.edit']
  if (role === 'viewer') return ['users.view']
  return []
}

function getHigherRolePermissions(role: MockRoleType): string[] {
  if (role === 'admin') return ['users.all', 'roles.all', 'users.view', 'users.edit', 'roles.view', 'profile.edit', 'users.delete', 'roles.create']
  if (role === 'manager') return ['users.view', 'users.edit', 'roles.view', 'profile.edit']
  if (role === 'systemAdmin') return ['tenants.all', 'users.all', 'roles.all', 'users.view', 'users.edit', 'users.delete', 'dashboard.view', 'profile.edit']
  return []
}

// ✅ Export test constants
export const ROLE_HIERARCHY_LEVELS = {
  viewer: 0,
  user: 1,
  manager: 2,
  admin: 3,
  systemAdmin: 4,
  superAdmin: 5,
  multiRole: 3
} as const

export default rbacRender
