import { http, HttpResponse } from 'msw'
import type { User } from '@/types/index.js'
import { mockUsers, mockRoles, mockPermissions } from '../utils/test-utils.js'

const API_BASE_URL = 'http://localhost:5000/api'

// .NET 9 API Response Wrapper
const createApiResponse = <T>(data: T, success = true, message?: string) => ({
  success,
  data,
  message,
  errors: success ? [] : [message || 'Operation failed']
})

const createPagedResponse = <T>(items: T[], page: number, pageSize: number, totalCount?: number) => {
  const total = totalCount ?? items.length
  const startIndex = (page - 1) * pageSize
  const endIndex = startIndex + pageSize
  const pagedItems = items.slice(startIndex, endIndex)

  return createApiResponse({
    items: pagedItems,
    totalCount: total,
    pageNumber: page,
    pageSize: pageSize,
    totalPages: Math.ceil(total / pageSize)
  })
}

const enhancedMockUsers = Object.values(mockUsers).map(user => ({
  ...user,
  phoneNumber: user.phoneNumber || null,
  timeZone: user.timeZone || 'UTC',
  language: user.language || 'en',
  lastLoginAt: user.lastLoginAt || null
}))

const enhancedMockRoles = Object.values(mockRoles)

// 🔧 NEW: Mock tenant data
const mockTenants = [
  {
    id: '1',
    name: 'Default Tenant',
    domain: 'localhost',
    subscriptionPlan: 'Development',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  }
]

const mockTenantSettings = {
  theme: {
    primaryColor: '#1976d2',
    companyName: 'Default Tenant'
  },
  features: {
    multiUser: true,
    reports: true,
    analytics: false
  },
  subscriptionPlan: 'Development'
}

