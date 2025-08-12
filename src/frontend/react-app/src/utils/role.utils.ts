// Utility functions for handling role data consistently
export const normalizeRoles = (roles: string | string[] | undefined): string[] => {
  if (!roles) return [];
  
  if (typeof roles === 'string') {
    return [roles];
  }
  
  if (Array.isArray(roles)) {
    return roles.filter(role => typeof role === 'string' && role.length > 0);
  }
  
  return [];
};

export const getRoleDisplayName = (role: unknown): string => {
  if (typeof role === 'string') {
    return role;
  }
  
  if (typeof role === 'object' && role !== null) {
    const roleObj = role as { name?: string; roleName?: string };
    return roleObj.name || roleObj.roleName || 'Unknown Role';
  }
  
  return 'Unknown Role';
};
