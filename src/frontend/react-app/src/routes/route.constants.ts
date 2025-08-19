export const ROUTES = {
  // Public routes
  HOME: '/',
  WELCOME: '/welcome',
  LOGIN: '/login',
  REGISTER: '/register',
  FORGOT_PASSWORD: '/forgot-password',
  RESET_PASSWORD: '/reset-password/:token',
  EMAIL_CONFIRMATION: '/confirm-email',
  UNAUTHORIZED: '/unauthorized',

  // 🔧 FIX: Add /app prefix to ALL protected routes
  DASHBOARD: '/app/dashboard',
  PROFILE: '/app/profile',
  CHANGE_PASSWORD: '/app/change-password',
  
  // Admin routes
  USERS: '/app/users',
  USER_DETAILS: '/app/users/:id',
  USER_NEW: '/app/users/new',
  USER_ROLES: '/app/users/:id/roles',
  ROLES: '/app/roles',
  ROLE_DETAILS: '/app/roles/:id',
  ROLE_NEW: '/app/roles/new',
  ROLE_EDIT: '/app/roles/:id/edit',
  PERMISSIONS: '/app/permissions',
  SETTINGS: '/app/settings',
} as const;

export const NAVIGATION_ITEMS = [
  {
    label: 'Dashboard',
    path: ROUTES.DASHBOARD,
    icon: 'Dashboard',
    permission: undefined,
  },
  {
    label: 'Users',
    path: ROUTES.USERS,
    icon: 'People',
    permission: 'users.view',
  },
  {
    label: 'Roles',
    path: ROUTES.ROLES,
    icon: 'Security',
    permission: 'roles.view',
  },
  {
    label: 'Settings',
    path: ROUTES.SETTINGS,
    icon: 'Settings',
    permission: undefined,
  },
] as const;
