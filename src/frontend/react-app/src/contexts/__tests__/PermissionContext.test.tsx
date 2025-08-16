import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { render, screen, renderHook } from '@testing-library/react'
import React from 'react'
import { PermissionProvider, usePermission, PermissionContext } from '../PermissionContext'
import { AuthProvider } from '../AuthContext'
import { tokenManager } from '@/utils/token.manager'
import { authService } from '@/services/auth.service'
import type { User, AuthResponse } from '@/types/index'

// Mock the dependencies
vi.mock('@/utils/token.manager')
vi.mock('@/services/auth.service')
vi.mock('react-router-dom', () => ({
  useNavigate: vi.fn(),
  useLocation: vi.fn(() => ({ pathname: '/test', search: '', hash: '', state: null, key: 'test' })),
}))

const mockTokenManager = vi.mocked(tokenManager)
const mockAuthService = vi.mocked(authService)

// Mock users with different role/permission scenarios
const createMockUser = (roles: string | string[], overrides: Partial<User> = {}): User => ({
  id: '1',
  email: 'test@example.com',
  firstName: 'Test',
  lastName: 'User',
  fullName: 'Test User',
  roles,
  tenantId: 'tenant1',
  isActive: true,
  emailConfirmed: true,
  createdAt: '2024-01-01T00:00:00Z',
  updatedAt: '2024-01-01T00:00:00Z',
  ...overrides
})

const mockAuthResponse: AuthResponse = {
  accessToken: 'mock-jwt-token',
  refreshToken: 'mock-refresh-token',
  expiresAt: '2024-01-01T01:00:00Z',
  tokenType: 'Bearer',
  user: createMockUser(['User']),
  tenant: {
    id: 'tenant1',
    name: 'Test Tenant',
    isActive: true,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z'
  }
}

// Test wrapper that provides both AuthContext and PermissionContext
interface TestWrapperProps {
  children: React.ReactNode
  mockUser?: User | null
  mockPermissionContext?: any
  testMode?: boolean
}

function TestWrapper({ 
  children, 
  mockUser = createMockUser(['User']),
  mockPermissionContext,
  testMode = false
}: TestWrapperProps) {
  return (
    <AuthProvider testMode mockUser={mockUser || undefined} mockAuthState="authenticated">
      <PermissionProvider testMode={testMode} mockContext={mockPermissionContext}>
        {children}
      </PermissionProvider>
    </AuthProvider>
  )
}

// Test component to test usePermission hook
function TestPermissionComponent({ onPermissionCheck }: { onPermissionCheck?: (context: any) => void }) {
  const permissions = usePermission()
  
  React.useEffect(() => {
    if (onPermissionCheck) {
      onPermissionCheck(permissions)
    }
  }, [permissions, onPermissionCheck])

  return (
    <div>
      <div data-testid="has-users-read">{permissions.hasPermission('users.read').toString()}</div>
      <div data-testid="has-admin-role">{permissions.hasRole('Admin').toString()}</div>
      <div data-testid="is-admin">{permissions.isAdmin().toString()}</div>
      <div data-testid="user-roles">{permissions.getUserRoles().join(',')}</div>
      <div data-testid="user-permissions">{permissions.getUserPermissions().join(',')}</div>
    </div>
  )
}

