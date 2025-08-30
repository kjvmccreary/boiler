import { EditorWorkflowDefinition } from '../../../types/workflow';
import { enrichDefinition } from '../utils/enrichEdges';

interface CreateDefinitionPayload {
  name: string;
  description?: string;
  definition: EditorWorkflowDefinition;
}

export async function createWorkflowDefinitionDraft(p: CreateDefinitionPayload) {
  const enriched = enrichDefinition(p.definition);
  const body = {
    name: p.name,
    description: p.description ?? '',
    jsonDefinition: JSON.stringify(enriched)
  };

  console.debug('[WF][POST] draft edges',
    enriched.edges.map(e => ({ id: e.id, fromHandle: e.fromHandle, label: e.label }))
  );

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
  const enriched = p.definition ? enrichDefinition(p.definition) : undefined;
  const body: any = {};
  if (p.name !== undefined) body.name = p.name;
  if (p.description !== undefined) body.description = p.description;
  if (enriched) body.jsonDefinition = JSON.stringify(enriched);

  if (enriched) {
    console.debug('[WF][PUT] draft edges', p.id,
      enriched.edges.map(e => ({ id: e.id, fromHandle: e.fromHandle, label: e.label }))
    );
  }

  const res = await fetch(`/api/workflow/definitions/${p.id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });
  if (!res.ok) throw new Error(`Update draft failed: ${res.status}`);
  return res.json();
}
