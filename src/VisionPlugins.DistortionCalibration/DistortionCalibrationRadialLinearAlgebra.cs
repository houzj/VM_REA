namespace VisionPlugins.DistortionCalibration;

/// <summary>Double 180731610 full-pivot in-place inverse followed by the 180730d20
/// scalar multiply order. This is not the common single-precision 3x3 inverse.</summary>
internal static class DistortionCalibrationRadialLinearAlgebra {
 internal static double[]? Solve(double[,] input,double[] rhs){
  int n=rhs.Length;var a=(double[,])input.Clone();var rows=new int[n];var columns=new int[n];
  void SwapRows(int x,int y){for(int j=0;j<n;j++)(a[x,j],a[y,j])=(a[y,j],a[x,j]);}
  void SwapColumns(int x,int y){for(int i=0;i<n;i++)(a[i,x],a[i,y])=(a[i,y],a[i,x]);}
  for(int k=0;k<n;k++){
   double largest=0;int row=k,column=k;
   for(int i=k;i<n;i++)for(int j=k;j<n;j++){double value=Math.Abs(a[i,j]);if(value>largest){largest=value;row=i;column=j;}}
   if(largest==0)return null;rows[k]=row;columns[k]=column;
   if(row!=k)SwapRows(k,row);if(column!=k)SwapColumns(k,column);
   if(a[k,k]==0)return null;double reciprocal=1d/a[k,k];
   // Native writes reciprocal into the pivot slot before scaling the row,
   // then restores it; omit that temporary square as the slot is not consumed.
   for(int j=0;j<n;j++)if(j!=k)a[k,j]=a[k,j]*reciprocal;
   a[k,k]=reciprocal;
   for(int i=0;i<n;i++)if(i!=k)for(int j=0;j<n;j++)if(j!=k)a[i,j]=a[i,j]-a[i,k]*a[k,j];
   double negative=-reciprocal;for(int i=0;i<n;i++)if(i!=k)a[i,k]=negative*a[i,k];
  }
  for(int k=n-1;k>=0;k--){if(columns[k]!=k)SwapRows(k,columns[k]);if(rows[k]!=k)SwapColumns(k,rows[k]);}
  var output=new double[n];for(int i=0;i<n;i++){double sum=0;int j=0;for(;j+1<n;j+=2)sum=(sum+a[i,j]*rhs[j])+a[i,j+1]*rhs[j+1];if(j<n)sum+=a[i,j]*rhs[j];output[i]=sum;}return output;
 }
}
