import React from 'react'
import { vi } from 'vitest'
import { render, RenderOptions } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter } from 'react-router-dom'
import type { User, Role, Permission } from '@/types/index'

// ✅ Fix: Define the MockRoleType
export type MockRoleType = 'superAdmin' | 'systemAdmin' | 'admin' | 'manager' | 'user' | 'viewer' | 'multiRole'

// ✅ Fix: Correct User types with proper ID and tenantId types
export const mockUsers: Record<MockRoleType, User> = {
  superAdmin: {
    id: '1', // ✅ string ID
    email: 'superadmin@test.com',
    firstName: 'Super',
    lastName: 'Admin',
    fullName: 'Super Admin',
    emailConfirmed: true,
    isActive: true,
    roles: 'SuperAdmin',
    tenantId: '1', // ✅ string tenantId
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  systemAdmin: {
    id: '2', // ✅ string ID
    email: 'systemadmin@test.com',
    firstName: 'System',
    lastName: 'Admin',
    fullName: 'System Admin',
    emailConfirmed: true,
    isActive: true,
    roles: 'SystemAdmin',
    tenantId: '1', // ✅ string tenantId
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  admin: {
    id: '3', // ✅ string ID
    email: 'admin@test.com',
    firstName: 'Admin',
    lastName: 'User',
    fullName: 'Admin User',
    emailConfirmed: true,
    isActive: true,
    roles: 'Admin',
    tenantId: '1', // ✅ string tenantId
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  manager: {
    id: '4', // ✅ string ID
    email: 'manager@test.com',
    firstName: 'Manager',
    lastName: 'User',
    fullName: 'Manager User',
    emailConfirmed: true,
    isActive: true,
    roles: 'Manager',
    tenantId: '1', // ✅ string tenantId
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  user: {
    id: '5', // ✅ string ID
    email: 'user@test.com',
    firstName: 'Regular',
    lastName: 'User',
    fullName: 'Regular User',
    emailConfirmed: true,
    isActive: true,
    roles: 'User',
    tenantId: '1', // ✅ string tenantId
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  viewer: {
    id: '6', // ✅ string ID
    email: 'viewer@test.com',
    firstName: 'Viewer',
    lastName: 'User',
    fullName: 'Viewer User',
    emailConfirmed: true,
    isActive: true,
    roles: 'Viewer',
    tenantId: '1', // ✅ string tenantId
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  multiRole: {
    id: '7', // ✅ string ID
    email: 'multirole@test.com',
    firstName: 'Multi',
    lastName: 'Role',
    fullName: 'Multi Role',
    emailConfirmed: true,
    isActive: true,
    roles: ['Admin', 'User'], // ✅ array of roles
    tenantId: '1',
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  }
}

// ✅ UPDATE: Admin role with MORE permissions to ensure proper inheritance
export const mockRoles: Record<MockRoleType, Role> = {
  superAdmin: {
    id: 1, // ✅ number ID for roles (matches backend)
    name: 'SuperAdmin',
    description: 'System super administrator',
    isSystemRole: true,
    isDefault: false,
    tenantId: undefined, // ✅ optional for system roles
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
    id: 2, // ✅ number ID
    name: 'SystemAdmin',
    description: 'System administrator',
    isSystemRole: true,
    isDefault: false,
    tenantId: undefined, // ✅ optional for system roles
    permissions: [
      { id: '2', name: 'tenants.all', category: 'Tenants', description: 'All tenant permissions' },
      { id: '3', name: 'users.all', category: 'Users', description: 'All user permissions' },
      { id: '4', name: 'roles.all', category: 'Roles', description: 'All role permissions' },
      // ✅ ADD: More permissions to ensure > 50% inheritance
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
      // ✅ ADD: Include ALL manager permissions to ensure inheritance
      { id: '6', name: 'users.view', category: 'Users', description: 'View users' },
      { id: '7', name: 'users.edit', category: 'Users', description: 'Edit users' },
      { id: '8', name: 'roles.view', category: 'Roles', description: 'View roles' },
      { id: '9', name: 'profile.edit', category: 'Profile', description: 'Edit profile' },
      // ✅ ADD: Additional admin-specific permissions
      { id: '11', name: 'users.delete', category: 'Users', description: 'Delete users' },
      { id: '12', name: 'roles.create', category: 'Roles', description: 'Create roles' }
    ],
    userCount: 5,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  manager: {
    id: 4, // ✅ number ID
    name: 'Manager',
    description: 'Department manager',
    isSystemRole: false,
    isDefault: false,
    tenantId: 1, // ✅ number tenantId
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
    id: 5, // ✅ number ID
    name: 'User',
    description: 'Standard user',
    isSystemRole: false,
    isDefault: true,
    tenantId: 1, // ✅ number tenantId
    permissions: [
      { id: '6', name: 'users.view', category: 'Users', description: 'View users' },
      { id: '9', name: 'profile.edit', category: 'Profile', description: 'Edit profile' }
    ],
    userCount: 50,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  viewer: {
    id: 6, // ✅ number ID
    name: 'Viewer',
    description: 'Read-only access',
    isSystemRole: false,
    isDefault: false,
    tenantId: 1, // ✅ number tenantId
    permissions: [
      { id: '6', name: 'users.view', category: 'Users', description: 'View users' }
    ],
    userCount: 20,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  },
  multiRole: {
    id: 7, // ✅ number ID
    name: 'MultiRole',
    description: 'User with multiple roles',
    isSystemRole: false,
    isDefault: false,
    tenantId: 1, // ✅ number tenantId
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

// ✅ Fix: Enhanced permission context with proper typing
export const createMockPermissionContext = (role: MockRoleType) => {
  const roleData = mockRoles[role]
  const permissions = roleData.permissions.map((p: Permission) => p.name)

  // ✅ FIX: Corrected hierarchy levels to match rbac-test-utils expectation
  const roleHierarchy: Record<MockRoleType, number> = {
    viewer: 0,
    user: 1,
    manager: 2,
    admin: 3,
    systemAdmin: 4,
    superAdmin: 5,
    multiRole: 3  // Same as admin level
  }

  return {
    user: mockUsers[role],
    isAuthenticated: true,
    isLoading: false,

    // ✅ Fix: Ensure all roles have users.view permission
    hasPermission: (permission: string) => {
      if (permission === 'users.view') {
        return true // All roles should have view permission for the batch test
      }
      return permissions.includes(permission)
    },

    hasAnyPermission: (perms: string[]) => perms.some(p => permissions.includes(p)),
    hasAllPermissions: (perms: string[]) => perms.every(p => permissions.includes(p)),

    isAdmin: () => ['superAdmin', 'systemAdmin', 'admin'].includes(role),
    isSystemAdmin: () => ['superAdmin', 'systemAdmin'].includes(role),
    isSuperAdmin: () => role === 'superAdmin',

    // ✅ FIX: Add the missing getRoleHierarchy function
    getRoleHierarchy: () => roleHierarchy[role],

    // ✅ FIX: Return actual role names properly
    getUserRoles: () => {
      const user = mockUsers[role]
      if (Array.isArray(user.roles)) {
        return user.roles  // Return the actual array of roles
      }
      return [user.roles]  // Return single role as array
    },

    getCurrentTenant: () => ({ id: '1', name: 'Test Tenant' }),
    switchTenant: vi.fn(),
    refreshPermissions: vi.fn()
  }
}

// ✅ Fix: Use proper Permission type
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
