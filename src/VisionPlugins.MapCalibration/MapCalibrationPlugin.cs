using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.MapCalibration;

public sealed partial class MapCalibrationPlugin:IVisionToolPlugin,IToolConfigurationProvider,IToolPointInputAliasProvider,IToolDisplayProvider {
 public const string Id="vision.map-calibration";
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","映射标定","calibration","catalog/IMVSMapCalibModuImageEnable.png");
 public IReadOnlyList<ToolPointInputAlias> PointInputAliases{get;}=[new("Physical Point","PhysicalPointX","PhysicalPointY")];
 public ToolDisplayDefinition DisplayDefinition{get;}=new("CurCornersPointNum:{0}ObjCornersPointNum:{1}ProCornersPointNum:{2}kx:{3}ky:{4}MapError:{5}Calib1:{6}Calib2:{7}Scale1:{8}Scale2:{9}",["CurCornersPointNum","ObjCornersPointNum","ProCornersPointNum","XRatio","YRatio","MapError","CalibStatus1","CalibStatus2","PixelScale1","PixelScale2"],["CurCornersPointNum","ObjCornersPointNum","ProCornersPointNum","XRatio","YRatio","MapError","CalibStatus1","CalibStatus2","PixelScale1","PixelScale2"],"InputImage2");
 public ToolDisplayFrame? LastDisplay(NodeDocument node,string baseDirectory,ModuleExecutionScope? scope){if(node.Parameters.TryGetValue("InputType",out var mode)&&mode.GetInt32()!=1)return null;var session=CurrentImages(node.Id,baseDirectory,scope);lock(session){if(session.Result is not {} result||session.Target is null)return null;var inputs=new Dictionary<string,PortValue>{{"InputImage2",PortValue.Image(session.Target)}};inputs["InImage2"]=inputs["InputImage2"];var outputs=Runtime.ImageResult(result).Result.Outputs;var frame=ToolDisplayService.Build(DisplayDefinition,ToolDisplayService.Read(node.Parameters,DisplayDefinition),inputs,outputs);return frame with{Graphics=ToolDisplayGraphics.Build(MapCalibrationMetadata.DisplayObjects,node.Parameters,inputs,outputs),ImageLayers=ToolDisplayImageLayers.Build(MapCalibrationMetadata.DisplayObjects,node.Parameters,inputs,outputs),ImageVisible=false};}}
 public ModuleDescriptor Descriptor{get;}=new(Id,"1.0.0","映射标定",[
  new("InputImage1",DataKind.GrayImage,false),new("InputImage2",DataKind.GrayImage,false),new("InputROI1",DataKind.Region,false),new("InputROI2",DataKind.Region,false),new("RefreshSignal",DataKind.Integer,false){SupportsDeferredIntegerRead=true},new("Physical Point",DataKind.Point,false),new("PhysicalPointX",DataKind.NumberArray,false){SupportsDeferredNumericRead=true},new("PhysicalPointY",DataKind.NumberArray,false){SupportsDeferredNumericRead=true},new("WorldRotateAngle",DataKind.NumberArray,false){SupportsDeferredNumericRead=true},new("Trigger",DataKind.Text,false){SupportsDeferredTextRead=true},new("TeachFlagInput",DataKind.Text,false){SupportsDeferredTextRead=true}
 ],new[]{"ModuStatus","CalibStatus","CurCornersPointNum","ObjCornersPointNum","ProCornersPointNum","CalibStatus1","CalibStatus2"}.Select(n=>new PortDescriptor(n,DataKind.Integer,false)).Append(new("ModuRunTime",DataKind.Number,true)).Concat(new[]{"XRatio","YRatio","MapError","PixelScale1","PixelScale2"}.Select(n=>new PortDescriptor(n,DataKind.Number,false))).Concat(new[]{"Current Corners Point","Object Corners Point","Projection Corners Point"}.Select(n=>new PortDescriptor(n,DataKind.Point,false){Cardinality=ValueCardinality.Array})).Concat(new[]{"CornersCurPointX","CornersCurPointY","CornersObjPointX","CornersObjPointY","CornersProPointX","CornersProPointY","DetectCenterX","DetectCenterY","DetectWidth","DetectHeight","DetectAngle"}.Select(n=>new PortDescriptor(n,DataKind.NumberArray,false))).Append(new("ROI",DataKind.Region,false)).ToArray(),Parameters()){RoiEditor=new([RoiKind.Rectangle],Maximum:1,FullImage:true)};
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 static ParameterDescriptor[] Parameters()=>[
  new("InputType",ParameterKind.Integer,true,J(1),1,2),new("TeachEnable",ParameterKind.Boolean,true,J(false)),new("TeachFlag",ParameterKind.Text,true,J("")),new("CalibPath0",ParameterKind.Text,true,J("")),new("CalibPath1",ParameterKind.Text,true,J("")),new("CalibPathName",ParameterKind.Text,true,J("")),new("RefreshFileEnable",ParameterKind.Boolean,true,J(false)),new("CalibBoardType",ParameterKind.Integer,true,J(7),6,9),new("HomoFreedom",ParameterKind.Integer,true,J(1),1,3),new("WeightFun",ParameterKind.Integer,true,J(3),1,3),new("WeightFactor",ParameterKind.Integer,true,J(20),1,200),new("FilterStatus",ParameterKind.Integer,true,J(1),1,2),new("FilterStatus2",ParameterKind.Integer,true,J(1),1,2),new("ConfigurationUserData",ParameterKind.Text,true,J("{}")),new(ToolDisplayService.SettingsParameter,ParameterKind.Text,true,J("")),new(ToolConfigurationDisplaySubscriptions.ParameterName,ParameterKind.Text,true,J("{}")),new(ToolDisplayObjectStore.ParameterName,ParameterKind.Text,true,J("{}"))
 ];
 readonly ConcurrentDictionary<string,MapCalibrationFileSession> standalone=new();
 sealed class ImageSession {public GrayImage? Source,Target;public bool SourcePending,TargetPending,TeachTriggered;public MapCalibrationPose SourcePose,TargetPose,TeachPose;public MapCalibrationImageResult? Result;}
 // One actual module/node ownership key: shell Retain/RemoveNode must retain and release both caches together.
 sealed class ScopedState(string directory) {public readonly ImageSession Images=new();public readonly MapCalibrationFileSession File=new(directory);}
 readonly ConcurrentDictionary<string,ImageSession> images=new();
 ImageSession CurrentImages(string node,string directory,ModuleExecutionScope? scope)=>scope?.State(Id,node,()=>new ScopedState(directory)).Images??images.GetOrAdd(Path.GetFullPath(directory)+"|"+node,_=>new());
 MapCalibrationFileSession Current(string node,string directory,ModuleExecutionScope? scope)=>scope?.State(Id,node,()=>new ScopedState(directory)).File??standalone.GetOrAdd(Path.GetFullPath(directory)+"|"+node,_=>new(directory));
 public void Reset(string nodeId){foreach(var key in standalone.Keys.Where(k=>k.EndsWith("|"+nodeId,StringComparison.Ordinal)))standalone.TryRemove(key,out _);foreach(var key in images.Keys.Where(k=>k.EndsWith("|"+nodeId,StringComparison.Ordinal)))images.TryRemove(key,out _);}
 public IModule CreateRuntime()=>new Runtime(this);
 public System.Windows.Forms.Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use the shared configuration provider.");
 sealed class Runtime(MapCalibrationPlugin plugin):IModule {
  public ModuleDescriptor Descriptor=>plugin.Descriptor;
  public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken token){
   var timer=Stopwatch.StartNew();
   var result=c.Integer("InputType")==1?await Image(c,token):await File(c,token);
   var outputs=result.Outputs.ToDictionary(p=>p.Key,p=>p.Value);
   outputs["ModuRunTime"]=PortValue.Number(timer.Elapsed.TotalMilliseconds);
   return result with{Outputs=outputs};
  }
  ValueTask<ModuleResult> File(ModuleExecutionContext c,CancellationToken token){
   token.ThrowIfCancellationRequested();var state=plugin.Current(c.NodeId,c.BaseDirectory,c.Scope);lock(state){
    for(int i=0;i<2;i++){var path=c.Text("CalibPath"+i);if(path!=state.InputPath(i)){uint code=state.SetInputPath(i,path);if(code!=0)return Fail(code,"读取输入标定文件失败。");}}
    state.RefreshOutput=c.Boolean("RefreshFileEnable");string output=c.Text("CalibPathName");if(output.Length>0)state.SetOutputPath(output);
    var signal=MapCalibrationInputReader.Integer(c,"RefreshSignal");bool refresh=signal.ErrorCode==0&&signal.Value!=0;
    if(refresh)for(int i=0;i<2;i++){uint code=state.SetInputPath(i,state.InputPath(i));if(code!=0)return Fail(code,"刷新输入标定文件失败。");}
    var result=state.Run();if(result.ErrorCode!=0)return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>{{"ModuStatus",PortValue.Integer(0)},{"CalibStatus",PortValue.Integer(0)}},"0x"+result.ErrorCode.ToString("x8"),"文件映射标定失败。结束状态已写出。"){NativeErrorCode=result.ErrorCode});
    // Source ProcessOutput's file branch publishes statuses only, even though the SDK has ratios.
    return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"ModuStatus",PortValue.Integer(1)},{"CalibStatus",PortValue.Integer(1)}}){NativeErrorCode=0});
   }
  }
  ValueTask<ModuleResult> Image(ModuleExecutionContext c,CancellationToken token){
   var session=plugin.CurrentImages(c.NodeId,c.BaseDirectory,c.Scope);lock(session){
    MapCalibrationSave Save(bool validate=false)=>MapCalibrationImageFiles.Save(c.Text("CalibPathName").Length==0?"":c.ResolvePath(c.Text("CalibPathName")),session.Result,c.Boolean("TeachEnable"),session.TeachTriggered,session.SourcePending,session.TargetPending,session.SourcePose,session.TargetPose,session.TeachPose,validateExtension:validate);
    bool teach=false;
    if(c.Boolean("TeachEnable")){
     var trigger=MapCalibrationInputReader.Text(c,"Trigger");var flag=MapCalibrationInputReader.Text(c,"TeachFlagInput");
     // 21690 checks only the SECOND return, both counts and C-string equality.
     teach=flag.ErrorCode==0&&trigger.Count>=1&&flag.Count>=1&&trigger.Value.Split('\0')[0]==flag.Value.Split('\0')[0];
    }
    if(teach){
     session.TeachPose=new(-999f,-999f,-999f);
     var px=MapCalibrationInputReader.Float(c,"PhysicalPointX");session.TeachPose=session.TeachPose with{X=MapCalibrationInputReader.Slot(px,-999f)};if(px.ErrorCode!=0)return Fail(px.ErrorCode,"示教物理X读取失败。");
     var py=MapCalibrationInputReader.Float(c,"PhysicalPointY");session.TeachPose=session.TeachPose with{Y=MapCalibrationInputReader.Slot(py,-999f)};if(py.ErrorCode!=0)return Fail(py.ErrorCode,"示教物理Y读取失败。");
     var angle=MapCalibrationInputReader.Float(c,"WorldRotateAngle");session.TeachPose=session.TeachPose with{Angle=MapCalibrationInputReader.Slot(angle,-999f)};if(angle.ErrorCode!=0)return Fail(angle.ErrorCode,"示教角度读取失败。");
     session.TeachTriggered=true;
     if(session.SourcePending||session.TargetPending)return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"ModuStatus",PortValue.Integer(1)},{"CalibStatus",PortValue.Integer(0)}}){NativeErrorCode=0});
     var saved=Save();if(saved.ErrorCode!=0)return Fail(saved.ErrorCode,"保存示教标定文件失败。");return ImageResult(session.Result!);
    }
    bool first=c.Inputs.TryGetValue("InputImage1",out var a),second=c.Inputs.TryGetValue("InputImage2",out var b);if(!first&&!second)return Fail(0xe0000314,"没有新到达的输入图像。");
    // Original GetInputImage resets each received frame angle to 0xc479c000 (-999f) before reading optional inputs.
    MapCalibrationPose Pose(){
     var x=MapCalibrationInputReader.Float(c,"PhysicalPointX");var y=MapCalibrationInputReader.Float(c,"PhysicalPointY");var angle=MapCalibrationInputReader.Float(c,"WorldRotateAngle");
     return new(MapCalibrationInputReader.Slot(x,-999999f),MapCalibrationInputReader.Slot(y,-999f),MapCalibrationInputReader.Slot(angle,-999f));
    }
    if(first){session.Source=a!.As<GrayImage>();session.SourcePending=true;session.SourcePose=Pose();}if(second){session.Target=b!.As<GrayImage>();session.TargetPending=true;session.TargetPose=Pose();}
    if(!session.SourcePending||!session.TargetPending)return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,new Dictionary<string,PortValue>{{"ModuStatus",PortValue.Integer(1)},{"CalibStatus",PortValue.Integer(0)}}){NativeErrorCode=0});
    session.SourcePending=session.TargetPending=false;
    IReadOnlyList<RoiDocument> Group(int side)=>c.RoiGroups.GetValueOrDefault("RoiType_"+side)??(side==0?c.Rois:Array.Empty<RoiDocument>());
    IReadOnlyList<RoiDocument> rois1,rois2;
    try {
     IReadOnlyList<RoiDocument> Region(int side){
      RoiRegionValue? region=null;
      if(c.Inputs.TryGetValue("InputROI"+(side+1),out var value)){
       if(value.Value is not RoiRegionValue supplied)throw new ArgumentException("UnsupportedRegion: Region载荷无效。");
       region=supplied;
      }
      return MapCalibrationRegionInput.Resolve(Group(side),region);
     }
     rois1=Region(0);rois2=Region(1);
    }catch(ArgumentException error){return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>(),error.Message.StartsWith("RegionConfigurationConflict:",StringComparison.Ordinal)?"RegionConfigurationConflict":"UnsupportedRegion",error.Message));}
    var result=MapCalibrationImageSolver.Solve(session.Source!,session.Target!,c.Integer("CalibBoardType"),c.Integer("HomoFreedom"),c.Integer("WeightFun"),(float)c.Integer("WeightFactor")*.1f,c.Integer("FilterStatus"),c.Integer("FilterStatus2"),token,rois1.FirstOrDefault(),rois2.FirstOrDefault());session.Result=result.ErrorCode==0?result:null;
    if(result.ErrorCode==0&&c.Boolean("RefreshFileEnable")&&c.Text("CalibPathName").Length>0&&(!c.Boolean("TeachEnable")||session.TeachTriggered))Save();return ImageResult(result);
   }
  }
  internal static ValueTask<ModuleResult> ImageResult(MapCalibrationImageResult result){
    var output=new Dictionary<string,PortValue>{{"ModuStatus",PortValue.Integer(result.ErrorCode==0?1:0)},{"CalibStatus",PortValue.Integer(result.ErrorCode==0?1:0)}};
    if(result.ErrorCode!=0)return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,output,"0x"+result.ErrorCode.ToString("x8"),"图像映射求解失败；共享检测/IRLS的未闭合边界见工具证据。"){NativeErrorCode=result.ErrorCode});
    foreach(var p in new Dictionary<string,float>{{"XRatio",result.XRatio},{"YRatio",result.YRatio},{"MapError",result.MapError},{"PixelScale1",result.PixelScale1},{"PixelScale2",result.PixelScale2}})output[p.Key]=PortValue.Number(p.Value);
    output["CalibStatus1"]=PortValue.Integer(1);output["CalibStatus2"]=PortValue.Integer(1);
    if(result.Boxes.Count>0){Func<MapCalibrationOutputBox,float>[] fields=[b=>b.X,b=>b.Y,b=>b.Width,b=>b.Height,b=>b.Angle];string[] names=["DetectCenterX","DetectCenterY","DetectWidth","DetectHeight","DetectAngle"];for(int i=0;i<fields.Length;i++)output[names[i]]=PortValue.Numbers(result.Boxes.Select(b=>(double)fields[i](b)));output["ROI"]=PortValue.Region(result.Boxes.Select((b,i)=>(Box:b,Index:i)).Where(p=>p.Box.Width>0&&p.Box.Height>0).Select(p=>new RoiDocument("result-"+p.Index,RoiKind.Rectangle,p.Box.X,p.Box.Y,p.Box.Width,p.Box.Height,AngleDegrees:p.Box.Angle)));}
    void Points(string root,string count,string pair,IReadOnlyList<MapCalibrationImagePoint> points){output[count]=PortValue.Integer(points.Count);output[root+"X"]=PortValue.Numbers(points.Select(p=>(double)p.X));output[root+"Y"]=PortValue.Numbers(points.Select(p=>(double)p.Y));output[pair]=PortValue.PointArray(points.Select(p=>new Point2(p.X,p.Y)));}
    Points("CornersCurPoint","CurCornersPointNum","Current Corners Point",result.Source);Points("CornersObjPoint","ObjCornersPointNum","Object Corners Point",result.Target);Points("CornersProPoint","ProCornersPointNum","Projection Corners Point",result.Projection);
    return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,output){NativeErrorCode=0});
  }
  static ValueTask<ModuleResult> Fail(uint code,string message)=>ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"0x"+code.ToString("x8"),message){NativeErrorCode=code});
 }
}




