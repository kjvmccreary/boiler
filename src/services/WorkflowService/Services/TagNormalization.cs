using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkflowService.Services;

internal static class TagNormalization
{
    // Normalizes a tag string (comma-delimited) into:
    // - canonical: comma-joined, original casing from first occurrence preserved
    // - list: ordered, de-duplicated (case-insensitive)
    public static (string canonical, List<string> list) Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return (string.Empty, new List<string>());

        // Split only on commas; trim; keep multi-word phrases intact.
        var pieces = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim())
            .Where(t => t.Length > 0)
            .Select(t => CollapseSpaces(t))
            .ToList();

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ordered = new List<string>();
        foreach (var p in pieces)
        {
            if (seen.Add(p))
                ordered.Add(p);
        }

        var canonical = string.Join(',', ordered);
        return (canonical, ordered);
    }

    private static string CollapseSpaces(string value)
    {
        if (value.IndexOf("  ", StringComparison.Ordinal) < 0)
            return value;
        return System.Text.RegularExpressions.Regex.Replace(value, "\\s{2,}", " ");
    }

    // Builds a predicate-safe boundary pattern string: ,tag,
    internal static string BuildBoundaryNeedle(string tag) => $",{tag},";

    // Prepares a tags column for boundary search: ,stored,
    internal static string WrapStored(string? stored) =>
        stored is null ? string.Empty : $",{stored},";
}
