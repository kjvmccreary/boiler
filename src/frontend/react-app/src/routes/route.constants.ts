export const ROUTES = {
  // Public routes
  HOME: '/',
  LOGIN: '/login',
  REGISTER: '/register',
  FORGOT_PASSWORD: '/forgot-password',
  RESET_PASSWORD: '/reset-password/:token',
  EMAIL_CONFIRMATION: '/confirm-email',
  UNAUTHORIZED: '/unauthorized',

  // Protected routes
  DASHBOARD: '/dashboard',
  PROFILE: '/profile',
  
  // Admin routes
  USERS: '/users',
  USER_DETAILS: '/users/:id',
  ROLES: '/roles',
  ROLE_DETAILS: '/roles/:id',
  PERMISSIONS: '/permissions',
  SETTINGS: '/settings',
} as const;

export const NAVIGATION_ITEMS = [
  {
    label: 'Dashboard',
    path: ROUTES.DASHBOARD,
    icon: 'Dashboard',
    permission: undefined, // Available to all authenticated users
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
