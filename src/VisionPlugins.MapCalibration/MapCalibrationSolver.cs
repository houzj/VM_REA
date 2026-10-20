namespace VisionPlugins.MapCalibration;

public sealed record MapCalibrationSolveResult(uint ErrorCode,float[] Matrix,float XRatio,float YRatio);

/// <summary>Reconstructed file mapping: native 0x1807d4f20 and 0x180730be0.</summary>
public static class MapCalibrationSolver
{
 public static MapCalibrationSolveResult SolveFiles(IReadOnlyList<float> source,IReadOnlyList<float> target)=>Compose(source,target,true);
 // Image mode consumes the actual outputs of two fixed 7ff3c0 fits. These are not file-mode matrices.
 public static MapCalibrationSolveResult ComposeImageFits(IReadOnlyList<float> source,IReadOnlyList<float> target)=>Compose(source,target,false);
 public static float ImagePixelScale(IReadOnlyList<float> fitted){
  float k=1f/(fitted[8]+1.1920928955078125e-7f);float a=fitted[0]*k,b=fitted[1]*k,c=fitted[3]*k,d=fitted[4]*k;
  return (MathF.Sqrt(a*a+b*b)+MathF.Sqrt(c*c+d*d))*.5f;
 }
 static MapCalibrationSolveResult Compose(IReadOnlyList<float> source,IReadOnlyList<float> target,bool fileMode){
  ArgumentNullException.ThrowIfNull(source);ArgumentNullException.ThrowIfNull(target);if(source.Count!=9||target.Count!=9)throw new ArgumentException("Calibration matrices contain nine floats.");
  var b=target;float p=b[0]*b[4],q=b[1]*b[5];float determinant=(((b[8]*p+b[6]*q+b[7]*b[2]*b[3])-b[7]*b[0]*b[5])-b[8]*b[1]*b[3])-b[6]*b[4]*b[2];
  // Original PE 0x181628540 = double epsilon; 0x18162854c = float epsilon.
  if(!(Math.Abs(determinant)>=2.220446049250313e-16))return new(0x10103002,[],0,0);
  float k=1/determinant;float[] inverse=[(b[4]*b[8]-b[5]*b[7])*k,(b[2]*b[7]-b[8]*b[1])*k,(q-b[4]*b[2])*k,(b[5]*b[6]-b[8]*b[3])*k,(b[8]*b[0]-b[2]*b[6])*k,(b[2]*b[3]-b[0]*b[5])*k,(b[3]*b[7]-b[4]*b[6])*k,(b[1]*b[6]-b[0]*b[7])*k,(p-b[1]*b[3])*k];
  var h=new float[9];for(int row=0;row<3;row++)for(int col=0;col<3;col++){float sum=0;sum=sum+inverse[row*3]*source[col]+inverse[row*3+1]*source[3+col];sum+=inverse[row*3+2]*source[6+col];h[row*3+col]=sum;}
  float denominator=fileMode?h[8]+1.1920928955078125e-7f:h[8];if(!fileMode&&!(MathF.Abs(denominator)>=1.1920928955078125e-7f))return new(0x10103002,[],0,0);for(int i=0;i<9;i++)h[i]/=denominator;
  return new(0,h,MathF.Sqrt(h[0]*h[0]+h[3]*h[3]),MathF.Sqrt(h[1]*h[1]+h[4]*h[4]));
 }
}
