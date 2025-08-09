import type { ReactNode } from 'react';
import { usePermissions } from '@/contexts/PermissionContext.js';
import { useAuth } from '@/contexts/AuthContext.js';

interface CanAccessProps {
  children: ReactNode;
  permission?: string;
  permissions?: string[];
  requireAll?: boolean;
  role?: string;
  roles?: string[];
  requireAuthentication?: boolean;
  fallback?: ReactNode;
}

export function CanAccess({
  children,
  permission,
  permissions,
  requireAll = false,
  role,
  roles,
  requireAuthentication = true,
  fallback = null,
}: CanAccessProps) {
  const { isAuthenticated } = useAuth();
  const { hasPermission, hasAnyPermission, hasAllPermissions, hasRole, hasAnyRole } = usePermissions();

  // Check authentication first
  if (requireAuthentication && !isAuthenticated) {
    return <>{fallback}</>;
  }

  // If no specific permissions or roles required, just check authentication
  if (!permission && !permissions && !role && !roles) {
    return <>{children}</>;
  }

  // Check single permission
  if (permission && !hasPermission(permission)) {
    return <>{fallback}</>;
  }

  // Check multiple permissions
  if (permissions) {
    const hasRequiredPermissions = requireAll 
      ? hasAllPermissions(permissions)
      : hasAnyPermission(permissions);
    
    if (!hasRequiredPermissions) {
      return <>{fallback}</>;
    }
  }

  // Check single role
  if (role && !hasRole(role)) {
    return <>{fallback}</>;
  }

  // Check multiple roles
  if (roles && !hasAnyRole(roles)) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
}
