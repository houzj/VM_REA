using System.Diagnostics;
using System.IO;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.Calibration;

public sealed partial class CoordinateModulePlugin:IVisionToolPlugin,IWpfToolEditorFactory,IToolConfigurationProvider,IToolPointInputAliasProvider,IToolDisplayProvider
{
 public const string Id="vision.coordinate-module";
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","坐标系","calibration","catalog/CoordinateModuleImageEnable.png");
 public IReadOnlyList<ToolPointInputAlias> PointInputAliases{get;}=[new("Src Input Point","SrcPointX","SrcPointY")];
 static ParameterDescriptor P<T>(string n,ParameterKind k,T v,double? min=null,double? max=null)=>new(n,k,true,JsonSerializer.SerializeToElement(v),min,max);
 public ModuleDescriptor Descriptor{get;}=new(Id,"1.0.0","坐标系",
  new[]{"SrcPointX","SrcPointY","SrcAngle","CalibMatrix"}.Select(n=>new PortDescriptor(n,DataKind.NumberArray,false){SupportsDeferredNumericRead=true})
   .Concat(new[]{new PortDescriptor("Src Input Point",DataKind.Point,false){Cardinality=ValueCardinality.Array},new("InputImage",DataKind.Image,true)}).ToArray(),
  new[]{"ModuRunTime","CoordinateOriPointX","CoordinateOriPointY","CoordinateAxisXX","CoordinateAxisXY","CoordinateAxisYX","CoordinateAxisYY"}.Select(n=>new PortDescriptor(n,DataKind.Number,false))
   .Concat(new[]{new PortDescriptor("ModuStatus",DataKind.Integer,false),new("CorCalibMatrix",DataKind.NumberArray,false)}).ToArray(),
  [P("CordinateCreate",ParameterKind.Integer,0,0,1),P("InputWay",ParameterKind.Integer,0,0,2),P("ConfigurationUserData",ParameterKind.Text,"{}"),P(ToolDisplayService.SettingsParameter,ParameterKind.Text,""),P(ToolDisplayObjectStore.ParameterName,ParameterKind.Text,"{}")]){RoiEditor=new([RoiKind.Coordinate],Maximum:1,FullImage:false,Exclusions:false)};
 sealed class State{public readonly object Gate=new();public readonly CoordinateSession Session=new();public Dictionary<string,PortValue>? Inputs,Outputs;public ToolDisplayDiagnostic? Diagnostic;}
 readonly Dictionary<string,State> fallback=[];readonly object gate=new();
 State Get(string node,string directory,ModuleExecutionScope? scope){if(scope is not null)return scope.State(Id,node,()=>new State());lock(gate){string key=Path.GetFullPath(directory)+"|"+node;if(!fallback.TryGetValue(key,out var s))fallback[key]=s=new();return s;}}
 public void Reset(string nodeId){lock(gate)foreach(var key in fallback.Keys.Where(k=>k.EndsWith("|"+nodeId,StringComparison.Ordinal)).ToArray())fallback.Remove(key);}
 public IModule CreateRuntime()=>new Runtime(this);
 public System.Windows.Forms.Form CreateEditor(ToolEditorContext c)=>throw new InvalidOperationException("Use WPF editor factory.");
 public System.Windows.Window CreateWpfEditor(ToolEditorContext c)=>new ToolConfigurationModelWindow(c,CreateConfiguration(c));
 static ToolNumericInputReadResult Read(ModuleExecutionContext c,string name,int index){
  if(c.Scope?.Service<IToolNumericInputReader>() is {} reader)return reader.Read(c,name,index);
  if(c.ReadNumericInput?.Invoke(name,index) is {} r)return new(r.ErrorCode,r.Count,r.Value);
  if(c.Inputs.TryGetValue(name,out var input)){
   var values=input.Kind==DataKind.NumberArray?input.As<IReadOnlyList<double>>():input.Kind==DataKind.Number?input.Elements().Select(v=>v.As<double>()).ToArray():Array.Empty<double>();
   if(index>=0&&index<values.Count)return new(0,values.Count,values[index]);
   return new(0xe0000314,0,0);
  }
  // Independent materialized POINT compatibility. Normal configuration aliases
  // use authoritative leaves; neither an explicit leaf error nor a scope/deferred
  // result may be replaced with this parent payload.
  if((name is "SrcPointX" or "SrcPointY")&&c.Inputs.TryGetValue("Src Input Point",out var point)&&point.Kind==DataKind.Point){
   var points=point.Elements().Select(p=>p.As<Point2>()).ToArray();
   if(points.Length==0)return new(0xe0000314,0,0);
   if(index<0||index>=points.Length)return new(0xe0000014,0,0);
   return new(0,points.Length,name=="SrcPointX"?points[index].X:points[index].Y);
  }
  return new(0xe0000314,0,0);
 }
 static CoordinateSession.NumericRead ReadFloat(ModuleExecutionContext c,string name){var r=Read(c,name,0);return new(r.ErrorCode,r.Count,(float)r.Value,r.ErrorCode==0&&r.Count>0);}
 // First helper discards GetFloat status but uses Count; second helper re-reads
 // index0 and gates status/count. Per-index callback below is the managed batch
 // adapter boundary, not a claim of one native BatchGetFloat invocation.
 static (uint ErrorCode,float[] Values) ReadMatrix(ModuleExecutionContext c){
  var probe=Read(c,"CalibMatrix",0);int count=probe.Count;if(count is <1 or >9)return(0xe0000306,[]);
  var first=Read(c,"CalibMatrix",0);if(first.ErrorCode!=0)return(first.ErrorCode,[]);if(first.Count<count)return(0xe0000001,[]);
  if(count>1&&c.Scope?.Service<IToolNumericInputReader>() is not null)throw new NotSupportedException("Dynamic scope matrix batch timing is unsupported by the per-index reader contract.");
  if(count>1&&c.Inputs.TryGetValue("CalibMatrix",out var immutable)&&immutable.Kind==DataKind.NumberArray){var snapshot=immutable.As<IReadOnlyList<double>>().Take(count).Select(v=>(float)v).ToArray();return snapshot.Length==count?(0,snapshot):(0xe0000001,[]);}
  var values=new float[count];values[0]=first.Count>0?(float)first.Value:0;
  for(int i=1;i<count;i++){var r=Read(c,"CalibMatrix",i);if(r.ErrorCode!=0)return(r.ErrorCode,[]);if(r.Count<=i)return(0xe0000001,[]);values[i]=(float)r.Value;}
  return(0,values);
 }
 sealed class Runtime(CoordinateModulePlugin owner):IModule{
  public ModuleDescriptor Descriptor=>owner.Descriptor;
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken token){
   token.ThrowIfCancellationRequested();var watch=Stopwatch.StartNew();var state=owner.Get(c.NodeId,c.BaseDirectory,c.Scope);
   var output=new Dictionary<string,PortValue>();var nonFinite=new List<string>();uint inner=0;string? unsupported=null,unsupportedCode=null;uint outer=0;
   void Number(string name,double value){if(double.IsFinite(value))output[name]=PortValue.Number(value);else nonFinite.Add(name);}
   lock(state.Gate){
    var image=c.Inputs.GetValueOrDefault("InputImage")?.As<IImageData>();
    int format=image is null?0:(int)image.Format;
    if(image is null)outer=0xe0000510;else if(image.Width<1||image.Height<1)outer=0xe0000001;
    else if(format is not (0x01000000 or 0x01080001 or 0x02180014 or 0x02180021))outer=0xe0000510;
    else if(image.Pixels.Length!=(long)image.Width*image.Height*(format is 0x02180014 or 0x02180021?3:1))outer=0xe0000314;
    else{
     var roi=(c.RoiGroups.GetValueOrDefault("RoiType")??c.Rois).FirstOrDefault(r=>r.Kind==RoiKind.Coordinate);
     if(roi is not null)roi=RoiGeometry.ToPixels(roi,image.Width,image.Height);
     // Runtime pixel conversion must match the persisted geometry coordinate space.
     (float X,float Y,float Angle)? point=roi is null?null:((float)roi.CenterX,(float)roi.CenterY,(float)roi.AngleDegrees);
     try{inner=state.Session.Process(c.Integer("CordinateCreate"),c.Integer("InputWay"),point,n=>ReadFloat(c,n),()=>ReadMatrix(c));}
     catch(NotSupportedException e){unsupported=e.Message;unsupportedCode="UnsupportedDynamicMatrixBatch";}
     output["ModuStatus"]=PortValue.Integer(state.Session.Status);
     Number("CoordinateOriPointX",state.Session.X);Number("CoordinateOriPointY",state.Session.Y);
     if(unsupported is null&&state.Session.UnsupportedDiagnostic is {} unknown){unsupported=unknown;unsupportedCode="NativeUninitializedMatrix";}
     if(unsupported is null){
      if(state.Session.Matrix.All(float.IsFinite))output["CorCalibMatrix"]=PortValue.Numbers(state.Session.Matrix.Select(v=>(double)v));else nonFinite.Add("CorCalibMatrix");var axes=state.Session.Axes();
      string[] names=["CoordinateAxisXX","CoordinateAxisXY","CoordinateAxisYX","CoordinateAxisYY"];for(int i=0;i<4;i++)Number(names[i],axes[i]);
     }
    }
    if(nonFinite.Count>0)unsupported="Native non-finite state cannot be represented by finite PortValue: "+string.Join(", ",nonFinite);
    output["ModuRunTime"]=PortValue.Number(watch.Elapsed.TotalMilliseconds);
    state.Inputs=c.Inputs.ToDictionary(p=>p.Key,p=>p.Value);state.Outputs=output;
    state.Diagnostic=unsupported is not null?new(0,unsupported):inner!=0?new(inner,"Original inner error; outer publishes retained state."):outer!=0?new(outer,"Coordinate image prelude failed."):null;
   }
   return ValueTask.FromResult(new ModuleResult(outer!=0||unsupported is not null?ModuleStatus.Failed:ModuleStatus.Success,output,nonFinite.Count>0?"NativeNonFiniteResult":unsupportedCode??(outer!=0?"CoordinateInput":null),unsupported){NativeErrorCode=outer});
  }
 }
 // Original result view defaults to the related input IMAGE when this module has
 // no owned display-image object. Explicit host adapter, not an invented InImage IO.
 public ToolDisplayDefinition DisplayDefinition{get;}=new("Module Status:{0}",["ModuStatus"],["ModuRunTime","ModuStatus","CorCalibMatrix"],"InputImage"){TextVisible=false};
 public static IReadOnlyList<ToolDisplayObjectDefinition> DisplayObjects{get;}=ToolDisplayObjectDefinition.Parse(Resource("CoordinateModuleDisplay.xml"));
 public ToolDisplayFrame? LastDisplay(NodeDocument node,string directory,ModuleExecutionScope? scope){
  var state=Get(node.Id,directory,scope);lock(state.Gate){
   if(state.Outputs is not {} outputs)return null;
   // Omitted unsafe components are not zero-valued feature facts. The common
   // graphic builder's missing-feature fallback must not invent an axis here.
   bool complete=new[]{"CoordinateOriPointX","CoordinateOriPointY","CoordinateAxisXX","CoordinateAxisXY","CoordinateAxisYX","CoordinateAxisYY"}.All(outputs.ContainsKey);
   return ToolDisplayService.Build(DisplayDefinition,ToolDisplayService.Read(node.Parameters,DisplayDefinition),state.Inputs??[],outputs,diagnostic:state.Diagnostic) with{Graphics=complete?ToolDisplayGraphics.Build(DisplayObjects,node.Parameters,state.Inputs??[],outputs):[]};
  }
 }
 static string Resource(string name){using var stream=typeof(CoordinateModulePlugin).Assembly.GetManifestResourceStream("VisionPlugins.CoordinateModule.Resources."+name)??throw new InvalidDataException(name);using var reader=new StreamReader(stream);return reader.ReadToEnd();}
}
