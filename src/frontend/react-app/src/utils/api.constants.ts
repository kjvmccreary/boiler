export const API_ENDPOINTS = {
  // Auth endpoints
  AUTH: {
    LOGIN: '/api/auth/login',
    REGISTER: '/api/auth/register',
    LOGOUT: '/api/auth/logout',
    REFRESH: '/api/auth/refresh',
    CHANGE_PASSWORD: '/api/auth/change-password',
    FORGOT_PASSWORD: '/api/auth/reset-password', // Backend endpoint
    RESET_PASSWORD: '/api/auth/reset-password',
    CONFIRM_EMAIL: '/api/auth/confirm-email',
    VALIDATE_TOKEN: '/api/auth/validate-token',
  },
  
  // User endpoints
  USERS: {
    BASE: '/api/users',
    PROFILE: '/api/users/profile',
    BY_ID: (id: string) => `/api/users/${id}`,
    ROLES: (id: string) => `/api/users/${id}/roles`,
  },
  
  // Role endpoints
  ROLES: {
    BASE: '/api/roles',
    BY_ID: (id: string) => `/api/roles/${id}`,
    PERMISSIONS: (id: string) => `/api/roles/${id}/permissions`,
    ASSIGN_USER: (roleId: string, userId: string) => `/api/roles/${roleId}/users/${userId}`,
  },
  
  // Permission endpoints
  PERMISSIONS: {
    BASE: '/api/permissions',
    CATEGORIES: '/api/permissions/categories',
    GROUPED: '/api/permissions/grouped',
    USER_PERMISSIONS: (userId: number) => `/api/permissions/users/${userId}`,
    CHECK_PERMISSION: (userId: number, permission: string) => `/api/permissions/users/${userId}/check/${permission}`,
    CHECK_ANY: (userId: number) => `/api/permissions/users/${userId}/check-any`,
    CHECK_ALL: (userId: number) => `/api/permissions/users/${userId}/check-all`,
    ME: '/api/permissions/me',
  },
  
  // Health check
  HEALTH: '/health',
} as const;

export const PERMISSIONS = {
  // User permissions
  USERS_VIEW: 'users.view',
  USERS_CREATE: 'users.create',
  USERS_EDIT: 'users.edit',
  USERS_DELETE: 'users.delete',
  USERS_MANAGE_ROLES: 'users.manage_roles',
  
  // Role permissions
  ROLES_VIEW: 'roles.view',
  ROLES_CREATE: 'roles.create',
  ROLES_EDIT: 'roles.edit',
  ROLES_DELETE: 'roles.delete',
  ROLES_ASSIGN: 'roles.assign',
  
  // Admin permissions
  ADMIN_ACCESS: 'admin.access',
  ADMIN_SYSTEM_SETTINGS: 'admin.system_settings',
  ADMIN_VIEW_LOGS: 'admin.view_logs',
  
  // Settings permissions
  SETTINGS_VIEW: 'settings.view',
  SETTINGS_EDIT: 'settings.edit',
} as const;
