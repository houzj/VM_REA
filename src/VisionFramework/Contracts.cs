using System.Text.Json;
using VisionCore;
namespace VisionFramework;
public enum DataKind { GrayImage, Number, Integer, Boolean, Text, Components, Circle, Region, Fixture, Point, RgbImage, Image, NumberArray, Calibration }
public static class DataKinds {
 public static bool IsImage(DataKind kind)=>kind is DataKind.GrayImage or DataKind.RgbImage or DataKind.Image;
 public static bool CanFlow(DataKind source,DataKind target)=>source==target||source==DataKind.Number&&target==DataKind.NumberArray||source==DataKind.Image&&IsImage(target)||target==DataKind.Image&&IsImage(source);
 public static bool CanFlow(DataKind source,PortDescriptor target)=>CanFlow(source,target.Kind)||target.AcceptInteger&&source==DataKind.Integer&&target.Kind is DataKind.Number or DataKind.NumberArray;
 public static PortValue Coerce(PortValue value,DataKind target)=>value.Kind==DataKind.Number&&target==DataKind.NumberArray?PortValue.Numbers([value.As<double>()]):CanFlow(value.Kind,target)?value:throw new InvalidDataException("Value type mismatch.");
 public static PortValue Coerce(PortValue value,PortDescriptor target)=>PortTypes.DeferredReadAccepts(target,value.Kind)?value:target.AcceptInteger&&value.Kind==DataKind.Integer?target.Kind switch{DataKind.Number=>PortValue.Number((float)value.As<long>()),DataKind.NumberArray=>PortValue.Numbers([(float)value.As<long>()]),_=>throw new InvalidDataException("Invalid integer promotion target.")}:Coerce(value,target.Kind);
}
public enum ParameterKind { Number, Integer, Boolean, Text }
public sealed record PortDescriptor(string Name,DataKind Kind,bool Required=true,bool AcceptInteger=false){public ValueCardinality Cardinality{get;init;}=ValueCardinality.Scalar;public bool SupportsDeferredNumericRead{get;init;}public bool SupportsDeferredIntegerRead{get;init;}public bool SupportsDeferredTextRead{get;init;}public bool CanSubscribe{get;init;}=true;}
public enum ParameterEditorKind{File,Folder,Enumeration,Range,Roi}
public sealed record ParameterEditorChoice(string Name,JsonElement Value);
public sealed record ParameterEditorDefinition(ParameterEditorKind Kind,IReadOnlyList<string>? Fields=null,IReadOnlyList<ParameterEditorChoice>? Choices=null,string? Filter=null){public RoiEditorCapabilities? Roi{get;init;}};
public sealed record ParameterDescriptor(string Name,ParameterKind Kind,bool Required,JsonElement? DefaultValue=null,double? Minimum=null,double? Maximum=null){public ParameterEditorDefinition? Editor{get;init;}}
public sealed record ModuleDescriptor(string Id,string Version,string DisplayName,IReadOnlyList<PortDescriptor> Inputs,IReadOnlyList<PortDescriptor> Outputs,IReadOnlyList<ParameterDescriptor> Parameters){public RoiEditorCapabilities? RoiEditor{get;init;}public bool SupportsDynamicOutputs{get;init;}}
public sealed record ResourceDocument(string Id,string Path,string? Sha256=null){[System.Text.Json.Serialization.JsonIgnore(Condition=System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]public string? EmbeddedBase64{get;init;}}
public sealed record NodeDocument(string Id,string ModuleId,string ModuleVersion,string Name,Dictionary<string,JsonElement> Parameters,double X=0,double Y=0,int TimeoutMs=0,bool Enabled=true,IReadOnlyList<RoiDocument>? Rois=null){[System.Text.Json.Serialization.JsonIgnore(Condition=System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]public IReadOnlyDictionary<string,RoiDisplayStyle>? DisplayStyles{get;init;}[System.Text.Json.Serialization.JsonIgnore(Condition=System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]public IReadOnlyDictionary<string,IReadOnlyList<RoiDocument>>? RoiGroups{get;init;}public WorkflowCallDocument? Call{get;init;}[System.Text.Json.Serialization.JsonIgnore(Condition=System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]public GroupBreakBindings? GroupBreak{get;init;}};

public sealed record ConnectionDocument(string FromNode,string FromPort,string ToNode,string ToPort){public int SourceIndex{get;init;}=3;public int TargetIndex{get;init;}=1;}
public sealed record WirePointDocument(double X,double Y);
public sealed record ExecutionLinkDocument(string FromNode,string ToNode){public int SourceIndex{get;init;}=3;public int TargetIndex{get;init;}=1;public IReadOnlyList<WirePointDocument>? PathPoints{get;init;}}
// Null links retain legacy data-DAG ordering; an explicit list separates execution and subscriptions.
public sealed record GroupOwnership(string ProcedureId,string NodeId);
public sealed record ProcedureDocument(string Id,string Name,IReadOnlyList<NodeDocument> Nodes,IReadOnlyList<ConnectionDocument> Connections,IReadOnlyList<ExecutionLinkDocument>? ExecutionLinks=null){public GroupOwnership? GroupOwner{get;init;}public int CanvasZoomIndex{get;init;}=9;public double? OverviewX{get;init;}public double? OverviewY{get;init;}public int ContinuousRunIntervalMs{get;init;}=80;public int WaitTimeMs{get;init;}public bool StopOnNg{get;init;}public WorkflowConfiguration Configuration{get;init;}=new();}
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
    public static PortValue Image(IImageData v)=>v switch{GrayImage gray=>Image(gray),RgbImage rgb=>new(DataKind.RgbImage,rgb),RawImage raw=>new(DataKind.Image,raw),_=>throw new ArgumentException("Unsupported owned image format.",nameof(v))};
    public static PortValue Number(double v)=>double.IsFinite(v)?new(DataKind.Number,v):throw new ArgumentOutOfRangeException(nameof(v));
    public static PortValue Numbers(IEnumerable<double> values){var copy=values.ToArray();if(copy.Any(v=>!double.IsFinite(v)))throw new ArgumentException("Non-finite numeric array.",nameof(values));return new(DataKind.NumberArray,Array.AsReadOnly(copy));}
    public static PortValue Calibration(CalibrationData value)=>new(DataKind.Calibration,value??throw new ArgumentNullException(nameof(value)));
    public static PortValue Integer(long v)=>new(DataKind.Integer,v);
    public static PortValue Boolean(bool v)=>new(DataKind.Boolean,v);
    public static PortValue Text(string v)=>new(DataKind.Text,v??throw new ArgumentNullException(nameof(v)));
    public static PortValue Components(IReadOnlyList<Component> v)=>new(DataKind.Components,Array.AsReadOnly(v.ToArray()));
    public static PortValue Point(Point2 point){if(!double.IsFinite(point.X)||!double.IsFinite(point.Y))throw new ArgumentException("点坐标无效。");return new(DataKind.Point,point);}
    public static PortValue Region(IEnumerable<RoiDocument> regions)=>new(DataKind.Region,new RoiRegionValue(regions));
    public static PortValue Fixture(FixtureValue value){value.ValidateTransport();return new(DataKind.Fixture,value);}
    public static PortValue Circle(CircleFit v)=>new[]{v.CenterX,v.CenterY,v.Radius,v.RadialRmsError}.All(double.IsFinite)&&v.Radius>0&&v.RadialRmsError>=0?new(DataKind.Circle,v):throw new ArgumentException("Invalid circle value.");
    public T As<T>()=>Value is T v?v:throw new InvalidOperationException($"Value {Kind} cannot be read as {typeof(T).Name}.");
}
public enum ModuleStatus { Success, NoMatch, Failed }
public enum ModuleContinuation { Continue, SuppressSuccessors }
public sealed record ExecutionSuccessor(string NodeId,int? NumericId=null);
public sealed record ModuleResult(ModuleStatus Status,IReadOnlyDictionary<string,PortValue> Outputs,string? ErrorCode=null,string? Message=null){public ModuleContinuation Continuation{get;init;}public IReadOnlyList<string>? SelectedExecutionSuccessors{get;init;}public uint? NativeErrorCode{get;init;}public ModuleDescriptor? UpdatedDescriptor{get;init;}public bool PreserveFailedOutputs{get;init;}}
public enum NodeRunStatus { Pending, Running, Success, NoMatch, Failed, Skipped, Cancelled, TimedOut }
public enum SessionStatus { Success, Failed, Cancelled, TimedOut }
public sealed record ExecutionEvent(string NodeId,NodeRunStatus Status,string? Message=null){public ModuleDescriptor? UpdatedDescriptor{get;init;}}
public sealed record NodeRunRecord(string NodeId,NodeRunStatus Status,IReadOnlyDictionary<string,PortValue> Outputs,double ElapsedMilliseconds,string? ErrorCode=null,string? Message=null){public uint? NativeErrorCode{get;init;}}
public sealed record ExecutionSessionResult(SessionStatus Status,IReadOnlyList<NodeRunRecord> Nodes){public IReadOnlyDictionary<string,IReadOnlyList<PortValue>> WorkflowDisplays{get;init;}=new Dictionary<string,IReadOnlyList<PortValue>>();public IReadOnlyList<string> DisplayErrors{get;init;}=[];public bool GroupBroke{get;init;}public int? GroupBreakIndex{get;init;}public IReadOnlyList<string> GroupDiagnostics{get;init;}=[];public int? GroupLoopIndex{get;init;}public int CompletedGroupIterations{get;init;}public IReadOnlyDictionary<string,PortValue> WorkflowInputs{get;init;}=new Dictionary<string,PortValue>();public IReadOnlyDictionary<string,ExecutionSessionResult> Calls{get;init;}=new Dictionary<string,ExecutionSessionResult>();public IReadOnlyDictionary<string,PortValue> WorkflowOutputs{get;init;}=new Dictionary<string,PortValue>();public IReadOnlyDictionary<string,PortValue> LocalVariables{get;init;}=new Dictionary<string,PortValue>();public IReadOnlyList<string> OutputErrors{get;init;}=[];public IReadOnlyDictionary<string,ModuleDescriptor> NodeDescriptors{get;init;}=new Dictionary<string,ModuleDescriptor>();}

public sealed class ModuleExecutionContext {
    public IReadOnlyList<ExecutionSuccessor> ExecutionSuccessors {get;init;}=Array.Empty<ExecutionSuccessor>();
    public ModuleExecutionScope? Scope {get;init;}
    public ModuleRunControl? Control {get;init;}
    public string? ProcedureId {get;init;}
    public ResourceCatalog? Resources {get;init;}
    // Per-execution, opt-in primitive read. Null means this input is not handled.
    public Func<string,int,ModuleNumericInputReadResult?>? ReadNumericInput {get;init;}
    public Func<string,int,ModuleIntegerInputReadResult?>? ReadIntegerInput {get;init;}
    public Func<string,int,int,ModuleTextInputReadResult?>? ReadTextInput {get;init;}
    public string NodeId {get;} public string BaseDirectory {get;}
    public IReadOnlyDictionary<string,PortValue> Inputs {get;}
    public IReadOnlyDictionary<string,JsonElement> Parameters {get;}
    public IReadOnlyList<RoiDocument> Rois {get;}
    public IReadOnlyDictionary<string,IReadOnlyList<RoiDocument>> RoiGroups {get;}
    public ModuleExecutionContext(string nodeId,string baseDirectory,IReadOnlyDictionary<string,PortValue> inputs,IReadOnlyDictionary<string,JsonElement> parameters,IReadOnlyList<RoiDocument>? rois=null,IReadOnlyDictionary<string,IReadOnlyList<RoiDocument>>? roiGroups=null){NodeId=nodeId;BaseDirectory=baseDirectory;Inputs=inputs;Parameters=parameters;Rois=Array.AsReadOnly(SnapshotRois(rois??Array.Empty<RoiDocument>()));RoiGroups=new System.Collections.ObjectModel.ReadOnlyDictionary<string,IReadOnlyList<RoiDocument>>((roiGroups??new Dictionary<string,IReadOnlyList<RoiDocument>>()).ToDictionary(p=>p.Key,p=>(IReadOnlyList<RoiDocument>)Array.AsReadOnly(SnapshotRois(p.Value)),StringComparer.Ordinal));}
    static RoiDocument[] SnapshotRois(IEnumerable<RoiDocument> rois)=>rois.Select(r=>r with{Vertices=r.Vertices is null?null:Array.AsReadOnly(r.Vertices.ToArray())}).ToArray();
    public T Input<T>(string name)=>Inputs.TryGetValue(name,out var value)?value.As<T>():throw new InvalidOperationException("Missing input: "+name);
    public string Text(string name)=>Parameters[name].GetString()??throw new InvalidDataException("Null text parameter: "+name);
    public double Number(string name)=>Parameters[name].GetDouble();
    public int Integer(string name)=>Parameters[name].GetInt32();
    public bool Boolean(string name)=>Parameters[name].GetBoolean();
    public string ResolvePath(string path)=>Path.GetFullPath(path,BaseDirectory);
}
public interface IModule {ModuleDescriptor Descriptor {get;} ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken cancellationToken);}
public sealed class ModuleRegistry {
    readonly Dictionary<(string Id,string Version),(ModuleDescriptor Descriptor,Func<IModule> Factory,Func<NodeDocument,ModuleDescriptor>? DescribeNode)> modules=new();
    public IReadOnlyList<ModuleDescriptor> Descriptors=>modules.Values.Select(x=>x.Descriptor).OrderBy(x=>x.Id,StringComparer.Ordinal).ThenBy(x=>x.Version,StringComparer.Ordinal).ToArray();
    public void Register(ModuleDescriptor descriptor,Func<IModule> factory,Func<NodeDocument,ModuleDescriptor>? describeNode=null){
        ArgumentNullException.ThrowIfNull(factory);var frozen=Freeze(descriptor);if(!modules.TryAdd((frozen.Id,frozen.Version),(frozen,factory,describeNode)))throw new InvalidOperationException("Duplicate module registration.");
    }
    static ModuleDescriptor Freeze(ModuleDescriptor descriptor){
        ArgumentNullException.ThrowIfNull(descriptor);
        if(string.IsNullOrWhiteSpace(descriptor.Id)||string.IsNullOrWhiteSpace(descriptor.Version)||string.IsNullOrWhiteSpace(descriptor.DisplayName)||descriptor.Inputs is null||descriptor.Outputs is null||descriptor.Parameters is null)throw new ArgumentException("Invalid module descriptor.");
        bool BadPorts(IReadOnlyList<PortDescriptor> ports)=>ports.Any(p=>p is null||string.IsNullOrWhiteSpace(p.Name)||(!PortTypes.Valid(p.Kind,p.Cardinality)||p.AcceptInteger&&p.Kind is not (DataKind.Number or DataKind.NumberArray)||p.SupportsDeferredNumericRead&&(p.Kind is not(DataKind.Number or DataKind.NumberArray)||p.Cardinality!=ValueCardinality.Scalar)||p.SupportsDeferredIntegerRead&&p.Kind is not(DataKind.Integer or DataKind.Number or DataKind.NumberArray)||p.SupportsDeferredTextRead&&p.Kind!=DataKind.Text))||ports.Select(p=>p.Name).Distinct(StringComparer.Ordinal).Count()!=ports.Count;
        if(BadPorts(descriptor.Inputs)||BadPorts(descriptor.Outputs)||descriptor.Parameters.Any(p=>p is null||string.IsNullOrWhiteSpace(p.Name)||!Enum.IsDefined(p.Kind)||p.Minimum is double min&&!double.IsFinite(min)||p.Maximum is double max&&!double.IsFinite(max)||p.Minimum>p.Maximum)||descriptor.Parameters.Select(p=>p.Name).Distinct(StringComparer.Ordinal).Count()!=descriptor.Parameters.Count)throw new ArgumentException("Invalid module ports or parameters.");
        if(descriptor.RoiEditor is {} nodeRoi&&(nodeRoi.Maximum<1||nodeRoi.Maximum>1024||nodeRoi.PolygonPointMaximum<3||nodeRoi.PolygonPointMaximum>10000||nodeRoi.Kinds is null||nodeRoi.Kinds.Any(k=>!Enum.IsDefined(k))))throw new ArgumentException("Invalid node ROI capabilities.");
        foreach(var parameter in descriptor.Parameters)if(parameter.Editor is {} editor){if(!Enum.IsDefined(editor.Kind)||editor.Fields?.Any(f=>!descriptor.Parameters.Any(p=>p.Name==f))==true)throw new ArgumentException("Invalid parameter editor metadata.");if(editor.Roi is {} capability&&(editor.Kind!=ParameterEditorKind.Roi||capability.Maximum<1||capability.Maximum>1024||capability.PolygonPointMaximum<3||capability.PolygonPointMaximum>10000||capability.Kinds is null||capability.Kinds.Any(k=>!Enum.IsDefined(k))))throw new ArgumentException("Invalid ROI editor capabilities.");if(editor.Kind==ParameterEditorKind.Range&&(editor.Fields?.Count!=2||editor.Fields.Any(f=>descriptor.Parameters.Single(p=>p.Name==f).Kind is not (ParameterKind.Integer or ParameterKind.Number))))throw new ArgumentException("Range editor requires two numeric fields.");if(editor.Kind==ParameterEditorKind.Enumeration&&(editor.Choices is null||editor.Choices.Count==0))throw new ArgumentException("Enumeration editor requires choices.");}
        var frozen=descriptor with {RoiEditor=descriptor.RoiEditor is {} nodeCapability?nodeCapability with{Kinds=Array.AsReadOnly(nodeCapability.Kinds.ToArray())}:null,Inputs=Array.AsReadOnly(descriptor.Inputs.ToArray()),Outputs=Array.AsReadOnly(descriptor.Outputs.ToArray()),Parameters=Array.AsReadOnly(descriptor.Parameters.Select(p=>p with {DefaultValue=p.DefaultValue?.Clone(),Editor=p.Editor is {} e?e with{Fields=e.Fields?.ToArray(),Choices=e.Choices?.Select(c=>c with{Value=c.Value.Clone()}).ToArray(),Roi=e.Roi is {} roi?roi with{Kinds=Array.AsReadOnly(roi.Kinds.ToArray())}:null}:null}).ToArray())};
        return frozen;
    }
    public ModuleDescriptor GetDescriptor(NodeDocument node){var item=modules.TryGetValue((node.ModuleId,node.ModuleVersion),out var registered)?registered:throw new KeyNotFoundException($"Unknown module: {node.ModuleId}@{node.ModuleVersion}");var descriptor=item.DescribeNode?.Invoke(node)??item.Descriptor;if(descriptor.Id!=node.ModuleId||descriptor.Version!=node.ModuleVersion)throw new InvalidDataException("Dynamic module identity mismatch.");return CompoundOutputFields.Describe(Freeze(node.ModuleId==WorkflowCalls.ModuleId?WorkflowCalls.Describe(node):descriptor));}

