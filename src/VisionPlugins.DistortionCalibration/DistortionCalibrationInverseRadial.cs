using VisionCore;
namespace VisionPlugins.DistortionCalibration;

/// <summary>Two-variable inverse radial iteration recovered from 180857a60/180854e90.
/// This does not implement the separate fourteen-variable calibration fit.</summary>
public static class DistortionCalibrationInverseRadial {
 public static Point2 Solve(Point2 observed,Point2 center,IReadOnlyList<float> radial){
  if(radial.Count!=4)throw new ArgumentException("Four radial coefficients required.",nameof(radial));
  double x=(float)observed.X,y=(float)observed.Y,ox=x,oy=y,cx=(float)center.X,cy=(float)center.Y,kx=radial[0],ky=radial[1],aspect=radial[2],second=radial[3],mu=0,nu=2;
  // Actual 854e90 param9=1 arithmetic: cache kx*k2 / k2*ky before
  // multiplying offsets, and retain the source Jacobian term association.
  (double Ex,double Ey,double A,double B,double C,double D) Evaluate(double qx,double qy){
   double dx=qx-cx,dy=qy-cy,r=dx*dx+dy*dy*aspect,rx=dx+dx,ry=(dy+dy)*aspect;
   double kx2=kx*second,ky2=second*ky,dx2=dx*kx2,dy2=dy*ky2,rrx=(r+r)*rx,rry=(r+r)*ry;
   return(ox-(qx+kx*dx*r+kx2*dx*r*r),oy-(qy+ky*dy*r+ky2*dy*r*r),
    (kx*r+1)+rx*kx*dx+kx2*r*r+dx2*rrx,
    kx*dx*ry+dx2*rry,
    rx*ky*dy+rrx*dy2,
    (r*ky+1)+ry*ky*dy+r*r*ky2+rry*dy2);
  }
  var v=Evaluate(x,y);double error=Math.Sqrt(v.Ex*v.Ex+v.Ey*v.Ey);
  for(int iteration=0;iteration<200;iteration++){
   double a=v.A*v.A+v.C*v.C,b=v.A*v.B+v.C*v.D,d=v.B*v.B+v.D*v.D,gx=v.A*v.Ex+v.C*v.Ey,gy=v.B*v.Ex+v.D*v.Ey;if(iteration==0){mu=double.MinValue;foreach(var entry in new[]{a,b,b,d})if(!(entry<=mu))mu=entry;mu*=.001;}
   // 854e90 param9=1 uses a full 2x2 double normal matrix (stride2), then inverse*gradient.
   var delta=DistortionCalibrationRadialLinearAlgebra.Solve(new double[,]{{a+mu,b},{b,d+mu}},new[]{gx,gy});
   // 857a60 writes the initial observed point when the LM call returns0, even if earlier iterations advanced.
   if(delta is null)return new((float)ox,(float)oy);
   double sx=delta[0],sy=delta[1];
   if(Math.Max(Math.Abs(sx)/Math.Max(Math.Abs(x),2.220446049250313e-16),Math.Abs(sy)/Math.Max(Math.Abs(y),2.220446049250313e-16))<=1e-10||Math.Max(Math.Abs(sx),Math.Abs(sy))<=1e-30)break;
   var candidate=Evaluate(x+sx,y+sy);double next=Math.Sqrt(candidate.Ex*candidate.Ex+candidate.Ey*candidate.Ey),predicted=sx*(mu*sx+gx)+sy*(mu*sy+gy);if(predicted<=1e-30)break;double rho=(error*error-next*next)/predicted;
   if(rho>0){x+=sx;y+=sy;v=candidate;error=next;if(error*error<=1e-30)break;double factor=2*rho-1;mu*=Math.Max(1d/3,1-factor*factor*factor);nu=2;}else{mu*=nu;nu+=nu;}
  }
  return new((float)x,(float)y);
 }
}




