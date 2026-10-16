using System.Diagnostics;
using System.IO;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.ImageSource;

/// <summary>Independent implementation; original Process RVA 0xde30 and CopyImage RVA 0xe6a0.</summary>
public sealed class ImageBufferPlugin:IVisionToolPlugin,IWpfToolEditorFactory,IToolImageOutputProvider {
 public const string Id="vision.image-buffer";
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","缓存图像","collection","catalog/ImageBufferModuleImageEnable.png");
 public IReadOnlyList<ToolImageOutput> ImageOutputs{get;}=Enumerable.Range(0,15).Select(i=>new ToolImageOutput($"OutImage{i}Image","输出图像"+i)).ToArray();
 public ModuleDescriptor Descriptor{get;}=new(Id,"1.0.0","缓存图像",[new("InputImage",DataKind.Image,false),new("RefreshFlag",DataKind.Integer,false),new("BufferCount",DataKind.Integer,false)],
  Enumerable.Range(0,15).SelectMany(i=>new[]{new PortDescriptor($"OutImage{i}Image",DataKind.Image,false),new($"OutImage{i}Width",DataKind.Integer,false),new($"OutImage{i}Height",DataKind.Integer,false),new($"OutImage{i}PixelFormat",DataKind.Integer,false)}).Concat(new[]{new PortDescriptor("ModuStatus",DataKind.Integer),new("ModuRunTime",DataKind.Number)}).ToArray(),[]);
 sealed class State {public readonly object Gate=new();public readonly Queue<IImageData> Images=new();}
 readonly Dictionary<string,State> fallback=new();readonly object gate=new();
 State GetState(ModuleExecutionContext c){if(c.Scope is {} scope)return scope.State(Id,c.NodeId,()=>new State());lock(gate){string key=Path.GetFullPath(c.BaseDirectory)+"|"+c.NodeId;if(!fallback.TryGetValue(key,out var state))fallback[key]=state=new();return state;}}
 public void Reset(string nodeId){lock(gate)foreach(string key in fallback.Keys.Where(k=>k.EndsWith("|"+nodeId,StringComparison.Ordinal)).ToArray())fallback.Remove(key);}
 public IModule CreateRuntime()=>new Runtime(this);
 public Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use WPF editor factory.");
 public System.Windows.Window CreateWpfEditor(ToolEditorContext context)=>new ImageBufferWindow(context);
 sealed class Runtime(ImageBufferPlugin owner):IModule {
  public ModuleDescriptor Descriptor=>owner.Descriptor;
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken token){
   token.ThrowIfCancellationRequested();var clock=Stopwatch.StartNew();var output=new Dictionary<string,PortValue>();uint code=0;string? error=null;
   int Read(string name,int value)=>c.Inputs.TryGetValue(name,out var p)?checked((int)p.As<long>()):value;
   int count=Read("BufferCount",15),flag=Read("RefreshFlag",1);
   // Native validates count before reading/copying input or mutating cache.
   if(count is <0 or >15){code=0xe000031c;error="InvalidBufferCount";}
   else {var state=owner.GetState(c);lock(state.Gate){
    token.ThrowIfCancellationRequested();
    if(flag==1){
     if(!c.Inputs.TryGetValue("InputImage",out var input)){code=0xe0000510;error="InputImageMissing";}
     else {var image=input.As<IImageData>();IImageData copy=image switch{GrayImage=>new GrayImage(image.Width,image.Height,image.Pixels),RgbImage=>new RgbImage(image.Width,image.Height,image.Pixels),RawImage=>new RawImage(image.Width,image.Height,image.Format,image.Pixels,image.Stride),_=>throw new ArgumentException("Unsupported image format.")};state.Images.Enqueue(copy);while(state.Images.Count>count)state.Images.Dequeue();}
    }
    if(code==0){int i=0;foreach(var image in state.Images){string prefix=$"OutImage{i++}";output[prefix+"Image"]=PortValue.Image(image);output[prefix+"Width"]=PortValue.Integer(image.Width);output[prefix+"Height"]=PortValue.Integer(image.Height);output[prefix+"PixelFormat"]=PortValue.Integer((int)image.Format);}}
   }}
   output["ModuStatus"]=PortValue.Integer(code==0?1:0);output["ModuRunTime"]=PortValue.Number(clock.Elapsed.TotalMilliseconds);
   return ValueTask.FromResult(new ModuleResult(code==0?ModuleStatus.Success:ModuleStatus.Failed,output,error){NativeErrorCode=code});
  }
 }
}
