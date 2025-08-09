import { createContext, useContext, type ReactNode } from 'react';
import { useAuth } from './AuthContext.js';

interface PermissionContextValue {
  hasPermission: (permission: string) => boolean;
  hasAnyPermission: (permissions: string[]) => boolean;
  hasAllPermissions: (permissions: string[]) => boolean;
  hasRole: (roleName: string) => boolean;
  hasAnyRole: (roleNames: string[]) => boolean;
  getUserPermissions: () => string[];
  getUserRoles: () => string[];
}

const PermissionContext = createContext<PermissionContextValue | undefined>(undefined);

interface PermissionProviderProps {
  children: ReactNode;
}

export function PermissionProvider({ children }: PermissionProviderProps) {
  const { user, permissions, isAuthenticated } = useAuth();

  const hasPermission = (permission: string): boolean => {
    if (!isAuthenticated || !permissions) return false;
    return permissions.includes(permission);
  };

  const hasAnyPermission = (requiredPermissions: string[]): boolean => {
    if (!isAuthenticated || !permissions) return false;
    return requiredPermissions.some(permission => permissions.includes(permission));
  };

  const hasAllPermissions = (requiredPermissions: string[]): boolean => {
    if (!isAuthenticated || !permissions) return false;
    return requiredPermissions.every(permission => permissions.includes(permission));
  };

  const hasRole = (roleName: string): boolean => {
    if (!isAuthenticated || !user?.roles) return false;
    return user.roles.some(role => role.name === roleName);
  };

  const hasAnyRole = (roleNames: string[]): boolean => {
    if (!isAuthenticated || !user?.roles) return false;
    return roleNames.some(roleName => 
      user.roles.some(role => role.name === roleName)
    );
  };

  const getUserPermissions = (): string[] => {
    return permissions || [];
  };

  const getUserRoles = (): string[] => {
    return user?.roles?.map(role => role.name) || [];
  };

  const value: PermissionContextValue = {
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    hasRole,
    hasAnyRole,
    getUserPermissions,
    getUserRoles,
  };

  return (
    <PermissionContext.Provider value={value}>
      {children}
    </PermissionContext.Provider>
  );
}

export function usePermissions() {
  const context = useContext(PermissionContext);
  if (context === undefined) {
    throw new Error('usePermissions must be used within a PermissionProvider');
  }
  return context;
}
