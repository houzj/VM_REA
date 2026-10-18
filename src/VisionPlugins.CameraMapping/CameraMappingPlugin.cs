using System.Diagnostics;
using System.IO;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.CameraMapping;
public sealed partial class CameraMappingPlugin:IVisionToolPlugin,IToolConfigurationProvider,IToolDisplayProvider,IToolPointInputAliasProvider {
 public const string Id="vision.camera-mapping";
 public static string PointInputName(string side,int index)=>side+(index<=2?"PointInput":"Input")+index;
 public IReadOnlyList<ToolPointInputAlias> PointInputAliases{get;}=Enumerable.Range(1,8).SelectMany(i=>new[]{"Target","Object"}.Select(s=>new ToolPointInputAlias(PointInputName(s,i),s+"PointX"+i,s+"PointY"+i))).ToArray();
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","相机映射","calibration","catalog/IMVSCameraMapModuImageEnable.png");
 public static IReadOnlyList<ToolDisplayObjectDefinition> DisplayObjectDefinitions{get;}=LoadDisplayObjects();
 static IReadOnlyList<ToolDisplayObjectDefinition> LoadDisplayObjects(){using var stream=typeof(CameraMappingPlugin).Assembly.GetManifestResourceStream("VisionPlugins.CameraMapping.Resources.CameraMappingDisplay.xml")??throw new InvalidDataException("Missing camera mapping display declaration.");using var reader=new StreamReader(stream);return ToolDisplayObjectDefinition.Parse(reader.ReadToEnd());}
 public ToolDisplayDefinition DisplayDefinition{get;}=new("CalibStatus:{0}, xOrientRate:{1}, yOrientRate:{2}",["CalibStatus","ScaleX","ScaleY"],["CalibStatus","ScaleX","ScaleY","TranslateX","TranslateY","Rotate","Scale","Skew","Aspect"]);
 public ModuleDescriptor Descriptor{get;}=new(Id,"1.0.0","相机映射",Inputs(),new[]{new PortDescriptor("CalibStatus",DataKind.Integer,false),new("ModuRunTime",DataKind.Number,false)}.Concat(new[]{"ScaleX","ScaleY","TranslateX","TranslateY","Rotate","Scale","Skew","Aspect"}.Select(x=>new PortDescriptor(x,DataKind.Number,false))).ToArray(),Parameters());
 static PortDescriptor[] Inputs()=>Enumerable.Range(1,8).SelectMany(i=>new[]{"Target","Object"}.SelectMany(s=>new[]{new PortDescriptor(PointInputName(s,i),DataKind.Point,false),new(s+"PointX"+i,DataKind.NumberArray,false,AcceptInteger:true),new(s+"PointY"+i,DataKind.NumberArray,false,AcceptInteger:true)})).ToArray();
 static ParameterDescriptor[] Parameters()=>new[]{new ParameterDescriptor("HomoFreedom",ParameterKind.Integer,true,J(1),1,3),new("WeightFun",ParameterKind.Integer,true,J(3),1,3),new("WeightFactor",ParameterKind.Integer,true,J(20),1,200),new("CalibPathName",ParameterKind.Text,true,J("")),new("RefreshFileEnable",ParameterKind.Boolean,true,J(false)),new("ConfigurationUserData",ParameterKind.Text,true,J("{}")),new(ToolDisplayService.SettingsParameter,ParameterKind.Text,true,J("")),new(ToolConfigurationDisplaySubscriptions.ParameterName,ParameterKind.Text,true,J("{}")),new(ToolDisplayObjectStore.ParameterName,ParameterKind.Text,true,J("{}"))}.Concat(Enumerable.Range(1,8).Select(i=>new ParameterDescriptor("Data"+i,ParameterKind.Text,true,J("Data"+i+(i<=2?"#1":"#0"))))).ToArray();
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 sealed class State {public readonly object Gate=new();public bool CalibrationValid;public string File="";public double[]? Matrix;public CameraMappingPair[]? Points;public IReadOnlyDictionary<string,PortValue>? Inputs,Outputs;}
 readonly Dictionary<string,State> states=[];readonly object stateGate=new();
 State Get(string node,string directory,ModuleExecutionScope? scope){if(scope is not null)return scope.State(Id,node,()=>new State());lock(stateGate){string key=Path.GetFullPath(directory)+"|"+node;if(!states.TryGetValue(key,out var state))states[key]=state=new();return state;}}
 public void Reset(string nodeId){lock(stateGate)foreach(var key in states.Keys.Where(k=>k.EndsWith("|"+nodeId,StringComparison.Ordinal)).ToArray())states.Remove(key);}
 public IModule CreateRuntime()=>new Runtime(this);
 public System.Windows.Forms.Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use the shared configuration provider.");
 public ToolDisplayFrame? LastDisplay(NodeDocument node,string baseDirectory,ModuleExecutionScope? scope){var s=Get(node.Id,baseDirectory,scope);lock(s.Gate)return s.Outputs is null?null:ToolDisplayService.Build(DisplayDefinition,ToolDisplayService.Read(node.Parameters,DisplayDefinition),s.Inputs??new Dictionary<string,PortValue>(),s.Outputs);}
 public void SaveCalibration(ToolEditorContext context,string path){var s=Get(context.Node.Id,context.BaseDirectory,context.Scope);lock(s.Gate){if(!s.CalibrationValid||s.Matrix is null)throw new CameraMappingOperationException(0xe0000018,"尚未成功标定，不能生成标定文件。");var effective=context.ReadEffectiveParameters?.Invoke();JsonElement Parameter(string name)=>effective?.TryGetValue(name,out var current)==true?current:context.Node.Parameters[name];var solved=CameraMappingSolver.Solve(s.Points??[],Parameter("HomoFreedom").GetInt32(),Parameter("WeightFun").GetInt32(),Parameter("WeightFactor").GetInt32());if(!solved.Success)throw new CameraMappingOperationException(solved.ErrorCode,"缓存点重新标定失败。");s.Matrix=solved.Matrix;CameraMappingFile.Write(Path.GetFullPath(path,context.BaseDirectory),s.Matrix);}}
 sealed class Runtime(CameraMappingPlugin owner):IModule {
  public ModuleDescriptor Descriptor=>owner.Descriptor;
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken cancellationToken){cancellationToken.ThrowIfCancellationRequested();var watch=Stopwatch.StartNew();var state=owner.Get(c.NodeId,c.BaseDirectory,c.Scope);lock(state.Gate){state.CalibrationValid=false;state.Inputs=null;state.Outputs=null;try{
   var points=new List<CameraMappingPair>();bool anyActive=false;
   for(int i=1;i<=8;i++){
    if(!c.Parameters["Data"+i].GetString()!.EndsWith("#1",StringComparison.Ordinal))continue;anyActive=true;
    var objects=Read("Object",i);var targets=Read("Target",i);
    if(objects.Count!=targets.Count)return Fail(0xe0000001,"对象点与目标点数量不一致。");
    for(int j=0;j<objects.Count;j++)points.Add(new(objects[j].X,objects[j].Y,targets[j].X,targets[j].Y));
   }
   if(!anyActive)return Fail(0xe00000ff,"没有活动图像点组。");
   // Original GetProcessInput grows its point cache but does not shrink it.
   // ProcessAlg uses the current count; explicit ProcessCalibMatrix uses cache capacity.
   if(state.Points is null||state.Points.Length<points.Count)state.Points=points.ToArray();else points.CopyTo(state.Points);
   if(points.Count==0)return Fail(0xe0000309,"没有可用标定输入。");
   var solved=CameraMappingSolver.Solve(points,c.Integer("HomoFreedom"),c.Integer("WeightFun"),c.Integer("WeightFactor"));if(!solved.Success)return Fail(solved.ErrorCode,"相机映射求解失败。");
   string path=c.Text("CalibPathName");var h=solved.Matrix;var inv=CameraMappingLinearAlgebra.Inverse(h);if(inv is null||inv[8]==0)return Fail(0x1000010f,"映射矩阵不能转换为标定参数。");float normalization=(float)inv[8];inv=inv.Select(v=>(double)((float)v/normalization)).ToArray();state.Matrix=h;state.CalibrationValid=true;if(path.Length>0&&c.Boolean("RefreshFileEnable")){CameraMappingFile.Write(c.ResolvePath(path),h);state.File=c.ResolvePath(path);}
   const float epsilon=1.1920928955078125e-7f;float theta=MathF.Atan2((float)inv[3],(float)inv[0]);float scale=(float)inv[0]/(MathF.Cos(theta)+epsilon);float skew=MathF.Atan2(-(float)inv[1],(float)inv[4])-theta;if(skew< -MathF.PI)skew+=2*MathF.PI;if(skew>MathF.PI)skew-=2*MathF.PI;
   var output=new Dictionary<string,PortValue>{["CalibStatus"]=PortValue.Integer(1),["ScaleX"]=PortValue.Number(solved.RatioX),["ScaleY"]=PortValue.Number(solved.RatioY),["TranslateX"]=PortValue.Number(inv[2]),["TranslateY"]=PortValue.Number(inv[5]),["Rotate"]=PortValue.Number(theta),["Scale"]=PortValue.Number(scale),["Skew"]=PortValue.Number(skew),["Aspect"]=PortValue.Number(-(float)inv[1]/(MathF.Sin(theta+skew)*scale+epsilon)),["ModuRunTime"]=PortValue.Number(watch.Elapsed.TotalMilliseconds)};
   state.Inputs=c.Inputs;state.Outputs=output;return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,output){NativeErrorCode=0});
   IReadOnlyList<Point2> Read(string side,int index){
    var baseline=c.Inputs.TryGetValue(PointInputName(side,index),out var combined)?combined.Elements().Select(p=>p.As<Point2>()).ToArray():null;
    // Independent compatibility boundary while shared native combination aliases are being reconstructed.
    var x=c.Inputs.ContainsKey(side+"PointX"+index)||baseline is null?Numbers(side+"PointX"+index):baseline.Select(p=>p.X).ToArray();
    var y=c.Inputs.ContainsKey(side+"PointY"+index)||baseline is null?Numbers(side+"PointY"+index):baseline.Select(p=>p.Y).ToArray();if(x.Count!=y.Count)throw new InvalidDataException("X/Y 点数量不一致。");return x.Zip(y,(a,b)=>new Point2((float)a,(float)b)).ToArray();}
   IReadOnlyList<double> Numbers(string name)=>c.Inputs.TryGetValue(name,out var p)?p.Kind==DataKind.NumberArray?p.As<IReadOnlyList<double>>():p.Kind==DataKind.Number?[p.As<double>()]:throw new InvalidDataException("点坐标类型错误。"):Array.Empty<double>();
  }catch(Exception e)when(e is InvalidDataException or IOException or UnauthorizedAccessException or ArgumentException or InvalidOperationException){return Fail(0xe0000001,e.Message);}}}
  static ValueTask<ModuleResult> Fail(uint code,string message)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"0x"+code.ToString("x8"),message){NativeErrorCode=code});
 }
 internal static double[]? Inverse(double[] h){double d=h[0]*(h[4]*h[8]-h[5]*h[7])-h[1]*(h[3]*h[8]-h[5]*h[6])+h[2]*(h[3]*h[7]-h[4]*h[6]);if(Math.Abs(d)<1e-12)return null;return new[]{h[4]*h[8]-h[5]*h[7],h[2]*h[7]-h[1]*h[8],h[1]*h[5]-h[2]*h[4],h[5]*h[6]-h[3]*h[8],h[0]*h[8]-h[2]*h[6],h[2]*h[3]-h[0]*h[5],h[3]*h[7]-h[4]*h[6],h[1]*h[6]-h[0]*h[7],h[0]*h[4]-h[1]*h[3]}.Select(x=>(double)(float)(x/d)).ToArray();}
}
