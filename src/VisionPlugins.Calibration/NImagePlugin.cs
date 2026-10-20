using System.Diagnostics;
using System.IO;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.Calibration;

/// <summary>Independent owned-batch N-image tool. Original evidence and numerical limits are
/// recorded per stage; registration does not establish complete native desktop parity.</summary>
public sealed class NImagePlugin:IVisionToolPlugin,IWpfToolEditorFactory,IToolConfigurationProvider,IToolDisplayProvider,IToolPointInputAliasProvider{
 public IReadOnlyList<ToolPointInputAlias> PointInputAliases{get;}=[new("PhyPoint","MovePhysicalPointX","MovePhysicalPointY"),new("Image Point","PicPointX","PicPointY"),new("Physical Point","PhysicalPointX","PhysicalPointY")];
 public const string Id=NImageConfigurationProvider.ModuleId;
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","N图像标定","calibration","catalog/IMVSNImageCalibModuImageEnable.png");
 public static readonly string[] IntegerOutputs=["ModuStatus","CalibStatus","CalibErrStatus","RotDirectState","CornerPairsNum","TransErrMaxPtsNum","RotErrMaxPtsNum","TransMotionErrPicIndex","RotMotionErrPicIndex","CurNum","TotalNum"];
 public static readonly string[] NumberOutputs=["TransError","TransEstMax","TransWorldError","TransErrWorldMax","RotError","RotErrMax","RotWorldError","RotErrWorldMax","TransMotionErrAvgTrans","TransMotionErrAvgScale","TransMotionErrAvgRotate","TransMotionErrMaxTrans","TransMotionErrMaxScale","TransMotionErrMaxRotate","RotMotionErrAvg","RotMotionErrMax","RotateCenterPointX","RotateCenterPointY","DirectionXVectorX","DirectionXVectorY","DirectionYVectorX","DirectionYVectorY","MoveDirec","ModuRunTime"];
 public ModuleDescriptor Descriptor{get;}=new(Id,"1.0.0","N图像标定",new[]{new PortDescriptor("InputImage",DataKind.Image),new("PhyPoint",DataKind.Point,false),new("Image Point",DataKind.Point,false),new("Physical Point",DataKind.Point,false)}.Concat(new[]{"MovePhysicalPointX","MovePhysicalPointY","RotateAngle","PicPointX","PicPointY","PhysicalPointX","PhysicalPointY",ToolDisplayService.PositionXInput,ToolDisplayService.PositionYInput}.Select(n=>new PortDescriptor(n,DataKind.Number,false,AcceptInteger:true){SupportsDeferredNumericRead=n!=ToolDisplayService.PositionXInput&&n!=ToolDisplayService.PositionYInput})).ToArray(),IntegerOutputs.Select(n=>new PortDescriptor(n,DataKind.Integer,false)).Concat(NumberOutputs.Select(n=>new PortDescriptor(n,DataKind.Number,false))).Concat(new[]{new PortDescriptor("OutImagePointX",DataKind.NumberArray,false),new("OutImagePointY",DataKind.NumberArray,false),new("CalibData",DataKind.Calibration,false)}).ToArray(),NImageParameters.Definitions);
 public ToolDisplayDefinition DisplayDefinition=>NImageConfigurationProvider.DisplayDefinition;
 public static IReadOnlyList<ToolDisplayObjectDefinition> DisplayObjectDefinitions{get;}=LoadDisplay();
 static IReadOnlyList<ToolDisplayObjectDefinition> LoadDisplay(){using var stream=typeof(NImagePlugin).Assembly.GetManifestResourceStream("VisionPlugins.Calibration.Resources.NImageDisplay.xml")??throw new InvalidDataException("Missing source NImage display model.");using var reader=new StreamReader(stream);return ToolDisplayObjectDefinition.Parse(reader.ReadToEnd());}
 sealed class State:IDisposable{internal readonly object Gate=new();internal readonly NImageRuntime Runtime=new();internal NImageRecognitionBatch? CachedBatch;internal NImageBatchSolveResult? CachedSolve;internal NImageOutputMetricsResult? CachedMetrics;internal IReadOnlyList<double>? CachedFixedMatrix;internal NImageRecognitionBatch? Batch;internal NImageBatchSolveResult? Solve;internal NImageOutputMetricsResult? Metrics;internal IReadOnlyList<double>? Matrix;internal Dictionary<string,PortValue>? Inputs,Outputs;internal ToolDisplayDiagnostic? Diagnostic;public void Dispose(){lock(Gate){Runtime.Clear();CachedBatch=null;CachedSolve=null;CachedMetrics=null;CachedFixedMatrix=null;Batch=null;Solve=null;Metrics=null;Matrix=null;Inputs=null;Outputs=null;}}}
 readonly object gate=new();readonly Dictionary<string,State> fallback=new();
 State GetState(string node,string directory,ModuleExecutionScope? scope){if(scope is not null)return scope.State(Id,node,()=>new State());string key=Path.GetFullPath(directory)+"|"+node;lock(gate){if(!fallback.TryGetValue(key,out var state))fallback.Add(key,state=new());return state;}}
 public void Reset(string nodeId){lock(gate)foreach(var key in fallback.Keys.Where(k=>k.EndsWith("|"+nodeId,StringComparison.Ordinal)).ToArray()){fallback[key].Dispose();fallback.Remove(key);}}
 public IModule CreateRuntime()=>new Runtime(this);
 public Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use shared WPF configuration host.");
 public System.Windows.Window CreateWpfEditor(ToolEditorContext context)=>new ToolConfigurationModelWindow(context,CreateConfiguration(context));
 public ToolConfigurationSession CreateConfiguration(ToolEditorContext context)=>new NImageConfigurationProvider(Clear,Export).CreateConfiguration(context);
 public void Clear(ToolEditorContext context){var state=GetState(context.Node.Id,context.BaseDirectory,context.Scope);lock(state.Gate){state.Runtime.Clear();state.Batch=null;state.Solve=null;state.Metrics=null;state.Matrix=null;state.Outputs=null;state.Diagnostic=null;}context.Scope?.RequestRefresh(context.Node.Id);}
 public void Export(ToolEditorContext context,string path){var state=GetState(context.Node.Id,context.BaseDirectory,context.Scope);lock(state.Gate){if(!path.EndsWith(".xml",StringComparison.Ordinal))throw new NImageExportException(0xe0000204,"标定文件后缀必须为.xml。");
int trans=context.Node.Parameters["CalibPointTotalNum"].GetInt32(),rot=context.Node.Parameters["RotPointTotalNum"].GetInt32();var sequence=state.Runtime.Inspect(trans+rot);
if(sequence.Current<trans+rot)throw new NImageExportException(0xe0000517,"当前采集数量不足。");
if(state.CachedBatch is null||state.CachedSolve is null||state.CachedMetrics is null)throw new NImageExportException(0x80100011,"尚无SDK标定结果。");
var matrix=context.Node.Parameters["HomoFixEnable"].GetBoolean()?state.CachedFixedMatrix:state.CachedSolve.Matrix;
if(matrix is null)throw new NImageExportException(0xe0000001,"尚无矩阵修正结果。");
NImageFile.WriteModule(Path.GetFullPath(path,context.BaseDirectory),NImageCalibrationExport.Build(state.CachedBatch,state.CachedSolve,state.CachedMetrics,matrix,context.Node.Parameters["CareraMove"].GetBoolean(),currentSequence:sequence,translationCount:trans,rotationCount:rot));}}
 public ToolDisplayFrame? LastDisplay(NodeDocument node,string baseDirectory,ModuleExecutionScope? scope){var state=GetState(node.Id,baseDirectory,scope);lock(state.Gate){if(state.Outputs is null)return null;var inputs=state.Inputs??[];var frame=ToolDisplayService.Build(DisplayDefinition,ToolDisplayService.Read(node.Parameters,DisplayDefinition),inputs,state.Outputs,diagnostic:state.Diagnostic);return frame with{Graphics=ToolDisplayGraphics.Build(DisplayObjectDefinitions,node.Parameters,inputs,state.Outputs),ImageLayers=ToolDisplayImageLayers.Build(DisplayObjectDefinitions,node.Parameters,inputs,state.Outputs),ImageVisible=false};}}
 public NImageSequenceSnapshot Inspect(ToolEditorContext context){var state=GetState(context.Node.Id,context.BaseDirectory,context.Scope);lock(state.Gate)return state.Runtime.Inspect(context.Node.Parameters["CalibPointTotalNum"].GetInt32()+context.Node.Parameters["RotPointTotalNum"].GetInt32());}
 // Original Process reads motion index0 unconditionally; missing Count selects automatic motion.
 static ToolNumericInputReadResult ReadNumber(ModuleExecutionContext context,string name,string? combined=null,bool y=false){
  if(context.Scope?.Service<IToolNumericInputReader>() is {} reader)return reader.Read(context,name,0);
  if(context.ReadNumericInput?.Invoke(name,0) is {} read)return new(read.ErrorCode,read.Count,read.Value);
  // Legacy combined-point materialization is a managed compatibility bridge.
  if(combined is not null&&context.Inputs.TryGetValue(combined,out var point)){
   var values=point.Elements().ToArray();if(values.Length>0&&values[0].Kind==DataKind.Point){var p=values[0].As<Point2>();return new(0,values.Length,(float)(y?p.Y:p.X));}
  }
  if(context.Inputs.TryGetValue(name,out var input)){
   if(input.Kind==DataKind.NumberArray){var values=input.As<IReadOnlyList<double>>();if(values.Count>0)return new(0,values.Count,(float)values[0]);}
   else if(input.Kind is DataKind.Number or DataKind.Integer){var values=input.Elements().ToArray();if(values.Length>0)return new(0,values.Length,values[0].Kind==DataKind.Integer?(float)values[0].As<long>():(float)values[0].As<double>());}
  }
  return new(0xe0000314,0,0);
 }
 static (double? X,double? Y,double? Angle) ReadMotion(ModuleExecutionContext context){
  var x=ReadNumber(context,"MovePhysicalPointX","PhyPoint");var y=ReadNumber(context,"MovePhysicalPointY","PhyPoint",true);var a=ReadNumber(context,"RotateAngle");
  return x.Count==0||y.Count==0||a.Count==0?(null,null,null):((float)x.Value,(float)y.Value,(float)a.Value);
 }
 static (uint Error,double X,double Y,double WorldX,double WorldY) ReadFix(ModuleExecutionContext context){
  var x=ReadNumber(context,"PicPointX","Image Point");if(x.ErrorCode!=0)return(x.ErrorCode,0,0,0,0);
  var y=ReadNumber(context,"PicPointY","Image Point",true);if(y.ErrorCode!=0)return(y.ErrorCode,0,0,0,0);
  var wx=ReadNumber(context,"PhysicalPointX","Physical Point");if(wx.ErrorCode!=0)return(wx.ErrorCode,0,0,0,0);
  var wy=ReadNumber(context,"PhysicalPointY","Physical Point",true);return(wy.ErrorCode,(float)x.Value,(float)y.Value,(float)wx.Value,(float)wy.Value);
 }
 static Dictionary<string,PortValue> Pending(int current,int total){var values=IntegerOutputs.ToDictionary(n=>n,n=>PortValue.Integer(0));foreach(var name in NumberOutputs)values[name]=PortValue.Number(0);values["ModuStatus"]=PortValue.Integer(1);values["CurNum"]=PortValue.Integer(current);values["TotalNum"]=PortValue.Integer(total);values["OutImagePointX"]=PortValue.Numbers([0]);values["OutImagePointY"]=PortValue.Numbers([0]);return values;}
 sealed class Runtime(NImagePlugin owner):IModule{
  public ModuleDescriptor Descriptor=>owner.Descriptor;
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext context,CancellationToken cancellationToken){
   cancellationToken.ThrowIfCancellationRequested();var watch=Stopwatch.StartNew();var scope=context.Scope;if(scope is not null&&!string.IsNullOrEmpty(context.ProcedureId))scope=scope.ForProcedure(context.ProcedureId);var state=owner.GetState(context.NodeId,context.BaseDirectory,scope);uint error=0;string? message=null;Dictionary<string,PortValue> outputs=[];
   bool queuedFixInputFailure=false;lock(state.Gate){try{
    foreach(var spec in Descriptor.Parameters)if(!context.Parameters.TryGetValue(spec.Name,out var value)||!GraphValidator.ValidParameter(spec,value))throw new ArgumentException("Invalid NImage parameter: "+spec.Name);
    if(!context.Inputs.TryGetValue("InputImage",out var imageValue)||imageValue.Value is not IImageData sourceImage){error=0xe000031c;message="Missing InputImage.";}
    else{
     var image=ImageColorConversion.ToMono8(sourceImage);
      var motion=ReadMotion(context);var parameters=NImageParameters.Sequence(context.Parameters);
      var batch=state.Runtime.Add(image,parameters,context.Integer("CalibBoardType"),context.Integer("ClearRevision"),motion.X,motion.Y,motion.Angle,cancellationToken,context.Integer("FilterStatus"),runRecognition:false);
      bool fix=context.Boolean("HomoFixEnable");(uint Error,double X,double Y,double WorldX,double WorldY) correction=default;
      if(batch.Sequence.Ready&&fix){correction=ReadFix(context);queuedFixInputFailure=correction.Error!=0;}
      if(!queuedFixInputFailure&&batch.Sequence.Ready){state.CachedBatch=null;state.CachedSolve=null;state.CachedMetrics=null;batch=NImageRuntime.RecognizeSnapshot(batch.Sequence,parameters,context.Integer("CalibBoardType"),context.Integer("FilterStatus"),cancellationToken);}
      state.Batch=batch;state.Solve=null;state.Metrics=null;state.Matrix=null;outputs=Pending(batch.Sequence.Current,batch.Sequence.Total);error=batch.ErrorCode;
      if(queuedFixInputFailure){error=correction.Error;message="矩阵修正输入读取失败。";}
      else{
      if(error==0&&batch.Ready){var solved=NImageBatchSolver.Solve(batch,cancellationToken);state.Solve=solved;error=solved.ErrorCode;if(error==0&&solved.Complete){var frames=batch.Sequence.Frames;var metrics=NImageOutputMetrics.Measure(solved,frames.Where(f=>f.Kind==1).Select(f=>new NImagePose(f.PhysicalX,f.PhysicalY,f.Angle)).ToArray(),frames.Where(f=>f.Kind==2).Select(f=>new NImagePose(f.PhysicalX,f.PhysicalY,f.Angle)).ToArray());state.Metrics=metrics;state.CachedBatch=batch;state.CachedSolve=solved;state.CachedMetrics=metrics;if(fix){state.CachedFixedMatrix=null;state.CachedFixedMatrix=NImageMatrixFix.Apply(solved.Matrix,correction.X,correction.Y,correction.WorldX,correction.WorldY);}state.Matrix=fix?state.CachedFixedMatrix!:solved.Matrix;foreach(var pair in metrics.Numbers)outputs[pair.Key]=PortValue.Number(pair.Value);foreach(var pair in metrics.Integers)outputs[pair.Key]=PortValue.Integer(pair.Value);outputs["CalibStatus"]=PortValue.Integer(1);outputs["CalibErrStatus"]=PortValue.Integer(metrics.Status);var tracks=NImageTrackedCorners.Build(batch.Boards);outputs["CornerPairsNum"]=PortValue.Integer(tracks.Count);outputs["OutImagePointX"]=PortValue.Numbers(tracks.Count==0?[]:tracks[0].Pixels.Select(p=>p.X));outputs["OutImagePointY"]=PortValue.Numbers(tracks.Count==0?[]:tracks[0].Pixels.Select(p=>p.Y));outputs["CalibData"]=PortValue.Calibration(new(state.Matrix,Id));
       if(context.Boolean("RefreshFileEnable")&&!string.IsNullOrEmpty(context.Text("CalibPathName")))try{NImageFile.WriteModule(context.ResolvePath(context.Text("CalibPathName")),NImageCalibrationExport.Build(batch,solved,metrics,state.Matrix,context.Boolean("CareraMove")));}catch(Exception failure)when(failure is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException){/* Original ProcessAlg ignores automatic WriteXmlData return. */}
       }}
      }
    }
   }catch(OperationCanceledException){throw;}catch(Exception failure)when(failure is ArgumentException or InvalidOperationException or IOException or OverflowException or NotSupportedException){error=0x10000019;message=failure.Message;}
     if(error!=0){if(!queuedFixInputFailure){outputs.Clear();outputs["ModuStatus"]=PortValue.Integer(0);}state.Solve=null;state.Metrics=null;state.Matrix=null;}
    outputs["ModuRunTime"]=PortValue.Number(watch.Elapsed.TotalMilliseconds);state.Inputs=context.Inputs.ToDictionary(p=>p.Key,p=>p.Value);if(state.Inputs.TryGetValue("InputImage",out var captured))state.Inputs["InImage"]=captured;state.Outputs=new(outputs);state.Diagnostic=error==0?null:new(error,message??"NImage calibration failed.");
   }
   return ValueTask.FromResult(new ModuleResult(error==0?ModuleStatus.Success:ModuleStatus.Failed,outputs,error==0?null:"NImageCalibration",message){NativeErrorCode=error});
  }
 }
}
