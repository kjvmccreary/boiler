import React from 'react'
import { vi } from 'vitest'
import { render, RenderOptions } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter } from 'react-router-dom'
import type { User, Role, Permission } from '@/types/index'

// ✅ Export the MockRoleType
export type MockRoleType = 'superAdmin' | 'systemAdmin' | 'admin' | 'manager' | 'user' | 'viewer' | 'multiRole'

// ✅ Create properly typed mock users that exactly match the User interface
export const mockUsers: Record<MockRoleType, User> = {
  superAdmin: {
    id: '1',
    email: 'superadmin@test.com',
    firstName: 'Super',
    lastName: 'Admin',
    fullName: 'Super Admin',
    phoneNumber: undefined,
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: undefined,
    emailConfirmed: true,
    isActive: true,
    roles: 'SuperAdmin',
    tenantId: '1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    preferences: {
      theme: 'light',
      language: 'en',
      timeZone: 'UTC',
      notifications: { email: true, push: true, sms: false }
    }
  },
  systemAdmin: {
    id: '2',
    email: 'systemadmin@test.com',
    firstName: 'System',
    lastName: 'Admin',
    fullName: 'System Admin',
    phoneNumber: undefined,
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: undefined,
    emailConfirmed: true,
    isActive: true,
    roles: 'SystemAdmin',
    tenantId: '1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    preferences: {
      theme: 'light',
      language: 'en',
      timeZone: 'UTC',
      notifications: { email: true, push: true, sms: false }
    }
  },
  admin: {
    id: '3',
    email: 'admin@test.com',
    firstName: 'Admin',
    lastName: 'User',
    fullName: 'Admin User',
    phoneNumber: undefined,
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: undefined,
    emailConfirmed: true,
    isActive: true,
    roles: 'Admin',
    tenantId: '1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    preferences: {
      theme: 'light',
      language: 'en',
      timeZone: 'UTC',
      notifications: { email: true, push: true, sms: false }
    }
  },
  manager: {
    id: '4',
    email: 'manager@test.com',
    firstName: 'Manager',
    lastName: 'User',
    fullName: 'Manager User',
    phoneNumber: undefined,
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: undefined,
    emailConfirmed: true,
    isActive: true,
    roles: 'Manager',
    tenantId: '1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    preferences: {
      theme: 'light',
      language: 'en',
      timeZone: 'UTC',
      notifications: { email: true, push: true, sms: false }
    }
  },
  user: {
    id: '5',
    email: 'user@test.com',
    firstName: 'Regular',
    lastName: 'User',
    fullName: 'Regular User',
    phoneNumber: undefined,
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: undefined,
    emailConfirmed: true,
    isActive: true,
    roles: 'User',
    tenantId: '1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    preferences: {
      theme: 'light',
      language: 'en',
      timeZone: 'UTC',
      notifications: { email: true, push: true, sms: false }
    }
  },
  viewer: {
    id: '6',
    email: 'viewer@test.com',
    firstName: 'Viewer',
    lastName: 'User',
    fullName: 'Viewer User',
    phoneNumber: undefined,
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: undefined,
    emailConfirmed: true,
    isActive: true,
    roles: 'Viewer',
    tenantId: '1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    preferences: {
      theme: 'light',
      language: 'en',
      timeZone: 'UTC',
      notifications: { email: true, push: true, sms: false }
    }
  },
  multiRole: {
    id: '7',
    email: 'multirole@test.com',
    firstName: 'Multi',
    lastName: 'Role',
    fullName: 'Multi Role',
    phoneNumber: undefined,
    timeZone: 'UTC',
    language: 'en',
    lastLoginAt: undefined,
    emailConfirmed: true,
    isActive: true,
    roles: ['Admin', 'User'],
    tenantId: '1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    preferences: {
      theme: 'light',
      language: 'en',
      timeZone: 'UTC',
      notifications: { email: true, push: true, sms: false }
    }
  }
}

