import { http, HttpResponse } from 'msw'
import type { User, Role } from '@/types/index.ts'
import { mockUsers, mockRoles, mockPermissions } from '../utils/test-utils.tsx'
import { validateDefinition } from '@/features/workflow/dsl/dsl.validate'
import type { DslDefinition, DslNode, DslEdge } from '@/features/workflow/dsl/dsl.types'

// ---------- EXISTING HELPERS (unchanged) ----------
const createApiResponse = <T>(data: T, success = true, message?: string) => ({
  success,
  data,
  message,
  errors: success ? [] : [message || 'Operation failed']
})

const createPagedResponse = <T>(items: T[], page: number, pageSize: number, totalCount?: number) => {
  const total = totalCount ?? items.length
  const startIndex = (page - 1) * pageSize
  const endIndex = startIndex + pageSize
  const pagedItems = items.slice(startIndex, endIndex)

  return createApiResponse({
    items: pagedItems,
    totalCount: total,
    pageNumber: page,
    pageSize: pageSize,
    totalPages: Math.ceil(total / pageSize)
  })
}

const enhancedMockUsers: User[] = Object.values(mockUsers)
const enhancedMockRoles: Role[] = Object.values(mockRoles)

// ---------- EXISTING TENANT / AUTH MOCK DATA ----------
const mockTenants = [
  {
    id: 1,
    name: 'Test Tenant',
    domain: 'test.local',
    subscriptionPlan: 'Development',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  {
    id: 2,
    name: 'Tenant One',
    domain: 'tenant1.test',
    subscriptionPlan: 'Basic',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  },
  {
    id: 3,
    name: 'Tenant Two',
    domain: 'tenant2.test',
    subscriptionPlan: 'Pro',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  }
]

const mockSingleTenant = [
  {
    id: 1,
    name: 'Single Tenant',
    domain: 'single.local',
    subscriptionPlan: 'Premium',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  }
]

const mockTenantSettings = {
  theme: {
    primaryColor: '#1976d2',
    companyName: 'Default Tenant'
  },
  features: {
    multiUser: true,
    reports: true,
    analytics: false
  },
  subscriptionPlan: 'Development'
}

const createCompleteUser = (id: string, email: string, firstName: string, lastName: string): User => ({
  id,
  email,
  firstName,
  lastName,
  fullName: `${firstName} ${lastName}`,
  phoneNumber: undefined,
  timeZone: 'UTC',
  language: 'en',
  lastLoginAt: undefined,
  emailConfirmed: true,
  isActive: true,
  roles: ['User'],
  tenantId: '1',
  createdAt: new Date().toISOString(),
  updatedAt: new Date().toISOString(),
  preferences: {
    theme: 'light',
    language: 'en',
    timeZone: 'UTC',
    notifications: {
      email: true,
      push: false,
      sms: false
    }
  }
})

// ---------- NEW: WORKFLOW DOMAIN IN-MEMORY STATE ----------
interface MockDefinition {
  id: string
  key: string
  version: number
  status: 'Draft' | 'Published'
  json: DslDefinition
  createdAt: string
  publishedAt?: string
}

interface MockInstance {
  id: string
  definitionKey: string
  definitionVersion: number
  status: 'Running' | 'Completed' | 'Terminated'
  currentNodeIds: string[]
  startedAt: string
  completedAt?: string
  context: Record<string, unknown>
}

interface MockTask {
  id: string
  instanceId: string
  nodeId: string
  label?: string
  assignedToUser?: string
  assignedRole?: string
  status: 'Open' | 'Claimed' | 'Completed'
  dueDate?: string
  createdAt: string
  completedAt?: string
}

interface MockEvent {
  id: string
  instanceId: string
  type: string
  name: string
  data: Record<string, unknown>
  occurredAt: string
}

const workflowState = {
  definitions: [] as MockDefinition[],
  instances: [] as MockInstance[],
  tasks: [] as MockTask[],
  events: [] as MockEvent[]
}

const genId = (p: string) => `${p}-${Math.random().toString(36).slice(2, 10)}`
const nowIso = () => new Date().toISOString()

function nextVersionForKey(key: string) {
  const versions = workflowState.definitions
    .filter(d => d.key === key)
    .map(d => d.version)
  return versions.length === 0 ? 1 : Math.max(...versions) + 1
}

function latestPublishedDefinition(key: string, version?: number) {
  const defs = workflowState.definitions
    .filter(d => d.key === key && d.status === 'Published')
  if (version != null) return defs.find(d => d.version === version)
  return defs.sort((a, b) => b.version - a.version)[0]
}

function buildInitialTasks(instance: MockInstance, def: MockDefinition) {
  const startNode = def.json.nodes.find(n => n.type === 'start')
  if (!startNode) return
  // naive progression: find edges from start, create tasks for humanTask nodes directly reachable, set current node
  const outgoing = def.json.edges.filter(e => e.from === startNode.id)
  const humanTargets = outgoing
    .map(e => def.json.nodes.find(n => n.id === e.to))
    .filter((n): n is DslNode => !!n && n.type === 'humanTask')
  instance.currentNodeIds = humanTargets.length > 0 ? humanTargets.map(h => h.id) : [startNode.id]
  humanTargets.forEach(h => {
    workflowState.tasks.push({
      id: genId('task'),
      instanceId: instance.id,
      nodeId: h.id,
      label: h.label,
      status: 'Open',
      createdAt: nowIso()
    })
  })
  workflowState.events.push({
    id: genId('evt'),
    instanceId: instance.id,
    type: 'Instance',
    name: 'InstanceStarted',
    data: { current: instance.currentNodeIds },
    occurredAt: nowIso()
  })
}

function completeTaskAdvance(task: MockTask) {
  task.status = 'Completed'
  task.completedAt = nowIso()
  const instance = workflowState.instances.find(i => i.id === task.instanceId)
  if (!instance) return
  // remove node from currentNodeIds
  instance.currentNodeIds = instance.currentNodeIds.filter(id => id !== task.nodeId)
  // If no remaining open tasks for instance, mark completed
  const openForInstance = workflowState.tasks.some(
    t => t.instanceId === instance.id && t.status !== 'Completed'
  )
  if (!openForInstance && instance.currentNodeIds.length === 0) {
    instance.status = 'Completed'
    instance.completedAt = nowIso()
    workflowState.events.push({
      id: genId('evt'),
      instanceId: instance.id,
      type: 'Instance',
      name: 'InstanceCompleted',
      data: {},
      occurredAt: nowIso()
    })
  }
}

// Scenario helper (query or header)
function workflowScenario(req: Request) {
  const url = new URL(req.url)
  const qp = url.searchParams.get('scenario')
  const header = req.headers.get('X-Workflow-Scenario')
  return header || qp || ''
}

// ---------- NEW WORKFLOW HANDLERS (inserted before existing handlers array export end) ----------
const workflowHandlers = [

  // Create draft definition
  http.post('/api/workflow/definitions/draft', async ({ request }) => {
    const scen = workflowScenario(request)
    if (scen === 'error') {
      return HttpResponse.json(createApiResponse(null, false, 'Draft creation failed'), { status: 500 })
    }
    const body = await request.json() as { key: string; json: DslDefinition }
    if (!body?.key || !body?.json) {
      return HttpResponse.json(createApiResponse(null, false, 'Invalid payload'), { status: 400 })
    }
    const version = nextVersionForKey(body.key)
    const def: MockDefinition = {
      id: genId('def'),
      key: body.key,
      version,
      status: 'Draft',
      json: { ...body.json, key: body.key, version },
      createdAt: nowIso()
    }
    workflowState.definitions.push(def)
    return HttpResponse.json(createApiResponse(def), { status: 201 })
  }),

  // List definitions
  http.get('/api/workflow/definitions', ({ request }) => {
    const scen = workflowScenario(request)
    if (scen === 'error') {
        return HttpResponse.json(createApiResponse(null, false, 'Failed to list definitions'), { status: 500 })
    }
    if (scen === 'empty') {
      return HttpResponse.json(createApiResponse([]), { status: 200 })
    }
    return HttpResponse.json(createApiResponse(workflowState.definitions), { status: 200 })
  }),

  // Get definition by id
  http.get('/api/workflow/definitions/:id', ({ params, request }) => {
    const scen = workflowScenario(request)
    const { id } = params
    if (scen === 'notfound') {
      return HttpResponse.json(createApiResponse(null, false, 'Definition not found'), { status: 404 })
    }
    const def = workflowState.definitions.find(d => d.id === id)
    if (!def) {
      return HttpResponse.json(createApiResponse(null, false, 'Definition not found'), { status: 404 })
    }
    return HttpResponse.json(createApiResponse(def), { status: 200 })
  }),

  // Validate definition
  http.post('/api/workflow/definitions/:id/validate', ({ params, request }) => {
    const { id } = params
    const scen = workflowScenario(request)
    const def = workflowState.definitions.find(d => d.id === id)
    if (!def) {
      return HttpResponse.json(createApiResponse(null, false, 'Definition not found'), { status: 404 })
    }
    if (scen === 'invalid' || scen === 'unreachable') {
      // force inject invalid condition
      def.json.nodes = def.json.nodes.filter(n => n.type !== 'end')
    }
    const result = validateDefinition(def.json)
    return HttpResponse.json(createApiResponse({
      isValid: result.isValid,
      errors: result.errors,
      warnings: result.warnings
    }, result.isValid, result.isValid ? 'Valid' : 'Invalid'), { status: result.isValid ? 200 : 422 })
  }),

  // Publish definition
  http.post('/api/workflow/definitions/:id/publish', ({ params, request }) => {
    const { id } = params
    const scen = workflowScenario(request)
    const def = workflowState.definitions.find(d => d.id === id)
    if (!def) {
      return HttpResponse.json(createApiResponse(null, false, 'Definition not found'), { status: 404 })
    }
    if (scen === 'error') {
      return HttpResponse.json(createApiResponse(null, false, 'Publish failed'), { status: 500 })
    }
    if (def.status === 'Published') {
      return HttpResponse.json(createApiResponse(def, true, 'Already published'), { status: 200 })
    }
    const validation = validateDefinition(def.json)
    if (!validation.isValid) {
      return HttpResponse.json(createApiResponse({
        errors: validation.errors,
        warnings: validation.warnings
      }, false, 'Validation failed'), { status: 422 })
    }
    def.status = 'Published'
    def.publishedAt = nowIso()
    return HttpResponse.json(createApiResponse(def, true, 'Published'), { status: 200 })
  }),

  // Start instance
  http.post('/api/workflow/instances', async ({ request }) => {
    const scen = workflowScenario(request)
    if (scen === 'error') {
      return HttpResponse.json(createApiResponse(null, false, 'Start failed'), { status: 500 })
    }
    const body = await request.json() as { definitionKey: string; definitionVersion?: number; input?: Record<string, unknown> }
    const def = latestPublishedDefinition(body.definitionKey, body.definitionVersion)
    if (!def) {
      return HttpResponse.json(createApiResponse(null, false, 'Published definition not found'), { status: 404 })
    }
    const instance: MockInstance = {
      id: genId('inst'),
      definitionKey: def.key,
      definitionVersion: def.version,
      status: 'Running',
      currentNodeIds: [],
      startedAt: nowIso(),
      context: body.input || {}
    }
    workflowState.instances.push(instance)
    workflowState.events.push({
      id: genId('evt'),
      instanceId: instance.id,
      type: 'Definition',
      name: 'InstanceCreated',
      data: { defId: def.id, version: def.version },
      occurredAt: nowIso()
    })
    buildInitialTasks(instance, def)
    return HttpResponse.json(createApiResponse(instance), { status: 201 })
  }),

  // Get instance
  http.get('/api/workflow/instances/:id', ({ params, request }) => {
    const { id } = params
    const scen = workflowScenario(request)
    if (scen === 'notfound') {
      return HttpResponse.json(createApiResponse(null, false, 'Instance not found'), { status: 404 })
    }
    const inst = workflowState.instances.find(i => i.id === id)
    if (!inst) {
      return HttpResponse.json(createApiResponse(null, false, 'Instance not found'), { status: 404 })
    }
    return HttpResponse.json(createApiResponse(inst), { status: 200 })
  }),

  // Snapshot
  http.get('/api/workflow/instances/:id/snapshot', ({ params }) => {
    const { id } = params
    const inst = workflowState.instances.find(i => i.id === id)
    if (!inst) {
      return HttpResponse.json(createApiResponse(null, false, 'Instance not found'), { status: 404 })
    }
    const def = workflowState.definitions.find(d => d.key === inst.definitionKey && d.version === inst.definitionVersion)
    const events = workflowState.events.filter(e => e.instanceId === inst.id)
    return HttpResponse.json(createApiResponse({
      instance: inst,
      definition: def,
      events
    }), { status: 200 })
  }),

  // List tasks
  http.get('/api/workflow/tasks', ({ request }) => {
    const scen = workflowScenario(request)
    if (scen === 'error') {
      return HttpResponse.json(createApiResponse(null, false, 'Task query failed'), { status: 500 })
    }
    const url = new URL(request.url)
    const mine = url.searchParams.get('mine') === 'true'
    const userId = 'user-1'
    let list = workflowState.tasks
    if (mine) {
      list = list.filter(t => !t.assignedToUser || t.assignedToUser === userId)
    }
    return HttpResponse.json(createApiResponse(list), { status: 200 })
  }),

  // Task summary
  http.get('/api/workflow/tasks/summary', () => {
    const total = workflowState.tasks.length
    const open = workflowState.tasks.filter(t => t.status === 'Open').length
    const claimed = workflowState.tasks.filter(t => t.status === 'Claimed').length
    const completed = workflowState.tasks.filter(t => t.status === 'Completed').length
    return HttpResponse.json(createApiResponse({
      total, open, claimed, completed
    }), { status: 200 })
  }),

  // Claim task
  http.post('/api/workflow/tasks/:id:claim', ({ params, request }) => {
    const { id } = params
    const scen = workflowScenario(request)
    if (scen === 'error') {
      return HttpResponse.json(createApiResponse(null, false, 'Claim failed'), { status: 500 })
    }
    const task = workflowState.tasks.find(t => t.id === id)
    if (!task) {
      return HttpResponse.json(createApiResponse(null, false, 'Task not found'), { status: 404 })
    }
    if (task.status !== 'Open') {
      return HttpResponse.json(createApiResponse(null, false, 'Task not claimable'), { status: 409 })
    }
    task.status = 'Claimed'
    task.assignedToUser = 'user-1'
    return HttpResponse.json(createApiResponse(task, true, 'Claimed'), { status: 200 })
  }),

  // Complete task
  http.post('/api/workflow/tasks/:id:complete', async ({ params, request }) => {
    const { id } = params
    const scen = workflowScenario(request)
    if (scen === 'error') {
      return HttpResponse.json(createApiResponse(null, false, 'Completion failed'), { status: 500 })
    }
    const task = workflowState.tasks.find(t => t.id === id)
    if (!task) {
      return HttpResponse.json(createApiResponse(null, false, 'Task not found'), { status: 404 })
    }
    if (task.status === 'Completed') {
      return HttpResponse.json(createApiResponse(task, true, 'Already completed'), { status: 200 })
    }
    completeTaskAdvance(task)
    return HttpResponse.json(createApiResponse(task, true, 'Completed'), { status: 200 })
  })
]

// ---------- EXISTING NON-WORKFLOW HANDLERS (kept) ----------
export const handlers = [
  // Insert new workflow handlers first so they are matched before catch-alls
  ...workflowHandlers,

  // Tenants
  http.get('/api/users/:userId/tenants', ({ params, request }) => {
    const { userId } = params
    const url = new URL(request.url)
    const scenario = url.searchParams.get('scenario')

    console.log('🎯 MSW: Handling /api/users/:userId/tenants request', { userId, scenario })

    if (scenario === 'no-tenants' || userId === '7' || userId === 'no-tenants-user') {
      return HttpResponse.json(createApiResponse([]), { status: 200 })
    }
    if (scenario === 'single-tenant' || userId === '2' || userId === 'single-tenant-user') {
      return HttpResponse.json(createApiResponse(mockSingleTenant), { status: 200 })
    }
    if (scenario === 'error' || userId === '6' || userId === 'error-user') {
      return HttpResponse.json(
        createApiResponse(null, false, 'Failed to load tenants'),
        { status: 500 }
      )
    }
    return HttpResponse.json(createApiResponse(mockTenants), { status: 200 })
  }),

  // Auth / login
  http.post('/api/auth/login', async ({ request }) => {
    const body = await request.json() as any
    const { email, password } = body
    if (email === 'error@test.com' || password === 'wrong') {
      return HttpResponse.json(
        createApiResponse(null, false, 'Invalid credentials'),
        { status: 401 }
      )
    }
    const user = createCompleteUser('1', email, 'Admin', 'User')
    return HttpResponse.json(createApiResponse({
      accessToken: 'phase1-token-no-tenant',
      refreshToken: 'refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user,
      tenant: {
        id: '1',
        name: 'Default Tenant',
        domain: 'default.local',
        subscriptionPlan: 'Development',
        isActive: true,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      }
    }), { status: 200 })
  }),

  http.post('/api/auth/select-tenant', async ({ request }) => {
    const body = await request.json() as any
    const { tenantId } = body
    if (tenantId === 'error-tenant') {
      return HttpResponse.json(
        createApiResponse(null, false, 'Failed to select tenant'),
        { status: 500 }
      )
    }
    const selectedTenant = mockTenants.find(t => t.id.toString() === tenantId) || mockTenants[0]
    const user = createCompleteUser('1', 'admin@tenant1.com', 'Admin', 'User')
    return HttpResponse.json(createApiResponse({
      accessToken: `phase2-token-tenant-${tenantId}`,
      refreshToken: 'refresh-token-updated',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer',
      user,
      tenant: {
        id: selectedTenant.id.toString(),
        name: selectedTenant.name,
        domain: selectedTenant.domain,
        subscriptionPlan: selectedTenant.subscriptionPlan,
        isActive: selectedTenant.isActive,
        createdAt: selectedTenant.createdAt,
        updatedAt: selectedTenant.updatedAt,
      }
    }), { status: 200 })
  }),

  http.post('/api/auth/switch-tenant', async ({ request }) => {
    const body = await request.json() as any
    const { tenantId } = body
    if (tenantId === 'error-tenant') {
      return HttpResponse.json(
        createApiResponse(null, false, 'Failed to switch tenant'),
        { status: 500 }
      )
    }
    return HttpResponse.json(createApiResponse({
      accessToken: `switched-token-tenant-${tenantId}`,
      refreshToken: 'refresh-token-switched',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer'
    }), { status: 200 })
  }),

  http.get('/api/roles', async ({ request }) => {
    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')
    await new Promise(resolve => setTimeout(resolve, 100))
    const response = createPagedResponse(enhancedMockRoles, page, pageSize)
    return HttpResponse.json(response, { status: 200 })
  }),

  http.get('/api/tenants/:tenantId/settings', () => {
    return HttpResponse.json(createApiResponse(mockTenantSettings), { status: 200 })
  }),

  http.get('/api/users', ({ request }) => {
    const url = new URL(request.url)
    const page = parseInt(url.searchParams.get('page') || '1')
    const pageSize = parseInt(url.searchParams.get('pageSize') || '10')
    const authHeader = request.headers.get('Authorization')
    if (authHeader?.includes('viewer')) {
      return HttpResponse.json(createPagedResponse([], page, pageSize), { status: 200 })
    }
    return HttpResponse.json(createPagedResponse(enhancedMockUsers, page, pageSize), { status: 200 })
  }),

  http.post('/api/users', ({ request }) => {
    const authHeader = request.headers.get('Authorization')
    if (!authHeader) {
      return HttpResponse.json(createApiResponse(null, false, 'Unauthorized'), { status: 401 })
    }
    if (authHeader.includes('viewer')) {
      return HttpResponse.json(createApiResponse(null, false, 'Insufficient permissions'), { status: 403 })
    }
    return HttpResponse.json(createApiResponse({ id: '1', name: 'New User' }), { status: 201 })
  }),

  http.delete('/api/users/:userId', ({ request }) => {
    const authHeader = request.headers.get('Authorization')
    if (!authHeader) {
      return HttpResponse.json(createApiResponse(null, false, 'Unauthorized'), { status: 401 })
    }
    if (!authHeader.includes('admin')) {
      return HttpResponse.json(createApiResponse(null, false, 'Insufficient permissions'), { status: 403 })
    }
    return HttpResponse.json(createApiResponse({ message: 'User deleted successfully' }), { status: 200 })
  }),

  http.get('/api/users/profile', () => {
    const user = createCompleteUser('1', 'admin@tenant1.com', 'Admin', 'User')
    return HttpResponse.json(createApiResponse(user), { status: 200 })
  }),

  http.get('/api/permissions', () => {
    return HttpResponse.json(createApiResponse(mockPermissions), { status: 200 })
  }),

  http.post('/api/auth/logout', () => {
    return HttpResponse.json(createApiResponse({ message: 'Logged out successfully' }), { status: 200 })
  }),

  http.post('/api/auth/refresh', () => {
    return HttpResponse.json(createApiResponse({
      accessToken: 'new-access-token',
      refreshToken: 'new-refresh-token',
      expiresAt: new Date(Date.now() + 3600000).toISOString(),
      tokenType: 'Bearer'
    }), { status: 200 })
  }),

  // Catch-alls (keep last)
  http.get('/api/*', ({ request }) => {
    const url = new URL(request.url)
    console.log('🎯 MSW: Unhandled GET request caught by catch-all:', url.pathname)
    return HttpResponse.json(createApiResponse([]), { status: 200 })
  }),
  http.post('/api/*', ({ request }) => {
    const url = new URL(request.url)
    console.log('🎯 MSW: Unhandled POST request caught by catch-all:', url.pathname)
    return HttpResponse.json(createApiResponse({ message: 'Operation completed' }), { status: 200 })
  }),
  http.delete('/api/*', ({ request }) => {
    const url = new URL(request.url)
    console.log('🎯 MSW: Unhandled DELETE request caught by catch-all:', url.pathname)
    return HttpResponse.json(createApiResponse({ message: 'Resource deleted' }), { status: 200 })
  }),
  http.put('/api/*', ({ request }) => {
    const url = new URL(request.url)
    console.log('🎯 MSW: Unhandled PUT request caught by catch-all:', url.pathname)
    return HttpResponse.json(createApiResponse({ message: 'Resource updated' }), { status: 200 })
  }),
  http.patch('/api/*', ({ request }) => {
    const url = new URL(request.url)
    console.log('🎯 MSW: Unhandled PATCH request caught by catch-all:', url.pathname)
    return HttpResponse.json(createApiResponse({ message: 'Resource updated' }), { status: 200 })
  })
]
