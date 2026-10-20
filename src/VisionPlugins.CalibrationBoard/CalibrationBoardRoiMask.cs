using VisionCore;
namespace VisionPlugins.CalibrationBoard;

public sealed record CalibrationBoardMask(int ImageWidth,int ImageHeight,byte[] Pixels,int Left,int Top,int Width,int Height,IReadOnlyList<(float X,float Y)> Vertices);

/// <summary>Board wrapper for the shared source-derived PreproMask 35a0/3240/3100/3860 rectangle rasterizer. Native fixtures remain tool evidence.</summary>
public static class CalibrationBoardRoiMask
{
 public static CalibrationBoardMask Rectangle(int imageWidth,int imageHeight,float cx,float cy,float width,float height,float angle)
 {
  var mask=RectangleRoiMaskRasterizer.Create(imageWidth,imageHeight,cx,cy,width,height,angle);
  return new(mask.ImageWidth,mask.ImageHeight,mask.Pixels,mask.Left,mask.Top,mask.Width,mask.Height,mask.Vertices);
 }
}
