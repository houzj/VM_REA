using VisionCore;
namespace VisionPlugins.Calibration;
// 80a7f0 fixed 200x200 opening producer. REVIEW DRAFT, not in B patch.
internal static class NImageSourceOpening {
 internal sealed record Result(NImageSourceTagSampler.Sample? Sample,bool Refined,string? UnsupportedReason,byte[]? Normalized=null,IReadOnlyList<NImageSourceLocalPrelocator.Result>? Prelocations=null,IReadOnlyList<NImageSourceTagSampler.F>? ActualInner=null,float[]? OuterFit=null,float[]? InverseOuterFit=null);
 internal static Result Decode(GrayImage source,IReadOnlyList<NImageSourceTagSampler.F> proposal,NImageSourceTagSampler.MeanDispatch dispatch){
  if(dispatch==NImageSourceTagSampler.MeanDispatch.Ordinary)return new(null,false,"Ordinary local feature branch not translated");
  NImageSourceTagSampler.F[] canonical=[new(0,0),new(200,0),new(200,200),new(0,200)];
  var outerFit=NPointSolver.FitNativePoints(Enumerable.Range(0,4).Select(i=>(proposal[i].X,proposal[i].Y,canonical[i].X,canonical[i].Y)).ToArray(),1,3,2f,20,1e-4f);
  NImageSourceTagSampler.F[] inner=[];bool refined=false;byte[]? normalized=null;float[]? inverseOuterFit=null;var preliminaries=new List<NImageSourceLocalPrelocator.Result>();
  if(outerFit.ErrorCode==0&&outerFit.Matrix is not null){
   var inverse=Invert(outerFit.Matrix);inverseOuterFit=inverse;if(inverse is not null){
    // 80ac20 has a distinct truncate/float four-term weighted sampling, +.5 conversion.
    var pixels=new byte[40000];normalized=pixels;int rows=Math.Min(source.Height,200),columns=Math.Min(source.Width,200);
    for(int y=0;y<rows;y++)for(int x=0;x<columns;x++){
     // 80b230..80b29c precomputes each row's h1*y+h2 before h0*x.
     // Decompiled left-associative expression hides this observable float order.
     float den=inverse[6]*x+(inverse[7]*y+inverse[8]);
     var p=new NImageSourceTagSampler.F((inverse[0]*x+(inverse[1]*y+inverse[2]))/den,(inverse[3]*x+(inverse[4]*y+inverse[5]))/den);int ix=(int)p.X,iy=(int)p.Y;
     if(ix<0||iy<0||ix>=source.Width||iy>=source.Height)continue;int index=iy*source.Width+ix;
     if(index<source.Width*source.Height-source.Width-1){float fx=p.X-ix,fy=p.Y-iy;
      float value=source[ix,iy]*(1f-fx)*(1f-fy)+source[ix+1,iy]*fx*(1f-fy)+source[ix,iy+1]*(1f-fx)*fy+source[ix+1,iy+1]*fx*fy;
      pixels[y*200+x]=unchecked((byte)(int)(value>=0?value+.5f:value-.5f));
     }else pixels[y*200+x]=source[ix,iy];
    }
    // 80ac20 rectangle packets are (0,0),(100,0),(100,100),(0,100), 100x100.
    (int X,int Y)[] origins=[(0,0),(100,0),(100,100),(0,100)];var found=new List<NImageSourceTagSampler.F>();
    foreach(var origin in origins){var quadrant=new byte[10000];for(int y=0;y<100;y++)Array.Copy(pixels,(origin.Y+y)*200+origin.X,quadrant,y*100,100);
     var image=new GrayImage(100,100,quadrant);var pre=NImageSourceLocalPrelocator.Locate(image);preliminaries.Add(pre);var feature=NImageSourceLocalFeature.Find(image,pre);
     if(feature.UnsupportedReason is not null)return new(null,false,feature.UnsupportedReason,normalized,preliminaries);
     float radius=feature.Fit?.Circle.Radius??0;
     // 80ac20 only checks radius [15,21], not feature status or RMS.
     if(radius<15f||21f<radius)break;
     var center=feature.Fit!.Circle;found.Add(new(origin.X+center.X,origin.Y+center.Y));
    }
    if(found.Count==4){inner=found.ToArray();refined=true;}
   }
  }
  if(refined){for(int i=0;i<4;i++){canonical[i]=new(canonical[i].X/10f,canonical[i].Y/10f);inner[i]=new(inner[i].X/10f,inner[i].Y/10f);}}
  else{canonical=[new(0,0),new(20,0),new(20,20),new(0,20)];inner=[new(4,4),new(16,4),new(12,16),new(4,16)];}
  var fit=NPointSolver.FitNativePoints(Enumerable.Range(0,4).Select(i=>(canonical[i].X,canonical[i].Y,proposal[i].X,proposal[i].Y)).ToArray(),1,3,2f,20,1e-4f);
  if(fit.ErrorCode!=0||fit.Matrix is null)return new(null,refined,null,normalized,preliminaries);
  var actual=inner.Select(p=>NImageSourceTagSampler.Project(fit.Matrix,p.X,p.Y)).ToArray();
  return new(NImageSourceTagSampler.Decode(source,actual,dispatch),refined,null,normalized,preliminaries,actual,outerFit.Matrix,inverseOuterFit);
 }
 // 730e90 operation sequence, same existing NPoint normal/fullpivot reconstruction.
 static float[]? Invert(float[] input){int n=3;var a=new float[3,3];for(int i=0;i<9;i++)a[i/3,i%3]=input[i];var rows=new int[n];var cols=new int[n];
  for(int k=0;k<n;k++){float maximum=0;int p=k,q=k;for(int i=k;i<n;i++)for(int j=k;j<n;j++)if(MathF.Abs(a[i,j])>maximum){maximum=MathF.Abs(a[i,j]);p=i;q=j;}if(maximum==0)return null;rows[k]=p;cols[k]=q;
   if(p!=k)for(int j=0;j<n;j++)(a[k,j],a[p,j])=(a[p,j],a[k,j]);if(q!=k)for(int i=0;i<n;i++)(a[i,k],a[i,q])=(a[i,q],a[i,k]);float r=1f/a[k,k];
   for(int j=0;j<n;j++)if(j!=k)a[k,j]*=r;for(int i=0;i<n;i++)if(i!=k)for(int j=0;j<n;j++)if(j!=k)a[i,j]-=a[i,k]*a[k,j];for(int i=0;i<n;i++)if(i!=k)a[i,k]*=-r;a[k,k]=r;}
  for(int k=n-1;k>=0;k--){if(cols[k]!=k)for(int j=0;j<n;j++)(a[k,j],a[cols[k],j])=(a[cols[k],j],a[k,j]);if(rows[k]!=k)for(int i=0;i<n;i++)(a[i,k],a[i,rows[k]])=(a[i,rows[k]],a[i,k]);}
  var output=new float[9];for(int i=0;i<9;i++)output[i]=a[i/3,i%3];return output;
 }
}


