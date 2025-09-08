// Adapter layer for instance operations to satisfy test imports.

import { workflowService } from './workflow.service';
import type {
  WorkflowInstanceDto,
  StartInstanceRequestDto,
  SignalInstanceRequestDto,
  RetryInstanceRequestDto,
  MoveToNodeRequestDto,
  PagedResultDto,
  InstanceRuntimeSnapshotDto
} from '@/types/workflow';

export function getInstancesPaged(filters?: any) {
  return workflowService.getInstancesPaged(filters);
}

export function getInstances(filters?: any) {
  return workflowService.getInstances(filters);
}

export function getInstance(id: number): Promise<WorkflowInstanceDto> {
  return workflowService.getInstance(id);
}

export function getInstanceStatus(id: number) {
  return workflowService.getInstanceStatus(id);
}

export function startInstance(req: StartInstanceRequestDto) {
  return workflowService.startInstance(req);
}

export function signalInstance(id: number, req: SignalInstanceRequestDto) {
  return workflowService.signalInstance(id, req);
}

export function terminateInstance(id: number) {
  return workflowService.terminateInstance(id);
}

export function suspendInstance(id: number) {
  return workflowService.suspendInstance(id);
}

export function resumeInstance(id: number) {
  return workflowService.resumeInstance(id);
}

export function retryInstance(id: number, req: RetryInstanceRequestDto) {
  return workflowService.retryInstance(id, req);
}

export function moveInstanceToNode(id: number, req: MoveToNodeRequestDto) {
  return workflowService.moveInstanceToNode(id, req);
}

export function getRuntimeSnapshot(id: number): Promise<InstanceRuntimeSnapshotDto> {
  return workflowService.getRuntimeSnapshot(id);
}
