import { http, HttpResponse } from 'msw'
import type { User, Role } from '@/types/index.ts'
import { mockUsers, mockRoles, mockPermissions } from '../utils/test-utils.tsx'

// ✅ FIX: Use relative paths for MSW to catch all requests
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

// ✅ FIX: Enhanced mock tenant data with proper scenarios
const mockTenants = [
  {
    id: 1, // ✅ Use number for backend compatibility
    name: 'Test Tenant',
    domain: 'test.local',
    subscriptionPlan: 'Development',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  {
    id: 2,
    name: 'Tenant One',  // ✅ Match test expectations
    domain: 'tenant1.test',
    subscriptionPlan: 'Basic',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  {
    id: 3,
    name: 'Tenant Two',  // ✅ Match test expectations
    domain: 'tenant2.test',
    subscriptionPlan: 'Pro',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  }
]

// ✅ FIX: Single tenant scenario
const mockSingleTenant = [
  {
    id: 1,
    name: 'Single Tenant',
    domain: 'single.local',
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
      push: false,
      sms: false
    }
  }
})

export const handlers = [
  // ✅ FIX: Enhanced tenant handlers with scenario support
  http.get('/api/users/:userId/tenants', ({ params, request }) => {
    const { userId } = params
    const url = new URL(request.url)
    const scenario = url.searchParams.get('scenario')
    
    console.log('🎯 MSW: Handling /api/users/:userId/tenants request', { userId, scenario })
    
    // ✅ FIX: Handle different scenarios based on user ID patterns
    if (scenario === 'no-tenants' || userId === '7' || userId === 'no-tenants-user') {
      console.log('🎯 MSW: Returning no tenants scenario')
      return HttpResponse.json(createApiResponse([]), { status: 200 })
    }
    
    if (scenario === 'single-tenant' || userId === '2' || userId === 'single-tenant-user') {
      console.log('🎯 MSW: Returning single tenant scenario')
      return HttpResponse.json(createApiResponse(mockSingleTenant), { status: 200 })
    }
    
    if (scenario === 'error' || userId === '6' || userId === 'error-user') {
      console.log('🎯 MSW: Returning error scenario')
      return HttpResponse.json(
        createApiResponse(null, false, 'Failed to load tenants'),
        { status: 500 }
      )
    }
    
    // Default: multiple tenants for users 3, 4, 5, etc.
    console.log('🎯 MSW: Returning multiple tenants scenario')
    return HttpResponse.json(createApiResponse(mockTenants), { status: 200 })
  }),

  // ✅ FIX: Enhanced login handler
  http.post('/api/auth/login', async ({ request }) => {
    const body = await request.json() as any
    const { email, password } = body
    
    console.log('🎯 MSW: Handling login request', { email })
    
    // ✅ FIX: Handle error scenarios
    if (email === 'error@test.com' || password === 'wrong') {
      return HttpResponse.json(
        createApiResponse(null, false, 'Invalid credentials'),
        { status: 401 }
      )
    }
    
    // ✅ FIX: Phase 1 login (no tenant context)
    const user = createCompleteUser('1', email, 'Admin', 'User')
    
    return HttpResponse.json(createApiResponse({
      accessToken: 'phase1-token-no-tenant',
      refreshToken: 'refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user,
      tenant: {
        id: '1',
        name: 'Default Tenant',
        domain: 'default.local',
        subscriptionPlan: 'Development',
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      }
    }), { status: 200 })
  }),

  // ✅ FIX: Enhanced select tenant handler
  http.post('/api/auth/select-tenant', async ({ request }) => {
    const body = await request.json() as any
    const { tenantId } = body
    
    console.log('🎯 MSW: Handling select-tenant request', { tenantId })
    
    // ✅ FIX: Handle error scenarios
    if (tenantId === 'error-tenant') {
      return HttpResponse.json(
        createApiResponse(null, false, 'Failed to select tenant'),
        { status: 500 }
      )
    }
    
    const selectedTenant = mockTenants.find(t => t.id.toString() === tenantId) || mockTenants[0]
    const user = createCompleteUser('1', 'admin@tenant1.com', 'Admin', 'User')
    
    return HttpResponse.json(createApiResponse({
      accessToken: `phase2-token-tenant-${tenantId}`,
      refreshToken: 'refresh-token-updated',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user,
      tenant: {
        id: selectedTenant.id.toString(),
        name: selectedTenant.name,
        domain: selectedTenant.domain,
        subscriptionPlan: selectedTenant.subscriptionPlan,
        isActive: selectedTenant.isActive,
        createdAt: selectedTenant.createdAt,
        updatedAt: selectedTenant.updatedAt,
      }
    }), { status: 200 })
  }),

  // ✅ FIX: Enhanced switch tenant handler
  http.post('/api/auth/switch-tenant', async ({ request }) => {
    const body = await request.json() as any
    const { tenantId } = body
    
    console.log('🎯 MSW: Handling switch-tenant request', { tenantId })
    
    // ✅ FIX: Handle error scenarios
    if (tenantId === 'error-tenant') {
      return HttpResponse.json(
        createApiResponse(null, false, 'Failed to switch tenant'),
        { status: 500 }
      )
    }
    
    const selectedTenant = mockTenants.find(t => t.id.toString() === tenantId) || mockTenants[0]
    
    return HttpResponse.json(createApiResponse({
      accessToken: `switched-token-tenant-${tenantId}`,
      refreshToken: 'refresh-token-switched',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer'
    }), { status: 200 })
  }),

  // ✅ FIX: Enhanced roles handler with delay simulation
  http.get('/api/roles', async ({ request }) => {
    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')
    const searchTerm = url.searchParams.get('searchTerm')
    
    console.log('🎯 MSW: Handling /api/roles request', { page, pageSize, searchTerm })
    
    // ✅ FIX: Add small delay to simulate real API
    await new Promise(resolve => setTimeout(resolve, 100))
    
    const response = createPagedResponse(enhancedMockRoles, page, pageSize)
    
    console.log('🎯 MSW: Returning /api/roles response', response)
    
    return HttpResponse.json(response, { status: 200 })
  }),

  // ✅ FIX: Tenant settings handler
  http.get('/api/tenants/:tenantId/settings', ({ params }) => {
    const { tenantId } = params
    console.log('🎯 MSW: Handling tenant settings request', { tenantId })
    
    return HttpResponse.json(createApiResponse(mockTenantSettings), { status: 200 })
  }),

  // ✅ FIX: Users handler
  http.get('/api/users', ({ request }) => {
    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')
    const authHeader = request.headers.get('Authorization')
    
    console.log('🎯 MSW: Handling /api/users request', { page, pageSize, authHeader })
    
    // ✅ FIX: Role-based filtering for permission tests
    if (authHeader?.includes('viewer')) {
      return HttpResponse.json(
        createPagedResponse([], page, pageSize), // Viewers see no users
        { status: 200 }
      )
    }
    
    return HttpResponse.json(
      createPagedResponse(enhancedMockUsers, page, pageSize),
      { status: 200 }
    )
  }),

  // ✅ NEW: Create user handler (for API permission tests)
  http.post('/api/users', ({ request }) => {
    const authHeader = request.headers.get('Authorization')
    
    console.log('🎯 MSW: Handling POST /api/users request', { authHeader })
    
    // ✅ FIX: Check authorization
    if (!authHeader) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Unauthorized'),
        { status: 401 }
      )
    }
    
    if (authHeader.includes('viewer')) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Insufficient permissions'),
        { status: 403 }
      )
    }
    
    // ✅ FIX: Simulate successful creation for admins
    return HttpResponse.json(
      createApiResponse({ id: '1', name: 'New User' }),
      { status: 201 }
    )
  }),

  // ✅ NEW: Delete user handler (for API permission tests)
  http.delete('/api/users/:userId', ({ params, request }) => {
    const { userId } = params
    const authHeader = request.headers.get('Authorization')
    
    console.log('🎯 MSW: Handling DELETE /api/users/:userId request', { userId, authHeader })
    
    // ✅ FIX: Check authorization
    if (!authHeader) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Unauthorized'),
        { status: 401 }
      )
    }
    
    if (!authHeader.includes('admin')) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Insufficient permissions'),
        { status: 403 }
      )
    }
    
    // ✅ FIX: Simulate successful deletion for admins
    return HttpResponse.json(
      createApiResponse({ message: 'User deleted successfully' }),
      { status: 200 }
    )
  }),

  // ✅ FIX: User profile handler
  http.get('/api/users/profile', () => {
    const user = createCompleteUser('1', 'admin@tenant1.com', 'Admin', 'User')
    
    return HttpResponse.json(createApiResponse(user), { status: 200 })
  }),

  // ✅ FIX: Permissions handler
  http.get('/api/permissions', () => {
    return HttpResponse.json(createApiResponse(mockPermissions), { status: 200 })
  }),

  // ✅ NEW: Logout handler
  http.post('/api/auth/logout', () => {
    console.log('🎯 MSW: Handling logout request')
    return HttpResponse.json(createApiResponse({ message: 'Logged out successfully' }), { status: 200 })
  }),

  // ✅ NEW: Refresh token handler
  http.post('/api/auth/refresh', () => {
    console.log('🎯 MSW: Handling refresh token request')
    return HttpResponse.json(createApiResponse({
      accessToken: 'new-access-token',
      refreshToken: 'new-refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer'
    }), { status: 200 })
  }),

  // ✅ CATCH-ALL: Handlers for missing endpoints (MUST BE AT THE END)
  http.get('/api/*', ({ request }) => {
    const url = new URL(request.url)
    console.log('🎯 MSW: Unhandled GET request caught by catch-all:', url.pathname)
    // ✅ FIX: Return success with empty data to prevent network errors
    return HttpResponse.json(createApiResponse([]), { status: 200 })
  }),

  http.post('/api/*', ({ request }) => {
    const url = new URL(request.url)
    console.log('🎯 MSW: Unhandled POST request caught by catch-all:', url.pathname)
    return HttpResponse.json(createApiResponse({ message: 'Operation completed' }), { status: 200 })
  }),

  http.delete('/api/*', ({ request }) => {
    const url = new URL(request.url)
    console.log('🎯 MSW: Unhandled DELETE request caught by catch-all:', url.pathname)
    return HttpResponse.json(createApiResponse({ message: 'Resource deleted' }), { status: 200 })
  }),

  http.put('/api/*', ({ request }) => {
    const url = new URL(request.url)
    console.log('🎯 MSW: Unhandled PUT request caught by catch-all:', url.pathname)
    return HttpResponse.json(createApiResponse({ message: 'Resource updated' }), { status: 200 })
  }),

  http.patch('/api/*', ({ request }) => {
    const url = new URL(request.url)
    console.log('🎯 MSW: Unhandled PATCH request caught by catch-all:', url.pathname)
    return HttpResponse.json(createApiResponse({ message: 'Resource updated' }), { status: 200 })
  })
]
