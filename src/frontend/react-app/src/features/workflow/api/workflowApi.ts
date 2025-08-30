import { EditorWorkflowDefinition, EditorWorkflowEdge } from '../../../types/workflow';
import { enrichDefinition } from '../utils/enrichEdges';

function normalizeGatewayEdges(def: EditorWorkflowDefinition): EditorWorkflowDefinition {
  const clone: EditorWorkflowDefinition = {
    ...def,
    edges: def.edges.map(e => ({ ...e }))
  };

  const groups: Record<string, EditorWorkflowEdge[]> = {};
  for (const e of clone.edges) {
    (groups[e.from] ||= []).push(e);
  }

  for (const list of Object.values(groups)) {
    // Detect potential gateway branching by presence of any labeled edges or >1 outgoing
    if (list.length < 2 &&
        !list.some(e => /^(true|false|else)$/i.test(e.fromHandle || e.label || ''))) {
      continue;
    }

    // Harvest existing branch markers
    for (const e of list) {
      const branch = (e.fromHandle || e.label || '').toLowerCase();
      if (!branch && (e as any).sourceHandle && ['true','false','else'].includes((e as any).sourceHandle)) {
        const h = (e as any).sourceHandle;
        e.fromHandle = h;
        e.label = h;
      }
    }

    const trues  = list.filter(e => (e.fromHandle || e.label)?.toLowerCase() === 'true');
    const falses = list.filter(e => (e.fromHandle || e.label)?.toLowerCase() === 'false');
    const elses  = list.filter(e => (e.fromHandle || e.label)?.toLowerCase() === 'else');

    // Collapse else to binary
    if (elses.length) {
      if (!trues.length && !falses.length) {
        // single else -> treat as false
        for (const e of elses) {
          e.fromHandle = 'false';
          e.label = 'false';
        }
      } else {
        // remove else edges
        for (const e of elses) {
          const idx = clone.edges.findIndex(x => x.id === e.id);
          if (idx >= 0) clone.edges.splice(idx, 1);
        }
      }
    }

    // Deduplicate
    const keepTrue = trues[0];
    for (const extra of trues.slice(1)) {
      const idx = clone.edges.findIndex(x => x.id === extra.id);
      if (idx >= 0) clone.edges.splice(idx, 1);
    }
    const keepFalse = falses[0];
    for (const extra of falses.slice(1)) {
      const idx = clone.edges.findIndex(x => x.id === extra.id);
      if (idx >= 0) clone.edges.splice(idx, 1);
    }

    // Final pass: ensure parity
    for (const e of list) {
      const b = (e.fromHandle || e.label || '').toLowerCase();
      if (b === 'true' || b === 'false') {
        e.fromHandle = b;
        e.label = b;
      }
    }
  }

  return clone;
}

function buildOutgoingJson(def: EditorWorkflowDefinition): string {
  // 1. Run first pass normalization (capture sourceHandle-based branches)
  const normalized = normalizeGatewayEdges(def);
  // 2. Existing enrichment (harmless for already labeled edges)
  const enriched = enrichDefinition(normalized);
  // 3. Second pass to guard against enrichment dropping anything (defensive)
  const finalPass = normalizeGatewayEdges(enriched);
  return JSON.stringify(finalPass);
}

interface CreateDefinitionPayload {
  name: string;
  description?: string;
  definition: EditorWorkflowDefinition;
}

export async function createWorkflowDefinitionDraft(p: CreateDefinitionPayload) {
  const jsonDefinition = buildOutgoingJson(p.definition);
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
  if (p.definition) body.jsonDefinition = buildOutgoingJson(p.definition);

  const res = await fetch(`/api/workflow/definitions/${p.id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });
  if (!res.ok) throw new Error(`Update draft failed: ${res.status}`);
  return res.json();
}
