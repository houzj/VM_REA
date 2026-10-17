using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.ImageSource;

public sealed record AcquisitionImageConfig(double Exposure=0,double Gain=0,int TriggerTime=500,int Channel=1,int Brightness=0,double Distribution=0,double Irradiation=0,int LightId=-1);

/// <summary>Independent reconstruction of ImageAcquisitionModule; evidence in docs/plugins/IMAGE-ACQUISITION.md.</summary>
public sealed class ImageAcquisitionPlugin:IVisionToolPlugin,IWpfToolEditorFactory,IToolImageListProvider,IToolImagePreviewProvider,IToolImageOutputProvider,IToolAssetPersistence {
 public bool SupportsSingleImageExecution=>false;
 public IReadOnlyList<ToolImageOutput> ImageOutputs{get;}=Enumerable.Range(0,8).Select(i=>new ToolImageOutput($"OutImage{i}Image","输出图像"+i)).ToArray();
 public const string Id="vision.image-acquisition";
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","多图采集","collection","catalog/ImageAcquisitionModuleImageEnable.png");
 internal static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 static ParameterDescriptor P<T>(string name,ParameterKind kind,T value,double? min=null,double? max=null)=>new(name,kind,true,J(value),min,max);
 static PortDescriptor[] Outputs()=>Enumerable.Range(0,8).SelectMany(i=>new[]{new PortDescriptor($"OutImage{i}Image",DataKind.Image,false),new($"OutImage{i}Width",DataKind.Integer,false),new($"OutImage{i}Height",DataKind.Integer,false),new($"OutImage{i}PixelFormat",DataKind.Integer,false),new($"OutImage{i}Distribution",DataKind.Number,false),new($"OutImage{i}Irradiation",DataKind.Number,false)}).Concat(new[]{new PortDescriptor("ImageCount",DataKind.Integer),new("FrameNum",DataKind.Integer,false),new("LostFrameCount",DataKind.Integer,false),new("LostPacketCount",DataKind.Integer,false),new("GetCameraFailCount",DataKind.Integer,false),new("ModuRunTime",DataKind.Number)}).ToArray();
 public ModuleDescriptor Descriptor{get;}=new(Id,"1.0.0","多图采集",[],Outputs(),[
  P("ImageSourceType",ParameterKind.Integer,2,1,2),P("CameraID",ParameterKind.Integer,-1),P("CameraIdentity",ParameterKind.Text,""),P("Interval",ParameterKind.Integer,1000,0,1000),P("SolSaveImageData",ParameterKind.Boolean,false),P("ImageDir",ParameterKind.Text,""),P("ImageCount",ParameterKind.Integer,4,3,8),P("DelayAfterTriggerLight",ParameterKind.Integer,10,0,1000),P("ImagePaths",ParameterKind.Text,"[]"),P("ImageConfigList",ParameterKind.Text,"[]"),P("EmbeddedImageResources",ParameterKind.Text,"{}")]);
 internal static IAcquisitionLightService? LightService(ModuleExecutionScope? scope)=>scope?.Service<IAcquisitionLightService>()??(scope?.Service<IAcquisitionLightModuleTransport>() is {} transport?new AcquisitionLightProtocolService(transport):null);
 sealed class State {public readonly SemaphoreSlim Gate=new(1,1);public int Index;public int Mode;public string[] Paths=[];public int Failures;public int CameraId=int.MinValue;}
 readonly Dictionary<string,State> fallback=new(); readonly object gate=new();
 public IModule CreateRuntime()=>new Runtime(this);
 public Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use WPF editor factory.");
 public System.Windows.Window CreateWpfEditor(ToolEditorContext context)=>new ImageAcquisitionWindow(context);
 public void Reset(string nodeId){lock(gate)foreach(var key in fallback.Keys.Where(k=>k.EndsWith("|"+nodeId,StringComparison.Ordinal)).ToArray())fallback.Remove(key);}
 State GetState(ModuleExecutionContext c){if(c.Scope is {} s)return s.State(Id,c.NodeId,()=>new State());lock(gate){string key=Path.GetFullPath(c.BaseDirectory)+"|"+c.NodeId;if(!fallback.TryGetValue(key,out var value))fallback[key]=value=new();return value;}}
 internal static string[] Paths(string json){var paths=JsonSerializer.Deserialize<string[]>(json)??[];if(paths.Any(string.IsNullOrWhiteSpace))throw new InvalidDataException("图像列表包含空路径。");return paths;}
 internal static AcquisitionImageConfig[] Configs(string json){var entries=JsonSerializer.Deserialize<AcquisitionImageConfig[]>(json)??[];if(entries.Length>8||entries.Any(e=>e is null||!double.IsFinite(e.Exposure)||!double.IsFinite(e.Gain)||!double.IsFinite(e.Distribution)||!double.IsFinite(e.Irradiation)||e.TriggerTime is <0 or >65535||e.Channel is <1 or >6||e.Brightness is <0 or >255||e.Distribution is <-180 or >180||e.Irradiation is <0 or >90))throw new InvalidDataException("无效的逐图配置。");return entries;}
 // UI traversal is limited to the selected directory and its immediate children.
 // SDK SetImageDirPath is a separate recursive, append-only operation in the original.
 public static string[] ReadUiDirectory(string directory){var result=new List<string>();void Visit(string path,int layer){foreach(var entry in Directory.GetFileSystemEntries(path).OrderBy(p=>p.Length).ThenBy(p=>p,StringComparer.CurrentCulture)){if(File.Exists(entry)){if(new[]{".jpg",".jpeg",".bmp",".png",".tif",".tiff"}.Contains(Path.GetExtension(entry).ToLowerInvariant()))result.Add(entry);}else if(layer==1)Visit(entry,2);}}Visit(directory,1);return result.ToArray();}
 public NodeDocument AppendSdkImageDirectory(NodeDocument node,string directory){if(!Directory.Exists(directory))throw new DirectoryNotFoundException(directory);var paths=Paths(node.Parameters["ImagePaths"].GetString()??"[]").ToList();void Visit(string path){foreach(var item in new DirectoryInfo(path).GetFileSystemInfos()){if(item is DirectoryInfo){if((item.Attributes&FileAttributes.ReparsePoint)==0)Visit(item.FullName);}else if(new[]{".jpg",".jpeg",".bmp",".png",".tif",".tiff"}.Contains(item.Extension.ToLowerInvariant()))paths.Add(item.FullName);}}Visit(directory);var values=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());values["ImagePaths"]=J(JsonSerializer.Serialize(paths));return node with{Parameters=values};}
 public IReadOnlyList<ToolImageEntry> GetImages(NodeDocument node)=>Paths(node.Parameters["ImagePaths"].GetString()??"[]").Select(p=>new ToolImageEntry(p,false)).ToArray();
 static Dictionary<string,string> Map(NodeDocument n)=>JsonSerializer.Deserialize<Dictionary<string,string>>(n.Parameters["EmbeddedImageResources"].GetString()??"{}")??new();
 public Stream OpenEncodedImage(NodeDocument node,string path,string baseDirectory,ResourceCatalog? resources=null)=>node.Parameters["SolSaveImageData"].GetBoolean()&&resources is not null&&Map(node).TryGetValue(path,out var id)?resources.OpenVerified(id):File.OpenRead(Path.GetFullPath(path,baseDirectory));
 public IImageData LoadPreview(NodeDocument node,string path,string baseDirectory,ResourceCatalog? resources=null){using var s=OpenEncodedImage(node,path,baseDirectory,resources);return LocalImageDecoder.ReadImage(s,false);}
 public ToolAssetSnapshot PrepareSave(NodeDocument node,string baseDirectory,ResourceCatalog resources){var values=node.Parameters.ToDictionary(p=>p.Key,p=>p.Value.Clone());var paths=Paths(values["ImagePaths"].GetString()??"[]");var map=new Dictionary<string,string>();var assets=new Dictionary<string,ResourceDocument>();if(values["SolSaveImageData"].GetBoolean())foreach(string path in paths){using var stream=File.Exists(Path.GetFullPath(path,baseDirectory))?File.OpenRead(Path.GetFullPath(path,baseDirectory)):OpenEncodedImage(node,path,baseDirectory,resources);if(stream.Length>256L*1024*1024)throw new InvalidDataException("图像资源过大。");using var copy=new MemoryStream();stream.CopyTo(copy);byte[] bytes=copy.ToArray();string hash=Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(),id="plugin:"+Id+":"+node.Id+":"+hash,full=Path.GetFullPath(path,baseDirectory);map[full]=id;assets[id]=new(id,full,hash){EmbeddedBase64=Convert.ToBase64String(bytes)};}values["ImagePaths"]=J(JsonSerializer.Serialize(paths.Select(p=>Path.GetFullPath(p,baseDirectory))));values["EmbeddedImageResources"]=J(JsonSerializer.Serialize(map));if(values["ImageDir"].GetString() is {Length:>0} dir)values["ImageDir"]=J(Path.GetFullPath(dir,baseDirectory));return new(node with{Parameters=values},assets.Values.ToArray());}
 sealed class Runtime(ImageAcquisitionPlugin owner):IModule {
  public ModuleDescriptor Descriptor=>owner.Descriptor;
  public async ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken token){
   using var lease=c.Scope?.Enter();using var linked=CancellationTokenSource.CreateLinkedTokenSource(token,c.Scope?.Stopping??CancellationToken.None);token=linked.Token;token.ThrowIfCancellationRequested();
   foreach(var p in Descriptor.Parameters)if(!c.Parameters.TryGetValue(p.Name,out var value)||!GraphValidator.ValidParameter(p,value))return new(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"InvalidParameter",p.Name);
   var clock=Stopwatch.StartNew();State? state=null;bool entered=false;var output=new Dictionary<string,PortValue>();uint? native=null;string? error=null,message=null;
   try{
    var configs=Configs(c.Text("ImageConfigList"));int mode=c.Integer("ImageSourceType"),count=mode==1?c.Integer("ImageCount"):configs.Length;output["ImageCount"]=PortValue.Integer(count);
    state=owner.GetState(c);await state.Gate.WaitAsync(token);entered=true;if(state.Mode!=mode){state.Mode=mode;state.Failures=0;}
    if(mode==1){var paths=Paths(c.Text("ImagePaths"));if(!state.Paths.SequenceEqual(paths,StringComparer.Ordinal)){bool append=paths.Length>=state.Paths.Length&&paths.Take(state.Paths.Length).SequenceEqual(state.Paths,StringComparer.Ordinal);if(!append)state.Index=0;state.Paths=paths;}if(paths.Length==0)throw new AcquisitionFailure(0xe0000203,"NoImage","本地图像列表为空。");
     var node=new NodeDocument(c.NodeId,Id,"1.0.0","多图采集",c.Parameters.ToDictionary(p=>p.Key,p=>p.Value));
     for(int i=0;i<count;i++){token.ThrowIfCancellationRequested();using var stream=owner.OpenEncodedImage(node,paths[state.Index],c.BaseDirectory,c.Resources);IImageData image=LocalImageDecoder.ReadImage(stream,false);Publish(output,i,image,configs.ElementAtOrDefault(i));state.Index=(state.Index+1)%paths.Length;}
    }else{
     var camera=c.Scope?.Service<IImageAcquisitionService>();int id=c.Integer("CameraID");string serial=c.Text("CameraIdentity");if(serial.Length>0)id=camera?.Cameras.FirstOrDefault(item=>item.PersistentId==serial)?.Id??-1;
     if(state.CameraId!=id){state.CameraId=id;state.Failures=0;}if(id<0||configs.Length==0)throw new AcquisitionFailure(0xe0000001,"ConfigurationMissing","未选择相机或未设置图像配置。");if(camera is null)throw new AcquisitionFailure(0xe0000309,"CameraUnavailable","相机服务不可用。");
     for(int i=0;i<configs.Length;i++){token.ThrowIfCancellationRequested();var step=Stopwatch.StartNew();var cfg=configs[i];
      var lights=cfg.LightId>=0?LightService(c.Scope):null;if(cfg.LightId>=0&&lights is null)message="光源服务不可用。";var lightRequest=new AcquisitionLightRequest(cfg.LightId,cfg.Channel,cfg.Brightness,cfg.TriggerTime);
      try{
      if(lights is not null)try{await lights.TriggerAsync(lightRequest,token);}catch(IOException ex){message="光源触发失败："+ex.Message;}
      await Task.Delay(c.Integer("DelayAfterTriggerLight"),token);
      try{var frame=await camera.AcquireAsync(new(id,cfg.Exposure,cfg.Gain,0){ConvertParametersToFloatBeforeFormatting=false},token);Publish(output,i,frame.Image,cfg);output["FrameNum"]=PortValue.Integer(unchecked((int)frame.FrameNum));output["LostFrameCount"]=PortValue.Integer(unchecked((int)frame.LostFrameCount));output["LostPacketCount"]=PortValue.Integer(unchecked((int)frame.LostPacketCount));}
      // Native ProcessCameraImage ignores the per-slot acquisition return code,
      // continues later slots and reports its final camera-info query status.
      catch(CameraAcquisitionException ex){state.Failures++;native=0;message=ex.Message;}
      }finally{if(lights is not null)try{await lights.CompleteAsync(lightRequest,CancellationToken.None);}catch(IOException ex){message="光源结束失败："+ex.Message;}}
      int remaining=c.Integer("Interval")-(int)Math.Round(step.Elapsed.TotalMilliseconds,MidpointRounding.AwayFromZero);if(remaining>0)await Task.Delay(remaining,token);
     }
     output["GetCameraFailCount"]=PortValue.Integer(state.Failures);
    }
   }catch(OperationCanceledException){throw;}catch(AcquisitionFailure ex){native=ex.Code;error=ex.Kind;message=ex.Message;}catch(FileNotFoundException ex){native=0xe0000201;error="ImageMissing";message=ex.Message;}catch(DirectoryNotFoundException ex){native=0xe0000201;error="ImageMissing";message=ex.Message;}catch(ImageSourceDecodeException ex){native=ex.NativeErrorCode;error="DecodeFailed";message=ex.Message;}catch(Exception ex)when(ex is IOException or JsonException or ArgumentException){error="AcquisitionFailed";message=ex.Message;}
   finally{if(entered)state!.Gate.Release();}
   output["ModuRunTime"]=PortValue.Number(clock.Elapsed.TotalMilliseconds);return new(error is null?ModuleStatus.Success:ModuleStatus.Failed,output,error,message){NativeErrorCode=native??(error is null?0u:null)};
  }
  static void Publish(Dictionary<string,PortValue> o,int slot,IImageData image,AcquisitionImageConfig? config){string prefix=$"OutImage{slot}";o[prefix+"Image"]=PortValue.Image(image);o[prefix+"Width"]=PortValue.Integer(image.Width);o[prefix+"Height"]=PortValue.Integer(image.Height);o[prefix+"PixelFormat"]=PortValue.Integer(image is GrayImage?0x1080001:0x2180014);o[prefix+"Distribution"]=PortValue.Number((float)(config?.Distribution??0));o[prefix+"Irradiation"]=PortValue.Number((float)(config?.Irradiation??0));}
 }
 sealed class AcquisitionFailure(uint? code,string kind,string message):IOException(message){public uint? Code=>code;public string Kind=>kind;}
}
