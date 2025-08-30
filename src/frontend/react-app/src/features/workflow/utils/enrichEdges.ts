import { EditorWorkflowDefinition, EditorWorkflowEdge } from '../../../types/workflow';

function normalize(v: string): string {
  const l = v.trim().toLowerCase();
  if (l === 'yes') return 'true';
  if (l === 'no') return 'false';
  if (l === 'default') return 'else';
  return l;
}

function classify(e: EditorWorkflowEdge): string | undefined {
  if (e.fromHandle) return normalize(e.fromHandle);
  if (e.label) return normalize(e.label);
  const id = e.id.toLowerCase();
  if (id.includes('true')) return 'true';
  if (id.includes('false')) return 'false';
  if (id.includes('else')) return 'else';
  return undefined;
}

export function enrichDefinition(def: EditorWorkflowDefinition): EditorWorkflowDefinition {
  const copy: EditorWorkflowDefinition = { ...def, edges: def.edges.map(e => ({ ...e })) };

  // initial classification
  for (const e of copy.edges) {
    const c = classify(e);
    if (c) {
      e.fromHandle = c;
      e.label = c;
    }
  }

  // group per source
  const groups = copy.edges.reduce<Record<string, EditorWorkflowEdge[]>>((acc, e) => {
    (acc[e.from] ||= []).push(e);
    return acc;
  }, {});

  for (const list of Object.values(groups)) {
    const trues  = list.filter(e => e.fromHandle === 'true');
    const falses = list.filter(e => e.fromHandle === 'false');
    const elses  = list.filter(e => e.fromHandle === 'else');

    if (elses.length) {
      if (!trues.length && !falses.length) {
        // lone else becomes false
        elses.forEach(e => { e.fromHandle = 'false'; e.label = 'false'; });
      } else {
        // drop else (binary only)
        for (const e of elses) {
          const idx = copy.edges.findIndex(x => x.id === e.id);
            if (idx >= 0) copy.edges.splice(idx, 1);
        }
      }
    }
    // ensure only one true / one false
    trues.slice(1).forEach(extra => {
      const i = copy.edges.findIndex(x => x.id === extra.id);
      if (i >= 0) copy.edges.splice(i, 1);
    });
    falses.slice(1).forEach(extra => {
      const i = copy.edges.findIndex(x => x.id === extra.id);
      if (i >= 0) copy.edges.splice(i, 1);
    });
  }
  for (const e of copy.edges) {
    if (!e.fromHandle && (e as any).sourceHandle && ['true','false'].includes((e as any).sourceHandle)) {
      e.fromHandle = (e as any).sourceHandle;
      e.label = (e as any).sourceHandle;
    }
  }
  return copy;
}
