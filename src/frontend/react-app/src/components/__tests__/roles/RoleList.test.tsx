import { describe, it, expect, vi, beforeEach } from 'vitest';
import React from 'react';
import { render, screen } from '@testing-library/react';
import { BrowserRouter } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

// Hoisted mocks
const { mockGetRoles, mockGetRoleUsers } = vi.hoisted(() => ({
  mockGetRoles: vi.fn(),
  mockGetRoleUsers: vi.fn()
}));

vi.mock('@/services/role.service', () => ({
  roleService: {
    getRoles: (...args: any[]) => mockGetRoles(...args),
    getRoleById: vi.fn(),
    getRoleUsers: (...args: any[]) => mockGetRoleUsers(...args),
    createRole: vi.fn(),
    updateRole: vi.fn(),
    deleteRole: vi.fn(),
    getRolePermissions: vi.fn(),
    assignRoleToUser: vi.fn(),
    removeRoleFromUser: vi.fn(),
    getUserPermissions: vi.fn(),
    getUserRoles: vi.fn(),
    getRoleByName: vi.fn()
  }
}));

vi.mock('@/contexts/AuthContext.js', () => ({
  useAuth: () => ({
    user: {
      id: '3',
      firstName: 'Admin',
      lastName: 'User',
      email: 'admin@test.com',
      permissions: ['roles.view', 'roles.edit', 'roles.delete']
    },
    isAuthenticated: true
  })
}));

vi.mock('@/contexts/TenantContext.js', () => ({
  useTenant: () => ({
    currentTenant: { id: '1', name: 'Test Tenant' },
    availableTenants: [{ id: '1', name: 'Test Tenant' }],
    isLoading: false,
    error: null
  })
}));

vi.mock('@/components/authorization/CanAccess.js', () => ({
  CanAccess: ({ children }: { children: React.ReactNode }) => <>{children}</>
}));

import { RoleList } from '@/components/roles/RoleList';

function Wrapper({ children }: { children: React.ReactNode }) {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false, gcTime: 0 } }
  });
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>{children}</BrowserRouter>
    </QueryClientProvider>
  );
}

describe('RoleList', () => {
  const mockOnEditRole = vi.fn();
  const mockOnDeleteRole = vi.fn();

  beforeEach(() => {
    vi.clearAllMocks();
    mockGetRoles.mockResolvedValue({
      roles: [
        {
          id: 1,
            name: 'Admin',
            description: 'Administrator role',
            isSystemRole: false,
            isDefault: false,
            tenantId: 1,
            permissions: [
              { id: '1', name: 'users.view', category: 'Users', description: 'View users' },
              { id: '2', name: 'users.edit', category: 'Users', description: 'Edit users' }
            ],
            userCount: 5,
            createdAt: '2024-01-01T00:00:00Z',
            updatedAt: '2024-01-01T00:00:00Z'
        },
        {
          id: 2,
          name: 'User',
          description: 'Standard user role',
          isSystemRole: true,
          isDefault: true,
          tenantId: 1,
          permissions: [
            { id: '1', name: 'users.view', category: 'Users', description: 'View users' }
          ],
          userCount: 10,
          createdAt: '2024-01-01T00:00:00Z',
          updatedAt: '2024-01-01T00:00:00Z'
        }
      ],
      pagination: {
        totalCount: 2,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1
      }
    });

    mockGetRoleUsers.mockResolvedValue([
      {
        id: 1,
        firstName: 'John',
        lastName: 'Doe',
        email: 'john@test.com',
        fullName: 'John Doe',
        isActive: true
      }
    ]);
  });

  it('renders role data (stable assertion)', async () => {
    render(
      <Wrapper>
        <RoleList onEditRole={mockOnEditRole} onDeleteRole={mockOnDeleteRole} />
      </Wrapper>
    );

    // Wait for a definitive post-loading element
    const adminCell = await screen.findByText('Admin');
    expect(adminCell).toBeInTheDocument();

    // Other role
    expect(screen.getByText('User')).toBeInTheDocument();

    // Core columns present
    expect(screen.getByText('Description')).toBeInTheDocument();
    expect(screen.getByText('Permissions')).toBeInTheDocument();
    expect(screen.getByText('Users')).toBeInTheDocument();

    // Permission counts
    expect(screen.getByText('2')).toBeInTheDocument(); // Admin role permissions length
    expect(screen.getAllByText('1').length).toBeGreaterThan(0); // User role permissions count or user counts

    // System role should have disabled edit button
    const editButtons = screen.getAllByTestId('edit-button');
    const deleteButtons = screen.getAllByTestId('delete-button');

    // Expect at least one enabled (Admin) and one disabled (system role)
    const editDisabledStates = editButtons.map(btn => (btn as HTMLButtonElement).disabled);
    expect(editDisabledStates).toContain(true);
    expect(editDisabledStates).toContain(false);

    // Delete buttons: system role disabled; custom role maybe enabled/disabled depending on userCount > 0
    const deleteDisabledStates = deleteButtons.map(btn => (btn as HTMLButtonElement).disabled);
    expect(deleteDisabledStates).toContain(true);

    // Service called at least once
    expect(mockGetRoles).toHaveBeenCalledTimes(1);
  });

  it('shows loading state', () => {
    // Force unresolved promise to keep loading
    mockGetRoles.mockImplementation(() => new Promise(() => {}));

    render(
      <Wrapper>
        <RoleList onEditRole={mockOnEditRole} onDeleteRole={mockOnDeleteRole} />
      </Wrapper>
    );

    expect(screen.getByRole('progressbar')).toBeInTheDocument();
    expect(screen.getByText(/Loading roles for/i)).toBeInTheDocument();
  });
});
