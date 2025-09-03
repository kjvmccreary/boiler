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

/* =========================================================
   TENANT-PARTITIONED STATE
   ========================================================= */
const workflowState: Record<string, {
  definitions: MockDefinition[];
  instances: MockInstance[];
  tasks: MockTask[];
}> = {};

function getTenantIdFromRequest(request: Request) {
  const hdr = request.headers.get('X-Tenant-Id');
  return hdr && hdr.trim() ? hdr.trim() : 'default';
}

function bucket(tenantId: string) {
  if (!workflowState[tenantId]) {
    workflowState[tenantId] = { definitions: [], instances: [], tasks: [] };
  }
  return workflowState[tenantId];
}

/* =========================================================
   HELPERS
   ========================================================= */
function genId(prefix: string) {
  return `${prefix}-${Math.random().toString(36).slice(2, 10)}`;
}
function nowIso() { return new Date().toISOString(); }
function createApiResponse<T>(data: T, success = true, message?: string) {
  return { success, data, message, errors: success ? [] : ['Operation failed'] };
}
function nextVersionForKey(store: ReturnType<typeof bucket>, key: string) {
  const existing = store.definitions.filter(d => d.key === key);
  if (existing.length === 0) return 1;
  return Math.max(...existing.map(d => d.version)) + 1;
}
function scenario(request: Request) {
  const url = new URL(request.url);
  return url.searchParams.get('wfScenario');
}

/* =========================================================
   HANDLERS
   ========================================================= */

// Create draft
const createDraftHandler = http.post('/api/workflow/definitions/draft', async ({ request }) => {
  const tenantId = getTenantIdFromRequest(request);
  const store = bucket(tenantId);

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

  const jsonObj = typeof jsonRaw === 'string'
    ? (() => { try { return JSON.parse(jsonRaw); } catch { return null; } })()
    : jsonRaw;

  if (!jsonObj || !jsonObj.nodes || !jsonObj.edges) {
    return HttpResponse.json(createApiResponse(null, false, 'Invalid payload'), { status: 400 });
  }

  const version = nextVersionForKey(store, key);
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
  store.definitions.push(def);
  return HttpResponse.json(createApiResponse(def), { status: 201 });
});

