using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
using VisionIO;
namespace VisionPlugins.ImageSource;
public sealed class ImageSourcePlugin:IVisionToolPlugin,IToolImageListProvider,IWpfToolEditorFactory {
 public ToolManifest Manifest{get;}=new("vision.image-source","1.0.0","图像源","collection","tool-image-file.png");
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 static ParameterDescriptor P<T>(string name,ParameterKind kind,T value,double? min=null,double? max=null)=>new(name,kind,true,J(value),min,max);
 public ModuleDescriptor Descriptor{get;}=new("vision.image-source","1.0.0","图像源",[],[new("Image",DataKind.GrayImage),new("Width",DataKind.Integer),new("Height",DataKind.Integer),new("PixelFormat",DataKind.Integer),new("SNCode",DataKind.Integer),new("CurImageName",DataKind.Text),new("CurImagePath",DataKind.Text),new("ModuStatus",DataKind.Integer),new("ModuRunTime",DataKind.Number),new("ImageMono8",DataKind.GrayImage,false),new("WidthMono8",DataKind.Integer,false),new("HeightMono8",DataKind.Integer,false),new("PixelFormatMono8",DataKind.Integer,false)],[
 P("ImageSourceType",ParameterKind.Integer,1,1,3),P("PixelFormat",ParameterKind.Integer,1,1,2),P("SolSaveImageData",ParameterKind.Boolean,false),P("ShowImageName",ParameterKind.Boolean,true),P("InitialSN",ParameterKind.Integer,1,0,100000),P("PathCache",ParameterKind.Integer,0,0,5),P("StitchEnable",ParameterKind.Boolean,false),P("AutoPlay",ParameterKind.Boolean,true),P("AutoStop",ParameterKind.Boolean,false),P("TriggerStringEnable",ParameterKind.Boolean,false),P("IsSubscribeFolderMode",ParameterKind.Boolean,false),P("SubscribeFolderPath",ParameterKind.Text,""),P("RefreshImage",ParameterKind.Boolean,true),P("TriggerProcessRun",ParameterKind.Boolean,false),P("CameraID",ParameterKind.Integer,-1),P("CurrentImagePath",ParameterKind.Text,""),P("ImagePaths",ParameterKind.Text,"[]"),P("Interval",ParameterKind.Integer,0,0,1000),P("ExecuteType",ParameterKind.Integer,1,1,2),P("SelectedPaths",ParameterKind.Text,"[]"),P("StitchStartHeight",ParameterKind.Integer,0,0,20000),P("StitchHeight",ParameterKind.Integer,2000,0,20000),P("ClearTrigger",ParameterKind.Boolean,false),P("OutMono8",ParameterKind.Boolean,false),P("SetCameraParamWaitTime",ParameterKind.Integer,0,0)]);

 readonly Dictionary<(string Directory,string NodeId),State> states=new();
 readonly object gate=new();
 sealed class State {
  public readonly SemaphoreSlim Execution=new(1,1);
  public readonly Dictionary<string,VisionCore.GrayImage> Cache=new(StringComparer.Ordinal);
  public string[] Paths=[];
  public string RequestedPath="";
  public int Index=-1;
  public int InitialSN=-1;
  public int Sequence;
  public int StitchWidth;
  public byte[] StitchBuffer=[];
 }

 internal static string[] ReadPaths(string json) {
  var paths=JsonSerializer.Deserialize<string[]>(json)??[];
  if(paths.Any(string.IsNullOrWhiteSpace))throw new InvalidDataException("图像列表包含空路径");
  // AddImageCheck compares literal path strings and ignores duplicate entries.
  return paths.Distinct(StringComparer.Ordinal).ToArray();
 }
 public IReadOnlyList<ToolImageEntry> GetImages(NodeDocument node) {
  var paths=ReadPaths(node.Parameters["ImagePaths"].GetString()??"[]");
  return paths.Select(p=>new ToolImageEntry(p,p==node.Parameters["CurrentImagePath"].GetString())).ToArray();
 }
 public IModule CreateRuntime()=>new Runtime(this);
 public Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use WPF editor factory");
 public System.Windows.Window CreateWpfEditor(ToolEditorContext context)=>new ImageSourceWindow(context);
 public void Reset(string nodeId) {
  lock(gate)foreach(var key in states.Keys.Where(k=>k.NodeId==nodeId).ToArray())states.Remove(key);
 }
 State GetState(ModuleExecutionContext context) {
  // Separate solutions until the host supplies the session identity requested in IS-002.
  var key=(Path.GetFullPath(context.BaseDirectory),context.NodeId);
  lock(gate) {
   if(!states.TryGetValue(key,out var state))states.Add(key,state=new());
   return state;
  }
 }

