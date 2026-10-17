using System.Globalization;
using System.IO;
using System.Text.Json;
using VisionFramework;

namespace VisionPluginContracts;

/// <summary>Independent typed display-only bindings. These never become algorithm graph edges.</summary>
public sealed class ToolConfigurationDisplaySubscriptions : IToolConfigurationSubscriptionService
{
    public const string ParameterName = "DisplayInputBindings";
    public sealed record StoredBinding(string Kind, string NativeType, string? ProcedureId = null,
        string? NodeId = null, string? PortId = null, string? Literal = null);
    readonly IToolConfigurationSubscriptionService inner;
    readonly Func<ProcedureDocument> procedure;
    readonly ModuleRegistry registry;
    readonly string targetId;
    readonly string ownerProcedureId;
    readonly Func<IReadOnlyDictionary<string, JsonElement>> read;
    readonly Action<Dictionary<string, JsonElement>> apply;
    public static IReadOnlyList<string> UnsupportedCapabilities { get; } = new[] {
        "GlobalVariables", "LocalVariables", "WorkflowInputs", "ParentInputs", "ArrayIndex", "RemoteInputs", "NativeVisibilityMetadata" };

    public ToolConfigurationDisplaySubscriptions(IToolConfigurationSubscriptionService inner, Func<ProcedureDocument> procedure,
        ModuleRegistry registry, string targetNodeId, Func<IReadOnlyDictionary<string, JsonElement>> readParams,
        Action<Dictionary<string, JsonElement>> apply)
    {
        this.inner = inner; this.procedure = procedure; this.registry = registry; targetId = targetNodeId;
        read = readParams; this.apply = apply; ownerProcedureId = procedure().Id;
    }

    static bool Display(ConfigurationSubscriptionDestination d) => d.Domain == ConfigurationSubscriptionDomain.Display;
    static bool Position(string name) => name is ToolDisplayService.PositionXInput or ToolDisplayService.PositionYInput;
    static string? Native(PortDescriptor p) => p.Cardinality == ValueCardinality.Scalar ? p.Kind switch
    { DataKind.Integer => "int", DataKind.Number => "float", _ => null } : null;
    static string Id(params string[] parts) => string.Join("|", parts.Select(x => x.Length + ":" + x));
    ProcedureDocument Snapshot(ConfigurationSubscriptionDestination d)
    {
        var p = procedure();
        if (p.Id != ownerProcedureId || p.Nodes.Count(n => n.Id == targetId) != 1)
            throw new InvalidDataException("Display subscription owner has closed, changed procedure or been removed.");
        if (!Position(d.Name) || d.NativeType.Split(';').Any(t => t is not ("int" or "float")))
            throw new NotSupportedException("Only numeric Show Text position destinations are supported.");
        return p;
    }

    HashSet<string> Allowed(ProcedureDocument p)
    {
        if (p.GroupOwner is not null || p.ExecutionLinks is null)
            throw new NotSupportedException("Display binding requires explicit root-procedure execution links.");
        var ids = p.Nodes.Select(n => n.Id).ToHashSet(StringComparer.Ordinal);
        if (ids.Count != p.Nodes.Count) throw new InvalidDataException("Duplicate node identity.");
        if (p.ExecutionLinks.Any(e => !ids.Contains(e.FromNode) || !ids.Contains(e.ToNode)))
            throw new InvalidDataException("Execution link references missing node.");
        var result = new HashSet<string>(StringComparer.Ordinal); var pending = new Stack<string>(); pending.Push(targetId);
        while (pending.TryPop(out var id))
        {
            if (!result.Add(id)) continue;
            foreach (var edge in p.ExecutionLinks.Where(e => e.ToNode == id)) pending.Push(edge.FromNode);
        }
        // Cyclic execution ancestors are not a valid source catalog.
        var degree = result.ToDictionary(id => id, id => p.ExecutionLinks.Count(e => e.ToNode == id && result.Contains(e.FromNode)));
        var ready = new Queue<string>(degree.Where(x => x.Value == 0).Select(x => x.Key)); int count = 0;
        while (ready.TryDequeue(out var id)) { count++; foreach (var edge in p.ExecutionLinks.Where(e => e.FromNode == id && result.Contains(e.ToNode))) if (--degree[edge.ToNode] == 0) ready.Enqueue(edge.ToNode); }
        if (count != result.Count) throw new InvalidDataException("Execution predecessor cycle.");
        return result;
    }

