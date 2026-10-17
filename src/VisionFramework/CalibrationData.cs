namespace VisionFramework;

/// <summary>Owned image-to-world projective matrix. The type does not imply native SOL compatibility.</summary>
public sealed class CalibrationData {
 public IReadOnlyList<double> ImageToWorld {get;}
 public string Source {get;}
 public CalibrationData(IEnumerable<double> imageToWorld,string source){var matrix=imageToWorld.ToArray();if(matrix.Length!=9||matrix.Any(v=>!double.IsFinite(v)))throw new ArgumentException("A finite 3x3 matrix is required.",nameof(imageToWorld));ImageToWorld=Array.AsReadOnly(matrix);Source=source??throw new ArgumentNullException(nameof(source));}
 public (double X,double Y) Transform(double x,double y){if(!double.IsFinite(x)||!double.IsFinite(y))throw new ArgumentOutOfRangeException(nameof(x));var h=ImageToWorld;double w=h[6]*x+h[7]*y+h[8];if(w==0)throw new InvalidOperationException("Point maps to infinity.");double tx=(h[0]*x+h[1]*y+h[2])/w,ty=(h[3]*x+h[4]*y+h[5])/w;if(!double.IsFinite(tx)||!double.IsFinite(ty))throw new InvalidOperationException("Non-finite transformed point.");return(tx,ty);}
}
