import type { ReactNode } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { Box, CircularProgress } from '@mui/material';
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
  // 🔧 NEW: Add flag to indicate if this is post-tenant-switch
  redirectToAccessibleRoute?: boolean;
}

export function ProtectedRoute({
  children,
  requirePermission,
  requireRole,
  requirePermissions,
  requireRoles,
  requireAll = false,
  redirectTo = '/login',
  redirectToAccessibleRoute = false, // 🔧 NEW: Default to false for backward compatibility
}: ProtectedRouteProps) {
  const { user, isAuthenticated, isLoading } = useAuth();
  const location = useLocation();
  const {
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    hasRole,
    hasAnyRole,
  } = usePermission();

  // 🔧 FIX: Show loading while authentication is being checked
  if (isLoading) {
    return (
      <Box 
        display="flex" 
        justifyContent="center" 
        alignItems="center" 
        minHeight="100vh"
      >
        <CircularProgress />
      </Box>
    );
  }

  // Check if user is authenticated (only after loading is complete)
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

  // 🔧 NEW: Smart redirect logic for tenant switching
  if (!hasAccess) {
    // Check if this might be a post-tenant-switch scenario
    const isPostTenantSwitch = sessionStorage.getItem('tenant_switch_in_progress') === 'true';
    
    if (isPostTenantSwitch || redirectToAccessibleRoute) {
      console.log('🔧 ProtectedRoute: Access denied after tenant switch, redirecting to dashboard');
      
      // Clear the flag
      sessionStorage.removeItem('tenant_switch_in_progress');
      
      // Redirect to dashboard instead of unauthorized
      return <Navigate to="/app/dashboard" replace />;
    }
    
    // Normal permission denial - redirect to unauthorized
    return <Navigate to="/unauthorized" replace />;
  }

  return <>{children}</>;
}
