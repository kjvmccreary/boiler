import { describe, it, expect } from 'vitest';
import { validateAutomaticAction } from '../automaticActionRules';
import { validateDefinition } from '../dsl.validate';
import type { AutomaticNode, DslDefinition } from '../dsl.types';

function automaticNode(overrides: Partial<AutomaticNode> = {}): AutomaticNode {
  return {
    id: overrides.id || 'auto1',
    type: 'automatic',
    label: overrides.label || 'Auto',
    x: 0,
    y: 0,
    action: overrides.action
  };
}

function baseDefinition(node: AutomaticNode): DslDefinition {
  return {
    key: 'wf-auto-test',
    nodes: [
      { id: 'start', type: 'start', x: 0, y: 0 },
      node,
      { id: 'end', type: 'end', x: 200, y: 0 }
    ] as any,
    edges: [
      { id: 'e1', from: 'start', to: node.id },
      { id: 'e2', from: node.id, to: 'end' }
    ]
  };
}

describe('validateAutomaticAction (unit)', () => {
  it('no action => no errors', () => {
    const node = automaticNode({ action: undefined });
    const r = validateAutomaticAction(node);
    expect(r.errors.length).toBe(0);
  });

  it('noop action => no errors', () => {
    const node = automaticNode({ action: { kind: 'noop' } });
    const r = validateAutomaticAction(node);
    expect(r.errors.length).toBe(0);
  });

  it('webhook missing url => error', () => {
    const node = automaticNode({
      action: { kind: 'webhook', url: '', method: 'POST' as const }
    });
    const r = validateAutomaticAction(node);
    expect(r.errors).toContain('webhook url is required');
  });

  it('webhook invalid url scheme => error', () => {
    const node = automaticNode({
      action: { kind: 'webhook', url: 'ftp://bad', method: 'POST' as const }
    });
    const r = validateAutomaticAction(node);
    expect(r.errors).toContain('webhook url must start with http:// or https://');
  });

  it('webhook missing method => error', () => {
    const node = automaticNode({
      action: { kind: 'webhook', url: 'https://ok', method: undefined as any }
    });
    const r = validateAutomaticAction(node);
    expect(r.errors).toContain('webhook method is required');
  });

  it('webhook header empty key => error', () => {
    const node = automaticNode({
      action: {
        kind: 'webhook',
        url: 'https://ok',
        method: 'POST',
        headers: { '': 'x' }
      }
    });
    const r = validateAutomaticAction(node);
    expect(r.errors).toContain('webhook headers contain an empty key');
  });

  it('webhook header empty value => error', () => {
    const node = automaticNode({
      action: {
        kind: 'webhook',
        url: 'https://ok',
        method: 'GET',
        headers: { Auth: undefined as any }
      }
    });
    const r = validateAutomaticAction(node);
    expect(r.errors).toContain('webhook header "Auth" has empty value');
  });

  it('bodyTemplate invalid JSON (looks like JSON) => warning only', () => {
    const node = automaticNode({
      action: {
        kind: 'webhook',
        url: 'https://ok',
        method: 'POST',
        bodyTemplate: '{ "a": 1' // invalid
      }
    });
    const r = validateAutomaticAction(node);
    expect(r.errors.length).toBe(0);
    expect(r.warnings).toContain('bodyTemplate is not valid JSON (allowed, but will be sent as-is)');
  });

  it('bodyTemplate large (>10KB) => warning', () => {
    const big = '{' + '"x":' + '"'.padEnd(10240, 'a') + '"}';
    const node = automaticNode({
      action: { kind: 'webhook', url: 'https://ok', method: 'POST', bodyTemplate: big }
    });
    const r = validateAutomaticAction(node);
    expect(r.warnings).toContain('bodyTemplate length exceeds 10KB');
  });

  it('retryPolicy invalid maxAttempts <1 => error', () => {
    const node = automaticNode({
      action: {
        kind: 'webhook',
        url: 'https://ok',
        method: 'POST',
        retryPolicy: { maxAttempts: 0, backoffSeconds: 1 }
      }
    });
    const r = validateAutomaticAction(node);
    expect(r.errors).toContain('retryPolicy.maxAttempts must be >= 1');
  });

  it('retryPolicy negative backoff => error', () => {
    const node = automaticNode({
      action: {
        kind: 'webhook',
        url: 'https://ok',
        method: 'POST',
        retryPolicy: { maxAttempts: 2, backoffSeconds: -1 }
      }
    });
    const r = validateAutomaticAction(node);
    expect(r.errors).toContain('retryPolicy.backoffSeconds must be >= 0');
  });

  it('retryPolicy maxAttempts >1 & backoffSeconds 0 => warning', () => {
    const node = automaticNode({
      action: {
        kind: 'webhook',
        url: 'https://ok',
        method: 'POST',
        retryPolicy: { maxAttempts: 3, backoffSeconds: 0 }
      }
    });
    const r = validateAutomaticAction(node);
    expect(r.warnings).toContain('retryPolicy backoffSeconds is 0 (rapid retry)');
  });

  it('retryPolicy maxAttempts > 10 => warning', () => {
    const node = automaticNode({
      action: {
        kind: 'webhook',
        url: 'https://ok',
        method: 'POST',
        retryPolicy: { maxAttempts: 11, backoffSeconds: 1 }
      }
    });
    const r = validateAutomaticAction(node);
    expect(r.warnings).toContain('retryPolicy maxAttempts > 10 (consider reducing)');
  });

  it('valid minimal webhook => no errors', () => {
    const node = automaticNode({
      action: { kind: 'webhook', url: 'https://ok', method: 'GET' }
    });
    const r = validateAutomaticAction(node);
    expect(r.errors.length).toBe(0);
  });
});

describe('validateDefinition integration (automatic actions)', () => {
  it('surfaces automatic action errors in global validation', () => {
    const auto = automaticNode({
      id: 'autoX',
      action: { kind: 'webhook', url: '', method: 'POST' }
    });
    const def = baseDefinition(auto);
    const vr = validateDefinition(def);
    const found = vr.errors.find(e => e.includes('Automatic') && e.includes('webhook url is required'));
    expect(found).toBeTruthy();
  });

  it('no errors when noop', () => {
    const auto = automaticNode({ id: 'autoY', action: { kind: 'noop' } });
    const def = baseDefinition(auto);
    const vr = validateDefinition(def);
    const autoErrors = vr.errors.filter(e => e.includes('Automatic'));
    expect(autoErrors.length).toBe(0);
  });
});
