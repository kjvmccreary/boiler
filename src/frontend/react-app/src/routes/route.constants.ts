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

  // ✅ NEW: Workflow routes
  WORKFLOW_DEFINITIONS: '/app/workflow/definitions',
  WORKFLOW_DEFINITION_DETAILS: '/app/workflow/definitions/:id',
  WORKFLOW_BUILDER: '/app/workflow/builder/:id?',
  WORKFLOW_INSTANCES: '/app/workflow/instances',
  WORKFLOW_INSTANCE_DETAILS: '/app/workflow/instances/:id',
  WORKFLOW_MY_TASKS: '/app/workflow/tasks/mine',
  WORKFLOW_TASK_DETAILS: '/app/workflow/tasks/:id',
  WORKFLOW_ADMIN: '/app/workflow/admin',
} as const;

// ✅ FIX: Add proper TypeScript types for navigation items
interface BaseNavigationItem {
  label: string;
  icon: string;
  permission?: string;
}

interface NavigationLeafItem extends BaseNavigationItem {
  path: string;
  children?: never;
}

interface NavigationParentItem extends BaseNavigationItem {
  path?: never;
  children: NavigationLeafItem[];
}

type NavigationItem = NavigationLeafItem | NavigationParentItem;

export const NAVIGATION_ITEMS: NavigationItem[] = [
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
  // ✅ NEW: Workflow section
  {
    label: 'Workflows',
    icon: 'AccountTree',
    permission: 'workflow.read',
    children: [
      {
        label: 'My Tasks',
        path: ROUTES.WORKFLOW_MY_TASKS,
        icon: 'Assignment',
        permission: 'workflow.read',
      },
      {
        label: 'Definitions',
        path: ROUTES.WORKFLOW_DEFINITIONS,
        icon: 'Description',
        permission: 'workflow.read',
      },
      {
        label: 'Instances',
        path: ROUTES.WORKFLOW_INSTANCES,
        icon: 'PlayArrow',
        permission: 'workflow.read',
      },
      {
        label: 'Builder',
        path: '/app/workflow/builder/new',
        icon: 'Edit',
        permission: 'workflow.write',
      },
    ],
  },
  {
    label: 'Settings',
    path: ROUTES.SETTINGS,
    icon: 'Settings',
    permission: undefined,
  },
];