    public ConfigurationSubscriptionTreeResult Enumerate(ConfigurationSubscriptionQuery query)
    {
        if (!Display(query.Destination)) return inner.Enumerate(query);
        try
        {
            var p = Snapshot(query.Destination); if (query.IndexOnly || query.IncludeInside) throw new NotSupportedException("Display indexes/internal modules are unavailable.");
            var allowed = Allowed(p); var roots = new List<ConfigurationSubscriptionTreeNode>();
            foreach (var node in p.Nodes.Where(n => allowed.Contains(n.Id) && n.Enabled && (query.IncludeSelf || n.Id != targetId)))
            {
                var leaves = registry.GetDescriptor(node).Outputs.Where(port => Native(port) is { } type &&
                    query.Destination.NativeType.Split(';').Contains(type) && (query.NativeTypes.Count == 0 || query.NativeTypes.Contains(type)))
                    .Select(port => new ConfigurationSubscriptionTreeNode(Id(p.Id, node.Id, port.Name), port.Name,
                        binding: new(new ConfigurationOutputReference(p.Id, node.Id, port.Name, Native(port)!), ConfigurationSubscriptionIndex.All))).ToArray();
                if (leaves.Length > 0) roots.Add(new(Id(p.Id, node.Id), node.Name, leaves));
            }
            if (query.IncludeEmpty) roots.Insert(0, new("display-unbind", "null", isUnbind: true));
            return new(ConfigurationSubscriptionStatus.Available, roots, UnsupportedCapabilities: UnsupportedCapabilities);
        }
        catch (Exception e) { return new(Status(e), [], e.Message, UnsupportedCapabilities); }
    }

    public ConfigurationSubscriptionBindingResult Read(ConfigurationSubscriptionDestination destination)
    {
        if (!Display(destination)) return inner.Read(destination);
        try { var p = Snapshot(destination); var item = Stored(read()).GetValueOrDefault(destination.Name); return Decode(p, destination, item); }
        catch (Exception e) { return new(Status(e), null, Diagnostic: e.Message); }
    }

    public ConfigurationSubscriptionBindingResult Bind(ConfigurationSubscriptionDestination destination, ConfigurationSubscriptionBinding? binding)
    {
        if (!Display(destination)) return inner.Bind(destination, binding);
        try
        {
            var p = Snapshot(destination); StoredBinding? value = null;
            if (binding is not null)
            {
                if (binding.Index.Kind != ConfigurationSubscriptionIndexKind.All) throw new NotSupportedException("Display index bindings are unavailable.");
                value = Encode(p, destination, binding.Reference);
            }
            var parameters = read().ToDictionary(x => x.Key, x => x.Value.Clone()); var map = Stored(parameters);
            if (value is null) map.Remove(destination.Name); else map[destination.Name] = value;
            parameters[ParameterName] = JsonSerializer.SerializeToElement(JsonSerializer.Serialize(map));
            apply(parameters);
            var after = Snapshot(destination); var actual = Stored(read()).GetValueOrDefault(destination.Name);
            if (after.Id != p.Id || actual != value) throw new InvalidDataException("Host did not persist the requested display binding.");
            return Decode(after, destination, actual);
        }
        catch (Exception e) { return new(Status(e), null, Diagnostic: e.Message); }
    }

    StoredBinding Encode(ProcedureDocument p, ConfigurationSubscriptionDestination d, ConfigurationSubscriptionReference reference)
    {
        switch (reference)
        {
            case ConfigurationOutputReference o:
                if (o.ProcedureId != p.Id || !Allowed(p).Contains(o.NodeId)) throw new NotSupportedException("Display source must be self or an explicit execution ancestor in the same procedure.");
                var node = p.Nodes.Single(n => n.Id == o.NodeId); if (!node.Enabled) throw new InvalidDataException("Display source is disabled.");
                var port = registry.GetDescriptor(node).Outputs.SingleOrDefault(x => x.Name == o.PortId);
                if (port is null || Native(port) != o.NativeType || !d.NativeType.Split(';').Contains(o.NativeType)) throw new InvalidDataException("Display output type/cardinality does not match.");
                return new("output", o.NativeType, p.Id, o.NodeId, o.PortId);
            case ConfigurationConstantReference c:
                if (!d.NativeType.Split(';').Contains(c.NativeType)) throw new InvalidDataException("Display constant native type mismatch.");
                if (c.NativeType == "int") return new("constant", "int", Literal: int.Parse(c.Literal, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture));
                if (c.NativeType == "float" && float.TryParse(c.Literal, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) && float.IsFinite(number)) return new("constant", "float", Literal: number.ToString("R", CultureInfo.InvariantCulture));
                throw new InvalidDataException("Finite int/float display constant required.");
            default: throw new NotSupportedException("Display globals/external/index references are unavailable.");
        }
    }

