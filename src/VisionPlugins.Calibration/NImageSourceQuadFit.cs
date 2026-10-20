namespace VisionPlugins.Calibration;
// 89faa0 fixed count4/model1 -> 8a0650 -> 8580e0 -> 7075e0.
// Uses the same float normal/full-pivot operation order already reconstructed in NPointSolver,
// without its IRLS, alternative 7ff3c0 normalization or double homography fallback.
internal static class NImageSourceQuadFit {
 internal static float[]? Fit(IReadOnlyList<NImageSourceTagSampler.F> from,IReadOnlyList<NImageSourceTagSampler.F> to){
  if(from.Count!=4||to.Count!=4)return null;
  var (a,ta)=Normalize(from);var (b,tb)=Normalize(to);
  var rows=new float[8,8];var values=new float[8];
  for(int i=0;i<4;i++){float x=a[i].X,y=a[i].Y,u=b[i].X,v=b[i].Y;
   rows[i*2,0]=x;rows[i*2,1]=y;rows[i*2,2]=1;rows[i*2,6]=-u*x;rows[i*2,7]=-u*y;values[i*2]=u;
   rows[i*2+1,3]=x;rows[i*2+1,4]=y;rows[i*2+1,5]=1;rows[i*2+1,6]=-v*x;rows[i*2+1,7]=-v*y;values[i*2+1]=v;
  }
  var h=Normal(rows,values);if(h is null)return null;float[] normalized=[..h,1f];
  // 89faa0 explicitly forms inverse(Tb) via (Tb' Tb)^-1 Tb', not the algebraic shortcut.
  float[] transpose=[tb[0],tb[3],tb[6],tb[1],tb[4],tb[7],tb[2],tb[5],tb[8]];
  var gram=Multiply(transpose,tb);var inverse=InverseGram(gram);if(inverse is null)return null;
  var raw=Multiply(Multiply(Multiply(inverse,transpose),normalized),ta);
  if(MathF.Abs(raw[8])<1.1920928955078125e-7f)return new float[9]; // source success path writes zero matrix
  float reciprocal=1f/raw[8];for(int i=0;i<9;i++)raw[i]*=reciprocal;return raw;
 }
 static (NImageSourceTagSampler.F[],float[]) Normalize(IReadOnlyList<NImageSourceTagSampler.F> p){
  float x=0,y=0;for(int i=0;i<p.Count;i++){x+=p[i].X;y+=p[i].Y;}float inverseCount=1f/p.Count;
  float cx=x*inverseCount,cy=y*inverseCount,sum=0;var centered=new NImageSourceTagSampler.F[p.Count];
  for(int i=0;i<p.Count;i++){float dx=p[i].X-cx,dy=p[i].Y-cy;centered[i]=new(dx,dy);sum+=MathF.Sqrt(dx*dx+dy*dy);}
  float scale=sum>0?1.4142135381698608f/(sum*inverseCount):1f;
  for(int i=0;i<p.Count;i++)centered[i]=new(centered[i].X*scale,centered[i].Y*scale);
  return(centered,[scale,0,-(cx*scale),0,scale,-(cy*scale),0,0,1]);
 }
 static float[] Multiply(float[] a,float[] b){var c=new float[9];for(int i=0;i<3;i++)for(int j=0;j<3;j++){float s=0;for(int k=0;k<3;k++)s+=a[i*3+k]*b[k*3+j];c[i*3+j]=s;}return c;}
 static float[]? InverseGram(float[] m){
  float a=m[0]*m[4],b=m[1]*m[5],c=m[0]*m[5],d=m[4]*m[2];
  float det=(((m[8]*a+m[6]*b+m[7]*m[2]*m[3])-m[7]*c)-m[8]*m[1]*m[3])-m[6]*d;
  if((double)MathF.Abs(det)<2.220446049250313e-16)return null;float r=1f/det;
  return[(m[4]*m[8]-m[5]*m[7])*r,(m[2]*m[7]-m[1]*m[8])*r,(m[1]*m[5]-m[2]*m[4])*r,
   (m[5]*m[6]-m[3]*m[8])*r,(m[0]*m[8]-m[2]*m[6])*r,(m[2]*m[3]-m[0]*m[5])*r,
   (m[3]*m[7]-m[4]*m[6])*r,(m[1]*m[6]-m[0]*m[7])*r,(a-m[1]*m[3])*r];
 }
 // 7075e0 first builds A' A and A' b via 730be0, then 730e90 inverse and multiply.
 static float[]? Normal(float[,] rows,float[] values){
  int n=rows.GetLength(1),count=rows.GetLength(0);var a=new float[n,n];var rhs=new float[n];
  for(int i=0;i<n;i++){for(int r=0;r<count;r++)rhs[i]+=rows[r,i]*values[r];for(int j=0;j<n;j++)for(int r=0;r<count;r++)a[i,j]+=rows[r,i]*rows[r,j];}
  var row=new int[n];var col=new int[n];for(int k=0;k<n;k++){
   float maximum=0;int p=k,q=k;for(int i=k;i<n;i++)for(int j=k;j<n;j++)if(MathF.Abs(a[i,j])>maximum){maximum=MathF.Abs(a[i,j]);p=i;q=j;}
   if(maximum==0)return null;row[k]=p;col[k]=q;
   if(p!=k)for(int j=0;j<n;j++)(a[k,j],a[p,j])=(a[p,j],a[k,j]);
   if(q!=k)for(int i=0;i<n;i++)(a[i,k],a[i,q])=(a[i,q],a[i,k]);
   float inv=1f/a[k,k];for(int j=0;j<n;j++)if(j!=k)a[k,j]*=inv;
   for(int i=0;i<n;i++)if(i!=k)for(int j=0;j<n;j++)if(j!=k)a[i,j]-=a[i,k]*a[k,j];
   for(int i=0;i<n;i++)if(i!=k)a[i,k]*=-inv;a[k,k]=inv;
  }
  for(int k=n-1;k>=0;k--){if(col[k]!=k)for(int j=0;j<n;j++)(a[k,j],a[col[k],j])=(a[col[k],j],a[k,j]);if(row[k]!=k)for(int i=0;i<n;i++)(a[i,k],a[i,row[k]])=(a[i,row[k]],a[i,k]);}
  var result=new float[n];for(int i=0;i<n;i++)for(int j=0;j<n;j++)result[i]+=a[i,j]*rhs[j];return result;
 }
}

