import { describe, it, expect } from 'vitest';
import { validateHumanTaskAssignment } from '../assignmentRules';
import { validateDefinition } from '../dsl.validate';
import type { DslDefinition, HumanTaskNode } from '../dsl.types';

function humanTask(overrides: Partial<HumanTaskNode> = {}): HumanTaskNode {
  return {
    id: overrides.id || 'task1',
    type: 'humanTask',
    label: overrides.label || 'HT',
    x: 0,
    y: 0,
    assigneeRoles: overrides.assigneeRoles,
    dueInMinutes: overrides.dueInMinutes,
    formSchema: overrides.formSchema,
    assignment: overrides.assignment
  };
}

function baseDefinition(ht: HumanTaskNode): DslDefinition {
  return {
    key: 'wf-test',
    nodes: [
      { id: 'start', type: 'start', x: 0, y: 0 },
      ht,
      { id: 'end', type: 'end', x: 100, y: 0 }
    ],
    edges: [
      { id: 'e1', from: 'start', to: ht.id },
      { id: 'e2', from: ht.id, to: 'end' }
    ]
  };
}

describe('validateHumanTaskAssignment (unit)', () => {
  it('users mode with no users -> error', () => {
    const node = humanTask({
      assignment: { mode: 'users', users: [] }
    });
    const r = validateHumanTaskAssignment(node);
    expect(r.errors).toContain('At least one user must be selected for users mode.');
  });

  it('roles mode dedupes & warns on duplicates', () => {
    const node = humanTask({
      assignment: { mode: 'roles', roles: ['Ops', 'Ops', 'QA'] }
    });
    const r = validateHumanTaskAssignment(node);
    expect(r.errors.length).toBe(0);
    expect(r.normalized?.roles).toEqual(['Ops', 'QA']);
    expect(r.warnings).toContain('Duplicate roles removed.');
  });

  it('expression mode missing expression -> error', () => {
    const node = humanTask({
      assignment: { mode: 'expression', expression: '' }
    });
    const r = validateHumanTaskAssignment(node);
    expect(r.errors).toContain('Expression is required for expression mode.');
  });

  it('expression mode with expression -> ok', () => {
    const node = humanTask({
      assignment: { mode: 'expression', expression: '{"users":["u1"]}' }
    });
    const r = validateHumanTaskAssignment(node);
    expect(r.errors.length).toBe(0);
  });

  it('hybrid missing roles & expression -> two errors', () => {
    const node = humanTask({
      assignment: { mode: 'hybrid' }
    });
    const r = validateHumanTaskAssignment(node);
    expect(r.errors).toContain('Hybrid mode requires at least one role.');
    expect(r.errors).toContain('Hybrid mode requires an expression.');
  });

  it('hybrid valid -> ok', () => {
    const node = humanTask({
      assignment: {
        mode: 'hybrid',
        roles: ['Ops'],
        expression: '{"roles":["Backup"]}'
      }
    });
    const r = validateHumanTaskAssignment(node);
    expect(r.errors.length).toBe(0);
  });

  it('SLA soft >= target -> error', () => {
    const node = humanTask({
      assignment: {
        mode: 'roles',
        roles: ['Ops'],
        sla: { targetMinutes: 60, softWarningMinutes: 60 }
      }
    });
    const r = validateHumanTaskAssignment(node);
    expect(r.errors).toContain('SLA soft warning must be less than targetMinutes.');
  });

  it('SLA very low target -> warning', () => {
    const node = humanTask({
      assignment: {
        mode: 'roles',
        roles: ['Ops'],
        sla: { targetMinutes: 3 }
      }
    });
    const r = validateHumanTaskAssignment(node);
    expect(r.errors.length).toBe(0);
    expect(r.warnings).toContain('Very low SLA target may be unrealistic.');
  });

  it('Escalation before target -> warning', () => {
    const node = humanTask({
      assignment: {
        mode: 'roles',
        roles: ['Ops'],
        sla: { targetMinutes: 120 },
        escalation: { escalateToRole: 'Supervisors', afterMinutes: 60 }
      }
    });
    const r = validateHumanTaskAssignment(node);
    expect(r.warnings).toContain('Escalation occurs before or at SLA target.');
  });

  it('Legacy assigneeRoles migrates to roles assignment', () => {
    const node = humanTask({
      assigneeRoles: ['Ops']
    });
    const r = validateHumanTaskAssignment(node);
    expect(r.warnings).toContain('Legacy assigneeRoles mapped to assignment.roles (consider updating node).');
    expect(r.normalized?.mode).toBe('roles');
    expect(r.normalized?.roles).toEqual(['Ops']);
  });
});

describe('validateDefinition integration (assignment errors propagate)', () => {
  it('propagates assignment errors into global errors list', () => {
    const ht = humanTask({
      assignment: { mode: 'users', users: [] }, // invalid: no users
      id: 'taskA'
    });
    const def = baseDefinition(ht);
    const vr = validateDefinition(def);
    // Expect prefixed error from integration
    const found = vr.errors.find(e => e.includes('HumanTask') && e.includes('assignment'));
    expect(found).toBeTruthy();
  });

  it('no assignment but legacy roles triggers warning (not error)', () => {
    const ht = humanTask({
      assigneeRoles: ['Ops'],
      id: 'taskB'
    });
    const def = baseDefinition(ht);
    const vr = validateDefinition(def);
    const hasLegacyWarning = vr.warnings.some(w => w.includes('HumanTask') && w.includes('should have assignee roles'));
    // Should NOT warn because legacy roles present
    expect(hasLegacyWarning).toBe(false);
  });

  it('hybrid valid assignment does not add errors', () => {
    const ht = humanTask({
      assignment: {
        mode: 'hybrid',
        roles: ['Ops'],
        expression: '{"users":["u1"]}'
      },
      id: 'taskC'
    });
    const def = baseDefinition(ht);
    const vr = validateDefinition(def);
    const hasAssignmentError = vr.errors.some(e => e.includes('assignment:'));
    expect(hasAssignmentError).toBe(false);
  });
});
