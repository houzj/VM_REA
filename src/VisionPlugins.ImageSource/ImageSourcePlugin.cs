using System.Text.Json;
using VisionFramework;
using VisionPluginContracts;
using VisionIO;
namespace VisionPlugins.ImageSource;
public sealed partial class ImageSourcePlugin:IVisionToolPlugin,IToolImageListProvider,IToolImagePreviewProvider,IWpfToolEditorFactory {
 public ToolManifest Manifest{get;}=new("vision.image-source","1.0.0","图像源","collection","tool-image-file.png");
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 static ParameterDescriptor P<T>(string name,ParameterKind kind,T value,double? min=null,double? max=null)=>new(name,kind,true,J(value),min,max);
 public ModuleDescriptor Descriptor{get;}=new("vision.image-source","1.0.0","图像源",[new("ClearTriggerValue",DataKind.Integer,false),new("TriggerString",DataKind.Text,false),new("TriggerFilter",DataKind.Text,false),new("ExposureTimeInput",DataKind.Number,false),new("GainInput",DataKind.Number,false)],[new("Image",DataKind.Image),new("Width",DataKind.Integer),new("Height",DataKind.Integer),new("PixelFormat",DataKind.Integer),new("SNCode",DataKind.Integer,false),new("CurImageName",DataKind.Text,false),new("CurImagePath",DataKind.Text,false),new("ModuStatus",DataKind.Integer),new("ModuRunTime",DataKind.Number),new("FrameNum",DataKind.Integer,false),new("LostFrameCount",DataKind.Integer,false),new("LostPacketCount",DataKind.Integer,false),new("GetCameraFailCount",DataKind.Integer,false),new("ImageMono8",DataKind.GrayImage,false),new("WidthMono8",DataKind.Integer,false),new("HeightMono8",DataKind.Integer,false),new("PixelFormatMono8",DataKind.Integer,false)],[
 P("ImageSourceType",ParameterKind.Integer,1,1,3),P("PixelFormat",ParameterKind.Integer,1,1,2),P("SolSaveImageData",ParameterKind.Boolean,false),P("ShowImageName",ParameterKind.Boolean,true),P("InitialSN",ParameterKind.Integer,1,0,100000),P("PathCache",ParameterKind.Integer,0,0,5),P("StitchEnable",ParameterKind.Boolean,false),P("AutoPlay",ParameterKind.Boolean,true),P("AutoStop",ParameterKind.Boolean,false),P("TriggerStringEnable",ParameterKind.Boolean,false),P("IsSubscribeFolderMode",ParameterKind.Boolean,false),P("SubscribeFolderPath",ParameterKind.Text,""),P("RefreshImage",ParameterKind.Boolean,true),P("TriggerProcessRun",ParameterKind.Boolean,false),P("CameraID",ParameterKind.Integer,-1),P("CameraIdentity",ParameterKind.Text,""),P("CurrentImagePath",ParameterKind.Text,""),P("ImagePaths",ParameterKind.Text,"[]"),P("Interval",ParameterKind.Integer,0,0,1000),P("ExecuteType",ParameterKind.Integer,1,1,2),P("SelectedPaths",ParameterKind.Text,"[]"),P("StitchStartHeight",ParameterKind.Integer,0,0,20000),P("StitchHeight",ParameterKind.Integer,2000,0,20000),P("ClearTrigger",ParameterKind.Boolean,false),P("OutMono8",ParameterKind.Boolean,false),P("SetCameraParamWaitTime",ParameterKind.Integer,0,0),P("ConfigurationUserData",ParameterKind.Text,"{}"),P("EmbeddedImageResources",ParameterKind.Text,"{}")]);

