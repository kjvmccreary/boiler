import { EditorWorkflowDefinition, EditorWorkflowEdge } from '../../../types/workflow';

function classify(edge: EditorWorkflowEdge): string | undefined {
  if (edge.fromHandle) return edge.fromHandle;
  if (edge.label) return edge.label;
  const id = edge.id.toLowerCase();
  if (id.includes('true')) return 'true';
  if (id.includes('false')) return 'false';
  if (id.includes('else')) return 'else';
  return undefined;
}

export function enrichDefinition(def: EditorWorkflowDefinition): EditorWorkflowDefinition {
  const copy: EditorWorkflowDefinition = {
    ...def,
    edges: def.edges.map(e => ({ ...e }))
  };

  for (const e of copy.edges) {
    const cls = classify(e);
    if (cls) {
      e.fromHandle = cls;
      e.label = cls;
    }
  }

  const groups = copy.edges.reduce<Record<string, EditorWorkflowEdge[]>>((acc, e) => {
    (acc[e.from] ||= []).push(e);
    return acc;
  }, {});

  for (const list of Object.values(groups)) {
    if (list.length < 2) continue;
    const hasTrue = list.some(e => e.fromHandle === 'true');
    const hasFalse = list.some(e => e.fromHandle === 'false');
    const hasElse = list.some(e => e.fromHandle === 'else');
    const unlabeled = list.filter(e => !e.fromHandle);
    if ((hasTrue || hasFalse) && !hasElse && unlabeled.length === 1) {
      unlabeled[0].fromHandle = 'else';
      unlabeled[0].label = 'else';
    }
  }
  return copy;
}
