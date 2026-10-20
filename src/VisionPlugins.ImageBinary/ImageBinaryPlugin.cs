using System.IO;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.ImageBinary;
// Independent CPU implementation. Native module XML defines the parameter and ROI contract.
public sealed class ImageBinaryPlugin:IVisionToolPlugin,IWpfToolEditorFactory,IWpfToolParameterEditorFactory {
 public const string Id="vision.image-binary";
 public static RoiEditorCapabilities RoiCapabilities{get;}=new([RoiKind.Rectangle,RoiKind.SectorRing,RoiKind.Polygon,RoiKind.Sector],16,32,true,true);
 static JsonElement J<T>(T value)=>JsonSerializer.SerializeToElement(value);
 static ParameterDescriptor P(string name,int value,int min,int max)=>new(name,ParameterKind.Integer,true,J(value),min,max);
 static ParameterDescriptor N(string name,double value,double min,double max)=>new(name,ParameterKind.Number,true,J(value),min,max);
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","图像二值化","ImageProcessing","catalog/IMVSBinaryModuImageEnable.png");
 public ModuleDescriptor Descriptor{get;}=new(Id,"1.0.0","图像二值化",new[]{new PortDescriptor("Image",DataKind.GrayImage),new("InputMask",DataKind.GrayImage,false),new("Region",DataKind.Region,false),new("Region2",DataKind.Region,false),new("InputBlindRegion",DataKind.Region,false),new("CorrectInfo",DataKind.Fixture,false),new("OriginPoint",DataKind.Point,false),new("RunPoint",DataKind.Point,false)}.Concat(BinaryRegionResolver.NumericFields.Select(n=>new PortDescriptor(n,DataKind.Number,false))).ToArray(),[new("Image",DataKind.GrayImage),new("OutMask",DataKind.GrayImage,false),new("ProcessedPixels",DataKind.Integer)],new ParameterDescriptor[]{
 new("OutputMaskImage",ParameterKind.Boolean,true,J(false)),P("ROIAreaType",0,0,1),P("BinaryType",1,1,8),P("NonROIPrcType",0,0,1),P("LowThreshold",100,0,255) with{Editor=new(ParameterEditorKind.Range,["LowThreshold","HighThreshold"])},P("HighThreshold",255,0,255),P("KernelWidth",3,1,51),P("KernelHeight",3,1,51),P("GaussKernelSize",3,1,51),N("GaussSigma",1,.1,100),P("CompareType",0,0,3),P("ThresholdOffset",0,-255,255),N("SauvolaAdjustCoef",.2,-1,1),N("SauvolaRange",128,.1,255),P("SauvolaSegType",0,0,1),P("SauvolaWinWidth",15,1,100),P("SauvolaWinHeight",15,1,100),P("RoiType",0,0,0) with{Editor=new(ParameterEditorKind.Roi){Roi=RoiCapabilities}},new("Assist0",ParameterKind.Boolean,true,J(false)),P("RoiSelect",0,0,1),P("ExternRoiSelect",0,0,1),P("InheritWay",0,0,3),P("SelectWay",0,0,2)}.Concat(BinaryRegionResolver.NumericFields.Select(n=>new ParameterDescriptor(n,ParameterKind.Number,true,J(n.Contains("Scale")?1d:n.Contains("Width")||n.Contains("Height")||n.Contains("OuterRadius")?100d:n.Contains("AngleExtend")?360d:0d)))).ToArray());
 public IModule CreateRuntime()=>new Runtime(this);
 public Form CreateEditor(ToolEditorContext c)=>throw new NotSupportedException("Use WPF.");
 public System.Windows.Window CreateWpfEditor(ToolEditorContext c)=>new ImageBinaryWindow(this,c);
 public System.Windows.Window CreateParameterEditor(ToolParameterEditorContext c)=>new ImageBinaryParameterWindow(c);
 public void Reset(string nodeId){}
 sealed class Runtime(ImageBinaryPlugin owner):IModule {
  public ModuleDescriptor Descriptor=>owner.Descriptor;
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken token){try{
   if(c.Integer("BinaryType")!=1)return new(new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"BinaryModeUnavailable","当前独立实现只支持硬阈值，其他原生模式尚未验证。"));
   int low=c.Integer("LowThreshold"),high=c.Integer("HighThreshold"),outside=c.Integer("NonROIPrcType");if(low<0||high>255||low>high||outside is <0 or >1)throw new InvalidDataException("阈值范围或ROI外处理方式无效。");RoiGeometry.ValidateForEditor(c.Rois,RoiCapabilities);
   if(c.Rois.Any(r=>r.Exclusion&&r.Kind!=RoiKind.Polygon))throw new InvalidDataException("原生屏蔽区域只接受多边形。");
   var input=c.Input<GrayImage>("Image");GrayImage? mask=c.Inputs.TryGetValue("InputMask",out var m)?m.As<GrayImage>():null;if(c.Integer("ROIAreaType")==1&&mask is not null&&(mask.Width!=input.Width||mask.Height!=input.Height))throw new InvalidDataException("掩膜尺寸与输入图像不一致。");
   int areaType=c.Integer("ROIAreaType");if(areaType==1&&mask is null)throw new InvalidDataException("请订阅输入掩膜图像。");byte[] output=new byte[input.Width*input.Height],outMask=areaType==1?RoiRasterizer.CreateMask([],input.Width,input.Height,token):BinaryRegionResolver.Mask(c,input.Width,input.Height,token);long count=0;
   for(int y=0;y<input.Height;y++){token.ThrowIfCancellationRequested();for(int x=0;x<input.Width;x++){int i=y*input.Width+x;byte value=input[x,y];bool inside=outMask[i]!=0&&(areaType==0||mask![x,y]!=0);if(inside){output[i]=(byte)(value>=low&&value<=high?255:0);outMask[i]=255;count++;}else {output[i]=outside==1?value:(byte)0;outMask[i]=0;}}}
   var outputs=new Dictionary<string,PortValue>{{"Image",PortValue.Image(new(input.Width,input.Height,output))},{"ProcessedPixels",PortValue.Integer(count)}};if(c.Boolean("OutputMaskImage"))outputs["OutMask"]=PortValue.Image(new(input.Width,input.Height,outMask));return new(new ModuleResult(ModuleStatus.Success,outputs));
  }catch(Exception e)when(e is InvalidDataException or ArgumentException or InvalidOperationException){return new(new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"InvalidRoiOrInput",e.Message));}}
 }
}
