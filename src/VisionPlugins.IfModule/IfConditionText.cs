using System.Text;

namespace VisionPlugins.IfModule;

// The source-confirmed managed IfModuleControl IFText UI contract. Native parsing is tracked separately.
public sealed record IfConditionRow(string Name, string Kind, string Minimum, string Maximum, string Remark, bool Visible = true);

public static class IfConditionText {
    public static string Serialize(IReadOnlyList<IfConditionRow> rows) {
        var text = new StringBuilder();
        foreach (var row in rows.Where(x => x.Visible)) {
            if (row.Remark.Length > 256 || row.Remark.Contains(':') || row.Remark.Contains(';'))
                throw new ArgumentException("Remark violates the original IfDynamicIO editor gate.", nameof(rows));
            text.Append(row.Name).Append(':').Append(row.Minimum).Append(':')
                .Append(row.Maximum).Append(':').Append(row.Remark).Append(';');
        }
        return text.Length == 0 ? "0" : text.ToString();
    }

    // Original GetParam iterates existing visible rows and restores matching names; malformed rows are ignored.
    public static IReadOnlyList<IfConditionRow> Restore(IReadOnlyList<IfConditionRow> existing, string serialized) {
        var parts = serialized.Split(';');
        var restored = new IfConditionRow[existing.Count];
        for (int i = 0; i < existing.Count; i++) {
            var current = existing[i];
            if (!current.Visible) { restored[i] = current; continue; }
            foreach (var part in parts) {
                var fields = part.Split(':');
                if (fields.Length > 2 && fields[0] == current.Name) {
                    current = current with { Minimum = fields[1], Maximum = fields[2] };
                    if (fields.Length <= 3) break; // The original caught the missing remark after writing both bounds.
                    current = current with { Remark = fields[3] };
                }
            }
            restored[i] = current;
        }
        return Array.AsReadOnly(restored);
    }
}
