import { http, HttpResponse } from 'msw'
import type { User } from '@/types/index.js'
import { mockUsers, mockRoles, mockPermissions } from '../utils/test-utils.js'

const API_BASE_URL = 'http://localhost:5000/api'

// 🔧 .NET 9 API Response Wrapper Function
const createApiResponse = <T>(data: T, success = true, message?: string) => ({
  success,
  data,
  message,
  errors: success ? [] : [message || 'Operation failed']
})

// 🔧 .NET 9 Paginated Response Helper
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

// 🔧 Enhanced Mock Data for .NET 9 Structure
const enhancedMockUsers = Object.values(mockUsers).map(user => ({
  ...user,
  phoneNumber: user.phoneNumber || null,
  timeZone: user.timeZone || 'UTC',
  language: user.language || 'en',
  lastLoginAt: user.lastLoginAt || null
}))

const enhancedMockRoles = Object.values(mockRoles)

export const handlers = [
  
  // ========================================
  // 🔐 AUTHENTICATION ENDPOINTS
  // ========================================
  
  http.post(`${API_BASE_URL}/auth/login`, async ({ request }) => {
    const body = await request.json() as { email: string; password: string }
    
    // Enhanced login responses for different user types
    const loginMap: Record<string, any> = {
      'superadmin@test.com': {
        user: mockUsers.superAdmin,
        token: 'mock-superadmin-token'
      },
      'admin@test.com': {
        user: mockUsers.admin,
        token: 'mock-admin-token'
      },
      'manager@test.com': {
        user: mockUsers.manager,
        token: 'mock-manager-token'
      },
      'user@test.com': {
        user: mockUsers.user,
        token: 'mock-user-token'
      },
      'viewer@test.com': {
        user: mockUsers.viewer,
        token: 'mock-viewer-token'
      }
    }
    
    const loginData = loginMap[body.email]
    
    if (loginData && body.password === 'password') {
      return HttpResponse.json(createApiResponse({
        accessToken: loginData.token,
        refreshToken: 'mock-refresh-token',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        tokenType: 'Bearer',
        user: loginData.user,
        tenant: {
          id: '1',
          name: 'Test Tenant',
          isActive: true,
          createdAt: '2024-01-01T00:00:00Z',
          updatedAt: '2024-01-01T00:00:00Z'
        }
      }))
    }
    
    return HttpResponse.json(
      createApiResponse(null, false, 'Invalid credentials'),
      { status: 401 }
    )
  }),

  http.post(`${API_BASE_URL}/auth/register`, async ({ request }) => {
    const body = await request.json() as any
    
    return HttpResponse.json(createApiResponse({
      accessToken: 'mock-new-user-token',
      refreshToken: 'mock-refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user: {
        id: Date.now().toString(),
        email: body.email,
        firstName: body.firstName,
        lastName: body.lastName,
        fullName: `${body.firstName} ${body.lastName}`,
        emailConfirmed: false,
        isActive: true,
        roles: ['User'],
        tenantId: 'tenant-1',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      },
      tenant: {
        id: '1',
        name: 'Test Tenant',
        isActive: true,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      }
    }))
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
    
    return HttpResponse.json(createApiResponse(true, true, 'Password changed successfully'))
  }),

  http.post(`${API_BASE_URL}/auth/reset-password`, async ({ request }) => {
    const body = await request.json() as { email?: string; token?: string; newPassword?: string }
    
    if (body.email && !body.token) {
      // Request password reset
      return HttpResponse.json(createApiResponse(true, true, 'Password reset email sent'))
    } else if (body.token && body.newPassword) {
      // Complete password reset
      if (body.token === 'valid-reset-token') {
        return HttpResponse.json(createApiResponse(true, true, 'Password reset successfully'))
      } else {
        return HttpResponse.json(
          createApiResponse(null, false, 'Invalid or expired reset token'),
          { status: 400 }
        )
      }
    }
    
    return HttpResponse.json(
      createApiResponse(null, false, 'Invalid request'),
      { status: 400 }
    )
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

  http.get(`${API_BASE_URL}/auth/validate-token`, ({ request }) => {
    const auth = request.headers.get('Authorization')
    
    if (auth?.startsWith('Bearer mock-')) {
      const tokenType = auth.split('mock-')[1]?.split('-')[0]
      const user = tokenType ? mockUsers[tokenType as keyof typeof mockUsers] : mockUsers.user
      
      return HttpResponse.json(createApiResponse(user))
    }
    
    return HttpResponse.json(
      createApiResponse(null, false, 'Invalid token'),
      { status: 401 }
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
    const sortBy = url.searchParams.get('sortBy') || 'fullName'
    const sortDirection = url.searchParams.get('sortDirection') || 'asc'
    
    let users = [...enhancedMockUsers]
    
    // Apply search filter
    if (searchTerm) {
      const term = searchTerm.toLowerCase()
      users = users.filter(user => 
        user.firstName.toLowerCase().includes(term) ||
        user.lastName.toLowerCase().includes(term) ||
        user.email.toLowerCase().includes(term) ||
        user.fullName.toLowerCase().includes(term)
      )
    }
    
    // Apply sorting
    users.sort((a, b) => {
      const aVal = (a as any)[sortBy] || ''
      const bVal = (b as any)[sortBy] || ''
      const comparison = aVal.localeCompare(bVal)
      return sortDirection === 'desc' ? -comparison : comparison
    })
    
    return HttpResponse.json(createPagedResponse(users, page, pageSize))
  }),

  http.get(`${API_BASE_URL}/users/profile`, ({ request }) => {
    const auth = request.headers.get('Authorization')
    
    if (auth?.startsWith('Bearer mock-')) {
      const tokenType = auth.split('mock-')[1]?.split('-')[0]
      const user = tokenType ? mockUsers[tokenType as keyof typeof mockUsers] : mockUsers.user
      
      return HttpResponse.json(createApiResponse(user))
    }
    
    return HttpResponse.json(
      createApiResponse(null, false, 'Unauthorized'),
      { status: 401 }
    )
  }),

  http.put(`${API_BASE_URL}/users/profile`, async ({ request }) => {
    const body = await request.json() as Partial<User>
    const auth = request.headers.get('Authorization')
    
    if (auth?.startsWith('Bearer mock-')) {
      const tokenType = auth.split('mock-')[1]?.split('-')[0]
      const user = tokenType ? mockUsers[tokenType as keyof typeof mockUsers] : mockUsers.user
      
      const updatedUser = {
        ...user,
        ...body,
        fullName: body.firstName && body.lastName ? `${body.firstName} ${body.lastName}` : user.fullName,
        updatedAt: new Date().toISOString()
      }
      
      return HttpResponse.json(createApiResponse(updatedUser))
    }
    
    return HttpResponse.json(
      createApiResponse(null, false, 'Unauthorized'),
      { status: 401 }
    )
  }),

  http.get(`${API_BASE_URL}/users/:id`, ({ params }) => {
    const userId = params.id as string
    const user = enhancedMockUsers.find(u => u.id === userId)
    
    if (user) {
      return HttpResponse.json(createApiResponse(user))
    }
    
    return HttpResponse.json(
      createApiResponse(null, false, 'User not found'),
      { status: 404 }
    )
  }),

  http.put(`${API_BASE_URL}/users/:id`, async ({ params, request }) => {
    const userId = params.id as string
    const body = await request.json() as Partial<User>
    const user = enhancedMockUsers.find(u => u.id === userId)
    
    if (user) {
      const updatedUser = {
        ...user,
        ...body,
        fullName: body.firstName && body.lastName ? `${body.firstName} ${body.lastName}` : user.fullName,
        updatedAt: new Date().toISOString()
      }
      
      return HttpResponse.json(createApiResponse(updatedUser))
    }
    
    return HttpResponse.json(
      createApiResponse(null, false, 'User not found'),
      { status: 404 }
    )
  }),

  http.delete(`${API_BASE_URL}/users/:id`, ({ params }) => {
    const userId = params.id as string
    const user = enhancedMockUsers.find(u => u.id === userId)
    
    if (user) {
      return HttpResponse.json(createApiResponse(true))
    }
    
    return HttpResponse.json(
      createApiResponse(null, false, 'User not found'),
      { status: 404 }
    )
  }),

  http.post(`${API_BASE_URL}/users`, async ({ request }) => {
    const body = await request.json() as {
      firstName: string;
      lastName: string;
      email: string;
      password?: string;
      roles?: string[];
    }
    
    // Validate required fields
    if (!body.firstName || !body.lastName || !body.email) {
      return HttpResponse.json(
        createApiResponse(null, false, 'First name, last name, and email are required'),
        { status: 400 }
      )
    }
    
    // Check for duplicate email
    const existingUser = enhancedMockUsers.find(u => u.email === body.email)
    if (existingUser) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Email already exists'),
        { status: 409 }
      )
    }
    
    // Create new user
    const newUser = {
      id: (Math.max(...enhancedMockUsers.map(u => parseInt(u.id))) + 1).toString(),
      firstName: body.firstName,
      lastName: body.lastName,
      fullName: `${body.firstName} ${body.lastName}`,
      email: body.email,
      emailConfirmed: false,
      isActive: true,
      roles: body.roles || ['User'],
      tenantId: 'tenant-1',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    }
    
    return HttpResponse.json(createApiResponse(newUser), { status: 201 })
  }),

  // ========================================
  // 🛡️ ROLE MANAGEMENT ENDPOINTS
  // ========================================
  
  http.get(`${API_BASE_URL}/roles`, ({ request }) => {
    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')
    const searchTerm = url.searchParams.get('searchTerm')
    const includeSystem = url.searchParams.get('includeSystem') === 'true'
    
    let roles = [...enhancedMockRoles]
    
    // Filter system roles if not requested
    if (!includeSystem) {
      roles = roles.filter(role => !role.isSystemRole)
    }
    
    // Apply search filter
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

  http.post(`${API_BASE_URL}/roles`, async ({ request }) => {
    const body = await request.json() as {
      name: string
      description?: string
      permissions: string[]
    }
    
    // Validate required fields
    if (!body.name || !Array.isArray(body.permissions)) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Name and permissions are required'),
        { status: 400 }
      )
    }
    
    // Check for duplicate role name
    const existingRole = enhancedMockRoles.find(r => r.name === body.name)
    if (existingRole) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Role name already exists'),
        { status: 409 }
      )
    }
    
    // Create new role
    const newRole = {
      id: Math.max(...enhancedMockRoles.map(r => r.id)) + 1,
      name: body.name,
      description: body.description || '',
      isSystemRole: false,
      isDefault: false,
      tenantId: 1,
      permissions: body.permissions.map(permName => {
        const allPermissions = Object.values(mockPermissions).flat()
        return allPermissions.find(p => p.name === permName)!
      }).filter(Boolean),
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      userCount: 0
    }
    
    return HttpResponse.json(createApiResponse(newRole), { status: 201 })
  }),

  http.put(`${API_BASE_URL}/roles/:id`, async ({ params, request }) => {
    const roleId = parseInt(params.id as string)
    const body = await request.json() as {
      name: string
      description?: string
      permissions: string[]
    }
    const role = enhancedMockRoles.find(r => r.id === roleId)
    
    if (!role) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Role not found'),
        { status: 404 }
      )
    }
    
    if (role.isSystemRole) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Cannot modify system role'),
        { status: 400 }
      )
    }
    
    const updatedRole = {
      ...role,
      name: body.name,
      description: body.description || role.description,
      permissions: body.permissions.map(permName => {
        const allPermissions = Object.values(mockPermissions).flat()
        return allPermissions.find(p => p.name === permName)!
      }).filter(Boolean),
      updatedAt: new Date().toISOString()
    }
    
    return HttpResponse.json(createApiResponse(updatedRole))
  }),

  http.delete(`${API_BASE_URL}/roles/:id`, ({ params }) => {
    const roleId = parseInt(params.id as string)
    const role = enhancedMockRoles.find(r => r.id === roleId)
    
    if (!role) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Role not found'),
        { status: 404 }
      )
    }
    
    if (role.isSystemRole) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Cannot delete system role'),
        { status: 400 }
      )
    }
    
    if (role.userCount && role.userCount > 0) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Cannot delete role with assigned users'),
        { status: 400 }
      )
    }
    
    return HttpResponse.json(createApiResponse(true))
  }),

  // ========================================
  // 🔐 ROLE-USER ASSIGNMENT ENDPOINTS
  // ========================================
  
  http.get(`${API_BASE_URL}/users/:id/roles`, ({ params }) => {
    const userId = params.id as string
    const user = enhancedMockUsers.find(u => u.id === userId)
    
    if (!user) {
      return HttpResponse.json(
        createApiResponse(null, false, 'User not found'),
        { status: 404 }
      )
    }
    
    // Get roles for the user based on their role names
    const userRoleNames = Array.isArray(user.roles) ? user.roles : [user.roles]
    const userRoles = enhancedMockRoles.filter(role => 
      userRoleNames.includes(role.name)
    )
    
    return HttpResponse.json(createApiResponse(userRoles))
  }),

  http.get(`${API_BASE_URL}/roles/:id/users`, ({ params }) => {
    const roleId = parseInt(params.id as string)
    const role = enhancedMockRoles.find(r => r.id === roleId)
    
    if (!role) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Role not found'),
        { status: 404 }
      )
    }
    
    // Get users with this role
    const usersWithRole = enhancedMockUsers.filter(user => {
      const userRoles = Array.isArray(user.roles) ? user.roles : [user.roles]
      return userRoles.includes(role.name)
    })
    
    return HttpResponse.json(createApiResponse(usersWithRole))
  }),

  http.post(`${API_BASE_URL}/roles/assign`, async ({ request }) => {
    const body = await request.json() as { userId: string; roleId: number }
    
    const user = enhancedMockUsers.find(u => u.id === body.userId)
    const role = enhancedMockRoles.find(r => r.id === body.roleId)
    
    if (!user || !role) {
      return HttpResponse.json(
        createApiResponse(null, false, 'User or role not found'),
        { status: 404 }
      )
    }
    
    return HttpResponse.json(createApiResponse(true, true, 'Role assigned successfully'))
  }),

  http.delete(`${API_BASE_URL}/roles/:roleId/users/:userId`, ({ params }) => {
    const roleId = parseInt(params.roleId as string)
    const userId = params.userId as string
    
    const user = enhancedMockUsers.find(u => u.id === userId)
    const role = enhancedMockRoles.find(r => r.id === roleId)
    
    if (!user || !role) {
      return HttpResponse.json(
        createApiResponse(null, false, 'User or role not found'),
        { status: 404 }
      )
    }
    
    return HttpResponse.json(createApiResponse(true, true, 'Role removed successfully'))
  }),

  // ========================================
  // 🔑 PERMISSION ENDPOINTS
  // ========================================
  
  http.get(`${API_BASE_URL}/permissions`, () => {
    const allPermissions = Object.values(mockPermissions).flat()
    return HttpResponse.json(createApiResponse(allPermissions))
  }),

  http.get(`${API_BASE_URL}/permissions/categories`, () => {
    const categories = Object.keys(mockPermissions)
    return HttpResponse.json(createApiResponse(categories))
  }),

  http.get(`${API_BASE_URL}/permissions/grouped`, () => {
    return HttpResponse.json(createApiResponse(mockPermissions))
  }),

  http.get(`${API_BASE_URL}/permissions/me`, ({ request }) => {
    const auth = request.headers.get('Authorization')
    
    if (auth?.startsWith('Bearer mock-')) {
      const tokenType = auth.split('mock-')[1]?.split('-')[0]
      
      if (tokenType && mockUsers[tokenType as keyof typeof mockUsers]) {
        const userRole = tokenType as keyof typeof mockRoles
        const role = mockRoles[userRole]
        const permissions = role ? role.permissions.map(p => p.name) : []
        
        return HttpResponse.json(createApiResponse(permissions))
      }
    }
    
    return HttpResponse.json(createApiResponse([]))
  }),

  http.get(`${API_BASE_URL}/permissions/users/:userId`, ({ params }) => {
    const userId = params.userId as string
    const user = enhancedMockUsers.find(u => u.id === userId)
    
    if (!user) {
      return HttpResponse.json(
        createApiResponse(null, false, 'User not found'),
        { status: 404 }
      )
    }
    
    // Get user's permissions based on their roles
    const userRoleNames = Array.isArray(user.roles) ? user.roles : [user.roles]
    const allPermissions = new Set<string>()
    
    userRoleNames.forEach(roleName => {
      const role = enhancedMockRoles.find(r => r.name === roleName)
      if (role) {
        role.permissions.forEach(perm => allPermissions.add(perm.name))
      }
    })
    
    return HttpResponse.json(createApiResponse(Array.from(allPermissions)))
  }),

  http.get(`${API_BASE_URL}/permissions/users/:userId/check/:permission`, ({ params }) => {
    const userId = params.userId as string
    const permission = params.permission as string
    const user = enhancedMockUsers.find(u => u.id === userId)
    
    if (!user) {
      return HttpResponse.json(
        createApiResponse(null, false, 'User not found'),
        { status: 404 }
      )
    }
    
    // Check if user has the permission
    const userRoleNames = Array.isArray(user.roles) ? user.roles : [user.roles]
    let hasPermission = false
    
    for (const roleName of userRoleNames) {
      const role = enhancedMockRoles.find(r => r.name === roleName)
      if (role && role.permissions.some(p => p.name === permission)) {
        hasPermission = true
        break
      }
    }
    
    return HttpResponse.json(createApiResponse(hasPermission))
  }),

  http.post(`${API_BASE_URL}/permissions/users/:userId/check-any`, async ({ params, request }) => {
    const userId = params.userId as string
    const requestedPermissions = await request.json() as string[] // 🔧 CHANGE THIS LINE
    const user = enhancedMockUsers.find(u => u.id === userId)
    
    if (!user) {
      return HttpResponse.json(
        createApiResponse(null, false, 'User not found'),
        { status: 404 }
      )
    }
    
    // Check if user has any of the permissions
    const userRoleNames = Array.isArray(user.roles) ? user.roles : [user.roles]
    let hasAnyPermission = false
    
    for (const roleName of userRoleNames) {
      const role = enhancedMockRoles.find(r => r.name === roleName)
      if (role && requestedPermissions.some(perm => role.permissions.some(p => p.name === perm))) { // 🔧 AND USE IT HERE
        hasAnyPermission = true
        break
      }
    }
    
    return HttpResponse.json(createApiResponse(hasAnyPermission))
  }),

  http.post(`${API_BASE_URL}/permissions/users/:userId/check-all`, async ({ params, request }) => {
    const userId = params.userId as string
    const requestedPermissions = await request.json() as string[] // 🔧 CHANGE THIS LINE
    const user = enhancedMockUsers.find(u => u.id === userId)
    
    if (!user) {
      return HttpResponse.json(
        createApiResponse(null, false, 'User not found'),
        { status: 404 }
      )
    }
    
    // Get all user permissions
    const userRoleNames = Array.isArray(user.roles) ? user.roles : [user.roles]
    const userPermissions = new Set<string>()
    
    userRoleNames.forEach(roleName => {
      const role = enhancedMockRoles.find(r => r.name === roleName)
      if (role) {
        role.permissions.forEach(perm => userPermissions.add(perm.name))
      }
    })
    
    // Check if user has all requested permissions
    const hasAllPermissions = requestedPermissions.every(perm => userPermissions.has(perm)) // 🔧 AND USE IT HERE
    
    return HttpResponse.json(createApiResponse(hasAllPermissions))
  }),

  // ========================================
  // 🔐 ROLE PERMISSIONS ENDPOINTS
  // ========================================
  
  http.get(`${API_BASE_URL}/roles/:id/permissions`, ({ params }) => {
    const roleId = parseInt(params.id as string)
    const role = enhancedMockRoles.find(r => r.id === roleId)
    
    if (!role) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Role not found'),
        { status: 404 }
      )
    }
    
    const permissionNames = role.permissions.map(p => p.name)
    return HttpResponse.json(createApiResponse(permissionNames))
  }),

  http.put(`${API_BASE_URL}/roles/:id/permissions`, async ({ params }) => {
    const roleId = parseInt(params.id as string)
    //const _permissions = await request.json() as string[] // 🔧 CHANGE FROM 'permissions' TO 'permissionNames'
    const role = enhancedMockRoles.find(r => r.id === roleId)
    
    if (!role) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Role not found'),
        { status: 404 }
      )
    }
    
    if (role.isSystemRole) {
      return HttpResponse.json(
        createApiResponse(null, false, 'Cannot modify system role permissions'),
        { status: 400 }
      )
    }
    
    // Even though we don't use permissionNames in this mock, the variable is now properly named
    return HttpResponse.json(createApiResponse(true, true, 'Role permissions updated successfully'))
  }),

  // ========================================
  // 🏥 HEALTH CHECK ENDPOINTS
  // ========================================
  
  http.get(`${API_BASE_URL.replace('/api', '')}/health`, () => {
    return HttpResponse.json({
      status: 'Healthy',
      timestamp: new Date().toISOString(),
      services: {
        database: 'Healthy',
        cache: 'Healthy',
        authentication: 'Healthy'
      }
    })
  }),

  // ========================================
  // 🧪 TEST & ERROR SIMULATION
  // ========================================
  
  http.get(`${API_BASE_URL}/test/error`, () => {
    return HttpResponse.json(
      createApiResponse(null, false, 'Simulated server error'),
      { status: 500 }
    )
  }),

  http.get(`${API_BASE_URL}/test/unauthorized`, () => {
    return HttpResponse.json(
      createApiResponse(null, false, 'Unauthorized access'),
      { status: 401 }
    )
  }),

  http.get(`${API_BASE_URL}/test/forbidden`, () => {
    return HttpResponse.json(
      createApiResponse(null, false, 'Insufficient permissions'),
      { status: 403 }
    )
  })
]
