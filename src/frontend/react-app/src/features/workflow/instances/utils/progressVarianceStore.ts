/**
 * Lightweight in-memory variance aggregation (per browser session).
 * Records variance deltas between raw and deduped progress for each instance.
 * Used to emit a summary telemetry event once an instance reaches a terminal state.
 */

export interface ProgressVarianceSample {
  ts: number;
  raw: number;
  deduped: number;
  delta: number;
}

export interface ProgressVarianceSummary {
  instanceId: number;
  samples: number;
  avgDelta: number;
  maxDelta: number;
  lastDelta: number;
  firstDelta: number;
  rawAtMax: number;
  dedupedAtMax: number;
}

const store = new Map<number, ProgressVarianceSample[]>();

export function recordProgressVariance(instanceId: number, raw: number, deduped: number) {
  const delta = Math.abs(raw - deduped);
  if (!store.has(instanceId)) store.set(instanceId, []);
  store.get(instanceId)!.push({
    ts: Date.now(),
    raw,
    deduped,
    delta
  });
}

export function getProgressVarianceSummary(instanceId: number): ProgressVarianceSummary | null {
  const list = store.get(instanceId);
  if (!list || list.length === 0) return null;
  const samples = list.length;
  let sum = 0;
  let maxDelta = -1;
  let rawAtMax = 0;
  let dedupedAtMax = 0;
  for (const s of list) {
    sum += s.delta;
    if (s.delta > maxDelta) {
      maxDelta = s.delta;
      rawAtMax = s.raw;
      dedupedAtMax = s.deduped;
    }
  }
  return {
    instanceId,
    samples,
    avgDelta: sum / samples,
    maxDelta,
    lastDelta: list[list.length - 1].delta,
    firstDelta: list[0].delta,
    rawAtMax,
    dedupedAtMax
  };
}

export function clearProgressVariance(instanceId: number) {
  store.delete(instanceId);
}
