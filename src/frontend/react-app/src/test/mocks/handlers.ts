import { http, HttpResponse } from 'msw'

export const handlers = [
  // Auth endpoints
  http.post('/api/auth/login', async ({ request }) => {
    const body = await request.json() as { email: string; password: string }
    
    if (body.email === 'admin@test.com' && body.password === 'password') {
      return HttpResponse.json({
        accessToken: 'mock-access-token',
        refreshToken: 'mock-refresh-token',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        tokenType: 'Bearer',
        user: {
          id: '1',
          email: 'admin@test.com',
          firstName: 'Admin',
          lastName: 'User',
          fullName: 'Admin User',
          emailConfirmed: true,
          isActive: true,
          roles: ['Admin'],
          tenantId: '1',
          createdAt: '2024-01-01T00:00:00Z',
          updatedAt: '2024-01-01T00:00:00Z'
        },
        tenant: {
          id: '1',
          name: 'Test Tenant',
          isActive: true,
          createdAt: '2024-01-01T00:00:00Z',
          updatedAt: '2024-01-01T00:00:00Z'
        }
      })
    }
    
    return HttpResponse.json({ error: 'Invalid credentials' }, { status: 401 })
  }),

  http.post('/api/auth/register', async () => {
    return HttpResponse.json({
      accessToken: 'mock-access-token',
      refreshToken: 'mock-refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user: {
        id: '2',
        email: 'newuser@test.com',
        firstName: 'New',
        lastName: 'User',
        fullName: 'New User',
        emailConfirmed: true,
        isActive: true,
        roles: ['User'],
        tenantId: '1',
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
    })
  }),

  http.post('/api/auth/logout', () => {
    return HttpResponse.json({ success: true })
  }),

  http.post('/api/auth/reset-password', () => {
    return HttpResponse.json({ success: true, message: 'Password reset email sent' })
  }),

  // Change password endpoint
  http.post('/api/auth/change-password', async ({ request }) => {
    const body = await request.json() as {
      currentPassword: string;
      newPassword: string;
      confirmNewPassword: string;
    }
    
    // Simulate validation
    if (body.currentPassword === 'wrongpassword') {
      return HttpResponse.json(
        { success: false, message: 'Current password is incorrect.' },
        { status: 400 }
      )
    }
    
    if (body.newPassword !== body.confirmNewPassword) {
      return HttpResponse.json(
        { success: false, message: 'New password and confirmation do not match.' },
        { status: 400 }
      )
    }
    
    if (body.currentPassword === body.newPassword) {
      return HttpResponse.json(
        { success: false, message: 'New password must be different from current password.' },
        { status: 400 }
      )
    }
    
    return HttpResponse.json({
      success: true,
      message: 'Password changed successfully'
    })
  }),

  // Email confirmation endpoint
  http.post('/api/auth/confirm-email', async ({ request }) => {
    const body = await request.json() as { token: string }
    
    if (body.token === 'valid-token') {
      return HttpResponse.json({
        success: true,
        message: 'Email confirmed successfully'
      })
    }
    
    if (body.token === 'expired-token') {
      return HttpResponse.json(
        { error: 'The confirmation link has expired' },
        { status: 400 }
      )
    }
    
    if (body.token === 'invalid-token') {
      return HttpResponse.json(
        { error: 'The confirmation link is invalid' },
        { status: 400 }
      )
    }
    
    return HttpResponse.json(
      { error: 'Token not found' },
      { status: 404 }
    )
  }),

  // Permission endpoints
  http.get('/api/permissions/grouped', () => {
    return HttpResponse.json({
      Users: [
        { id: 1, name: 'users.view', category: 'Users', description: 'View users', isActive: true },
        { id: 2, name: 'users.create', category: 'Users', description: 'Create users', isActive: true },
        { id: 3, name: 'users.edit', category: 'Users', description: 'Edit users', isActive: true },
        { id: 4, name: 'users.delete', category: 'Users', description: 'Delete users', isActive: true }
      ],
      Roles: [
        { id: 5, name: 'roles.view', category: 'Roles', description: 'View roles', isActive: true },
        { id: 6, name: 'roles.create', category: 'Roles', description: 'Create roles', isActive: true },
        { id: 7, name: 'roles.edit', category: 'Roles', description: 'Edit roles', isActive: true },
        { id: 8, name: 'roles.delete', category: 'Roles', description: 'Delete roles', isActive: true }
      ]
    })
  }),

  // Roles endpoints
  http.get('/api/roles', () => {
    return HttpResponse.json([
      {
        id: '1',
        name: 'Admin',
        description: 'Administrator role',
        isSystemRole: true,
        isDefault: false,
        tenantId: '1',
        permissions: [
          { id: '1', name: 'users.view', category: 'Users', description: 'View users' },
          { id: '2', name: 'users.create', category: 'Users', description: 'Create users' }
        ],
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      },
      {
        id: '2',
        name: 'User',
        description: 'Standard user role',
        isSystemRole: false,
        isDefault: true,
        tenantId: '1',
        permissions: [
          { id: '1', name: 'users.view', category: 'Users', description: 'View users' }
        ],
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: '2024-01-01T00:00:00Z'
      }
    ])
  }),

  // Error simulation endpoint
  http.get('/api/test/error', () => {
    return HttpResponse.json({ error: 'Test error' }, { status: 500 })
  })
]