export const handlers = [

  // ========================================
  // 🔐 AUTHENTICATION ENDPOINTS
  // ========================================

  http.post(`${API_BASE_URL}/auth/login`, async ({ request }) => {
    const body = await request.json() as { email: string; password: string }

    const loginMap: Record<string, any> = {
      'superadmin@test.com': { user: mockUsers.superAdmin, token: 'mock-superadmin-token' },
      'admin@test.com': { user: mockUsers.admin, token: 'mock-admin-token' },
      'manager@test.com': { user: mockUsers.manager, token: 'mock-manager-token' },
      'user@test.com': { user: mockUsers.user, token: 'mock-user-token' },
      'viewer@test.com': { user: mockUsers.viewer, token: 'mock-viewer-token' }
    }

    const loginData = loginMap[body.email]

    if (loginData && body.password === 'password') {
      return HttpResponse.json(createApiResponse({
        accessToken: loginData.token,
        refreshToken: 'mock-refresh-token',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        tokenType: 'Bearer',
        user: loginData.user
      }))
    }

    return HttpResponse.json(
      createApiResponse(null, false, 'Invalid credentials'),
      { status: 401 }
    )
  }),

  http.post(`${API_BASE_URL}/auth/logout`, () => {
    return HttpResponse.json(createApiResponse(true))
  }),

  http.post(`${API_BASE_URL}/auth/refresh`, async ({ request }) => {
    const body = await request.json() as { refreshToken: string }

    if (body.refreshToken === 'mock-refresh-token') {
      return HttpResponse.json(createApiResponse({
        accessToken: 'new-mock-token',
        refreshToken: 'new-mock-refresh-token',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        tokenType: 'Bearer'
      }))
    }

    return HttpResponse.json(
      createApiResponse(null, false, 'Invalid refresh token'),
      { status: 401 }
    )
  }),

  http.post(`${API_BASE_URL}/auth/change-password`, async ({ request }) => {
    const body = await request.json() as {
      currentPassword: string;
      newPassword: string;
      confirmNewPassword: string;
    }

    if (body.currentPassword === 'wrongpassword') {
      return HttpResponse.json(
        createApiResponse(null, false, 'Current password is incorrect'),
        { status: 400 }
      )
    }

    if (body.newPassword !== body.confirmNewPassword) {
      return HttpResponse.json(
        createApiResponse(null, false, 'New password and confirmation do not match'),
        { status: 400 }
      )
    }

    return HttpResponse.json(createApiResponse(true))
  }),

  http.post(`${API_BASE_URL}/auth/confirm-email`, async ({ request }) => {
    const body = await request.json() as { token: string }

    const tokenMap: Record<string, { success: boolean, message: string, status?: number }> = {
      'valid-token': { success: true, message: 'Email confirmed successfully' },
      'expired-token': { success: false, message: 'The confirmation link has expired', status: 400 },
      'invalid-token': { success: false, message: 'The confirmation link is invalid', status: 400 }
    }

    const result = tokenMap[body.token] || { success: false, message: 'Token not found', status: 404 }

    return HttpResponse.json(
      createApiResponse(result.success, result.success, result.message),
      { status: result.status || 200 }
    )
  }),

  // ========================================
  // 👥 USER MANAGEMENT ENDPOINTS  
  // ========================================

  http.get(`${API_BASE_URL}/users`, ({ request }) => {
    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')
    const searchTerm = url.searchParams.get('searchTerm')
    const auth = request.headers.get('Authorization')

    // Role-based filtering for API permission integration tests
    let users = [...enhancedMockUsers]

    if (auth?.includes('viewer')) {
      users = [] // Viewers can't see user list
    }

    if (searchTerm) {
      const term = searchTerm.toLowerCase()
      users = users.filter(user =>
        user.firstName.toLowerCase().includes(term) ||
        user.lastName.toLowerCase().includes(term) ||
        user.email.toLowerCase().includes(term)
      )
    }

    return HttpResponse.json(createPagedResponse(users, page, pageSize))
  }),

  http.post(`${API_BASE_URL}/users`, async ({ request }) => {
    const body = await request.json() as any
    const auth = request.headers.get('Authorization')

    if (!auth?.includes('admin')) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Insufficient permissions'),
        { status: 403 }
      )
    }

    const newUser = {
      id: (Math.max(...enhancedMockUsers.map(u => parseInt(u.id))) + 1).toString(),
      firstName: body.firstName,
      lastName: body.lastName,
      fullName: `${body.firstName} ${body.lastName}`,
      email: body.email,
      emailConfirmed: false,
      isActive: true,
      roles: body.roles || ['User'],
      tenantId: '1', // 🔧 FIX: Use string ID
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    }

    return HttpResponse.json(createApiResponse(newUser), { status: 201 })
  }),

  http.delete(`${API_BASE_URL}/users/:id`, ({ params, request }) => {
    const userId = params.id as string
    const auth = request.headers.get('Authorization')

    if (!auth?.includes('admin')) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Insufficient permissions'),
        { status: 403 }
      )
    }

    const user = enhancedMockUsers.find(u => u.id === userId)
    if (user) {
      return HttpResponse.json(createApiResponse(true))
    }

    return HttpResponse.json(
      createApiResponse(null, false, 'User not found'),
      { status: 404 }
    )
  }),

  // 🔧 NEW: Add tenant endpoints for tests
  // ========================================
  // 🏢 TENANT MANAGEMENT ENDPOINTS
  // ========================================

  http.get(`${API_BASE_URL}/users/:userId/tenants`, ({ params }) => {
    const userId = params.userId as string
    
    // Return mock tenant data for any user
    return HttpResponse.json(createApiResponse(mockTenants))
  }),

  http.get(`${API_BASE_URL}/tenants/:tenantId/settings`, ({ params }) => {
    const tenantId = params.tenantId as string
    
    return HttpResponse.json(createApiResponse(mockTenantSettings))
  }),

  http.post(`${API_BASE_URL}/auth/switch-tenant`, async ({ request }) => {
    const body = await request.json() as { tenantId: string }
    
    return HttpResponse.json(createApiResponse(true, true, 'Tenant switched successfully'))
  }),

  // ========================================
  // 🛡️ ROLE MANAGEMENT ENDPOINTS
  // ========================================

  http.get(`${API_BASE_URL}/roles`, ({ request }) => {
    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')
    const searchTerm = url.searchParams.get('searchTerm')

    let roles = [...enhancedMockRoles]

    if (searchTerm) {
      const term = searchTerm.toLowerCase()
      roles = roles.filter(role =>
        role.name.toLowerCase().includes(term) ||
        (role.description && role.description.toLowerCase().includes(term))
      )
    }

    return HttpResponse.json(createPagedResponse(roles, page, pageSize))
  }),

  http.get(`${API_BASE_URL}/roles/:id`, ({ params }) => {
    const roleId = parseInt(params.id as string)
    const role = enhancedMockRoles.find(r => r.id === roleId)

    if (role) {
      return HttpResponse.json(createApiResponse(role))
    }

    return HttpResponse.json(
      createApiResponse(null, false, 'Role not found'),
      { status: 404 }
    )
  }),

  // ========================================
  // 🔑 PERMISSION ENDPOINTS
  // ========================================

  http.get(`${API_BASE_URL}/permissions`, () => {
    const allPermissions = Object.values(mockPermissions).flat()
    return HttpResponse.json(createApiResponse(allPermissions))
  }),

  http.get(`${API_BASE_URL}/permissions/grouped`, () => {
    return HttpResponse.json(createApiResponse(mockPermissions))
  }),

  // ========================================
  // 🔐 API INTEGRATION FIXES
  // ========================================

  // Fix for API permission integration tests
  http.post('/api/users', ({ request }) => {
    const auth = request.headers.get('Authorization')
    if (auth?.includes('admin')) {
      return HttpResponse.json({ success: true, data: { id: '1', name: 'New User' } })
    }
    return HttpResponse.json(
      { success: false, message: 'Insufficient permissions' },
      { status: 403 }
    )
  }),

  http.delete('/api/users/:id', ({ request }) => {
    const auth = request.headers.get('Authorization')
    if (auth?.includes('admin')) {
      return HttpResponse.json({ success: true })
    }
    return HttpResponse.json(
      { success: false, message: 'Insufficient permissions' },
      { status: 403 }
    )
  }),

  // Fix for missing GET /api/users handler
  http.get('/api/users', ({ request }) => {
    const auth = request.headers.get('Authorization')

    if (auth?.includes('viewer')) {
      return HttpResponse.json({
        success: true,
        data: { items: [], totalCount: 0 }
      })
    }

    if (auth?.includes('admin')) {
      return HttpResponse.json({
        success: true,
        data: {
          items: [
            { id: '1', name: 'John Doe' },
            { id: '2', name: 'Jane Smith' }
          ],
          totalCount: 2
        }
      })
    }

    return HttpResponse.json(
      { success: false, message: 'Unauthorized' },
      { status: 401 }
    )
  }),

  // ========================================
  // 🧪 TEST ERROR SIMULATION
  // ========================================

  http.get(`${API_BASE_URL}/test/error`, () => {
    return HttpResponse.json(
      createApiResponse(null, false, 'Simulated server error'),
      { status: 500 }
    )
  }),

  // Replace the registration handler around line 420:
  http.post(`${API_BASE_URL}/auth/register`, async ({ request }) => {
    const body = await request.json() as {
      email: string;
      password: string;
      confirmPassword: string;
      firstName: string;
      lastName: string;
      tenantName?: string;
    }

    // Validate request
    if (body.password !== body.confirmPassword) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Passwords do not match'),
        { status: 400 }
      )
    }

    // Check if user already exists
    const existingUser = enhancedMockUsers.find(user => user.email === body.email)
    
    // 🔧 NEW: Handle existing user creating new tenant (consultant use case)
    if (existingUser && body.tenantName) {
      console.log(`🏢 CONSULTANT FLOW: ${body.email} creating tenant "${body.tenantName}"`)
      
      // Check if tenant already exists
      const existingTenant = mockTenants.find(t => t.name === body.tenantName)
      if (existingTenant) {
        return HttpResponse.json(
          createApiResponse(null, false, `Organization "${body.tenantName}" already exists`),
          { status: 400 }
        )
      }
      
      // Create new tenant
      const newTenantId = (mockTenants.length + 1).toString()
      const newTenant = {
        id: newTenantId,
        name: body.tenantName,
        domain: `${body.tenantName.toLowerCase().replace(/\s+/g, '')}.local`,
        subscriptionPlan: 'Basic',
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      }
      
      mockTenants.push(newTenant)
      
      // 🔧 DECISION POINT: What to do with different personal info?
      
      // Option A: Keep existing user info (recommended)
      console.log(`📝 KEEPING existing user info: ${existingUser.firstName} ${existingUser.lastName}`)
      console.log(`📝 IGNORING form input: ${body.firstName} ${body.lastName}`)
      
      // Option B: Update user info (alternative)
      // existingUser.firstName = body.firstName
      // existingUser.lastName = body.lastName
      // existingUser.fullName = `${body.firstName} ${body.lastName}`
      // console.log(`📝 UPDATED user info to: ${body.firstName} ${body.lastName}`)
      
      // 🔧 NOTE: Password is NOT updated - existing user keeps their original password
      
      // Return success with existing user + new tenant
      return HttpResponse.json(createApiResponse({
        accessToken: `mock-consultant-token-${existingUser.id}-${newTenantId}`,
        refreshToken: `mock-refresh-${existingUser.id}-${newTenantId}`,
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        tokenType: 'Bearer',
        user: existingUser, // Using existing user info
        tenant: newTenant
      }), { status: 201 })
    }
    
    // 🔧 EXISTING: Handle existing user without tenant creation
    if (existingUser) {
      return HttpResponse.json(
        createApiResponse(null, false, 'User with this email already exists. Please sign in instead.'),
        { status: 400 }
      )
    }

    // 🔧 EXISTING: Continue with new user registration...
    // ... rest of existing code for new users
  }),
]
