using System.Diagnostics;
using System.Text.Json;
using VisionCore;
using VisionFramework;
using VisionPluginContracts;
namespace VisionPlugins.CalibrationBoard;

public sealed partial class CalibrationBoardPlugin:IVisionToolPlugin,IToolConfigurationProvider,IWpfToolEditorFactory {
 public const string Id="vision.calibration-board";
 public ToolManifest Manifest{get;}=new(Id,"1.0.0","标定板标定","calibration","catalog/IMVSCalibBoardCalibModuImageEnable.png");
 public ModuleDescriptor Descriptor{get;}=new(Id,"1.0.0","标定板标定",[new("InputImage",DataKind.GrayImage)],new[]{"EstimationError","Scale","TranslateX","TranslateY","Rotate","Skew","Aspect","PixelPrecision","CalibOriPointX","CalibOriPointY","CalibOriPointXVecX","CalibOriPointXVecY","CalibOriPointYVecX","CalibOriPointYVecY","ModuRunTime","DetectCenterX","DetectCenterY","DetectWidth","DetectHeight","DetectAngle"}.Select(n=>new PortDescriptor(n,DataKind.Number,false)).Concat(new[]{new PortDescriptor("ModuStatus",DataKind.Integer,false),new("CalibPointNum",DataKind.Integer,false),new("CalibPointX",DataKind.NumberArray,false),new("CalibPointY",DataKind.NumberArray,false),new("CalibData",DataKind.Calibration,false),new("Calibration Point",DataKind.Point,false){Cardinality=ValueCardinality.Array},new("Calibration Origin",DataKind.Point,false),new("PosXVector",DataKind.Point,false),new("PosYVector",DataKind.Point,false),new("ROI",DataKind.Region,false)}).ToArray(),CalibrationBoardMetadata.Parameters()){RoiEditor=new([RoiKind.Rectangle],Maximum:1,FullImage:true)};
 public IModule CreateRuntime()=>new Runtime(this);
 public System.Windows.Forms.Form CreateEditor(ToolEditorContext context)=>throw new InvalidOperationException("Use common WPF configuration host.");
 public System.Windows.Window CreateWpfEditor(ToolEditorContext context)=>new ToolConfigurationModelWindow(context,CreateConfiguration(context));
 sealed class Snapshot {public double[]? Matrix;public Dictionary<string,PortValue>? Inputs,Outputs;}
 readonly System.Collections.Concurrent.ConcurrentDictionary<string,Snapshot> standalone=new();
 Snapshot Current(string node,string directory,ModuleExecutionScope? scope)=>scope?.State(Id,node,()=>new Snapshot())??standalone.GetOrAdd(System.IO.Path.GetFullPath(directory)+"|"+node,_=>new());
 public void Export(ToolEditorContext context,string path){if(!System.IO.Path.GetExtension(path).Equals(".iwcal",StringComparison.Ordinal))throw new CalibrationBoardFileFailure(0xe0000204,"标定文件必须使用 .iwcal（0xe0000204）。");var snapshot=Current(context.Node.Id,context.BaseDirectory,context.Scope);lock(snapshot){if(snapshot.Matrix is null)throw new CalibrationBoardFileFailure(0x80100003,"尚无成功标定结果（0x80100003）。");CalibrationBoardFile.WriteExplicit(path,snapshot.Matrix);}}
 public void Reset(string nodeId){foreach(var key in standalone.Keys.Where(k=>k.EndsWith("|"+nodeId,StringComparison.Ordinal)))standalone.TryRemove(key,out _);}
 static uint TryCrop(GrayImage source,RoiDocument roi,out GrayImage image,out int originX,out int originY){
  image=source;originX=originY=0;
  // RectangleF values and native GetLeftTop/Width/Height are float based.
  float width=(float)roi.Width,height=(float)roi.Height;
  float left=(float)((float)roi.CenterX-width*.5f),top=(float)((float)roi.CenterY-height*.5f);
  float right=(float)(left+width),bottom=(float)(top+height);
  if(left>=source.Width||top>=source.Height||right<0||bottom<0)return 0x80100025;
  int x=Math.Max(0,(int)((double)left+.5)),y=Math.Max(0,(int)((double)top+.5));
  int r=(int)Math.Clamp((long)((double)right+.5),0,source.Width),b=(int)Math.Clamp((long)((double)bottom+.5),0,source.Height);
  int cropWidth=r-x,cropHeight=b-y;if(cropWidth<=0||cropHeight<=0)return 0x1000002d;
  byte[] input=source.Pixels.ToArray(),pixels=new byte[cropWidth*cropHeight];
  for(int row=0;row<cropHeight;row++)Buffer.BlockCopy(input,(y+row)*source.Width+x,pixels,row*cropWidth,cropWidth);
  image=new(cropWidth,cropHeight,pixels);originX=x;originY=y;return 0;
 }
 sealed class Runtime(CalibrationBoardPlugin plugin):IModule{
  public ModuleDescriptor Descriptor=>plugin.Descriptor;
  public ValueTask<ModuleResult> ExecuteAsync(ModuleExecutionContext c,CancellationToken token){
   // SDK Run closes the current-result gate but retains the result object used
   // by explicit ExportCalibFile. A failed detection must not erase that matrix.
   var clock=Stopwatch.StartNew();token.ThrowIfCancellationRequested();var snapshot=plugin.Current(c.NodeId,c.BaseDirectory,c.Scope);lock(snapshot){snapshot.Outputs=null;snapshot.Inputs=c.Inputs.ToDictionary(p=>p.Key,p=>p.Value);}foreach(var spec in Descriptor.Parameters)if(!c.Parameters.TryGetValue(spec.Name,out var value)||!GraphValidator.ValidParameter(spec,value))return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"InvalidParameter",spec.Name));
   var sourceImage=c.Input<GrayImage>("InputImage");if(sourceImage.Width<64||sourceImage.Height<64)return ValueTask.FromResult(new ModuleResult(ModuleStatus.NoMatch,new Dictionary<string,PortValue>(),"0x10103054","标定板原始输入图像的宽和高均须至少64像素。"){NativeErrorCode=0x10103054});var image=sourceImage;int roiOriginX=0,roiOriginY=0;if(c.Rois.Count>1||c.Rois.Any(r=>r.Exclusion||r.Kind is not (RoiKind.FullImage or RoiKind.Rectangle)))return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"UnsupportedROI","标定板仅支持矩形或全图ROI。"));
   bool maskedRoi=false;CalibrationBoardMask? mask=null;
   if(c.Rois.Count>0&&c.Rois[0].Kind==RoiKind.Rectangle){var roi=c.Rois[0];if(Math.Abs((double)(float)roi.AngleDegrees)>.0001d){mask=CalibrationBoardRoiMask.Rectangle(sourceImage.Width,sourceImage.Height,(float)roi.CenterX,(float)roi.CenterY,(float)roi.Width,(float)roi.Height,(float)roi.AngleDegrees);if(mask.Width<=0||mask.Height<=0)return ValueTask.FromResult(new ModuleResult(ModuleStatus.NoMatch,new Dictionary<string,PortValue>(),"ROIEmpty","0x1000002d"){NativeErrorCode=0x1000002d});maskedRoi=true;}else{uint roiError=TryCrop(sourceImage,roi,out image,out roiOriginX,out roiOriginY);if(roiError!=0)return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"ROIError","0x"+roiError.ToString("x8")){NativeErrorCode=roiError});}}
   var options=new BoardCalibrationOptions(c.Integer("CalibBoardType"),c.Integer("CoordinateMode"),c.Number("PhysicalSize"),c.Number("OriginX"),c.Number("OriginY"),c.Number("RotationAngle"),c.Integer("HomoFreedom"),c.Integer("CalibBoardWeightFun"),c.Integer("FilterStatus"),c.Integer("CircleBoardCircleMode"),c.Integer("Circularity"),c.Number("WeightFactor"),c.Number("DistThreshold"),c.Number("SampleRatio"),c.Integer("EdgeThreshLow"),c.Integer("EdgeThreshHigh"));
   BoardCalibrationResult result;
   try{result=maskedRoi?CalibrationBoardSolver.SolveMasked(sourceImage,options,mask!,token):CalibrationBoardSolver.SolveCropped(sourceImage,image,options,roiOriginX,roiOriginY,token);}
   catch(NotSupportedException error){return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,new Dictionary<string,PortValue>(),"UnsupportedFeature",error.Message));}
   if(result.Error!=0){var failedOutput=new Dictionary<string,PortValue>{{"ModuStatus",PortValue.Integer(0)}};if(c.Rois.Count>0){var failedDetect=c.Rois[0].Kind==RoiKind.FullImage?new RoiDocument("roi",RoiKind.Rectangle,sourceImage.Width/2d,sourceImage.Height/2d,sourceImage.Width,sourceImage.Height):c.Rois[0];failedOutput["DetectCenterX"]=PortValue.Number((float)failedDetect.CenterX);failedOutput["DetectCenterY"]=PortValue.Number((float)failedDetect.CenterY);failedOutput["DetectWidth"]=PortValue.Number((float)failedDetect.Width);failedOutput["DetectHeight"]=PortValue.Number((float)failedDetect.Height);failedOutput["DetectAngle"]=PortValue.Number((float)failedDetect.AngleDegrees);}lock(snapshot)snapshot.Outputs=failedOutput.ToDictionary(p=>p.Key,p=>p.Value);return ValueTask.FromResult(new ModuleResult(ModuleStatus.NoMatch,failedOutput,"0x"+result.Error.ToString("x8"),result.Diagnostic){NativeErrorCode=result.Error});}
   var fit=result.Fit!;lock(snapshot)snapshot.Matrix=fit.Matrix.ToArray();var output=new Dictionary<string,PortValue>{{"ModuStatus",PortValue.Integer(1)},{"CalibPointNum",PortValue.Integer(result.Points.Count)},{"CalibPointX",PortValue.Numbers(result.Points.Select(p=>p.X))},{"CalibPointY",PortValue.Numbers(result.Points.Select(p=>p.Y))},{"CalibData",PortValue.Calibration(new(fit.Matrix,Id))}};
   foreach(var pair in new Dictionary<string,double>{{"EstimationError",fit.OffsetError},{"Scale",fit.Scale},{"TranslateX",fit.DeltaX},{"TranslateY",fit.DeltaY},{"Rotate",fit.Theta},{"Skew",fit.Skew},{"Aspect",fit.Aspect},{"PixelPrecision",fit.PixelPrecision},{"ModuRunTime",clock.Elapsed.TotalMilliseconds}})output[pair.Key]=PortValue.Number(pair.Value);
   // Keep the source 730e90 float inverse/projection order for origins and unit axes.
   var axes=CalibrationBoardAxes.FromMatrix(fit.Matrix.Select(v=>(float)v).ToArray());
   foreach(var p in new Dictionary<string,double>{{"CalibOriPointX",axes.OriginX},{"CalibOriPointY",axes.OriginY},{"CalibOriPointXVecX",axes.XVectorX},{"CalibOriPointXVecY",axes.XVectorY},{"CalibOriPointYVecX",axes.YVectorX},{"CalibOriPointYVecY",axes.YVectorY}})output[p.Key]=PortValue.Number(p.Value);
   output["Calibration Point"]=PortValue.PointArray(result.Points.Select(p=>new Point2(p.X,p.Y)));output["Calibration Origin"]=PortValue.Point(new(axes.OriginX,axes.OriginY));output["PosXVector"]=PortValue.Point(new(axes.XVectorX,axes.XVectorY));output["PosYVector"]=PortValue.Point(new(axes.YVectorX,axes.YVectorY));var detect=c.Rois.Count==0||c.Rois[0].Kind==RoiKind.FullImage?new RoiDocument("roi",RoiKind.Rectangle,sourceImage.Width/2d,sourceImage.Height/2d,sourceImage.Width,sourceImage.Height):c.Rois[0];output["DetectCenterX"]=PortValue.Number(detect.CenterX);output["DetectCenterY"]=PortValue.Number(detect.CenterY);output["DetectWidth"]=PortValue.Number(detect.Width);output["DetectHeight"]=PortValue.Number(detect.Height);output["DetectAngle"]=PortValue.Number(detect.AngleDegrees);output["ROI"]=PortValue.Region(c.Rois.Count==0||c.Rois[0].Kind==RoiKind.FullImage?[new RoiDocument("roi",RoiKind.Rectangle,sourceImage.Width/2d,sourceImage.Height/2d,sourceImage.Width,sourceImage.Height)]:c.Rois);
   if(!string.IsNullOrEmpty(c.Text("CalibPathName"))&&(c.Boolean("RefreshFileEnable")||!System.IO.File.Exists(c.ResolvePath(c.Text("CalibPathName")))))try{CalibrationBoardFile.Write(c.ResolvePath(c.Text("CalibPathName")),fit.Matrix);}catch(Exception error)when(error is System.IO.IOException or UnauthorizedAccessException or InvalidOperationException){return ValueTask.FromResult(new ModuleResult(ModuleStatus.Failed,output,"CalibrationFileWrite",error.Message));}
   lock(snapshot)snapshot.Outputs=output.ToDictionary(p=>p.Key,p=>p.Value);return ValueTask.FromResult(new ModuleResult(ModuleStatus.Success,output));
  }
 }
}










