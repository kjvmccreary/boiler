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
  const { user } = useAuth();

  const hasPermission = (permission: string): boolean => {
    if (!user) return false;
    
    // Get all permissions from user's roles
    const userPermissions = user.roles.flatMap(role => 
      role.permissions.map(p => p.name)
    );
    
    return userPermissions.includes(permission);
  };

  const hasAnyPermission = (permissions: string[]): boolean => {
    return permissions.some(permission => hasPermission(permission));
  };

  const hasAllPermissions = (permissions: string[]): boolean => {
    return permissions.every(permission => hasPermission(permission));
  };

  const hasRole = (roleName: string): boolean => {
    if (!user) return false;
    return user.roles.some(role => role.name === roleName);
  };

  const hasAnyRole = (roleNames: string[]): boolean => {
    return roleNames.some(roleName => hasRole(roleName));
  };

  const getUserRoles = (): string[] => {
    return user?.roles?.map(role => role.name) || [];
  };

  const getUserPermissions = (): string[] => {
    if (!user) return [];
    
    return user.roles.flatMap(role => 
      role.permissions.map(p => p.name)
    );
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
