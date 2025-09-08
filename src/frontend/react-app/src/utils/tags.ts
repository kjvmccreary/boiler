// Tag normalization utilities (comma-only delimiter policy).
// Policy:
//  - Split ONLY on commas.
//  - Preserve internal spaces (collapse multiple to single).
//  - Trim leading/trailing whitespace.
//  - De-duplicate case-insensitively, preserving first original casing.
//  - Ignore empty tokens.
//  - Returned order = first occurrence order.

export interface NormalizeTagsResult {
  original: string;
  normalized: string[];
  canonicalQuery: string; // comma-joined (no spaces)
}

export function normalizeTags(input: string | string[] | undefined | null): NormalizeTagsResult {
  if (input == null) {
    return { original: '', normalized: [], canonicalQuery: '' };
  }

  const raw = Array.isArray(input) ? input.join(',') : input;
  const pieces = raw.split(',').map(t => t.trim()).filter(t => t.length > 0);

  const seen = new Set<string>();
  const result: string[] = [];
  for (const p of pieces) {
    // Collapse internal multiple spaces to single (optional robustness)
    const collapsed = p.replace(/\s{2,}/g, ' ');
    const key = collapsed.toLowerCase();
    if (!seen.has(key)) {
      seen.add(key);
      result.push(collapsed);
    }
  }

  return {
    original: raw,
    normalized: result,
    canonicalQuery: result.join(',') // no added spaces
  };
}

// Convenience helpers
export function joinTagsForQuery(tags: string[] | undefined | null): string {
  if (!tags || tags.length === 0) return '';
  return normalizeTags(tags).canonicalQuery;
}

export function parseTagsFromBackend(stored: string | undefined | null): string[] {
  if (!stored) return [];
  return normalizeTags(stored).normalized;
}
