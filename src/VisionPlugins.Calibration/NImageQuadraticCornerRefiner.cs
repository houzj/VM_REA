using VisionCore;
namespace VisionPlugins.Calibration;

/// <summary>Original 8b6f30 quadratic saddle refinement. Source arithmetic and rejection order are preserved.</summary>
public static class NImageQuadraticCornerRefiner {
 static readonly (int x,int y)[] Offsets=(from y in Enumerable.Range(-4,9) from x in Enumerable.Range(-4,9) where x*x+y*y<=20 select (x,y)).ToArray();
 static readonly float[] Basis=CreateBasis();
 static float[] CreateBasis(){var offsets=Offsets;int samples=offsets.Length;
var design=new float[samples*6];for(int i=0;i<samples;i++){var(x,y)=offsets[i];float[] row=[x*x,y*y,x*y,x,y,1];row.CopyTo(design,i*6);}var transpose=new float[6*samples];for(int i=0;i<samples;i++)for(int j=0;j<6;j++)transpose[j*samples+i]=design[i*6+j];return Multiply(Inverse(Multiply(transpose,6,samples,design,6),6),6,6,transpose,samples);
}
static float[] Multiply(float[] a,int ar,int ac,float[] b,int bc){var c=new float[ar*bc];for(int i=0;i<ar;i++)for(int j=0;j<bc;j++){float sum=0;for(int k=0;k<ac;k++)sum=sum+a[i*ac+k]*b[k*bc+j];c[i*bc+j]=sum;}return c;}
static float[] Inverse(float[] input,int n){var a=(float[])input.Clone();var rows=new int[n];var cols=new int[n];void SwapRow(int x,int y){for(int j=0;j<n;j++)(a[x*n+j],a[y*n+j])=(a[y*n+j],a[x*n+j]);}void SwapCol(int x,int y){for(int i=0;i<n;i++)(a[i*n+x],a[i*n+y])=(a[i*n+y],a[i*n+x]);}for(int k=0;k<n;k++){float largest=0;int row=k,col=k;for(int i=k;i<n;i++)for(int j=k;j<n;j++)if(MathF.Abs(a[i*n+j])>largest){largest=MathF.Abs(a[i*n+j]);row=i;col=j;}rows[k]=row;cols[k]=col;if(largest==0)throw new Exception("Singular basis");if(row!=k)SwapRow(k,row);if(col!=k)SwapCol(k,col);float inv=1f/a[k*n+k];a[k*n+k]=inv;for(int j=0;j<n;j++)if(j!=k)a[k*n+j]=a[k*n+j]*inv;for(int i=0;i<n;i++)if(i!=k)for(int j=0;j<n;j++)if(j!=k)a[i*n+j]=a[i*n+j]-a[i*n+k]*a[k*n+j];for(int i=0;i<n;i++)if(i!=k)a[i*n+k]=-inv*a[i*n+k];}for(int k=n-1;k>=0;k--){if(cols[k]!=k)SwapRow(k,cols[k]);if(rows[k]!=k)SwapCol(k,rows[k]);}return a;}
 public static bool TryRefine(GrayImage image,float initialX,float initialY,int radius,int capacity,int candidateCount,out Point2 refined){
  ArgumentNullException.ThrowIfNull(image);refined=default;if(radius<0||capacity<0||candidateCount<0)throw new ArgumentOutOfRangeException(nameof(radius));if(!float.IsFinite(initialX)||!float.IsFinite(initialY))return false;
  int imageWidth=image.Width,imageHeight=image.Height,samples=Offsets.Length;float x=initialX,y=initialY;
  if(x-radius<0||x+radius>=imageWidth-1||y-radius<0||y+radius>=imageHeight-1)return false;
  float Sample(float px,float py,int ox,int oy){int ix=(int)px,iy=(int)py;float dx=px-ix,dy=py-iy,w11=dx*dy,w10=dx-w11,w01=dy-w11,w00=((1f-dx)-dy)+w11;ix+=ox;iy+=oy;return image[ix,iy]*w00+image[ix+1,iy]*w10+image[ix,iy+1]*w01+image[ix+1,iy+1]*w11;}
  for(int iteration=0;iteration<5;iteration++){
   if(x<4||x+4>=imageWidth-1||y<4||y+4>=imageHeight-1)return false;
   var values=Offsets.Select(p=>Sample(x,y,p.x,p.y)).ToArray();var coefficient=Multiply(Basis,6,samples,values,1);
   float a=coefficient[0],b=coefficient[1],c=coefficient[2],d=coefficient[3],e=coefficient[4];float determinant=4f*a*b-c*c;if(determinant>=0)return false;
   float deltaX=(c*e-b*2f*d)/determinant,deltaY=(c*d-a*2f*e)/determinant;x=x+deltaX;y=y+deltaY;
   float movement=MathF.Sqrt((x-initialX)*(x-initialX)+(y-initialY)*(y-initialY));
   if(movement>20f||(movement>2.5f&&candidateCount>(int)((double)(capacity*.005f)+.5)))return false;
   if(Math.Sqrt((double)(deltaX*deltaX+deltaY*deltaY))<=0.009999999776482582)break;
  }
  refined=new(x,y);return true;
 }
}
