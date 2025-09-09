// Enhanced operator metadata with richer markdown documentation & snippet friendliness
export interface JsonLogicOperatorMeta {
  label: string;
  insertText: string;
  detail: string;
  documentation: string; // markdown supported by Monaco
  sort?: number;
}

export const JSONLOGIC_OPERATORS: JsonLogicOperatorMeta[] = [
  {
    label: '==',
    insertText: '{"==": [$1, $2]}',
    detail: 'Equality',
    documentation: '**Equality**\n\nChecks if two values are strictly equal.\n```json\n{"==": [{"var":"status"}, "Open"]}\n```'
  },
  {
    label: '!=',
    insertText: '{"!=": [$1, $2]}',
    detail: 'Inequality',
    documentation: '**Inequality**\n\n```json\n{"!=": [{"var":"userId"}, 0]}\n```'
  },
  {
    label: '>',
    insertText: '{">": [$1, $2]}',
    detail: 'Greater than',
    documentation: '**Greater than**\n\n```json\n{">": [{"var":"amount"}, 100]}\n```'
  },
  {
    label: '>=',
    insertText: '{">=": [$1, $2]}',
    detail: 'Greater or equal',
    documentation: '**Greater or Equal**\n\n```json\n{">=": [{"var":"progress"}, 0.75]}\n```'
  },
  {
    label: '<',
    insertText: '{"<": [$1, $2]}',
    detail: 'Less than',
    documentation: '**Less than**\n\n```json\n{"<": [{"var":"elapsedMinutes"}, 30]}\n```'
  },
  {
    label: '<=',
    insertText: '{"<=": [$1, $2]}',
    detail: 'Less or equal',
    documentation: '**Less or Equal**\n\n```json\n{"<=": [{"var":"retries"}, 3]}\n```'
  },
  {
    label: 'and',
    insertText: '{"and": [$1, $2]}',
    detail: 'Logical AND',
    documentation: '**Logical AND**\nAll expressions must evaluate truthy.\n```json\n{"and":[{">":[{"var":"amount"},100]},{"==":[{"var":"currency"},"USD"]}]}\n```'
  },
  {
    label: 'or',
    insertText: '{"or": [$1, $2]}',
    detail: 'Logical OR',
    documentation: '**Logical OR**\nAt least one expression must be truthy.\n```json\n{"or":[{">":[{"var":"priority"},5]},{"==":[{"var":"expedite"},true]}]}\n```'
  },
  {
    label: '!',
    insertText: '{"!": $1}',
    detail: 'Logical NOT',
    documentation: '**Logical NOT**\nNegates a value.\n```json\n{"!": {"var":"isActive"}}\n```'
  },
  {
    label: '+',
    insertText: '{"+": [$1, $2]}',
    detail: 'Addition / concat',
    documentation: '**Addition / Concatenation**\n```json\n{"+": [1, 2]}\n```'
  },
  {
    label: '-',
    insertText: '{"-": [$1, $2]}',
    detail: 'Subtraction',
    documentation: '**Subtraction**\n```json\n{"-":[10, {"var":"used"}]}\n```'
  },
  {
    label: '*',
    insertText: '{"*": [$1, $2]}',
    detail: 'Multiplication',
    documentation: '**Multiplication**\n```json\n{"*":[{"var":"quantity"},{"var":"unitPrice"}]}\n```'
  },
  {
    label: '/',
    insertText: '{"/": [$1, $2]}',
    detail: 'Division',
    documentation: '**Division**\n```json\n{"/":[{"var":"distance"},{"var":"time"}]}\n```'
  },
  {
    label: 'var',
    insertText: '{"var": "$1"}',
    detail: 'Variable reference',
    documentation: '**Variable Reference**\nResolves a value from provided context.\n```json\n{"var":"user.role"}\n```'
  },
  {
    label: 'in',
    insertText: '{"in": [$1, $2]}',
    detail: 'Contains',
    documentation: '**Contains**\nLeft value must be present in right array/string.\n```json\n{"in":[{"var":"role"}, ["admin","ops"]]}\n```'
  },
  {
    label: 'missing',
    insertText: '{"missing": [$1]}',
    detail: 'Missing keys',
    documentation: '**Missing Keys**\nReturns array of missing property names.\n```json\n{"missing":["email","userId"]}\n```'
  },
  {
    label: 'missing_some',
    insertText: '{"missing_some": [$1, $2]}',
    detail: 'Minimum present keys',
    documentation: '**Missing Some**\nEnsure at least N keys exist.\n```json\n{"missing_some":[2,["a","b","c"]]}\n```'
  }
];

export const JSONLOGIC_OPERATOR_LABELS = new Set(JSONLOGIC_OPERATORS.map(o => o.label));

// Default variable context (can be overridden at runtime)
export const DEFAULT_JSONLOGIC_VARIABLES: string[] = [
  'instance.id',
  'instance.status',
  'instance.startedAt',
  'task.id',
  'task.assignee',
  'task.assigneeRoles',
  'nowUtc',
  'workflow.version',
  'workflow.key',
  'branch.arrivals',      // join context candidate
  'branch.totalExpected', // join context candidate
  'input.payload',        // user-supplied data
  'user.id',
  'user.roles'
];

let runtimeVariables: string[] = [...DEFAULT_JSONLOGIC_VARIABLES];

export function setJsonLogicVariables(vars: string[]) {
  runtimeVariables = Array.from(new Set(vars.filter(v => typeof v === 'string' && v.trim().length)));
}

export function getJsonLogicVariables(): string[] {
  return runtimeVariables;
}
