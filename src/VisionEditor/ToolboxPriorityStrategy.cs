using System.Globalization;

namespace VisionEditor;

/// <summary>
/// Source-backed adapter for VMControls.Data.ToolBoxData.ChangePriorityByInfo.
/// The original metadata is a semicolon-separated list of ToolBoxType:priority
/// pairs. Missing or malformed pairs are ignored by the original LINQ filter.
/// </summary>
internal static class ToolboxPriorityStrategy
{
    internal static double? Resolve(string? strategy, string toolboxType)
    {
        if (string.IsNullOrEmpty(strategy))
            return null;

        foreach (var pair in strategy.Split(';'))
        {
            var fields = pair.Split(':');
            if (fields.Length != 2 || fields[0] != toolboxType)
                continue;

            // VM uses float.Parse(value) after the two-field filter.
            if (float.TryParse(fields[1], NumberStyles.Float, CultureInfo.CurrentCulture, out var value))
                return value;
        }

        return null;
    }
}
