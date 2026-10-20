using VisionCore;
using VisionFramework;
using VisionPlugins.Calibration;
namespace VisionPlugins.MapCalibration;

public readonly record struct MapCalibrationOutputBox(float X,float Y,float Width,float Height,float Angle);
public sealed record MapCalibrationImageResult(uint ErrorCode,float[] Matrix,float XRatio,float YRatio,float PixelScale1,float PixelScale2,float MapError,IReadOnlyList<MapCalibrationImagePoint> Source,IReadOnlyList<MapCalibrationImagePoint> Target,IReadOnlyList<MapCalibrationImagePoint> Projection){public IReadOnlyList<MapCalibrationOutputBox> Boxes{get;init;}=[];}

/// <summary>Tool-specific composition over the shared detector and fixed 7ff3c0 entry. Shared detector/IRLS differences remain explicit in the tool evidence.</summary>
public static class MapCalibrationImageSolver {
 public static MapCalibrationImageResult Solve(GrayImage source,GrayImage target,int boardType,int freedom,int weight,float nativeWeightFactor,int filter1=1,int filter2=1,CancellationToken token=default,RoiDocument? sourceRoi=null,RoiDocument? targetRoi=null,NImageSourceExecution? sourceExecution=null){
  var execution=sourceExecution??NImageSourceExecution.Current();var a=Detect(source,sourceRoi,boardType,filter1,token,execution);var b=Detect(target,targetRoi,boardType,filter2,token,execution);
  if(a is null||b is null)return new(0x10103002,[],0,0,0,0,0,[],[],[]);
  // Module image-member ctor 6f80 initializes Detect fields to zero; normal Process does not copy configured ROI into them.
  return SolveBoards(a,b,freedom,weight,nativeWeightFactor) with{Boxes=[default,default]};
 }
 static NImageDetectedBoard? Detect(GrayImage image,RoiDocument? roi,int boardType,int filter,CancellationToken token,NImageSourceExecution execution){
  if(roi is null||roi.Kind==RoiKind.FullImage)return DetectImage(image,boardType,filter,token,execution);
  if(roi.Kind!=RoiKind.Rectangle||roi.Exclusion)throw new ArgumentException("Original Map ROI selector accepts one inclusion rectangle.");
  var mask=RectangleRoiMaskRasterizer.Create(image.Width,image.Height,(float)roi.CenterX,(float)roi.CenterY,(float)roi.Width,(float)roi.Height,(float)roi.AngleDegrees);if(mask.Width<=0||mask.Height<=0)return null;
  var original=image.Pixels.ToArray();var pixels=new byte[checked(mask.Width*mask.Height)];for(int y=0;y<mask.Height;y++)Buffer.BlockCopy(original,(y+mask.Top)*image.Width+mask.Left,pixels,y*mask.Width,mask.Width);
  // 80e690: crop -> source grid -> local mask compaction -> float source origin ->
  // retained-ID minima/dense cells -> ORIGINAL image identity decoding.
  var local=NImageSourceLatticeAdapter.FindSource(new(mask.Width,mask.Height,pixels),token,boardType is 6 or 8?1:2,filter);if(local is null)return null;
  var points=local.Points.Where(p=>{int x=(int)((float)p.X+.5f),y=(int)((float)p.Y+.5f);return x>=0&&y>=0&&x<mask.Width&&y<mask.Height&&mask.Pixels[(y+mask.Top)*image.Width+mask.Left+x]!=0;})
   .Select(p=>p with{X=(float)p.X+(float)mask.Left,Y=(float)p.Y+(float)mask.Top}).ToArray();
  Point2 Offset(Point2 p)=>new((float)p.X+(float)mask.Left,(float)p.Y+(float)mask.Top);
  var quads=local.CodeQuadrilaterals.Select(q=>(IReadOnlyList<Point2>)q.Select(Offset).ToArray()).ToArray();
  var context=local.SourceGrid?.RebuildForProjectedPoints(points.Select(p=>new NImageSourceGridPoint(p.Column,p.Row,(float)p.X,(float)p.Y)).ToArray());
  var projected=new NImageLattice(local.PitchX,local.PitchY,points,quads){SourceGrid=context};
  return DecodeSource(image,boardType,projected,execution,token);
 }
 static NImageDetectedBoard? DetectImage(GrayImage image,int boardType,int filter,CancellationToken token,NImageSourceExecution execution){
  var lattice=NImageSourceLatticeAdapter.FindSource(image,token,boardType is 6 or 8?1:2,filter);
  return lattice is null?null:DecodeSource(image,boardType,lattice,execution,token);
 }
 static NImageDetectedBoard? DecodeSource(GrayImage image,int boardType,NImageLattice lattice,NImageSourceExecution execution,CancellationToken token){
  var result=NImageSourceRecognition.RecognizeLattice(image,boardType,lattice,execution,token);
  if(result.UnsupportedReason is not null)throw new NotSupportedException(result.UnsupportedReason);
  return result.Board;
 }

 public static MapCalibrationImageResult SolveBoards(NImageDetectedBoard a,NImageDetectedBoard b,int freedom,int weight,float nativeWeightFactor){
  (float,float,float,float)[] Pairs(NImageDetectedBoard board)=>board.Points.Select(p=>((float)p.X,(float)p.Y,(float)p.IndexX*board.GridPitch,(float)p.IndexY*board.GridPitch)).ToArray();
  MapCalibrationImagePoint[] Points(NImageDetectedBoard board)=>board.Points.Select(p=>new MapCalibrationImagePoint((float)p.X,(float)p.Y,p.IndexX,p.IndexY)).ToArray();
  var source=Points(a);var target=Points(b);var ah=NPointSolver.FitNativePoints(Pairs(a),freedom,weight,nativeWeightFactor);var bh=NPointSolver.FitNativePoints(Pairs(b),freedom,weight,nativeWeightFactor);
  if(ah.ErrorCode!=0||bh.ErrorCode!=0)return new(ah.ErrorCode!=0?ah.ErrorCode:bh.ErrorCode,[],0,0,0,0,0,source,target,[]);
  var fit=MapCalibrationSolver.ComposeImageFits(ah.Matrix!,bh.Matrix!);if(fit.ErrorCode!=0)return new(fit.ErrorCode,[],0,0,0,0,0,source,target,[]);
  var evaluation=MapCalibrationImageEvaluator.Evaluate(fit.Matrix,source,target);
  return new(evaluation.ErrorCode,fit.Matrix,fit.XRatio,fit.YRatio,MapCalibrationSolver.ImagePixelScale(ah.Matrix!),MapCalibrationSolver.ImagePixelScale(bh.Matrix!),evaluation.MapError,source,target,evaluation.Projection);
 }
}
