import { createContext, useContext, type ReactNode } from 'react';
import { useAuth } from './AuthContext.js';
import { tokenManager } from '@/utils/token.manager.js';

interface PermissionContextType {
  hasPermission: (permission: string) => boolean;
  hasAnyPermission: (permissions: string[]) => boolean;
  hasAllPermissions: (permissions: string[]) => boolean;
  hasRole: (roleName: string) => boolean;
  hasAnyRole: (roleNames: string[]) => boolean;
  hasAllRoles: (roleNames: string[]) => boolean;  // 🔧 NEW: Multi-role support
  getUserRoles: () => string[];
  getUserPermissions: () => string[];
  isAdmin: () => boolean;
}

const PermissionContext = createContext<PermissionContextType | undefined>(undefined);

interface PermissionProviderProps {
  children: ReactNode;
}

export function PermissionProvider({ children }: PermissionProviderProps) {
  const { user } = useAuth();

  // 🔧 .NET 9 MULTI-ROLE: Enhanced permission extraction from JWT token
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

  // 🔧 .NET 9 MULTI-ROLE: Enhanced role extraction from JWT token
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
        rolesRaw: roles,
        rolesType: typeof roles
      });
      
      // 🔧 MULTI-ROLE FIX: Handle single role string, array, or comma-separated string
      if (Array.isArray(roles)) {
        return roles.filter(role => role && role.length > 0);
      } else if (typeof roles === 'string' && roles.length > 0) {
        // Handle comma-separated roles in single string (some JWT implementations)
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

  // 🔧 MULTI-ROLE: Enhanced single role checking
  const hasRole = (roleName: string): boolean => {
    if (!user) return false;
    
    const userRoles = getUserRoles();
    const hasRole = userRoles.includes(roleName);
    
    console.log('🔍 PermissionContext: Single role check:', {
      roleName,
      hasRole,
      userRoles,
      checkType: 'single-role'
    });
    
    return hasRole;
  };

  // 🔧 MULTI-ROLE: Check if user has ANY of the specified roles
  const hasAnyRole = (roleNames: string[]): boolean => {
    if (!user || roleNames.length === 0) return false;
    
    const userRoles = getUserRoles();
    const hasAnyRole = roleNames.some(roleName => userRoles.includes(roleName));
    
    console.log('🔍 PermissionContext: Multi-role ANY check:', {
      requestedRoles: roleNames,
      userRoles,
      hasAnyRole,
      checkType: 'any-role'
    });
    
    return hasAnyRole;
  };

  // 🔧 MULTI-ROLE: Check if user has ALL of the specified roles
  const hasAllRoles = (roleNames: string[]): boolean => {
    if (!user || roleNames.length === 0) return false;
    
    const userRoles = getUserRoles();
    const hasAllRoles = roleNames.every(roleName => userRoles.includes(roleName));
    
    console.log('🔍 PermissionContext: Multi-role ALL check:', {
      requestedRoles: roleNames,
      userRoles,
      hasAllRoles,
      checkType: 'all-roles'
    });
    
    return hasAllRoles;
  };

  // 🔧 MULTI-ROLE: Enhanced admin check based on permissions AND roles
  const isAdmin = (): boolean => {
    if (!user) return false;
    
    // Method 1: Check for admin permissions (most reliable)
    const adminPermissions = [
      'users.edit',
      'users.create', 
      'users.delete',
      'roles.edit',
      'tenants.edit'
    ];
    
    const hasAdminPermissions = hasAnyPermission(adminPermissions);
    
    // Method 2: Check for admin roles (fallback)
    const adminRoles = ['Admin', 'SuperAdmin', 'TenantAdmin'];
    const hasAdminRole = hasAnyRole(adminRoles);
    
    const isAdminUser = hasAdminPermissions || hasAdminRole;
    
    console.log('🔍 PermissionContext: Multi-role admin check:', {
      hasAdminPermissions,
      hasAdminRole,
      isAdminUser,
      userRoles: getUserRoles(),
      userPermissions: getPermissionsFromToken().slice(0, 5)
    });
    
    return isAdminUser;
  };

  // 🔧 MULTI-ROLE: Enhanced role retrieval supporting multiple roles
  const getUserRoles = (): string[] => {
    // Priority 1: JWT token roles (most reliable for current session)
    const tokenRoles = getRolesFromToken();
    if (tokenRoles.length > 0) {
      return tokenRoles;
    }
    
    // Priority 2: User object roles (from API responses)
    if (!user?.roles) return [];
    
    // 🔧 MULTI-ROLE: Handle various role formats from API
    if (typeof user.roles === 'string') {
      return [user.roles]; // Single role
    } else if (Array.isArray(user.roles)) {
      // Handle string array (most common for RBAC)
      if (user.roles.length === 0) return [];
      
      if (typeof user.roles[0] === 'string') {
        return user.roles as string[];
      }
      
      // Handle Role object array (when populated from UserRoleAssignment)
      return user.roles.map((role: any) => {
        if (typeof role === 'string') return role;
        return role?.name || role?.roleName || '';
      }).filter(role => role.length > 0);
    }
    
    return [];
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
    hasAllRoles,        // 🔧 NEW: Multi-role support
    getUserRoles,
    getUserPermissions,
    isAdmin,
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