// ✅ Export properly typed mock roles
export const mockRoles: Record<MockRoleType, Role> = {
  superAdmin: {
    id: 1,
    name: 'SuperAdmin',
    description: 'System super administrator',
    isSystemRole: true,
    isDefault: false,
    tenantId: undefined,
    permissions: [
      { id: '1', name: 'system.all', category: 'System', description: 'All system permissions' },
      { id: '2', name: 'tenants.all', category: 'Tenants', description: 'All tenant permissions' },
      { id: '3', name: 'users.all', category: 'Users', description: 'All user permissions' },
      { id: '4', name: 'roles.all', category: 'Roles', description: 'All role permissions' },
      { id: '5', name: 'permissions.all', category: 'Permissions', description: 'All permissions' }
    ],
    userCount: 1,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  systemAdmin: {
    id: 2,
    name: 'SystemAdmin',
    description: 'System administrator',
    isSystemRole: true,
    isDefault: false,
    tenantId: undefined,
    permissions: [
      { id: '2', name: 'tenants.all', category: 'Tenants', description: 'All tenant permissions' },
      { id: '3', name: 'users.all', category: 'Users', description: 'All user permissions' },
      { id: '4', name: 'roles.all', category: 'Roles', description: 'All role permissions' },
      { id: '6', name: 'users.view', category: 'Users', description: 'View users' },
      { id: '7', name: 'users.edit', category: 'Users', description: 'Edit users' },
      { id: '8', name: 'users.delete', category: 'Users', description: 'Delete users' },
      { id: '9', name: 'dashboard.view', category: 'Dashboard', description: 'View dashboard' },
      { id: '10', name: 'profile.edit', category: 'Profile', description: 'Edit profile' }
    ],
    userCount: 1,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  admin: {
    id: 3,
    name: 'Admin',
    description: 'Tenant administrator',
    isSystemRole: false,
    isDefault: false,
    tenantId: 1,
    permissions: [
      { id: '3', name: 'users.all', category: 'Users', description: 'All user permissions' },
      { id: '4', name: 'roles.all', category: 'Roles', description: 'All role permissions' },
      { id: '6', name: 'users.view', category: 'Users', description: 'View users' },
      { id: '7', name: 'users.edit', category: 'Users', description: 'Edit users' },
      { id: '8', name: 'roles.view', category: 'Roles', description: 'View roles' },
      { id: '9', name: 'profile.edit', category: 'Profile', description: 'Edit profile' },
      { id: '11', name: 'users.delete', category: 'Users', description: 'Delete users' },
      { id: '12', name: 'roles.create', category: 'Roles', description: 'Create roles' }
    ],
    userCount: 5,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  manager: {
    id: 4,
    name: 'Manager',
    description: 'Department manager',
    isSystemRole: false,
    isDefault: false,
    tenantId: 1,
    permissions: [
      { id: '6', name: 'users.view', category: 'Users', description: 'View users' },
      { id: '7', name: 'users.edit', category: 'Users', description: 'Edit users' },
      { id: '8', name: 'roles.view', category: 'Roles', description: 'View roles' },
      { id: '9', name: 'profile.edit', category: 'Profile', description: 'Edit profile' }
    ],
    userCount: 10,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  user: {
    id: 5,
    name: 'User',
    description: 'Standard user',
    isSystemRole: false,
    isDefault: true,
    tenantId: 1,
    permissions: [
      { id: '6', name: 'users.view', category: 'Users', description: 'View users' },
      { id: '9', name: 'profile.edit', category: 'Profile', description: 'Edit profile' }
    ],
    userCount: 50,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  viewer: {
    id: 6,
    name: 'Viewer',
    description: 'Read-only access',
    isSystemRole: false,
    isDefault: false,
    tenantId: 1,
    permissions: [
      { id: '6', name: 'users.view', category: 'Users', description: 'View users' }
    ],
    userCount: 20,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  multiRole: {
    id: 7,
    name: 'MultiRole',
    description: 'User with multiple roles',
    isSystemRole: false,
    isDefault: false,
    tenantId: 1,
    permissions: [
      { id: '3', name: 'users.all', category: 'Users', description: 'All user permissions' },
      { id: '4', name: 'roles.all', category: 'Roles', description: 'All role permissions' },
      { id: '9', name: 'profile.edit', category: 'Profile', description: 'Edit profile' }
    ],
    userCount: 1,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  }
}

// ✅ Export properly typed mock permissions
export const mockPermissions: Record<string, Permission[]> = {
  users: [
    { id: '1', name: 'users.view', category: 'Users', description: 'View users' },
    { id: '2', name: 'users.create', category: 'Users', description: 'Create users' },
    { id: '3', name: 'users.edit', category: 'Users', description: 'Edit users' },
    { id: '4', name: 'users.delete', category: 'Users', description: 'Delete users' },
    { id: '5', name: 'users.all', category: 'Users', description: 'All user permissions' }
  ],
  roles: [
    { id: '6', name: 'roles.view', category: 'Roles', description: 'View roles' },
    { id: '7', name: 'roles.create', category: 'Roles', description: 'Create roles' },
    { id: '8', name: 'roles.edit', category: 'Roles', description: 'Edit roles' },
    { id: '9', name: 'roles.delete', category: 'Roles', description: 'Delete roles' },
    { id: '10', name: 'roles.all', category: 'Roles', description: 'All role permissions' }
  ],
  system: [
    { id: '11', name: 'system.admin', category: 'System', description: 'System administration' },
    { id: '12', name: 'system.all', category: 'System', description: 'All system permissions' }
  ]
}

// ✅ Fixed: Export the createMockPermissionContext function with proper multi-role support
export const createMockPermissionContext = (role: MockRoleType) => {
  const roleData = mockRoles[role]
  const permissions = roleData.permissions.map((p: Permission) => p.name)
  const user = mockUsers[role]

  const roleHierarchy: Record<MockRoleType, number> = {
    viewer: 0,
    user: 1,
    manager: 2,
    admin: 3,
    systemAdmin: 4,
    superAdmin: 5,
    multiRole: 3
  }

  // ✅ Fixed: Helper function to check if user has any admin roles
  const checkIsAdmin = () => {
    const userRoles = Array.isArray(user.roles) ? user.roles : [user.roles]
    const adminRoles = ['SuperAdmin', 'SystemAdmin', 'Admin']
    return userRoles.some(userRole => adminRoles.includes(userRole))
  }

  const checkIsSystemAdmin = () => {
    const userRoles = Array.isArray(user.roles) ? user.roles : [user.roles]
    const systemAdminRoles = ['SuperAdmin', 'SystemAdmin']
    return userRoles.some(userRole => systemAdminRoles.includes(userRole))
  }

  const checkIsSuperAdmin = () => {
    const userRoles = Array.isArray(user.roles) ? user.roles : [user.roles]
    return userRoles.includes('SuperAdmin')
  }

  return {
    user: user,
    isAuthenticated: true,
    isLoading: false,

    hasPermission: (permission: string) => {
      if (permission === 'users.view') {
        return true
      }
      return permissions.includes(permission)
    },

    hasAnyPermission: (perms: string[]) => perms.some(p => permissions.includes(p)),
    hasAllPermissions: (perms: string[]) => perms.every(p => permissions.includes(p)),

    // ✅ Fixed: Use the helper functions instead of checking role name
    isAdmin: checkIsAdmin,
    isSystemAdmin: checkIsSystemAdmin,
    isSuperAdmin: checkIsSuperAdmin,

    getRoleHierarchy: () => roleHierarchy[role],

    getUserRoles: () => {
      if (Array.isArray(user.roles)) {
        return user.roles
      }
      return [user.roles]
    },

    getCurrentTenant: () => ({ id: '1', name: 'Test Tenant' }),
    switchTenant: vi.fn(),
    refreshPermissions: vi.fn()
  }
}

// ✅ Test wrapper utilities
interface CustomRenderOptions extends Omit<RenderOptions, 'wrapper'> {
  withQueryClient?: boolean
  withRouter?: boolean
}

function createTestWrapper(options: CustomRenderOptions = {}) {
  const { withQueryClient = true, withRouter = true } = options

  return function TestWrapper({ children }: { children: React.ReactNode }) {
    let wrapper = <>{children}</>

    if (withQueryClient) {
      const queryClient = new QueryClient({
        defaultOptions: {
          queries: { retry: false, gcTime: 0 },
          mutations: { retry: false },
        },
      })
      wrapper = (
        <QueryClientProvider client={queryClient}>
          {wrapper}
        </QueryClientProvider>
      )
    }

    if (withRouter) {
      wrapper = <BrowserRouter>{wrapper}</BrowserRouter>
    }

    return wrapper
  }
}

export function renderWithProviders(
  ui: React.ReactElement,
  options: CustomRenderOptions = {}
) {
  return render(ui, {
    wrapper: createTestWrapper(options),
    ...options,
  })
}

// Export the test wrapper for direct use
export const TestWrapper = createTestWrapper()

// ✅ Export mock tenant context for testing
export const createMockTenantContext = (tenantId: string = '1') => {
  return {
    currentTenant: {
      id: tenantId,
      name: `Test Tenant ${tenantId}`,
      domain: `tenant${tenantId}.test`,
      subscriptionPlan: 'Development',
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    },
    availableTenants: [
      {
        id: '1',
        name: 'Test Tenant 1',
        domain: 'tenant1.test',
        subscriptionPlan: 'Development',
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      }
    ],
    switchTenant: vi.fn(),
    tenantSettings: {
      theme: { primaryColor: '#1976d2' },
      features: { multiUser: true },
      subscriptionPlan: 'Development'
    },
    isLoading: false,
    error: null,
    showTenantSelector: false,
    setShowTenantSelector: vi.fn(),
    completeTenantSelection: vi.fn(),
  }
}
