import { type ReactElement } from 'react'
import { render, type RenderOptions } from '@testing-library/react'
import { BrowserRouter, MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { AuthProvider } from '@/contexts/AuthContext.js'
// 🔧 REMOVED: Don't import PermissionProvider yet since we're getting build errors
// We'll add this later when the context is properly set up
import type { User, Permission, Role } from '@/types/index.js'

// Create a test query client with disabled retries for faster tests
const createTestQueryClient = () => new QueryClient({
  defaultOptions: {
    queries: {
      retry: false,
      gcTime: 0,
      staleTime: 0,
    },
    mutations: {
      retry: false,
    }
  },
})

// Keep all the existing mockPermissions and mockRoles definitions...
export const mockPermissions = {
  Users: [
    { id: '1', name: 'users.view', category: 'Users', description: 'View users' },
    { id: '2', name: 'users.create', category: 'Users', description: 'Create users' },
    { id: '3', name: 'users.edit', category: 'Users', description: 'Edit users' },
    { id: '4', name: 'users.delete', category: 'Users', description: 'Delete users' },
    { id: '5', name: 'users.manage_roles', category: 'Users', description: 'Manage user roles' },
    { id: '6', name: 'users.view_details', category: 'Users', description: 'View user details' },
    { id: '7', name: 'users.export', category: 'Users', description: 'Export user data' }
  ] as Permission[],
  Roles: [
    { id: '8', name: 'roles.view', category: 'Roles', description: 'View roles' },
    { id: '9', name: 'roles.create', category: 'Roles', description: 'Create roles' },
    { id: '10', name: 'roles.edit', category: 'Roles', description: 'Edit roles' },
    { id: '11', name: 'roles.delete', category: 'Roles', description: 'Delete roles' },
    { id: '12', name: 'roles.assign_permissions', category: 'Roles', description: 'Assign permissions to roles' },
    { id: '13', name: 'roles.view_permissions', category: 'Roles', description: 'View role permissions' }
  ] as Permission[],
  Reports: [
    { id: '14', name: 'reports.view', category: 'Reports', description: 'View reports' },
    { id: '15', name: 'reports.create', category: 'Reports', description: 'Create reports' },
    { id: '16', name: 'reports.export', category: 'Reports', description: 'Export reports' },
    { id: '17', name: 'reports.delete', category: 'Reports', description: 'Delete reports' }
  ] as Permission[],
  Tenants: [
    { id: '18', name: 'tenants.view', category: 'Tenants', description: 'View tenants' },
    { id: '19', name: 'tenants.manage', category: 'Tenants', description: 'Manage tenants' },
    { id: '20', name: 'tenants.configure', category: 'Tenants', description: 'Configure tenant settings' },
    { id: '21', name: 'tenants.create', category: 'Tenants', description: 'Create new tenants' }
  ] as Permission[],
  System: [
    { id: '22', name: 'system.admin', category: 'System', description: 'System administration' },
    { id: '23', name: 'system.logs', category: 'System', description: 'View system logs' },
    { id: '24', name: 'system.health', category: 'System', description: 'View system health' },
    { id: '25', name: 'system.maintenance', category: 'System', description: 'System maintenance' }
  ] as Permission[]
}

// Keep all the existing mockRoles definitions...
export const mockRoles = {
  superAdmin: {
    id: 1,
    name: 'SuperAdmin',
    description: 'System super administrator with all permissions',
    isSystemRole: true,
    isDefault: false,
    tenantId: undefined,
    permissions: [
      ...mockPermissions.Users,
      ...mockPermissions.Roles,
      ...mockPermissions.Reports,
      ...mockPermissions.Tenants,
      ...mockPermissions.System
    ],
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    userCount: 1
  } as Role,
  
  systemAdmin: {
    id: 2,
    name: 'SystemAdmin',
    description: 'System administrator with system management permissions',
    isSystemRole: true,
    isDefault: false,
    tenantId: undefined,
    permissions: [
      ...mockPermissions.Users,
      ...mockPermissions.Roles,
      ...mockPermissions.Reports,
      ...mockPermissions.System.slice(0, 3)
    ],
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    userCount: 2
  } as Role,
  
  admin: {
    id: 3,
    name: 'Admin',
    description: 'Tenant administrator with full tenant permissions',
    isSystemRole: false,
    isDefault: false,
    tenantId: 1,
    permissions: [
      ...mockPermissions.Users,
      ...mockPermissions.Roles,
      ...mockPermissions.Reports,
      mockPermissions.Tenants[0],
      mockPermissions.Tenants[2]
    ],
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    userCount: 5
  } as Role,
  
  manager: {
    id: 4,
    name: 'Manager',
    description: 'Manager with user and report management permissions',
    isSystemRole: false,
    isDefault: false,
    tenantId: 1,
    permissions: [
      mockPermissions.Users[0],
      mockPermissions.Users[2],
      mockPermissions.Users[5],
      mockPermissions.Roles[0],
      mockPermissions.Roles[5],
      ...mockPermissions.Reports.slice(0, 3)
    ],
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    userCount: 8
  } as Role,
  
  user: {
    id: 5,
    name: 'User',
    description: 'Standard user with basic permissions',
    isSystemRole: false,
    isDefault: true,
    tenantId: 1,
    permissions: [
      mockPermissions.Users[0],
      mockPermissions.Reports[0]
    ],
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    userCount: 25
  } as Role,
  
  viewer: {
    id: 6,
    name: 'Viewer',
    description: 'Read-only access to basic information',
    isSystemRole: false,
    isDefault: false,
    tenantId: 1,
    permissions: [
      mockPermissions.Users[0],
      mockPermissions.Roles[0],
      mockPermissions.Reports[0]
    ],
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    userCount: 12
  } as Role
}

// Keep all the existing mockUsers definitions...
export const mockUsers = {
  superAdmin: {
    id: '1',
    email: 'superadmin@example.com',
    firstName: 'Super',
    lastName: 'Admin',
    fullName: 'Super Admin',
    phoneNumber: '+1234567890',
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: '2024-01-01T00:00:00Z',
    emailConfirmed: true,
    isActive: true,
    roles: ['SuperAdmin'],
    tenantId: 'system',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  } as User,
  
  systemAdmin: {
    id: '2',
    email: 'sysadmin@example.com',
    firstName: 'System',
    lastName: 'Admin',
    fullName: 'System Admin',
    phoneNumber: '+1234567891',
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: '2024-01-01T00:00:00Z',
    emailConfirmed: true,
    isActive: true,
    roles: ['SystemAdmin'],
    tenantId: 'system',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  } as User,
  
  admin: {
    id: '3',
    email: 'admin@example.com',
    firstName: 'Admin',
    lastName: 'User',
    fullName: 'Admin User',
    phoneNumber: '+1234567892',
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: '2024-01-01T00:00:00Z',
    emailConfirmed: true,
    isActive: true,
    roles: ['Admin'],
    tenantId: 'tenant-1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  } as User,
  
  manager: {
    id: '4',
    email: 'manager@example.com',
    firstName: 'Manager',
    lastName: 'User',
    fullName: 'Manager User',
    phoneNumber: '+1234567893',
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: '2024-01-01T00:00:00Z',
    emailConfirmed: true,
    isActive: true,
    roles: ['Manager'],
    tenantId: 'tenant-1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  } as User,
  
  user: {
    id: '5',
    email: 'user@example.com',
    firstName: 'Regular',
    lastName: 'User',
    fullName: 'Regular User',
    phoneNumber: '+1234567894',
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: '2024-01-01T00:00:00Z',
    emailConfirmed: true,
    isActive: true,
    roles: ['User'],
    tenantId: 'tenant-1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  } as User,
  
  viewer: {
    id: '6',
    email: 'viewer@example.com',
    firstName: 'View',
    lastName: 'Only',
    fullName: 'View Only',
    phoneNumber: '+1234567895',
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: '2024-01-01T00:00:00Z',
    emailConfirmed: true,
    isActive: true,
    roles: ['Viewer'],
    tenantId: 'tenant-1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  } as User,
  
  multiRole: {
    id: '7',
    email: 'multirole@example.com',
    firstName: 'Multi',
    lastName: 'Role',
    fullName: 'Multi Role User',
    phoneNumber: '+1234567896',
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: '2024-01-01T00:00:00Z',
    emailConfirmed: true,
    isActive: true,
    roles: ['Manager', 'User'],
    tenantId: 'tenant-1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  } as User
}

// 🔧 SIMPLIFIED: Define basic permission context interface for testing
interface MockPermissionContextType {
  hasPermission: (permission: string) => boolean;
  hasAnyPermission: (permissions: string[]) => boolean;
  hasAllPermissions: (permissions: string[]) => boolean;
  hasRole: (roleName: string) => boolean;
  hasAnyRole: (roleNames: string[]) => boolean;
  hasAllRoles: (roleNames: string[]) => boolean;
  getUserRoles: () => string[];
  getUserPermissions: () => string[];
  isAdmin: () => boolean;
  isSuperAdmin: () => boolean;
  isSystemAdmin: () => boolean;
  isTenantAdmin: () => boolean;
  canManageUsers: () => boolean;
  canManageRoles: () => boolean;
  getEffectivePermissions: () => string[];
  getRoleHierarchy: () => number;
}

// 🔧 SIMPLIFIED: Mock Permission Context Factory (build-safe version)
export const createMockPermissionContext = (
  userRole: keyof typeof mockRoles | 'multiRole',
  customPermissions?: string[],
  customRoles?: string[]
): MockPermissionContextType => {
  
  let permissions: string[]
  let roles: string[]
  
  if (userRole === 'multiRole') {
    const managerPermissions = mockRoles.manager.permissions.map(p => p.name)
    const userPermissions = mockRoles.user.permissions.map(p => p.name)
    permissions = customPermissions || [...new Set([...managerPermissions, ...userPermissions])]
    roles = customRoles || ['Manager', 'User']
  } else {
    const role = mockRoles[userRole]
    permissions = customPermissions || role.permissions.map(p => p.name)
    roles = customRoles || [role.name]
  }
  
  return {
    hasPermission: (permission: string) => permissions.includes(permission),
    hasAnyPermission: (requiredPermissions: string[]) => 
      requiredPermissions.some(perm => permissions.includes(perm)),
    hasAllPermissions: (requiredPermissions: string[]) => 
      requiredPermissions.every(perm => permissions.includes(perm)),
    hasRole: (roleName: string) => roles.includes(roleName),
    hasAnyRole: (roleNames: string[]) => 
      roleNames.some(role => roles.includes(role)),
    hasAllRoles: (roleNames: string[]) => 
      roleNames.every(role => roles.includes(role)),
    getUserRoles: () => roles,
    getUserPermissions: () => permissions,
    isAdmin: () => roles.some(role => ['SuperAdmin', 'SystemAdmin', 'Admin'].includes(role)),
    isSuperAdmin: () => roles.includes('SuperAdmin'),
    isSystemAdmin: () => roles.some(role => ['SuperAdmin', 'SystemAdmin'].includes(role)),
    isTenantAdmin: () => roles.includes('Admin'),
    canManageUsers: () => permissions.some(perm => 
      ['users.create', 'users.edit', 'users.delete', 'users.manage_roles'].includes(perm)
    ),
    canManageRoles: () => permissions.some(perm => 
      ['roles.create', 'roles.edit', 'roles.delete', 'roles.assign_permissions'].includes(perm)
    ),
    getEffectivePermissions: () => permissions,
    getRoleHierarchy: () => {
      const hierarchy = ['SuperAdmin', 'SystemAdmin', 'Admin', 'Manager', 'User', 'Viewer']
      let highestLevel = hierarchy.length
      for (const role of roles) {
        const level = hierarchy.indexOf(role)
        if (level !== -1 && level < highestLevel) {
          highestLevel = level
        }
      }
      return highestLevel
    }
  }
}

// 🔧 SIMPLIFIED: Wrapper component (without PermissionProvider for now)
interface AllTheProvidersProps {
  children: React.ReactNode
  queryClient?: QueryClient
  initialEntries?: string[]
  userRole?: keyof typeof mockRoles | 'multiRole'
  mockUser?: User
  mockAuthState?: 'authenticated' | 'unauthenticated' | 'loading'
}

function AllTheProviders({ 
  children, 
  queryClient = createTestQueryClient(),
  initialEntries = ['/'],
  userRole = 'user',
  mockUser,
  mockAuthState = 'authenticated'
}: AllTheProvidersProps) {
  const RouterComponent = initialEntries.length > 1 || initialEntries[0] !== '/' 
    ? ({ children }: { children: React.ReactNode }) => (
        <MemoryRouter initialEntries={initialEntries}>
          {children}
        </MemoryRouter>
      )
    : BrowserRouter

  const resolvedMockUser = mockUser || (userRole === 'multiRole' ? mockUsers.multiRole : mockUsers[userRole as keyof typeof mockUsers])

  return (
    <QueryClientProvider client={queryClient}>
      <RouterComponent>
        <AuthProvider 
          mockUser={resolvedMockUser}
          mockAuthState={mockAuthState}
          testMode={true}
        >
          {children}
        </AuthProvider>
      </RouterComponent>
    </QueryClientProvider>
  )
}

// 🔧 SIMPLIFIED: Custom render function
const customRender = (
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'> & {
    queryClient?: QueryClient
    initialEntries?: string[]
    userRole?: keyof typeof mockRoles | 'multiRole'
    mockUser?: User
    mockAuthState?: 'authenticated' | 'unauthenticated' | 'loading'
  }
) => {
  const { 
    queryClient, 
    initialEntries, 
    userRole, 
    mockUser, 
    mockAuthState,
    ...renderOptions 
  } = options || {}
  
  return render(ui, {
    wrapper: ({ children }) => (
      <AllTheProviders 
        queryClient={queryClient}
        initialEntries={initialEntries}
        userRole={userRole}
        mockUser={mockUser}
        mockAuthState={mockAuthState}
      >
        {children}
      </AllTheProviders>
    ),
    ...renderOptions,
  })
}

// 🔧 SIMPLIFIED: RBAC Test Utilities (build-safe version)
export const rbacTestUtils = {
  // Role-specific render functions
  renderAsSuperAdmin: (ui: ReactElement, options?: any) => 
    customRender(ui, { ...options, userRole: 'superAdmin', mockUser: mockUsers.superAdmin }),
  
  renderAsSystemAdmin: (ui: ReactElement, options?: any) => 
    customRender(ui, { ...options, userRole: 'systemAdmin', mockUser: mockUsers.systemAdmin }),
  
  renderAsAdmin: (ui: ReactElement, options?: any) => 
    customRender(ui, { ...options, userRole: 'admin', mockUser: mockUsers.admin }),
  
  renderAsManager: (ui: ReactElement, options?: any) => 
    customRender(ui, { ...options, userRole: 'manager', mockUser: mockUsers.manager }),
  
  renderAsUser: (ui: ReactElement, options?: any) => 
    customRender(ui, { ...options, userRole: 'user', mockUser: mockUsers.user }),
  
  renderAsViewer: (ui: ReactElement, options?: any) => 
    customRender(ui, { ...options, userRole: 'viewer', mockUser: mockUsers.viewer }),
  
  renderAsMultiRole: (ui: ReactElement, options?: any) => 
    customRender(ui, { ...options, userRole: 'multiRole', mockUser: mockUsers.multiRole }),
  
  renderUnauthenticated: (ui: ReactElement, options?: any) => 
    customRender(ui, { ...options, mockAuthState: 'unauthenticated' }),

  // Permission validation utilities
  expectElementIfPermission: async (permission: string, userRole: keyof typeof mockRoles | 'multiRole', elementTestId: string) => {
    const { screen } = await import('@testing-library/react')
    const context = createMockPermissionContext(userRole)
    const hasPermission = context.hasPermission(permission)
    
    if (hasPermission) {
      expect(screen.getByTestId(elementTestId)).toBeInTheDocument()
    } else {
      expect(screen.queryByTestId(elementTestId)).not.toBeInTheDocument()
    }
  },

  expectElementIfRole: async (roleName: string, userRole: keyof typeof mockRoles | 'multiRole', elementTestId: string) => {
    const { screen } = await import('@testing-library/react')
    const context = createMockPermissionContext(userRole)
    const hasRole = context.hasRole(roleName)
    
    if (hasRole) {
      expect(screen.getByTestId(elementTestId)).toBeInTheDocument()
    } else {
      expect(screen.queryByTestId(elementTestId)).not.toBeInTheDocument()
    }
  },

  // Get mock data by role
  getMockUserByRole: (role: keyof typeof mockRoles | 'multiRole') => {
    return role === 'multiRole' ? mockUsers.multiRole : mockUsers[role as keyof typeof mockUsers]
  },

  getMockRoleByName: (roleName: string) => {
    return Object.values(mockRoles).find(role => role.name === roleName)
  },

  // Create test scenarios
  createPermissionTestScenario: (permission: string) => ({
    permission,
    testCases: Object.keys(mockRoles).map(roleKey => ({
      role: roleKey as keyof typeof mockRoles,
      user: mockUsers[roleKey as keyof typeof mockUsers],
      hasPermission: createMockPermissionContext(roleKey as keyof typeof mockRoles).hasPermission(permission)
    }))
  })
}

// Keep all the existing mock auth responses and utilities...
export const mockAuthResponses = {
  superAdminLogin: {
    success: true,
    data: {
      accessToken: 'mock-superadmin-token',
      refreshToken: 'mock-refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user: mockUsers.superAdmin
    }
  },
  
  adminLogin: {
    success: true,
    data: {
      accessToken: 'mock-admin-token',
      refreshToken: 'mock-refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user: mockUsers.admin
    }
  },
  
  userLogin: {
    success: true,
    data: {
      accessToken: 'mock-user-token',
      refreshToken: 'mock-refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user: mockUsers.user
    }
  }
}

// Utility functions for common test scenarios
export const waitForLoadingToFinish = async () => {
  const { screen } = await import('@testing-library/react')
  try {
    await screen.findByText('loading', { exact: false }, { timeout: 100 })
  } catch {
    // Loading finished or never appeared
  }
}

export const expectToastMessage = (toast: any, type: 'success' | 'error' | 'info' | 'warning', message: string) => {
  expect(toast[type]).toHaveBeenCalledWith(message)
}

// Re-export everything from React Testing Library
export * from '@testing-library/react'

// Override render method with our custom render
export { customRender as render }
