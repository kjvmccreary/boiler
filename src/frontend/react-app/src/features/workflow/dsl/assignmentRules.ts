import type {
  HumanTaskNode,
  HumanTaskAssignment,
  HumanTaskAssignmentMode
} from './dsl.types';

export interface AssignmentValidationResult {
  normalized?: HumanTaskAssignment;
  errors: string[];
  warnings: string[];
}

/**
 * Validate & normalize HumanTask assignment (H1).
 * - Performs shallow checks; expression semantic validation is deferred to existing expression validator.
 * - Duplicates removed (users / roles).
 * - SLA / escalation sanity constraints enforced.
 * - If no new assignment provided but legacy assigneeRoles exist, emits a synthesized roles-mode assignment (no error).
 */
export function validateHumanTaskAssignment(node: HumanTaskNode): AssignmentValidationResult {
  const errors: string[] = [];
  const warnings: string[] = [];

  let assignment: HumanTaskAssignment | undefined = node.assignment;

  // Backward compatibility migration (only if new model absent)
  if (!assignment && node.assigneeRoles?.length) {
    assignment = {
      mode: 'roles',
      roles: [...node.assigneeRoles]
    };
    warnings.push('Legacy assigneeRoles mapped to assignment.roles (consider updating node).');
  }

  if (!assignment) {
    return { errors, warnings, normalized: undefined };
  }

  const mode: HumanTaskAssignmentMode | undefined = assignment.mode;
  if (!mode) {
    errors.push('Assignment mode is required.');
    return { errors, warnings };
  }

  // Copy shallow to avoid mutating original
  const norm: HumanTaskAssignment = { ...assignment };

  // Dedupe arrays
  if (norm.users) {
    const before = norm.users.length;
    norm.users = Array.from(new Set(norm.users.map(u => u.trim()).filter(Boolean)));
    if (norm.users.length < before) warnings.push('Duplicate users removed.');
  }
  if (norm.roles) {
    const before = norm.roles.length;
    norm.roles = Array.from(new Set(norm.roles.map(r => r.trim()).filter(Boolean)));
    if (norm.roles.length < before) warnings.push('Duplicate roles removed.');
  }

  // Mode-specific validation
  switch (mode) {
    case 'users':
      if (!norm.users?.length) errors.push('At least one user must be selected for users mode.');
      break;
    case 'roles':
      if (!norm.roles?.length) errors.push('At least one role must be selected for roles mode.');
      break;
    case 'expression':
      if (!norm.expression || !norm.expression.trim()) errors.push('Expression is required for expression mode.');
      break;
    case 'hybrid':
      if (!norm.roles?.length) errors.push('Hybrid mode requires at least one role.');
      if (!norm.expression || !norm.expression.trim()) errors.push('Hybrid mode requires an expression.');
      break;
  }

  // SLA validation
  if (norm.sla) {
    const { targetMinutes, softWarningMinutes } = norm.sla;
    if (targetMinutes == null || isNaN(targetMinutes)) {
      errors.push('SLA targetMinutes is required when SLA is configured.');
    } else if (targetMinutes < 0) {
      errors.push('SLA targetMinutes cannot be negative.');
    } else if (targetMinutes < 5) {
      warnings.push('Very low SLA target may be unrealistic.');
    }
    if (softWarningMinutes != null) {
      if (softWarningMinutes < 0) {
        errors.push('SLA softWarningMinutes cannot be negative.');
      } else if (softWarningMinutes >= (targetMinutes ?? Number.MAX_SAFE_INTEGER)) {
        errors.push('SLA soft warning must be less than targetMinutes.');
      }
    }
  }

  // Escalation validation
  if (norm.escalation) {
    if (!norm.escalation.escalateToRole?.trim()) {
      errors.push('Escalation escalateToRole is required when escalation is configured.');
    }
    if (norm.escalation.afterMinutes == null || isNaN(norm.escalation.afterMinutes)) {
      errors.push('Escalation afterMinutes is required when escalation is configured.');
    } else if (norm.escalation.afterMinutes < 0) {
      errors.push('Escalation afterMinutes cannot be negative.');
    }
    if (norm.sla?.targetMinutes != null &&
        norm.escalation.afterMinutes != null &&
        norm.escalation.afterMinutes <= norm.sla.targetMinutes) {
      warnings.push('Escalation occurs before or at SLA target.');
    }
  } else if (norm.sla && norm.sla.targetMinutes != null) {
    // Escalation optional – no error, but could be a future advisory
  }

  return { errors, warnings, normalized: norm };
}

/**
 * Quick helper to surface assignment error strings (used when integrating into global validateDefinition later).
 */
export function summarizeAssignmentErrors(node: HumanTaskNode): { errors: string[]; warnings: string[] } {
  return validateHumanTaskAssignment(node);
}
