namespace VisionPlugins.Calibration;
// 70d2e0 current model1/strategy2/maxiter20/trim0/cutoff5/nominal30/tolerance20 only.
internal static class NImageSourceCircleFit {
 internal readonly record struct Model(float X,float Y,float Radius);
 internal sealed record Result(int Status,Model Circle,float Rms,float[] Residuals,byte[] Mask);
 const double Eps=2.220446049250313e-16;
 sealed class Moments {
  internal double N,X,Y,XX,YY,XY,XXX,YYY,XYY,YXX;
  internal void Add(float px,float py,double weight){
   double x=px,y=py,xx=x*x,yy=y*y;
   if(weight==1){N+=1;X+=x;Y+=y;XX+=xx;YY+=yy;XY+=x*y;XXX+=xx*x;YYY+=yy*y;XYY+=x*yy;YXX+=y*xx;}
   else{double xw=x*weight;N+=weight;X+=xw;Y+=y*weight;XX+=xx*weight;YY+=yy*weight;XY+=y*xw;XXX+=xx*x*weight;YYY+=weight*yy*y;XYY+=yy*xw;YXX+=y*xx*weight;}
  }
  internal Model? Solve(){
   if(N<Eps)return null;double a=N*XX-X*X,b=N*YY-Y*Y,c=N*XY-X*Y;
   double dx=(XXX+XYY)*N-X*(XX+YY),dy=(YYY+YXX)*N-(XX+YY)*Y,det=a*b-c*c;
   if(Math.Abs(det)<Eps)return null;double u=(b*dx-c*dy)*(1d/det),v=(a*dy-c*dx)*(1d/det);
   double q=(u*u+v*v)-((((X*u+Y*v)-XX)-YY)/N)*4d;if(q<Eps)return null;
   return new((float)(u*.5d),(float)(v*.5d),(float)(Math.Sqrt(q)*.5d));
  }
 }
 static float Residual(NImageSourceTagSampler.F p,Model m){float dx=p.X-m.X,dy=p.Y-m.Y;return MathF.Sqrt(dx*dx+dy*dy)-m.Radius;}
 internal static Result Fit(IReadOnlyList<NImageSourceTagSampler.F> points){
  int n=points.Count;var residuals=new float[n];var mask=new byte[n];var initialMoments=new Moments();foreach(var p in points)initialMoments.Add(p.X,p.Y,1);
  var initial=n>=3?initialMoments.Solve():null;if(initial is null)return new(0,new(0,0,0),0,residuals,mask);
  Model current=initial.Value;bool valid=true;float previousRms=float.MaxValue;
  for(int iteration=0;iteration<20;iteration++){
   for(int i=0;i<n;i++)residuals[i]=MathF.Abs(Residual(points[i],current));
   var sorted=residuals.Order().ToArray();float median=(n&1)!=0?sorted[n/2]:(sorted[n/2-1]+sorted[n/2])*.5f;
   double scale=n>3?(double)median*2.96516+1.1920928955078125e-7:double.MaxValue;
   var moments=new Moments();int accepted=0;double squared=0;
   for(int i=0;i<n;i++){
    float r=residuals[i];if(5f<=r){mask[i]=0;continue;}mask[i]=1;accepted++;squared+=(double)(r*r);
    double weight=scale<=r?scale/(double)(r+1.1754943508222875e-38f):1d;moments.Add(points[i].X,points[i].Y,weight);
   }
   var next=accepted>=3?moments.Solve():null;valid=next is not null;if(valid)current=next!.Value;
   float rms=accepted>0?(float)Math.Sqrt(squared/accepted):0;
   if(!valid||rms<9.999999747378752e-6f||previousRms*9.999999747378752e-6f>MathF.Abs(previousRms-rms))break;
   previousRms=rms;
  }
  // Failed refinement restores INITIAL model, not last successful model; mask stays last iteration.
  Model output=valid?current:initial.Value;int status=valid?1:2,count=0;double error=0;
  for(int i=0;i<n;i++){residuals[i]=Residual(points[i],output);if(mask[i]==1){count++;error+=(double)(residuals[i]*residuals[i]);}}
  if(count<3)return new(0,output,0,residuals,mask);float finalRms=(float)Math.Sqrt(error/count);
  if(output.Radius<10f||50f<output.Radius)status=2;return new(status,output,finalRms,residuals,mask);
 }
}
