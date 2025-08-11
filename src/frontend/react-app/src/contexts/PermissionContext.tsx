import { createContext, useContext, type ReactNode } from 'react';
import { useAuth } from './AuthContext.js';
import { tokenManager } from '@/utils/token.manager.js';

interface PermissionContextType {
  hasPermission: (permission: string) => boolean;
  hasAnyPermission: (permissions: string[]) => boolean;
  hasAllPermissions: (permissions: string[]) => boolean;
  hasRole: (roleName: string) => boolean;
  hasAnyRole: (roleNames: string[]) => boolean;
  getUserRoles: () => string[];
  getUserPermissions: () => string[];
  isAdmin: () => boolean; // 🔧 ADD: New admin check method
}

const PermissionContext = createContext<PermissionContextType | undefined>(undefined);

interface PermissionProviderProps {
  children: ReactNode;
}

export function PermissionProvider({ children }: PermissionProviderProps) {
  const { user, permissions: authPermissions } = useAuth();

  // 🔧 .NET 9 FIX: Get permissions from JWT token
  const getPermissionsFromToken = (): string[] => {
    try {
      const token = tokenManager.getToken();
      if (!token) return [];
      
      const claims = tokenManager.getTokenClaims(token);
      if (!claims) return [];
      
      // JWT permissions are in the 'permission' claim as an array
      const permissions = claims.permission || [];
      
      console.log('🔍 PermissionContext: Extracting permissions from JWT token:', {
        tokenClaims: Object.keys(claims),
        permissions: permissions
      });
      
      // Handle both array and string formats
      if (Array.isArray(permissions)) {
        return permissions;
      } else if (typeof permissions === 'string') {
        return permissions.split(',').map(p => p.trim()).filter(p => p.length > 0);
      }
      
      return [];
    } catch (error) {
      console.error('🔍 PermissionContext: Error extracting permissions from token:', error);
      return [];
    }
  };

  // 🔧 .NET 9 FIX: Get roles from JWT token
  const getRolesFromToken = (): string[] => {
    try {
      const token = tokenManager.getToken();
      if (!token) return [];
      
      const claims = tokenManager.getTokenClaims(token);
      if (!claims) return [];
      
      // Check multiple possible role claim names for .NET 9
      const roles = claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
                   claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'] ||
                   claims.role || 
                   claims.roles || 
                   [];
      
      console.log('🔍 PermissionContext: Extracting roles from JWT token:', {
        tokenClaims: Object.keys(claims),
        roles: roles
      });
      
      // Handle both array and string formats
      if (Array.isArray(roles)) {
        return roles;
      } else if (typeof roles === 'string') {
        return roles.split(',').map(r => r.trim()).filter(r => r.length > 0);
      }
      
      return [];
    } catch (error) {
      console.error('🔍 PermissionContext: Error extracting roles from token:', error);
      return [];
    }
  };

  const hasPermission = (permission: string): boolean => {
    if (!user) return false;
    
    // 🔧 FIX: Use permissions from JWT token (most reliable)
    const tokenPermissions = getPermissionsFromToken();
    const hasPermission = tokenPermissions.includes(permission);
    
    console.log('🔍 PermissionContext: Permission check:', {
      permission,
      hasPermission,
      allPermissions: tokenPermissions.slice(0, 10) // Show first 10 for brevity
    });
    
    return hasPermission;
  };

  const hasAnyPermission = (permissions: string[]): boolean => {
    return permissions.some(permission => hasPermission(permission));
  };

  const hasAllPermissions = (permissions: string[]): boolean => {
    return permissions.every(permission => hasPermission(permission));
  };

  const hasRole = (roleName: string): boolean => {
    if (!user) return false;
    
    // 🔧 .NET 9 FIX: Get roles from JWT token (most reliable)
    const tokenRoles = getRolesFromToken();
    const hasRole = tokenRoles.includes(roleName);
    
    console.log('🔍 PermissionContext: Role check:', {
      roleName,
      hasRole,
      tokenRoles
    });
    
    return hasRole;
  };

  const hasAnyRole = (roleNames: string[]): boolean => {
    return roleNames.some(roleName => hasRole(roleName));
  };

  // 🔧 .NET 9 FIX: New admin check based on permissions instead of roles
  const isAdmin = (): boolean => {
    if (!user) return false;
    
    // An admin user has these key administrative permissions
    const adminPermissions = [
      'users.edit',
      'users.create', 
      'users.delete',
      'roles.edit',
      'tenants.edit'
    ];
    
    // If user has any of these core admin permissions, they're an admin
    const hasAdminPermissions = hasAnyPermission(adminPermissions);
    
    console.log('🔍 PermissionContext: Admin check via permissions:', {
      hasAdminPermissions,
      userPermissions: getPermissionsFromToken().slice(0, 10) // Show first 10
    });
    
    return hasAdminPermissions;
  };

  const getUserRoles = (): string[] => {
    // 🔧 .NET 9 FIX: Get roles from token first, then fallback to user object
    const tokenRoles = getRolesFromToken();
    if (tokenRoles.length > 0) {
      return tokenRoles;
    }
    
    // Fallback to user object with safe access
    if (!user?.roles) return [];
    
    // Handle string array format
    if (typeof user.roles[0] === 'string') {
      return user.roles as string[];
    }
    
    // Handle object array format
    return user.roles.map((role: any) => 
      typeof role === 'string' ? role : role?.name || ''
    ).filter(role => role.length > 0);
  };

  const getUserPermissions = (): string[] => {
    // 🔧 FIX: Use permissions from JWT token (most reliable)
    return getPermissionsFromToken();
  };

  const value: PermissionContextType = {
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    hasRole,
    hasAnyRole,
    getUserRoles,
    getUserPermissions,
    isAdmin, // 🔧 ADD: New admin check method
  };

  return (
    <PermissionContext.Provider value={value}>
      {children}
    </PermissionContext.Provider>
  );
}

export function usePermission(): PermissionContextType {
  const context = useContext(PermissionContext);
  if (context === undefined) {
    throw new Error('usePermission must be used within a PermissionProvider');
  }
  return context;
}
