import type { ReactNode } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext.js';
import { usePermissions } from '@/contexts/PermissionContext.js';

interface ProtectedRouteProps {
  children: ReactNode;
  requirePermission?: string;
  requirePermissions?: string[];
  requireAllPermissions?: boolean;
  requireRole?: string;
  requireRoles?: string[];
  redirectTo?: string;
}

export function ProtectedRoute({
  children,
  requirePermission,
  requirePermissions,
  requireAllPermissions = false,
  requireRole,
  requireRoles,
  redirectTo = '/login',
}: ProtectedRouteProps) {
  const { isAuthenticated, isLoading } = useAuth();
  const { hasPermission, hasAnyPermission, hasAllPermissions, hasRole, hasAnyRole } = usePermissions();
  const location = useLocation();

  // Show loading state while checking authentication
  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="loading-spinner"></div>
      </div>
    );
  }

  // Redirect to login if not authenticated
  if (!isAuthenticated) {
    return <Navigate to={redirectTo} state={{ from: location }} replace />;
  }

  // Check single permission
  if (requirePermission && !hasPermission(requirePermission)) {
    return <Navigate to="/unauthorized" replace />;
  }

  // Check multiple permissions
  if (requirePermissions) {
    const hasRequiredPermissions = requireAllPermissions 
      ? hasAllPermissions(requirePermissions)
      : hasAnyPermission(requirePermissions);
    
    if (!hasRequiredPermissions) {
      return <Navigate to="/unauthorized" replace />;
    }
  }

  // Check single role
  if (requireRole && !hasRole(requireRole)) {
    return <Navigate to="/unauthorized" replace />;
  }

  // Check multiple roles
  if (requireRoles && !hasAnyRole(requireRoles)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>;
}
