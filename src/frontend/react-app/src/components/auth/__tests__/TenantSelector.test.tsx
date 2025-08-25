import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { TenantSelector } from '../TenantSelector.tsx'

// Enhanced mock tenant data
const mockSingleTenant = [
  {
    id: '1',
    name: 'Single Tenant',
    domain: 'single.test',
    subscriptionPlan: 'Premium',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  }
]

const mockMultipleTenants = [
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

// Create a mock context provider that accepts different states
const createMockTenantContext = (overrides = {}) => ({
  currentTenant: null,
  availableTenants: mockMultipleTenants,
  switchTenant: vi.fn(),
  tenantSettings: {},
  isLoading: false,
  error: null,
  showTenantSelector: true,
  setShowTenantSelector: vi.fn(),
  completeTenantSelection: vi.fn(),
  shouldRedirectToDashboard: false,
  clearRedirectFlag: vi.fn(),
  refreshUserTenants: vi.fn().mockResolvedValue([]),
  ...overrides
})

// ✅ FIX: Create the mock function properly
const mockUseTenant = vi.fn()

// ✅ FIX: Move the mock setup outside and use factory function
vi.mock('@/contexts/TenantContext.tsx', () => ({
  useTenant: () => mockUseTenant(),
  TenantProvider: ({ children }: any) => children,
}))

vi.mock('@/contexts/AuthContext.tsx', () => ({
  useAuth: () => ({
    user: { id: '1', email: 'test@example.com' },
    isAuthenticated: true,
    refreshAuth: vi.fn(),
  }),
}))

describe('TenantSelector - Enhanced Tests', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    // Reset to default mock behavior
    mockUseTenant.mockReturnValue(createMockTenantContext())
  })

  describe('Multiple Tenants', () => {
    it('should display available tenants', () => {
      const mockOnTenantSelected = vi.fn()
      
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
      
      expect(screen.getByText('Tenant One')).toBeInTheDocument()
      expect(screen.getByText('Tenant Two')).toBeInTheDocument()
      expect(screen.getByText('Select Organization')).toBeInTheDocument()
    })

    it('should call onTenantSelected when tenant is selected and continue clicked', async () => {
      const user = userEvent.setup()
      const mockOnTenantSelected = vi.fn()
      
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
      
      // Select first tenant
      await user.click(screen.getByText('Tenant One'))
      
      // Click continue button
      const continueButton = screen.getByRole('button', { name: /select organization|continue/i })
      await user.click(continueButton)
      
      expect(mockOnTenantSelected).toHaveBeenCalledWith('1')
    })

    it('should show subscription plan badges', () => {
      const mockOnTenantSelected = vi.fn()
      
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
      
      expect(screen.getByText('Basic')).toBeInTheDocument()
      expect(screen.getByText('Pro')).toBeInTheDocument()
    })
  })

  describe('Single Tenant Auto-Selection', () => {
    it('should auto-select single tenant and show continue button', () => {
      // ✅ FIX: Use the mockUseTenant function properly
      mockUseTenant.mockReturnValue(
        createMockTenantContext({
          availableTenants: mockSingleTenant,
        })
      )

      const mockOnTenantSelected = vi.fn()
      
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
      
      expect(screen.getByText('Continue')).toBeInTheDocument()
      expect(screen.getByText('Single Tenant')).toBeInTheDocument()
    })

    it('should call onTenantSelected immediately for single tenant', async () => {
      const user = userEvent.setup()
      const mockOnTenantSelected = vi.fn()

      mockUseTenant.mockReturnValue(
        createMockTenantContext({
          availableTenants: mockSingleTenant,
        })
      )
      
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
      
      const continueButton = screen.getByRole('button', { name: /continue/i })
      await user.click(continueButton)
      
      expect(mockOnTenantSelected).toHaveBeenCalledWith('1')
    })
  })

  describe('Loading and Error States', () => {
    it('should show loading state', () => {
      mockUseTenant.mockReturnValue(
        createMockTenantContext({
          isLoading: true,
          availableTenants: [],
        })
      )

      const mockOnTenantSelected = vi.fn()
      
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
      
      expect(screen.getByRole('progressbar')).toBeInTheDocument()
      expect(screen.getByText('Loading your organizations...')).toBeInTheDocument()
    })

    it('should show error state', () => {
      mockUseTenant.mockReturnValue(
        createMockTenantContext({
          error: 'Failed to load tenants',
          availableTenants: [],
        })
      )

      const mockOnTenantSelected = vi.fn()
      
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
      
      expect(screen.getByText('Failed to load tenants')).toBeInTheDocument()
    })

    it('should show no tenants message', () => {
      mockUseTenant.mockReturnValue(
        createMockTenantContext({
          availableTenants: [],
          isLoading: false,
        })
      )

      const mockOnTenantSelected = vi.fn()
      
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
      
      expect(screen.getByText(/you don't have access to any organizations/i)).toBeInTheDocument()
    })

    it('should disable continue button when no tenant selected', () => {
      const mockOnTenantSelected = vi.fn()
      
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
      
      const continueButton = screen.getByRole('button')
      expect(continueButton).toBeDisabled()
    })
  })

  describe('Selection Interaction', () => {
    it('should enable continue button after tenant selection', async () => {
      const user = userEvent.setup()
      const mockOnTenantSelected = vi.fn()
      
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
      
      // Initially disabled
      const continueButton = screen.getByRole('button')
      expect(continueButton).toBeDisabled()
      
      // Select tenant
      await user.click(screen.getByText('Tenant One'))
      
      // Should be enabled now
      expect(continueButton).not.toBeDisabled()
    })

    it('should show visual selection feedback', async () => {
      const user = userEvent.setup()
      const mockOnTenantSelected = vi.fn()
      
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
      
      const tenantButton = screen.getByText('Tenant One').closest('button')
      
      await user.click(screen.getByText('Tenant One'))
      
      // Should show selected state (this depends on your actual implementation)
      expect(tenantButton).toHaveAttribute('aria-selected', 'true')
    })
  })
})
