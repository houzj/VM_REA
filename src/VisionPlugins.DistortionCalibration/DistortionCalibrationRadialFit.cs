using VisionCore;
namespace VisionPlugins.DistortionCalibration;

/// <summary>Independent fourteen-variable radial refinement from 180853820/180854e90.
/// Callers supply the original normalized-fit seed; native bit parity is not assumed.</summary>
public static class DistortionCalibrationRadialFit {
 public sealed record Result(float[] Radial,Point2 Center,double[] Parameters,double Rms,int AcceptedStages);
 public static Result FromSeed(IReadOnlyList<Point2> grid,IReadOnlyList<Point2> observed,IReadOnlyList<double> homography,int width,int height,int freedom,CancellationToken token=default){
  if(grid.Count!=observed.Count||grid.Count<4||homography.Count!=9)throw new ArgumentException("Invalid calibration pairs or seed.");
  var p=homography.Take(8).Select(x=>(double)(float)x).Concat(new[]{0d,0d,1d,0d,width*.5,height*.5}).ToArray();double best=Error(p);int stages=0;
  for(int stage=0;stage<3;stage++){var active=Enumerable.Repeat(true,14).ToArray();if(freedom!=1)active[6]=active[7]=false;active[10]=stage>0;active[11]=stage>1;active[12]=active[13]=stage>0;var candidate=Optimize(p,active)??throw new DistortionCalibrationRadialFitFailure();double rms=Error(candidate);if(rms<best*.8999999761581421){p=candidate;best=rms;stages++;}}
  return new([(float)p[8],(float)p[9],(float)p[10],(float)p[11]],new((float)p[12],(float)p[13]),p,best,stages);
  double SquaredError(double[] value){double sum=0;for(int i=0;i<grid.Count;i++){var q=Evaluate(value,grid[i],null,null);double dx=(float)observed[i].X-q.X,dy=(float)observed[i].Y-q.Y;sum=(sum+dx*dx)+dy*dy;}return sum;}
  double Error(double[] value)=>Math.Sqrt(SquaredError(value)/grid.Count);
  double[]? Optimize(double[] initial,bool[] active){var value=initial.ToArray();var indices=Enumerable.Range(0,14).ToArray();int n=indices.Length;double mu=0,nu=2;double error=Math.Sqrt(SquaredError(value));
   for(int iteration=0;iteration<200;iteration++){token.ThrowIfCancellationRequested();var normal=new double[n,n];var gradient=new double[n];var jx=new double[14];var jy=new double[14];for(int k=0;k<grid.Count;k++){var q=Evaluate(value,grid[k],jx,jy);for(int axis=0;axis<14;axis++)if(!active[axis])jx[axis]=jy[axis]=0;double ex=(float)observed[k].X-q.X,ey=(float)observed[k].Y-q.Y;for(int i=0;i<n;i++){int ii=indices[i];gradient[i]=(gradient[i]+jx[ii]*ex)+jy[ii]*ey;for(int j=0;j<n;j++){int jj=indices[j];normal[i,j]=(normal[i,j]+jx[ii]*jx[jj])+jy[ii]*jy[jj];}}}
    if(iteration==0){mu=double.MinValue;for(int i=0;i<n;i++)for(int j=0;j<n;j++)if(!(normal[i,j]<=mu))mu=normal[i,j];mu*=.001;}for(int i=0;i<n;i++)normal[i,i]+=mu;var delta=DistortionCalibrationRadialLinearAlgebra.Solve(normal,gradient);if(delta is null)return null;double relative=0,absolute=0;for(int i=0;i<n;i++){absolute=Math.Max(absolute,Math.Abs(delta[i]));relative=Math.Max(relative,Math.Abs(delta[i])/Math.Max(Math.Abs(value[indices[i]]),2.220446049250313e-16));}if(relative<=1e-10||absolute<=1e-30)break;
    var candidate=value.ToArray();double predicted=0;for(int i=0;i<n;i++){candidate[indices[i]]+=delta[i];predicted+=delta[i]*(mu*delta[i]+gradient[i]);}if(predicted<=1e-30)break;double next=Math.Sqrt(SquaredError(candidate));double rho=(error*error-next*next)/predicted;if(rho>0){value=candidate;error=next;if(error*error<=1e-30)break;double factor=2*rho-1;mu*=Math.Max(1d/3,1-factor*factor*factor);nu=2;}else{mu*=nu;nu+=nu;}}
   return value;
  }
 }
 static Point2 Evaluate(double[] p,Point2 grid,double[]? jx,double[]? jy){double u=(float)grid.X,v=(float)grid.Y,den=p[6]*u+p[7]*v+1,qx=(p[0]*u+p[1]*v+p[2])/den,qy=(p[3]*u+p[4]*v+p[5])/den,dx=qx-p[12],dy=qy-p[13],r=dx*dx+dy*dy*p[10],r2=r*r,sx=p[8]*r+p[8]*p[11]*r2,sy=p[9]*r+p[9]*p[11]*r2,fx=p[8]+2*p[8]*p[11]*r,fy=p[9]+2*p[9]*p[11]*r;
  if(jx is not null&&jy is not null){double a=1+sx+dx*fx*2*dx,b=dx*fx*2*p[10]*dy,c=dy*fy*2*dx,d=1+sy+dy*fy*2*p[10]*dy;Array.Clear(jx);Array.Clear(jy);double[] hx=[u/den,v/den,1/den,0,0,0,-qx*u/den,-qx*v/den],hy=[0,0,0,u/den,v/den,1/den,-qy*u/den,-qy*v/den];for(int i=0;i<8;i++){jx[i]=a*hx[i]+b*hy[i];jy[i]=c*hx[i]+d*hy[i];}jx[8]=dx*(r+p[11]*r2);jy[9]=dy*(r+p[11]*r2);jx[10]=dx*fx*dy*dy;jy[10]=dy*fy*dy*dy;jx[11]=p[8]*dx*r2;jy[11]=p[9]*dy*r2;jx[12]=-sx-dx*fx*2*dx;jy[12]=-dy*fy*2*dx;jx[13]=-dx*fx*2*p[10]*dy;jy[13]=-sy-dy*fy*2*p[10]*dy;}
  return new(qx+p[8]*dx*r+dx*(p[8]*p[11])*r*r,qy+p[9]*dy*r+dy*(p[11]*p[9])*r*r);
 }
}



public sealed class DistortionCalibrationRadialFitFailure():InvalidOperationException("Original radial optimization failed (0x1010300b)."){
 public uint NativeErrorCode=>0x1010300b;
}

