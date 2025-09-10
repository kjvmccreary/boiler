export interface ActiveTasksCount {
  total: number;
  available: number;
  assignedToMe: number;
  assignedToMyRoles: number;
  claimed: number;
  inProgress: number;
  overdue: number;
  failed: number;
}

export const emptyActiveTasksCount: ActiveTasksCount = {
  total: 0,
  available: 0,
  assignedToMe: 0,
  assignedToMyRoles: 0,
  claimed: 0,
  inProgress: 0,
  overdue: 0,
  failed: 0
};

export function mergeActiveTasksCount(a?: ActiveTasksCount, b?: Partial<ActiveTasksCount>): ActiveTasksCount {
  return { ...(a ?? emptyActiveTasksCount), ...(b ?? {}) };
}
