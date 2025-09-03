import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { TenantSelector } from '../TenantSelector.tsx';

// Fixtures
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
];

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
];

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
});

const mockUseTenant = vi.fn();

vi.mock('@/contexts/TenantContext.tsx', () => ({
  useTenant: () => mockUseTenant(),
  TenantProvider: ({ children }: any) => children,
}));

vi.mock('@/contexts/AuthContext.tsx', () => ({
  useAuth: () => ({
    user: { id: '1', email: 'test@example.com' },
    isAuthenticated: true,
    refreshAuth: vi.fn(),
  }),
}));

describe('TenantSelector - Enhanced Tests', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockUseTenant.mockReturnValue(createMockTenantContext());
  });

  describe('Multiple Tenants', () => {
    it('displays tenants and heading (no duplicate query issues)', () => {
      const mockOnTenantSelected = vi.fn();
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />);

      expect(screen.getByTestId('tenant-selector-title')).toHaveTextContent('Select Organization');
      expect(screen.getByTestId('tenant-name-1')).toHaveTextContent('Tenant One');
      expect(screen.getByTestId('tenant-name-2')).toHaveTextContent('Tenant Two');
      // Button also has same visible text; we do not use getByText to avoid collision.
      expect(screen.getByTestId('tenant-continue-button')).toBeInTheDocument();
    });

    it('calls onTenantSelected when tenant selected then continue clicked', async () => {
      const user = userEvent.setup();
      const mockOnTenantSelected = vi.fn();
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />);

      await user.click(screen.getByTestId('tenant-item-1'));
      await user.click(screen.getByTestId('tenant-continue-button'));

      expect(mockOnTenantSelected).toHaveBeenCalledWith('1');
    });

    it('shows subscription plan badges', () => {
      const mockOnTenantSelected = vi.fn();
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />);

      expect(screen.getByTestId('tenant-plan-1')).toHaveTextContent('Basic');
      expect(screen.getByTestId('tenant-plan-2')).toHaveTextContent('Pro');
    });
  });

  describe('Single Tenant Auto-Selection', () => {
    it('auto-selects the single tenant and shows Continue button', () => {
      mockUseTenant.mockReturnValue(
        createMockTenantContext({
          availableTenants: mockSingleTenant
        })
      );
      const mockOnTenantSelected = vi.fn();
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />);

      expect(screen.getByTestId('tenant-name-1')).toHaveTextContent('Single Tenant');
      expect(screen.getByTestId('tenant-continue-button')).toHaveTextContent('Continue');
    });

    it('invokes onTenantSelected after clicking continue', async () => {
      const user = userEvent.setup();
      mockUseTenant.mockReturnValue(
        createMockTenantContext({
          availableTenants: mockSingleTenant
        })
      );
      const mockOnTenantSelected = vi.fn();
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />);

      await user.click(screen.getByTestId('tenant-continue-button'));
      expect(mockOnTenantSelected).toHaveBeenCalledWith('1');
    });
  });

  describe('Loading and Error States', () => {
    it('shows loading state', () => {
      mockUseTenant.mockReturnValue(
        createMockTenantContext({
          isLoading: true,
          availableTenants: []
        })
      );
      render(<TenantSelector onTenantSelected={vi.fn()} />);
      expect(screen.getByTestId('tenant-selector-loading')).toBeInTheDocument();
      expect(screen.getByRole('progressbar')).toBeInTheDocument();
    });

    it('shows error state', () => {
      mockUseTenant.mockReturnValue(
        createMockTenantContext({
          error: 'Failed to load tenants',
          availableTenants: []
        })
      );
      render(<TenantSelector onTenantSelected={vi.fn()} />);
      expect(screen.getByTestId('tenant-selector-error')).toHaveTextContent('Failed to load tenants');
    });

    it('shows no tenants message', () => {
      mockUseTenant.mockReturnValue(
        createMockTenantContext({
          availableTenants: [],
          isLoading: false
        })
      ); 
      render(<TenantSelector onTenantSelected={vi.fn()} />);
      expect(screen.getByTestId('tenant-selector-empty')).toBeInTheDocument();
    });

    it('disables continue button when nothing selected', () => {
      const mockOnTenantSelected = vi.fn();
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />);
      const actionButton = screen.getByTestId('tenant-continue-button');
      expect(actionButton).toBeDisabled();
    });
  });

  describe('Selection Interaction', () => {
    it('enables continue button after selecting a tenant', async () => {
      const user = userEvent.setup();
      const mockOnTenantSelected = vi.fn();
      render(<TenantSelector onTenantSelected={mockOnTenantSelected} />);

      const actionButton = screen.getByTestId('tenant-continue-button');
      expect(actionButton).toBeDisabled();

      await user.click(screen.getByTestId('tenant-item-1'));
      expect(actionButton).not.toBeDisabled();
    });

    it('shows visual selection feedback (aria-selected)', async () => {
      const user = userEvent.setup();
      render(<TenantSelector onTenantSelected={vi.fn()} />);

      const item = screen.getByTestId('tenant-item-1');
      expect(item).toHaveAttribute('aria-selected', 'false');

      await user.click(item);
      expect(item).toHaveAttribute('aria-selected', 'true');
    });
  });
});
