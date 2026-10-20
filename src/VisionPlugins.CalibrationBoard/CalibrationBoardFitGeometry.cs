using VisionPlugins.Calibration;

namespace VisionPlugins.CalibrationBoard;

/// <summary>7ed0c0 pre-fit index recoding. Independently implemented; not yet wired into production fitting.</summary>
public static class CalibrationBoardFitGeometry
{
 public static (int X,int Y)[] Recode(IReadOnlyList<NImageBoardPoint> points)
 {
  float min=float.MaxValue,max=0,horizontalAngle=0,horizontalY=0,verticalAngle=0,verticalY=0,step=0;
  for(int i=0;i<points.Count;i++)for(int j=i+1;j<points.Count;j++)if(Math.Abs(points[j].IndexX-points[i].IndexX)==1&&points[i].IndexY==points[j].IndexY)
  {
   float dx=(float)points[i].X-(float)points[j].X,dy=(float)points[i].Y-(float)points[j].Y;
   float distance=MathF.Sqrt(MathF.Abs(dx)*MathF.Abs(dx)+MathF.Abs(dy)*MathF.Abs(dy));
   min=MathF.Min(min,distance);max=MathF.Max(max,distance);horizontalAngle=MathF.Acos(MathF.Abs(dx)/distance);
   horizontalY=points[j].IndexX<points[i].IndexX?dy:-dy;break;
  }
  if((max-min)/min>.1f)return points.Select(p=>(p.IndexX,p.IndexY)).ToArray();
  for(int i=0;i<points.Count;i++)for(int j=i+1;j<points.Count;j++)if(Math.Abs(points[j].IndexY-points[i].IndexY)==1&&points[i].IndexX==points[j].IndexX)
  {
   float dx=(float)points[i].X-(float)points[j].X,dy=(float)points[i].Y-(float)points[j].Y;
   step=MathF.Sqrt(MathF.Abs(dx)*MathF.Abs(dx)+MathF.Abs(dy)*MathF.Abs(dy));verticalAngle=MathF.Acos(MathF.Abs(dx)/step);
   verticalY=points[j].IndexY<points[i].IndexY?dy:-dy;break;
  }
  float angle=verticalAngle<=horizontalAngle?(verticalY<0?verticalAngle:-verticalAngle):(horizontalY>=0?-horizontalAngle:horizontalAngle);
  float c=MathF.Cos(angle),s=MathF.Sin(angle),left=float.MaxValue,top=float.MaxValue;
  var rotated=points.Select(p=>((float)p.X*c-(float)p.Y*s,(float)p.X*s+(float)p.Y*c)).ToArray();
  foreach(var p in rotated){left=MathF.Min(left,p.Item1);top=MathF.Min(top,p.Item2);}
  int anchor=0,best=999999;
  // Original cvttss2si + xor/sub absolute value preserve int.MinValue on
  // missing axis neighbors (step0); Math.Abs(int.MinValue) would throw.
  int Round(float v){float rounded=v>=0?v+.5f:v-.5f;return !float.IsFinite(rounded)||rounded>=2147483648f||rounded< -2147483648f?int.MinValue:unchecked((int)rounded);}
  int AbsUnchecked(int v)=>v<0?unchecked(-v):v;
  for(int i=0;i<rotated.Length;i++){int distance=unchecked(AbsUnchecked(Round((rotated[i].Item1-left)/step))+AbsUnchecked(Round((rotated[i].Item2-top)/step)));if(distance<best){best=distance;anchor=i;}}
  bool transpose=false;
  for(int i=0;i<points.Count;i++)for(int j=i+1;j<points.Count;j++)if(points[i].IndexX==points[j].IndexX&&Math.Abs(points[i].IndexY-points[j].IndexY)==1){transpose=MathF.Abs(rotated[i].Item2-rotated[j].Item2)<MathF.Abs(rotated[i].Item1-rotated[j].Item1);break;}
  return points.Select((p,i)=>
  {
   int x=Math.Abs(p.IndexX-points[anchor].IndexX),y=Math.Abs(p.IndexY-points[anchor].IndexY);if(transpose)(x,y)=(y,x);
   return(rotated[i].Item1<=rotated[anchor].Item1?-x:x,rotated[i].Item2<=rotated[anchor].Item2?-y:y);
  }).ToArray();
 }
}
