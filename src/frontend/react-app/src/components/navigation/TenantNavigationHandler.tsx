import { useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useTenant } from '@/contexts/TenantContext.js';
import { usePermission } from '@/contexts/PermissionContext.js';

export function TenantNavigationHandler() {
  const navigate = useNavigate();
  const location = useLocation();
  const { shouldRedirectToDashboard, clearRedirectFlag } = useTenant();
  const { hasPermission } = usePermission();

  useEffect(() => {
    if (shouldRedirectToDashboard) {
      console.log('🔧 TenantNavigationHandler: Redirect flag detected, checking current route accessibility');
      
      // Check if current route requires permissions that user might not have
      const protectedRoutes = [
        { path: '/app/users', permission: 'users.view' },
        { path: '/app/roles', permission: 'roles.view' },
      ];
      
      const currentRoute = protectedRoutes.find(route => 
        location.pathname.startsWith(route.path)
      );
      
      let shouldRedirect = false;
      
      if (currentRoute) {
        const hasAccess = hasPermission(currentRoute.permission);
        console.log('🔧 TenantNavigationHandler: Current route access check:', {
          path: location.pathname,
          requiredPermission: currentRoute.permission,
          hasAccess
        });
        
        if (!hasAccess) {
          shouldRedirect = true;
        }
      } else if (sessionStorage.getItem('tenant_switch_in_progress') === 'true') {
        // If tenant switch is in progress and we're not sure about route access, redirect as safety
        shouldRedirect = true;
      }
      
      if (shouldRedirect) {
        console.log('🔧 TenantNavigationHandler: Redirecting to dashboard due to insufficient permissions');
        navigate('/app/dashboard', { replace: true });
      }
      
      // Clear the redirect flag
      clearRedirectFlag();
    }
  }, [shouldRedirectToDashboard, location.pathname, navigate, clearRedirectFlag, hasPermission]);

  return null; // This component doesn't render anything
}
