namespace VisionPlugins.Calibration;

public sealed record NImageRotationSolveResult(uint ErrorCode,double CenterX,double CenterY,double AverageRadialError,double MaximumRadialError,int MaximumErrorFrame,IReadOnlyList<double> AnglesDegrees,int Direction,double AverageAngleError,double MaximumAngleError){public IReadOnlyList<double> AnglesRadians{get;init;}=Array.Empty<double>();}
/// <summary>Independent 7cf3f0/7cab10 branch: fit perpendicular bisectors of
/// adjacent matched coded corners, then measure radius and angle consistency.</summary>
public static class NImageSolveRotation {
 const float FloatEpsilon=1.1920928955078125e-7f;
 public static NImageRotationSolveResult Solve(IReadOnlyList<NImageSolveFrame> frames){
  NImageRotationSolveResult Failure(uint code)=>new(code,0,0,0,0,0,[],0,0,0);if(frames.Count is <2 or >9)return Failure(0x10000019);
  var pairs=new List<(float X,float Y,float U,float V)>[frames.Count-1];double slopes=0,intercepts=0,products=0,squares=0;int equations=0;
  for(int i=0;i<pairs.Length;i++){pairs[i]=[];foreach(var match in Match(frames[i].Board.Points,frames[i+1].Board.Points)){float x=(float)match.Before.X,y=(float)match.Before.Y,u=(float)match.After.X,v=(float)match.After.Y;pairs[i].Add((x,y,u,v));float dx=u-x,dy=v-y;if(Math.Abs(dx)<FloatEpsilon&&Math.Abs(dy)<FloatEpsilon||Math.Abs(dy)<1e-4f)continue;float mx=(x+u)*.5f,my=(y+v)*.5f,k=-dx/dy,b=my-k*mx;slopes+=k;intercepts+=b;products+=(double)b*k;squares+=k*k;equations++;}}
  if(equations<2)return Failure(0x10103006);double determinant=squares*equations-slopes*slopes;if(Math.Abs(determinant)<2.220446049250313e-16)return Failure(0x1010305a);double preciseX=(slopes*intercepts-products*equations)/determinant;float cx=(float)preciseX,cy=(float)(slopes*preciseX/equations+intercepts/equations);
  float total=0,max=0;int totalPairs=0,maxIndex=0,directionVotes=0;float angleError=0,angleMax=0;var angles=new List<double>();var radians=new List<double>();
  for(int i=0;i<pairs.Length;i++){float sum=0;double angle=0;foreach(var p in pairs[i]){float x=p.X-cx,y=p.Y-cy,u=p.U-cx,v=p.V-cy;sum+=Math.Abs(MathF.Sqrt(x*x+y*y)-MathF.Sqrt(u*u+v*v));double dx=x,dy=y,du=u,dv=v;double cosine=(dx*du+dy*dv)/(Math.Sqrt(dx*dx+dy*dy)*Math.Sqrt(du*du+dv*dv)+2.220446049250313e-16);double theta=Math.Acos(Math.Clamp(cosine,-1,1));if(dx*dv-dy*du<0)theta=-theta;angle+=theta;}
   float average=sum/(pairs[i].Count+FloatEpsilon);if(average>max){max=average;maxIndex=i;}total+=sum;totalPairs+=pairs[i].Count;float rad=(float)(angle/(pairs[i].Count+2.220446049250313e-16));radians.Add(rad);float degrees=rad*57.295780181884766f;angles.Add(degrees);float expected=(float)frames[i+1].Pose.R-(float)frames[i].Pose.R;if(expected>180)expected-=360;if(expected< -180)expected+=360;float error=Math.Abs(degrees-expected);angleError+=error;angleMax=Math.Max(angleMax,error);directionVotes+=degrees*expected>0?1:-1;
  }
  return new(0,cx,cy,total/(totalPairs+FloatEpsilon),max,maxIndex,angles.AsReadOnly(),directionVotes<0?-1:1,angleError/(pairs.Length+FloatEpsilon),angleMax){AnglesRadians=radians.AsReadOnly()};
 }
 public static IReadOnlyList<double> CenterMatrix(IReadOnlyList<double> translationMatrix,NImageRotationSolveResult rotation){if(translationMatrix.Count!=9||rotation.ErrorCode!=0)throw new ArgumentException("Valid matrix and rotation center required.");var h=translationMatrix.ToArray();h[2]=-(float)((float)h[0]*(float)rotation.CenterX+(float)h[1]*(float)rotation.CenterY);h[5]=-(float)((float)h[3]*(float)rotation.CenterX+(float)h[4]*(float)rotation.CenterY);return Array.AsReadOnly(h);}
 // Original 7cb4c0 preserves every previous-frame corner. Missing world IDs in
 // the next frame are extrapolated from its nearest ID and observed grid axes,
 // rather than discarding them by a global/all-frame intersection.
 static IEnumerable<(NImageBoardPoint Before,NImageBoardPoint After)> Match(IReadOnlyList<NImageBoardPoint> before,IReadOnlyList<NImageBoardPoint> after){
  if(after.Count==0)yield break;float xLength=0,yLength=0,xx=0,xy=0,yx=0,yy=0;int nx=0,ny=0;
  for(int i=0;i<after.Count;i++)for(int j=i+1;j<after.Count;j++){var a=after[i];var b=after[j];int dx=b.IndexX-a.IndexX,dy=b.IndexY-a.IndexY;float vx=(float)b.X-(float)a.X,vy=(float)b.Y-(float)a.Y;if(Math.Abs(dx)==1&&dy==0){xLength+=MathF.Sqrt(vx*vx+vy*vy);xx+=dx==1?vx:-vx;xy+=dx==1?vy:-vy;nx++;}else if(dx==0&&Math.Abs(dy)==1){yLength+=MathF.Sqrt(vx*vx+vy*vy);yx+=dy==1?vx:-vx;yy+=dy==1?vy:-vy;ny++;}}
  float xn=MathF.Sqrt(xx*xx+xy*xy),yn=MathF.Sqrt(yx*yx+yy*yy);
  foreach(var p in before){var q=after[0];long minimum=long.MaxValue;foreach(var candidate in after){long dx=(long)p.IndexX-candidate.IndexX,dy=(long)p.IndexY-candidate.IndexY,distance=dx*dx+dy*dy;if(distance<minimum){minimum=distance;q=candidate;}}
   if(minimum==0){yield return(p,q);continue;}
   if(nx==0||ny==0||xn==0||yn==0)continue;float dxp=(p.IndexX-q.IndexX)*(xLength/nx),dyp=(p.IndexY-q.IndexY)*(yLength/ny);float u=(xx/xn)*dxp+(float)q.X+(yx/yn)*dyp,v=dxp*(xy/xn)+(float)q.Y+dyp*(yy/yn);yield return(p,new(p.IndexX,p.IndexY,u,v));
  }
 }
}
