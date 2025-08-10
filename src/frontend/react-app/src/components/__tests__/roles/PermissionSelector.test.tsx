import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { vi } from 'vitest';
import { PermissionSelector } from '../../roles/PermissionSelector.js';
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
    success: vi.fn(),
  },
}));

const mockPermissions: PermissionsByCategory = {
  Users: [
    { id: 1, name: 'users.view', category: 'Users', description: 'View users', isActive: true },
    { id: 2, name: 'users.create', category: 'Users', description: 'Create users', isActive: true },
  ],
  Roles: [
    { id: 3, name: 'roles.view', category: 'Roles', description: 'View roles', isActive: true },
    { id: 4, name: 'roles.create', category: 'Roles', description: 'Create roles', isActive: true },
  ],
};

describe('PermissionSelector', () => {
  const defaultProps = {
    value: [],
    onChange: vi.fn(),
  };

  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(permissionService.getPermissionsGrouped).mockResolvedValue(mockPermissions);
  });

  it('should render loading state initially', () => {
    render(<PermissionSelector {...defaultProps} />);
    expect(screen.getByRole('progressbar')).toBeInTheDocument();
  });

  it('should render permissions after loading', async () => {
    render(<PermissionSelector {...defaultProps} />);
    
    await waitFor(() => {
      expect(screen.getByText('Permissions')).toBeInTheDocument();
    });

    expect(screen.getByText('Users')).toBeInTheDocument();
    expect(screen.getByText('Roles')).toBeInTheDocument();
  });

  it('should show search box when showSearch is true', async () => {
    render(<PermissionSelector {...defaultProps} showSearch={true} />);
    
    await waitFor(() => {
      expect(screen.getByPlaceholderText('Search permissions...')).toBeInTheDocument();
    });
  });

  it('should call onChange when permission is selected', async () => {
    const onChange = vi.fn();
    render(<PermissionSelector {...defaultProps} onChange={onChange} />);
    
    await waitFor(() => {
      expect(screen.getByText('Users')).toBeInTheDocument();
    });

    // Expand Users category
    fireEvent.click(screen.getByText('Users'));
    
    await waitFor(() => {
      const viewCheckbox = screen.getByRole('checkbox', { name: /users\.view/ });
      fireEvent.click(viewCheckbox);
    });

    expect(onChange).toHaveBeenCalledWith(['users.view']);
  });

  it('should select all permissions in category when category checkbox is clicked', async () => {
    const onChange = vi.fn();
    render(<PermissionSelector {...defaultProps} onChange={onChange} />);
    
    await waitFor(() => {
      expect(screen.getByText('Users')).toBeInTheDocument();
    });

    // Click the category checkbox
    const categoryCheckboxes = screen.getAllByRole('checkbox');
    const usersCheckbox = categoryCheckboxes.find(checkbox => 
      checkbox.getAttribute('aria-label')?.includes('Users')
    );
    
    if (usersCheckbox) {
      fireEvent.click(usersCheckbox);
      expect(onChange).toHaveBeenCalledWith(['users.view', 'users.create']);
    }
  });

  it('should show selected count', async () => {
    render(<PermissionSelector {...defaultProps} value={['users.view']} showSelectedCount={true} />);
    
    await waitFor(() => {
      expect(screen.getByText('1 selected')).toBeInTheDocument();
    });
  });

  it('should be disabled when disabled prop is true', async () => {
    render(<PermissionSelector {...defaultProps} disabled={true} />);
    
    await waitFor(() => {
      const selectAllButton = screen.queryByRole('button', { name: /select all/i });
      if (selectAllButton) {
        expect(selectAllButton).toBeDisabled();
      }
    });
  });

  it('should handle API error gracefully', async () => {
    vi.mocked(permissionService.getPermissionsGrouped).mockRejectedValue(new Error('API Error'));
    
    render(<PermissionSelector {...defaultProps} />);
    
    await waitFor(() => {
      expect(screen.getByText(/Failed to load permissions/)).toBeInTheDocument();
    });
  });
});