    public ModuleDescriptor ValidateUpdatedDescriptor(ModuleDescriptor registered,ModuleDescriptor updated){
        ArgumentNullException.ThrowIfNull(registered);ArgumentNullException.ThrowIfNull(updated);
        if(!registered.SupportsDynamicOutputs||!updated.SupportsDynamicOutputs)throw new InvalidDataException("Module does not support dynamic output descriptors.");
        if(registered.Id!=updated.Id||registered.Version!=updated.Version||registered.DisplayName!=updated.DisplayName)throw new InvalidDataException("Dynamic descriptor identity changed.");
        static string Shape<T>(T value)=>JsonSerializer.Serialize(value);
        if(Shape(registered.Inputs)!=Shape(updated.Inputs)||Shape(registered.Parameters)!=Shape(updated.Parameters)||Shape(registered.RoiEditor)!=Shape(updated.RoiEditor))throw new InvalidDataException("Dynamic descriptor may update outputs only.");
        return CompoundOutputFields.Describe(Freeze(updated));
    }

    public ModuleDescriptor GetDescriptor(string id,string version)=>modules.TryGetValue((id,version),out var item)?item.Descriptor:throw new KeyNotFoundException($"Unknown module: {id}@{version}");
    public IModule Create(string id,string version){var item=modules.TryGetValue((id,version),out var m)?m:throw new KeyNotFoundException($"Unknown module: {id}@{version}");var result=item.Factory()??throw new InvalidOperationException("Module factory returned null.");if(result.Descriptor.Id!=id||result.Descriptor.Version!=version)throw new InvalidOperationException("Module factory identity mismatch.");return result;}
}
public sealed record ValidationIssue(string Code,string Message,string? NodeId=null);
public sealed record ProcedureValidationResult(IReadOnlyList<ValidationIssue> Issues,IReadOnlyList<string> ExecutionOrder){public bool IsValid=>Issues.Count==0;}
