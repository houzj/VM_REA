namespace VisionPluginContracts;

public sealed partial class ToolConfigurationState
{
    static bool IsNumericRange(ConfigurationElement element) => element.Tag is "IntegerBettween" or "FloatBettween";

    static bool IsNumericRangeMember(ConfigurationElement element) =>
        element.Tag == "Integer" && element.Parent?.Tag == "Integers" && element.Parent.Parent?.Tag == "IntegerBettween" ||
        element.Tag == "Float" && element.Parent?.Tag == "Floats" && element.Parent.Parent?.Tag == "FloatBettween";

    /// <summary>Source-declared ValueBettween.SubChildren, in declaration order.
    /// These are real parameter nodes; the composite name is not a backend parameter.</summary>
    public IReadOnlyList<ConfigurationNodeState> GetNumericRangeChildren(ConfigurationNodeState range)
    {
        ArgumentNullException.ThrowIfNull(range);
        if (!ReferenceEquals(GetNode(range.Element), range)) throw new ArgumentException("Range belongs to another configuration state.", nameof(range));
        if (!IsNumericRange(range.Element)) throw new ArgumentException("Unsupported numeric range type.", nameof(range));
        string collection = range.Element.Tag == "IntegerBettween" ? "Integers" : "Floats";
        return Array.AsReadOnly((range.Element.Children.FirstOrDefault(x => x.Tag == collection)?.Children ?? [])
            .Where(IsNumericRangeMember).Select(x => GetNode(x)!).ToArray());
    }
}
