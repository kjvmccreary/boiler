import { http, HttpResponse } from 'msw';
import approvalBasic from '@/test/fixtures/workflow/approval.basic.json';
import { validateDefinition } from '@/features/workflow/dsl/dsl.validate';

interface MockDefinition {
  id: string;
  key: string;
  version: number;
  status: 'Draft' | 'Published';
  json: any;
  createdAt: string;
  publishedAt?: string;
  name?: string;
  description?: string;
}
interface MockInstance {
  id: string;
  definitionKey: string;
  definitionVersion: number;
  status: 'Running' | 'Completed';
  currentNodeIds: string[];
  startedAt: string;
  context: Record<string, any>;
}
interface MockTask {
  id: string;
  instanceId: string;
  nodeId: string;
  label: string;
  status: 'Open' | 'Claimed' | 'Completed';
  createdAt: string;
}

const workflowState = {
  definitions: [] as MockDefinition[],
  instances: [] as MockInstance[],
  tasks: [] as MockTask[]
};

function genId(prefix: string) {
  return `${prefix}-${Math.random().toString(36).slice(2,10)}`;
}
function nowIso() { return new Date().toISOString(); }
function createApiResponse<T>(data: T, success = true, message?: string) {
  return { success, data, message, errors: success ? [] : ['Operation failed'] };
}
function nextVersionForKey(key: string) {
  const existing = workflowState.definitions.filter(d => d.key === key);
  if (existing.length === 0) return 1;
  return Math.max(...existing.map(d => d.version)) + 1;
}

// Scenario helper (optional query flag ?wfScenario=invalid|error)
function scenario(request: Request) {
  const url = new URL(request.url);
  return url.searchParams.get('wfScenario');
}

// Create draft (tolerant to multiple body shapes)
const createDraftHandler = http.post('/api/workflow/definitions/draft', async ({ request }) => {
  const scen = scenario(request);
  if (scen === 'error') {
    return HttpResponse.json(createApiResponse(null, false, 'Draft creation failed'), { status: 500 });
  }
  const body = await request.json() as any;
  const key = body.key;
  const jsonRaw = body.json ?? body.jsonDefinition ?? body.JSONDefinition ?? body.JSONdefinition;
  const name = body.name;
  const description = body.description;

  if (!key || !jsonRaw) {
    return HttpResponse.json(createApiResponse(null, false, 'Invalid payload'), { status: 400 });
  }

  // Accept either already-parsed object OR raw DSL object
  const jsonObj = typeof jsonRaw === 'string'
    ? (() => { try { return JSON.parse(jsonRaw); } catch { return null; } })()
    : jsonRaw;

  if (!jsonObj || !jsonObj.nodes || !jsonObj.edges) {
    return HttpResponse.json(createApiResponse(null, false, 'Invalid payload'), { status: 400 });
  }

  const version = nextVersionForKey(key);
  const def: MockDefinition = {
    id: genId('def'),
    key,
    version,
    status: 'Draft',
    json: { ...jsonObj, key, version },
    createdAt: nowIso(),
    name,
    description
  };
  workflowState.definitions.push(def);
  return HttpResponse.json(createApiResponse(def), { status: 201 });
});

// Validate by ID
const validateByIdHandler = http.get('/api/workflow/definitions/:id/validate', ({ params }) => {
  const id = params.id as string;
  const def = workflowState.definitions.find(d => d.id === id);
  if (!def) {
    return HttpResponse.json({ success: false, errors: ['Definition not found'], warnings: [] }, { status: 404 });
  }
  const result = validateDefinition(def.json);
  return HttpResponse.json({
    success: result.isValid,
    errors: result.errors,
    warnings: result.warnings
  }, { status: result.isValid ? 200 : 422 });
});

