import { createContext, useContext, type ReactNode } from 'react';
import { useAuth } from './AuthContext.js';

interface PermissionContextType {
  hasPermission: (permission: string) => boolean;
  hasAnyPermission: (permissions: string[]) => boolean;
  hasAllPermissions: (permissions: string[]) => boolean;
  hasRole: (roleName: string) => boolean;
  hasAnyRole: (roleNames: string[]) => boolean;
  getUserRoles: () => string[];
  getUserPermissions: () => string[];
}

const PermissionContext = createContext<PermissionContextType | undefined>(undefined);

interface PermissionProviderProps {
  children: ReactNode;
}

export function PermissionProvider({ children }: PermissionProviderProps) {
  const { user, permissions: authPermissions } = useAuth();

  const hasPermission = (permission: string): boolean => {
    if (!user) return false;
    
    // 🔧 FIX: Use permissions from AuthContext (extracted from JWT token)
    // This is more reliable than trying to access user.roles.permissions
    return authPermissions.includes(permission);
  };

  const hasAnyPermission = (permissions: string[]): boolean => {
    return permissions.some(permission => hasPermission(permission));
  };

  const hasAllPermissions = (permissions: string[]): boolean => {
    return permissions.every(permission => hasPermission(permission));
  };

  const hasRole = (roleName: string): boolean => {
    if (!user || !user.roles) return false;
    
    // 🔧 FIX: Safely access user.roles
    return user.roles.some(role => role.name === roleName);
  };

  const hasAnyRole = (roleNames: string[]): boolean => {
    return roleNames.some(roleName => hasRole(roleName));
  };

  const getUserRoles = (): string[] => {
    // 🔧 FIX: Safely access user.roles with null checks
    return user?.roles?.map(role => role.name) || [];
  };

  const getUserPermissions = (): string[] => {
    // 🔧 FIX: Use permissions from AuthContext instead of trying to access role.permissions
    // This avoids the "can't access property 'map', role.permissions is undefined" error
    return authPermissions || [];
  };

  const value: PermissionContextType = {
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    hasRole,
    hasAnyRole,
    getUserRoles,
    getUserPermissions,
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
