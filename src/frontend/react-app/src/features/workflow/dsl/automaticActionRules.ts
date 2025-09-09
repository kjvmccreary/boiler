import type { AutomaticAction, AutomaticActionWebhook, AutomaticNode } from './dsl.types';

export interface AutomaticActionValidation {
  errors: string[];
  warnings: string[];
}

/**
 * validateAutomaticAction
 * Performs shallow static validation of AutomaticNode.action.
 * - Treats absent action as implicit noop (no errors).
 * - Enforces url/method for webhook.
 * - Performs header key/value sanity checks.
 * - Advises (warnings) on large bodies or JSON-like bodies that fail parsing.
 * - Validates retryPolicy ranges; warns on aggressive settings.
 * NOTE: Does not perform any outbound network call or template interpolation.
 */
export function validateAutomaticAction(node: AutomaticNode): AutomaticActionValidation {
  const errors: string[] = [];
  const warnings: string[] = [];

  const action = node.action;
  if (!action) {
    // Treat missing action as noop (no errors) – UI will encourage explicit selection later.
    return { errors, warnings };
  }

  if (action.kind === 'noop') {
    return { errors, warnings };
  }

  // Webhook validation
  const wh = action as AutomaticActionWebhook;

  if (!wh.url || !wh.url.trim()) {
    errors.push('webhook url is required');
  } else if (!/^https?:\/\//i.test(wh.url.trim())) {
    errors.push('webhook url must start with http:// or https://');
  }

  if (!wh.method) {
    errors.push('webhook method is required');
  }

  // Headers sanity
  if (wh.headers) {
    for (const [k, v] of Object.entries(wh.headers)) {
      if (!k.trim()) {
        errors.push('webhook headers contain an empty key');
        break;
      }
      if (v == null) {
        errors.push(`webhook header "${k}" has empty value`);
        break;
      }
    }
  }

  // Body template (if present, basic JSON parse advisory only if it "looks" like JSON)
  if (wh.bodyTemplate && wh.bodyTemplate.trim().length) {
    const body = wh.bodyTemplate.trim();
    if ((body.startsWith('{') && body.endsWith('}')) || (body.startsWith('[') && body.endsWith(']'))) {
      try {
        JSON.parse(body);
      } catch {
        warnings.push('bodyTemplate is not valid JSON (allowed, but will be sent as-is)');
      }
    }
    if (body.length > 10_000) {
      warnings.push('bodyTemplate length exceeds 10KB');
    }
  }

  // Retry policy
  if (wh.retryPolicy) {
    const { maxAttempts, backoffSeconds } = wh.retryPolicy;
    if (maxAttempts == null || maxAttempts < 1) {
      errors.push('retryPolicy.maxAttempts must be >= 1');
    }
    if (backoffSeconds == null || backoffSeconds < 0) {
      errors.push('retryPolicy.backoffSeconds must be >= 0');
    }
    if (maxAttempts > 1 && (backoffSeconds ?? 0) === 0) {
      warnings.push('retryPolicy backoffSeconds is 0 (rapid retry)');
    }
    if (maxAttempts > 10) {
      warnings.push('retryPolicy maxAttempts > 10 (consider reducing)');
    }
  }

  return { errors, warnings };
}
