import type { ReactNode } from 'react';
import { usePermission } from '@/contexts/PermissionContext.js';

interface CanAccessProps {
  permission?: string;
  role?: string;
  permissions?: string[];
  roles?: string[];
  requireAll?: boolean;
  children: ReactNode;
  fallback?: ReactNode;
}

export function CanAccess({
  permission,
  role,
  permissions,
  roles,
  requireAll = false,
  children,
  fallback = null,
}: CanAccessProps) {
  const {
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    hasRole,
    hasAnyRole,
  } = usePermission();

  let hasAccess = true;

  // Check single permission
  if (permission && !hasPermission(permission)) {
    hasAccess = false;
  }

  // Check single role
  if (role && !hasRole(role)) {
    hasAccess = false;
  }

  // Check multiple permissions
  if (permissions && permissions.length > 0) {
    if (requireAll) {
      hasAccess = hasAccess && hasAllPermissions(permissions);
    } else {
      hasAccess = hasAccess && hasAnyPermission(permissions);
    }
  }

  // Check multiple roles
  if (roles && roles.length > 0) {
    if (requireAll) {
      hasAccess = hasAccess && roles.every(r => hasRole(r));
    } else {
      hasAccess = hasAccess && hasAnyRole(roles);
    }
  }

  return hasAccess ? <>{children}</> : <>{fallback}</>;
}
