import { http, HttpResponse } from 'msw'
import type { User, Role } from '@/types/index'
import { mockUsers, mockRoles, mockPermissions } from '../utils/test-utils.tsx' // ✅ Explicit extension

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

// ✅ Fixed: Remove the problematic map and use the exports directly
const enhancedMockUsers: User[] = Object.values(mockUsers)
const enhancedMockRoles: Role[] = Object.values(mockRoles)

// Mock tenant data
const mockTenants = [
  {
    id: '1',
    name: 'Test Tenant',
    domain: 'test.local',
    subscriptionPlan: 'Development',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  {
    id: '2',
    name: 'Test Tenant 2',
    domain: 'test2.local',
    subscriptionPlan: 'Premium',
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

// Create complete User objects that match the User interface
const createCompleteUser = (id: string, email: string, firstName: string, lastName: string): User => ({
  id,
  email,
  firstName,
  lastName,
  fullName: `${firstName} ${lastName}`,
  phoneNumber: undefined,
  timeZone: 'UTC',
  language: 'en',
  lastLoginAt: undefined,
  emailConfirmed: true,
  isActive: true,
  roles: ['User'],
  tenantId: '1',
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString(),
  preferences: {
    theme: 'light',
    language: 'en',
    timeZone: 'UTC',
    notifications: {
      email: true,
      push: true,
      sms: false
    }
  }
})

export const handlers = [

  // ========================================
  // 🔐 AUTHENTICATION ENDPOINTS
  // ========================================

  http.post(`${API_BASE_URL}/auth/login`, async ({ request }) => {
    const body = await request.json() as { email: string; password: string }

    const loginMap: Record<string, any> = {
      'superadmin@test.com': { user: mockUsers.superAdmin, token: 'mock-superadmin-token' },
      'systemadmin@test.com': { user: mockUsers.systemAdmin, token: 'mock-systemadmin-token' },
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
        user: loginData.user,
        tenant: mockTenants[0]
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
  // 🔐 TWO-PHASE AUTHENTICATION ENDPOINTS
  // ========================================

  http.post(`${API_BASE_URL}/auth/select-tenant`, async ({ request }) => {
    const body = await request.json() as { tenantId: string | number }
    const tenantId = body.tenantId.toString()
    
    const selectedTenant = mockTenants.find(t => t.id === tenantId)
    
    if (!selectedTenant) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Tenant not found'),
        { status: 404 }
      )
    }
    
    const completeUser = createCompleteUser('3', 'admin@test.com', 'Admin', 'User')
    
    return HttpResponse.json(createApiResponse({
      accessToken: `mock-phase2-token-tenant-${tenantId}`,
      refreshToken: `mock-refresh-tenant-${tenantId}`,
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user: completeUser,
      tenant: selectedTenant
    }))
  }),

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
    
    if (existingUser) {
      return HttpResponse.json(
        createApiResponse(null, false, 'User with this email already exists. Please sign in instead.'),
        { status: 400 }
      )
    }

    // Handle new user registration
    const newUserId = (Math.max(...enhancedMockUsers.map(u => parseInt(u.id))) + 1).toString()
    const newUser: User = {
      id: newUserId,
      firstName: body.firstName,
      lastName: body.lastName,
      fullName: `${body.firstName} ${body.lastName}`,
      email: body.email,
      phoneNumber: undefined,
      timeZone: 'UTC',
      language: 'en',
      lastLoginAt: undefined,
      emailConfirmed: false,
      isActive: true,
      roles: ['User'],
      tenantId: '1',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      preferences: {
        theme: 'light',
        language: 'en',
        timeZone: 'UTC',
        notifications: {
          email: true,
          push: true,
          sms: false
        }
      }
    }

    // If tenant name provided, create new tenant
    let tenant = mockTenants[0]
    if (body.tenantName) {
      const newTenantId = (mockTenants.length + 1).toString()
      tenant = {
        id: newTenantId,
        name: body.tenantName,
        domain: `${body.tenantName.toLowerCase().replace(/\s+/g, '')}.local`,
        subscriptionPlan: 'Basic',
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      }
      mockTenants.push(tenant)
    }

    return HttpResponse.json(createApiResponse({
      accessToken: `mock-new-user-token-${newUserId}`,
      refreshToken: `mock-refresh-${newUserId}`,
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user: newUser,
      tenant: tenant
    }), { status: 201 })
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

    let users = [...enhancedMockUsers]

    if (auth?.includes('viewer')) {
      users = []
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

  http.get(`${API_BASE_URL}/users/profile`, () => {
    const completeUser = createCompleteUser('1', 'admin@test.com', 'Admin', 'User')
    return HttpResponse.json(createApiResponse(completeUser))
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

    const newUserId = (Math.max(...enhancedMockUsers.map(u => parseInt(u.id))) + 1).toString()
    const newUser = createCompleteUser(newUserId, body.email, body.firstName, body.lastName)

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

  // ========================================
  // 🏢 TENANT MANAGEMENT ENDPOINTS
  // ========================================

  http.get(`${API_BASE_URL}/users/:userId/tenants`, ({ params }) => {
    const userId = params.userId as string
    
    if (userId === 'error-user') {
      return HttpResponse.json(
        createApiResponse(null, false, 'Failed to load tenants'),
        { status: 500 }
      )
    }
    
    if (userId === 'no-tenant-user') {
      return HttpResponse.json(createApiResponse([]))
    }
    
    if (userId === 'single-tenant-user' || userId === '3') {
      return HttpResponse.json(createApiResponse([mockTenants[0]]))
    }
    
    return HttpResponse.json(createApiResponse(mockTenants))
  }),

  http.get(`${API_BASE_URL}/tenants/:tenantId/settings`, ({ params }) => {
    const tenantId = params.tenantId as string
    
    return HttpResponse.json(createApiResponse(mockTenantSettings))
  }),

  http.post(`${API_BASE_URL}/auth/switch-tenant`, async ({ request }) => {
    const body = await request.json() as { tenantId: string | number }
    const tenantId = body.tenantId.toString()
    
    const targetTenant = mockTenants.find(t => t.id === tenantId)
    
    if (!targetTenant) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Tenant not found'),
        { status: 404 }
      )
    }
    
    const completeUser = createCompleteUser('3', 'admin@test.com', 'Admin', 'User')
    
    return HttpResponse.json(createApiResponse({
      accessToken: `mock-switched-token-tenant-${tenantId}`,
      refreshToken: `mock-switched-refresh-tenant-${tenantId}`,
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user: completeUser,
      tenant: targetTenant
    }))
  }),

  // ========================================
  // 🛡️ ROLE MANAGEMENT ENDPOINTS
  // ========================================

  // 🔧 CRITICAL FIX: Add both role endpoint patterns
  http.get(`${API_BASE_URL}/roles`, ({ request }) => {
    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')
    const searchTerm = url.searchParams.get('searchTerm')

    console.log('🎯 MSW: Handling full URL roles request', { page, pageSize, searchTerm })

    let roles = [...enhancedMockRoles]

    if (searchTerm) {
      const term = searchTerm.toLowerCase()
      roles = roles.filter((role: Role) =>
        role.name.toLowerCase().includes(term) ||
        (role.description && role.description.toLowerCase().includes(term))
      )
    }

    const response = createPagedResponse(roles, page, pageSize)
    console.log('🎯 MSW: Returning full URL roles response', response)
    
    return HttpResponse.json(response)
  }),

  // 🔧 CRITICAL FIX: Direct /api/roles handler
  http.get('/api/roles', ({ request }) => {
    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')
    const searchTerm = url.searchParams.get('searchTerm')

    console.log('🎯 MSW: Handling direct /api/roles request', { page, pageSize, searchTerm })

    let roles = [...enhancedMockRoles]

    if (searchTerm) {
      const term = searchTerm.toLowerCase()
      roles = roles.filter((role: Role) =>
        role.name.toLowerCase().includes(term) ||
        (role.description && role.description.toLowerCase().includes(term))
      )
    }

    const response = createPagedResponse(roles, page, pageSize)
    console.log('🎯 MSW: Returning direct /api/roles response', response)
    
    return HttpResponse.json(response)
  }),

  http.get(`${API_BASE_URL}/roles/:id`, ({ params }) => {
    const roleId = parseInt(params.id as string)
    const role = enhancedMockRoles.find((r: Role) => r.id === roleId)

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

  // ========================================
  // 🧪 ERROR SIMULATION FOR TESTING
  // ========================================

  http.get(`${API_BASE_URL}/test/error`, () => {
    return HttpResponse.json(
      createApiResponse(null, false, 'Simulated server error'),
      { status: 500 }
    )
  }),

  // Simulate getUserTenants failure
  http.get(`${API_BASE_URL}/users/error-user/tenants`, () => {
    return HttpResponse.json(
      createApiResponse(null, false, 'Failed to load tenants'),
      { status: 500 }
    )
  }),

  // Simulate selectTenant failure
  http.post(`${API_BASE_URL}/auth/select-tenant-error`, () => {
    return HttpResponse.json(
      createApiResponse(null, false, 'Failed to select tenant'),
      { status: 500 }
    )
  }),

  // Simulate switchTenant failure
  http.post(`${API_BASE_URL}/auth/switch-tenant-error`, () => {
    return HttpResponse.json(
      createApiResponse(null, false, 'Failed to switch tenant'),
      { status: 500 }
    )
  }),

]
