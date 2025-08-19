import { describe, it, expect, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { TenantSelector } from '../TenantSelector.js'
import { TenantProvider } from '@/contexts/TenantContext.js'

// Mock tenant data
const mockTenants = [
  {
    id: '1',
    name: 'Tenant One',
    domain: 'tenant1.test',
    subscriptionPlan: 'Basic',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  {
    id: '2', 
    name: 'Tenant Two',
    domain: 'tenant2.test',
    subscriptionPlan: 'Pro',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  }
]

// Mock the tenant context
const mockTenantContext = {
  currentTenant: null,
  availableTenants: mockTenants,
  switchTenant: vi.fn(),
  tenantSettings: {},
  isLoading: false,
  error: null,
  showTenantSelector: true,
  setShowTenantSelector: vi.fn(),
  completeTenantSelection: vi.fn(),
}

vi.mock('@/contexts/TenantContext.js', () => ({
  useTenant: () => mockTenantContext,
  TenantProvider: ({ children }: any) => children,
}))

describe('TenantSelector', () => {
  it('should display available tenants', () => {
    const mockOnTenantSelected = vi.fn()
    
    render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
    
    expect(screen.getByText('Tenant One')).toBeInTheDocument()
    expect(screen.getByText('Tenant Two')).toBeInTheDocument()
  })

  it('should call onTenantSelected when tenant is selected and continue clicked', async () => {
    const user = userEvent.setup()
    const mockOnTenantSelected = vi.fn()
    
    render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
    
    // Select first tenant
    await user.click(screen.getByText('Tenant One'))
    
    // 🔧 FIX: Use getByRole to target the button specifically instead of text
    const continueButton = screen.getByRole('button', { name: /select organization|continue/i })
    await user.click(continueButton)
    
    expect(mockOnTenantSelected).toHaveBeenCalledWith('1')
  })
})
