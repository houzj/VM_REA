namespace VisionPlugins.Calibration;

/// <summary>Independent MatrixFix reconstruction, checked against five controlled original SDK cases.
/// Only numerator translation coefficients change; perspective/normalization terms remain untouched.</summary>
public static class NImageMatrixFix{
 public static IReadOnlyList<double> Apply(IReadOnlyList<double> matrix,double imageX,double imageY,double worldX,double worldY){
  ArgumentNullException.ThrowIfNull(matrix);if(matrix.Count!=9)throw new ArgumentException("A nine-component homography is required.",nameof(matrix));
  if(matrix.Concat(new[]{imageX,imageY,worldX,worldY}).Any(v=>!double.IsFinite(v)||!float.IsFinite((float)v)))throw new ArgumentException("MatrixFix input must be representable as a finite native float.");
  var h=matrix.Select(v=>(float)v).ToArray();float x=(float)imageX,y=(float)imageY,wx=(float)worldX,wy=(float)worldY;
  float denominator=h[6]*x+h[7]*y+h[8];h[2]=wx*denominator-h[0]*x-h[1]*y;h[5]=wy*denominator-h[3]*x-h[4]*y;
  return Array.AsReadOnly(h.Select(v=>(double)v).ToArray());
 }
}
