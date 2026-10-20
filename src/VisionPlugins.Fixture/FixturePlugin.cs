using System.IO;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.Fixture;
public sealed partial class FixturePlugin:IVisionToolPlugin,IToolConfigurationProvider,IWpfToolEditorFactory,IToolDisplayProvider,IToolAssetPersistence,IToolPointInputAliasProvider,IToolScopedNodeCopyProvider {
 public const string Id="vision.fixture";
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","位置修正","location","catalog/IMVSFixtureModuImageEnable.png");
 public IReadOnlyList<ToolPointInputAlias> PointInputAliases{get;}=[new("Origin","InPointX","InPointY")];
 static readonly string[] Fields=["FixInitPointX","FixInitPointY","FixInitAngle","FixRunPointX","FixRunPointY","FixRunAngle","BasisInitDataFlag","InitFlag"];
 public ModuleDescriptor Descriptor{get;}=new(Id,"1.0.0","位置修正",new[]{"InPointX","InPointY","InAngle","MatchScaleX","MatchScaleY"}.Select(n=>new PortDescriptor(n,DataKind.Number,n is "InPointX" or "InPointY" or "InAngle"){SupportsDeferredNumericRead=true}).Append(new("Origin",DataKind.Point,false)).ToArray(),new[]{new PortDescriptor("ModuRunTime",DataKind.Number,false),new("ModuStatus",DataKind.Integer,false),new("Position Correction Info",DataKind.Fixture,false),new("Fixtured Point",DataKind.Point,false),new("Unfixtured Point",DataKind.Point,false)}.Concat(new[]{"FixtureInitPointX","FixtureInitPointY","FixtureInitAngle","FixtureInitScaleX","FixtureInitScaleY","FixtureRunPointX","FixtureRunPointY","FixtureRunAngle","FixtureRunScaleX","FixtureRunScaleY"}.Select(n=>new PortDescriptor(n,DataKind.Number,false))).ToArray(),Fields.Select(n=>new ParameterDescriptor(n,n.EndsWith("Flag")?ParameterKind.Integer:ParameterKind.Number,true,JsonSerializer.SerializeToElement(0))).Append(new("ConfigurationUserData",ParameterKind.Text,true,JsonSerializer.SerializeToElement("{}"))).Append(new("DisplayObjects",ParameterKind.Text,true,JsonSerializer.SerializeToElement("{}"))).ToArray());
 public ToolDisplayDefinition DisplayDefinition{get;}=new("Module Status:{0} BasePoint:({1},{2}) RunningPoint:({3},{4})",["ModuStatus","FixtureInitPointX","FixtureInitPointY","FixtureRunPointX","FixtureRunPointY"],["ModuStatus","FixtureInitPointX","FixtureInitPointY","FixtureRunPointX","FixtureRunPointY"]);
 static readonly IReadOnlyList<ToolDisplayObjectDefinition> DisplayObjects=ToolDisplayObjectDefinition.Parse(Resource("IMVSFixtureModuDisplay.xml"));
 sealed class State{internal readonly object Gate=new();internal readonly FixtureState Core=new();internal readonly Dictionary<string,JsonElement> Seed=[];internal Dictionary<string,JsonElement>? SaveProjection;internal Dictionary<string,PortValue>? Outputs,Inputs;}
 readonly Dictionary<string,State> fallback=[];
 State Get(string node,string directory,ModuleExecutionScope? scope){if(scope is not null)return scope.State(Id,node,()=>new State());lock(fallback){string key=Path.GetFullPath(directory)+"|"+node;if(!fallback.TryGetValue(key,out var state))fallback[key]=state=new();return state;}}
 static void Sync(State state,IReadOnlyDictionary<string,JsonElement> values){var prepared=state.SaveProjection;state.SaveProjection=null;foreach(string name in Fields)if(values.TryGetValue(name,out var value)&&(!state.Seed.TryGetValue(name,out var old)||old.GetRawText()!=value.GetRawText())){if(prepared is null||!prepared.TryGetValue(name,out var projected)||projected.GetRawText()!=value.GetRawText())Set(state.Core,name,value.GetDouble());state.Seed[name]=value.Clone();}}
 static void Set(FixtureState core,string name,double value){float f=(float)value;switch(name){case "FixInitPointX":core.Reference=core.Reference with{X=f};break;case "FixInitPointY":core.Reference=core.Reference with{Y=f};break;case "FixInitAngle":core.Reference=core.Reference with{Angle=f};break;case "FixRunPointX":core.Run=core.Run with{X=f};break;case "FixRunPointY":core.Run=core.Run with{Y=f};break;case "FixRunAngle":core.Run=core.Run with{Angle=f};break;case "BasisInitDataFlag":core.BasisInitDataFlag=(int)value;break;case "InitFlag":core.InitFlag=(int)value;break;}}
 static Dictionary<string,JsonElement> Snapshot(State state,IReadOnlyDictionary<string,JsonElement> values){var result=values.ToDictionary(p=>p.Key,p=>p.Value.Clone());double[] numeric=[state.Core.Reference.X,state.Core.Reference.Y,state.Core.Reference.Angle,state.Core.Run.X,state.Core.Run.Y,state.Core.Run.Angle,state.Core.BasisInitDataFlag,state.Core.InitFlag];for(int i=0;i<Fields.Length;i++)result[Fields[i]]=i>=6?JsonSerializer.SerializeToElement((int)numeric[i]):JsonSerializer.SerializeToElement(numeric[i]);return result;}
 public IModule CreateRuntime()=>new Runtime(this);
 public void Reset(string node){lock(fallback)foreach(string key in fallback.Keys.Where(k=>k.EndsWith("|"+node,StringComparison.Ordinal)).ToArray())fallback.Remove(key);}
 public ToolAssetSnapshot PrepareSave(NodeDocument node,string directory,ResourceCatalog resources)=>PrepareSave(node,directory,resources,null);
 public ToolAssetSnapshot PrepareSave(NodeDocument node,string directory,ResourceCatalog resources,ModuleExecutionScope? scope){var state=Get(node.Id,directory,scope);lock(state.Gate){Sync(state,node.Parameters);var saved=Snapshot(state,node.Parameters);foreach(string name in Fields.Take(6))saved[name]=JsonSerializer.SerializeToElement((double)FixtureSaveFormat.RoundTrip((float)saved[name].GetDouble()));state.SaveProjection=Fields.ToDictionary(name=>name,name=>saved[name].Clone());return new(node with{Parameters=saved},[]);}}
 public NodeDocument PrepareCopiedNode(NodeDocument source,NodeDocument copy,IReadOnlyDictionary<string,string> nodeMap,IReadOnlyList<ExecutionSuccessor> successors)=>PrepareCopiedNode(source,copy,nodeMap,successors,null);
 public NodeDocument PrepareCopiedNode(NodeDocument source,NodeDocument copy,IReadOnlyDictionary<string,string> nodeMap,IReadOnlyList<ExecutionSuccessor> successors,ModuleExecutionScope? sourceScope){
  if(source.ModuleId!=Id||copy.ModuleId!=Id)throw new ArgumentException("Unexpected Fixture copy.");
  var parameters=copy.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());
  IReadOnlyDictionary<string,JsonElement> native=source.Parameters;
  if(sourceScope is not null){var state=Get(source.Id,Environment.CurrentDirectory,sourceScope);lock(state.Gate){Sync(state,source.Parameters);native=Snapshot(state,source.Parameters);}}
  foreach(string name in Fields)if(native.TryGetValue(name,out var value))parameters[name]=name.EndsWith("Flag")?value.Clone():JsonSerializer.SerializeToElement((double)FixtureSaveFormat.RoundTrip((float)value.GetDouble()));
  return copy with{Parameters=parameters};
 }
 public ToolDisplayFrame? LastDisplay(NodeDocument node,string directory,ModuleExecutionScope? scope){var state=Get(node.Id,directory,scope);lock(state.Gate)return state.Outputs is null?null:ToolDisplayService.Build(DisplayDefinition,ToolDisplayService.Read(node.Parameters,DisplayDefinition),state.Inputs??[],state.Outputs) with{Graphics=ToolDisplayGraphics.Build(DisplayObjects,node.Parameters,state.Inputs??[],state.Outputs)};}
 internal static string Resource(string name){using var stream=typeof(FixturePlugin).Assembly.GetManifestResourceStream("VisionPlugins.Fixture.Resources."+name)??throw new InvalidDataException(name);using var reader=new StreamReader(stream);return reader.ReadToEnd();}
 public System.Windows.Window CreateWpfEditor(ToolEditorContext context)=>new ToolConfigurationModelWindow(context,CreateConfiguration(context));
 public System.Windows.Forms.Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use WPF configuration host.");
 sealed class Runtime(FixturePlugin owner):IModule{
  public ModuleDescriptor Descriptor=>owner.Descriptor;
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken token){token.ThrowIfCancellationRequested();var watch=System.Diagnostics.Stopwatch.StartNew();var state=owner.Get(c.NodeId,c.BaseDirectory,c.Scope);lock(state.Gate){Sync(state,c.Parameters);var result=state.Core.Process(name=>FixtureInputReader.Read(c,name));var outputs=new Dictionary<string,PortValue>();if(result.Status is {} status){outputs["ModuStatus"]=PortValue.Integer(status);outputs["ModuRunTime"]=PortValue.Number((float)watch.Elapsed.TotalMilliseconds);}if(result.Reference is {} reference&&result.Run is {} run){void Pose(string prefix,FixturePoseFields pose){outputs[prefix+"PointX"]=PortValue.Number(pose.X);outputs[prefix+"PointY"]=PortValue.Number(pose.Y);outputs[prefix+"Angle"]=PortValue.Number(pose.Angle);outputs[prefix+"ScaleX"]=PortValue.Number(pose.ScaleX);outputs[prefix+"ScaleY"]=PortValue.Number(pose.ScaleY);}Pose("FixtureRun",run);Pose("FixtureInit",reference);outputs["Fixtured Point"]=PortValue.Point(new(reference.X,reference.Y));outputs["Unfixtured Point"]=PortValue.Point(new(run.X,run.Y));outputs["Position Correction Info"]=PortValue.Fixture(new(new(reference.X,reference.Y,reference.Angle,reference.ScaleX,reference.ScaleY),new(run.X,run.Y,run.Angle,run.ScaleX,run.ScaleY)));}state.Inputs=c.Inputs.ToDictionary(p=>p.Key,p=>p.Value);state.Outputs=outputs;return ValueTask.FromResult(new ModuleResult(result.ErrorCode==0?ModuleStatus.Success:ModuleStatus.Failed,outputs,result.ErrorCode==0?null:"FixtureReadOrScale",result.ErrorCode==0?null:$"位置修正失败 0x{result.ErrorCode:x8}"){NativeErrorCode=result.ErrorCode});}}
 }
}