 readonly Dictionary<(string Directory,string NodeId),State> states=new();
 readonly object gate=new();
 sealed class State:IDisposable {
  public readonly SemaphoreSlim Execution=new(1,1);
  public readonly Dictionary<string,VisionCore.IImageData> Cache=new(StringComparer.Ordinal);
  public string[] Paths=[];
  public string RequestedPath="";
  public int Index=-1;
  public int InitialSN=-1;
  public int Sequence;
  public VisionCore.IImageData? PendingFrame;
  public string[] FolderPaths=[];
  public LocalFolderSubscription? Folder;
  public void Dispose(){Folder?.Dispose();Folder=null;}
  public int CameraFailCount;
  public int PixelFormat;
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
 public IReadOnlyList<ToolImageEntry> GetImages(NodeDocument node,ModuleExecutionScope scope){
  var state=scope.State("vision.image-source",node.Id,()=>new State());bool folder=node.Parameters.TryGetValue("IsSubscribeFolderMode",out var enabled)&&enabled.GetBoolean();var paths=folder?state.FolderPaths:ReadPaths(node.Parameters["ImagePaths"].GetString()??"[]");string current=state.Index>=0&&state.Index<state.Paths.Length?state.Paths[state.Index]:node.Parameters["CurrentImagePath"].GetString()??"";return paths.Select(path=>new ToolImageEntry(path,path==current)).ToArray();
 }
 public VisionCore.IImageData LoadPreview(NodeDocument node,string path,string baseDirectory,ResourceCatalog? resources=null){using var stream=OpenEncodedImage(node,path,baseDirectory,resources);return LocalImageDecoder.ReadImage(stream,node.Parameters["PixelFormat"].GetInt32()==2);}
 public void SubmitImage(ModuleExecutionScope scope,string nodeId,VisionCore.IImageData image){ArgumentNullException.ThrowIfNull(image);var state=scope.State("vision.image-source",nodeId,()=>new State());Interlocked.Exchange(ref state.PendingFrame,image);}
 public void Reset(ModuleExecutionScope scope,string nodeId)=>scope.RemoveNode("vision.image-source",nodeId);
 public bool IsImageListVisible(NodeDocument node)=>!node.Parameters.TryGetValue("ImageSourceType",out var sourceType)||sourceType.GetInt32()==1;
 public IModule CreateRuntime()=>new Runtime(this);
 public Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use WPF editor factory");
 public System.Windows.Window CreateWpfEditor(ToolEditorContext context)=>new ImageSourceWindow(context);
 public void Reset(string nodeId) {
  lock(gate)foreach(var key in states.Keys.Where(k=>k.NodeId==nodeId).ToArray())states.Remove(key);
 }
 State GetState(ModuleExecutionContext context) {
  if(context.Scope is {} scope)return scope.State("vision.image-source",context.NodeId,()=>new State());
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
   using var lease=c.Scope?.Enter();
   using var scopeCancellation=CancellationTokenSource.CreateLinkedTokenSource(token,c.Scope?.Stopping??CancellationToken.None);token=scopeCancellation.Token;
   token.ThrowIfCancellationRequested();
   State? state=null;
   bool entered=false;
   try {
    foreach(var descriptor in Descriptor.Parameters) {
     if(!c.Parameters.TryGetValue(descriptor.Name,out var value)||!GraphValidator.ValidParameter(descriptor,value))
      return Failed("InvalidParameter","无效参数: "+descriptor.Name);
    }
    int sourceMode=c.Integer("ImageSourceType");


    if(sourceMode==1&&c.Boolean("IsSubscribeFolderMode")&&c.Scope is null)return Failed("FolderWatchUnavailable","目录订阅需要方案会话。");


    string trigger=c.Inputs.TryGetValue("TriggerString",out var triggerValue)?triggerValue.As<string>():"",filter=c.Inputs.TryGetValue("TriggerFilter",out var filterValue)?filterValue.As<string>():"";
    if(c.Boolean("TriggerStringEnable")&&trigger.Length>0&&filter.Length>0&&trigger!=filter&&trigger!="##RunProcedureSilently##"&&filter!="##RunProcedureSilently##")return new(ModuleStatus.NoMatch,new Dictionary<string,PortValue>{["ModuStatus"]=PortValue.Integer(0)},"TriggerFiltered"){Continuation=ModuleContinuation.SuppressSuccessors,NativeErrorCode=0xe000001b};

    bool folderMode=sourceMode==1&&c.Boolean("IsSubscribeFolderMode");
    var paths=sourceMode==1&&!folderMode?ReadPaths(c.Text("ImagePaths")):[];
    if(c.Integer("ExecuteType")==2) {
     var selected=ReadPaths(c.Text("SelectedPaths")).ToHashSet(StringComparer.Ordinal);
     paths=paths.Where(selected.Contains).ToArray();
    }
    // CurrentImagePath selects a list entry; it is not the native useLocalPic one-shot API.
    if(sourceMode==1&&!folderMode&&paths.Length==0)return Failed("NoImage","未配置可执行图像");
    state=owner.GetState(c);
    await state.Execution.WaitAsync(token);
    entered=true;
    var clock=System.Diagnostics.Stopwatch.StartNew();
    VisionCore.IImageData image;CameraFrame? acquired=null;string selectedPath="";
    if(sourceMode!=3&&state.InitialSN!=c.Integer("InitialSN")){state.InitialSN=c.Integer("InitialSN");state.Sequence=state.InitialSN;}
    if(!folderMode){state.Folder?.Dispose();state.Folder=null;}
    if(sourceMode==1){
    if(folderMode){
     if(string.IsNullOrWhiteSpace(c.Text("SubscribeFolderPath")))return Failed("NoImage","未配置订阅目录。");
     string directory=c.ResolvePath(c.Text("SubscribeFolderPath"));
     if(state.Folder?.DirectoryPath!=directory){state.Folder?.Dispose();state.Folder=new LocalFolderSubscription(directory,c.Scope!,c.NodeId);}
     state.Folder.TriggerRun=c.Boolean("TriggerProcessRun");
     if(state.Folder.ConsumeDirty()){state.FolderPaths=LocalFolderCatalog.Read(directory);if(c.Boolean("RefreshImage"))c.Scope!.RequestRefresh(c.NodeId);}
     paths=state.FolderPaths;
     if(c.Integer("ExecuteType")==2){var selected=ReadPaths(c.Text("SelectedPaths")).ToHashSet(StringComparer.Ordinal);paths=paths.Where(selected.Contains).ToArray();}
     if(paths.Length==0)return Failed("NoImage","订阅目录中无可执行图像。");
    }
    if(state.InitialSN!=c.Integer("InitialSN")) {
     state.InitialSN=c.Integer("InitialSN");
     state.Sequence=state.InitialSN;
    }
    if(!state.Paths.SequenceEqual(paths,StringComparer.Ordinal)) {
     string? previous=state.Index>=0&&state.Index<state.Paths.Length?state.Paths[state.Index]:null;
     if(!folderMode)state.Index=previous is null?-1:Array.IndexOf(paths,previous);
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
    selectedPath=paths[state.Index];
    if(c.Boolean("AutoPlay")&&c.Boolean("AutoStop")&&state.Index==paths.Length-1&&c.Control?.Mode==ExecutionMode.Continuous)c.Control.RequestStop();
    await Task.Delay(c.Integer("Interval"),token);clock.Restart();

    if(state.PixelFormat!=c.Integer("PixelFormat")){state.Cache.Clear();state.PixelFormat=c.Integer("PixelFormat");}
    if(!state.Cache.TryGetValue(selectedPath,out var cachedImage)) {
     var node=new NodeDocument(c.NodeId,owner.Manifest.Id,owner.Manifest.Version,owner.Manifest.Name,c.Parameters.ToDictionary(p=>p.Key,p=>p.Value));
     using var encoded=owner.OpenEncodedImage(node,selectedPath,c.BaseDirectory,c.Resources);cachedImage=LocalImageDecoder.ReadImage(encoded,c.Integer("PixelFormat")==2);
     // Native local RGB+OutMono8 loads the file through a second MONO decoder.
     // Its small-color restriction is distinct from converting an acquired RGB frame.
     if(c.Boolean("OutMono8")&&cachedImage is VisionCore.RgbImage){encoded.Position=0;_=LocalImageDecoder.ReadImage(encoded,false);}
     token.ThrowIfCancellationRequested();
     // The native cache admits the first N paths and retains hits; it is not an LRU cache.
     if(state.Cache.Count<c.Integer("PathCache"))state.Cache.Add(selectedPath,cachedImage);
    }
    image=cachedImage;
    } else if(sourceMode==3){
     image=Interlocked.Exchange(ref state.PendingFrame,null)!;
     if(image is null)return Failed("SdkImageNotSubmitted","SDK本次运行前未提交新图像（原厂 0xe000000f）。");
    } else {
     var acquisition=c.Scope?.Service<IImageAcquisitionService>();
     string identity=c.Parameters.TryGetValue("CameraIdentity",out var savedIdentity)?savedIdentity.GetString()??"":"";var camera=acquisition?.Cameras.FirstOrDefault(camera=>identity.Length>0?camera.PersistentId==identity:camera.Id==c.Integer("CameraID"));
     if(acquisition is null||camera is null)return Failed("CameraNotBound","相机未绑定或不可用（原厂 0xe0000309）。");
     double? InputNumber(string name)=>c.Inputs.TryGetValue(name,out var input)?input.As<double>():null;
     var frame=await acquisition.AcquireAsync(new(camera.Id,InputNumber("ExposureTimeInput"),InputNumber("GainInput"),c.Integer("SetCameraParamWaitTime")),token);acquired=frame;image=frame.Image;
    }
    token.ThrowIfCancellationRequested();
    if(sourceMode==2)state.Sequence=unchecked(state.Sequence+1);
    if(c.Boolean("StitchEnable")&&c.Integer("StitchHeight")>0) {
     int height=c.Integer("StitchHeight"),start=c.Integer("StitchStartHeight");
     if(c.Boolean("ClearTrigger")&&c.Inputs.TryGetValue("ClearTriggerValue",out var clear)&&clear.As<long>()!=0){Array.Clear(state.StitchBuffer);state.StitchWidth=0;}
     // StitchImageOutput rejects a changed width before touching its retained buffer.
     if((state.StitchWidth!=0&&state.StitchWidth!=image.Width)||(long)start+height>image.Height){
      var failure=Failed("StitchGeometryInvalid","拼接起始高度与高度超出图像，或拼接图像宽度发生变化（原厂 0xe0000502）");
      if(acquired is not null){var partial=failure.Outputs.ToDictionary(p=>p.Key,p=>p.Value);partial["SNCode"]=PortValue.Integer(state.Sequence);partial["FrameNum"]=PortValue.Integer(acquired.FrameNum);partial["GetCameraFailCount"]=PortValue.Integer(state.CameraFailCount);return failure with{Outputs=partial};}return failure;
     }
     int channels=image.Format==VisionCore.ImagePixelFormat.Rgb24?3:1;
     int length=checked(image.Stride*(image.Height+height)),prefix=checked(image.Stride*height);
     // Native reallocates and zeroes only when the complete output byte count changes.
     if(state.StitchBuffer.Length!=length)state.StitchBuffer=new byte[length];
     image.Pixels.CopyTo(state.StitchBuffer.AsSpan(prefix));
     VisionCore.IImageData stitched=channels==3?new VisionCore.RgbImage(image.Width,image.Height+height,state.StitchBuffer):new VisionCore.GrayImage(image.Width,image.Height+height,state.StitchBuffer);
     image.Pixels.Slice(start*image.Stride,prefix).CopyTo(state.StitchBuffer);
     state.StitchWidth=image.Width;
     image=stitched;
    }
    if(sourceMode==1)state.Sequence=unchecked(state.Sequence+1);
    var outputs=new Dictionary<string,PortValue> {
     ["Image"]=PortValue.Image(image),["Width"]=PortValue.Integer(image.Width),["Height"]=PortValue.Integer(image.Height),
     ["PixelFormat"]=PortValue.Integer((int)image.Format),["SNCode"]=PortValue.Integer(state.Sequence),
     ["CurImageName"]=PortValue.Text(Path.GetFileName(selectedPath)),["CurImagePath"]=PortValue.Text(c.ResolvePath(selectedPath)),
     ["ModuStatus"]=PortValue.Integer(1),["ModuRunTime"]=PortValue.Number(clock.Elapsed.TotalMilliseconds)
    };
    if(sourceMode!=1){if(sourceMode==3)outputs.Remove("SNCode");outputs.Remove("CurImageName");outputs.Remove("CurImagePath");}
    if(acquired is not null){outputs["FrameNum"]=PortValue.Integer(acquired.FrameNum);outputs["LostFrameCount"]=PortValue.Integer(acquired.LostFrameCount);outputs["LostPacketCount"]=PortValue.Integer(acquired.LostPacketCount);outputs["GetCameraFailCount"]=PortValue.Integer(state.CameraFailCount);}
    // Native OutMono8 is an additional conversion for RGB, not a duplicate MONO8 port emission.
    if(c.Boolean("OutMono8")&&image is VisionCore.RgbImage color){var mono=ConvertMono(color);outputs["ImageMono8"]=PortValue.Image(mono);outputs["WidthMono8"]=PortValue.Integer(mono.Width);outputs["HeightMono8"]=PortValue.Integer(mono.Height);outputs["PixelFormatMono8"]=PortValue.Integer((int)mono.Format);}
    return new(ModuleStatus.Success,outputs);
   } catch(CameraAcquisitionException ex) {
    if(state is not null)state.CameraFailCount++;return new(ModuleStatus.Failed,new Dictionary<string,PortValue>{["ModuStatus"]=PortValue.Integer(0),["GetCameraFailCount"]=PortValue.Integer(state?.CameraFailCount??1)},"CameraGrabFailed",ex.Message){NativeErrorCode=ex.NativeErrorCode is 0x80000007 or 0x80000107?0:ex.NativeErrorCode};
   } catch(ImageSourceDecodeException ex) {
    return Failed("ImageReadFailed",ex.Message) with{NativeErrorCode=ex.NativeErrorCode};
   } catch(FileNotFoundException ex) {
    return Failed("ImageReadFailed",ex.Message) with{NativeErrorCode=0x80100006};
   } catch(NotSupportedException ex) {
    return Failed("ImageCodecUnavailable",ex.Message);
   } catch(Exception ex)when(ex is IOException or InvalidDataException or UnauthorizedAccessException or JsonException or ArgumentException or InvalidOperationException or KeyNotFoundException) {
    return Failed("ImageReadFailed",ex.Message);
   } finally {
    if(entered)state!.Execution.Release();
   }
  }
  static VisionCore.GrayImage ConvertMono(VisionCore.RgbImage image){
   byte[] pixels=new byte[image.Width*image.Height];var source=image.Pixels;for(int i=0;i<pixels.Length;i++)pixels[i]=(byte)((76*source[i*3]+150*source[i*3+1]+30*source[i*3+2])>>8);return new VisionCore.GrayImage(image.Width,image.Height,pixels);
  }
  static ModuleResult Failed(string code,string message)=>new(ModuleStatus.Failed,new Dictionary<string,PortValue>{["ModuStatus"]=PortValue.Integer(0)},code,message){NativeErrorCode=code switch{"NoImage"=>0xe0000203,"SdkImageNotSubmitted"=>0xe000000f,"CameraNotBound"=>0xe0000309,"StitchGeometryInvalid"=>0xe0000502,_=>(uint?)null}};
 }
}
