import type { ReactNode } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext.js';
import { usePermission } from '@/contexts/PermissionContext.js';

interface ProtectedRouteProps {
  children: ReactNode;
  requirePermission?: string;
  requireRole?: string;
  requirePermissions?: string[];
  requireRoles?: string[];
  requireAll?: boolean;
  redirectTo?: string;
}

export function ProtectedRoute({
  children,
  requirePermission,
  requireRole,
  requirePermissions,
  requireRoles,
  requireAll = false,
  redirectTo = '/login',
}: ProtectedRouteProps) {
  const { user, isAuthenticated } = useAuth();
  const location = useLocation();
  const {
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    hasRole,
    hasAnyRole,
  } = usePermission();

  // Check if user is authenticated
  if (!isAuthenticated || !user) {
    return <Navigate to={redirectTo} state={{ from: location }} replace />;
  }

  let hasAccess = true;

  // Check single permission
  if (requirePermission && !hasPermission(requirePermission)) {
    hasAccess = false;
  }

  // Check single role
  if (requireRole && !hasRole(requireRole)) {
    hasAccess = false;
  }

  // Check multiple permissions
  if (requirePermissions && requirePermissions.length > 0) {
    if (requireAll) {
      hasAccess = hasAccess && hasAllPermissions(requirePermissions);
    } else {
      hasAccess = hasAccess && hasAnyPermission(requirePermissions);
    }
  }

  // Check multiple roles
  if (requireRoles && requireRoles.length > 0) {
    if (requireAll) {
      hasAccess = hasAccess && requireRoles.every(r => hasRole(r));
    } else {
      hasAccess = hasAccess && hasAnyRole(requireRoles);
    }
  }

  // Redirect to unauthorized page if no access
  if (!hasAccess) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>;
}
