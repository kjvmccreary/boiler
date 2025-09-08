// Adapter layer re‑exporting definition operations from the consolidated workflow.service.
// Keeps existing test imports (`../../services/workflowDefinitionsService`) working
// without duplicating HTTP / unwrap logic.

import { workflowService } from './workflow.service';
import type {
  WorkflowDefinitionDto,
  CreateWorkflowDefinitionDto,
  UpdateWorkflowDefinitionDto,
  PublishDefinitionRequestDto,
  ValidationResultDto,
  CreateNewVersionRequestDto,
  PagedResultDto
} from '@/types/workflow';

export function getDefinitionsPaged(filters?: Parameters<typeof workflowService.getDefinitionsPaged>[0]) {
  return workflowService.getDefinitionsPaged(filters);
}

export function getDefinitions(filters?: Parameters<typeof workflowService.getDefinitions>[0]) {
  return workflowService.getDefinitions(filters);
}

export function getDefinition(id: number): Promise<WorkflowDefinitionDto> {
  return workflowService.getDefinition(id);
}

export function createDraft(dto: CreateWorkflowDefinitionDto) {
  return workflowService.createDraft(dto);
}

export function updateDefinition(id: number, dto: UpdateWorkflowDefinitionDto) {
  return workflowService.updateDefinition(id, dto);
}

export function publishDefinition(id: number, req?: PublishDefinitionRequestDto) {
  return workflowService.publishDefinition(id, req);
}

export function validateDefinitionJson(json: string): Promise<{ success: boolean; errors: string[]; warnings: string[] }> {
  return workflowService.validateDefinitionJson(json);
}

export function validateDefinitionById(id: number) {
  return workflowService.validateDefinitionById(id);
}

export function createNewVersion(id: number, req: CreateNewVersionRequestDto) {
  return workflowService.createNewVersion(id, req);
}

export function revalidateDefinition(id: number): Promise<ValidationResultDto> {
  return workflowService.revalidateDefinition(id);
}

export function archiveDefinition(id: number) {
  return workflowService.archiveDefinition(id);
}

export function unpublishDefinition(id: number) {
  return workflowService.unpublishDefinition(id);
}

export function terminateDefinitionInstances(id: number) {
  return workflowService.terminateDefinitionInstances(id);
}

export function deleteDefinition(id: number) {
  return workflowService.deleteDefinition(id);
}
