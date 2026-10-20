namespace VisionPlugins.CameraMapping;

public readonly record struct CameraMappingPair(double ObjectX,double ObjectY,double TargetX,double TargetY);
public sealed record CameraMappingSolution(uint ErrorCode,double[] Matrix,double RatioX=0,double RatioY=0) {
 public bool Success=>ErrorCode==0;
}

/// <summary>Independent managed implementation. SDK differential coverage and remaining floating-point/robust differences are documented separately.</summary>
public static class CameraMappingSolver {
 public static CameraMappingSolution Solve(IReadOnlyList<CameraMappingPair> points,int freedom=1,int weight=3,int factor=20) {
  if(freedom is <1 or >3)return Error(0x10103026);
  if(weight<1)return Error(0x10103027);
  if(factor is <1 or >200)return Error(0x10103028);
  if(points.Count is <2 or >32)return Error(0x80100005);
  if(points.Any(p=>!new[]{p.ObjectX,p.ObjectY,p.TargetX,p.TargetY}.All(double.IsFinite)))return Error(0x80100006);
  // Original SDK uses inverse two-point similarity irrespective of requested freedom;
  // three-point input uses forward affine irrespective of requested freedom.
  var p=points.Select(x=>new CameraMappingPair((float)x.ObjectX,(float)x.ObjectY,(float)x.TargetX,(float)x.TargetY)).ToArray();
  if(p.Length==2)p=p.Select(x=>new CameraMappingPair(x.TargetX,x.TargetY,x.ObjectX,x.ObjectY)).ToArray();
  int model=p.Length==2?3:p.Length==3?2:freedom;
  // SDK rejects collinear 3+ sets, even similarity where the algebra alone could fit.
  if(p.Length>2&&(!Noncollinear(p)||p.Length>3&&!Noncollinear(p.Select(x=>new CameraMappingPair(x.TargetX,x.TargetY,x.ObjectX,x.ObjectY)).ToArray())))return Error(p.Length==3?0x10103002u:0x10103051u);
  var h=p.Length==2?FitTwo(p):p.Length>=4?FitNormalized(p,model,weight,factor):FitLinear(p,model,Enumerable.Repeat(1d,p.Length).ToArray());if(h is null)return Error(p.Length<=3?0x10103002u:0x10103051u);
  h=h.Select(x=>(double)(float)x).ToArray();return new(0,h,MathF.Sqrt((float)h[0]*(float)h[0]+(float)h[3]*(float)h[3]),MathF.Sqrt((float)h[1]*(float)h[1]+(float)h[4]*(float)h[4]));
 }
 static CameraMappingSolution Error(uint error)=>new(error,[]);
 // Dedicated two-point similarity, rather than a normal-equation fit. A constant
 // destination therefore retains the original exact zero scale and singular matrix.
 static double[]? FitTwo(CameraMappingPair[] p){float x=(float)p[0].ObjectX,y=(float)p[0].ObjectY,u=(float)p[0].TargetX,v=(float)p[0].TargetY;float dx=(float)p[1].ObjectX-x,dy=(float)p[1].ObjectY-y,du=(float)p[1].TargetX-u,dv=(float)p[1].TargetY-v;float d=dx*dx+dy*dy;if(d==0)return null;float a=(du*dx+dv*dy)/d,b=(dv*dx-du*dy)/d;return[a,-b,u-(a*x-b*y),b,a,v-(b*x+a*y),0,0,1];}
 static bool Noncollinear(CameraMappingPair[] points){for(int i=1;i<points.Length-1;i++)for(int j=i+1;j<points.Length;j++)if(Math.Abs((points[i].ObjectX-points[0].ObjectX)*(points[j].ObjectY-points[0].ObjectY)-(points[j].ObjectX-points[0].ObjectX)*(points[i].ObjectY-points[0].ObjectY))>1e-10)return true;return false;}
 static double Residual(double[] h,CameraMappingPair p){var t=Transform(h,p.ObjectX,p.ObjectY);return Math.Sqrt(Math.Pow(t.X-p.TargetX,2)+Math.Pow(t.Y-p.TargetY,2));}
 public static (double X,double Y) Transform(IReadOnlyList<double> h,double x,double y){double z=h[6]*x+h[7]*y+h[8];return((h[0]*x+h[1]*y+h[2])/z,(h[3]*x+h[4]*y+h[5])/z);}
 // Source kernel 0x7ff3c0 / 0x7fd4e0 / 0x7fec40: normalize both clouds,
 // inverse reprojection residual, upper median scale, fixed clipping/iteration semantics.
 static double[]? FitNormalized(CameraMappingPair[] points,int freedom,int weight,int factor){
  float mx=0,my=0,mu=0,mv=0;foreach(var p in points){mx+=(float)p.ObjectX;my+=(float)p.ObjectY;mu+=(float)p.TargetX;mv+=(float)p.TargetY;}float reciprocal=1f/points.Length;mx*=reciprocal;my*=reciprocal;mu*=reciprocal;mv*=reciprocal;
  float ds=0,dt=0;foreach(var p in points){float x=(float)p.ObjectX-mx,y=(float)p.ObjectY-my,u=(float)p.TargetX-mu,v=(float)p.TargetY-mv;ds+=MathF.Sqrt(x*x+y*y);dt+=MathF.Sqrt(u*u+v*v);}float ss=ds>0?1/(ds*reciprocal):1,st=dt>0?1/(dt*reciprocal):1;
  var normalized=points.Select(p=>new CameraMappingPair(((float)p.ObjectX-mx)*ss,((float)p.ObjectY-my)*ss,((float)p.TargetX-mu)*st,((float)p.TargetY-mv)*st)).ToArray();
  var weights=Enumerable.Repeat(1d,points.Length).ToArray();var h=FitLinear(normalized,freedom,weights,true,false);if(h is null)return null;
  h=h.Select(v=>(double)(float)v).ToArray();
  // Original 7fcf60 publishes a denormalized float matrix; 7fd4e0 then
  // normalizes that rounded matrix again before its first residual pass.
  // Omitting this roundtrip changes the weights even when the initial fit matches.
  double[] sourceNormalize=[ss,0,-ss*mx,0,ss,-ss*my,0,0,1],targetNormalize=[st,0,-st*mu,0,st,-st*mv,0,0,1];
  var sourceInverse=TransformInverse(sourceNormalize);var targetInverse=TransformInverse(targetNormalize);if(sourceInverse is null||targetInverse is null)return null;
  var denormalized=NormalizeFloat(Multiply(Multiply(targetInverse,h),sourceNormalize));
  h=NormalizeFloat(Multiply(Multiply(targetNormalize,denormalized),sourceInverse));
  if(h[8]==0)return h;
  // 7fd4e0 passes the eight free coefficients to 7fec40, whose ninth slot is 1.
  h[8]=1;
  // The source's four-point perspective path forces the unweighted fit.
  if(points.Length==4&&freedom==1)weight=1;
  const float epsilon=1.1920928955078125e-7f;
  for(int iteration=0;iteration<50;iteration++){
   var inverse=CameraMappingLinearAlgebra.Inverse(h);if(inverse is null)return null;var errors=normalized.Select(p=>{float u=(float)p.TargetX,v=(float)p.TargetY;float z=(float)inverse[6]*u+(float)inverse[7]*v+(float)inverse[8];float dx=((float)inverse[0]*u+(float)inverse[1]*v+(float)inverse[2])/z-(float)p.ObjectX,dy=((float)inverse[3]*u+(float)inverse[4]*v+(float)inverse[5])/z-(float)p.ObjectY;return MathF.Sqrt(dx*dx+dy*dy);}).ToArray();
   var sorted=errors.Order().ToArray();float cutoff=factor*.1f*(sorted[sorted.Length/2]*1.4825797080993652f);
   for(int i=0;i<weights.Length;i++){float error=errors[i];if(weight==1||weight>3){weights[i]=1;}else if(weight==2){error+=epsilon;weights[i]=error>cutoff?cutoff/error:1;}else{float value=1-error*error/(cutoff*cutoff+epsilon);weights[i]=error>cutoff?epsilon:value*value;}}
   var next=FitLinear(normalized,freedom,weights,true,true);if(next is null)break;next=next.Select(v=>(double)(float)v).ToArray();
   int[] free=freedom==1?[0,1,2,3,4,5,6,7]:freedom==2?[0,1,2,3,4,5]:[0,1,2,5];float square=0;for(int i=0;i<free.Length;i+=2){float a=(float)h[free[i]]-(float)next[free[i]],b=(float)h[free[i+1]]-(float)next[free[i+1]];square+=a*a+b*b;}h=next;if(MathF.Sqrt(square)<1e-4f)break;
  }
  return NormalizeFloat(Multiply(Multiply(targetInverse,h),sourceNormalize));
  static double[] NormalizeFloat(double[] matrix){
   // Original constants at 18162b698/69c and DIVSS followed by MULPS/MULSS.
   if(MathF.Abs((float)matrix[8])<1.1920928955078125e-7f)return new double[9];
   float reciprocal=1f/(float)matrix[8];return matrix.Select(v=>(double)((float)v*reciprocal)).ToArray();
  }
  static double[]? TransformInverse(double[] matrix){
   // Both normalization matrices have diagonal scale and a fixed bottom row.
   // The original explicit inverse rejects determinants below double epsilon.
   float determinant=(float)matrix[0]*(float)matrix[4]*(float)matrix[8];
   return Math.Abs((double)determinant)<2.220446049250313e-16?null:CameraMappingLinearAlgebra.Inverse(matrix);
  }
 }
 static double[]? Inverse(double[] h){double d=h[0]*(h[4]*h[8]-h[5]*h[7])-h[1]*(h[3]*h[8]-h[5]*h[6])+h[2]*(h[3]*h[7]-h[4]*h[6]);if(Math.Abs(d)<1e-15)return null;return new[]{h[4]*h[8]-h[5]*h[7],h[2]*h[7]-h[1]*h[8],h[1]*h[5]-h[2]*h[4],h[5]*h[6]-h[3]*h[8],h[0]*h[8]-h[2]*h[6],h[2]*h[3]-h[0]*h[5],h[3]*h[7]-h[4]*h[6],h[1]*h[6]-h[0]*h[7],h[0]*h[4]-h[1]*h[3]}.Select(v=>(double)(float)(v/d)).ToArray();}
 static double[] Multiply(double[] a,double[] b)=>Enumerable.Range(0,9).Select(i=>{float sum=0;for(int k=0;k<3;k++)sum+=(float)a[i/3*3+k]*(float)b[k*3+i%3];return(double)sum;}).ToArray();
 static double[]? FitLinear(CameraMappingPair[] points,int freedom,double[] weights,bool nativeNormal=false,bool nested=true) {
  var rows=new List<double[]>();var targets=new List<double>();var rowWeights=new List<double>();int n=freedom==1?8:freedom==2?6:4;var normal=new double[n,n];var values=new double[n];
  for(int i=0;i<points.Length;i++){
   var p=points[i];double x=p.ObjectX,y=p.ObjectY,u=p.TargetX,v=p.TargetY;
   double[] a=freedom==1?[x,y,1,0,0,0,-u*x,-u*y]:freedom==2?[x,y,1,0,0,0]:[x,-y,1,0];
   double[] b=freedom==1?[0,0,0,x,y,1,-v*x,-v*y]:freedom==2?[0,0,0,x,y,1]:[y,x,0,1];
   Add(a,u,weights[i]);Add(b,v,weights[i]);
  }
  void Add(double[] row,double target,double w){rows.Add(row);targets.Add(target);rowWeights.Add(w);for(int i=0;i<n;i++){values[i]+=w*row[i]*target;for(int j=0;j<n;j++)normal[i,j]+=w*row[i]*row[j];}}
  if(nativeNormal){var result=CameraMappingLinearAlgebra.Solve(rows,targets,rowWeights,nested);return result is null?null:freedom==1?[..result,1]:freedom==2?[..result,0,0,1]:[result[0],-result[1],result[2],result[1],result[0],result[3],0,0,1];}
  for(int k=0;k<n;k++){
   int pivot=k;for(int i=k+1;i<n;i++)if(Math.Abs(normal[i,k])>Math.Abs(normal[pivot,k]))pivot=i;
   if(Math.Abs(normal[pivot,k])<1e-12)return null;
   if(pivot!=k){for(int j=k;j<n;j++)(normal[k,j],normal[pivot,j])=(normal[pivot,j],normal[k,j]);(values[k],values[pivot])=(values[pivot],values[k]);}
   double scale=normal[k,k];for(int j=k;j<n;j++)normal[k,j]/=scale;values[k]/=scale;
   for(int i=0;i<n;i++)if(i!=k){double f=normal[i,k];for(int j=k;j<n;j++)normal[i,j]-=f*normal[k,j];values[i]-=f*values[k];}
  }
  return freedom==1?[..values,1]:freedom==2?[..values,0,0,1]:[values[0],-values[1],values[2],values[1],values[0],values[3],0,0,1];
 }
}
