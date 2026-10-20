namespace VisionPlugins.MapCalibration;

public readonly record struct MapCalibrationImagePoint(float X,float Y,int WorldX,int WorldY);
public sealed record MapCalibrationImageEvaluation(uint ErrorCode,float MapError,IReadOnlyList<MapCalibrationImagePoint> Projection);

/// <summary>Image-mode 7d3e90 ID matching and symmetric mapping error; no point detection or fit.</summary>
public static class MapCalibrationImageEvaluator {
 public static MapCalibrationImageEvaluation Evaluate(IReadOnlyList<float> h,IReadOnlyList<MapCalibrationImagePoint> source,IReadOnlyList<MapCalibrationImagePoint> target){
  if(h.Count!=9)throw new ArgumentException("Nine matrix coefficients required.");
  float p=h[0]*h[4],q=h[1]*h[5];float det=(((h[8]*p+h[6]*q+h[7]*h[2]*h[3])-h[7]*h[0]*h[5])-h[8]*h[1]*h[3])-h[6]*h[4]*h[2];
  if(!(Math.Abs(det)>=2.220446049250313e-16))return new(0x10103002,0,[]);
  float k=1f/det;float[] inverse=[(h[4]*h[8]-h[5]*h[7])*k,(h[2]*h[7]-h[8]*h[1])*k,(q-h[4]*h[2])*k,(h[5]*h[6]-h[8]*h[3])*k,(h[8]*h[0]-h[2]*h[6])*k,(h[2]*h[3]-h[0]*h[5])*k,(h[3]*h[7]-h[4]*h[6])*k,(h[1]*h[6]-h[0]*h[7])*k,(p-h[1]*h[3])*k];
  const float epsilon=1.1920928955078125e-7f;var projection=new List<MapCalibrationImagePoint>();float sum=0;int count=0;
  foreach(var point in source){foreach(var other in target){if(point.WorldX!=other.WorldX||point.WorldY!=other.WorldY)continue;
   float denominator=point.X*h[6]+point.Y*h[7]+h[8];if(!(epsilon<MathF.Abs(denominator)))return new(0x10103050,float.MaxValue,projection);
   float x=(h[0]*point.X+h[1]*point.Y+h[2])/denominator,y=(h[3]*point.X+h[4]*point.Y+h[5])/denominator;projection.Add(new(x,y,point.WorldX,point.WorldY));
   float dx=other.X-x,dy=other.Y-y;float back=other.X*inverse[6]+other.Y*inverse[7]+inverse[8];if(!(epsilon<MathF.Abs(back)))return new(0x10103050,float.MaxValue,projection);
   float by=point.Y-(inverse[3]*other.X+inverse[4]*other.Y+inverse[5])/back,bx=point.X-(inverse[0]*other.X+inverse[1]*other.Y+inverse[2])/back;
   sum=sum+MathF.Sqrt(dx*dx+dy*dy)+MathF.Sqrt(bx*bx+by*by);count+=2;break;
  }}
  float error=count<1?0:sum/((float)count+epsilon);return new(error>=1.5f?0x10103050u:0,error,projection);
 }
}