    ConfigurationSubscriptionBindingResult Decode(ProcedureDocument p, ConfigurationSubscriptionDestination d, StoredBinding? item)
    {
        if (item is null) return new(ConfigurationSubscriptionStatus.Available, null);
        ConfigurationSubscriptionReference r = item.Kind switch
        {
            "output" when item.ProcedureId is not null && item.NodeId is not null && item.PortId is not null => new ConfigurationOutputReference(item.ProcedureId, item.NodeId, item.PortId, item.NativeType),
            "constant" when item.Literal is not null => new ConfigurationConstantReference(item.NativeType, item.Literal),
            _ => throw new InvalidDataException("Invalid stored display binding.")
        };
        if (Encode(p, d, r) != item) throw new InvalidDataException("Noncanonical stored display binding.");
        var text = r is ConfigurationOutputReference o ? p.Nodes.Single(n => n.Id == o.NodeId).Name + "." + o.PortId : item.Literal;
        return new(ConfigurationSubscriptionStatus.Available, new(r, ConfigurationSubscriptionIndex.All), text);
    }
    static ConfigurationSubscriptionStatus Status(Exception e) => e is NotSupportedException ? ConfigurationSubscriptionStatus.Unsupported : ConfigurationSubscriptionStatus.Failed;
    static Dictionary<string, StoredBinding> Stored(IReadOnlyDictionary<string, JsonElement> parameters)
    {
        if (!parameters.TryGetValue(ParameterName, out var value)) return new(StringComparer.Ordinal);
        if (value.ValueKind != JsonValueKind.String) throw new InvalidDataException("DisplayInputBindings must be a JSON string parameter.");
        return JsonSerializer.Deserialize<Dictionary<string, StoredBinding>>(value.GetString() ?? "") ?? throw new InvalidDataException("Null display bindings.");
    }

    public static ToolDisplayFrame ApplyPositions(ToolDisplayFrame frame, NodeDocument node, string procedureId,
        Func<string, string, PortValue?> outputs, Action<string>? diagnostic = null)
    {
        Dictionary<string, StoredBinding> map;
        try { map = Stored(node.Parameters); } catch (Exception e) { diagnostic?.Invoke(e.Message); return frame with { PositionX = 0, PositionY = 0 }; }
        double Resolve(string key, double unchanged)
        {
            if (!map.TryGetValue(key, out var item)) return unchanged;
            try
            {
                if (item.Kind == "constant")
                {
                    if (item.NativeType == "int" && int.TryParse(item.Literal, NumberStyles.Integer, CultureInfo.InvariantCulture, out var integer)) return integer;
                    if (item.NativeType == "float" && float.TryParse(item.Literal, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) && float.IsFinite(number)) return number;
                }
                if (item.Kind == "output" && item.ProcedureId == procedureId && item.NodeId is not null && item.PortId is not null && outputs(item.NodeId, item.PortId) is { } value && value.Cardinality == ValueCardinality.Scalar)
                {
                    if (item.NativeType == "int" && value.Kind == DataKind.Integer) return value.As<long>();
                    if (item.NativeType == "float" && value.Kind == DataKind.Number && double.IsFinite(value.As<double>())) return value.As<double>();
                }
                diagnostic?.Invoke("Display position unavailable: " + key); return 0;
            }
            catch (Exception e) { diagnostic?.Invoke(key + ": " + e.Message); return 0; }
        }
        return frame with { PositionX = Resolve(ToolDisplayService.PositionXInput, frame.PositionX), PositionY = Resolve(ToolDisplayService.PositionYInput, frame.PositionY) };
    }
}
