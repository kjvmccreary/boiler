import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { UserMenu } from '../UserMenu.js'
import { BrowserRouter } from 'react-router-dom'
import { AuthProvider } from '@/contexts/AuthContext.js'
import { TenantProvider } from '@/contexts/TenantContext.js'
import { mockUsers, createMockTenantContext } from '@/test/utils/test-utils.js'

// Mock the route constants
vi.mock('@/routes/route.constants.js', () => ({
  ROUTES: {
    PROFILE: '/profile',
    SETTINGS: '/settings',
    LOGIN: '/login'
  }
}))

// 🔧 NEW: Mock the services to prevent actual API calls
vi.mock('@/services/tenant.service.js', () => ({
  tenantService: {
    getUserTenants: vi.fn().mockResolvedValue({
      success: true,
      data: [
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
    }),
    getTenantSettings: vi.fn().mockResolvedValue({
      success: true,
      data: {
        theme: { primaryColor: '#1976d2' },
        features: { multiUser: true },
        subscriptionPlan: 'Development'
      }
    })
  }
}))

function TestWrapper({ children }: { children: React.ReactNode }) {
  return (
    <BrowserRouter>
      <AuthProvider mockUser={mockUsers.admin} mockAuthState="authenticated" testMode>
        <TenantProvider>
          {children}
        </TenantProvider>
      </AuthProvider>
    </BrowserRouter>
  )
}

describe('UserMenu', () => {
  it('should display current tenant in user menu', async () => {
    const user = userEvent.setup()
    
    render(
      <TestWrapper>
        <UserMenu />
      </TestWrapper>
    )

    // Click to open menu
    const avatarButton = screen.getByRole('button', { name: /account of current user/i })
    await user.click(avatarButton)

    // 🔧 FIX: Use the correct tenant name that matches the mock data
    expect(screen.getByText('Default Tenant')).toBeInTheDocument()
  })

  it('should not show tenant switching option for single tenant', async () => {
    const user = userEvent.setup()
    
    render(
      <TestWrapper>
        <UserMenu />
      </TestWrapper>
    )

    const avatarButton = screen.getByRole('button', { name: /account of current user/i })
    await user.click(avatarButton)

    // Should not show switch organization option
    expect(screen.queryByText('Switch Organization')).not.toBeInTheDocument()
  })
})
