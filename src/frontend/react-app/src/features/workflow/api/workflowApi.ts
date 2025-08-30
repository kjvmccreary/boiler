import { EditorWorkflowDefinition } from '../../../types/workflow';
import { enrichDefinition } from '../utils/enrichEdges';

function ensureEnrichedJson(def: EditorWorkflowDefinition): string {
  const enriched = enrichDefinition(def);
  return JSON.stringify(enriched);
}

interface CreateDefinitionPayload {
  name: string;
  description?: string;
  definition: EditorWorkflowDefinition;
}

export async function createWorkflowDefinitionDraft(p: CreateDefinitionPayload) {
  const jsonDefinition = ensureEnrichedJson(p.definition);
  const body = {
    name: p.name,
    description: p.description ?? '',
    jsonDefinition
  };
  const res = await fetch('/api/workflow/definitions/draft', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });
  if (!res.ok) throw new Error(`Create draft failed: ${res.status}`);
  return res.json();
}

interface UpdateDefinitionPayload {
  id: number;
  name?: string;
  description?: string;
  definition?: EditorWorkflowDefinition;
}

export async function updateWorkflowDefinitionDraft(p: UpdateDefinitionPayload) {
  const body: any = {};
  if (p.name !== undefined) body.name = p.name;
  if (p.description !== undefined) body.description = p.description;
  if (p.definition) body.jsonDefinition = ensureEnrichedJson(p.definition);

  const res = await fetch(`/api/workflow/definitions/${p.id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });
  if (!res.ok) throw new Error(`Update draft failed: ${res.status}`);
  return res.json();
}
