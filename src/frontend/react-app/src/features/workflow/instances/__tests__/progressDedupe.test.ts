import { describe, it, expect } from 'vitest';
import { computeDedupedProgress, shouldEmitVariance } from '../utils/progress';

const defJson = JSON.stringify({
  nodes: [
    { id: 'start1', type: 'start' },
    { id: 'A', type: 'humanTask' },
    { id: 'B', type: 'automatic' },
    { id: 'C', type: 'gateway' },
    { id: 'D', type: 'end' }
  ],
  edges: []
});

describe('computeDedupedProgress', () => {
  it('excludes start node from executable total', () => {
    const m = computeDedupedProgress(defJson, []);
    expect(m.totalNodes).toBe(5);
    expect(m.executableTotal).toBe(4); // start excluded
  });

  it('dedupes revisits', () => {
    const m = computeDedupedProgress(defJson, ['start1', 'A', 'B', 'A', 'C']);
    // visitedCount includes duplicates
    expect(m.visitedCount).toBe(5);
    // dedup visited executable excludes start1 and duplicate A
    expect(m.dedupVisitedExecutable).toBe(3); // A,B,C
    // raw percent uses totalNodes=5 with visitedCount 5 = 100
    expect(m.rawPercent).toBeCloseTo(100);
    // dedup percent uses executableTotal=4, dedupVisited=3 => 75%
    expect(m.dedupedPercent).toBeCloseTo(75);
  });

  it('falls back to raw when no executable nodes (edge case)', () => {
    const emptyDef = JSON.stringify({ nodes: [{ id: 's', type: 'start' }], edges: [] });
    const m = computeDedupedProgress(emptyDef, ['s']);
    expect(m.executableTotal).toBe(0);
    expect(m.dedupedPercent).toBe(m.rawPercent);
  });

  it('handles invalid JSON gracefully', () => {
    const m = computeDedupedProgress('{bad json', ['A']);
    expect(m.totalNodes).toBe(0);
    expect(m.dedupedPercent).toBe(0);
  });
});

describe('shouldEmitVariance', () => {
  it('emits when delta >= 1', () => {
    expect(shouldEmitVariance(50, 48.9)).toBe(false);
    expect(shouldEmitVariance(50, 48.0)).toBe(true);
  });

  it('respects custom threshold', () => {
    expect(shouldEmitVariance(50, 49.4, 0.5)).toBe(true);
    expect(shouldEmitVariance(50, 49.7, 0.8)).toBe(false);
  });
});
