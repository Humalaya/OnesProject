using System;
using System.Linq;

namespace backend.Application.Common
{
    public static class ToDoFieldMapper
    {
        public static int PriorityToInt(string? priority) => priority?.ToLowerInvariant() switch
        {
            "low" => 0,
            "high" => 2,
            _ => 1 // "medium" or unrecognized/empty defaults to Medium
        };

        public static string PriorityToString(int priority) => priority switch
        {
            0 => "low",
            2 => "high",
            _ => "medium"
        };

        public static string? TagsToString(string[]? tags) =>
            tags == null || tags.Length == 0
                ? null
                : string.Join(",", tags.Select(t => t.Trim()).Where(t => t.Length > 0));

        public static string[] TagsToArray(string? tags) =>
            string.IsNullOrWhiteSpace(tags)
                ? Array.Empty<string>()
                : tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