// Validate by ID
const validateByIdHandler = http.get('/api/workflow/definitions/:id/validate', ({ params, request }) => {
  const store = bucket(getTenantIdFromRequest(request));
  const def = store.definitions.find(d => d.id === params.id);
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

// Generic validate
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

// Publish
const publishHandler = http.post('/api/workflow/definitions/:id/publish', ({ params, request }) => {
  const store = bucket(getTenantIdFromRequest(request));
  const def = store.definitions.find(d => d.id === params.id);
  if (!def) {
    return HttpResponse.json(createApiResponse(null, false, 'Definition not found'), { status: 404 });
  }
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
  return HttpResponse.json(createApiResponse(def, true, 'Published'));
});

// Start instance
const startInstanceHandler = http.post('/api/workflow/instances', async ({ request }) => {
  const tenantId = getTenantIdFromRequest(request);
  const store = bucket(tenantId);

  const body = await request.json() as any;
  const key = body.definitionKey;

  const def = store.definitions
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
  store.instances.push(inst);

  if (human) {
    store.tasks.push({
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
const listTasksHandler = http.get('/api/workflow/tasks', ({ request }) => {
  const store = bucket(getTenantIdFromRequest(request));
  return HttpResponse.json(createApiResponse(store.tasks));
});

// Task summary
const taskSummaryHandler = http.get('/api/workflow/tasks/mine/summary', ({ request }) => {
  const store = bucket(getTenantIdFromRequest(request));
  const tasks = store.tasks;
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

// Task find helper (tenant-aware)
function findTask(store: ReturnType<typeof bucket>, id: string) {
  return store.tasks.find(t => t.id === id);
}

// Get instance
const getInstanceHandler = http.get('/api/workflow/instances/:id', ({ params, request }) => {
  const store = bucket(getTenantIdFromRequest(request));
  const inst = store.instances.find(i => i.id === params.id);
  if (!inst) {
    return HttpResponse.json(createApiResponse(null, false, 'Instance not found'), { status: 404 });
  }
  return HttpResponse.json(createApiResponse(inst));
});

// Get task
const getTaskHandler = http.get('/api/workflow/tasks/:id', ({ params, request }) => {
  const store = bucket(getTenantIdFromRequest(request));
  const task = findTask(store, params.id as string);
  if (!task) return HttpResponse.json(createApiResponse(null, false, 'Task not found'), { status: 404 });
  return HttpResponse.json(createApiResponse(task));
});

// Claim task
const claimTaskHandler = http.post('/api/workflow/tasks/:id/claim', ({ params, request }) => {
  const store = bucket(getTenantIdFromRequest(request));
  const task = findTask(store, params.id as string);
  if (!task) {
    return HttpResponse.json(createApiResponse(null, false, 'Task not found'), { status: 404 });
  }
  if (task.status !== 'Open') {
    return HttpResponse.json(createApiResponse(null, false, 'Task cannot be claimed'), { status: 409 });
  }
  task.status = 'Claimed';
  return HttpResponse.json(createApiResponse(task));
});

// Complete task
const completeTaskHandler = http.post('/api/workflow/tasks/:id/complete', ({ params, request }) => {
  const store = bucket(getTenantIdFromRequest(request));
  const task = findTask(store, params.id as string);
  if (!task) {
    return HttpResponse.json(createApiResponse(null, false, 'Task not found'), { status: 404 });
  }
  if (task.status !== 'Claimed' && task.status !== 'Open') {
    return HttpResponse.json(createApiResponse(null, false, 'Task cannot be completed'), { status: 409 });
  }
  task.status = 'Completed';

  const inst = store.instances.find(i => i.id === task.instanceId);
  if (inst) {
    const remaining = store.tasks.filter(t => t.instanceId === inst.id && t.status !== 'Completed');
    if (remaining.length === 0) {
      inst.status = 'Completed';
      inst.currentNodeIds = [];
    }
  }
  return HttpResponse.json(createApiResponse(task));
});

// Tenants list (mock)
const tenantsListHandler = http.get('/api/tenants', () =>
  HttpResponse.json({
    success: true,
    data: [
      { id: '1', name: 'Test Tenant 1', domain: 'tenant1.test', isActive: true, createdAt: nowIso(), updatedAt: nowIso() }
    ]
  })
);

// Current tenant (mock)
const currentTenantHandler = http.get('/api/tenants/current', () =>
  HttpResponse.json({
    success: true,
    data: { id: '1', name: 'Test Tenant 1', domain: 'tenant1.test', isActive: true, createdAt: nowIso(), updatedAt: nowIso() }
  })
);

// List definitions (tenant-scoped)
const listDefinitionsHandler = http.get('/api/workflow/definitions', ({ request }) => {
  const store = bucket(getTenantIdFromRequest(request));
  return HttpResponse.json(createApiResponse(
    store.definitions.map(d => ({
      id: d.id,
      key: d.key,
      version: d.version,
      status: d.status,
      name: d.name ?? d.key,
      json: d.json,
      createdAt: d.createdAt,
      publishedAt: d.publishedAt
    }))
  ));
});

// Update draft (immutability guard)
const updateDefinitionHandler = http.put('/api/workflow/definitions/:id', async ({ params, request }) => {
  const store = bucket(getTenantIdFromRequest(request));
  const def = store.definitions.find(d => d.id === params.id);
  if (!def) {
    return HttpResponse.json(createApiResponse(null, false, 'Definition not found'), { status: 404 });
  }
  if (def.status === 'Published') {
    return HttpResponse.json(
      { success: false, data: null, message: 'Published definitions cannot be modified', errors: ['Immutable'] },
      { status: 409 }
    );
  }
  const body = await request.json() as any;
  if (body?.jsonDefinition) {
    def.json = body.jsonDefinition;
  }
  def.name = body?.name ?? def.name;
  def.description = body?.description ?? def.description;
  return HttpResponse.json(createApiResponse(def, true, 'Updated'));
});

/* =========================================================
   EXPORT
   ========================================================= */
export const workflowHandlers = [
  createDraftHandler,
  validateByIdHandler,
  genericValidateHandler,
  publishHandler,
  startInstanceHandler,
  listTasksHandler,
  taskSummaryHandler,
  getInstanceHandler,
  getTaskHandler,
  claimTaskHandler,
  completeTaskHandler,
  tenantsListHandler,
  currentTenantHandler,
  listDefinitionsHandler,
  updateDefinitionHandler
];

export const handlers = [...workflowHandlers];
