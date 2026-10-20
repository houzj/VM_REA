using VisionFramework;

namespace VisionPlugins.IfModule;

/// <summary>
/// Visible IfModuleDynamicIO AddIO/RemoveIO row and output-list transitions.
/// Input subscriptions and original SDK instance IDs are handled by their owners.
/// </summary>
public sealed class IfConditionDynamicInventory {
    readonly List<IfConditionPortRow> rows = [];
    readonly List<PortDescriptor> dynamicOutputs = [];

    public IReadOnlyList<IfConditionPortRow> Rows => rows.AsReadOnly();
    public IReadOnlyList<PortDescriptor> DynamicOutputs => dynamicOutputs.AsReadOnly();

    public void Add(IfConditionPortRow row) {
        ArgumentNullException.ThrowIfNull(row);
        // Reuse the source-confirmed visible AddIO int/float and name checks.
        var fresh = IfConditionPorts.VisibleRows([row]);
        if (rows.Any(existing => existing.Name == row.Name))
            throw new ArgumentException("Duplicate visible input name.", nameof(row));
        rows.Add(row);
        dynamicOutputs.AddRange(fresh.Outputs.Skip(IfConditionPorts.FixedOutputs.Count));
    }

    public IReadOnlyList<PortDescriptor> Remove(string inputName) {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputName);
        int index = rows.FindIndex(row => row.Name == inputName);
        if (index < 0) return [];
        var removed = rows[index];
        rows.RemoveAt(index);
        if (string.IsNullOrEmpty(removed.DisplayName)) return [];
        // Original RemoveIO deletes all output rows whose DisplayNameA begins
        // with the removed display name. This may include another row's suffixes.
        var affected = dynamicOutputs.Where(port => port.Name.StartsWith(removed.DisplayName, StringComparison.Ordinal)).ToArray();
        dynamicOutputs.RemoveAll(port => port.Name.StartsWith(removed.DisplayName, StringComparison.Ordinal));
        return Array.AsReadOnly(affected);
    }

    public IReadOnlyList<PortDescriptor> Outputs() =>
        Array.AsReadOnly(IfConditionPorts.FixedOutputs.Concat(dynamicOutputs).ToArray());
}
