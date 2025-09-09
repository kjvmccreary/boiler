/**
 * Progress (deduped) utility.
 * rawPercent = visitedCount / totalNodes (includes start)
 * dedupedPercent = unique visited executable nodes / total executable nodes (excludes 'start')
 */
export interface ProgressMetrics {
  totalNodes: number;
  executableTotal: number;
  visitedCount: number;
  dedupVisitedExecutable: number;
  rawPercent: number;
  dedupedPercent: number;
}

export function computeDedupedProgress(definitionJson: string | null, visitedNodeIds: string[]): ProgressMetrics {
  let totalNodes = 0;
  let executableTotal = 0;
  let dedupVisitedExecutable = 0;
  const visitedCount = visitedNodeIds.length;

  if (definitionJson) {
    try {
      const parsed = JSON.parse(definitionJson);
      const nodes: any[] = Array.isArray(parsed?.nodes) ? parsed.nodes : [];
      totalNodes = nodes.length;

      const execIds = new Set<string>(
        nodes
          .filter(n => n && n.id && n.type !== 'start')
          .map(n => n.id)
      );

      executableTotal = execIds.size;

      if (execIds.size > 0) {
        const seenExec = new Set<string>();
        for (const v of visitedNodeIds) {
          if (execIds.has(v)) seenExec.add(v);
        }
        dedupVisitedExecutable = seenExec.size;
      }
    } catch {
      // ignore parse errors; metrics fall back to zeros
    }
  }

  const rawPercent = totalNodes > 0 ? (visitedCount / totalNodes) * 100 : 0;
  const dedupedPercent =
    executableTotal > 0
      ? (dedupVisitedExecutable / executableTotal) * 100
      : rawPercent;

  return {
    totalNodes,
    executableTotal,
    visitedCount,
    dedupVisitedExecutable,
    rawPercent,
    dedupedPercent
  };
}

/**
 * Should we emit a variance telemetry event?
 * threshold (percentage points) default = 1.
 */
export function shouldEmitVariance(rawPercent: number, dedupedPercent: number, threshold = 1): boolean {
  return Math.abs(rawPercent - dedupedPercent) >= threshold;
}