 sealed class Runtime(ImageSourcePlugin owner):IModule {
  public ModuleDescriptor Descriptor=>owner.Descriptor;
  public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken token) {
   token.ThrowIfCancellationRequested();
   State? state=null;
   bool entered=false;
   try {
    foreach(var descriptor in Descriptor.Parameters) {
     if(!c.Parameters.TryGetValue(descriptor.Name,out var value)||!GraphValidator.ValidParameter(descriptor,value))
      return Failed("InvalidParameter","无效参数: "+descriptor.Name);
    }
    if(c.Integer("ImageSourceType")!=1)return Failed("InputProviderUnavailable","相机/SDK 输入提供器尚未接入（IS-002）");
    if(c.Integer("PixelFormat")!=1)return Failed("PixelFormatUnavailable","RGB24 输出契约尚未接入（IS-001）");
    if(c.Boolean("AutoPlay")&&c.Boolean("AutoStop"))return Failed("ProcedureStopUnavailable","末张停止需要所属流程停止服务（IS-003）");
    if(c.Boolean("IsSubscribeFolderMode"))return Failed("FolderWatchUnavailable","目录监视与更新生命周期尚未接入（IS-004）");
    if(c.Boolean("StitchEnable")&&c.Integer("StitchHeight")>0&&c.Boolean("ClearTrigger"))return Failed("StitchTriggerUnavailable","拼接触发清空需要整数订阅服务（IS-003）");
    if(c.Boolean("SolSaveImageData"))return Failed("ModelEmbeddingUnavailable","方案嵌入图像尚未实现（IS-003）");
    if(c.Boolean("TriggerStringEnable")||c.Boolean("TriggerProcessRun"))return Failed("TriggerUnavailable","触发订阅与流程控制尚未接入（IS-003）");

    var paths=ReadPaths(c.Text("ImagePaths"));
    if(c.Integer("ExecuteType")==2) {
     var selected=ReadPaths(c.Text("SelectedPaths")).ToHashSet(StringComparer.Ordinal);
     paths=paths.Where(selected.Contains).ToArray();
    }
    // CurrentImagePath selects a list entry; it is not the native useLocalPic one-shot API.
    if(paths.Length==0)return Failed("NoImage","未配置可执行图像");
    state=owner.GetState(c);
    await state.Execution.WaitAsync(token);
    entered=true;
    if(state.InitialSN!=c.Integer("InitialSN")) {
     state.InitialSN=c.Integer("InitialSN");
     state.Sequence=state.InitialSN;
    }
    if(!state.Paths.SequenceEqual(paths,StringComparer.Ordinal)) {
     string? previous=state.Index>=0&&state.Index<state.Paths.Length?state.Paths[state.Index]:null;
     state.Index=previous is null?-1:Array.IndexOf(paths,previous);
     state.Paths=paths;
    }
    string current=c.Text("CurrentImagePath");
    if(current!=state.RequestedPath) {
     int requested=Array.IndexOf(paths,current);
     if(requested>=0)state.Index=requested;
     state.RequestedPath=current;
    }
    // Process calls FUN_18000b4c0 before decoding. A read failure still consumes this position.
    if(c.Boolean("AutoPlay")||state.Index<0)state.Index=(state.Index+1)%paths.Length;
    string selectedPath=paths[state.Index];
    await Task.Delay(c.Integer("Interval"),token);
    var clock=System.Diagnostics.Stopwatch.StartNew(); // Native interval is excluded from module runtime.
    if(!state.Cache.TryGetValue(selectedPath,out var image)) {
     using var stream=File.OpenRead(c.ResolvePath(selectedPath));
     image=BmpReader.Read(stream);
     token.ThrowIfCancellationRequested();
     // The native cache admits the first N paths and retains hits; it is not an LRU cache.
     if(state.Cache.Count<c.Integer("PathCache"))state.Cache.Add(selectedPath,image);
    }
    token.ThrowIfCancellationRequested();
    if(c.Boolean("StitchEnable")&&c.Integer("StitchHeight")>0) {
     int height=c.Integer("StitchHeight"),start=c.Integer("StitchStartHeight");
     // StitchImageOutput rejects a changed width before touching its retained buffer.
     if((state.StitchWidth!=0&&state.StitchWidth!=image.Width)||(long)start+height>image.Height)
      return Failed("StitchGeometryInvalid","拼接起始高度与高度超出图像，或拼接图像宽度发生变化（原厂 0xe0000502）");
     int length=checked(image.Width*(image.Height+height)),prefix=checked(image.Width*height);
     // Native reallocates and zeroes only when the complete output byte count changes.
     if(state.StitchBuffer.Length!=length)state.StitchBuffer=new byte[length];
     image.Pixels.CopyTo(state.StitchBuffer.AsSpan(prefix));
     var stitched=new VisionCore.GrayImage(image.Width,image.Height+height,state.StitchBuffer);
     image.Pixels.Slice(start*image.Width,prefix).CopyTo(state.StitchBuffer);
     state.StitchWidth=image.Width;
     image=stitched;
    }
    state.Sequence=unchecked(state.Sequence+1);
    var outputs=new Dictionary<string,PortValue> {
     ["Image"]=PortValue.Image(image),["Width"]=PortValue.Integer(image.Width),["Height"]=PortValue.Integer(image.Height),
     ["PixelFormat"]=PortValue.Integer(0x1080001),["SNCode"]=PortValue.Integer(state.Sequence),
     ["CurImageName"]=PortValue.Text(Path.GetFileName(selectedPath)),["CurImagePath"]=PortValue.Text(c.ResolvePath(selectedPath)),
     ["ModuStatus"]=PortValue.Integer(1),["ModuRunTime"]=PortValue.Number(clock.Elapsed.TotalMilliseconds)
    };
    // Native OutMono8 is an additional conversion for RGB, not a duplicate MONO8 port emission.
    return new(ModuleStatus.Success,outputs);
   } catch(Exception ex)when(ex is IOException or InvalidDataException or UnauthorizedAccessException or JsonException or ArgumentException or InvalidOperationException or KeyNotFoundException) {
    return Failed("ImageReadFailed",ex.Message);
   } finally {
    if(entered)state!.Execution.Release();
   }
  }
  static ModuleResult Failed(string code,string message)=>new(ModuleStatus.Failed,new Dictionary<string,PortValue>{["ModuStatus"]=PortValue.Integer(0)},code,message);
 }
}
