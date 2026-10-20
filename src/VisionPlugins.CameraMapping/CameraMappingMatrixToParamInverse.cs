namespace VisionPlugins.CameraMapping;

// Original MatrixToParam core 0x800af0. Keep this separate from the solver's
// inverse: the original code hoists these six products before its determinant.
internal static class CameraMappingMatrixToParamInverse {
 internal static double[]? Inverse(double[] matrix) {
  var m=matrix.Select(v=>(float)v).ToArray();
  float p04=m[0]*m[4],p15=m[1]*m[5],p05=m[0]*m[5],p13=m[1]*m[3],p23=m[2]*m[3],p42=m[4]*m[2];
  float determinant=(((m[8]*p04+m[6]*p15+m[7]*p23)-m[7]*p05)-m[8]*p13)-m[6]*p42;
  // Original PE constants 18162b7c0 (double epsilon) and 18162b804 (float epsilon).
  if(Math.Abs((double)determinant)<2.220446049250313e-16)return null;
  float reciprocal=1f/determinant;
  float i8=(p04-p13)*reciprocal;
  if(MathF.Abs(i8)<1.1920928955078125e-7f)return null;
  return [(m[4]*m[8]-m[5]*m[7])*reciprocal,(m[2]*m[7]-m[8]*m[1])*reciprocal,(p15-p42)*reciprocal,
   (m[5]*m[6]-m[8]*m[3])*reciprocal,(m[8]*m[0]-m[2]*m[6])*reciprocal,(p23-p05)*reciprocal,
   (m[3]*m[7]-m[4]*m[6])*reciprocal,(m[1]*m[6]-m[0]*m[7])*reciprocal,i8];
 }
}
