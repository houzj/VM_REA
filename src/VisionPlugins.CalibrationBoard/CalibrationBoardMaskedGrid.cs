using VisionPlugins.Calibration;

namespace VisionPlugins.CalibrationBoard;

/// <summary>
/// Source 80f9a0/80e690/80cfc0/80bcd0 pre-decode mask projection.
/// Input points belong to the unmodified image cropped at the mask bbox. This
/// preserves the recognizer's indices and order; it does not decode identities.
/// </summary>
public static class CalibrationBoardMaskedGrid
{
 public static IReadOnlyList<NImageLatticePoint> Project(
  IReadOnlyList<NImageLatticePoint> localPoints,CalibrationBoardMask mask)
 {
  ArgumentNullException.ThrowIfNull(localPoints);ArgumentNullException.ThrowIfNull(mask);
  var result=new List<NImageLatticePoint>();
  foreach(var point in localPoints)
  {
   // The native input is a successful 873de0 grid: finite local pixel coordinates.
   float x=(float)point.X,y=(float)point.Y;
   if(!float.IsFinite(x)||!float.IsFinite(y))throw new ArgumentException("网格点必须是有限局部像素坐标。",nameof(localPoints));
   int ix=(int)(x+.5f),iy=(int)(y+.5f);
   if(ix<0||iy<0||ix>=mask.Width||iy>=mask.Height)continue;
   if(mask.Pixels[(mask.Top+iy)*mask.ImageWidth+mask.Left+ix]==0)continue;
   result.Add(point with{X=(float)(x+mask.Left),Y=(float)(y+mask.Top)});
  }
  return result.AsReadOnly();
 }
}