// Generic validate (ad-hoc JSON) endpoint
const genericValidateHandler = http.post('/api/workflow/definitions/validate', async ({ request }) => {
  const body = await request.json() as any;
  let raw = body.json ?? body.jsonDefinition ?? body.JSONDefinition ?? body.JSONdefinition;
  if (!raw) {
    return HttpResponse.json({ success: false, errors: ['Missing JSON definition payload'], warnings: [] }, { status: 400 });
  }
  if (typeof raw === 'string') {
    try { raw = JSON.parse(raw); } catch {
      return HttpResponse.json({ success: false, errors: ['JSONDefinition string is not valid JSON'], warnings: [] }, { status: 400 });
    }
  }
  if (!raw.nodes || !raw.edges) {
    return HttpResponse.json({ success: false, errors: ['Definition must include nodes and edges arrays'], warnings: [] }, { status: 422 });
  }
  const result = validateDefinition(raw);
  return HttpResponse.json({
    success: result.isValid,
    errors: result.errors,
    warnings: result.warnings
  }, { status: result.isValid ? 200 : 422 });
});

// Publish definition (simple promote)
const publishHandler = http.post('/api/workflow/definitions/:id/publish', async ({ params }) => {
  const id = params.id as string;
  const def = workflowState.definitions.find(d => d.id === id);
  if (!def) {
    return HttpResponse.json(createApiResponse(null, false, 'Definition not found'), { status: 404 });
  }
  // Validate before publish (simulate real behavior)
  const result = validateDefinition(def.json);
  if (!result.isValid) {
    return HttpResponse.json({
      success: false,
      errors: result.errors,
      message: 'Graph invalid'
    }, { status: 422 });
  }
  def.status = 'Published';
  def.publishedAt = nowIso();
  return HttpResponse.json(createApiResponse(def, true, 'Published'), { status: 200 });
});

// Start instance (auto-creates a single human task if a humanTask node exists)
const startInstanceHandler = http.post('/api/workflow/instances', async ({ request }) => {
  const body = await request.json() as any;
  const key = body.definitionKey;
  const def = workflowState.definitions
    .filter(d => d.key === key)
    .sort((a, b) => b.version - a.version)[0];
  if (!def || def.status !== 'Published') {
    return HttpResponse.json(createApiResponse(null, false, 'Definition not published'), { status: 400 });
  }

  const human = (def.json.nodes || []).find((n: any) => n.type === 'humanTask');
  const inst: MockInstance = {
    id: genId('inst'),
    definitionKey: def.key,
    definitionVersion: def.version,
    status: 'Running',
    currentNodeIds: human ? [human.id] : [],
    startedAt: nowIso(),
    context: {}
  };
  workflowState.instances.push(inst);

  if (human) {
    workflowState.tasks.push({
      id: genId('task'),
      instanceId: inst.id,
      nodeId: human.id,
      label: human.label || 'Task',
      status: 'Open',
      createdAt: nowIso()
    });
  }

  return HttpResponse.json(createApiResponse(inst), { status: 201 });
});

// List tasks
const listTasksHandler = http.get('/api/workflow/tasks', () => {
  // Return raw tasks
  return HttpResponse.json(createApiResponse(workflowState.tasks));
});

// Task summary (basic counts)
const taskSummaryHandler = http.get('/api/workflow/tasks/mine/summary', () => {
  const tasks = workflowState.tasks;
  const counts = {
    available: tasks.filter(t => t.status === 'Open').length,
    assignedToMe: 0,
    assignedToMyRoles: 0,
    claimed: tasks.filter(t => t.status === 'Claimed').length,
    inProgress: 0,
    completedToday: 0,
    overdue: 0,
    failed: 0,
    totalActionable: tasks.filter(t => t.status === 'Open' || t.status === 'Claimed').length
  };
  return HttpResponse.json(createApiResponse(counts));
});

export const workflowHandlers = [
  createDraftHandler,
  validateByIdHandler,
  genericValidateHandler,
  publishHandler,
  startInstanceHandler,
  listTasksHandler,
  taskSummaryHandler
];

// Backwards compatibility: expected by browser.ts & setup.ts
export const handlers = [...workflowHandlers];
