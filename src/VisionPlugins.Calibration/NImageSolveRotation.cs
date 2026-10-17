namespace VisionPlugins.Calibration;

public sealed record NImageRotationSolveResult(uint ErrorCode,double CenterX,double CenterY,double AverageRadialError,double MaximumRadialError,int MaximumErrorFrame,IReadOnlyList<double> AnglesDegrees,int Direction,double AverageAngleError,double MaximumAngleError);
/// <summary>Independent 7cf3f0/7cab10 branch: fit perpendicular bisectors of
/// adjacent matched coded corners, then measure radius and angle consistency.</summary>
public static class NImageSolveRotation {
 const float FloatEpsilon=1.1920928955078125e-7f;
 public static NImageRotationSolveResult Solve(IReadOnlyList<NImageSolveFrame> frames){
  NImageRotationSolveResult Failure(uint code)=>new(code,0,0,0,0,0,[],0,0,0);if(frames.Count is <2 or >9)return Failure(0x10000019);
  var pairs=new List<(float X,float Y,float U,float V)>[frames.Count-1];double slopes=0,intercepts=0,products=0,squares=0;int equations=0;
  for(int i=0;i<pairs.Length;i++){pairs[i]=[];var next=frames[i+1].Board.Points.ToDictionary(p=>(p.IndexX,p.IndexY));foreach(var p in frames[i].Board.Points)if(next.TryGetValue((p.IndexX,p.IndexY),out var q)){float x=(float)p.X,y=(float)p.Y,u=(float)q.X,v=(float)q.Y;pairs[i].Add((x,y,u,v));float dx=u-x,dy=v-y;if(Math.Abs(dx)<FloatEpsilon&&Math.Abs(dy)<FloatEpsilon||Math.Abs(dy)<1e-4f)continue;float mx=(x+u)*.5f,my=(y+v)*.5f,k=-dx/dy,b=my-k*mx;slopes+=k;intercepts+=b;products+=(double)b*k;squares+=k*k;equations++;}}
  if(equations<2)return Failure(0x10103006);double determinant=squares*equations-slopes*slopes;if(Math.Abs(determinant)<2.220446049250313e-16)return Failure(0x1010305a);double preciseX=(slopes*intercepts-products*equations)/determinant;float cx=(float)preciseX,cy=(float)(slopes*preciseX/equations+intercepts/equations);
  float total=0,max=0;int totalPairs=0,maxIndex=0,directionVotes=0;double angleError=0,angleMax=0;var angles=new List<double>();
  for(int i=0;i<pairs.Length;i++){float sum=0;double angle=0;foreach(var p in pairs[i]){float x=p.X-cx,y=p.Y-cy,u=p.U-cx,v=p.V-cy;sum+=Math.Abs(MathF.Sqrt(x*x+y*y)-MathF.Sqrt(u*u+v*v));double dx=x,dy=y,du=u,dv=v;double cosine=(dx*du+dy*dv)/(Math.Sqrt(dx*dx+dy*dy)*Math.Sqrt(du*du+dv*dv)+2.220446049250313e-16);double theta=Math.Acos(Math.Clamp(cosine,-1,1));if(dx*dv-dy*du<0)theta=-theta;angle+=theta;}
   float average=sum/(pairs[i].Count+FloatEpsilon);if(average>max){max=average;maxIndex=i;}total+=sum;totalPairs+=pairs[i].Count;double degrees=angle/(pairs[i].Count+2.220446049250313e-16)*180/Math.PI;angles.Add(degrees);double expected=frames[i+1].Pose.R-frames[i].Pose.R;if(expected>180)expected-=360;if(expected< -180)expected+=360;double error=Math.Abs(degrees-expected);angleError+=error;angleMax=Math.Max(angleMax,error);directionVotes+=degrees*expected>0?1:-1;
  }
  return new(0,cx,cy,total/(totalPairs+FloatEpsilon),max,maxIndex,angles.AsReadOnly(),directionVotes<0?-1:1,angleError/pairs.Length,angleMax);
 }
 public static IReadOnlyList<double> CenterMatrix(IReadOnlyList<double> translationMatrix,NImageRotationSolveResult rotation){if(translationMatrix.Count!=9||rotation.ErrorCode!=0)throw new ArgumentException("Valid matrix and rotation center required.");var h=translationMatrix.ToArray();h[2]=-(float)((float)h[0]*(float)rotation.CenterX+(float)h[1]*(float)rotation.CenterY);h[5]=-(float)((float)h[3]*(float)rotation.CenterX+(float)h[4]*(float)rotation.CenterY);return Array.AsReadOnly(h);}
}
