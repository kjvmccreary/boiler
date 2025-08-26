import { lazy } from 'react';
import { RouteObject } from 'react-router-dom';
import { ProtectedRoute } from '@/components/authorization/ProtectedRoute';

// Lazy load workflow components
const DefinitionsPage = lazy(() => import('@/features/workflow/definitions/DefinitionsPage'));
const InstanceDetailsPage = lazy(() => import('@/features/workflow/instances/InstanceDetailsPage'));
const MyTasksPage = lazy(() => import('@/features/workflow/tasks/MyTasksPage'));

// Placeholder for BuilderPage (to be implemented next)
const BuilderPage = lazy(() => import('@/features/workflow/builder/BuilderPage'));

export const workflowRoutes: RouteObject[] = [
  {
    path: 'workflow',
    children: [
      {
        path: 'definitions',
        element: (
          <ProtectedRoute permission="workflow.read">
            <DefinitionsPage />
          </ProtectedRoute>
        ),
      },
      {
        path: 'definitions/:id',
        element: (
          <ProtectedRoute permission="workflow.read">
            <InstanceDetailsPage />
          </ProtectedRoute>
        ),
      },
      {
        path: 'instances',
        element: (
          <ProtectedRoute permission="workflow.read">
            <DefinitionsPage />
          </ProtectedRoute>
        ),
      },
      {
        path: 'instances/:id',
        element: (
          <ProtectedRoute permission="workflow.read">
            <InstanceDetailsPage />
          </ProtectedRoute>
        ),
      },
      {
        path: 'tasks/mine',
        element: (
          <ProtectedRoute permission="workflow.read">
            <MyTasksPage />
          </ProtectedRoute>
        ),
      },
      {
        path: 'tasks/:id',
        element: (
          <ProtectedRoute permission="workflow.read">
            <InstanceDetailsPage />
          </ProtectedRoute>
        ),
      },
      {
        path: 'builder/:id?',
        element: (
          <ProtectedRoute permission="workflow.write">
            <BuilderPage />
          </ProtectedRoute>
        ),
      },
    ],
  },
];
