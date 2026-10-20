using VisionCore;
using VisionPlugins.Calibration;
using VisionPlugins.CalibrationBoard;
namespace VisionPlugins.DistortionCalibration;
public readonly record struct DistortionCalibrationPoint(float X,float Y,int GridX,int GridY);
public sealed record DistortionCalibrationSolveResult(uint ErrorCode,DistortionCalibrationFile? File,float EstimationError){public float[]? PointErrors{get;init;}}
public delegate double[]? DistortionFixedFit(IReadOnlyList<NPointPair> points,int freedom,int weight,double weightFactor);

/// <summary>Source-backed three-model payload assembly. Arbitrary rotated/recoded board-grid
/// preprocessing and final error evaluation remain separately verified work.</summary>
public static class DistortionCalibrationSolver {
 /// <summary>Complete source-default estimation stages. World layout is the observed
 /// distortion tool's fixed (origin0, rotation0, positive Y, pitch1) configuration.
 /// Evaluation/redetection and dirty native scratch context remain separate work.</summary>
 public static DistortionCalibrationSolveResult SolveFromPointsNativeDetailed(IReadOnlyList<DistortionCalibrationPoint> points,int width,int height,int model,int freedom,int weight,double weightFactor,Point2 correctCenter,CancellationToken token=default){
  uint fitError=0;IReadOnlyList<NPointPair>? lastPairs=null;float[]? lastMatrix=null;
  double[]? Fit(IReadOnlyList<NPointPair> pairs,int f,int w,double factor){
   var result=NativeFit(pairs,f,w,factor);fitError=result.ErrorCode;lastPairs=pairs;lastMatrix=result.Matrix;
   return result.Matrix?.Select(v=>(double)v).ToArray();
  }
  try{
   var file=SolveFromPoints(points,width,height,model,freedom,weight,weightFactor,correctCenter,Fit,token,1f);
   if(model is 1 or 2){
    var values=file.Values.ToArray();var center=new Point2(values[10],values[11]);var radial=values.Skip(12).Take(4).ToArray();
    var pairs=points.Select(p=>{var pixel=model==1?new Point2(p.X,p.Y):DistortionCalibrationInverseRadial.Solve(new(p.X,p.Y),center,radial);return new NPointPair((float)pixel.X,(float)pixel.Y,p.GridX,p.GridY);}).ToArray();
    if(Fit(pairs,freedom,weight,weightFactor) is null)return new(fitError,null,float.NaN);
   }
   var nativePairs=lastPairs!.Select(p=>((float)p.ImageX,(float)p.ImageY,(float)p.WorldX,(float)p.WorldY)).ToArray();
   var analysis=weight==1?NPointSolver.AnalyzeNativeLeastSquaresMatrix(nativePairs,lastMatrix!):NPointSolver.AnalyzeNativeIrlsMatrix(nativePairs,lastMatrix!);
   return analysis.ErrorCode==0?new DistortionCalibrationSolveResult(0,file,analysis.MeanInverseError){PointErrors=analysis.PointErrors}:new(analysis.ErrorCode,null,float.NaN);
  }catch(DistortionCalibrationRadialFitFailure failure){return new(failure.NativeErrorCode,null,float.NaN);}catch(InvalidOperationException)when(fitError!=0){return new(fitError,null,float.NaN);}
 }
 static (uint ErrorCode,float[]? Matrix) NativeFit(IReadOnlyList<NPointPair> pairs,int freedom,int weight,double factor){
  var values=pairs.Select(p=>((float)p.ImageX,(float)p.ImageY,(float)p.WorldX,(float)p.WorldY)).ToArray();
  return weight==1?NPointSolver.FitNativeLeastSquaresPoints(values,freedom):NPointSolver.FitNativePoints(values,freedom,weight,(float)factor*.1f,20,1e-4f);
 }
 /// <summary>Production fixed-kernel adapter. WeightFactor retains the UI value and is
 /// converted once to the native factor. Fixed-kernel numerical differences remain documented.</summary>
 public static DistortionCalibrationFile SolveFromPointsNative(IReadOnlyList<DistortionCalibrationPoint> points,int width,int height,int model,int freedom,int weight,double weightFactor,Point2 correctCenter,CancellationToken token=default,float gridPitch=1){
  double[]? Fit(IReadOnlyList<NPointPair> pairs,int f,int w,double factor){
   var result=NativeFit(pairs,f,w,factor);
   return result.Matrix?.Select(v=>(double)v).ToArray();
  }
  return SolveFromPoints(points,width,height,model,freedom,weight,weightFactor,correctCenter,Fit,token,gridPitch);
 }
 public static DistortionCalibrationFile SolveFromPoints(IReadOnlyList<DistortionCalibrationPoint> points,int width,int height,int model,int freedom,int weight,double weightFactor,Point2 correctCenter,DistortionFixedFit fit,CancellationToken token=default,float gridPitch=1){
  if(points.Count<4||width<64||height<64||model is <1 or >3)throw new ArgumentException("Invalid distortion calibration input.");
  var observed=points.Select(p=>new Point2(p.X,p.Y)).ToArray();var indices=points.Select(p=>new Point2(p.GridX,p.GridY)).ToArray();var recoded=CalibrationBoardFitGeometry.Recode(points.Select(p=>new NImageBoardPoint(p.GridX,p.GridY,p.X,p.Y)).ToArray());var grid=recoded.Select(p=>new Point2(p.X*gridPitch,p.Y*gridPitch)).ToArray();
  double[] Fit(IReadOnlyList<Point2> source,IReadOnlyList<Point2> destination)=>fit(source.Select((p,i)=>new NPointPair((float)p.X,(float)p.Y,(float)destination[i].X,(float)destination[i].Y)).ToArray(),freedom,weight,weightFactor)??throw new InvalidOperationException("Original fixed calibration fit failed.");
  double[] h=[1,0,0,0,1,0,0,0,1];float[] radial=[0,0,0,0];Point2 center=new(0,0);Point2[] undistorted=observed;
  if(model!=1){var refined=DistortionCalibrationRadialFit.FromSeed(indices,observed,Fit(indices,observed),width,height,freedom,token);radial=refined.Radial;center=refined.Center;undistorted=observed.Select(p=>DistortionCalibrationInverseRadial.Solve(p,center,radial)).ToArray();}
  if(model!=2){int anchor=0;float nearest=float.MaxValue;for(int i=0;i<points.Count;i++){float dx=points[i].X-(float)correctCenter.X,dy=points[i].Y-(float)correctCenter.Y,d=dx*dx+dy*dy;if(d<nearest){nearest=d;anchor=i;}}
   int neighbor=0;float minimum=float.MaxValue;foreach(var step in new[]{(-1,0),(1,0),(0,-1),(0,1)})for(int i=0;i<points.Count;i++)if(recoded[i].X==recoded[anchor].X+step.Item1&&recoded[i].Y==recoded[anchor].Y+step.Item2){float dx=points[i].X-points[anchor].X,dy=points[i].Y-points[anchor].Y,d=MathF.Sqrt(dx*dx+dy*dy);if(d<minimum){minimum=d;neighbor=i;}break;}
   float gx=(float)grid[anchor].X-(float)grid[neighbor].X,gy=(float)grid[anchor].Y-(float)grid[neighbor].Y,gridDistance=MathF.Sqrt(gx*gx+gy*gy),px=(float)undistorted[anchor].X-(float)undistorted[neighbor].X,py=(float)undistorted[anchor].Y-(float)undistorted[neighbor].Y;float scale=gridDistance>1.1920929e-7f?MathF.Sqrt(px*px+py*py)/gridDistance:1;
   var ideal=grid.Select(p=>new Point2(((float)p.X-(float)grid[anchor].X)*scale+(float)undistorted[anchor].X,((float)p.Y-(float)grid[anchor].Y)*scale+(float)undistorted[anchor].Y)).ToArray();h=Fit(ideal,undistorted);
  }
  return new DistortionCalibrationFile(new[]{2f}.Concat(h.Select(v=>(float)v)).Concat(new[]{(float)center.X,(float)center.Y}).Concat(radial).Concat(new[]{(float)width,(float)height,(float)model}));
 }
}


