using VisionPlugins.Calibration;

namespace VisionPlugins.CalibrationBoard;

public sealed record BoardFitStages(uint Error,float[]? NormalizedGridToImage,float[]? ImageToWorld,float InitialPixelScale);

/// <summary>Public board calibration branch of 7ed0c0; both fits retain the user-selected homography model.</summary>
public static class CalibrationBoardFitting
{
 public static BoardFitStages Fit(IReadOnlyList<NImageBoardPoint> points,BoardCalibrationOptions options,double? encodedPitch=null,float referenceX=0,float referenceY=0)
 {
  if(points.Count<4||options.Freedom is <1 or >3||options.Weight is <1 or >4)return new(0x10000019,null,null,0);
  var recoded=CalibrationBoardFitGeometry.Recode(points);
  // Public SDK captures pass float reference(0,0), independently of world origin.
  int anchor=0;float nearest=float.MaxValue,cx=referenceX,cy=referenceY;
  for(int i=0;i<points.Count;i++){float dx=(float)points[i].X-cx,dy=(float)points[i].Y-cy,d=dx*dx+dy*dy;if(d<nearest){nearest=d;anchor=i;}}
  int adjacent=0;float distance=float.MaxValue;
  foreach(var delta in new[]{(-1,0),(1,0),(0,-1),(0,1)})
  {
   for(int i=0;i<points.Count;i++)if(recoded[i].X==recoded[anchor].X+delta.Item1&&recoded[i].Y==recoded[anchor].Y+delta.Item2)
   {float dx=(float)points[i].X-(float)points[anchor].X,dy=(float)points[i].Y-(float)points[anchor].Y,d=MathF.Sqrt(dx*dx+dy*dy);if(d<distance){distance=d;adjacent=i;}break;}
  }
  float pitch=(float)(encodedPitch??options.PhysicalSize);
  float wx=recoded[anchor].X*pitch-recoded[adjacent].X*pitch,wy=recoded[anchor].Y*pitch-recoded[adjacent].Y*pitch;
  float worldDistance=MathF.Sqrt(wy*wy+wx*wx),scale=1;
  if(worldDistance>1.1920928955078125e-7f){float dx=(float)points[anchor].X-(float)points[adjacent].X,dy=(float)points[anchor].Y-(float)points[adjacent].Y;scale=MathF.Sqrt(dy*dy+dx*dx)/worldDistance;}
  float originX=recoded[anchor].X*pitch,originY=recoded[anchor].Y*pitch;
  var initial=points.Select((p,i)=>((recoded[i].X*pitch-originX)*scale+(float)points[anchor].X,(recoded[i].Y*pitch-originY)*scale+(float)points[anchor].Y,(float)p.X,(float)p.Y)).ToArray();
  int weight=options.Weight==4?3:options.Weight;
  (uint ErrorCode,float[]? Matrix) FitPairs(IReadOnlyList<(float,float,float,float)> pairs)=>weight==1?NPointSolver.FitNativeLeastSquaresPoints(pairs,options.Freedom):NPointSolver.FitNativePoints(pairs,options.Freedom,weight,(float)options.WeightFactor*.1f,20,1e-4f);
  var first=FitPairs(initial);if(first.ErrorCode!=0)return new(first.ErrorCode,null,null,scale);
  var pairs=CalibrationBoardCoordinates.Build(points,options,pitch).Select(p=>((float)p.ImageX,(float)p.ImageY,(float)p.WorldX,(float)p.WorldY)).ToArray();
  var second=FitPairs(pairs);return new(second.ErrorCode,first.Matrix,second.Matrix,scale);
 }
 public static NPointSolveResult Analyze(IReadOnlyList<NImageBoardPoint> points,BoardCalibrationOptions options,BoardFitStages stages,double? encodedPitch=null)
 {
  if(stages.Error!=0||stages.ImageToWorld is null)return new(){Status=1,ErrorCode=stages.Error};
  var matrix=stages.ImageToWorld;
  var pairs=CalibrationBoardCoordinates.Build(points,options,encodedPitch??options.PhysicalSize).Select(p=>((float)p.ImageX,(float)p.ImageY,(float)p.WorldX,(float)p.WorldY)).ToArray();
  var errors=options.Weight==1?NPointSolver.AnalyzeNativeLeastSquaresMatrix(pairs,matrix):NPointSolver.AnalyzeNativeIrlsMatrix(pairs,matrix);
  var decomposition=NPointSolver.MatrixToNativeParameters(matrix);
  if(errors.ErrorCode!=0||decomposition.ErrorCode!=0)return new(){Status=1,ErrorCode=errors.ErrorCode!=0?errors.ErrorCode:decomposition.ErrorCode};
  var p=decomposition.Parameters!.Value;
  // SDK pixel precision is a distinct result from its six-value decomposition.
  // SDK003fc0 -> process6a -> 01fcb0 -> 7c6ba0 command4 -> 7f5b80.
  // Normalize each coefficient before its row norm; dividing the norm afterward rounds differently.
  const float epsilon=1.1920928955078125e-7f;
  float reciprocal=1f/(matrix[8]+epsilon),a=matrix[0]*reciprocal,b=matrix[1]*reciprocal,c=matrix[3]*reciprocal,d=matrix[4]*reciprocal;
  float precision=(MathF.Sqrt(a*a+b*b)+MathF.Sqrt(c*c+d*d))*.5f;
  return new(){Status=0,ErrorCode=0,Matrix=matrix.Select(v=>(double)v).ToArray(),OffsetError=errors.MeanInverseError,OffsetMaxError=errors.PointErrors!.Max(),Scale=p.Scale,DeltaX=p.DeltaX,DeltaY=p.DeltaY,Theta=p.Theta,Skew=p.Skew,Aspect=p.Aspect,PixelPrecision=precision};
 }
}
