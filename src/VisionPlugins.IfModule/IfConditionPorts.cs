using VisionFramework;

namespace VisionPlugins.IfModule;

/// <summary>The visible IfModuleDynamicIO AddIO path; SDK-only string additions are not synthesized.</summary>
public sealed record IfConditionPortRow(string Name, string DisplayName, string Kind);

public static class IfConditionPorts {
    public static readonly IReadOnlyList<PortDescriptor> FixedOutputs = Array.AsReadOnly(new[] {
        new PortDescriptor("ModuRunTime",DataKind.Number,Required:false),
        new PortDescriptor("ModuStatus",DataKind.Integer,Required:false),
        new PortDescriptor("StrResult",DataKind.Text,Required:false),
        new PortDescriptor("NIfResult",DataKind.Integer,Required:false),
        new PortDescriptor("IfItemResult",DataKind.Text,Required:false){Cardinality=ValueCardinality.Array},
        new PortDescriptor("IfItemStatus",DataKind.Integer,Required:false){Cardinality=ValueCardinality.Array}
    });

    public static (IReadOnlyList<PortDescriptor> Inputs,IReadOnlyList<PortDescriptor> Outputs) VisibleRows(
        IReadOnlyList<IfConditionPortRow> rows) {
        ArgumentNullException.ThrowIfNull(rows);
        var inputs=new List<PortDescriptor>(rows.Count);
        var outputs=new List<PortDescriptor>(FixedOutputs);
        var inputNames=new HashSet<string>(StringComparer.Ordinal);
        var outputNames=FixedOutputs.Select(port=>port.Name).ToHashSet(StringComparer.Ordinal);
        foreach(var row in rows) {
            if(string.IsNullOrWhiteSpace(row.Name)||string.IsNullOrWhiteSpace(row.DisplayName))
                throw new ArgumentException("A visible condition row needs its source and display names.",nameof(rows));
            var kind=row.Kind switch {
                "int"=>DataKind.Integer,
                "float"=>DataKind.Number,
                _=>throw new ArgumentException("The original visible AddIO admits only int or float.",nameof(rows))
            };
            if(!inputNames.Add(row.Name))throw new ArgumentException("Duplicate visible input name.",nameof(rows));
            inputs.Add(new(row.Name,kind,Required:false){SupportsDeferredIntegerRead=kind==DataKind.Integer,
                SupportsDeferredNumericRead=kind==DataKind.Number,AcceptInteger=kind==DataKind.Number});
            foreach(var (suffix,outputKind) in new[]{("Out",DataKind.Integer),("Min",kind),("Max",kind)}) {
                var name=row.DisplayName+suffix;
                if(!outputNames.Add(name))throw new ArgumentException("Duplicate original output name.",nameof(rows));
                outputs.Add(new(name,outputKind,Required:false));
            }
        }
        return (Array.AsReadOnly(inputs.ToArray()),Array.AsReadOnly(outputs.ToArray()));
    }
}
