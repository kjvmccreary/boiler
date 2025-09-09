using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkflowService.Services.Validation;

/// <summary>
/// Validates query-time tag filters (anyTags / allTags).
/// Public so test project can exercise logic without InternalsVisibleTo.
/// Policy:
///  - Max 12 tags per list
///  - Tag length 1..40
///  - Comma delimiter authoritative (spaces inside tag preserved; consecutive spaces collapsed)
///  - Case-insensitive de-duplication (first casing preserved)
/// Failure of ANY rule blocks entire request (422).
/// </summary>
public static class TagFilterValidator
{
    private const int MaxTagsPerList = 12;
    private const int MaxTagLength = 40;

    public sealed record TagFilterValidationResult(
        bool IsValid,
        IReadOnlyList<string> Errors,
        string? NormalizedAny,
        string? NormalizedAll);

    public static TagFilterValidationResult Validate(string? anyTagsRaw, string? allTagsRaw)
    {
        var errors = new List<string>();
        var normalizedAny = Normalize(anyTagsRaw);
        var normalizedAll = Normalize(allTagsRaw);

        if (normalizedAny.List.Count > MaxTagsPerList)
            errors.Add($"anyTags: too many tags (max {MaxTagsPerList})");
        if (normalizedAll.List.Count > MaxTagsPerList)
            errors.Add($"allTags: too many tags (max {MaxTagsPerList})");

        var tooLongAny = normalizedAny.List.FirstOrDefault(t => t.Length > MaxTagLength);
        if (tooLongAny != null)
            errors.Add($"anyTags: tag \"{tooLongAny}\" exceeds {MaxTagLength} characters");

        var tooLongAll = normalizedAll.List.FirstOrDefault(t => t.Length > MaxTagLength);
        if (tooLongAll != null)
            errors.Add($"allTags: tag \"{tooLongAll}\" exceeds {MaxTagLength} characters");

        // Reject empty tokens like ",," (Normalize already drops empties; if user supplied only delimiters treat as no tags)
        // No extra error needed unless both raw strings were non-empty yet normalize produced zero tags.
        if (!string.IsNullOrWhiteSpace(anyTagsRaw) && normalizedAny.List.Count == 0)
            errors.Add("anyTags: no valid tags parsed");
        if (!string.IsNullOrWhiteSpace(allTagsRaw) && normalizedAll.List.Count == 0)
            errors.Add("allTags: no valid tags parsed");

        var isValid = errors.Count == 0;

        return new TagFilterValidationResult(
            isValid,
            errors,
            isValid ? normalizedAny.Canonical : null,
            isValid ? normalizedAll.Canonical : null);
    }

    // Local normalization mirroring TagNormalization.Normalize (without dependency to keep scope tight)
    private static (string Canonical, List<string> List) Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return (string.Empty, new List<string>());

        var pieces = raw
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => CollapseSpaces(t.Trim()))
            .Where(t => t.Length > 0)
            .ToList();

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ordered = new List<string>();
        foreach (var p in pieces)
        {
            if (seen.Add(p))
                ordered.Add(p);
        }

        return (string.Join(',', ordered), ordered);
    }

    private static string CollapseSpaces(string value) =>
        value.IndexOf("  ", StringComparison.Ordinal) < 0
            ? value
            : System.Text.RegularExpressions.Regex.Replace(value, "\\s{2,}", " ");
}
