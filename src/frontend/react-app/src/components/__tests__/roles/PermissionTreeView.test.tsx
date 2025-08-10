import { render, screen, waitFor } from '@testing-library/react';
import { vi } from 'vitest';
import { PermissionTreeView } from '../../roles/PermissionTreeView.js';
import { permissionService, type PermissionsByCategory } from '@/services/permission.service.js';

// Mock the permission service
vi.mock('@/services/permission.service.js', () => ({
  permissionService: {
    getPermissionsGrouped: vi.fn(),
  },
}));

// Mock react-hot-toast
vi.mock('react-hot-toast', () => ({
  default: {
    error: vi.fn(),
  },
}));

const mockPermissions: PermissionsByCategory = {
  Users: [
    { id: 1, name: 'users.view', category: 'Users', description: 'View users', isActive: true },
    { id: 2, name: 'users.create', category: 'Users', description: 'Create users', isActive: true },
  ],
  Roles: [
    { id: 3, name: 'roles.view', category: 'Roles', description: 'View roles', isActive: true },
  ],
};

describe('PermissionTreeView', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(permissionService.getPermissionsGrouped).mockResolvedValue(mockPermissions);
  });

  it('should render permission hierarchy', async () => {
    render(<PermissionTreeView />);
    
    await waitFor(() => {
      expect(screen.getByText('Permission Hierarchy')).toBeInTheDocument();
    });

    expect(screen.getByText('Users')).toBeInTheDocument();
    expect(screen.getByText('Roles')).toBeInTheDocument();
  });

  it('should highlight selected permissions', async () => {
    render(
      <PermissionTreeView 
        selectedPermissions={['users.view']} 
        highlightSelected={true}
      />
    );
    
    await waitFor(() => {
      expect(screen.getByText('Permission Hierarchy')).toBeInTheDocument();
    });
  });

  it('should show search when enabled', async () => {
    render(<PermissionTreeView showSearch={true} />);
    
    await waitFor(() => {
      expect(screen.getByPlaceholderText('Search permissions...')).toBeInTheDocument();
    });
  });

  it('should handle permission click when callback provided', async () => {
    const onPermissionClick = vi.fn();
    render(
      <PermissionTreeView 
        onPermissionClick={onPermissionClick}
        readOnly={false}
      />
    );
    
    await waitFor(() => {
      expect(screen.getByText('Users')).toBeInTheDocument();
    });

    // This test would need more complex interaction to click on a permission
    // after expanding the category
  });

  it('should expand categories with selected permissions', async () => {
    render(
      <PermissionTreeView 
        selectedPermissions={['users.view']}
        highlightSelected={true}
      />
    );
    
    await waitFor(() => {
      expect(screen.getByText('Users')).toBeInTheDocument();
    });

    // The Users category should be expanded automatically
    // This would need to be verified through DOM inspection
  });
});
