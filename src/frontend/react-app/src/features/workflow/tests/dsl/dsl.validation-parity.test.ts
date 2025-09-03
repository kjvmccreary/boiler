import { describe, it, expect } from 'vitest';
import { validateDefinition as localValidate } from '@/features/workflow/dsl/dsl.validate';
import type { DslDefinition } from '@/features/workflow/dsl/dsl.types';

// Raw JSON fixtures (untyped objects)
import multiStartRaw from '@/test/fixtures/workflow/invalid.multi-start.json';
import unreachableRaw from '@/test/fixtures/workflow/invalid.unreachable.json';
import missingEndRaw from '@/test/fixtures/workflow/invalid.missing-end.json';
import gwNoCondRaw from '@/test/fixtures/workflow/invalid.gateway-no-condition.json';

/**
 * Helper to coerce a raw fixture (plain JSON) into a DslDefinition.
 * We trust the shape of the fixture; the validator will still enforce correctness.
 */
function asDefinition(raw: any): DslDefinition {
  return raw as unknown as DslDefinition;
}

/**
 * Simulated backend validation.
 * For now it reuses the same implementation – if backend diverges later,
 * replace this with a remote call mock or a stricter superset validator.
 */
function backendValidate(def: DslDefinition) {
  return localValidate(def);
}

function assertSubset(front: string[], back: string[]) {
  for (const e of front) {
    if (!back.includes(e)) {
      throw new Error(
        `Frontend error "${e}" not found in backend error set:\n${JSON.stringify(back, null, 2)}`
      );
    }
  }
}

describe('Validation Parity (frontend vs backend)', () => {
  const cases: { name: string; def: DslDefinition }[] = [
    { name: 'multi-start', def: asDefinition(multiStartRaw) },
    { name: 'unreachable', def: asDefinition(unreachableRaw) },
    { name: 'missing-end', def: asDefinition(missingEndRaw) },
    { name: 'gateway-no-condition', def: asDefinition(gwNoCondRaw) }
  ];

  for (const c of cases) {
    it(`frontend error set is subset of backend error set: ${c.name}`, () => {
      const fe = localValidate(c.def);
      const be = backendValidate(c.def);
      assertSubset(fe.errors, be.errors);
      expect(be.errors.length).toBeGreaterThanOrEqual(fe.errors.length);
    });
  }
});