describe('PermissionContext', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    // Reset console.log and console.error mocks
    vi.spyOn(console, 'log').mockImplementation(() => {})
    vi.spyOn(console, 'error').mockImplementation(() => {})
    
    // Setup default mocks
    mockTokenManager.getToken.mockReturnValue(null)
    mockTokenManager.getTokenClaims.mockReturnValue({})
  })

  afterEach(() => {
    vi.restoreAllMocks()
  })

  describe('PermissionProvider', () => {
    it('should render children without permission context', () => {
      render(
        <TestWrapper mockUser={null}>
          <div data-testid="test-child">Test Content</div>
        </TestWrapper>
      )

      expect(screen.getByTestId('test-child')).toBeInTheDocument()
      expect(screen.getByText('Test Content')).toBeInTheDocument()
    })

    it('should provide permission context with authenticated user', () => {
      const mockUser = createMockUser(['User'])
      
      // Set up token manager mocks BEFORE rendering
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.getTokenClaims.mockReturnValue({ 
        permission: ['users.read', 'profile.edit'],
        role: 'User'
      })
      
      render(
        <TestWrapper mockUser={mockUser}>
          <TestPermissionComponent />
        </TestWrapper>
      )

      expect(screen.getByTestId('has-users-read')).toHaveTextContent('true')
      expect(screen.getByTestId('user-roles')).toHaveTextContent('User')
    })

    it('should use mock context in test mode', () => {
      const mockPermissionContext = {
        hasPermission: vi.fn(() => true),
        hasAnyPermission: vi.fn(() => true),
        hasAllPermissions: vi.fn(() => false),
        hasRole: vi.fn(() => true),
        hasAnyRole: vi.fn(() => true),
        hasAllRoles: vi.fn(() => false),
        getUserRoles: vi.fn(() => ['MockRole']),
        getUserPermissions: vi.fn(() => ['mock.permission']),
        isAdmin: vi.fn(() => true),
        isSuperAdmin: vi.fn(() => false),
        isSystemAdmin: vi.fn(() => false),
        isTenantAdmin: vi.fn(() => false),
        canManageUsers: vi.fn(() => true),
        canManageRoles: vi.fn(() => false),
        getEffectivePermissions: vi.fn(() => ['mock.permission']),
        getRoleHierarchy: vi.fn(() => 0)
      }

      render(
        <TestWrapper 
          testMode={true} 
          mockPermissionContext={mockPermissionContext}
        >
          <TestPermissionComponent />
        </TestWrapper>
      )

      expect(screen.getByTestId('has-users-read')).toHaveTextContent('true')
      expect(screen.getByTestId('has-admin-role')).toHaveTextContent('true')
      expect(screen.getByTestId('is-admin')).toHaveTextContent('true')
      expect(screen.getByTestId('user-roles')).toHaveTextContent('MockRole')
      expect(screen.getByTestId('user-permissions')).toHaveTextContent('mock.permission')
    })
  })

  describe('usePermission hook', () => {
    it('should throw error when used outside PermissionProvider', () => {
      // Suppress console.error for this test
      const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
      
      expect(() => {
        renderHook(() => usePermission())
      }).toThrow('usePermission must be used within a PermissionProvider')
      
      consoleSpy.mockRestore()
    })

    it('should return permission context when used within provider', () => {
      const { result } = renderHook(() => usePermission(), {
        wrapper: ({ children }) => (
          <TestWrapper>
            {children}
          </TestWrapper>
        )
      })

      expect(result.current).toBeDefined()
      expect(typeof result.current.hasPermission).toBe('function')
      expect(typeof result.current.hasRole).toBe('function')
      expect(typeof result.current.isAdmin).toBe('function')
    })
  })

  describe('Permission checking methods', () => {
    describe('hasPermission', () => {
      it('should return false when user is not authenticated', () => {
        render(
          <TestWrapper mockUser={null}>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('has-users-read')).toHaveTextContent('false')
      })

      it('should return true when user has the permission via JWT token', () => {
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['users.read', 'users.edit'] 
        })
        
        render(
          <TestWrapper>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('has-users-read')).toHaveTextContent('true')
      })

      it('should return false when user does not have the permission', () => {
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['profile.read'] 
        })
        
        render(
          <TestWrapper>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('has-users-read')).toHaveTextContent('false')
      })

      it('should handle string permission format from token', () => {
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: 'users.read,users.edit,profile.read' 
        })
        
        render(
          <TestWrapper>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('has-users-read')).toHaveTextContent('true')
      })

      it('should handle empty permissions gracefully', () => {
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: [] 
        })
        
        render(
          <TestWrapper>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('has-users-read')).toHaveTextContent('false')
      })
    })

    describe('hasAnyPermission', () => {
      it('should return true when user has at least one permission', () => {
        const mockUser = createMockUser(['User'])
        let permissionContext: any
        
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['users.read', 'profile.edit'] 
        })
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        const hasAny = permissionContext?.hasAnyPermission(['users.read', 'admin.access'])
        expect(hasAny).toBe(true)
      })

      it('should return false when user has none of the permissions', () => {
        const mockUser = createMockUser(['User'])
        let permissionContext: any
        
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['profile.read'] 
        })
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        const hasAny = permissionContext?.hasAnyPermission(['users.edit', 'admin.access'])
        expect(hasAny).toBe(false)
      })
    })

    describe('hasAllPermissions', () => {
      it('should return true when user has all required permissions', () => {
        const mockUser = createMockUser(['Admin'])
        let permissionContext: any
        
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['users.read', 'users.edit', 'users.delete'] 
        })
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        const hasAll = permissionContext?.hasAllPermissions(['users.read', 'users.edit'])
        expect(hasAll).toBe(true)
      })

      it('should return false when user is missing some permissions', () => {
        const mockUser = createMockUser(['User'])
        let permissionContext: any
        
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['users.read'] 
        })
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        const hasAll = permissionContext?.hasAllPermissions(['users.read', 'users.edit'])
        expect(hasAll).toBe(false)
      })
    })
  })

  describe('Role checking methods', () => {
    describe('hasRole', () => {
      it('should return false when user is not authenticated', () => {
        render(
          <TestWrapper mockUser={null}>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('has-admin-role')).toHaveTextContent('false')
      })

      it('should return true when user has the role', () => {
        render(
          <TestWrapper 
            mockUser={createMockUser(['Admin', 'User'])}
          >
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('has-admin-role')).toHaveTextContent('true')
      })

      it('should return false when user does not have the role', () => {
        render(
          <TestWrapper 
            mockUser={createMockUser(['User'])}
          >
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('has-admin-role')).toHaveTextContent('false')
      })

      it('should handle single role string format', () => {
        render(
          <TestWrapper 
            mockUser={createMockUser('Admin')}
          >
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('has-admin-role')).toHaveTextContent('true')
      })
    })

    describe('hasAnyRole', () => {
      it('should return true when user has at least one of the roles', () => {
        const mockUser = createMockUser(['Manager'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        const hasAny = permissionContext?.hasAnyRole(['Admin', 'Manager', 'SuperAdmin'])
        expect(hasAny).toBe(true)
      })

      it('should return false when user has none of the roles', () => {
        const mockUser = createMockUser(['User'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        const hasAny = permissionContext?.hasAnyRole(['Admin', 'SuperAdmin'])
        expect(hasAny).toBe(false)
      })

      it('should return false when roles array is empty', () => {
        const mockUser = createMockUser(['User'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        const hasAny = permissionContext?.hasAnyRole([])
        expect(hasAny).toBe(false)
      })
    })

    describe('hasAllRoles', () => {
      it('should return true when user has all required roles', () => {
        const mockUser = createMockUser(['Admin', 'Manager', 'User'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        const hasAll = permissionContext?.hasAllRoles(['Admin', 'User'])
        expect(hasAll).toBe(true)
      })

      it('should return false when user is missing some roles', () => {
        const mockUser = createMockUser(['User'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        const hasAll = permissionContext?.hasAllRoles(['Admin', 'User'])
        expect(hasAll).toBe(false)
      })
    })
  })

  describe('Admin checking methods', () => {
    describe('isAdmin', () => {
      it('should return false when user is not authenticated', () => {
        render(
          <TestWrapper mockUser={null}>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('is-admin')).toHaveTextContent('false')
      })

      it('should return true when user has admin permissions', () => {
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['users.edit', 'users.create', 'roles.edit'] 
        })
        
        render(
          <TestWrapper mockUser={createMockUser(['User'])}>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('is-admin')).toHaveTextContent('true')
      })

      it('should return true when user has admin role', () => {
        render(
          <TestWrapper mockUser={createMockUser(['Admin'])}>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('is-admin')).toHaveTextContent('true')
      })

      it('should return false when user has neither admin permissions nor roles', () => {
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['profile.read', 'profile.edit'] 
        })
        
        render(
          <TestWrapper mockUser={createMockUser(['User'])}>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('is-admin')).toHaveTextContent('false')
      })
    })

    describe('isSuperAdmin', () => {
      it('should return true when user has SuperAdmin role', () => {
        const mockUser = createMockUser(['SuperAdmin'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.isSuperAdmin()).toBe(true)
      })

      it('should return true when user has system.admin permission', () => {
        const mockUser = createMockUser(['Admin'])
        let permissionContext: any
        
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['system.admin'] 
        })
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.isSuperAdmin()).toBe(true)
      })

      it('should return false when user is not SuperAdmin', () => {
        const mockUser = createMockUser(['Admin'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.isSuperAdmin()).toBe(false)
      })
    })

    describe('isSystemAdmin', () => {
      it('should return true when user has SuperAdmin role', () => {
        const mockUser = createMockUser(['SuperAdmin'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.isSystemAdmin()).toBe(true)
      })

      it('should return true when user has SystemAdmin role', () => {
        const mockUser = createMockUser(['SystemAdmin'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.isSystemAdmin()).toBe(true)
      })

      it('should return false when user has neither role', () => {
        const mockUser = createMockUser(['Admin'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.isSystemAdmin()).toBe(false)
      })
    })

    describe('isTenantAdmin', () => {
      it('should return true when user has Admin role', () => {
        const mockUser = createMockUser(['Admin'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.isTenantAdmin()).toBe(true)
      })

      it('should return true when user has tenants.manage permission', () => {
        const mockUser = createMockUser(['Manager'])
        let permissionContext: any
        
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['tenants.manage'] 
        })
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.isTenantAdmin()).toBe(true)
      })
    })

    describe('canManageUsers', () => {
      it('should return true when user has user management permissions', () => {
        const mockUser = createMockUser(['Admin'])
        let permissionContext: any
        
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['users.create', 'users.edit'] 
        })
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.canManageUsers()).toBe(true)
      })

      it('should return false when user has no user management permissions', () => {
        const mockUser = createMockUser(['User'])
        let permissionContext: any
        
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['profile.read'] 
        })
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.canManageUsers()).toBe(false)
      })
    })

    describe('canManageRoles', () => {
      it('should return true when user has role management permissions', () => {
        const mockUser = createMockUser(['SuperAdmin'])
        let permissionContext: any
        
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['roles.create', 'roles.edit'] 
        })
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.canManageRoles()).toBe(true)
      })

      it('should return false when user has no role management permissions', () => {
        const mockUser = createMockUser(['User'])
        let permissionContext: any
        
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['users.read'] 
        })
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.canManageRoles()).toBe(false)
      })
    })
  })

  describe('Data retrieval methods', () => {
    describe('getUserRoles', () => {
      it('should return roles from JWT token when available', () => {
        // Set up JWT token with roles (should take priority)
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          role: ['Admin', 'Manager'] // JWT token roles (should take priority)
        })
        
        render(
          <TestWrapper mockUser={createMockUser(['User'])}> {/* User object role - should be ignored */}
            <TestPermissionComponent />
          </TestWrapper>
        )

        // Should show JWT token roles, not user object roles
        expect(screen.getByTestId('user-roles')).toHaveTextContent('Admin,Manager')
      })

      it('should handle Microsoft role claim format', () => {
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': 'Admin'
        })
        
        render(
          <TestWrapper mockUser={createMockUser(['User'])}>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('user-roles')).toHaveTextContent('Admin')
      })

      it('should handle comma-separated role string', () => {
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          role: 'Admin,Manager,SuperAdmin'
        })
        
        render(
          <TestWrapper mockUser={createMockUser(['User'])}>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('user-roles')).toHaveTextContent('Admin,Manager,SuperAdmin')
      })

      it('should fallback to user object roles when no token roles', () => {
        // Set up mocks - no token or empty claims
        mockTokenManager.getToken.mockReturnValue(null) // No token
        mockTokenManager.getTokenClaims.mockReturnValue({}) // No role claims
        
        render(
          <TestWrapper mockUser={createMockUser(['User', 'Manager'])}>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('user-roles')).toHaveTextContent('User,Manager')
      })

      it('should handle single role string from user object', () => {
        // Set up mocks - no token
        mockTokenManager.getToken.mockReturnValue(null) // No token
        mockTokenManager.getTokenClaims.mockReturnValue({})
        
        render(
          <TestWrapper mockUser={createMockUser('Admin')}>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('user-roles')).toHaveTextContent('Admin')
      })

      it('should return empty array when no roles available', () => {
        // Set up mocks - no token and no user roles
        mockTokenManager.getToken.mockReturnValue(null)
        mockTokenManager.getTokenClaims.mockReturnValue({})
        
        render(
          <TestWrapper mockUser={createMockUser([])}>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('user-roles')).toBeEmptyDOMElement()
      })
    })

    describe('getUserPermissions', () => {
      it('should return permissions from JWT token', () => {
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['users.read', 'users.edit', 'profile.read'] 
        })
        
        render(
          <TestWrapper>
            <TestPermissionComponent />
          </TestWrapper>
        )

        const permissions = screen.getByTestId('user-permissions').textContent
        expect(permissions).toContain('users.read')
        expect(permissions).toContain('users.edit')
        expect(permissions).toContain('profile.read')
      })

      it('should handle string permission format', () => {
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: 'users.read,users.edit,profile.read' 
        })
        
        render(
          <TestWrapper>
            <TestPermissionComponent />
          </TestWrapper>
        )

        const permissions = screen.getByTestId('user-permissions').textContent
        expect(permissions).toContain('users.read')
        expect(permissions).toContain('users.edit')
      })

      it('should return empty array when no permissions', () => {
        // Set up mocks - no token
        mockTokenManager.getToken.mockReturnValue(null)
        mockTokenManager.getTokenClaims.mockReturnValue({})
        
        render(
          <TestWrapper>
            <TestPermissionComponent />
          </TestWrapper>
        )

        expect(screen.getByTestId('user-permissions')).toBeEmptyDOMElement()
      })
    })

    describe('getEffectivePermissions', () => {
      it('should return same as getUserPermissions', () => {
        const mockUser = createMockUser(['Admin'])
        let permissionContext: any
        
        // Set up mocks before render
        mockTokenManager.getToken.mockReturnValue('valid-token')
        mockTokenManager.getTokenClaims.mockReturnValue({ 
          permission: ['users.read', 'admin.access'] 
        })
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        const userPermissions = permissionContext?.getUserPermissions()
        const effectivePermissions = permissionContext?.getEffectivePermissions()
        
        expect(effectivePermissions).toEqual(userPermissions)
      })
    })

    describe('getRoleHierarchy', () => {
      it('should return correct hierarchy level for SuperAdmin', () => {
        const mockUser = createMockUser(['SuperAdmin'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.getRoleHierarchy()).toBe(0) // Highest level
      })

      it('should return correct hierarchy level for Admin', () => {
        const mockUser = createMockUser(['Admin'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.getRoleHierarchy()).toBe(2) // Admin is index 2
      })

      it('should return highest role level when user has multiple roles', () => {
        const mockUser = createMockUser(['User', 'Manager', 'Admin'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.getRoleHierarchy()).toBe(2) // Admin (highest role)
      })

      it('should return maximum level for unknown roles', () => {
        const mockUser = createMockUser(['UnknownRole'])
        let permissionContext: any
        
        render(
          <TestWrapper mockUser={mockUser}>
            <TestPermissionComponent onPermissionCheck={(ctx) => permissionContext = ctx} />
          </TestWrapper>
        )

        expect(permissionContext?.getRoleHierarchy()).toBe(6) // Max hierarchy level
      })
    })
  })

  describe('Error handling and edge cases', () => {
    it('should handle token manager errors gracefully', () => {
      // Set up mocks to throw errors
      mockTokenManager.getToken.mockImplementation(() => {
        throw new Error('Token manager error')
      })
      
      render(
        <TestWrapper>
          <TestPermissionComponent />
        </TestWrapper>
      )

      // Should still render without crashing
      expect(screen.getByTestId('has-users-read')).toBeInTheDocument()
      expect(screen.getByTestId('user-permissions')).toBeEmptyDOMElement()
    })

    it('should handle malformed JWT claims gracefully', () => {
      // Set up mocks before render - malformed claims
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.getTokenClaims.mockReturnValue(null)
      
      render(
        <TestWrapper mockUser={createMockUser([])}>  {/* Empty roles to avoid fallback */}
          <TestPermissionComponent />
        </TestWrapper>
      )

      expect(screen.getByTestId('user-permissions')).toBeEmptyDOMElement()
      expect(screen.getByTestId('user-roles')).toBeEmptyDOMElement()
    })

    it('should handle null user gracefully', () => {
      render(
        <TestWrapper mockUser={null}>
          <TestPermissionComponent />
        </TestWrapper>
      )

      expect(screen.getByTestId('has-users-read')).toHaveTextContent('false')
      expect(screen.getByTestId('has-admin-role')).toHaveTextContent('false')
      expect(screen.getByTestId('is-admin')).toHaveTextContent('false')
      expect(screen.getByTestId('user-roles')).toBeEmptyDOMElement()
    })

    it('should handle complex role object arrays from API', () => {
      const userWithRoleObjects = createMockUser([
        { name: 'Admin', id: 1 },
        { roleName: 'Manager', id: 2 },
        'User' // Mixed format
      ] as any)
      
      // Set up mocks - force fallback to user object
      mockTokenManager.getToken.mockReturnValue(null) // Force fallback to user object
      mockTokenManager.getTokenClaims.mockReturnValue({})
      
      render(
        <TestWrapper mockUser={userWithRoleObjects}>
          <TestPermissionComponent />
        </TestWrapper>
      )

      const roles = screen.getByTestId('user-roles').textContent
      expect(roles).toContain('Admin')
      expect(roles).toContain('Manager')
      expect(roles).toContain('User')
    })
  })

  describe('Console logging', () => {
    it('should log permission checks for debugging', () => {
      const consoleSpy = vi.spyOn(console, 'log')
      
      // Set up mocks before render
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.getTokenClaims.mockReturnValue({ 
        permission: ['users.read'] 
      })
      
      render(
        <TestWrapper>
          <TestPermissionComponent />
        </TestWrapper>
      )

      expect(consoleSpy).toHaveBeenCalledWith(
        expect.stringContaining('PermissionContext: Permission check:'),
        expect.any(Object)
      )
    })

    it('should log role extraction process', () => {
      const consoleSpy = vi.spyOn(console, 'log')
      
      // Set up mocks before render
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.getTokenClaims.mockReturnValue({ 
        role: ['Admin', 'User']
      })
      
      render(
        <TestWrapper>
          <TestPermissionComponent />
        </TestWrapper>
      )

      expect(consoleSpy).toHaveBeenCalledWith(
        expect.stringContaining('PermissionContext: Extracting roles from JWT token:'),
        expect.any(Object)
      )
    })

    it('should log admin checks', () => {
      const consoleSpy = vi.spyOn(console, 'log')
      
      // Set up mocks before render
      mockTokenManager.getToken.mockReturnValue('valid-token')
      mockTokenManager.getTokenClaims.mockReturnValue({ 
        permission: ['users.edit'] 
      })
      
      render(
        <TestWrapper 
          mockUser={createMockUser(['Admin'])}
        >
          <TestPermissionComponent />
        </TestWrapper>
      )

      expect(consoleSpy).toHaveBeenCalledWith(
        expect.stringContaining('PermissionContext: Multi-role admin check:'),
        expect.any(Object)
      )
    })
  })
})
