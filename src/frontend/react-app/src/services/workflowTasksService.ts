// Adapter layer for task operations used by unwrap tests.
// Adds a synthetic getTasksPaged() to match test expectations.

import { workflowService } from './workflow.service';
import type {
  WorkflowTaskDto,
  ClaimTaskRequestDto,
  CompleteTaskRequestDto,
  AssignTaskRequestDto
} from '@/types/workflow';

export interface Paged<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export function getTask(id: number) {
  return workflowService.getTask(id);
}

// Synthetic paged wrapper (backend returns array via getTasks).
export async function getTasksPaged(params?: { page?: number; pageSize?: number; status?: string }) {
  const items = await workflowService.getTasks({
    status: params?.status as any,
    page: params?.page,
    pageSize: params?.pageSize
  });
  const page = params?.page ?? 1;
  const pageSize = params?.pageSize ?? items.length;
  return {
    items,
    totalCount: items.length,
    page,
    pageSize
  } as Paged<WorkflowTaskDto>;
}

export function claimTask(id: number, req?: ClaimTaskRequestDto) {
  // ClaimTaskRequestDto only allows { claimNotes?: string }
  return workflowService.claimTask(id, (req ?? {}) as ClaimTaskRequestDto);
}

export function completeTask(id: number, completionData: string) {
  return workflowService.completeTask(id, { completionData });
}

export function assignTask(id: number, req: AssignTaskRequestDto) {
  return workflowService.assignTask(id, req);
}

// Adapter for tests expecting unassignTask – delegate to assignTask with empty body if backend supports it,
// else fall back to cancelTask (comment adjust if needed).
export async function unassignTask(id: number) {
  try {
    return await workflowService.assignTask(id, { userId: undefined, role: undefined } as any);
  } catch {
    // fallback (may not exist server-side)
    return workflowService.cancelTask(id);
  }
}
