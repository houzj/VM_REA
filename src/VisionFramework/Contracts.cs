using System.Text.Json;
using VisionCore;
namespace VisionFramework;
public enum DataKind { GrayImage, Number, Integer, Boolean, Text, Components, Circle }
public enum ParameterKind { Number, Integer, Boolean, Text }
public sealed record PortDescriptor(string Name,DataKind Kind,bool Required=true){public ValueCardinality Cardinality{get;init;}=ValueCardinality.Scalar;}
public sealed record ParameterDescriptor(string Name,ParameterKind Kind,bool Required,JsonElement? DefaultValue=null,double? Minimum=null,double? Maximum=null);
public sealed record ModuleDescriptor(string Id,string Version,string DisplayName,IReadOnlyList<PortDescriptor> Inputs,IReadOnlyList<PortDescriptor> Outputs,IReadOnlyList<ParameterDescriptor> Parameters);
public sealed record ResourceDocument(string Id,string Path,string? Sha256=null);
public sealed record NodeDocument(string Id,string ModuleId,string ModuleVersion,string Name,Dictionary<string,JsonElement> Parameters,double X=0,double Y=0,int TimeoutMs=0,bool Enabled=true,IReadOnlyList<RoiDocument>? Rois=null){public WorkflowCallDocument? Call{get;init;}};
public sealed record ConnectionDocument(string FromNode,string FromPort,string ToNode,string ToPort){public int SourceIndex{get;init;}=3;public int TargetIndex{get;init;}=1;}
public sealed record WirePointDocument(double X,double Y);
public sealed record ExecutionLinkDocument(string FromNode,string ToNode){public int SourceIndex{get;init;}=3;public int TargetIndex{get;init;}=1;public IReadOnlyList<WirePointDocument>? PathPoints{get;init;}}
// Null links retain legacy data-DAG ordering; an explicit list separates execution and subscriptions.
public sealed record GroupOwnership(string ProcedureId,string NodeId);
public sealed record ProcedureDocument(string Id,string Name,IReadOnlyList<NodeDocument> Nodes,IReadOnlyList<ConnectionDocument> Connections,IReadOnlyList<ExecutionLinkDocument>? ExecutionLinks=null){public GroupOwnership? GroupOwner{get;init;}public double? OverviewX{get;init;}public double? OverviewY{get;init;}public int ContinuousRunIntervalMs{get;init;}=80;public bool StopOnNg{get;init;}public WorkflowConfiguration Configuration{get;init;}=new();}
public static class ProcedureGraph {
 public static IReadOnlyList<ExecutionLinkDocument> ExecutionEdges(ProcedureDocument p)=>p.ExecutionLinks??p.Connections.Select(c=>new ExecutionLinkDocument(c.FromNode,c.ToNode)).Distinct().ToArray();
 public static ProcedureDocument Separate(ProcedureDocument p)=>p with{ExecutionLinks=ExecutionEdges(p).ToArray()};
}
public sealed record SolutionDocument(int FormatVersion,string Id,string Name,IReadOnlyList<ProcedureDocument> Procedures,IReadOnlyList<ResourceDocument> Resources);
public sealed partial class PortValue {
    public DataKind Kind {get;} public object Value {get;}
    public ValueCardinality Cardinality{get;}
    private PortValue(DataKind kind,object value,ValueCardinality cardinality=ValueCardinality.Scalar){Kind=kind;Value=value;Cardinality=cardinality;}
    public static PortValue Image(GrayImage v)=>new(DataKind.GrayImage,v??throw new ArgumentNullException(nameof(v)));
    public static PortValue Number(double v)=>double.IsFinite(v)?new(DataKind.Number,v):throw new ArgumentOutOfRangeException(nameof(v));
    public static PortValue Integer(long v)=>new(DataKind.Integer,v);
    public static PortValue Boolean(bool v)=>new(DataKind.Boolean,v);
    public static PortValue Text(string v)=>new(DataKind.Text,v??throw new ArgumentNullException(nameof(v)));
    public static PortValue Components(IReadOnlyList<Component> v)=>new(DataKind.Components,Array.AsReadOnly(v.ToArray()));
    public static PortValue Circle(CircleFit v)=>new[]{v.CenterX,v.CenterY,v.Radius,v.RadialRmsError}.All(double.IsFinite)&&v.Radius>0&&v.RadialRmsError>=0?new(DataKind.Circle,v):throw new ArgumentException("Invalid circle value.");
    public T As<T>()=>Value is T v?v:throw new InvalidOperationException($"Value {Kind} cannot be read as {typeof(T).Name}.");
}
public enum ModuleStatus { Success, NoMatch, Failed }
public sealed record ModuleResult(ModuleStatus Status,IReadOnlyDictionary<string,PortValue> Outputs,string? ErrorCode=null,string? Message=null);
public enum NodeRunStatus { Pending, Running, Success, NoMatch, Failed, Skipped, Cancelled, TimedOut }
public enum SessionStatus { Success, Failed, Cancelled, TimedOut }
public sealed record ExecutionEvent(string NodeId,NodeRunStatus Status,string? Message=null);
public sealed record NodeRunRecord(string NodeId,NodeRunStatus Status,IReadOnlyDictionary<string,PortValue> Outputs,double ElapsedMilliseconds,string? ErrorCode=null,string? Message=null);
public sealed record ExecutionSessionResult(SessionStatus Status,IReadOnlyList<NodeRunRecord> Nodes){public IReadOnlyDictionary<string,ExecutionSessionResult> Calls{get;init;}=new Dictionary<string,ExecutionSessionResult>();public IReadOnlyDictionary<string,PortValue> WorkflowOutputs{get;init;}=new Dictionary<string,PortValue>();public IReadOnlyDictionary<string,PortValue> LocalVariables{get;init;}=new Dictionary<string,PortValue>();public IReadOnlyList<string> OutputErrors{get;init;}=[];}
public sealed class ModuleExecutionContext {
    public string NodeId {get;} public string BaseDirectory {get;}
    public IReadOnlyDictionary<string,PortValue> Inputs {get;}
    public IReadOnlyDictionary<string,JsonElement> Parameters {get;}
    public IReadOnlyList<RoiDocument> Rois {get;}
    public ModuleExecutionContext(string nodeId,string baseDirectory,IReadOnlyDictionary<string,PortValue> inputs,IReadOnlyDictionary<string,JsonElement> parameters,IReadOnlyList<RoiDocument>? rois=null){NodeId=nodeId;BaseDirectory=baseDirectory;Inputs=inputs;Parameters=parameters;Rois=Array.AsReadOnly((rois??Array.Empty<RoiDocument>()).Select(r=>r with{Vertices=r.Vertices is null?null:Array.AsReadOnly(r.Vertices.ToArray())}).ToArray());}
    public T Input<T>(string name)=>Inputs.TryGetValue(name,out var value)?value.As<T>():throw new InvalidOperationException("Missing input: "+name);
    public string Text(string name)=>Parameters[name].GetString()??throw new InvalidDataException("Null text parameter: "+name);
    public double Number(string name)=>Parameters[name].GetDouble();
    public int Integer(string name)=>Parameters[name].GetInt32();
    public bool Boolean(string name)=>Parameters[name].GetBoolean();
    public string ResolvePath(string path)=>Path.GetFullPath(path,BaseDirectory);
}
public interface IModule {ModuleDescriptor Descriptor {get;} ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken cancellationToken);}
public sealed class ModuleRegistry {
    readonly Dictionary<(string Id,string Version),(ModuleDescriptor Descriptor,Func<IModule> Factory)> modules=new();
    public IReadOnlyList<ModuleDescriptor> Descriptors=>modules.Values.Select(x=>x.Descriptor).OrderBy(x=>x.Id,StringComparer.Ordinal).ThenBy(x=>x.Version,StringComparer.Ordinal).ToArray();
    public void Register(ModuleDescriptor descriptor,Func<IModule> factory){
        ArgumentNullException.ThrowIfNull(descriptor);ArgumentNullException.ThrowIfNull(factory);
        if(string.IsNullOrWhiteSpace(descriptor.Id)||string.IsNullOrWhiteSpace(descriptor.Version)||string.IsNullOrWhiteSpace(descriptor.DisplayName)||descriptor.Inputs is null||descriptor.Outputs is null||descriptor.Parameters is null)throw new ArgumentException("Invalid module descriptor.");
        bool BadPorts(IReadOnlyList<PortDescriptor> ports)=>ports.Any(p=>p is null||string.IsNullOrWhiteSpace(p.Name)||!PortTypes.Valid(p.Kind,p.Cardinality))||ports.Select(p=>p.Name).Distinct(StringComparer.Ordinal).Count()!=ports.Count;
        if(BadPorts(descriptor.Inputs)||BadPorts(descriptor.Outputs)||descriptor.Parameters.Any(p=>p is null||string.IsNullOrWhiteSpace(p.Name)||!Enum.IsDefined(p.Kind)||p.Minimum is double min&&!double.IsFinite(min)||p.Maximum is double max&&!double.IsFinite(max)||p.Minimum>p.Maximum)||descriptor.Parameters.Select(p=>p.Name).Distinct(StringComparer.Ordinal).Count()!=descriptor.Parameters.Count)throw new ArgumentException("Invalid module ports or parameters.");
        var frozen=descriptor with {Inputs=Array.AsReadOnly(descriptor.Inputs.ToArray()),Outputs=Array.AsReadOnly(descriptor.Outputs.ToArray()),Parameters=Array.AsReadOnly(descriptor.Parameters.Select(p=>p with {DefaultValue=p.DefaultValue?.Clone()}).ToArray())};
        if(!modules.TryAdd((frozen.Id,frozen.Version),(frozen,factory)))throw new InvalidOperationException("Duplicate module registration.");
    }
    public ModuleDescriptor GetDescriptor(NodeDocument node){var descriptor=GetDescriptor(node.ModuleId,node.ModuleVersion);return node.ModuleId==WorkflowCalls.ModuleId?WorkflowCalls.Describe(node):descriptor;}
    public ModuleDescriptor GetDescriptor(string id,string version)=>modules.TryGetValue((id,version),out var item)?item.Descriptor:throw new KeyNotFoundException($"Unknown module: {id}@{version}");
    public IModule Create(string id,string version){var item=modules.TryGetValue((id,version),out var m)?m:throw new KeyNotFoundException($"Unknown module: {id}@{version}");var result=item.Factory()??throw new InvalidOperationException("Module factory returned null.");if(result.Descriptor.Id!=id||result.Descriptor.Version!=version)throw new InvalidOperationException("Module factory identity mismatch.");return result;}
}
public sealed record ValidationIssue(string Code,string Message,string? NodeId=null);
public sealed record ProcedureValidationResult(IReadOnlyList<ValidationIssue> Issues,IReadOnlyList<string> ExecutionOrder){public bool IsValid=>Issues.Count==0;}



