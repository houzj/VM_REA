using System.Text;

namespace VisionPlugins.IfModule;

/// <summary>
/// The managed IfParam.DynamicParams binary payload's normal UTF-8 record shape.
/// This is a parsed snapshot, not the original SDK-backed ParamItem instance.
/// </summary>
public sealed record IfDynamicParameter(string Name, string Kind, double? FirstNumericLimit, double? SecondNumericLimit);

public sealed record IfDynamicItemParameters(IfDynamicParameter Maximum, IfDynamicParameter Minimum, IfDynamicParameter Remark);

public static class IfConditionDynamicParameters {
    public static IReadOnlyDictionary<string, IfDynamicParameter> Parse(ReadOnlySpan<byte> data) {
        // Original IfParam.DynamicParams clears its dictionary on every successful
        // GetBinaryData, then uses UTF8, ';' and ',' with RemoveEmptyEntries.
        // Invalid UTF-8 rejection is an independent safe boundary of this port.
        var decoded = new UTF8Encoding(false, true).GetString(data);
        var items = new Dictionary<string, IfDynamicParameter>(StringComparer.Ordinal);
        foreach (var entry in decoded.Split(';', StringSplitOptions.RemoveEmptyEntries)) {
            var fields = entry.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (fields.Length <= 1) continue;
            var definition = fields[1] switch {
                "int" => new IfDynamicParameter(fields[0], "int", 999999999.0, -999999999.0),
                "float" => new IfDynamicParameter(fields[0], "float", 100000.0, -100000.0),
                "string" => new IfDynamicParameter(fields[0], "string", null, null),
                _ => null
            };
            if (definition is not null) items[fields[0]] = definition;
        }
        return items;
    }

    public static IfDynamicItemParameters? FindItem(IReadOnlyDictionary<string, IfDynamicParameter> definitions, string name) {
        ArgumentNullException.ThrowIfNull(definitions);
        if (string.IsNullOrEmpty(name)) throw new ArgumentException("Original IfParam rejects an empty item name.", nameof(name));
        return definitions.TryGetValue(name + "_Max", out var maximum)
            && definitions.TryGetValue(name + "_Min", out var minimum)
            && definitions.TryGetValue(name + "_ReMark", out var remark)
            ? new(maximum, minimum, remark) : null;
    }
}
