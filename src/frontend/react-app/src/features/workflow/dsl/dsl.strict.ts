// Strict structural (dominance-style) convergence analysis for Parallel -> Join.
// This supplements heuristic diagnostics in dsl.validate.ts without replacing them.
// Approach: forward reachability sets per branch + iterative elimination to find
// joins dominated by the gateway (i.e., every path from gateway to join passes through join).
// We also compute strict common joins and mismatches vs heuristic result.

import type { DslDefinition } from './dsl.types';
import type { StructuralDiagnostics } from './dsl.validate';

export interface StrictParallelGatewayResult {
  gatewayId: string;
  branchCount: number;
  dominatedJoins: string[];            // Joins strictly common (all branches hit one of these at first convergence level)
  allFirstJoins: string[];             // Union of first-join sets
  perBranchFirstJoins: Record<string, string[]>;
  orphanBranches: string[];
  heuristicCommon: string[];           // From heuristic diagnostics
  strictCommon: string[];              // Same as dominatedJoins (alias for clarity)
  missingFromHeuristic: string[];      // strictCommon not reported by heuristic
  heuristicOnly: string[];             // heuristic common not in strict common
  analysisTimeMs: number;
}

export interface StrictStructuralReport {
  generatedAt: string;
  gatewayResults: StrictParallelGatewayResult[];
  warnings: string[];
}

interface GraphMaps {
  from: Record<string, string[]>;
}

function buildMaps(def: DslDefinition): GraphMaps {
  const from: Record<string, string[]> = {};
  for (const e of def.edges) {
    (from[e.from] ||= []).push(e.to);
  }
  return { from };
}

function scanFirstJoins(
  start: string,
  maps: GraphMaps,
  kindById: Record<string, string>,
  max = 500
) {
  const stack = [start];
  const seen = new Set<string>();
  const joins = new Set<string>();
  let steps = 0;
  while (stack.length) {
    const id = stack.pop()!;
    if (seen.has(id)) continue;
    seen.add(id);
    if (++steps > max) break;
    if (kindById[id] === 'join') {
      joins.add(id);
      continue; // stop past first join
    }
    for (const nxt of maps.from[id] || []) stack.push(nxt);
  }
  return [...joins];
}

export function runStrictStructuralAnalysis(
  def: DslDefinition,
  heuristic: StructuralDiagnostics
): StrictStructuralReport {
  const t0 = performance.now ? performance.now() : Date.now();
  const maps = buildMaps(def);
  const kind: Record<string, string> = {};
  def.nodes.forEach(n => { kind[n.id] = n.type; });

  const results: StrictParallelGatewayResult[] = [];

  for (const gw of def.nodes) {
    if (gw.type !== 'gateway') continue;
    const anyGw: any = gw;
    const strategy = anyGw.strategy || (anyGw.condition ? 'conditional' : 'exclusive');
    if (strategy !== 'parallel') continue;

    const branches = maps.from[gw.id] || [];
    if (branches.length < 2) continue;

    const perBranchFirst: Record<string, string[]> = {};
    const allFirst = new Set<string>();
    const orphanBranches: string[] = [];

    branches.forEach(b => {
      const firstJoins = scanFirstJoins(b, maps, kind);
      perBranchFirst[b] = firstJoins;
      if (firstJoins.length === 0) orphanBranches.push(b);
      firstJoins.forEach(j => allFirst.add(j));
    });

    // strict dominated = intersection of per-branch first joins
    let intersection: Set<string> | null = null;
    for (const b of branches) {
      const current = new Set<string>(perBranchFirst[b]);
      if (intersection == null) {
        intersection = current;
      } else {
        const prev = intersection; // non-null here
        // Avoid TS Set<never> inference: rebuild explicitly
        const next = new Set<string>();
        for (const val of prev) {
          if (current.has(val)) next.add(val);
        }
        intersection = next;
      }
    }
    const dominated = intersection ? [...intersection] : [];

    const heuristicEntry = heuristic.parallelGateways[gw.id];
    const heuristicCommon = heuristicEntry ? heuristicEntry.commonJoins : [];
    const heuristicSet = new Set(heuristicCommon);
    const strictSet = new Set(dominated);

    const missingFromHeuristic = dominated.filter(j => !heuristicSet.has(j));
    const heuristicOnly = heuristicCommon.filter(j => !strictSet.has(j));

    results.push({
      gatewayId: gw.id,
      branchCount: branches.length,
      dominatedJoins: dominated.slice().sort(),
      allFirstJoins: [...allFirst].sort(),
      perBranchFirstJoins: perBranchFirst,
      orphanBranches: orphanBranches.sort(),
      heuristicCommon: heuristicCommon.slice().sort(),
      strictCommon: dominated.slice().sort(),
      missingFromHeuristic: missingFromHeuristic.sort(),
      heuristicOnly: heuristicOnly.sort(),
      analysisTimeMs: 0
    });
  }

  const t1 = performance.now ? performance.now() : Date.now();
  const elapsed = t1 - t0;
  results.forEach(r => { r.analysisTimeMs = elapsed; });

  const warnings: string[] = [];
  results.forEach(r => {
    if (r.missingFromHeuristic.length) {
      warnings.push(`Gateway ${r.gatewayId}: heuristic missed ${r.missingFromHeuristic.join(', ')}`);
    }
    if (r.heuristicOnly.length) {
      warnings.push(`Gateway ${r.gatewayId}: heuristic included non-strict ${r.heuristicOnly.join(', ')}`);
    }
  });

  return {
    generatedAt: new Date().toISOString(),
    gatewayResults: results,
    warnings
  };
}
